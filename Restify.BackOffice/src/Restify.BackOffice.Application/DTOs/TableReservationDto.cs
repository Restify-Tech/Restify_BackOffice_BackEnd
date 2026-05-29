using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.DTOs;

public record TableReservationDto(
    Guid Id,
    Guid? TableId,
    string? TableName,
    Guid? BranchId,
    string CustomerName,
    string CustomerPhone,
    string? CustomerEmail,
    DateTime ReservationDateTime,
    int PartySize,
    string? SpecialRequests,
    ReservationStatus Status,
    string StatusName,
    string? ConfirmationCode,
    DateTime? ConfirmedAt,
    string? CancelledBy,
    string? CancellationReason,
    DateTime CreatedAt
);

public record CreateReservationRequest(
    Guid? TableId,
    Guid? BranchId,
    string CustomerName,
    string CustomerPhone,
    string? CustomerEmail,
    DateTime ReservationDateTime,
    int PartySize,
    string? SpecialRequests
);

public record ConfirmReservationRequest(
    Guid? TableId
);

public record CancelReservationRequest(
    string? Reason
);

public record SeatReservationRequest(
    Guid? TableId
);
