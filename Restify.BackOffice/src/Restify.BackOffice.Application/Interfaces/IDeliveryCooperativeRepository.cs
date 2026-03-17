using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IDeliveryCooperativeRepository
{
    Task<DeliveryCooperative?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryCooperative>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DeliveryCooperative?> GetByRucAsync(string ruc, CancellationToken cancellationToken = default);
    Task<DeliveryCooperative> AddAsync(DeliveryCooperative entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(DeliveryCooperative entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeliveryCooperative entity, CancellationToken cancellationToken = default);
}
