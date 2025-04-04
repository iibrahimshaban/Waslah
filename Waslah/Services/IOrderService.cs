namespace Waslah.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponse>> GetAllAsync(string userid,CancellationToken cancellationToken = default);
        Task<Result> CreateAsync(OrderRequest request, string userId, CancellationToken cancellationToken);
    }
}
