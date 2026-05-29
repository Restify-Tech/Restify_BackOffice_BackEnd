namespace Restify.BackOffice.Application.Interfaces;

public interface IBenefitHubClient
{
    Task<BenefitSimulationResult> SimulateAsync(BenefitRequest request);
    Task<BenefitApplyResult> ApplyAsync(BenefitRequest request);
    Task<bool> ReverseAsync(string externalTransactionId, string tenantSourceId, string? reason = null);
    Task<CustomerBenefitsResult> GetCustomerBenefitsAsync(string externalCustomerId, string tenantSourceId);
}

public record BenefitRequest(
    string TenantSourceId,
    string ExternalCustomerId,
    string ExternalTransactionId,
    List<BenefitItem> Items,
    decimal TotalAmount,
    string? CouponCode = null
);

public record BenefitItem(
    string ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    string? CategoryId = null
);

public record BenefitSimulationResult(
    bool Success,
    List<AppliedBenefit> Benefits,
    decimal TotalSaving,
    string? ErrorMessage = null
);

public record BenefitApplyResult(
    bool Success,
    string? RedemptionId,
    decimal TotalSaving,
    int PointsEarned,
    int NewPointsBalance,
    string? MembershipTier,
    string? ErrorMessage = null
);

public record AppliedBenefit(
    string BenefitId,
    string Name,
    string Type,
    decimal Amount
);

public record CustomerBenefitsResult(
    bool Success,
    int Points,
    string? Tier,
    List<AvailableBenefit> Benefits
);

public record AvailableBenefit(
    string Id,
    string Name,
    string Type,
    string Description
);
