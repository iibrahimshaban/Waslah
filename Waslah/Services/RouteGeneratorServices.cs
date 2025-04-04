using System.Collections.Generic;
using System.Security.Cryptography.Xml;

namespace Waslah.Services
{
    public class RouteGeneratorServices(ApplicationDbContext context) : IRouteGeneratorServices
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<IEnumerable<GeneratedRouteResponse>>> GetAllAsync(int CRId, CancellationToken cancellationToken = default)
        {
            var ChainedRouteExistis = await _context.ChainedRoutes.AnyAsync(x => x.Id == CRId,cancellationToken);

            if (!ChainedRouteExistis)
                return Result.Failure<IEnumerable<GeneratedRouteResponse>>(RouteErrors.NotFound);

            var routes = await _context.MyRoutes
                .Where(x => x.chainedRoutes.Any(cr => cr.Id == CRId))
                .Include(r => r.PriStation)
                .Include(r => r.SecStation)
                .ProjectToType<GeneratedRouteResponse>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (routes.Count > 0)
                return Result.Success<IEnumerable<GeneratedRouteResponse>>(routes);

            return Result.Failure<IEnumerable<GeneratedRouteResponse>>(RouteErrors.NotGenerated);

           
        }

        public async Task<Result<GeneratedRouteResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var route = await _context.MyRoutes
                .FindAsync(id, cancellationToken);

            if (route is null)
                return Result.Failure<GeneratedRouteResponse>(RouteErrors.NotFound);

            return Result.Success(route.Adapt<GeneratedRouteResponse>());
        }
        public async Task<Result<IEnumerable<GeneratedRouteResponse>>> GetRouteAsync(RouteGeneratorRequest Request,
            CancellationToken cancellationToken)
        {
            var response = new List<GeneratedRouteResponse>();

            var routes = await StationToStationRoute(Request.NearestToStart, Request.NearestToEnd, cancellationToken);

            if (routes.IsSuccess)
            {
                foreach (var route in routes.Value)
                {
                    var SingleRoute = new GeneratedRouteResponse(route.PrimaryLocId, route.SecondaryLocId);
                    response.Add(SingleRoute);
                }
                return Result.Success<IEnumerable<GeneratedRouteResponse>>(response);

            }

            var result = await StationToCity(Request.NearestToStart, Request.NearestToEnd, cancellationToken);

            
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
                .Where(r => StartIds.Contains(r.PrimaryLocId) && EndIds.Contains(r.SecondaryLocId))
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
