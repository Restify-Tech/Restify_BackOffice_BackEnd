using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface ICashRegisterRepository
{
    // ===== CRUD Cajas =====
    Task<List<CashRegister>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CashRegister?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CashRegister> CreateAsync(CashRegister entity, CancellationToken cancellationToken = default);
    Task<CashRegister> UpdateAsync(CashRegister entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // ===== Sesiones =====
    Task<CashRegisterSession?> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<CashRegisterSession?> GetActiveSessionAsync(Guid cashRegisterId, CancellationToken cancellationToken = default);
    Task<CashRegisterSession> CreateSessionAsync(CashRegisterSession session, CancellationToken cancellationToken = default);
    Task<CashRegisterSession> UpdateSessionAsync(CashRegisterSession session, CancellationToken cancellationToken = default);
    
    Task<List<CashRegisterSession>> GetSessionHistoryAsync(
        Guid? cashRegisterId = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
    
    // ===== Movimientos =====
    Task<CashRegisterMovement> CreateMovementAsync(CashRegisterMovement movement, CancellationToken cancellationToken = default);
    Task<List<CashRegisterMovement>> GetSessionMovementsAsync(Guid sessionId, CancellationToken cancellationToken = default);
    
    // ===== Queries especiales =====
    Task<bool> HasActiveSessionAsync(Guid cashRegisterId, CancellationToken cancellationToken = default);
    Task<int> CountActiveSessionsAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTodayCashSalesAsync(CancellationToken cancellationToken = default);
}
