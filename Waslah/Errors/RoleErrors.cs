namespace Waslah.Errors;

public static class RoleErrors
{
    public static readonly Error RoleNotFound = new(
       "Role.RoleNotFound", "role not found ", StatusCodes.Status404NotFound);

    public static readonly Error DoublicatedName = new(
        "Role.DoublicatedName", "Role name doublication ", StatusCodes.Status409Conflict);

    public static readonly Error InvalidPermissions = new(
        "Role.InvalidPermissions", "invalid permissions ", StatusCodes.Status400BadRequest);
}
