
namespace Waslah.Services
{
    public interface IAuthService
    {
        Task<Result> RegistrationAsync(RegistrationRequest request, CancellationToken cancellationToken = default);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<Result> ReasendEmailConfiramtionCode(ResendConfirmEmailRequest request, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<Result> RevokeRefreshTokenAsync(RefreshTokenRequest request);
        Task<Result> SendResetPasswordCodeAsync(ForgetPasswordRequest request, CancellationToken cancellationToken = default);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(RefreshTokenRequest request);
    }
}
