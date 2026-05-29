using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs;

public record PromotionDto(
    Guid Id,
    string Name,
    string? Description,
    PromotionType Type,
    string TypeName,
    decimal DiscountValue,
    string? ConditionsJson,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    bool IsActive,
    int? MaxUsageCount,
    int CurrentUsageCount,
    DateTime CreatedAt
);

public record CreatePromotionRequest(
    string Name,
    string? Description,
    PromotionType Type,
    decimal DiscountValue,
    string? ConditionsJson,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    int? MaxUsageCount
);

public record ApplicablePromotionDto(
    Guid PromotionId,
    string PromotionName,
    PromotionType Type,
    decimal DiscountValue,
    decimal CalculatedDiscount,
    string Description
);

public record LowStockAlertDto(
    Guid ItemId,
    string ItemName,
    decimal CurrentStock,
    decimal MinStockLevel,
    string Unit
);

public record UpdateMinStockRequest(
    decimal MinStockLevel
);

// ──── Calculate Promotions (sin orden creada) ────

public record CalculatePromotionsRequest(
    List<CalculationItemRequest> Items
);

public record CalculationItemRequest(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);

public record CalculatePromotionsResponse(
    List<AppliedPromotionDto> AppliedPromotions,
    decimal TotalDiscount
);

public record AppliedPromotionDto(
    Guid PromotionId,
    string Name,
    string Type,
    decimal DiscountAmount,
    string? Description
);

// ──── Upsell Suggestions ────

public record UpsellSuggestionDto(
    Guid ProductId,
    string ProductName,
    string? ProductImage,
    decimal Price,
    string CategoryName,
    string Reason
);

// ──── Table Customer Profile ────

public record TableCustomerProfileDto(
    Guid CustomerId,
    string CustomerName,
    string? PhoneNumber,
    string? Email,
    int TotalOrders
);
