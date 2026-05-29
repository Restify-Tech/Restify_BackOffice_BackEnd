using Restify.BackOffice.Domain.Enums;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Caja registradora física o virtual
/// </summary>
public class CashRegister : TenantEntity
{
    /// <summary>
    /// Nombre/código de la caja (ej: "Caja 1", "Caja Principal")
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Descripción o ubicación
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Estado actual de la caja
    /// </summary>
    public CashRegisterStatus Status { get; set; } = CashRegisterStatus.Closed;
    
    /// <summary>
    /// Sesión actual activa (si está abierta)
    /// </summary>
    public Guid? CurrentSessionId { get; set; }
    public CashRegisterSession? CurrentSession { get; set; }
    
    /// <summary>
    /// Si la caja está activa (habilitada para uso)
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Todas las sesiones de esta caja
    /// </summary>
    public ICollection<CashRegisterSession> Sessions { get; set; } = new List<CashRegisterSession>();

    // Sucursal
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
}

/// <summary>
/// Sesión de caja (apertura a cierre)
/// </summary>
public class CashRegisterSession : TenantEntity
{
    /// <summary>
    /// Caja registradora
    /// </summary>
    public Guid CashRegisterId { get; set; }
    public CashRegister CashRegister { get; set; } = null!;
    
    /// <summary>
    /// Usuario que abrió la caja
    /// </summary>
    public string OpenedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Fecha/hora de apertura
    /// </summary>
    public DateTime OpenedAt { get; set; }
    
    /// <summary>
    /// Monto inicial en efectivo
    /// </summary>
    public decimal OpeningBalance { get; set; }
    
    /// <summary>
    /// Usuario que cerró la caja
    /// </summary>
    public string? ClosedBy { get; set; }
    
    /// <summary>
    /// Fecha/hora de cierre
    /// </summary>
    public DateTime? ClosedAt { get; set; }
    
    /// <summary>
    /// Monto esperado al cierre (calculado)
    /// </summary>
    public decimal? ExpectedClosingBalance { get; set; }
    
    /// <summary>
    /// Monto real contado al cierre
    /// </summary>
    public decimal? ActualClosingBalance { get; set; }
    
    /// <summary>
    /// Diferencia (real - esperado)
    /// </summary>
    public decimal? Difference { get; set; }
    
    /// <summary>
    /// Notas del cierre (explicación de diferencias, etc)
    /// </summary>
    public string? ClosingNotes { get; set; }
    
    /// <summary>
    /// Estado de la sesión
    /// </summary>
    public CashRegisterStatus Status { get; set; } = CashRegisterStatus.Open;
    
    /// <summary>
    /// Movimientos de esta sesión
    /// </summary>
    public ICollection<CashRegisterMovement> Movements { get; set; } = new List<CashRegisterMovement>();

    /// <summary>
    /// Cierres formales de esta sesión
    /// </summary>
    public ICollection<CashClosing> Closings { get; set; } = new List<CashClosing>();
    
    // Propiedades calculadas (no mapeadas a BD)
    
    /// <summary>
    /// Total de ventas en efectivo
    /// </summary>
    public decimal TotalSales => Movements
        .Where(m => m.Type == CashMovementType.Sale)
        .Sum(m => m.Amount);
    
    /// <summary>
    /// Total de retiros
    /// </summary>
    public decimal TotalWithdrawals => Movements
        .Where(m => m.Type == CashMovementType.Withdrawal)
        .Sum(m => m.Amount);
    
    /// <summary>
    /// Total de ingresos adicionales
    /// </summary>
    public decimal TotalDeposits => Movements
        .Where(m => m.Type == CashMovementType.Deposit)
        .Sum(m => m.Amount);
    
    /// <summary>
    /// Total de gastos
    /// </summary>
    public decimal TotalExpenses => Movements
        .Where(m => m.Type == CashMovementType.Expense)
        .Sum(m => m.Amount);
}

/// <summary>
/// Movimiento individual de caja
/// </summary>
public class CashRegisterMovement : TenantEntity
{
    /// <summary>
    /// Sesión de caja a la que pertenece
    /// </summary>
    public Guid SessionId { get; set; }
    public CashRegisterSession Session { get; set; } = null!;
    
    /// <summary>
    /// Tipo de movimiento
    /// </summary>
    public CashMovementType Type { get; set; }
    
    /// <summary>
    /// Monto (siempre positivo, el tipo define si suma o resta)
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Referencia externa (ej: InvoiceId para ventas)
    /// </summary>
    public Guid? ReferenceId { get; set; }
    
    /// <summary>
    /// Tipo de referencia (ej: "Invoice", "Expense")
    /// </summary>
    public string? ReferenceType { get; set; }
    
    /// <summary>
    /// Descripción del movimiento
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Notas adicionales
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Fecha/hora del movimiento
    /// </summary>
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Usuario que registró el movimiento
    /// </summary>
    public string? RegisteredBy { get; set; }
}
