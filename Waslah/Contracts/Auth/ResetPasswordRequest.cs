namespace Waslah.Contracts.Auth;

public record ResetPasswordRequest(
    string Email,
    string NewPassword,
    string Otp
    );