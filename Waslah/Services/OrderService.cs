
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
            var RouteExistis = await _context.ChainedRoutes
                .Include(x => x.FirstLoc)
                .Include(x => x.LastLoc)
                .FirstOrDefaultAsync(x => x.Id == request.ChainedRouteId);

            if (RouteExistis == null)
                return Result.Failure(RouteErrors.NotFound);

            var UserExistis = await _context.UserPoints.AnyAsync(x => x.UserId == userId,cancellationToken);

            if (!UserExistis)
                return Result.Failure(UserErrors.NotFound);

            var UserPoint = await _context.UserPoints
                .Where(x => x.UserId == userId && x.IsLocked)
                .FirstOrDefaultAsync(cancellationToken);

            var DistanceFromStart = _stationServices.CalculateDistance(RouteExistis.FirstLoc.Latitude
                , RouteExistis.FirstLoc.Longitude,UserPoint!.OriginLatitude,UserPoint.OriginLongitude);

            var DistanceFromEnd = _stationServices.CalculateDistance(RouteExistis.LastLoc.Latitude
                , RouteExistis.LastLoc.Longitude, UserPoint!.DestinationLatitude, UserPoint.DestinationLongitude);

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
