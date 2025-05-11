namespace Waslah.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetDetailsAsync(string UserId, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(string userId);
    Task<Result> UnlockAsync(string userId);
    Task<UserProfileResponse> GetProfileAsync(string userId);
    Task<Result> UpdateProfileAsync(string UserId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(string UserId, ChangePasswordRequest request);
}
