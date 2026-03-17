using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class DeliveryDriverMappingExtensions
{
    public static DeliveryDriverDto ToDto(this DeliveryDriver entity)
    {
        return new DeliveryDriverDto(
            Id: entity.Id,
            FirstName: entity.FirstName,
            LastName: entity.LastName,
            IdentificationNumber: entity.IdentificationNumber,
            Phone: entity.Phone,
            Email: entity.Email,
            VehiclePlate: entity.VehiclePlate,
            VehicleDescription: entity.VehicleDescription,
            PhotoUrl: entity.PhotoUrl,
            DriverType: entity.DriverType.ToString(),
            Status: entity.Status.ToString(),
            CooperativeId: entity.CooperativeId,
            CooperativeName: entity.Cooperative?.Name,
            IsVerified: entity.IsVerified,
            IsActive: entity.IsActive,
            Rating: entity.Rating,
            TotalDeliveries: entity.TotalDeliveries,
            LastLoginAt: entity.LastLoginAt,
            CreatedAt: entity.CreatedAt);
    }

    public static DeliveryDriver ToEntity(this CreateDeliveryDriverRequest request, string passwordHash)
    {
        return new DeliveryDriver
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IdentificationNumber = request.IdentificationNumber,
            Phone = request.Phone,
            Email = request.Email,
            VehiclePlate = request.VehiclePlate,
            VehicleDescription = request.VehicleDescription,
            DriverType = (DriverType)request.DriverType,
            CooperativeId = request.CooperativeId,
            PasswordHash = passwordHash
        };
    }

    public static void UpdateFrom(this DeliveryDriver entity, UpdateDeliveryDriverRequest request)
    {
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.VehiclePlate = request.VehiclePlate;
        entity.VehicleDescription = request.VehicleDescription;
        entity.PhotoUrl = request.PhotoUrl;
        entity.DriverType = (DriverType)request.DriverType;
        entity.CooperativeId = request.CooperativeId;
        entity.IsActive = request.IsActive;
    }
}
