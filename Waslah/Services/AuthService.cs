using Hangfire;
using MimeKit.Utils;
using Waslah.Abstraction.Consts;
using Waslah.Helppers;

namespace Waslah.Services
{
    public class AuthService(
     UserManager<ApplicationUser> userManager,
     IWebHostEnvironment environment,
     IJwtProvider jwtProvider,
     ApplicationDbContext context,
     ICustomEmailService emailService,
     SignInManager<ApplicationUser> signInManager) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IWebHostEnvironment _webEnvironment = environment;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly ApplicationDbContext _context = context;
        private readonly ICustomEmailService _emailService = emailService;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;


        private static readonly int OtpExpiryIn = 15;
        private static readonly int RefreshTokenExpiryInDays = 90;

        public async Task<Result> RegistrationAsync(RegistrationRequest request, CancellationToken cancellationToken = default)
        {
            var EmailExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email,cancellationToken);

            if (EmailExists)
                return Result.Failure(UserErrors.DoublicatedEmail);

            var UsernameExists = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName,cancellationToken);

            if (EmailExists)
                return Result.Failure(UserErrors.DoublicatedUserName);

            var relativePath = string.Empty;

            if (request.ProfilePhoto is not null)
            {
                var uploadsFolder = Path.Combine(_webEnvironment.WebRootPath, "user-photos");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ProfilePhoto.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ProfilePhoto.CopyToAsync(stream, cancellationToken);
                }

                relativePath = $"/user-photos/{uniqueFileName}";
            }



            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                ProfilePhotoPath = relativePath
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {

                var code = RandomNumberGenerator.GetInt32(100000, 999999);

                await _context.OtpEntries.AddAsync(new OtpEntry
                {
                    Code = code,
                    Email = user.Email,
                    ExpiresIn = DateTime.UtcNow.AddMinutes(OtpExpiryIn),
                    Purpose = OtpPurpose.EmailVerification
                },
                cancellationToken);

                await OtpSendConfirmationEmailAsync(user, code);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

        }

        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } User)
                return Result.Failure(UserErrors.InvalidOtpCode);

            if (User!.EmailConfirmed)
                Result.Failure(UserErrors.DoublicatedConfirmation);

            var otp = await _context.OtpEntries
                        .Where(x => x.Email == User.Email && x.Purpose == OtpPurpose.EmailVerification)
                        .FirstOrDefaultAsync();

            if (otp == null || otp.IsExpired)
                return Result.Failure(UserErrors.InvalidOtpCode);

            if (int.Parse(request.Code) != otp.Code)
                return Result.Failure(UserErrors.InvalidOtpCode);

            User.EmailConfirmed = true;
            await _userManager.UpdateAsync(User);
            await _userManager.AddToRoleAsync(User, DefaultRoles.Member);

            return Result.Success();
        }

        public async Task<Result> ReasendEmailConfiramtionCode(ResendConfirmEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DoublicatedConfirmation);


            var NewOtp = RandomNumberGenerator.GetInt32(100000, 999999);

            await _context.OtpEntries
                .Where(x => x.Email == request.Email && x.Purpose == OtpPurpose.EmailVerification)
                .ExecuteUpdateAsync(setter =>
                      setter
                         .SetProperty(x => x.Code, NewOtp)
                         .SetProperty(x => x.ExpiresIn, DateTime.UtcNow.AddMinutes(OtpExpiryIn)),
                         cancellationToken
                         );

            await OtpSendConfirmationEmailAsync(user, NewOtp);

            return Result.Success();
        }

        public async Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {

            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Failure<AuthResponse>(UserErrors.Invalidcredentials);

            if (!user.EmailConfirmed)
                return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

            if (result.Succeeded)
            {
                (var roles, var permissions) = await GetUserRolesAndPermissions(user);

                (string Token, int ExpiresIn) = _jwtProvider.GenerateToken(user, roles, permissions);

                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenExpiryInDays);

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresOn = refreshTokenExpiryDate
                });

                await _userManager.UpdateAsync(user);

                var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email!, user.UserName!
                    , Token, ExpiresIn, refreshToken, refreshTokenExpiryDate);

                return Result.Success(response);

            }

            var error = result.IsLockedOut
                ? UserErrors.LockedOutUser
                : UserErrors.Invalidcredentials;

            return Result.Failure<AuthResponse>(error);
        }

        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest request)
        {
            var result = _jwtProvider.ValidateToken(request.Token);

            if (result.IsFailur)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

            if (await _userManager.FindByIdAsync(result.Value) is not { } user)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure<AuthResponse>(UserErrors.LockedOutUser);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == request.RefreshToken && x.IsActivated);

            if (userRefreshToken == null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            (var Roles, var Permissions) = await GetUserRolesAndPermissions(user);

            var newSecurityStamp = await _userManager.GetSecurityStampAsync(user);

            (var NewToken, var ExpiryIn) = _jwtProvider.GenerateToken(user, Roles, Permissions );

            var NewRefreshToken = GenerateRefreshToken();
            var RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenExpiryInDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = NewRefreshToken,
                ExpiresOn = RefreshTokenExpiryDate,
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email!, user.UserName!
            , NewToken, ExpiryIn, NewRefreshToken, RefreshTokenExpiryDate);

            return Result.Success(response);
        }
        public async Task<Result> RevokeRefreshTokenAsync(RefreshTokenRequest request)
        {
            var result = _jwtProvider.ValidateToken(request.Token);

            if (result.IsFailur)
                return Result.Failure(UserErrors.InvalidJwtToken);

            if (await _userManager.FindByIdAsync(result.Value) is not { } user)
                return Result.Failure(UserErrors.InvalidJwtToken);

            if (user.IsDisabled)
                return Result.Failure(UserErrors.DisabledUser);

            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure(UserErrors.LockedOutUser);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == request.RefreshToken && x.IsActivated);

            if (userRefreshToken == null)
                return Result.Failure(UserErrors.InvalidRefreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return Result.Success();
        }

        public async Task<Result> SendResetPasswordCodeAsync(ForgetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();

            var otpCode = RandomNumberGenerator.GetInt32(100000, 999999);

            var otpEntry = new OtpEntry
            {
                Email = request.Email,
                Code = otpCode,
                ExpiresIn = DateTime.UtcNow.AddMinutes(OtpExpiryIn),
                Purpose = OtpPurpose.PasswordReset
            };

            await _context.AddAsync(otpEntry, cancellationToken);
            await OtpSendResetPasswordEmailAsync(user, otpCode);


            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Failure(UserErrors.InvalidOtpCode);

            var activeOtpCode = await _context.OtpEntries
                            .Where(x =>
                                x.Email == request.Email &&
                                x.Purpose == OtpPurpose.PasswordReset &&
                                x.ExpiresIn > DateTime.UtcNow &&
                                x.Code.ToString() == request.Otp)
                            .FirstOrDefaultAsync(cancellationToken);

            if (activeOtpCode == null)
                return Result.Failure(UserErrors.InvalidOtpCode);

            if (!activeOtpCode.IsActive)
                return Result.Failure(UserErrors.InActiveOtpCode);

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

            activeOtpCode.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }


        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
        private async Task OtpSendConfirmationEmailAsync(ApplicationUser user, int Code)
        {
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/WaslahLogo.jpeg");
            var logoContentId = MimeUtils.GenerateMessageId();

            var EmailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmOtp", new Dictionary<string, string>
            {
                  {"{{Logo}}",$"cid:{logoContentId}" },
                  {"{{name}}",user.FirstName +" "+user.LastName },
                  {"{{otp}}",Code.ToString() }
            }
            );

            BackgroundJob.Enqueue(() =>
             _emailService.SendEmailAsync(user.Email!, "✅ Waslah : email verification", EmailBody, logoPath, logoContentId)
            );

            await Task.CompletedTask;
        }
        private async Task OtpSendResetPasswordEmailAsync(ApplicationUser user, int Code)
        {
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/WaslahLogo.jpeg");
            var logoContentId = MimeUtils.GenerateMessageId();

            var EmailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPasswordOtp", new Dictionary<string, string>
            {
                  {"{{Logo}}",$"cid:{logoContentId}" },
                  {"{{name}}",user.FirstName +" "+user.LastName },
                  {"{{otp}}",Code.ToString() }
            }
            );

            BackgroundJob.Enqueue(() =>
             _emailService.SendEmailAsync(user.Email!, "✅ Waslah : Reset Password", EmailBody, logoPath, logoContentId)
            );

            await Task.CompletedTask;
        }
        private async Task<(IEnumerable<string> Roles, IEnumerable<string> Permissions)> GetUserRolesAndPermissions(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var permissions = await (from r in _context.Roles
                                     join rc in _context.RoleClaims
                                     on r.Id equals rc.RoleId
                                     where roles.Contains(r.Name!)
                                     select rc.ClaimValue)
                                     .Distinct()
                                     .ToListAsync();

            return (roles, permissions);
        }


    }
}
