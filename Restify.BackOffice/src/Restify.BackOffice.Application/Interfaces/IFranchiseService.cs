using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IFranchiseService
{
    Task<Result<FranchiseConfigDto?>> GetMyFranchiseAsync(CancellationToken cancellationToken = default);
    Task<Result<FranchiseConfigDto>> CreateAsync(CreateFranchiseConfigRequest request, CancellationToken cancellationToken = default);
    Task<Result<FranchiseConfigDto>> UpdateAsync(Guid id, CreateFranchiseConfigRequest request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<FranchiseeRelationDto>>> GetFranchiseesAsync(Guid franchiseConfigId, CancellationToken cancellationToken = default);
    Task<Result<FranchiseeRelationDto>> AddFranchiseeAsync(Guid franchiseConfigId, AddFranchiseeRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveFranchiseeAsync(Guid franchiseConfigId, Guid franchiseeId, CancellationToken cancellationToken = default);
    Task<Result<FranchiseConsolidatedReportDto>> GetConsolidatedReportAsync(Guid franchiseConfigId, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
}
