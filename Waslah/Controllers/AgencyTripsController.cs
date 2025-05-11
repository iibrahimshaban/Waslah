using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Waslah.Authentication.Filters;

namespace Waslah.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AgencyTripsController(IAgencyTripService agencyTripService) : ControllerBase
{
    private readonly IAgencyTripService _agencyTripService = agencyTripService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var trips = await _agencyTripService.GetAllAsync(cancellationToken);

        return Ok(trips);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var result = await _agencyTripService.GetByIdAsync(Id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value) 
            : result.ToProblem();
    }
    [HttpPost("")]
    [HasPermission(Permissions.CreateAgencyTrips)]
    public async Task<IActionResult> Craete([FromBody] AgencyRequest request ,CancellationToken cancellationToken)
    {
        var result = await _agencyTripService.CreateAsync(request , cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById),new {result.Value.Id},result.Value)
            : result.ToProblem();
    }
    [HttpPut("{Id}")]
    [HasPermission(Permissions.UpdateAgencyTrips)]
    public async Task<IActionResult> Update([FromRoute] int Id,[FromBody] AgencyRequest request,CancellationToken cancellationToken)
    {
        var result = await _agencyTripService.UpdateAsync(Id, request, cancellationToken);

        return result.IsSuccess
            ? NoContent() 
            : result.ToProblem();
    }
    [HttpPut("{Id}/photo")]
    [HasPermission(Permissions.UpdateAgencyTrips)]
    public async Task<IActionResult> UpdatePhoto([FromRoute] int Id,[FromForm] AgencyPhotoRequest request,CancellationToken cancellationToken)
    {
        var result = await _agencyTripService.UpdateAgencyPhotoAsync(Id, request, cancellationToken);

        return result.IsSuccess
            ? NoContent() 
            : result.ToProblem();
    }
    [HttpPut("{Id}/toggle-status")]
    [HasPermission(Permissions.UpdateAgencyTrips)]
    public async Task<IActionResult> Update([FromRoute] int Id,CancellationToken cancellationToken)
    {
        var result = await _agencyTripService.ToggleStatusAsync(Id, cancellationToken);

        return result.IsSuccess
            ? NoContent() 
            : result.ToProblem();
    }
}
