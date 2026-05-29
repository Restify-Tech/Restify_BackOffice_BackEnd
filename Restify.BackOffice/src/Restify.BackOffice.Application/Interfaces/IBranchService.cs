using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IBranchService
{
    Task<Result<IEnumerable<BranchDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<BranchDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<BranchDto>> CreateAsync(CreateBranchRequest request, CancellationToken cancellationToken = default);
    Task<Result<BranchDto>> UpdateAsync(Guid id, UpdateBranchRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<BranchStatsDto>> GetStatsAsync(Guid id, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
    Task<Result<ConsolidatedBranchReportDto>> GetConsolidatedReportAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
}

public interface IManagerAssignmentService
{
    Task<Result<IEnumerable<ManagerAssignmentDto>>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<Result<ManagerAssignmentDto>> CreateAsync(CreateManagerAssignmentRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
