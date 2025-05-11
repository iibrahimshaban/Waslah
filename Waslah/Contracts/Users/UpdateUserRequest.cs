namespace Waslah.Contracts.Users;

public record UpdateUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string UserName,
    IList<string> Roles
    );
