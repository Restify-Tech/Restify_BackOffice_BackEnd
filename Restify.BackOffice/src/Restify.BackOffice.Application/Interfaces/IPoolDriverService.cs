using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IPoolDriverService
{
    Task<Result<PagedResponse<PoolDriverListDto>>> GetAllPoolDriversAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<Result<PoolDriverDetailDto>> GetPoolDriverByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> ApproveDriverAsync(Guid id, ApproveDriverRequest request, CancellationToken cancellationToken = default);
    Task<Result> RejectDriverAsync(Guid id, RejectDriverRequest request, CancellationToken cancellationToken = default);
    Task<Result> ReviewDocumentAsync(Guid driverId, Guid documentId, ReviewDocumentRequest request, CancellationToken cancellationToken = default);
}
