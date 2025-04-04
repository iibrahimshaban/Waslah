using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Waslah.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(IOrderService order) : ControllerBase
    {
        private readonly IOrderService _order = order;

        [HttpGet("All")]
        public async Task<IActionResult> GetAll (CancellationToken cancellationToken)
        {
            var UserId = User.GetUserId();
            var response = await _order.GetAllAsync(UserId!,cancellationToken);
            return Ok(response);
        }
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
