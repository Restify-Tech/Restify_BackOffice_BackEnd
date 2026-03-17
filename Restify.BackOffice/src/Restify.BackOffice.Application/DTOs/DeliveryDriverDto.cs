namespace Restify.BackOffice.Application.DTOs;

public record DeliveryDriverDto(
    Guid Id, string FirstName, string LastName,
    string IdentificationNumber, string Phone, string Email,
    string? VehiclePlate, string? VehicleDescription, string? PhotoUrl,
    string DriverType, string Status,
    Guid? CooperativeId, string? CooperativeName,
    bool IsVerified, bool IsActive,
    decimal? Rating, int TotalDeliveries,
    DateTime? LastLoginAt, DateTime CreatedAt);

public record CreateDeliveryDriverRequest(
    string FirstName, string LastName,
    string IdentificationNumber, string Phone, string Email,
    string Password, string? VehiclePlate, string? VehicleDescription,
    int DriverType, Guid? CooperativeId);

public record UpdateDeliveryDriverRequest(
    string FirstName, string LastName,
    string Phone, string Email,
    string? VehiclePlate, string? VehicleDescription, string? PhotoUrl,
    int DriverType, Guid? CooperativeId, bool IsActive);

public record UpdateDriverStatusRequest(int Status);
