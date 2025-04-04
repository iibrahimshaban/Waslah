

using Azure.Core;
using Waslah.Entities;

namespace Waslah.Services
{
    public class StationServices(ApplicationDbContext context) : IStationServices
    {
        private readonly ApplicationDbContext _context = context;
        private const double EarthRadiusKm = 6371;

        public async Task<Result<IEnumerable<StationResponse>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var Stations = await _context.Stations
                .ProjectToType<StationResponse>()
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<StationResponse>>(Stations);
        }
        public async Task<Result<StationResponse>> GetByLocationIdAsync(int locid, CancellationToken cancellationToken)
        {

            var station = await _context.Stations
                 .FirstOrDefaultAsync(s => s.LocationId == locid, cancellationToken);


            if (station is null)
                return Result.Failure<StationResponse>(StationErrors.NotFound);

            return Result.Success(station.Adapt<StationResponse>());
        }
        public async Task<IEnumerable<StationDistances>> GetNearestStationsAsync(double Latitude,double Longitude
            , Double MaxDistance, CancellationToken cancellationToken )
        {

            var Stations = await _context.Stations
                .Include(x => x.StationType)
                .ToListAsync(cancellationToken);

            var Response = new List<StationDistances>();
            double distance;

            foreach (var item in Stations)
            {
                distance = CalculateDistance(Latitude, Longitude,
                item.Latitude, item.Longitude);

                if (distance < MaxDistance)
                 Response.Add(new StationDistances { Station = item, Distance = distance });
            }

            return Response;

        }
        
        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double lat1Rad = DegreesToRadians(lat1);
            double lon1Rad = DegreesToRadians(lon1);
            double lat2Rad = DegreesToRadians(lat2);
            double lon2Rad = DegreesToRadians(lon2);

            // Difference in coordinates
            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;

            // Haversine formula
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Distance in kilometers
            double distance = EarthRadiusKm * c;
            return distance;
        }

        public double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }


    }
}
