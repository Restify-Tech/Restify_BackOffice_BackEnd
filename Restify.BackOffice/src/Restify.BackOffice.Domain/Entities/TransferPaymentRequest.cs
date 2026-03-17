using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Solicitud de aprobacion de pago por transferencia.
/// El cliente sube el comprobante y el cajero aprueba o rechaza.
/// </summary>
public class TransferPaymentRequest : TenantEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? ImageUrl { get; set; }
    public string? BankReference { get; set; }
    public string? PayerName { get; set; }
    public string? PayerIdentification { get; set; }
    public TransferApprovalStatus Status { get; set; } = TransferApprovalStatus.Pending;
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }
}

public enum TransferApprovalStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
