
namespace Waslah.Mapping
{
    public static class ContractMapping
    {
        public static ChainedRoute MapToOneRoute(this MyRoute route)
        {
            var ChainedRoute = new ChainedRoute
            {
                FirstLocId = route.PrimaryLocId,
                LastLocId = route.SecondaryLocId,
                Distance = route.Distance,
                Price = route.Price,
                Time = route.Time,
                NumberOfRides = 1,
                Name = route.PriStation.Name + " بيروح " + route.SecStation.Name,
                RoutesInfo =
                [
                    new RouteConnector {RouteOrder =1,MyRouteId=route.Id}
                ]
                
            };

            return ChainedRoute;
        }
        public static ChainedRoute MapToChainedRoute(this List<MyRoute> routes)
        {
            var ChainedRoute = new ChainedRoute
            {
                FirstLocId = routes.FirstOrDefault()!.PrimaryLocId,
                LastLocId = routes.LastOrDefault()!.SecondaryLocId,
                Distance = routes.Sum(x => x.Distance),
                Price = routes.Sum(x => x.Price),
                Time = routes.Sum(x => x.Time),
                NumberOfRides = routes.Count,
                Name = routes.FirstOrDefault()!.PriStation.Name + " بيروح " + routes.LastOrDefault()!.SecStation.Name
            };

            int order = 0;
            foreach (var route in routes)
            {
                var connector = new RouteConnector
                {
                    MyRouteId = route.Id,
                    RouteOrder = order++,
                };
                ChainedRoute.RoutesInfo.Add(connector);
            }

            return ChainedRoute;
        }
        public static IEnumerable<RouteDetailsResponse> MapToRouteResponse(this IEnumerable<ChainedRoute> chainedRoutes,
                Dictionary<int, double> start , Dictionary<int, double> end)
        {
            var response = new List<RouteDetailsResponse>();

            foreach (var cr in chainedRoutes)
            {
                // Get distances safely
                double startDistance = start.TryGetValue(cr.FirstLocId, out var sDist) ? sDist : 0;
                double endDistance = end.TryGetValue(cr.LastLocId, out var eDist) ? eDist : 0;

                // Format coordinates as "lat, lng"
                string firstCoords = $"{cr.FirstLoc!.Latitude}, {cr.FirstLoc.Longitude}";
                string lastCoords = $"{cr.LastLoc!.Latitude}, {cr.LastLoc.Longitude}";

                var routeResponse = new RouteDetailsResponse(
                    cr.Id,
                    new ChainedStationResponse(
                        cr.FirstLocId,
                        cr.FirstLoc?.Name ?? string.Empty,
                        cr.FirstLoc?.City ?? string.Empty,
                        firstCoords,
                        startDistance
                    ),
                    new ChainedStationResponse(
                        cr.LastLocId,
                        cr.LastLoc?.Name ?? string.Empty,
                        cr.LastLoc?.City ?? string.Empty,
                        lastCoords,
                        endDistance
                    ),
                    cr.Routes?.Adapt<IEnumerable<ListedRouteResponse>>() ?? Enumerable.Empty<ListedRouteResponse>(),
                    cr.Routes?.Count() ?? 0,
                    cr.Routes?.Sum(x => x.Price) ?? 0,
                    cr.Routes?.Sum(x => x.Distance) ?? 0,
                    cr.Routes?.Sum(x => x.Time) ?? 0
                );

                response.Add(routeResponse);
            }

            return response;
        }

    }
}
