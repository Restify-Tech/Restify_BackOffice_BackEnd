using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de orden de compra
/// </summary>
public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public List<PurchaseOrderItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }
    public string? OrderedBy { get; set; }
    public string? ReceivedBy { get; set; }
    public string? SupplierInvoiceNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO de item de orden de compra
/// </summary>
public class PurchaseOrderItemDto
{
    public Guid Id { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal QuantityOrdered { get; set; }
    public decimal QuantityReceived { get; set; }
    public decimal QuantityPending { get; set; }
    public string Unit { get; set; } = "unidades";
    public decimal UnitCost { get; set; }
    public decimal Subtotal { get; set; }
    public string? Notes { get; set; }
    public bool IsFullyReceived { get; set; }
}

/// <summary>
/// DTO resumido para listas
/// </summary>
public class PurchaseOrderSummaryDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
    public string? OrderedBy { get; set; }
}

/// <summary>
/// Request para crear orden de compra
/// </summary>
public class CreatePurchaseOrderRequest
{
    public Guid SupplierId { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
    public decimal Discount { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request para item de orden de compra
/// </summary>
public class CreatePurchaseOrderItemRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "unidades";
    public decimal UnitCost { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request para actualizar orden de compra
/// </summary>
public class UpdatePurchaseOrderRequest
{
    public DateTime? ExpectedDeliveryDate { get; set; }
    public decimal Discount { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request para recibir orden (total o parcial)
/// </summary>
public class ReceivePurchaseOrderRequest
{
    public List<ReceiveItemRequest> Items { get; set; } = new();
    public DateTime? DeliveryDate { get; set; }
    public string? SupplierInvoiceNumber { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request para recibir un item específico
/// </summary>
public class ReceiveItemRequest
{
    public Guid ItemId { get; set; }
    public decimal QuantityReceived { get; set; }
}
