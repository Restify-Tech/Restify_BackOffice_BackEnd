using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IDeliveryCooperativeService
{
    Task<Result<DeliveryCooperativeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DeliveryCooperativeDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<DeliveryCooperativeDto>> CreateAsync(CreateDeliveryCooperativeRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeliveryCooperativeDto>> UpdateAsync(Guid id, UpdateDeliveryCooperativeRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
