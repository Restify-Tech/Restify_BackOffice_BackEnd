using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class DeliveryCooperativeMappingExtensions
{
    public static DeliveryCooperativeDto ToDto(this DeliveryCooperative entity)
    {
        return new DeliveryCooperativeDto(
            Id: entity.Id,
            Name: entity.Name,
            Ruc: entity.Ruc,
            ContactName: entity.ContactName,
            Phone: entity.Phone,
            Email: entity.Email,
            Address: entity.Address,
            IsActive: entity.IsActive,
            CommissionPercentage: entity.CommissionPercentage,
            DriverCount: entity.Drivers?.Count ?? 0,
            CreatedAt: entity.CreatedAt);
    }

    public static DeliveryCooperative ToEntity(this CreateDeliveryCooperativeRequest request)
    {
        return new DeliveryCooperative
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Ruc = request.Ruc,
            ContactName = request.ContactName,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            CommissionPercentage = request.CommissionPercentage
        };
    }

    public static void UpdateFrom(this DeliveryCooperative entity, UpdateDeliveryCooperativeRequest request)
    {
        entity.Name = request.Name;
        entity.ContactName = request.ContactName;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.Address = request.Address;
        entity.IsActive = request.IsActive;
        entity.CommissionPercentage = request.CommissionPercentage;
    }
}
