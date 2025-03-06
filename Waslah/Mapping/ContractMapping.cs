
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
        public static IEnumerable<RouteResponse> MapToRouteResponse(this IEnumerable<ChainedRoute> chainedRoutes)
        {
            var response = new List<RouteResponse>();

            foreach (var cr in chainedRoutes)
            {
                var YourRoute = new RouteResponse(
                    cr.Id
                    , new ChainedStationResponse(
                        cr.FirstLocId, cr.FirstLoc.Name, cr.FirstLoc.City, cr.FirstLoc.Latitude + " ," + cr.FirstLoc.Longitude
                        , null
                    ), new ChainedStationResponse(
                        cr.LastLocId, cr.LastLoc.Name, cr.LastLoc.City, cr.LastLoc.Latitude + " ," + cr.LastLoc.Longitude
                        ,null
                    )
                    , cr.Routes.Adapt<IEnumerable<ListedRouteResponse>>()
                    , cr.Routes.Count()
                    , cr.Routes.Sum(x => x.Price),
                    cr.Routes.Sum(x => x.Distance),
                    cr.Routes.Sum(x => x.Time)
                    );

                response.Add(YourRoute);
            }

            return response;
        }
        public static IEnumerable<RouteResponse> MapToRouteResponseTemporary(this IEnumerable<ChainedRoute> chainedRoutes,
            Dictionary<int, double> start, Dictionary<int, double> end)
        {
            var response = new List<RouteResponse>();

            foreach (var cr in chainedRoutes)
            {
                var YourRoute = new RouteResponse(
                    cr.Id
                    , new ChainedStationResponse(
                        cr.FirstLocId, cr.FirstLoc.Name, cr.FirstLoc.City, cr.FirstLoc.Latitude + " ," + cr.FirstLoc.Longitude
                        , start[cr.FirstLocId]
                    ), new ChainedStationResponse(
                        cr.LastLocId, cr.LastLoc.Name, cr.LastLoc.City, cr.LastLoc.Latitude + " ," + cr.LastLoc.Longitude
                        , end[cr.LastLocId]
                    )
                    , cr.Routes.Adapt<IEnumerable<ListedRouteResponse>>()
                    , cr.Routes.Count()
                    , cr.Routes.Sum(x => x.Price),
                    cr.Routes.Sum(x => x.Distance),
                    cr.Routes.Sum(x => x.Time)
                    );

                response.Add(YourRoute);
            }

            return response;
        }
    }
}
