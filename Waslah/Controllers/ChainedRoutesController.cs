
namespace Waslah.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var result = await _chainedRoute.FindByLocationPointsAsync(Request,cancellationToken);

            if (result.IsT0)
                return Ok(result.AsT0);

            var LocationsResult = await _routeServices.GetRouteAsync(result.AsT1,cancellationToken);

            if (LocationsResult.IsSuccess)
            {
                var GeneratedRoute = await _chainedRoute.GetByLocationIdAsync(LocationsResult.Value, cancellationToken);
                Ok(GeneratedRoute);
            }


            return LocationsResult.ToProblem();
            
        }
    }
}
