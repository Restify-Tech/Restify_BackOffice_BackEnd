using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface ITableService
{
    Task<Result<TableDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TableDto>> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TableDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TableDto>>> GetByStatusAsync(TableStatus status, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TableDto>>> GetByZoneAsync(string zone, CancellationToken cancellationToken = default);
    Task<Result<TableDto>> CreateAsync(CreateTableRequest request, CancellationToken cancellationToken = default);
    Task<Result<TableDto>> UpdateAsync(Guid id, UpdateTableRequest request, CancellationToken cancellationToken = default);
    Task<Result<TableDto>> UpdateStatusAsync(Guid id, UpdateTableStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TableLayoutDto>> GetLayoutAsync(CancellationToken cancellationToken = default);
}
