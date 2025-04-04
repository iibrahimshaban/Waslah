namespace Waslah.Contracts.Auth
{
    public record RefreshTokenRequest(
    [Required] string Token,
    [Required] string RefreshToken
 );
}
