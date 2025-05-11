namespace Waslah.Errors
{
    public class UserErrors
    {
        public static readonly Error InvalidJwtToken = new(
         "User.InvalidToken", "can't validate the given token", StatusCodes.Status401Unauthorized);

        public static readonly Error Invalidcredentials = new(
            "User.Invalidcredentials", "invalid email / password", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidRefreshToken =
            new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

        public static readonly Error DisabledUser =
            new("User.DisabledUser", "Disabled user, please contact your administrator", StatusCodes.Status401Unauthorized);

        public static readonly Error LockedOutUser =
            new("User.LockedOutUser", "you have been locked for 5 minutes", StatusCodes.Status401Unauthorized);

        public static readonly Error DoublicatedEmail = new(
            "User.DoublicatedEmail", "you have signed in once with this email", StatusCodes.Status409Conflict);

        public static readonly Error DoublicatedUserName = new(
            "User.DoublicatedUserName", "username is already taken ", StatusCodes.Status409Conflict);

        public static readonly Error InvalidOtpCode = new(
            "User.InvalidCode", "please enter a valid otp code to confirm and continue the process ", StatusCodes.Status401Unauthorized);

        public static readonly Error InActiveOtpCode = new(
            "User.InActiveOtpCode", "you have once reset password with the same Otp ", StatusCodes.Status401Unauthorized);

        public static readonly Error ExpiredOtpcode = new(
            "User.ExpiredOtpcode", "your Otp code is been expired ", StatusCodes.Status401Unauthorized);

        public static readonly Error DoublicatedConfirmation = new(
            "User.DoublicatedConfirmation", "you eamil is already confirmed ", StatusCodes.Status409Conflict);

        public static readonly Error EmailNotConfirmed = new(
           "User.EmailNotConfirmed", "please confirm you email before sign in ", StatusCodes.Status401Unauthorized);

        public static readonly Error UserNotFound = new(
           "User.UserNotFound", "can't find user with given details", StatusCodes.Status404NotFound);

        public static readonly Error RoleNotFound = new(
           "User.RoleNotFound", "invalid roles ", StatusCodes.Status404NotFound);
    }
}
