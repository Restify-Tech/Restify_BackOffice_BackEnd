using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Cierre formal de caja con arqueo de denominaciones y aprobacion del gerente
/// </summary>
public class CashClosing : TenantEntity
{
    public Guid CashRegisterSessionId { get; set; }
    public CashRegisterSession CashRegisterSession { get; set; } = null!;

    // Responsables
    public string ClosedBy { get; set; } = string.Empty;       // userId del cajero
    public string ClosedByName { get; set; } = string.Empty;
    public string? SupervisedBy { get; set; }                   // userId del gerente
    public string? SupervisedByName { get; set; }

    // Arqueo por denominaciones (JSON)
    // Formato: {"200":0,"100":2,"50":3,"20":5,"10":8,"5":10,"1":15,"0.50":20,"0.25":30,"0.10":50,"0.05":60,"0.01":100}
    public string? DenominationsJson { get; set; }

    // Totales
    public decimal TotalCounted { get; set; }           // lo que el cajero conto
    public decimal TotalExpected { get; set; }          // lo que el sistema calculo
    public decimal Difference { get; set; }             // TotalCounted - TotalExpected
    public string? DifferenceReason { get; set; }

    // Deposito bancario
    public decimal? BankDepositAmount { get; set; }
    public string? DepositVoucherUrl { get; set; }
    public string? BankName { get; set; }
    public string? BankReference { get; set; }

    // Reporte Z
    public string? ReportZUrl { get; set; }             // PDF generado

    // Estado
    public CashClosingStatus Status { get; set; } = CashClosingStatus.Draft;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }

    // Periodo
    public DateTime ClosingDate { get; set; } = DateTime.UtcNow;
}

public enum CashClosingStatus
{
    Draft = 1,          // cajero lleno los datos
    PendingReview = 2,  // enviado a gerente
    Approved = 3,       // gerente aprobo
    Disputed = 4,       // gerente rechazo / hay discrepancia
    Resolved = 5        // discrepancia resuelta
}
