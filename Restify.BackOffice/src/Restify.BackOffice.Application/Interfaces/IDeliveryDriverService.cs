using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IDeliveryDriverService
{
    Task<Result<DeliveryDriverDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DeliveryDriverDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DeliveryDriverDto>>> GetAvailableAsync(CancellationToken cancellationToken = default);
    Task<Result<DeliveryDriverDto>> CreateAsync(CreateDeliveryDriverRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeliveryDriverDto>> UpdateAsync(Guid id, UpdateDeliveryDriverRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<DeliveryDriverDto>> UpdateStatusAsync(Guid id, UpdateDriverStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeliveryDriverDto>> VerifyAsync(Guid id, CancellationToken cancellationToken = default);
}
