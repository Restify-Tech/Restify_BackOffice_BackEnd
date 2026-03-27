using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IBenefitHubClient _benefitHubClient;
    private readonly IOrderRepository _orderRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentService paymentService,
        IBenefitHubClient benefitHubClient,
        IOrderRepository orderRepository,
        IConfiguration configuration,
        ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _benefitHubClient = benefitHubClient;
        _orderRepository = orderRepository;
        _configuration = configuration;
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

    /// <summary>
    /// Simula beneficios disponibles antes de confirmar el pago de un pedido
    /// GET /api/payments/benefit-preview/{orderId}?couponCode=XXX
    /// </summary>
    [HttpGet("benefit-preview/{orderId}")]
    public async Task<IActionResult> GetBenefitPreview(Guid orderId, [FromQuery] string? couponCode, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
                return NotFound("Pedido no encontrado");

            var externalCustomerId = order.CustomerId?.ToString()
                ?? order.CustomerPhone
                ?? "guest";

            var request = new BenefitRequest(
                _configuration["BenefitHub:TenantSourceId"] ?? "",
                externalCustomerId,
                order.Id.ToString(),
                order.Items.Select(i => new BenefitItem(
                    i.ProductId.ToString(),
                    i.Product?.Name ?? "",
                    i.UnitPrice,
                    i.Quantity
                )).ToList(),
                order.Total,
                couponCode
            );

            var result = await _benefitHubClient.SimulateAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener preview de beneficios para pedido {OrderId}", orderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
