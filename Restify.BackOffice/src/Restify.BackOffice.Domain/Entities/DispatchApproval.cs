using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Aprobación de despacho de un pedido
/// </summary>
public class DispatchApproval : TenantEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public DispatchApprovalStatus Status { get; set; } = DispatchApprovalStatus.Pending;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Estado de la aprobación de despacho
/// </summary>
public enum DispatchApprovalStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
