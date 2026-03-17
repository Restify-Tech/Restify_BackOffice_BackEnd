using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de item de inventario
/// </summary>
public class InventoryItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public decimal CurrentStock { get; set; }
    public string Unit { get; set; } = "unidades";
    public decimal MinimumStock { get; set; }
    public decimal? MaximumStock { get; set; }
    public decimal AverageCost { get; set; }
    public decimal? LastPurchaseCost { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public DateTime? LastSaleDate { get; set; }
    public string? StorageLocation { get; set; }
    public bool TrackStock { get; set; }
    public StockCostMethod CostMethod { get; set; }
    public string CostMethodName { get; set; } = string.Empty;
    
    // Propiedades calculadas
    public bool IsLowStock { get; set; }
    public bool IsOutOfStock { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO resumido para listas
/// </summary>
public class InventoryItemSummaryDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public string Unit { get; set; } = "unidades";
    public decimal MinimumStock { get; set; }
    public decimal AverageCost { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsOutOfStock { get; set; }
    public decimal TotalValue { get; set; }
}

/// <summary>
/// Request para crear/actualizar item de inventario
/// </summary>
public class UpsertInventoryItemRequest
{
    public Guid ProductId { get; set; }
    public decimal CurrentStock { get; set; }
    public string Unit { get; set; } = "unidades";
    public decimal MinimumStock { get; set; }
    public decimal? MaximumStock { get; set; }
    public decimal AverageCost { get; set; }
    public string? StorageLocation { get; set; }
    public bool TrackStock { get; set; } = true;
    public StockCostMethod CostMethod { get; set; } = StockCostMethod.WeightedAverage;
}

/// <summary>
/// Request para ajuste de inventario
/// </summary>
public class AdjustInventoryRequest
{
    public Guid InventoryItemId { get; set; }
    public decimal Quantity { get; set; }  // Positivo = entrada, Negativo = salida
    public decimal? UnitCost { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

/// <summary>
/// Estadísticas de inventario
/// </summary>
public class InventoryStatisticsDto
{
    public int TotalItems { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public List<InventoryItemSummaryDto> LowStockAlerts { get; set; } = new();
    public List<InventoryItemSummaryDto> OutOfStockAlerts { get; set; } = new();
}
