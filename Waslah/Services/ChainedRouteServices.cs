
using Waslah.Mapping;

namespace Waslah.Services
{
    public class ChainedRouteServices(ApplicationDbContext context,IStationServices station) : IChainedRouteServices
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IStationServices _station = station;

        public async Task<IEnumerable<RouteResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
           var result = await _context.ChainedRoutes
                .Include(x => x.Routes) .ThenInclude(x =>x.PriStation).
                Include(x => x.Routes).ThenInclude(x => x.SecStation).ToListAsync(cancellationToken);

            return result.MapToRouteResponse();
            
        }
        public async Task<IEnumerable<RouteResponse>> GetByLocationIdAsync(IEnumerable<GeneratedRouteResponse> LocationIds, CancellationToken cancellationToken)
        {
            var routes = new List<ChainedRoute>();

            foreach (var item in LocationIds)
            {
                var ChainedRoutes = await _context.ChainedRoutes
                .Where(r => r.FirstLocId == item.StartId && r.LastLocId == item.EndId)
                .Include(x => x.Routes).ThenInclude(x => x.PriStation)
                .Include(x => x.Routes).ThenInclude(x => x.SecStation)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

                routes.AddRange(ChainedRoutes);
            }

            var response = routes.MapToRouteResponse().ToList();

            return response;
        }
        public async Task<OneOf<IEnumerable<RouteResponse>, RouteGeneratorRequest>> FindByLocationPointsAsync(RouteRequest request
            , CancellationToken cancellationToken)
        {
            var Points =  request.EvaluatePoints();

            var NearestToStart = await _station.GetNearestStationsAsync(Points.StartLatitude,Points.StartLongitude,5);
            var NearestToEnd = await _station.GetNearestStationsAsync(Points.EndtLatitude,Points.EndLongitude,5);

            var StartIds = NearestToStart.Select(x => x.Station.LocationId).ToList();
            var EndIds = NearestToEnd.Select(x => x.Station.LocationId).ToList();

            var StartStations = new Dictionary<int, double>();
            var EndStations = new Dictionary<int, double>();

            NearestToStart.ToList().ForEach(x => StartStations.Add(x.Station.LocationId,x.Distance));
            NearestToEnd.ToList().ForEach(x => EndStations.Add(x.Station.LocationId,x.Distance));

            var routes = await _context.ChainedRoutes
                .Where(cr => StartIds.Contains(cr.FirstLocId) && EndIds.Contains(cr.LastLocId))
                .Include(x => x.FirstLoc).Include(x => x.LastLoc)
                .Include(x => x.Routes).ThenInclude(r => r.PriStation)
                .Include(x => x.Routes).ThenInclude(r => r.SecStation)
                .ToListAsync(cancellationToken);


            if (!routes.Any())
                return new RouteGeneratorRequest(NearestToStart, NearestToEnd);

            var response = routes.MapToRouteResponseTemporary(StartStations,EndStations).ToList();

            return response;


        }
    }
}
