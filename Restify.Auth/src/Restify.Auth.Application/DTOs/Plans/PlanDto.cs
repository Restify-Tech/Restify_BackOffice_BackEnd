namespace Restify.Auth.Application.DTOs.Plans;

public record PlanDto(
    Guid Id,
    string Name,
    string Description,
    decimal MonthlyPrice,
    decimal AnnualPrice,
    int MaxUsers,
    int MaxBranches,
    bool IsActive,
    int DisplayOrder,
    string? Color,
    bool IsDefault,
    int TenantCount,
    List<string> IncludedScreenCodes,
    DateTime CreatedAt
);
