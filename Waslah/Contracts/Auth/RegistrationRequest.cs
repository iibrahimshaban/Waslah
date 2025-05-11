namespace Waslah.Contracts.Auth;

public record RegistrationRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string UserName,
    IFormFile? ProfilePhoto
    );

