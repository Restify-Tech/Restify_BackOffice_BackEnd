namespace Restify.Auth.Application.DTOs.Plans;

public record CreatePlanRequest(
    string Name,
    string Description,
    decimal MonthlyPrice,
    decimal AnnualPrice,
    int MaxUsers,
    int MaxBranches,
    string? Color,
    int DisplayOrder,
    bool IsDefault,
    List<string> IncludedScreenCodes
);
