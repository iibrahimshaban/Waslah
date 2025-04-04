namespace Waslah.Contracts.Auth;

public record RegistrationRequest(
    string Email,
    string PhoneNumber,
    string Password,
    string FirstName,
    string LastName,
    string UserName
    );

