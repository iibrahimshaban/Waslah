using Microsoft.Data.SqlClient;
namespace Waslah.Services
{
    public class RouteGeneratorServices(ApplicationDbContext context) : IRouteGeneratorServices
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<ListedRouteResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var route = await _context.MyRoutes.
                AsNoTracking()
                .Where(x => x.Id == id)
                .ProjectToType<ListedRouteResponse>()
                .FirstOrDefaultAsync(cancellationToken);

            if (route is null)
                return Result.Failure<ListedRouteResponse>(RouteErrors.NotFound);

            return Result.Success(route);
        }

        public async Task<Result<ListedRouteResponse>> CreateAsync(RouteRequest request,CancellationToken cancellationToken = default)
        {
            var stationIds = new[] { request.PrimaryId, request.SecondaryId };

            var existingStationCount = await _context.Stations
                .CountAsync(x => stationIds.Contains(x.LocationId), cancellationToken);

            if (existingStationCount < 2)
                return Result.Failure<ListedRouteResponse>(StationErrors.NotFound);

            var Classification = string.Empty;

            if (request.IsExternal)
                Classification = RouteConstants.External;
            else 
                Classification = RouteConstants.Internal;

            var NewRoute = new MyRoute
            {
                PrimaryLocId = request.PrimaryId,
                SecondaryLocId = request.SecondaryId,
                Time = request.Time,
                Price = request.Price,
                Distance = request.Distance,
                Classification = Classification,
                IsDisabled = true
            };

            try
            {
                await _context.AddAsync(NewRoute, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
            {
                // 2601 = Cannot insert duplicate key row in object
                return Result.Failure<ListedRouteResponse>(RouteErrors.Doublicated);
            }

            var routeWithStations = await _context.MyRoutes
                .Include(x => x.PriStation)
                .Include(x => x.SecStation)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == NewRoute.Id, cancellationToken);

            // Map to DTO
            var routeDto = routeWithStations.Adapt<ListedRouteResponse>();

            return Result.Success(routeDto);

        }
        public async Task<Result> UpdateAsync(int Id, RouteRequest request, CancellationToken cancellationToken = default)
        {
            if (await _context.MyRoutes.FindAsync(Id, cancellationToken) is not { } Route)
                return Result.Failure(RouteErrors.NotFound);

            var stationIds = new[] { request.PrimaryId, request.SecondaryId };

            var existingStationCount = await _context.Stations
                .CountAsync(x => stationIds.Contains(x.LocationId), cancellationToken);

            if (existingStationCount < 2)
                return Result.Failure<ListedRouteResponse>(StationErrors.NotFound);

            var Classification = string.Empty;

            if (request.IsExternal)
                Classification = RouteConstants.External;
            else
                Classification = RouteConstants.Internal;

            Route.PrimaryLocId = request.PrimaryId;
            Route.SecondaryLocId = request.SecondaryId;
            Route.Time = request.Time;
            Route.Price = request.Price;
            Route.Distance = request.Distance;
            Route.Classification = Classification;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
            {
                // 2601 = Cannot insert duplicate key row in object
                return Result.Failure<ListedRouteResponse>(RouteErrors.Doublicated);
            }

            return Result.Success();

        }
        public async Task<Result> ToggleStatusAsync(int Id,CancellationToken cancellationToken= default)
        {
            if (await _context.MyRoutes.FindAsync(Id, cancellationToken) is not { } Route)
                return Result.Failure(RouteErrors.NotFound);

            Route.IsDisabled = !Route.IsDisabled;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<GeneratedRouteResponse>>> GetRouteAsync(IEnumerable<StationDistances> NearestToStart,
           IEnumerable<StationDistances> NearestToEnd, CancellationToken cancellationToken)
        {
            var response = new List<GeneratedRouteResponse>();

            var routes = await StationToStationRoute(NearestToStart, NearestToEnd, cancellationToken);

            if (routes.IsSuccess)
            {
                foreach (var route in routes.Value)
                {
                    var SingleRoute = new GeneratedRouteResponse(route.PrimaryLocId, route.SecondaryLocId);
                    response.Add(SingleRoute);
                }
                return Result.Success<IEnumerable<GeneratedRouteResponse>>(response);

            }

            var result = await StationToCity(NearestToStart,NearestToEnd, cancellationToken);

            
            if (result.IsSuccess)
            {
               foreach (var item in result.Value)
                {
                    var singleRoute = new GeneratedRouteResponse(item.Routes.FirstOrDefault()!.PrimaryLocId
                        ,item.Routes.LastOrDefault()!.SecondaryLocId);

                    response.Add(singleRoute);
                }
                return Result.Success<IEnumerable<GeneratedRouteResponse>>(response);
            }


            return Result.Failure<IEnumerable<GeneratedRouteResponse>>(RouteErrors.NotFound);

            
        }

        public async Task<Result<IEnumerable<MyRoute>>> StationToStationRoute(IEnumerable<StationDistances> NearestToStart,
            IEnumerable<StationDistances> NearestToEnd, CancellationToken cancellationToken)
        {

            var StartIds = NearestToStart.Select(x => x.Station.LocationId).ToList();
            var EndIds = NearestToEnd.Select(x => x.Station.LocationId).ToList();

            var ChainedRouteExistis = await _context.ChainedRoutes
                .AnyAsync(cr => StartIds.Contains(cr.FirstLocId) && EndIds.Contains(cr.LastLocId), cancellationToken);

            if (ChainedRouteExistis)
                return Result.Failure<IEnumerable<MyRoute>>(RouteErrors.Doublicated);

            var YourRoutes = await _context.MyRoutes
                .Where(r => (StartIds.Contains(r.PrimaryLocId) && EndIds.Contains(r.SecondaryLocId)) && !r.IsDisabled )
                .ToListAsync(cancellationToken);

            foreach ( var route in YourRoutes)
            {
                await _context.ChainedRoutes.AddAsync(route.MapToOneRoute(),cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return YourRoutes.Count > 0
                ? Result.Success<IEnumerable<MyRoute>>(YourRoutes)
                : Result.Failure<IEnumerable<MyRoute>>(RouteErrors.NotFound);

        }   
        public async Task<Result<IEnumerable<OrderRoutesResponse>>> StationToCity(IEnumerable<StationDistances> NearestToStart,
            IEnumerable<StationDistances> NearestToEnd, CancellationToken cancellationToken)
        {

            var StartLocationIds = NearestToStart.Select(x => x.Station.LocationId).ToList();
            var EndLocationIds = NearestToEnd.Select(x => x.Station.LocationId).ToList();
            var EndCities = NearestToEnd.Select(x => x.Station.City).ToList();
            
            var ReturnedRoutes = new Dictionary<int, List<MyRoute>>();

            //reutur all stations inside the end city
            var EndCityStations = await _context.Stations
                .Where(s => EndCities.Contains(s.City))
                .Select(s => s.LocationId)
                .ToListAsync(cancellationToken);

            var FirstRoutes = await _context.MyRoutes.
                Where(r => StartLocationIds.Contains(r.PrimaryLocId) && EndCityStations.Contains(r.SecondaryLocId))
                .ToListAsync(cancellationToken);

            var SecondRoutes = await _context.MyRoutes
                .Where(r => EndCityStations.Contains(r.PrimaryLocId) && EndLocationIds.Contains(r.SecondaryLocId))
                .ToListAsync(cancellationToken);
            int index = 0;
            foreach (var FRoute in FirstRoutes)
            {
                foreach (var ERoute in SecondRoutes)
                {
                    if (FRoute.SecondaryLocId == ERoute.PrimaryLocId)
                        ReturnedRoutes.Add(index++,[FRoute, ERoute]);
                }
            }

            if (ReturnedRoutes.Count == 0)
                return Result.Failure<IEnumerable<OrderRoutesResponse>>(RouteErrors.NotFound);

            foreach (var routes in ReturnedRoutes)
            {
                var chined = routes.Value.MapToChainedRoute();
                await _context.ChainedRoutes.AddAsync(chined, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Result.Success(ReturnedRoutes.Adapt<IEnumerable<OrderRoutesResponse>>());
        }
 
    }
}
