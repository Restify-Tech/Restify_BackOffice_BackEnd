using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de caja registradora
/// </summary>
public class CashRegisterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CashRegisterStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? CurrentSessionId { get; set; }
    public CashRegisterSessionDto? CurrentSession { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO de sesión de caja
/// </summary>
public class CashRegisterSessionDto
{
    public Guid Id { get; set; }
    public Guid CashRegisterId { get; set; }
    public string CashRegisterName { get; set; } = string.Empty;
    public string OpenedBy { get; set; } = string.Empty;
    public DateTime OpenedAt { get; set; }
    public decimal OpeningBalance { get; set; }
    public string? ClosedBy { get; set; }
    public DateTime? ClosedAt { get; set; }
    public decimal? ExpectedClosingBalance { get; set; }
    public decimal? ActualClosingBalance { get; set; }
    public decimal? Difference { get; set; }
    public string? ClosingNotes { get; set; }
    public CashRegisterStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    
    // Resumen de movimientos
    public decimal TotalSales { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public decimal TotalDeposits { get; set; }
    public decimal TotalExpenses { get; set; }
    public int MovementCount { get; set; }
    
    public List<CashRegisterMovementDto> Movements { get; set; } = new();
}

/// <summary>
/// DTO de movimiento de caja
/// </summary>
public class CashRegisterMovementDto
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public CashMovementType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime MovementDate { get; set; }
    public string? RegisteredBy { get; set; }
}

/// <summary>
/// Resumen de sesión para lista
/// </summary>
public class CashRegisterSessionSummaryDto
{
    public Guid Id { get; set; }
    public string CashRegisterName { get; set; } = string.Empty;
    public string OpenedBy { get; set; } = string.Empty;
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal? ActualClosingBalance { get; set; }
    public decimal? Difference { get; set; }
    public CashRegisterStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int MovementCount { get; set; }
}

// ===== REQUEST DTOs =====

/// <summary>
/// Request para abrir caja
/// </summary>
public class OpenCashRegisterRequest
{
    public Guid CashRegisterId { get; set; }
    public decimal OpeningBalance { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request para cerrar caja
/// </summary>
public class CloseCashRegisterRequest
{
    public decimal ActualClosingBalance { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request para registrar movimiento
/// </summary>
public class RegisterCashMovementRequest
{
    public CashMovementType Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
}

/// <summary>
/// Request para crear caja registradora
/// </summary>
public class CreateCashRegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// Request para actualizar caja registradora
/// </summary>
public class UpdateCashRegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Estadísticas de caja
/// </summary>
public class CashRegisterStatisticsDto
{
    public decimal TodayTotalSales { get; set; }
    public decimal TodayCashSales { get; set; }
    public int TodayTransactions { get; set; }
    public int ActiveRegisters { get; set; }
    public List<CashRegisterSessionSummaryDto> OpenSessions { get; set; } = new();
}
