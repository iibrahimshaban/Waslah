namespace Waslah.Services
{
    public interface IRouteGeneratorServices
    {
        Task<Result<ListedRouteResponse>> GetByIdAsync(int id ,CancellationToken cancellationToken);
        Task<Result<ListedRouteResponse>> CreateAsync(RouteRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int Id, RouteRequest request, CancellationToken cancellationToken = default);
        Task<Result> ToggleStatusAsync(int Id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<GeneratedRouteResponse>>> GetRouteAsync(IEnumerable<StationDistances> NearestToStart
            , IEnumerable<StationDistances> NearestToEnd, CancellationToken cancellationToken);
    }
}
