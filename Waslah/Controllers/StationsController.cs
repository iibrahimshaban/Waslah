
using Azure.Core;

namespace Waslah.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController(IStationServices stationServices) : ControllerBase
    {
        private readonly IStationServices _stationServices= stationServices;

        [HttpGet("All")]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await _stationServices.GetAllAsync(cancellationToken);

            return result.IsSuccess 
                ? Ok(result.Value)
                : result.ToProblem();
        }
        [HttpGet("{locid}")]
        public async Task<IActionResult> GetByLocationId(int locid, CancellationToken cancellationToken)
        {
            
            var result = await _stationServices.GetByLocationIdAsync(locid,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
        [HttpPost("Nearest")]
        public async Task<IActionResult> GetByNearestStationsAsync([FromBody] StationRequest request,
            CancellationToken cancellationToken)
        {
            string[] parts = request.Point.Split(',');

            var Latitude = double.Parse(parts[0]);
            var Longitude = double.Parse(parts[1]);

            var stations = await _stationServices.GetNearestStationsAsync(Latitude, Longitude,10 ,cancellationToken);

            if (!stations.Any())
                return BadRequest("you are far from all the stations in our system");

            return Ok(stations.Adapt<IEnumerable<StationDistanceResponse>>());
        }
    }
}
