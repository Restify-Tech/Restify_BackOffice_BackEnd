namespace Restify.BackOffice.Application.DTOs;

public record DeliveryDto(
    Guid Id, Guid OrderId, string OrderNumber,
    Guid? DriverId, string? DriverName, string? DriverPhone,
    string Status, string DeliveryAddress, string? DeliveryNotes,
    decimal DeliveryFee, int? EstimatedDeliveryMinutes,
    DateTime? AssignedAt, DateTime? PickedUpAt, DateTime? DeliveredAt,
    string? DeliveryProofUrl, int? CustomerRating, string? CustomerFeedback,
    string? FailureReason, DateTime CreatedAt);

public record CreateDeliveryRequest(
    Guid OrderId, string DeliveryAddress,
    string? DeliveryNotes, decimal DeliveryFee,
    int? EstimatedDeliveryMinutes);

public record AssignDeliveryRequest(Guid DriverId);

public record UpdateDeliveryStatusRequest(int Status, string? FailureReason);

public record UpdateDriverLocationRequest(decimal Latitude, decimal Longitude);

public record DeliveryTrackingDto(
    Guid Id, string Status, string DeliveryAddress,
    string? DriverName, string? DriverPhone,
    decimal? DriverLatitude, decimal? DriverLongitude,
    DateTime? LastLocationUpdate,
    int? EstimatedDeliveryMinutes,
    DateTime? AssignedAt, DateTime? PickedUpAt,
    DateTime? DeliveredAt);

public record DriverLocationDto(Guid DeliveryId, decimal Latitude, decimal Longitude, DateTime UpdatedAt);
