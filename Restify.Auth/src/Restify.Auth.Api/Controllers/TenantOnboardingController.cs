using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.Auth.Application.DTOs.Tenant;
using Restify.Auth.Application.Interfaces;

namespace Restify.Auth.Api.Controllers;

/// <summary>
/// Controller para el proceso de onboarding del tenant.
/// Requiere autenticación.
/// </summary>
[ApiController]
[Authorize]
[Route("api/tenant/onboarding")]
public class TenantOnboardingController : ControllerBase
{
    private readonly ITenantOnboardingService _onboardingService;
    private readonly ICurrentUserService _currentUserService;

    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public TenantOnboardingController(
        ITenantOnboardingService onboardingService,
        ICurrentUserService currentUserService)
    {
        _onboardingService = onboardingService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Actualizar datos de onboarding del tenant
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateOnboarding(
        [FromBody] TenantOnboardingRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized(new { error = "No se pudo determinar el tenant del usuario" });

        var result = await _onboardingService.UpdateOnboardingAsync(tenantId.Value, request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Subir logo del restaurante
    /// </summary>
    [HttpPost("logo")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadLogo(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized(new { error = "No se pudo determinar el tenant del usuario" });

        var validationError = ValidateImageFile(file);
        if (validationError != null)
            return BadRequest(new { error = validationError });

        using var stream = file.OpenReadStream();
        var result = await _onboardingService.UploadLogoAsync(tenantId.Value, stream, file.FileName, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { url = result.Data });
    }

    /// <summary>
    /// Subir firma electrónica del restaurante
    /// </summary>
    [HttpPost("signature")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadSignature(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized(new { error = "No se pudo determinar el tenant del usuario" });

        var validationError = ValidateImageFile(file);
        if (validationError != null)
            return BadRequest(new { error = validationError });

        using var stream = file.OpenReadStream();
        var result = await _onboardingService.UploadSignatureAsync(tenantId.Value, stream, file.FileName, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { url = result.Data });
    }

    private static string? ValidateImageFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return "El archivo es requerido";

        if (file.Length > MaxFileSize)
            return "El archivo no puede exceder 5 MB";

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension))
            return "Solo se permiten imágenes en formato JPG, PNG o WebP";

        return null;
    }
}
