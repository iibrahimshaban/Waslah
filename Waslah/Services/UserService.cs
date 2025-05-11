namespace Waslah.Services;

public class UserService(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IWebHostEnvironment webEnvironment,
    ApplicationDbContext context ,
    IJwtProvider jwtProvider) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IWebHostEnvironment _webEnvironment = webEnvironment;
    private readonly ApplicationDbContext _context = context;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var Users = await (from u in _context.Users
                           join ur in _context.UserRoles
                           on u.Id equals ur.UserId
                           join r in _context.Roles
                           on ur.RoleId equals r.Id into Roles
                           where Roles.All(x => x.Name != DefaultRoles.Member)
                           select new
                           {
                               u.Id,
                               u.Email,
                               u.UserName,
                               u.FirstName,
                               u.LastName,
                               u.IsDisabled,
                               roles = Roles.Select(x => x.Name).ToList()
                           })
                           .GroupBy(x => new { x.Id, x.Email, x.UserName, x.FirstName, x.LastName, x.IsDisabled })
                           .Select(x => new UserResponse(
                               x.Key.Id,
                               x.Key.Email,
                               x.Key.UserName,
                               x.Key.FirstName,
                               x.Key.LastName,
                               x.Key.IsDisabled,
                               x.SelectMany(x => x.roles)
                           ))
                           .ToListAsync(cancellationToken);

        return Users;
    }
    public async Task<Result<UserResponse>> GetDetailsAsync(string UserId, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByIdAsync(UserId) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var response = (user, roles).Adapt<UserResponse>();

        return Result.Success(response);
    }
    public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var EmailExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (EmailExists)
            return Result.Failure<UserResponse>(UserErrors.DoublicatedEmail);

        var userNameExists = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName, cancellationToken);

        if (userNameExists)
            return Result.Failure<UserResponse>(UserErrors.DoublicatedUserName);

        var currentRoles = await _context.Roles
                    .Where(x => !x.IsDisabled)
                    .Select(x => x.Name!)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

        if (request.Roles.Except(currentRoles).Any())
            return Result.Failure<UserResponse>(UserErrors.RoleNotFound);

        var user = request.Adapt<ApplicationUser>();

        user.EmailConfirmed = true;

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, request.Roles);

            var response = (user, request.Roles).Adapt<UserResponse>();

            return Result.Success(response);
        }

        var error = result.Errors.FirstOrDefault();

        return Result.Failure<UserResponse>(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> UpdateAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        var EmailExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != userId, cancellationToken);

        if (EmailExists)
            return Result.Failure(UserErrors.DoublicatedEmail);

        var userNameExists = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName && x.Id != userId, cancellationToken);

        if (userNameExists)
            return Result.Failure(UserErrors.DoublicatedUserName);

        var currentRoles = await _context.Roles
            .Where(x => !x.IsDisabled)
            .Select(x => x.Name!)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (request.Roles.Except(currentRoles).Any())
            return Result.Failure(UserErrors.RoleNotFound);

        user = request.Adapt(user);

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            await _context.UserRoles
                .Where(x => x.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);

            await _userManager.AddToRolesAsync(user, request.Roles);
            return Result.Success();
        }
        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<Result> ToggleStatusAsync(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        user.IsDisabled = !user.IsDisabled;
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }
    public async Task<Result> UnlockAsync(string userId)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        await _userManager.SetLockoutEndDateAsync(user, null);

        return Result.Success();
    }

    public async Task<UserProfileResponse> GetProfileAsync(string userId)
    {
        var user = await _userManager.Users
             .Where(x => x.Id == userId)
             .FirstOrDefaultAsync();

        var response = new UserProfileResponse(
            user!.Email!,
            user.FirstName,
            user.LastName,
            user.UserName!,
            user.ProfilePhotoPath.ToFullPhotoUrl(_httpContextAccessor)
            );

        return response;
    }
    public async Task<Result> UpdateProfileAsync(string UserId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var CurrentUser = await _userManager.FindByIdAsync(UserId);

        var relativePath = string.Empty;

        if (request.ProfilePhoto is not null)
        {
            DeleteOldProfilePhoto(CurrentUser!.ProfilePhotoPath);

            // Now upload the new photo
            var uploadsFolder = Path.Combine(_webEnvironment.WebRootPath, "user-photos");
            Directory.CreateDirectory(uploadsFolder); // Make sure the folder exists

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ProfilePhoto.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.ProfilePhoto.CopyToAsync(stream, cancellationToken);
            }

            relativePath = $"/user-photos/{uniqueFileName}";

        }
        else if (request.ProfilePhoto is null)
            DeleteOldProfilePhoto(CurrentUser!.ProfilePhotoPath);

        var userNameExists = await _userManager.Users
            .AnyAsync(x => x.UserName == request.UserName && x.Id != UserId,cancellationToken);

        if (userNameExists)
            return Result.Failure(UserErrors.DoublicatedUserName);


        await _userManager.Users.
            Where(x => x.Id == UserId)
            .ExecuteUpdateAsync(setter =>
                 setter
                 .SetProperty(x => x.FirstName, request.FirstName)
                 .SetProperty(x => x.LastName, request.LastName)
                 .SetProperty(x => x.UserName, request.UserName)
                 .SetProperty(x => x.ProfilePhotoPath, relativePath),
                 cancellationToken
            );

        return Result.Success();
    }
    public async Task<Result> ChangePasswordAsync(string UserId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(UserId);

        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
        {
            var activeTokens = user!.RefreshTokens.Where(x => x.IsActivated).ToList();

            foreach (var token in activeTokens)
            {
                token.RevokedOn = DateTime.UtcNow;
            }

            await _userManager.UpdateAsync(user);

            return Result.Success();
        }
            

        var error = result.Errors.FirstOrDefault();

        return Result.Failure(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    private void DeleteOldProfilePhoto(string? photoPath)
    {
        if (string.IsNullOrWhiteSpace(photoPath)) return;

        var fullPath = Path.Combine(_webEnvironment.WebRootPath, photoPath.TrimStart('/'));
        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }
    }
}
