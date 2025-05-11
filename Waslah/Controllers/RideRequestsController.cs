using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Waslah.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RideRequestsController(IOrderService order) : ControllerBase
    {
        private readonly IOrderService _order = order;

        [HttpPost("")]
        public async Task<IActionResult> Create(OrderRequest request,CancellationToken cancellationToken=default)
        {
            var result = await _order.CreateAsync(request,User.GetUserId()!,cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

    }
}
