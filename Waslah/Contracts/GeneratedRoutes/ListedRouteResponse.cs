namespace Waslah.Contracts.GeneratedRoutes
{
    public record ListedRouteResponse(
        int Id,
        string StartName,
        string StartCoordinates,
        string EndName,
        string EndCoordinates,
        Double Distance,
        double Time,
        double Price,
        string Classification,
        bool IsDisabled
        );
    
}
