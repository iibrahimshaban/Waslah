namespace Waslah.Contracts.ChainedRoutes
{
    public record RouteResponse(
        int Id,
       ChainedStationResponse FirstStation,
       ChainedStationResponse SecondStation,
       IEnumerable<ListedRouteResponse> Routes, 
       int NumberOfRides,
       double? Price,
       double? Distnace,
       double? Time
        );
    
}
