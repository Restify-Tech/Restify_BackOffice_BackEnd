using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Tenant;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantManagementService _service;
    private readonly ICurrentUserService _currentUser;

    public TenantsController(ITenantManagementService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.GetAllAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Contains("no encontrad") == true
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTenantStatusRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.UpdateStatusAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Contains("no encontrad") == true
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });

        return NoContent();
    }

    [HttpPatch("{id:guid}/delivery-mode")]
    public async Task<IActionResult> UpdateDeliveryMode(Guid id, [FromBody] UpdateTenantDeliveryModeRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsSuperAdmin)
            return Forbid();

        var result = await _service.UpdateDeliveryModeAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Contains("no encontrad") == true
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });

        return NoContent();
    }
}
