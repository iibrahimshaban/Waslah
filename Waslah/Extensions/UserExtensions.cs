namespace Waslah.Extensions
{
    public static class UserExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal User)
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public static string? ToFullPhotoUrl(this string? relativePath, IHttpContextAccessor httpContextAccessor)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            var request = httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return null;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}{relativePath}";
        }
    }
}
