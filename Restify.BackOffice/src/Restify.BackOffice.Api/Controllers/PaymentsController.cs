using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentService paymentService,
        ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene un pago por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.GetByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pago {PaymentId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene los pagos de un pedido
    /// </summary>
    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.GetByOrderIdAsync(orderId, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pagos del pedido {OrderId}", orderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Procesa un pago para un pedido
    /// </summary>
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessOrderPaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.ProcessPaymentAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar pago para pedido {OrderId}", request.OrderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Procesa un pago directo por código de pedido
    /// </summary>
    [HttpPost("process-direct")]
    public async Task<IActionResult> ProcessDirectPayment([FromBody] ProcessDirectPaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.ProcessDirectPaymentAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar pago directo para pedido {OrderCode}", request.OrderCode);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Procesa un pago dividido entre items
    /// </summary>
    [HttpPost("split")]
    public async Task<IActionResult> ProcessSplitPayment([FromBody] SplitPaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.ProcessSplitPaymentAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar pago dividido para pedido {OrderId}", request.OrderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Reembolsa un pago
    /// </summary>
    [HttpPost("{id}/refund")]
    public async Task<IActionResult> RefundPayment(Guid id, [FromBody] RefundPaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.RefundPaymentAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reembolsar pago {PaymentId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
