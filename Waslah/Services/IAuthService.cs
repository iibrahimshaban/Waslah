
namespace Waslah.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password,
        CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(string Token, string Refreshtoken,
        CancellationToken cancellationToken = default);
        Task<Result> RevokeRefreshTokenAsync(string Token, string Refreshtoken,
            CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> RegistrationAsync(RegistrationRequest request,
        CancellationToken cancellationToken = default);
    }
}
