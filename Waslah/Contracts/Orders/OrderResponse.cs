namespace Waslah.Contracts.Orders
{
    public record OrderResponse(
        int Id,
        int ChainedRouteId,
        string StartStation,
        double DistanceFromStart,
        string DestinationStation,
        double DistanceFromDestination,
        string UserName ,
        int Rating ,
        DateOnly UsedAt 
        );
    
}
