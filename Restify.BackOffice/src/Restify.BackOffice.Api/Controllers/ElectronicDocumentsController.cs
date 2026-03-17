using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ElectronicDocumentsController : ControllerBase
{
    private readonly IElectronicInvoiceService _service;
    private readonly ILogger<ElectronicDocumentsController> _logger;

    public ElectronicDocumentsController(
        IElectronicInvoiceService service,
        ILogger<ElectronicDocumentsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Lista documentos electronicos con filtros
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ElectronicDocumentFilter filter, CancellationToken ct)
    {
        try
        {
            var result = await _service.GetAllAsync(filter, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener documentos electrónicos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un documento electronico por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _service.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener documento electrónico {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Emite factura electronica desde una factura existente
    /// </summary>
    [HttpPost("invoices/{invoiceId}/emit")]
    public async Task<IActionResult> EmitInvoice(Guid invoiceId, CancellationToken ct)
    {
        try
        {
            var result = await _service.EmitElectronicInvoiceAsync(invoiceId, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al emitir factura electrónica para {InvoiceId}", invoiceId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Emite nota de credito electronica
    /// </summary>
    [HttpPost("credit-notes")]
    public async Task<IActionResult> EmitCreditNote([FromBody] CreateCreditNoteRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.EmitCreditNoteAsync(request, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al emitir nota de crédito para factura {InvoiceId}", request.InvoiceId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Emite comprobante de retencion electronico
    /// </summary>
    [HttpPost("withholdings")]
    public async Task<IActionResult> EmitWithholding([FromBody] CreateWithholdingRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.EmitWithholdingVoucherAsync(request, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al emitir comprobante de retención");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Consulta autorizacion de un documento en el SRI
    /// </summary>
    [HttpPost("{id}/check-authorization")]
    public async Task<IActionResult> CheckAuthorization(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _service.CheckAuthorizationAsync(id, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar autorización del documento {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Reenvia un documento al SRI
    /// </summary>
    [HttpPost("{id}/resend")]
    public async Task<IActionResult> Resend(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _service.ResendDocumentAsync(id, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reenviar documento {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Descarga PDF RIDE del documento
    /// </summary>
    [HttpGet("{id}/ride")]
    public async Task<IActionResult> GetRide(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _service.GetRidePdfAsync(id, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return File(result.Data!, "application/pdf", $"RIDE-{id}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar RIDE del documento {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
