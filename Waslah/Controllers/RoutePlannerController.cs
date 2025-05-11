using Waslah.Authentication.Filters;

namespace Waslah.Controllers
{
    [Route("api/route/plan")]
    [ApiController]
    public class RoutePlannerController(IChainedRouteServices chainedRoute ) : ControllerBase
    {
        private readonly IChainedRouteServices _chainedRoute = chainedRoute;

        [HttpGet("All")]
        [HasPermission(Permissions.GetRoutes)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var routes = await _chainedRoute.GetAllAsync(cancellationToken);

            return Ok(routes);
        }
        [HttpPost("ByLocation")]
        [Authorize(Roles = DefaultRoles.Member)]
        public async Task<IActionResult> GetByLocationAsync([FromBody] FindRouteRequest Request,
            CancellationToken cancellationToken)
        {
            var UserId = User.GetUserId();
            var result = await _chainedRoute.FindByLocationPointsAsync(Request,UserId!,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
            
        }
        [HttpGet("{Id}/Current-segments")]
        [Authorize(Roles = DefaultRoles.Member)]
        public async Task<IActionResult> GetAllAsync([FromRoute] int Id,CancellationToken cancellationToken)
        {
            var result = await _chainedRoute.GetAllCurrentAsync(Id,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
    }
}

