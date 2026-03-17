using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid? TableId { get; set; }
    public string? TableNumber { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
    public string? TakenBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public OrderItemStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<OrderItemModifierDto> Modifiers { get; set; } = new();
}

public class OrderItemModifierDto
{
    public string ModifierName { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}

public class CreateOrderRequest
{
    public OrderType Type { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid? TableId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
    public decimal Discount { get; set; } = 0;
    public string? Notes { get; set; }
}

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public List<CreateOrderItemModifierRequest> Modifiers { get; set; } = new();
}

public class CreateOrderItemModifierRequest
{
    public string ModifierName { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
}

public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
    public string? CancelReason { get; set; }
}

public class UpdateOrderItemStatusRequest
{
    public OrderItemStatus Status { get; set; }
}
