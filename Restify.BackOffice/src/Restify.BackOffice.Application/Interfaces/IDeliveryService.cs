using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IDeliveryService
{
    Task<Result<DeliveryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DeliveryDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<DeliveryDto>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DeliveryDto>>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DeliveryDto>>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<Result<DeliveryDto>> CreateAsync(CreateDeliveryRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeliveryDto>> AssignAsync(Guid id, AssignDeliveryRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeliveryDto>> UpdateStatusAsync(Guid id, UpdateDeliveryStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeliveryDto>> UpdateLocationAsync(Guid id, UpdateDriverLocationRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeliveryTrackingDto>> GetTrackingAsync(Guid id, CancellationToken cancellationToken = default);
}
