using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/v1/split-payments")]
[Authorize]
public class SplitPaymentsController : ControllerBase
{
    private readonly ISplitPaymentService _splitPaymentService;
    private readonly ILogger<SplitPaymentsController> _logger;

    public SplitPaymentsController(
        ISplitPaymentService splitPaymentService,
        ILogger<SplitPaymentsController> logger)
    {
        _splitPaymentService = splitPaymentService;
        _logger = logger;
    }

    /// <summary>
    /// Inicia un pago dividido para un pedido entre N personas o metodos de pago
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Initiate([FromBody] CreateSplitPaymentRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _splitPaymentService.InitiateAsync(request, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar pago dividido para pedido {OrderId}", request.OrderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Procesa (paga) un item especifico de un pago dividido por su indice
    /// </summary>
    [HttpPost("{id:guid}/items/{index:int}/pay")]
    public async Task<IActionResult> ProcessItem(Guid id, int index, [FromBody] ProcessSplitItemPayBody body, CancellationToken ct)
    {
        try
        {
            var request = new ProcessSplitItemRequest(id, index, body.PaymentMethod, body.Amount);
            var result = await _splitPaymentService.ProcessItemAsync(request, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar item {Index} del pago dividido {Id}", index, id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el pago dividido activo de un pedido
    /// </summary>
    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(Guid orderId, CancellationToken ct)
    {
        try
        {
            var result = await _splitPaymentService.GetByOrderAsync(orderId, ct);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pago dividido del pedido {OrderId}", orderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Cancela un pago dividido activo
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _splitPaymentService.CancelAsync(id, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cancelar pago dividido {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}

/// <summary>
/// Body para procesar un item de pago dividido
/// </summary>
public record ProcessSplitItemPayBody(Domain.Entities.PaymentMethodType PaymentMethod, decimal Amount);
