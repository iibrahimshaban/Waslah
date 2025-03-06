

using Azure.Core;
using Waslah.Entities;

namespace Waslah.Services
{
    public class StationServices(ApplicationDbContext context,IDistanceCalculator DistanceCalc) : IStationServices
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IDistanceCalculator _Calculator = DistanceCalc;

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
                distance = _Calculator.CalculateDistance(Latitude, Longitude,
                item.Latitude, item.Longitude);

                if (distance < MaxDistance)
                 Response.Add(new StationDistances { Station = item, Distance = distance });
            }

            return Response;

        }
       

    }
}
