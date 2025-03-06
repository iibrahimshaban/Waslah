namespace Waslah.Services
{
    public interface IRouteGeneratorServices
    {
        Task<Result<IEnumerable<GeneratedRouteResponse>>> GetAllAsync(int CRId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<GeneratedRouteResponse>>> GetRouteAsync(RouteGeneratorRequest routeRequest, CancellationToken cancellationToken);
        Task<Result<GeneratedRouteResponse>> GetByIdAsync(int id ,CancellationToken cancellationToken);
    }
}
