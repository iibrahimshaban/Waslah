namespace Waslah.Contracts.Users;

public record UserProfileResponse(
    string Email,
    string FirstName,
    string LastName,
    string UserName,
    string? ProfilePhotoUrl
    );
