using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderService orderService,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los pedidos
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.GetAllAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pedidos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un pedido por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.GetByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pedido {OrderId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un pedido por número de orden
    /// </summary>
    [HttpGet("number/{orderNumber}")]
    public async Task<IActionResult> GetByOrderNumber(string orderNumber, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.GetByOrderNumberAsync(orderNumber, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pedido {OrderNumber}", orderNumber);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene pedidos por estado
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(OrderStatus status, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.GetByStatusAsync(status, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pedidos por estado {Status}", status);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene pedidos de una mesa
    /// </summary>
    [HttpGet("table/{tableId}")]
    public async Task<IActionResult> GetByTableId(Guid tableId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.GetByTableIdAsync(tableId, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pedidos de mesa {TableId}", tableId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene pedidos activos
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveOrders(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.GetActiveOrdersAsync(cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pedidos activos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo pedido
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear pedido");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza el estado de un pedido
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.UpdateStatusAsync(id, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de pedido {OrderId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza el estado de un item del pedido
    /// </summary>
    [HttpPatch("{orderId}/items/{itemId}/status")]
    public async Task<IActionResult> UpdateItemStatus(
        Guid orderId, 
        Guid itemId, 
        [FromBody] UpdateOrderItemStatusRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.UpdateItemStatusAsync(orderId, itemId, request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de item {ItemId} del pedido {OrderId}", itemId, orderId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina un pedido
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.DeleteAsync(id, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar pedido {OrderId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
