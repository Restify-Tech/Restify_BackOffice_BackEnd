namespace Restify.Auth.Application.DTOs.Tenant;

/// <summary>
/// Request para actualizar datos de onboarding del tenant
/// </summary>
public record TenantOnboardingRequest(
    string? Name,
    string? LogoUrl,
    string? SignatureUrl,
    string? FullAddress,
    double? Latitude,
    double? Longitude,
    string? Currency,
    decimal? TaxPercentage,
    string? TimeZone
);
