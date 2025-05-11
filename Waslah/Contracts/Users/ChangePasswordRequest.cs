namespace Waslah.Contracts.Users;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);
