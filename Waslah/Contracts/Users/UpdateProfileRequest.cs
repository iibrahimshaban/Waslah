namespace Waslah.Contracts.Users;

public record UpdateProfileRequest(
    string UserName,
    string FirstName,
    string LastName,
    IFormFile? ProfilePhoto
    );
