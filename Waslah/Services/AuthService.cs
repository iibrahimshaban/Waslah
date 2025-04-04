namespace Waslah.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwt) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwt;
        private static readonly int _RefreshTokenExpiryDays = 30;

        public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
        {

            var User = await _userManager.FindByEmailAsync(Email);

            if (User is null)
                return Result.Failure<AuthResponse>(UserErrors.Invalid);

            var PasswordChecked = await _userManager.CheckPasswordAsync(User, Password);

            if (!PasswordChecked)
                return Result.Failure<AuthResponse>(UserErrors.Invalid);

            var (Token, ExpiresIn) = _jwtProvider.GenerateToken(User);

            var RefreshToken = GenerateRefreshToken();
            var RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDays);

            User.RefreshTokens.Add(new RefreshToken
            {
                Token = RefreshToken,
                ExpiresOn = RefreshTokenExpiryDate,
            });

            await _userManager.UpdateAsync(User);

            var response = new AuthResponse(User.Id, User.Email, User.FirstName, User.LastName, Token, ExpiresIn,
                RefreshToken, RefreshTokenExpiryDate);

            return Result.Success(response);

        }
        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string Token, string Refreshtoken,
            CancellationToken cancellationToken = default)
        {
            var UserId = _jwtProvider.ValidateToken(Token);

            if (UserId is null)
                return Result.Failure<AuthResponse>(UserErrors.ExpiredToken);

            var User = await _userManager.FindByIdAsync(UserId);

            if (User is null)
                return Result.Failure<AuthResponse>(UserErrors.NotFound);

            var UserRefreshToken = User.RefreshTokens
                .SingleOrDefault(x => x.Token == Refreshtoken && x.IsActivated);

            if (UserRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidRefresh);

            //give a revoke date for Old refresh token 
            UserRefreshToken.RevokedOn = DateTime.UtcNow;

            var (NewToken, ExpiresIn) = _jwtProvider.GenerateToken(User);

            var NewRefreshToken = GenerateRefreshToken();
            var RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDays);

            User.RefreshTokens.Add(new RefreshToken
            {
                Token = NewRefreshToken,
                ExpiresOn = RefreshTokenExpiryDate,
            });
            await _userManager.UpdateAsync(User);

            var response = new AuthResponse(User.Id, User.Email, User.FirstName, User.LastName, NewToken, ExpiresIn,
                NewRefreshToken, RefreshTokenExpiryDate);

            return Result.Success(response); 
        }

        public async Task<Result> RevokeRefreshTokenAsync(string Token, string Refreshtoken,
            CancellationToken cancellationToken = default)
        {
            var UserId = _jwtProvider.ValidateToken(Token);

            if (UserId is null)
                return Result.Failure<AuthResponse>(UserErrors.ExpiredToken);

            var User = await _userManager.FindByIdAsync(UserId);

            if (User is null)
                return Result.Failure<AuthResponse>(UserErrors.NotFound);

            var UserRefreshToken = User.RefreshTokens
                .SingleOrDefault(x => x.Token == Refreshtoken && x.IsActivated);

            if (UserRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidRefresh);

            //revoke refresh token 
            UserRefreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(User);
            return Result.Success();

        }

        public async Task<Result<AuthResponse>> RegistrationAsync(RegistrationRequest request, CancellationToken cancellationToken = default)
        {
            var UserIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (UserIsExists)
                return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

            var UserNameIsExists = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName, cancellationToken);

            if (UserNameIsExists)
                return Result.Failure<AuthResponse>(UserErrors.DuplicatedUsername);

            var PhoneNumberExissts = await _userManager.Users.AnyAsync(x => x.PhoneNumber == request.PhoneNumber, cancellationToken);

            if (PhoneNumberExissts)
                return Result.Failure<AuthResponse>(UserErrors.DuplicatedPhone);

            var User = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userManager.CreateAsync(User, request.Password);

            if (result.Succeeded)
            {
                var (Token, ExpiresIn) = _jwtProvider.GenerateToken(User);

                var RefreshToken = GenerateRefreshToken();
                var RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDays);

                User.RefreshTokens.Add(new RefreshToken
                {
                    Token = RefreshToken,
                    ExpiresOn = RefreshTokenExpiryDate,
                });

                await _userManager.UpdateAsync(User);

                var response = new AuthResponse(User.Id, User.Email, User.FirstName, User.LastName, Token, ExpiresIn,
                    RefreshToken, RefreshTokenExpiryDate);

                return Result.Success(response);
            }

            var error = result.Errors.FirstOrDefault();

            return Result.Failure<AuthResponse>(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));

        }
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        
    }
}
