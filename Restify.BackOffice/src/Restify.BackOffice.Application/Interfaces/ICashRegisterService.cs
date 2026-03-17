using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface ICashRegisterService
{
    // ===== CRUD de Cajas Registradoras =====
    
    /// <summary>
    /// Obtener todas las cajas registradoras
    /// </summary>
    Task<Result<List<CashRegisterDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtener caja por ID
    /// </summary>
    Task<Result<CashRegisterDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Crear nueva caja registradora
    /// </summary>
    Task<Result<CashRegisterDto>> CreateAsync(CreateCashRegisterRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Actualizar caja registradora
    /// </summary>
    Task<Result<CashRegisterDto>> UpdateAsync(Guid id, UpdateCashRegisterRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Eliminar caja registradora
    /// </summary>
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // ===== Operaciones de Sesión =====
    
    /// <summary>
    /// Abrir caja (iniciar sesión)
    /// </summary>
    Task<Result<CashRegisterSessionDto>> OpenSessionAsync(OpenCashRegisterRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cerrar caja (finalizar sesión)
    /// </summary>
    Task<Result<CashRegisterSessionDto>> CloseSessionAsync(Guid sessionId, CloseCashRegisterRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtener sesión activa de una caja
    /// </summary>
    Task<Result<CashRegisterSessionDto>> GetActiveSessionAsync(Guid cashRegisterId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtener sesión por ID
    /// </summary>
    Task<Result<CashRegisterSessionDto>> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtener historial de sesiones
    /// </summary>
    Task<Result<List<CashRegisterSessionSummaryDto>>> GetSessionHistoryAsync(
        Guid? cashRegisterId = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
    
    // ===== Movimientos =====
    
    /// <summary>
    /// Registrar movimiento en la sesión activa
    /// </summary>
    Task<Result<CashRegisterMovementDto>> RegisterMovementAsync(Guid sessionId, RegisterCashMovementRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Registrar venta desde factura (automático)
    /// </summary>
    Task<Result<CashRegisterMovementDto>> RegisterSaleFromInvoiceAsync(Guid sessionId, Guid invoiceId, decimal amount, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtener movimientos de una sesión
    /// </summary>
    Task<Result<List<CashRegisterMovementDto>>> GetSessionMovementsAsync(Guid sessionId, CancellationToken cancellationToken = default);
    
    // ===== Estadísticas =====
    
    /// <summary>
    /// Obtener estadísticas generales de cajas
    /// </summary>
    Task<Result<CashRegisterStatisticsDto>> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
