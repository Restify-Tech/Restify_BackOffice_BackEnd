using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FiscalConfigurationController : ControllerBase
{
    private readonly IFiscalConfigurationService _service;
    private readonly ILogger<FiscalConfigurationController> _logger;

    public FiscalConfigurationController(
        IFiscalConfigurationService service,
        ILogger<FiscalConfigurationController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la configuracion fiscal del tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        try
        {
            var result = await _service.GetAsync(ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuración fiscal");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Guarda la configuracion fiscal del tenant
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Save([FromBody] SaveFiscalConfigurationRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.SaveAsync(request, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar configuración fiscal");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Sube certificado digital .p12
    /// </summary>
    [HttpPost("certificate")]
    public async Task<IActionResult> UploadCertificate(IFormFile certificate, [FromForm] string password, CancellationToken ct)
    {
        try
        {
            if (certificate == null || certificate.Length == 0)
                return BadRequest("El archivo de certificado es requerido");

            using var stream = certificate.OpenReadStream();
            var result = await _service.UploadCertificateAsync(stream, password, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir certificado digital");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Valida el certificado digital configurado
    /// </summary>
    [HttpGet("certificate/validate")]
    public async Task<IActionResult> ValidateCertificate(CancellationToken ct)
    {
        try
        {
            var result = await _service.ValidateCertificateAsync(ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(new { valid = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar certificado digital");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
