namespace Waslah.Abstraction.Consts;

public static class RegexPattern
{
    public const string PhoneNumber = @"^01[0125][0-9]{8}$";

    public const string Password = "^(?=.*[0-9])(?=.*[!@#$%^&*()\\-_=+{};:,<.>/?])(?=.*[a-z])(?=.*[A-Z]).{8,}$";
}
