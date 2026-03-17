namespace Restify.BackOffice.Application.DTOs;

public record MenuCategoryDto(
    Guid Id,
    string Name,
    string? Description,
    string? Icon,
    string? ImageUrl,
    int DisplayOrder,
    int ProductCount);

public record MenuProductDto(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl,
    decimal Price,
    bool IsAvailable,
    int DisplayOrder,
    Guid CategoryId,
    string CategoryName,
    IEnumerable<MenuProductModifierDto> Modifiers);

public record MenuProductModifierDto(
    Guid Id,
    string Name,
    string? Description,
    decimal PriceAdjustment,
    bool IsRequired);

public record CustomerCreateOrderRequest(
    int Type,
    Guid? TableId,
    string? CustomerName,
    string? CustomerPhone,
    Guid? CustomerId,
    IEnumerable<CustomerCreateOrderItemRequest> Items,
    string? Notes);

public record CustomerCreateOrderItemRequest(
    Guid ProductId,
    int Quantity,
    string? Notes,
    IEnumerable<CustomerOrderItemModifierRequest>? Modifiers);

public record CustomerOrderItemModifierRequest(
    string ModifierName,
    decimal PriceAdjustment);

public record CustomerOrderStatusDto(
    Guid Id,
    string OrderNumber,
    string Status,
    string PaymentStatus,
    string? TableNumber,
    IEnumerable<CustomerOrderItemStatusDto> Items,
    decimal Total,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CustomerOrderItemStatusDto(
    Guid Id,
    string ProductName,
    int Quantity,
    string Status);
