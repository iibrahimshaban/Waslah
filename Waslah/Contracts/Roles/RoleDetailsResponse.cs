namespace Waslah.Contracts.Roles;

public record RoleDetailsResponse(
    string Id,
    string Name,
    bool IsDisabled,
    IEnumerable<string> Permissions
    );
