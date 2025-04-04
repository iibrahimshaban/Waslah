namespace Waslah.Services
{
    public interface IStationServices
    {
        Task<Result<IEnumerable<StationResponse>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<StationResponse>> GetByLocationIdAsync(int locid, CancellationToken cancellationToken);
        Task<IEnumerable<StationDistances>> GetNearestStationsAsync(double Latitude, double Longitude
            , Double MaxDistance = 10, CancellationToken cancellationToken=default);

        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
        double DegreesToRadians(double degrees);
    }
}
