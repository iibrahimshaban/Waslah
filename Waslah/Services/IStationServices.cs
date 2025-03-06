namespace Waslah.Services
{
    public interface IStationServices
    {
        Task<Result<IEnumerable<StationResponse>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<StationResponse>> GetByLocationIdAsync(int locid, CancellationToken cancellationToken);
        Task<IEnumerable<StationDistances>> GetNearestStationsAsync(double Latitude, double Longitude
            , Double MaxDistance = 10, CancellationToken cancellationToken=default);
    }
}
