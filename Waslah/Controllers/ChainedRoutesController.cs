namespace Waslah.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChainedRoutesController(IChainedRouteServices chainedRoute ,IRouteGeneratorServices routeServices) : ControllerBase
    {
        private readonly IChainedRouteServices _chainedRoute = chainedRoute;
        private readonly IRouteGeneratorServices _routeServices = routeServices;

        [HttpGet("All")]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var routes = await _chainedRoute.GetAllAsync(cancellationToken);

                if (routes.Any()) 
                     return Ok(routes);

            return BadRequest("can't find any routes");
        }
        [HttpPost("ByLocation")]
        public async Task<IActionResult> GetByLocationAsync([FromBody] RouteRequest Request,
            CancellationToken cancellationToken)
        {
            var UserId = User.GetUserId();
            var result = await _chainedRoute.FindByLocationPointsAsync(Request,UserId!,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
            
        }
    }
}
