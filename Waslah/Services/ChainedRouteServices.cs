namespace Waslah.Services
{
    public class ChainedRouteServices(ApplicationDbContext context,IStationServices station 
        , IRouteGeneratorServices routeGenerator) : IChainedRouteServices
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IStationServices _station = station;
        private readonly IRouteGeneratorServices _routeGenerator = routeGenerator;

        public async Task<IEnumerable<RouteResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
           var result = await _context.ChainedRoutes
                .Include(x => x.Routes) .ThenInclude(x =>x.PriStation).
                Include(x => x.Routes).ThenInclude(x => x.SecStation).ToListAsync(cancellationToken);

            return result.MapToRouteResponse();
            
        }
        public async Task<Result<IEnumerable<ChainedRoute>>> GetByLocationIdAsync(IEnumerable<GeneratedRouteResponse> LocationIds
            , CancellationToken cancellationToken)
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

            if (routes.Any())
                return Result.Success<IEnumerable<ChainedRoute>>(routes);

            return Result.Failure<IEnumerable<ChainedRoute>>(RouteErrors.NotFound);
        }
        public async Task<Result<IEnumerable<RouteResponse>>> FindByLocationPointsAsync(RouteRequest request
           ,string UserId , CancellationToken cancellationToken)
        {
            
            var Points =  request.EvaluatePoints();

            var UserExistis = await _context.UserPoints
                .Where(x => x.UserId == UserId)
                .ToListAsync(cancellationToken);

            if (UserExistis.Count > 0)
            {
                foreach (var point in UserExistis)
                {
                    point.IsLocked = false;
                }
            }


            var UserPoint = new UserPoints
            {
                OriginLatitude = Points.StartLatitude,
                OriginLongitude = Points.StartLongitude,
                DestinationLatitude = Points.EndtLatitude,
                DestinationLongitude = Points.EndLongitude,
                UserId = UserId
            };

            await _context.UserPoints.AddAsync(UserPoint);
            await _context.SaveChangesAsync(cancellationToken);

            var NearestToStart = await _station.GetNearestStationsAsync(Points.StartLatitude,Points.StartLongitude,5,cancellationToken);
            var NearestToEnd = await _station.GetNearestStationsAsync(Points.EndtLatitude,Points.EndLongitude,5,cancellationToken);

            var StartIds = NearestToStart.Select(x => x.Station.LocationId).ToList();
            var EndIds = NearestToEnd.Select(x => x.Station.LocationId).ToList();

            var StartStations = new Dictionary<int, double>();
            var EndStations = new Dictionary<int, double>();

            NearestToStart.ToList().ForEach(x => StartStations.Add(x.Station.LocationId,x.Distance));
            NearestToEnd.ToList().ForEach(x => EndStations.Add(x.Station.LocationId,x.Distance));

            // if chained route is already stored in DB
            var routes = await _context.ChainedRoutes
                .Where(cr => StartIds.Contains(cr.FirstLocId) && EndIds.Contains(cr.LastLocId))
                .Include(x => x.FirstLoc).Include(x => x.LastLoc)
                .Include(x => x.Routes).ThenInclude(r => r.PriStation)
                .Include(x => x.Routes).ThenInclude(r => r.SecStation)
                .ToListAsync(cancellationToken);


            if (routes.Any())
            {
                var response = routes.MapToRouteResponseTemporary(StartStations, EndStations).ToList();
                return Result.Success<IEnumerable<RouteResponse>>(response);
            }

            // using RCBA to chain a route
            var GeneratedIds = await _routeGenerator.GetRouteAsync(new (NearestToStart, NearestToEnd), cancellationToken);
            
            if (GeneratedIds.IsFailur)
                return Result.Failure<IEnumerable<RouteResponse>>(RouteErrors.NotFound);

            var result = await GetByLocationIdAsync(GeneratedIds.Value, cancellationToken);

            if (result.IsFailur)
                return Result.Failure<IEnumerable<RouteResponse>>(RouteErrors.NotFound);

            var GeneratedRoutes = result.Value.MapToRouteResponseTemporary(StartStations, EndStations);
            return Result.Success(GeneratedRoutes);
        }
    }
}
