namespace Waslah.Contracts.GeneratedRoutes
{
    public record RouteGeneratorRequest(
        IEnumerable<StationDistances> NearestToStart,
        IEnumerable<StationDistances> NearestToEnd
        );
    
}
