using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.DeliveryZones;

namespace Restify.Auth.Application.Interfaces;

public interface IDeliveryZoneService
{
    Task<Result<IEnumerable<DeliveryZoneListDto>>> GetAllAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeliveryZoneDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<DeliveryZoneDto>> CreateAsync(CreateDeliveryZoneRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeliveryZoneDto>> UpdateAsync(Guid id, UpdateDeliveryZoneRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
