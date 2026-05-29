namespace Restify.BackOffice.Application.DTOs;

public record FranchiseConfigDto(
    Guid Id,
    string FranchiseName,
    string? Description,
    bool AllowLocalMenuOverrides,
    bool AllowLocalPromotions,
    bool SyncMenuAutomatically,
    string? ContactEmail,
    string? LogoUrl,
    int FranchiseeCount
);

public record CreateFranchiseConfigRequest(
    string FranchiseName,
    string? Description,
    bool AllowLocalMenuOverrides,
    bool AllowLocalPromotions,
    bool SyncMenuAutomatically,
    string? ContactEmail
);

public record FranchiseeRelationDto(
    Guid Id,
    Guid FranchiseeTenantId,
    string FranchiseeName,
    string? FranchiseeCity,
    bool IsActive,
    DateTime JoinedAt,
    string? RoyaltyPercentage
);

public record AddFranchiseeRequest(
    Guid FranchiseeTenantId,
    string FranchiseeName,
    string? FranchiseeCity,
    string? RoyaltyPercentage
);

public record FranchiseConsolidatedReportDto(
    string FranchiseName,
    IEnumerable<FranchiseeStatsDto> Franchisees,
    decimal TotalRevenue,
    int TotalOrders
);

public record FranchiseeStatsDto(
    Guid TenantId,
    string FranchiseeName,
    string City,
    decimal Revenue,
    int Orders,
    decimal AvgTicket,
    bool IsActive
);
