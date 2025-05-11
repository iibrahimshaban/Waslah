namespace Waslah.Contracts.ChainedRoutes
{
    public record RouteDetailsResponse(
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
