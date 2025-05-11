namespace Waslah.Abstraction.Consts;

public static class Permissions
{
    public static string Type { get; } = "Permissions";

    public const string GetUsers = "User:Read";
    public const string CreateUsers = "User:Create";
    public const string UpdateUsers = "User:Update";
 
    public const string GetRoles = "Roles:Read";
    public const string CreateRoles = "Roles:Create";
    public const string UpdateRoles = "Roles:Update";

    public const string Results = "Results:Read";
    public const string Reports = "Reports:Read";

    public const string GetRoutes = "Route:Read";
    public const string CreateRoutes = "Route:Create";
    public const string UpdateRoutes = "Route:Update";
    
    public const string GetStations = "Station:Read";
    public const string CreateStations = "Station:Create";
    public const string UpdateStations = "Station:Update";

    public const string CreateAgencyTrips = "Agency:Create";
    public const string UpdateAgencyTrips = "Agency:Upadte";

    public static List<string?> GetAllPermissions()
    {
        return typeof(Permissions).GetFields().Select(field => field.GetValue(field) as string).ToList();
    }
}
