namespace Waslah.Services
{
    public interface IChainedRouteServices
    {
        Task<IEnumerable<RouteResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RouteResponse>> GetByLocationIdAsync(IEnumerable<GeneratedRouteResponse> LocationIds,CancellationToken cancellationToken);
        Task<OneOf<IEnumerable<RouteResponse>, RouteGeneratorRequest>> FindByLocationPointsAsync(RouteRequest request
            , CancellationToken cancellationToken);
    }
}
