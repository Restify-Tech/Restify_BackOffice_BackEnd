using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.Tenant;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

/// <summary>
/// Controller público para registro de nuevos restaurantes (tenants).
/// No requiere autenticación.
/// </summary>
[ApiController]
[Route("api/tenants/register")]
public class TenantRegistrationController : ControllerBase
{
    private readonly ITenantRegistrationService _registrationService;

    public TenantRegistrationController(ITenantRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    /// <summary>
    /// Registrar un nuevo restaurante
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TenantRegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] TenantRegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _registrationService.RegisterAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return StatusCode(StatusCodes.Status201Created, result.Data);
    }
}
