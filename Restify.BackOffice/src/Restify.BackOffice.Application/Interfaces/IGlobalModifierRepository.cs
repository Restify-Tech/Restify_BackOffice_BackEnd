using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IGlobalModifierRepository
{
    Task<GlobalModifier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GlobalModifier>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<GlobalModifier>> GetByTypeAsync(ModifierType type, CancellationToken cancellationToken = default);
    Task<GlobalModifier> CreateAsync(GlobalModifier modifier, CancellationToken cancellationToken = default);
    Task<GlobalModifier> UpdateAsync(GlobalModifier modifier, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Relación con productos
    Task<IEnumerable<GlobalModifier>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AssignToProductAsync(Guid modifierId, Guid productId, GlobalModifierProduct assignment, CancellationToken cancellationToken = default);
    Task RemoveFromProductAsync(Guid modifierId, Guid productId, CancellationToken cancellationToken = default);
}
