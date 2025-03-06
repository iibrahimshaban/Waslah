using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Waslah.Controllers
{
    [Route("api/ChainedRoute/{CRId}/[controller]")]
    [ApiController]
    public class GeneratedRoutesController(IRouteGeneratorServices routeGenerator) : ControllerBase
    {
        private readonly IRouteGeneratorServices _routeGenerator = routeGenerator;

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromRoute] int CRId,CancellationToken cancellationToken)
        {
            var result = await _routeGenerator.GetAllAsync(CRId,cancellationToken);

            return result.IsSuccess 
                ? Ok(result.Value) 
                : result.ToProblem();
        }
    }
}
