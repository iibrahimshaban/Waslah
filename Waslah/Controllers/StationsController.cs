
using Azure.Core;
using Waslah.Authentication.Filters;

namespace Waslah.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController(IStationServices stationServices) : ControllerBase
    {
        private readonly IStationServices _stationServices= stationServices;

        [HttpGet("info")]
        [HasPermission(Permissions.GetStations)]
        public async Task<IActionResult> Start(CancellationToken cancellationToken)
        {
            var stations = await _stationServices.GetAllStationsInfoAsync(cancellationToken);

            return Ok(stations);
        }
        [HttpGet("All")]
        [HasPermission(Permissions.GetStations)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var stations = await _stationServices.GetAllAsync(cancellationToken);

            return Ok(stations);
        }
        [HttpGet("{locid}")]
        [HasPermission(Permissions.GetStations)]
        public async Task<IActionResult> GetByLocationId(int locid, CancellationToken cancellationToken)
        {
            
            var result = await _stationServices.GetByLocationIdAsync(locid,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
        [HttpPost("Nearest")]
        [Authorize(Roles = DefaultRoles.Member)]
        public async Task<IActionResult> GetByNearestStationsAsync([FromBody] FindStationRequest request,
            CancellationToken cancellationToken)
        {
            string[] parts = request.Point.Split(',');

            var Latitude = double.Parse(parts[0]);
            var Longitude = double.Parse(parts[1]);

            var stations = await _stationServices.GetNearestStationsAsync(Latitude, Longitude,10 ,cancellationToken);

            if (!stations.Any())
                return BadRequest("you are far from all the stations in our system");

            return Ok(stations);
        }
        [HttpPost("")]
        [HasPermission(Permissions.CreateStations)]
        public async Task<IActionResult> Create([FromBody] StationRequest request,
            CancellationToken cancellationToken)
        {

            var result = await _stationServices.CreateAsync(request ,cancellationToken);

            return result.IsSuccess
            ? CreatedAtAction(nameof(GetByLocationId),new { locid = result.Value.StationId},result.Value)
            : result.ToProblem();
        }
        [HttpPut("{Id}")]
        [HasPermission(Permissions.UpdateStations)]
        public async Task<IActionResult> Update([FromRoute]int Id, [FromBody] StationRequest request,CancellationToken cancellationToken)
        {
            var result = await _stationServices.UpdateAsync(Id,request,cancellationToken);

            return result.IsSuccess 
                ? NoContent() 
                : result.ToProblem();
        }
        [HttpPut("{Id}/toggle-status")]
        [HasPermission(Permissions.UpdateStations)]
        public async Task<IActionResult> ToggleStatus([FromRoute]int Id,CancellationToken cancellationToken)
        {
            var result = await _stationServices.ToggleStatusAsync(Id,cancellationToken);

            return result.IsSuccess 
                ? NoContent() 
                : result.ToProblem();
        }
    }
}
