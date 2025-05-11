namespace Waslah.Contracts.Auth
{
    public record AuthResponse(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string Token,
    int ExpirsIn,
    string RefreshToken,
    DateTime RefreshTokenExpiryDate
    );
}
