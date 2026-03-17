using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;

    public InventoryController(IInventoryService service)
    {
        _service = service;
    }

    // ===== INVENTORY ITEMS =====

    /// <summary>
    /// Obtiene todos los items de inventario
    /// </summary>
    [HttpGet("items")]
    public async Task<IActionResult> GetAllItems(CancellationToken ct)
    {
        var result = await _service.GetAllItemsAsync(ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene items con stock bajo (alertas)
    /// </summary>
    [HttpGet("items/low-stock")]
    public async Task<IActionResult> GetLowStock(CancellationToken ct)
    {
        var result = await _service.GetLowStockItemsAsync(ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene un item de inventario por ID
    /// </summary>
    [HttpGet("items/{id:guid}")]
    public async Task<IActionResult> GetItemById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetItemByIdAsync(id, ct);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene item de inventario por ID de producto
    /// </summary>
    [HttpGet("items/by-product/{productId:guid}")]
    public async Task<IActionResult> GetByProduct(Guid productId, CancellationToken ct)
    {
        var result = await _service.GetItemByProductIdAsync(productId, ct);
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea o actualiza un item de inventario
    /// </summary>
    [HttpPost("items")]
    public async Task<IActionResult> UpsertItem([FromBody] UpsertInventoryItemRequest request, CancellationToken ct)
    {
        var result = await _service.UpsertItemAsync(request, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Elimina un item de inventario
    /// </summary>
    [HttpDelete("items/{id:guid}")]
    public async Task<IActionResult> DeleteItem(Guid id, CancellationToken ct)
    {
        var result = await _service.DeleteItemAsync(id, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    // ===== ADJUSTMENTS =====

    /// <summary>
    /// Ajusta el stock de un item (entrada/salida manual)
    /// </summary>
    [HttpPost("items/adjust")]
    public async Task<IActionResult> AdjustStock([FromBody] AdjustInventoryRequest request, CancellationToken ct)
    {
        var result = await _service.AdjustStockAsync(request, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    // ===== MOVEMENTS =====

    /// <summary>
    /// Obtiene movimientos de inventario de un producto específico
    /// </summary>
    [HttpGet("movements/product/{productId:guid}")]
    public async Task<IActionResult> GetMovementsByProduct(
        Guid productId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _service.GetMovementsByProductAsync(productId, from, to, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene los movimientos más recientes
    /// </summary>
    [HttpGet("movements/recent")]
    public async Task<IActionResult> GetRecentMovements([FromQuery] int limit = 50, CancellationToken ct = default)
    {
        var result = await _service.GetRecentMovementsAsync(limit, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    // ===== STATISTICS =====

    /// <summary>
    /// Obtiene estadísticas generales del inventario
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics(CancellationToken ct)
    {
        var result = await _service.GetStatisticsAsync(ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }
}
