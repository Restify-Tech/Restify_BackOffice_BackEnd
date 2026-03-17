namespace Restify.Auth.Application.DTOs.DeliveryZones;

public record DeliveryZoneDto(
    Guid Id,
    string Name,
    string City,
    string? Region,
    string Country,
    decimal DefaultCommissionPercentage,
    double MaxDeliveryRadiusKm,
    decimal MinDriverRating,
    bool IsActive,
    DateTime CreatedAt
);

public record DeliveryZoneListDto(
    Guid Id,
    string Name,
    string City,
    string? Region,
    string Country,
    decimal DefaultCommissionPercentage,
    bool IsActive
);

public record CreateDeliveryZoneRequest(
    string Name,
    string City,
    string? Region,
    string Country,
    decimal DefaultCommissionPercentage,
    double MaxDeliveryRadiusKm,
    decimal MinDriverRating
);

public record UpdateDeliveryZoneRequest(
    string Name,
    string City,
    string? Region,
    string Country,
    decimal DefaultCommissionPercentage,
    double MaxDeliveryRadiusKm,
    decimal MinDriverRating,
    bool IsActive
);
