using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using System.Linq.Expressions;
using Waslah.Entities;

namespace Waslah.Services
{
    public class StationServices(ApplicationDbContext context,HybridCache hybridCache) : IStationServices
    {
        private readonly ApplicationDbContext _context = context;
        private const double EarthRadiusKm = 6371;

        private readonly HybridCache _hybridCache = hybridCache;
        private readonly string _cachePrefix = "AvilableStations";

        public async Task<IEnumerable<GovernmentResponse>> GetAllStationsInfoAsync(CancellationToken cancellationToken)
        {
            var chacheKey = $"{_cachePrefix}";

            var info = await _hybridCache.GetOrCreateAsync<IEnumerable<GovernmentResponse>>(chacheKey, async chacheEntry =>
            await _context.Stations
                .Where(x => x.IsActivated)
                .GroupBy(s => s.Government)
                .Select(g => new GovernmentResponse(
                    g.Key,
                    g.GroupBy(s => s.City)
                     .Select(c => new CityResponse(
                         c.Key,
                         c.Select(s => new StationDto(
                             s.LocationId,
                             s.Name!
                         )).ToList()
                     )).ToList()
                ))
                .ToListAsync(cancellationToken), cancellationToken: cancellationToken);

            return info;
        }
        public async Task<IEnumerable<StationResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            var Stations = await _context.Stations
                .Where(x => x.IsActivated)
                .Distinct()
                .ProjectToType<StationResponse>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Stations;
        }
        public async Task<Result<StationResponse>> GetByLocationIdAsync(int locid, CancellationToken cancellationToken)
        {

            var station = await _context.Stations
                .Where(s => s.LocationId == locid)
                 .ProjectToType<StationResponse>()
                 .AsNoTracking()
                 .FirstOrDefaultAsync(cancellationToken);

            if (station is null)
                return Result.Failure<StationResponse>(StationErrors.NotFound);

            return Result.Success(station.Adapt<StationResponse>());
        }
        public async Task<IEnumerable<StationDistances>> GetNearestStationsAsync(double Latitude,double Longitude
            , Double MaxDistance, CancellationToken cancellationToken )
        {

            var Stations = await _context.Stations
                .Where(x => x.IsActivated)
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
        public async Task<Result<StationResponse>> CreateAsync(StationRequest request, CancellationToken cancellationToken)
        {
            string[] parts = request.Coordinates.Split(',');

            if (parts?.Length != 2 )
                return Result.Failure<StationResponse>(StationErrors.InvalidCoordinates);

            var latitude = double.Parse(parts[0].Trim());
            var longitude = double.Parse(parts[1].Trim());

            var GovNumber = await GetNumberOrMaxAsync(
                x => x.Government == request.Government.Trim(),
                x => true, // fallback: any station
                x => x.GovNo,
                cancellationToken);

            var CityNumber = await GetNumberOrMaxAsync(
                x => x.City == request.City.Trim(),
                x => x.Government == request.Government.Trim(), // fallback: same government
                x => x.CityNo,
                cancellationToken);

            var StationNumber = await GetNumberOrMaxAsync(
                x => x.Name == request.Name.Trim(),
                x => x.City == request.City.Trim(), // fallback: same city
                x => x.StationNo,
                cancellationToken);


            var LocationId = CalculateLocationId(GovNumber, CityNumber,StationNumber);

            if (await _context.Stations.AnyAsync(x => x.LocationId == LocationId, cancellationToken))
                return Result.Failure<StationResponse>(StationErrors.DoublicatedStation);

            string stationType = request.IsStation
                     ? (DepartmentConditions(LocationId) ? StationConstants.StationDept : StationConstants.StationCity)
                     : (DepartmentConditions(LocationId) ? StationConstants.VillageDept : StationConstants.VillageCity);


            var station = new Station
            {
                LocationId = LocationId,
                Government = request.Government,
                City = request.City,
                Name = request.Name,
                GovNo = GovNumber,
                CityNo = CityNumber,
                StationNo = StationNumber,
                Type = stationType,
                Latitude = latitude,
                Longitude = longitude,
                IsActivated = false
            };

            await _context.AddAsync(station, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            
            await _hybridCache.RemoveAsync(_cachePrefix, cancellationToken);

            return Result.Success(station.Adapt<StationResponse>());
        }

        public async Task<Result> UpdateAsync(int LocationId,StationRequest request,CancellationToken cancellationToken)
        {

            var CurrentStation = await _context.Stations.FirstOrDefaultAsync(x => x.LocationId == LocationId, cancellationToken);

            if (CurrentStation == null)
                return Result.Failure(StationErrors.NotFound);

            string[] parts = request.Coordinates.Split(',');

            if (parts?.Length != 2)
                return Result.Failure<StationResponse>(StationErrors.InvalidCoordinates);

            var latitude = double.Parse(parts[0].Trim());
            var longitude = double.Parse(parts[1].Trim());

            string stationType = request.IsStation
                     ? (DepartmentConditions(LocationId) ? StationConstants.StationDept : StationConstants.StationCity)
                     : (DepartmentConditions(LocationId) ? StationConstants.VillageDept : StationConstants.VillageCity);

            CurrentStation.Latitude = latitude;
            CurrentStation.Longitude = longitude;
            CurrentStation.Type = stationType;
            CurrentStation.Government = request.Government;
            CurrentStation.City = request.City;
            CurrentStation.Name = request.Name;

            await _context.SaveChangesAsync(cancellationToken);
            await _hybridCache.RemoveAsync(_cachePrefix, cancellationToken);

            return Result.Success();
        }
        public async Task<Result> ToggleStatusAsync(int LocId , CancellationToken cancellationToken = default)
        {
            if (await _context.Stations.FirstOrDefaultAsync(x => x.LocationId == LocId, cancellationToken) is not { } station)
                return Result.Failure(StationErrors.NotFound);

            station.IsActivated = !station.IsActivated;
            await _context.SaveChangesAsync(cancellationToken);
            await _hybridCache.RemoveAsync(_cachePrefix, cancellationToken);

            return Result.Success();
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

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        private async Task<int> GetNumberOrMaxAsync(
             Expression<Func<Station, bool>> matchCondition,
             Expression<Func<Station, bool>> fallbackCondition,
             Expression<Func<Station, int>> selector,
             CancellationToken cancellationToken)
        {
            var existing = await _context.Stations
                .Where(matchCondition)
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);

            if (existing != 0)
                return existing;

            var max = await _context.Stations
                .Where(fallbackCondition)
                .MaxAsync(selector, cancellationToken);

            return max + 1;
        }


        private static int CalculateLocationId(int govNo,int CityNo,int StNo)
        {
            return govNo*10000 + CityNo*100 + StNo;
        }

        private static bool DepartmentConditions(int LocationId) => 
                    LocationId.ToString().StartsWith(StationConstants.CairoCode) ||
                    LocationId.ToString().StartsWith(StationConstants.AlexCode) ||
                    LocationId.ToString().StartsWith(StationConstants.GizzCode);
       
    }
}
