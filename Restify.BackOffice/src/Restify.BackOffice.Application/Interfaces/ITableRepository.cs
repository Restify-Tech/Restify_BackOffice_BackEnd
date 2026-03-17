using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface ITableRepository
{
    Task<Table?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Table?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
    Task<IEnumerable<Table>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Table>> GetByStatusAsync(TableStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Table>> GetByZoneAsync(string zone, CancellationToken cancellationToken = default);
    Task<Table> CreateAsync(Table table, CancellationToken cancellationToken = default);
    Task<Table> UpdateAsync(Table table, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetZonesAsync(CancellationToken cancellationToken = default);
}
