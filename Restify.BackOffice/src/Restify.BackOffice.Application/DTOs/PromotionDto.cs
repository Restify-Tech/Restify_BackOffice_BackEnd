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
