namespace Waslah.Services
{
    public interface IChainedRouteServices
    {
        Task<IEnumerable<RouteResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<IEnumerable<RouteDetailsResponse>>> FindByLocationPointsAsync(FindRouteRequest request
           , string UserId, CancellationToken cancellationToken);
        Task<Result<IEnumerable<ListedRouteResponse>>> GetAllCurrentAsync(int CRId, CancellationToken cancellationToken = default);
    }
}
