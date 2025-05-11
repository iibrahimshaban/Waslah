namespace Waslah.Contracts.ChainedRoutes
{
    public record FindRouteRequest(
        [ValidLocation,Required]string Start,
        [ValidLocation, Required] string End);
    
}
