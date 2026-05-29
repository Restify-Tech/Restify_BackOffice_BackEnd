using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

public enum ReservationStatus
{
    Pending = 1,
    Confirmed = 2,
    Seated = 3,
    Completed = 4,
    Cancelled = 5,
    NoShow = 6
}

/// <summary>
/// Reserva de mesa de un cliente
/// </summary>
public class TableReservation : TenantEntity
{
    public Guid? TableId { get; set; }
    public Table? Table { get; set; }

    public Guid? BranchId { get; set; }

    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }

    public DateTime ReservationDateTime { get; set; }
    public int PartySize { get; set; }
    public string? SpecialRequests { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    /// <summary>
    /// Codigo de confirmacion de 6 digitos
    /// </summary>
    public string? ConfirmationCode { get; set; }

    public DateTime? ConfirmedAt { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }
}
