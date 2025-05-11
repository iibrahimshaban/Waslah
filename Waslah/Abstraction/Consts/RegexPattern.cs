namespace Waslah.Abstraction.Consts;

public static class RegexPattern
{
    public const string PhoneNumber = @"^01[0125][0-9]{8}$";

    public const string Password = "^(?=.*[0-9])(?=.*[!@#$%^&*()\\-_=+{};:,<.>/?])(?=.*[a-z])(?=.*[A-Z]).{8,}$";

    public const string UserName = "^[a-zA-Z][a-zA-Z0-9_]{2,15}$";

    public const string ArabicOnlyPattern = @"^[\u0600-\u06FF\s_\-,]+$";
    public const string UrlPattern = @"^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?$";
}
