namespace Waslah.Authentication
{
    public interface IJwtProvider
    {
        (string Token, int ExpiresIn) GenerateToken(ApplicationUser user ,IEnumerable<string> Roles,IEnumerable<string> Permissions);
        Result<string> ValidateToken(string Token);
    }
}
