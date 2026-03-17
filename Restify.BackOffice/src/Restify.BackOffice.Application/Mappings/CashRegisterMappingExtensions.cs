using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class CashRegisterMappingExtensions
{
    // ===== CashRegister =====
    
    public static CashRegisterDto ToDto(this CashRegister entity)
    {
        return new CashRegisterDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status,
            StatusName = entity.Status.ToString(),
            IsActive = entity.IsActive,
            CurrentSessionId = entity.CurrentSessionId,
            CurrentSession = entity.CurrentSession?.ToDto(),
            CreatedAt = entity.CreatedAt
        };
    }
    
    // ===== CashRegisterSession =====
    
    public static CashRegisterSessionDto ToDto(this CashRegisterSession entity)
    {
        return new CashRegisterSessionDto
        {
            Id = entity.Id,
            CashRegisterId = entity.CashRegisterId,
            CashRegisterName = entity.CashRegister?.Name ?? string.Empty,
            OpenedBy = entity.OpenedBy,
            OpenedAt = entity.OpenedAt,
            OpeningBalance = entity.OpeningBalance,
            ClosedBy = entity.ClosedBy,
            ClosedAt = entity.ClosedAt,
            ExpectedClosingBalance = entity.ExpectedClosingBalance,
            ActualClosingBalance = entity.ActualClosingBalance,
            Difference = entity.Difference,
            ClosingNotes = entity.ClosingNotes,
            Status = entity.Status,
            StatusName = entity.Status.ToString(),
            TotalSales = entity.TotalSales,
            TotalWithdrawals = entity.TotalWithdrawals,
            TotalDeposits = entity.TotalDeposits,
            TotalExpenses = entity.TotalExpenses,
            MovementCount = entity.Movements?.Count ?? 0,
            Movements = entity.Movements?.Select(m => m.ToDto()).ToList() ?? new()
        };
    }
    
    public static CashRegisterSessionSummaryDto ToSummaryDto(this CashRegisterSession entity)
    {
        return new CashRegisterSessionSummaryDto
        {
            Id = entity.Id,
            CashRegisterName = entity.CashRegister?.Name ?? string.Empty,
            OpenedBy = entity.OpenedBy,
            OpenedAt = entity.OpenedAt,
            ClosedAt = entity.ClosedAt,
            OpeningBalance = entity.OpeningBalance,
            ActualClosingBalance = entity.ActualClosingBalance,
            Difference = entity.Difference,
            Status = entity.Status,
            StatusName = entity.Status.ToString(),
            TotalSales = entity.TotalSales,
            MovementCount = entity.Movements?.Count ?? 0
        };
    }
    
    // ===== CashRegisterMovement =====
    
    public static CashRegisterMovementDto ToDto(this CashRegisterMovement entity)
    {
        return new CashRegisterMovementDto
        {
            Id = entity.Id,
            SessionId = entity.SessionId,
            Type = entity.Type,
            TypeName = entity.Type.ToString(),
            Amount = entity.Amount,
            ReferenceId = entity.ReferenceId,
            ReferenceType = entity.ReferenceType,
            Description = entity.Description,
            Notes = entity.Notes,
            MovementDate = entity.MovementDate,
            RegisteredBy = entity.RegisteredBy
        };
    }
}
