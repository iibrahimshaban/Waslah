using static Plotly.NET.StyleParam.DrawingStyle;
using System.Threading;

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
            var routes = await _context.ChainedRoutes
                 .Include(x => x.FirstLoc).Include(y => y.LastLoc)
                 .ProjectToType<RouteResponse>()
                 .AsNoTracking()
                 .ToListAsync(cancellationToken);

            return routes;
            
        }
        
        public async Task<Result<IEnumerable<RouteDetailsResponse>>> FindByLocationPointsAsync(FindRouteRequest request
           ,string UserId , CancellationToken cancellationToken)
        {
            
            var Points =  request.EvaluatePoints();

            await AddUserPoints(UserId, Points,cancellationToken);

            var NearestToStart = await _station.GetNearestStationsAsync(Points.StartLatitude,Points.StartLongitude,5,cancellationToken);
            var NearestToEnd = await _station.GetNearestStationsAsync(Points.EndtLatitude,Points.EndLongitude,5,cancellationToken);

            var StartIds = NearestToStart.Select(x => x.Station.LocationId).ToList();
            var EndIds = NearestToEnd.Select(x => x.Station.LocationId).ToList();

            var StartStations = new Dictionary<int, double>();
            var EndStations = new Dictionary<int, double>();

            NearestToStart.ToList().ForEach(x => StartStations.Add(x.Station.LocationId,x.Distance));
            NearestToEnd.ToList().ForEach(x => EndStations.Add(x.Station.LocationId,x.Distance));

            // if chained route is already stored in DB
            var routes = await GetByLocationIdAsync(StartIds, EndIds,cancellationToken);

            if (routes.Any())
            {
                var response = routes.MapToRouteResponse(StartStations, EndStations).ToList();
                return Result.Success<IEnumerable<RouteDetailsResponse>>(response);
            }

            // using RCBA to chain a route
            var GeneratedIds = await _routeGenerator.GetRouteAsync(NearestToStart, NearestToEnd, cancellationToken);
            
            if (GeneratedIds.IsFailur)
                return Result.Failure<IEnumerable<RouteDetailsResponse>>(RouteErrors.NotGenerated);


             routes = await GetByLocationIdAsync(
                GeneratedIds.Value.Select(x => x.StartId),
                GeneratedIds.Value.Select(x => x.EndId),
                cancellationToken);

            var responses =routes.MapToRouteResponse(StartStations, EndStations);

            return Result.Success(responses);
        }

        public async Task<Result<IEnumerable<ListedRouteResponse>>> GetAllCurrentAsync(int CRId, CancellationToken cancellationToken = default)
        {
            var ChainedRouteExistis = await _context.ChainedRoutes.AnyAsync(x => x.Id == CRId, cancellationToken);

            if (!ChainedRouteExistis)
                return Result.Failure<IEnumerable<ListedRouteResponse>>(RouteErrors.NotFound);

            var routes = await _context.MyRoutes
                .AsNoTracking()
                .Where(route => route.chainedRoutes.Any(cr => cr.Id == CRId))
                .Include(route => route.PriStation)
                .Include(route => route.SecStation)
                .ToListAsync(cancellationToken);


            if (routes.Count > 0)
                return Result.Success(routes.Adapt<IEnumerable<ListedRouteResponse>>());

            return Result.Failure<IEnumerable<ListedRouteResponse>>(RouteErrors.NotGenerated);
        }
        private async Task<IEnumerable<ChainedRoute>> GetByLocationIdAsync(IEnumerable<int> StartLocationIds, IEnumerable<int> EndLocationIds
            , CancellationToken cancellationToken)
        {

                var ChainedRoutes = await _context.ChainedRoutes
                .Where(r => StartLocationIds.Contains(r.FirstLocId)  && EndLocationIds.Contains(r.LastLocId))
                .Include(x => x.FirstLoc).Include(x => x.LastLoc)
                .Include(x => x.Routes).ThenInclude(x => x.PriStation)
                .Include(x => x.Routes).ThenInclude(x => x.SecStation)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            return ChainedRoutes;
        }

        private async Task AddUserPoints(string UserId , RoutePointsRequest Points,CancellationToken cancellationToken)
        {
            var UserExistis = await _context.UserPoints
                .Where(x => x.UserId == UserId)
                .ToListAsync(cancellationToken);

            if (UserExistis.Count > 0)
            {
                foreach (var point in UserExistis)
                {
                    point.IsLocked = true;
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

            
        }
    }
}
