using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Waslah.Authentication.Filters;

namespace Waslah.Controllers
{
    [Route("api/route/segment")]
    [ApiController]
    public class RouteSegmentsController(IRouteGeneratorServices routeGenerator) : ControllerBase
    {
        private readonly IRouteGeneratorServices _routeGenerator = routeGenerator;

        [HttpGet("{Id}")]
        [HasPermission(Permissions.GetRoutes)]
        public async Task<IActionResult> GetById([FromRoute] int Id,CancellationToken cancellationToken)
        {
            var result = await _routeGenerator.GetByIdAsync(Id,cancellationToken);

            return result.IsSuccess 
                ? Ok(result.Value) 
                : result.ToProblem();
        }
        [HttpPost("")]
        [HasPermission(Permissions.CreateRoutes)]
        public async Task<IActionResult> Create([FromBody] RouteRequest request ,CancellationToken cancellationToken)
        {
            var result = await _routeGenerator.CreateAsync(request,cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById),new {result.Value.Id },result.Value)
                : result.ToProblem();
        }
        [HttpPut("{Id}")]
        [HasPermission(Permissions.UpdateRoutes)]
        public async Task<IActionResult> Update([FromRoute]int Id,[FromBody] RouteRequest request ,CancellationToken cancellationToken)
        {
            var result = await _routeGenerator.UpdateAsync(Id,request,cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }
        [HttpPut("{Id}/toggle-status")]
        [HasPermission(Permissions.UpdateRoutes)]
        public async Task<IActionResult> ToggleStatus([FromRoute]int Id,CancellationToken cancellationToken)
        {
            var result = await _routeGenerator.ToggleStatusAsync(Id,cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }
        
    }
}
