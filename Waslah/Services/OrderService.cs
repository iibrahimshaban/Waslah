
namespace Waslah.Services
{
    public class OrderService(ApplicationDbContext context,IStationServices stationServices) : IOrderService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IStationServices _stationServices = stationServices;

        public async Task<IEnumerable<OrderResponse>> GetAllAsync(string UserId,CancellationToken cancellationToken = default)
        {
            var Orders = await _context.Orders
                        .Where(x => x.UserId == UserId)
                        .ProjectToType<OrderResponse>()
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

            return Orders;
        }
        public async Task<Result> CreateAsync(OrderRequest request,string userId, CancellationToken cancellationToken)
        {
            var Route = await _context.ChainedRoutes
                .Where(x => x.Id == request.ChainedRouteId)
                .Include(x => x.Routes).ThenInclude(x => x.PriStation)
                .Include(x => x.Routes).ThenInclude(x => x.SecStation)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (Route is null)
                return Result.Failure(OrderErrors.RouteNotFound);


            var UserPoint = await _context.UserPoints
                .Where(x => x.UserId == userId && !x.IsLocked)
                .FirstOrDefaultAsync(cancellationToken);

            if (UserPoint == null)
                return Result.Failure(OrderErrors.TripsNotFound);

            var StartStation = Route.Routes.FirstOrDefault()!.PriStation;
            var LastStation = Route.Routes.LastOrDefault()!.SecStation;

            var DistanceFromStart = _stationServices.CalculateDistance(StartStation.Latitude
                , StartStation.Longitude, UserPoint!.OriginLatitude,UserPoint.OriginLongitude);

            var DistanceFromEnd = _stationServices.CalculateDistance(LastStation.Latitude
                , LastStation.Longitude, UserPoint!.DestinationLatitude, UserPoint.DestinationLongitude);

            var Order = new Order
            {
                UserId = userId,
                RouteId = request.ChainedRouteId,
                Rating = request.Rate,
                DistanceFromStart = DistanceFromStart,
                DistanceFromDestination = DistanceFromEnd,
            };

            await _context.Orders.AddAsync(Order,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
