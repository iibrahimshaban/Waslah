using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Waslah.Services;

namespace Waslah.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] AuthRequest request, CancellationToken cancellationToken = default)
        {
            var LoginResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

            return LoginResult.IsSuccess
                ? Ok(LoginResult.Value)
                : LoginResult.ToProblem();
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest refresh,
        CancellationToken cancellationToken = default)
        {
            var AuthResult = await _authService.GetRefreshTokenAsync(refresh.Token
                , refresh.RefreshToken, cancellationToken);

            return AuthResult.IsSuccess
                ? Ok(AuthResult.Value)
                : AuthResult.ToProblem();
        }
        [HttpPost("Revoke-Refresh-Token")]
        public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest refresh,
        CancellationToken cancellationToken = default)
        {
            var IsRevoked = await _authService.RevokeRefreshTokenAsync(refresh.Token,
                refresh.RefreshToken, cancellationToken);

            return IsRevoked.IsSuccess
                ? NoContent()
                : IsRevoked.ToProblem();
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] RegistrationRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegistrationAsync(request, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
    }
}
