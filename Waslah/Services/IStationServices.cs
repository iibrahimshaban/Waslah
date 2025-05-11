namespace Waslah.Services
{
    public interface IStationServices
    {
        Task<IEnumerable<GovernmentResponse>> GetAllStationsInfoAsync(CancellationToken cancellationToken);
        Task<IEnumerable<StationResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<StationResponse>> GetByLocationIdAsync(int locid, CancellationToken cancellationToken);
        Task<IEnumerable<StationDistances>> GetNearestStationsAsync(double Latitude, double Longitude
            , Double MaxDistance = 10, CancellationToken cancellationToken=default);
        Task<Result<StationResponse>> CreateAsync(StationRequest request, CancellationToken cancellationToken);
        Task<Result> UpdateAsync(int LocationId, StationRequest request, CancellationToken cancellationToken);
        Task<Result> ToggleStatusAsync(int LocId, CancellationToken cancellationToken = default);

        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}
