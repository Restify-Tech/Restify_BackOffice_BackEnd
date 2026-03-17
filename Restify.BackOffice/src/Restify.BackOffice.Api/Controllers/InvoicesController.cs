using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IInvoiceService invoiceService,
        ILogger<InvoicesController> logger)
    {
        _invoiceService = invoiceService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las facturas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.GetAllAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener facturas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene una factura por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.GetByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener factura {InvoiceId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene una factura por número
    /// </summary>
    [HttpGet("number/{invoiceNumber}")]
    public async Task<IActionResult> GetByInvoiceNumber(string invoiceNumber, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.GetByInvoiceNumberAsync(invoiceNumber, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener factura {InvoiceNumber}", invoiceNumber);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene la factura de un pedido
    /// </summary>
    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.GetByOrderIdAsync(orderId, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener factura del pedido {OrderId}", orderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene facturas por estado
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(InvoiceStatus status, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.GetByStatusAsync(status, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener facturas por estado {Status}", status);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene facturas por método de pago
    /// </summary>
    [HttpGet("payment-method/{paymentMethod}")]
    public async Task<IActionResult> GetByPaymentMethod(PaymentMethod paymentMethod, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.GetByPaymentMethodAsync(paymentMethod, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener facturas por método de pago {PaymentMethod}", paymentMethod);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene facturas por rango de fechas
    /// </summary>
    [HttpGet("date-range")]
    public async Task<IActionResult> GetByDateRange(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.GetByDateRangeAsync(from, to, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener facturas entre {From} y {To}", from, to);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea una nueva factura desde un pedido
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear factura para pedido {OrderId}", request.OrderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza información de una factura
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.UpdateAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar factura {InvoiceId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Procesa el pago de una factura
    /// </summary>
    [HttpPost("{id}/process-payment")]
    public async Task<IActionResult> ProcessPayment(
        Guid id,
        [FromBody] ProcessPaymentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.ProcessPaymentAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar pago de factura {InvoiceId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Anula una factura
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.CancelAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al anular factura {InvoiceId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina una factura
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar factura {InvoiceId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene estadísticas de facturas
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _invoiceService.GetStatisticsAsync(from, to, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de facturas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // TODO: Implementar endpoints para impresión (PDF)
    // [HttpGet("{id}/print")]
    // public async Task<IActionResult> PrintInvoice(Guid id, CancellationToken cancellationToken)
    // {
    //     // Generar PDF de la factura
    // }

    // TODO: Implementar endpoint para pre-cuenta
    // [HttpGet("pre-bill/{orderId}")]
    // public async Task<IActionResult> PrintPreBill(Guid orderId, CancellationToken cancellationToken)
    // {
    //     // Generar PDF de pre-cuenta sin crear factura
    // }
}
