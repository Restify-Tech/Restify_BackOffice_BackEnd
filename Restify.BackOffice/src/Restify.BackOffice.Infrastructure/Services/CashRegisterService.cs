using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class CashRegisterService : ICashRegisterService
{
    private readonly ICashRegisterRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public CashRegisterService(
        ICashRegisterRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    // ===== CRUD de Cajas Registradoras =====

    public async Task<Result<List<CashRegisterDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto()).ToList();
        return Result<List<CashRegisterDto>>.Success(dtos);
    }

    public async Task<Result<CashRegisterDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<CashRegisterDto>.Failure("Caja registradora no encontrada");

        return Result<CashRegisterDto>.Success(entity.ToDto());
    }

    public async Task<Result<CashRegisterDto>> CreateAsync(CreateCashRegisterRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new CashRegister
        {
            Name = request.Name,
            Description = request.Description,
            Status = CashRegisterStatus.Closed,
            IsActive = true
        };

        var created = await _repository.CreateAsync(entity, cancellationToken);
        return Result<CashRegisterDto>.Success(created.ToDto());
    }

    public async Task<Result<CashRegisterDto>> UpdateAsync(Guid id, UpdateCashRegisterRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<CashRegisterDto>.Failure("Caja registradora no encontrada");

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.IsActive = request.IsActive;

        var updated = await _repository.UpdateAsync(entity, cancellationToken);
        return Result<CashRegisterDto>.Success(updated.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Caja registradora no encontrada");

        // Validar que no tenga sesión activa
        if (entity.Status == CashRegisterStatus.Open)
            return Result<bool>.Failure("No se puede eliminar una caja con sesión activa");

        await _repository.DeleteAsync(id, cancellationToken);
        return Result<bool>.Success(true);
    }

    // ===== Operaciones de Sesión =====

    public async Task<Result<CashRegisterSessionDto>> OpenSessionAsync(OpenCashRegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Validar que la caja exista
        var cashRegister = await _repository.GetByIdAsync(request.CashRegisterId, cancellationToken);
        if (cashRegister == null)
            return Result<CashRegisterSessionDto>.Failure("Caja registradora no encontrada");

        if (!cashRegister.IsActive)
            return Result<CashRegisterSessionDto>.Failure("La caja registradora no está activa");

        // Validar que no tenga sesión activa
        var hasActiveSession = await _repository.HasActiveSessionAsync(request.CashRegisterId, cancellationToken);
        if (hasActiveSession)
            return Result<CashRegisterSessionDto>.Failure("La caja ya tiene una sesión activa");

        // Crear sesión
        var session = new CashRegisterSession
        {
            CashRegisterId = request.CashRegisterId,
            OpenedBy = _currentUserService.Email ?? "System",
            OpenedAt = DateTime.UtcNow,
            OpeningBalance = request.OpeningBalance,
            Status = CashRegisterStatus.Open
        };

        var createdSession = await _repository.CreateSessionAsync(session, cancellationToken);

        // Crear movimiento de apertura
        var openingMovement = new CashRegisterMovement
        {
            SessionId = createdSession.Id,
            Type = CashMovementType.Opening,
            Amount = request.OpeningBalance,
            Description = "Apertura de caja",
            Notes = request.Notes,
            MovementDate = DateTime.UtcNow,
            RegisteredBy = _currentUserService.Email
        };

        await _repository.CreateMovementAsync(openingMovement, cancellationToken);

        // Actualizar estado de la caja
        cashRegister.Status = CashRegisterStatus.Open;
        cashRegister.CurrentSessionId = createdSession.Id;
        await _repository.UpdateAsync(cashRegister, cancellationToken);

        // Recargar sesión con movimientos
        var sessionWithMovements = await _repository.GetSessionByIdAsync(createdSession.Id, cancellationToken);
        return Result<CashRegisterSessionDto>.Success(sessionWithMovements!.ToDto());
    }

    public async Task<Result<CashRegisterSessionDto>> CloseSessionAsync(Guid sessionId, CloseCashRegisterRequest request, CancellationToken cancellationToken = default)
    {
        var session = await _repository.GetSessionByIdAsync(sessionId, cancellationToken);
        if (session == null)
            return Result<CashRegisterSessionDto>.Failure("Sesión no encontrada");

        if (session.Status != CashRegisterStatus.Open)
            return Result<CashRegisterSessionDto>.Failure("La sesión ya está cerrada");

        // Calcular balance esperado
        var expectedBalance = session.OpeningBalance + session.TotalSales + session.TotalDeposits - session.TotalWithdrawals - session.TotalExpenses;

        // Calcular diferencia
        var difference = request.ActualClosingBalance - expectedBalance;

        // Actualizar sesión
        session.ClosedBy = _currentUserService.Email ?? "System";
        session.ClosedAt = DateTime.UtcNow;
        session.ExpectedClosingBalance = expectedBalance;
        session.ActualClosingBalance = request.ActualClosingBalance;
        session.Difference = difference;
        session.ClosingNotes = request.Notes;
        session.Status = CashRegisterStatus.Closed;

        var updatedSession = await _repository.UpdateSessionAsync(session, cancellationToken);

        // Crear movimiento de cierre
        var closingMovement = new CashRegisterMovement
        {
            SessionId = sessionId,
            Type = CashMovementType.Closing,
            Amount = request.ActualClosingBalance,
            Description = "Cierre de caja",
            Notes = request.Notes,
            MovementDate = DateTime.UtcNow,
            RegisteredBy = _currentUserService.Email
        };

        await _repository.CreateMovementAsync(closingMovement, cancellationToken);

        // Actualizar estado de la caja
        var cashRegister = await _repository.GetByIdAsync(session.CashRegisterId, cancellationToken);
        if (cashRegister != null)
        {
            cashRegister.Status = CashRegisterStatus.Closed;
            cashRegister.CurrentSessionId = null;
            await _repository.UpdateAsync(cashRegister, cancellationToken);
        }

        // Recargar sesión completa
        var closedSession = await _repository.GetSessionByIdAsync(sessionId, cancellationToken);
        return Result<CashRegisterSessionDto>.Success(closedSession!.ToDto());
    }

    public async Task<Result<CashRegisterSessionDto>> GetActiveSessionAsync(Guid cashRegisterId, CancellationToken cancellationToken = default)
    {
        var session = await _repository.GetActiveSessionAsync(cashRegisterId, cancellationToken);
        if (session == null)
            return Result<CashRegisterSessionDto>.Failure("No hay sesión activa");

        return Result<CashRegisterSessionDto>.Success(session.ToDto());
    }

    public async Task<Result<CashRegisterSessionDto>> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _repository.GetSessionByIdAsync(sessionId, cancellationToken);
        if (session == null)
            return Result<CashRegisterSessionDto>.Failure("Sesión no encontrada");

        return Result<CashRegisterSessionDto>.Success(session.ToDto());
    }

    public async Task<Result<List<CashRegisterSessionSummaryDto>>> GetSessionHistoryAsync(
        Guid? cashRegisterId = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var sessions = await _repository.GetSessionHistoryAsync(cashRegisterId, from, to, cancellationToken);
        var dtos = sessions.Select(s => s.ToSummaryDto()).ToList();
        return Result<List<CashRegisterSessionSummaryDto>>.Success(dtos);
    }

    // ===== Movimientos =====

    public async Task<Result<CashRegisterMovementDto>> RegisterMovementAsync(Guid sessionId, RegisterCashMovementRequest request, CancellationToken cancellationToken = default)
    {
        var session = await _repository.GetSessionByIdAsync(sessionId, cancellationToken);
        if (session == null)
            return Result<CashRegisterMovementDto>.Failure("Sesión no encontrada");

        if (session.Status != CashRegisterStatus.Open)
            return Result<CashRegisterMovementDto>.Failure("La sesión no está activa");

        var movement = new CashRegisterMovement
        {
            SessionId = sessionId,
            Type = request.Type,
            Amount = request.Amount,
            Description = request.Description,
            Notes = request.Notes,
            ReferenceId = request.ReferenceId,
            ReferenceType = request.ReferenceType,
            MovementDate = DateTime.UtcNow,
            RegisteredBy = _currentUserService.Email
        };

        var created = await _repository.CreateMovementAsync(movement, cancellationToken);
        return Result<CashRegisterMovementDto>.Success(created.ToDto());
    }

    public async Task<Result<CashRegisterMovementDto>> RegisterSaleFromInvoiceAsync(Guid sessionId, Guid invoiceId, decimal amount, CancellationToken cancellationToken = default)
    {
        var request = new RegisterCashMovementRequest
        {
            Type = CashMovementType.Sale,
            Amount = amount,
            Description = $"Venta - Factura #{invoiceId.ToString().Substring(0, 8)}",
            ReferenceId = invoiceId,
            ReferenceType = "Invoice"
        };

        return await RegisterMovementAsync(sessionId, request, cancellationToken);
    }

    public async Task<Result<List<CashRegisterMovementDto>>> GetSessionMovementsAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var movements = await _repository.GetSessionMovementsAsync(sessionId, cancellationToken);
        var dtos = movements.Select(m => m.ToDto()).ToList();
        return Result<List<CashRegisterMovementDto>>.Success(dtos);
    }

    // ===== Estadísticas =====

    public async Task<Result<CashRegisterStatisticsDto>> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var todayCashSales = await _repository.GetTodayCashSalesAsync(cancellationToken);
        var activeSessionsCount = await _repository.CountActiveSessionsAsync(cancellationToken);

        // Obtener sesiones abiertas
        var today = DateTime.UtcNow.Date;
        var openSessions = await _repository.GetSessionHistoryAsync(null, today, null, cancellationToken);
        var openSessionDtos = openSessions.Where(s => s.Status == CashRegisterStatus.Open)
            .Select(s => s.ToSummaryDto())
            .ToList();

        var stats = new CashRegisterStatisticsDto
        {
            TodayCashSales = todayCashSales,
            ActiveRegisters = activeSessionsCount,
            OpenSessions = openSessionDtos
        };

        return Result<CashRegisterStatisticsDto>.Success(stats);
    }
}
