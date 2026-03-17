using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.PoolDriverAuth;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

[ApiController]
[Route("api/pool-drivers")]
public class PoolDriverAuthController : ControllerBase
{
    private readonly IPoolDriverAuthService _service;

    public PoolDriverAuthController(IPoolDriverAuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(PoolDriverLoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] PoolDriverRegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.RegisterAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Created("", result.Data);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(PoolDriverLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] PoolDriverLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.LoginAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] PoolDriverRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.RefreshTokenAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error });

        return Ok(result.Data);
    }
}
