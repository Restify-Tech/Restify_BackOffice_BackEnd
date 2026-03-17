namespace Restify.BackOffice.Application.DTOs;

public record DeliveryCooperativeDto(
    Guid Id, string Name, string Ruc, string? ContactName,
    string? Phone, string? Email, string? Address,
    bool IsActive, decimal? CommissionPercentage,
    int DriverCount, DateTime CreatedAt);

public record CreateDeliveryCooperativeRequest(
    string Name, string Ruc, string? ContactName,
    string? Phone, string? Email, string? Address,
    decimal? CommissionPercentage);

public record UpdateDeliveryCooperativeRequest(
    string Name, string? ContactName,
    string? Phone, string? Email, string? Address,
    bool IsActive, decimal? CommissionPercentage);
