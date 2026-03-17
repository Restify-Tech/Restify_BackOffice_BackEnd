using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class CashRegisterRepository : ICashRegisterRepository
{
    private readonly BackOfficeDbContext _context;

    public CashRegisterRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    // ===== CRUD Cajas =====

    public async Task<List<CashRegister>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CashRegisters
            .Include(cr => cr.CurrentSession)
            .OrderBy(cr => cr.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<CashRegister?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CashRegisters
            .Include(cr => cr.CurrentSession)
            .FirstOrDefaultAsync(cr => cr.Id == id, cancellationToken);
    }

    public async Task<CashRegister> CreateAsync(CashRegister entity, CancellationToken cancellationToken = default)
    {
        _context.CashRegisters.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<CashRegister> UpdateAsync(CashRegister entity, CancellationToken cancellationToken = default)
    {
        _context.CashRegisters.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.CashRegisters.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _context.CashRegisters.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // ===== Sesiones =====

    public async Task<CashRegisterSession?> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.CashRegisterSessions
            .Include(s => s.CashRegister)
            .Include(s => s.Movements.OrderBy(m => m.MovementDate))
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
    }

    public async Task<CashRegisterSession?> GetActiveSessionAsync(Guid cashRegisterId, CancellationToken cancellationToken = default)
    {
        return await _context.CashRegisterSessions
            .Include(s => s.CashRegister)
            .Include(s => s.Movements.OrderBy(m => m.MovementDate))
            .FirstOrDefaultAsync(s => s.CashRegisterId == cashRegisterId && s.Status == CashRegisterStatus.Open, cancellationToken);
    }

    public async Task<CashRegisterSession> CreateSessionAsync(CashRegisterSession session, CancellationToken cancellationToken = default)
    {
        _context.CashRegisterSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);
        return session;
    }

    public async Task<CashRegisterSession> UpdateSessionAsync(CashRegisterSession session, CancellationToken cancellationToken = default)
    {
        _context.CashRegisterSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
        return session;
    }

    public async Task<List<CashRegisterSession>> GetSessionHistoryAsync(
        Guid? cashRegisterId = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.CashRegisterSessions
            .Include(s => s.CashRegister)
            .Include(s => s.Movements)
            .AsQueryable();

        if (cashRegisterId.HasValue)
            query = query.Where(s => s.CashRegisterId == cashRegisterId.Value);

        if (from.HasValue)
            query = query.Where(s => s.OpenedAt.Date >= from.Value.Date);

        if (to.HasValue)
            query = query.Where(s => s.OpenedAt.Date <= to.Value.Date);

        return await query
            .OrderByDescending(s => s.OpenedAt)
            .ToListAsync(cancellationToken);
    }

    // ===== Movimientos =====

    public async Task<CashRegisterMovement> CreateMovementAsync(CashRegisterMovement movement, CancellationToken cancellationToken = default)
    {
        _context.CashRegisterMovements.Add(movement);
        await _context.SaveChangesAsync(cancellationToken);
        return movement;
    }

    public async Task<List<CashRegisterMovement>> GetSessionMovementsAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.CashRegisterMovements
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.MovementDate)
            .ToListAsync(cancellationToken);
    }

    // ===== Queries especiales =====

    public async Task<bool> HasActiveSessionAsync(Guid cashRegisterId, CancellationToken cancellationToken = default)
    {
        return await _context.CashRegisterSessions
            .AnyAsync(s => s.CashRegisterId == cashRegisterId && s.Status == CashRegisterStatus.Open, cancellationToken);
    }

    public async Task<int> CountActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CashRegisterSessions
            .CountAsync(s => s.Status == CashRegisterStatus.Open, cancellationToken);
    }

    public async Task<decimal> GetTodayCashSalesAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        
        return await _context.CashRegisterMovements
            .Where(m => m.Type == CashMovementType.Sale 
                && m.MovementDate.Date == today)
            .SumAsync(m => m.Amount, cancellationToken);
    }
}
