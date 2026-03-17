using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de movimiento de inventario
/// </summary>
public class InventoryMovementDto
{
    public Guid Id { get; set; }
    public Guid InventoryItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public InventoryMovementType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public decimal PreviousStock { get; set; }
    public decimal NewStock { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime MovementDate { get; set; }
    public string? RegisteredBy { get; set; }
    public bool IsInbound { get; set; }
    public bool IsOutbound { get; set; }
}

/// <summary>
/// DTO resumido para historial
/// </summary>
public class InventoryMovementSummaryDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public InventoryMovementType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal NewStock { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime MovementDate { get; set; }
    public string? RegisteredBy { get; set; }
}
