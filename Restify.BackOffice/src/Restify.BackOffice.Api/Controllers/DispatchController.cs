using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DispatchController : ControllerBase
{
    private readonly IDispatchService _dispatchService;
    private readonly ILogger<DispatchController> _logger;

    public DispatchController(
        IDispatchService dispatchService,
        ILogger<DispatchController> logger)
    {
        _dispatchService = dispatchService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la cola de despacho (pedidos listos y esperando aprobación)
    /// </summary>
    [HttpGet("queue")]
    public async Task<IActionResult> GetDispatchQueue(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dispatchService.GetDispatchQueueAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cola de despacho");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el estado de despacho de un pedido
    /// </summary>
    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dispatchService.GetByOrderIdAsync(orderId, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estado de despacho del pedido {OrderId}", orderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Aprueba el despacho de un pedido
    /// </summary>
    [HttpPost("{orderId}/approve")]
    public async Task<IActionResult> ApproveDispatch(Guid orderId, [FromBody] ApproveDispatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dispatchService.ApproveDispatchAsync(orderId, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aprobar despacho del pedido {OrderId}", orderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Rechaza el despacho de un pedido
    /// </summary>
    [HttpPost("{orderId}/reject")]
    public async Task<IActionResult> RejectDispatch(Guid orderId, [FromBody] RejectDispatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dispatchService.RejectDispatchAsync(orderId, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al rechazar despacho del pedido {OrderId}", orderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
