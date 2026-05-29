using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/v1/inventory")]
[Authorize]
public class StockAlertsController : ControllerBase
{
    private readonly IStockAlertService _alertService;
    private readonly IInventoryService _inventoryService;

    public StockAlertsController(IStockAlertService alertService, IInventoryService inventoryService)
    {
        _alertService = alertService;
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Obtiene todas las alertas de stock bajo actuales
    /// </summary>
    [HttpGet("alerts")]
    public async Task<IActionResult> GetCurrentAlerts(CancellationToken ct)
    {
        var result = await _alertService.GetCurrentAlertsAsync(ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }

    /// <summary>
    /// Actualiza el nivel minimo de stock de un item
    /// </summary>
    [HttpPut("{id:guid}/min-stock")]
    public async Task<IActionResult> UpdateMinStock(Guid id, [FromBody] UpdateMinStockRequest request, CancellationToken ct)
    {
        var itemResult = await _inventoryService.GetItemByIdAsync(id, ct);
        if (!itemResult.IsSuccess)
            return NotFound(new { error = itemResult.Error });

        // Actualizar via upsert con los datos existentes mas el nuevo MinimumStock
        var upsertRequest = new UpsertInventoryItemRequest
        {
            ProductId = itemResult.Data!.ProductId,
            CurrentStock = itemResult.Data.CurrentStock,
            Unit = itemResult.Data.Unit,
            MinimumStock = request.MinStockLevel,
            MaximumStock = itemResult.Data.MaximumStock,
            AverageCost = itemResult.Data.AverageCost,
            StorageLocation = itemResult.Data.StorageLocation,
            TrackStock = itemResult.Data.TrackStock,
            CostMethod = itemResult.Data.CostMethod
        };

        var result = await _inventoryService.UpsertItemAsync(upsertRequest, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Data);
    }
}
