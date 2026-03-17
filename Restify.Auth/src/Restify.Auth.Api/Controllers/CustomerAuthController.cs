using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.Auth;
using Restify.Auth.Application.DTOs.CustomerAuth;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

/// <summary>
/// Controller para autenticación de clientes (consumidores del restaurante)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomerAuthController : ControllerBase
{
    private readonly ICustomerAuthService _customerAuthService;

    public CustomerAuthController(ICustomerAuthService customerAuthService)
    {
        _customerAuthService = customerAuthService;
    }

    /// <summary>
    /// Registrar un nuevo cliente
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(CustomerLoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] CustomerRegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _customerAuthService.RegisterAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Created("", result.Data);
    }

    /// <summary>
    /// Iniciar sesión como cliente
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(CustomerLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] CustomerLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _customerAuthService.LoginAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Refrescar token de cliente
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] CustomerRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _customerAuthService.RefreshTokenAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error });

        return Ok(result.Data);
    }
}
