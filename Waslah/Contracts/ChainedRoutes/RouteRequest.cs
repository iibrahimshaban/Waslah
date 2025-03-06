namespace Waslah.Contracts.ChainedRoutes
{
    public record RouteRequest(
        [ValidLocation,Required]string Start,
        [ValidLocation, Required] string End);
    
}
