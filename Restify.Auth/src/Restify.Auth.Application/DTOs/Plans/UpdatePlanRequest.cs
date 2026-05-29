namespace Restify.Auth.Application.DTOs.Plans;

public record UpdatePlanRequest(
    string Name,
    string Description,
    decimal MonthlyPrice,
    decimal AnnualPrice,
    int MaxUsers,
    int MaxBranches,
    string? Color,
    int DisplayOrder,
    bool IsDefault,
    bool IsActive,
    List<string> IncludedScreenCodes
);
