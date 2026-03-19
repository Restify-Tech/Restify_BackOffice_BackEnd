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

    /// <summary>
    /// Obtener branding publico por RUC o cedula (para login 2 pasos)
    /// </summary>
    [AllowAnonymous]
    [HttpGet("branding/by-identification/{identificationNumber}")]
    public async Task<IActionResult> GetBrandingByIdentification(string identificationNumber, CancellationToken cancellationToken)
    {
        var result = await _service.GetBrandingByIdentificationAsync(identificationNumber, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtener branding publico por slug (para menu QR)
    /// </summary>
    [AllowAnonymous]
    [HttpGet("branding/{slug}")]
    public async Task<IActionResult> GetBrandingBySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await _service.GetBrandingBySlugAsync(slug, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtener branding del tenant por ID (admin)
    /// </summary>
    [HttpGet("{id:guid}/branding")]
    public async Task<IActionResult> GetBrandingById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetBrandingByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Actualizar branding del tenant
    /// </summary>
    [HttpPut("{id:guid}/branding")]
    public async Task<IActionResult> UpdateBranding(Guid id, [FromBody] UpdateTenantBrandingRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateBrandingAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Contains("no encontrad") == true
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Subir logo del tenant
    /// </summary>
    [HttpPost("{id:guid}/logo")]
    public async Task<IActionResult> UploadLogo(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "Debe seleccionar un archivo" });

        using var stream = file.OpenReadStream();
        var result = await _service.UploadLogoAsync(id, stream, file.FileName, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Contains("no encontrad") == true
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });

        return Ok(new { url = result.Data });
    }

    /// <summary>
    /// Subir imagen de portada del tenant
    /// </summary>
    [HttpPost("{id:guid}/cover-image")]
    public async Task<IActionResult> UploadCoverImage(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "Debe seleccionar un archivo" });

        using var stream = file.OpenReadStream();
        var result = await _service.UploadCoverImageAsync(id, stream, file.FileName, cancellationToken);

        if (!result.IsSuccess)
            return result.Error?.Contains("no encontrad") == true
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });

        return Ok(new { url = result.Data });
    }
}
