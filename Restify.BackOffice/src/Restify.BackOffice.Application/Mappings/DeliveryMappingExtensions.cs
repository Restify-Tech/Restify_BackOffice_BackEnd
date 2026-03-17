using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class DeliveryMappingExtensions
{
    public static DeliveryDto ToDto(this Delivery entity)
    {
        return new DeliveryDto(
            Id: entity.Id,
            OrderId: entity.OrderId,
            OrderNumber: entity.Order?.OrderNumber ?? string.Empty,
            DriverId: entity.DriverId,
            DriverName: entity.Driver != null ? $"{entity.Driver.FirstName} {entity.Driver.LastName}" : null,
            DriverPhone: entity.Driver?.Phone,
            Status: entity.Status.ToString(),
            DeliveryAddress: entity.DeliveryAddress,
            DeliveryNotes: entity.DeliveryNotes,
            DeliveryFee: entity.DeliveryFee,
            EstimatedDeliveryMinutes: entity.EstimatedDeliveryMinutes,
            AssignedAt: entity.AssignedAt,
            PickedUpAt: entity.PickedUpAt,
            DeliveredAt: entity.DeliveredAt,
            DeliveryProofUrl: entity.DeliveryProofUrl,
            CustomerRating: entity.CustomerRating,
            CustomerFeedback: entity.CustomerFeedback,
            FailureReason: entity.FailureReason,
            CreatedAt: entity.CreatedAt);
    }

    public static DeliveryTrackingDto ToTrackingDto(this Delivery entity)
    {
        return new DeliveryTrackingDto(
            Id: entity.Id,
            Status: entity.Status.ToString(),
            DeliveryAddress: entity.DeliveryAddress,
            DriverName: entity.Driver != null ? $"{entity.Driver.FirstName} {entity.Driver.LastName}" : null,
            DriverPhone: entity.Driver?.Phone,
            DriverLatitude: entity.DriverLatitude,
            DriverLongitude: entity.DriverLongitude,
            LastLocationUpdate: entity.LastLocationUpdate,
            EstimatedDeliveryMinutes: entity.EstimatedDeliveryMinutes,
            AssignedAt: entity.AssignedAt,
            PickedUpAt: entity.PickedUpAt,
            DeliveredAt: entity.DeliveredAt);
    }

    public static Delivery ToEntity(this CreateDeliveryRequest request)
    {
        return new Delivery
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            DeliveryAddress = request.DeliveryAddress,
            DeliveryNotes = request.DeliveryNotes,
            DeliveryFee = request.DeliveryFee,
            EstimatedDeliveryMinutes = request.EstimatedDeliveryMinutes
        };
    }
}
