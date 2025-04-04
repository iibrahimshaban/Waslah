namespace Waslah.Services
{
    public interface IChainedRouteServices
    {
        Task<IEnumerable<RouteResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<IEnumerable<ChainedRoute>>> GetByLocationIdAsync(IEnumerable<GeneratedRouteResponse> LocationIds,CancellationToken cancellationToken);
        Task<Result<IEnumerable<RouteResponse>>> FindByLocationPointsAsync(RouteRequest request
           , string UserId, CancellationToken cancellationToken);
    }
}
