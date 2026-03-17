namespace Restify.Auth.Domain.Interfaces;

/// <summary>
/// Interface para el patrón Unit of Work
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Guarda todos los cambios pendientes
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia una transacción
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma la transacción actual
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Revierte la transacción actual
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
