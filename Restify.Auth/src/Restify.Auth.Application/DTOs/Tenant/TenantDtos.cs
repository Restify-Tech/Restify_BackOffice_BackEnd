using Restify.Auth.Domain.Enums;

namespace Restify.Auth.Application.DTOs.Tenant;

public record TenantDto(
    Guid Id,
    string Name,
    string Slug,
    string? Ruc,
    string? BusinessName,
    string? Address,
    string? Phone,
    string? Email,
    string? LogoUrl,
    string? SignatureUrl,
    string? FullAddress,
    double? Latitude,
    double? Longitude,
    IdentificationType? IdentificationType,
    string? IdentificationNumber,
    string PrimaryColor,
    string SecondaryColor,
    string AccentColor,
    string TemplateName,
    string? FaviconUrl,
    string? CoverImageUrl,
    string? FontHeading,
    string? FontBody,
    string? CustomCss,
    string Currency,
    decimal TaxPercentage,
    string TimeZone,
    TenantStatus Status,
    DeliveryOperationMode DeliveryOperationMode,
    Guid? DeliveryZoneId,
    string? DeliveryZoneName,
    bool OnboardingCompleted,
    DateTime? TrialExpiresAt,
    DateTime CreatedAt,
    int UserCount
);

/// <summary>
/// DTO publico de branding para menu QR (AllowAnonymous)
/// </summary>
public record TenantBrandingDto(
    string Name,
    string Slug,
    string? LogoUrl,
    string? CoverImageUrl,
    string? FaviconUrl,
    string PrimaryColor,
    string SecondaryColor,
    string AccentColor,
    string TemplateName,
    string? FontHeading,
    string? FontBody,
    string? CustomCss,
    string Currency,
    decimal TaxPercentage
);

/// <summary>
/// Request para actualizar branding del tenant
/// </summary>
public record UpdateTenantBrandingRequest(
    string? PrimaryColor,
    string? SecondaryColor,
    string? AccentColor,
    string? TemplateName,
    string? FontHeading,
    string? FontBody,
    string? CustomCss
);

public record TenantListDto(
    Guid Id,
    string Name,
    string Slug,
    string? Email,
    TenantStatus Status,
    DeliveryOperationMode DeliveryOperationMode,
    bool OnboardingCompleted,
    DateTime CreatedAt
);

public record UpdateTenantRequest(
    string Name,
    string? BusinessName,
    string? Address,
    string? Phone,
    string? Email,
    string Currency,
    decimal TaxPercentage,
    string TimeZone,
    DeliveryOperationMode? DeliveryOperationMode,
    Guid? DeliveryZoneId
);

public record UpdateTenantStatusRequest(
    TenantStatus Status
);

public record UpdateTenantDeliveryModeRequest(
    DeliveryOperationMode DeliveryOperationMode,
    Guid? DeliveryZoneId
);
