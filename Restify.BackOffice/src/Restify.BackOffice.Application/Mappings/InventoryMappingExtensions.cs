using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class InventoryMappingExtensions
{
    // Supplier
    public static SupplierDto ToDto(this Supplier entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        TaxId = entity.TaxId,
        ContactName = entity.ContactName,
        Email = entity.Email,
        Phone = entity.Phone,
        Address = entity.Address,
        Notes = entity.Notes,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt
    };

    // InventoryItem
    public static InventoryItemDto ToDto(this InventoryItem entity) => new()
    {
        Id = entity.Id,
        ProductId = entity.ProductId,
        ProductName = entity.Product?.Name ?? string.Empty,
        ProductSku = entity.Product?.Sku,
        CurrentStock = entity.CurrentStock,
        Unit = entity.Unit,
        MinimumStock = entity.MinimumStock,
        MaximumStock = entity.MaximumStock,
        AverageCost = entity.AverageCost,
        LastPurchaseCost = entity.LastPurchaseCost,
        LastPurchaseDate = entity.LastPurchaseDate,
        LastSaleDate = entity.LastSaleDate,
        StorageLocation = entity.StorageLocation,
        TrackStock = entity.TrackStock,
        CostMethod = entity.CostMethod,
        CostMethodName = entity.CostMethod.ToString(),
        IsLowStock = entity.IsLowStock,
        IsOutOfStock = entity.IsOutOfStock,
        TotalValue = entity.TotalValue,
        CreatedAt = entity.CreatedAt
    };

    public static InventoryItemSummaryDto ToSummaryDto(this InventoryItem entity) => new()
    {
        Id = entity.Id,
        ProductId = entity.ProductId,
        ProductName = entity.Product?.Name ?? string.Empty,
        CurrentStock = entity.CurrentStock,
        Unit = entity.Unit,
        MinimumStock = entity.MinimumStock,
        AverageCost = entity.AverageCost,
        IsLowStock = entity.IsLowStock,
        IsOutOfStock = entity.IsOutOfStock,
        TotalValue = entity.TotalValue
    };

    // InventoryMovement
    public static InventoryMovementDto ToDto(this InventoryMovement entity) => new()
    {
        Id = entity.Id,
        InventoryItemId = entity.InventoryItemId,
        ProductId = entity.InventoryItem?.ProductId ?? Guid.Empty,
        ProductName = entity.InventoryItem?.Product?.Name ?? string.Empty,
        Type = entity.Type,
        TypeName = entity.Type.ToString(),
        Quantity = entity.Quantity,
        UnitCost = entity.UnitCost,
        TotalCost = entity.TotalCost,
        PreviousStock = entity.PreviousStock,
        NewStock = entity.NewStock,
        ReferenceId = entity.ReferenceId,
        ReferenceType = entity.ReferenceType,
        Description = entity.Description,
        Notes = entity.Notes,
        MovementDate = entity.MovementDate,
        RegisteredBy = entity.RegisteredBy,
        IsInbound = entity.IsInbound,
        IsOutbound = entity.IsOutbound
    };

    public static InventoryMovementSummaryDto ToSummaryDto(this InventoryMovement entity) => new()
    {
        Id = entity.Id,
        ProductName = entity.InventoryItem?.Product?.Name ?? string.Empty,
        Type = entity.Type,
        TypeName = entity.Type.ToString(),
        Quantity = entity.Quantity,
        NewStock = entity.NewStock,
        Description = entity.Description,
        MovementDate = entity.MovementDate,
        RegisteredBy = entity.RegisteredBy
    };

    // PurchaseOrder
    public static PurchaseOrderDto ToDto(this PurchaseOrder entity) => new()
    {
        Id = entity.Id,
        OrderNumber = entity.OrderNumber,
        SupplierId = entity.SupplierId,
        SupplierName = entity.Supplier?.Name ?? string.Empty,
        OrderDate = entity.OrderDate,
        ExpectedDeliveryDate = entity.ExpectedDeliveryDate,
        ActualDeliveryDate = entity.ActualDeliveryDate,
        Status = entity.Status,
        StatusName = entity.Status.ToString(),
        Items = entity.Items?.Select(i => i.ToDto()).ToList() ?? new(),
        Subtotal = entity.Subtotal,
        Tax = entity.Tax,
        Discount = entity.Discount,
        Total = entity.Total,
        Notes = entity.Notes,
        OrderedBy = entity.OrderedBy,
        ReceivedBy = entity.ReceivedBy,
        SupplierInvoiceNumber = entity.SupplierInvoiceNumber,
        CreatedAt = entity.CreatedAt
    };

    public static PurchaseOrderSummaryDto ToSummaryDto(this PurchaseOrder entity) => new()
    {
        Id = entity.Id,
        OrderNumber = entity.OrderNumber,
        SupplierName = entity.Supplier?.Name ?? string.Empty,
        OrderDate = entity.OrderDate,
        ExpectedDeliveryDate = entity.ExpectedDeliveryDate,
        Status = entity.Status,
        StatusName = entity.Status.ToString(),
        ItemCount = entity.Items?.Count ?? 0,
        Total = entity.Total,
        OrderedBy = entity.OrderedBy
    };

    // PurchaseOrderItem
    public static PurchaseOrderItemDto ToDto(this PurchaseOrderItem entity) => new()
    {
        Id = entity.Id,
        PurchaseOrderId = entity.PurchaseOrderId,
        ProductId = entity.ProductId,
        ProductName = entity.Product?.Name ?? string.Empty,
        QuantityOrdered = entity.QuantityOrdered,
        QuantityReceived = entity.QuantityReceived,
        QuantityPending = entity.QuantityPending,
        Unit = entity.Unit,
        UnitCost = entity.UnitCost,
        Subtotal = entity.Subtotal,
        Notes = entity.Notes,
        IsFullyReceived = entity.IsFullyReceived
    };
}
