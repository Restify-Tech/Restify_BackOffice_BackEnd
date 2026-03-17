using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class OrderMappingExtensions
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Type = order.Type,
            TypeName = order.Type.ToString(),
            Status = order.Status,
            StatusName = order.Status.ToString(),
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            TableId = order.TableId,
            TableNumber = order.Table?.Number,
            Items = order.Items.Select(i => i.ToDto()).ToList(),
            Subtotal = order.Subtotal,
            Tax = order.Tax,
            Discount = order.Discount,
            Total = order.Total,
            Notes = order.Notes,
            CompletedAt = order.CompletedAt,
            CancelledAt = order.CancelledAt,
            CancelReason = order.CancelReason,
            TakenBy = order.TakenBy,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }

    public static OrderItemDto ToDto(this OrderItem item)
    {
        return new OrderItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product?.Name ?? string.Empty,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Subtotal = item.Subtotal,
            Status = item.Status,
            StatusName = item.Status.ToString(),
            Notes = item.Notes,
            Modifiers = item.Modifiers.Select(m => m.ToDto()).ToList()
        };
    }

    public static OrderItemModifierDto ToDto(this OrderItemModifier modifier)
    {
        return new OrderItemModifierDto
        {
            ModifierName = modifier.ModifierName,
            PriceAdjustment = modifier.PriceAdjustment
        };
    }

    public static Order ToEntity(this CreateOrderRequest request, string orderNumber, Guid tenantId, string takenBy)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            OrderNumber = orderNumber,
            Type = request.Type,
            Status = OrderStatus.Pending,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            TableId = request.TableId,
            Discount = request.Discount,
            Notes = request.Notes,
            TakenBy = takenBy,
            CreatedAt = DateTime.UtcNow
        };

        return order;
    }

    public static OrderItem ToEntity(this CreateOrderItemRequest request, Guid orderId, Guid tenantId, Product product)
    {
        var item = new OrderItem
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            OrderId = orderId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = product.Price,
            Status = OrderItemStatus.Pending,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        // Calcular subtotal base
        decimal modifiersTotal = 0;

        foreach (var modRequest in request.Modifiers)
        {
            var modifier = new OrderItemModifier
            {
                Id = Guid.NewGuid(),
                OrderItemId = item.Id,
                ModifierName = modRequest.ModifierName,
                PriceAdjustment = modRequest.PriceAdjustment,
                CreatedAt = DateTime.UtcNow
            };

            item.Modifiers.Add(modifier);
            modifiersTotal += modRequest.PriceAdjustment;
        }

        item.Subtotal = (product.Price + modifiersTotal) * request.Quantity;

        return item;
    }

    public static void UpdateStatus(this Order order, UpdateOrderStatusRequest request)
    {
        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        if (request.Status == OrderStatus.Completed)
        {
            order.CompletedAt = DateTime.UtcNow;
        }
        else if (request.Status == OrderStatus.Cancelled)
        {
            order.CancelledAt = DateTime.UtcNow;
            order.CancelReason = request.CancelReason;
        }
    }

    public static void RecalculateTotals(this Order order, decimal taxRate = 0.12m)
    {
        order.Subtotal = order.Items.Sum(i => i.Subtotal);
        order.Tax = order.Subtotal * taxRate;
        order.Total = order.Subtotal + order.Tax - order.Discount;
    }
}
