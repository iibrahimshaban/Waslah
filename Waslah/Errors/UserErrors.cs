namespace Waslah.Errors
{
    public class UserErrors
    {
        public static readonly Error Invalid = new (
            "User.invalid" ,"wrong name and password",StatusCodes.Status400BadRequest); 
        public static readonly Error NotFound = new (
            "User.invalid" ,"wrong name and password",StatusCodes.Status404NotFound); 
        public static readonly Error ExpiredToken = new (
            "User.ExpiredToken", "this token is not working any more",StatusCodes.Status400BadRequest);
        public static readonly Error InvalidRefresh = new (
            "User.InvalidRefreshToken", "can't generate a new refresh token",StatusCodes.Status400BadRequest);
        public static readonly Error DuplicatedEmail =
            new("User.DuplicatedEmail", "there are already a signed in user", StatusCodes.Status409Conflict);
        public static readonly Error DuplicatedPhone =
            new("User.DuplicatedPhoneNumber", "there are already a signed in user with the same phone number", StatusCodes.Status409Conflict);

        public static readonly Error DuplicatedUsername =
            new("User.DuplicatedUsername", "there are already a Username takes this name ", StatusCodes.Status409Conflict);

    }
}
