using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Waslah.Services;

namespace Waslah.Controllers;
[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService,IOrderService orderService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IOrderService _orderService = orderService;

    [HttpGet("")]
    public async Task<IActionResult> Info()
    {
        var profile = await _userService.GetProfileAsync(User.GetUserId()!);

        return Ok(profile);
    }
    [HttpPut("")]
    public async Task<IActionResult> Update([FromForm] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateProfileAsync(User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpGet("pervious-rides")]
    public async Task<IActionResult> GetPervious(CancellationToken cancellationToken)
    {
        var Rides = await _orderService.GetAllAsync(User.GetUserId()!, cancellationToken: cancellationToken);
        return Ok(Rides);
    }
}
