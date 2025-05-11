using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Waslah.Authentication.Filters;

namespace Waslah.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    [HttpGet("")]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled, CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllAsync(includeDisabled, cancellationToken);

        return Ok(roles);
    }
    [HttpGet("{id}")]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetDetails([FromRoute] string id)
    {
        var result = await _roleService.GetDetailsAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("")]
    [HasPermission(Permissions.CreateRoles)]
    public async Task<IActionResult> Create([FromBody] RoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _roleService.CreateAsyc(request, cancellationToken);

        return result.IsSuccess
             ? CreatedAtAction(nameof(GetDetails), new { result.Value.Id }, result.Value)
             : result.ToProblem();
    }
    [HttpPut("{roleId}")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> Update([FromRoute] string roleId, [FromBody] RoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _roleService.UpdateAsync(roleId, request, cancellationToken);

        return result.IsSuccess
             ? NoContent()
            : result.ToProblem();
    }
    [HttpPut("{roleId}/toggle-status")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> ToggleStatus([FromRoute] string roleId)
    {
        var result = await _roleService.ToggleStatusAsync(roleId);

        return result.IsSuccess
             ? NoContent()
            : result.ToProblem();
    }
}
