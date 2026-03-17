using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.DriverAuth;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

/// <summary>
/// Controller para autenticación de motorizados (repartidores de delivery)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DriverAuthController : ControllerBase
{
    private readonly IDriverAuthService _driverAuthService;

    public DriverAuthController(IDriverAuthService driverAuthService)
    {
        _driverAuthService = driverAuthService;
    }

    /// <summary>
    /// Registrar un nuevo motorizado
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(DriverLoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] DriverRegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _driverAuthService.RegisterAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Created("", result.Data);
    }

    /// <summary>
    /// Iniciar sesión como motorizado
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(DriverLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] DriverLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _driverAuthService.LoginAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Refrescar token de motorizado
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] DriverRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _driverAuthService.RefreshTokenAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error });

        return Ok(result.Data);
    }
}
