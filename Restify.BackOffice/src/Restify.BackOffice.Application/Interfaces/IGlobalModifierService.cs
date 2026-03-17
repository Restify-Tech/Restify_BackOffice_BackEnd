using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IGlobalModifierService
{
    Task<Result<GlobalModifierDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<GlobalModifierDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<GlobalModifierDto>>> GetByTypeAsync(ModifierType type, CancellationToken cancellationToken = default);
    Task<Result<GlobalModifierDto>> CreateAsync(CreateGlobalModifierRequest request, CancellationToken cancellationToken = default);
    Task<Result<GlobalModifierDto>> UpdateAsync(Guid id, UpdateGlobalModifierRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Asignar/desasignar de productos
    Task<Result<bool>> AssignToProductAsync(Guid modifierId, Guid productId, AssignGlobalModifierRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveFromProductAsync(Guid modifierId, Guid productId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<GlobalModifierProductDto>>> GetProductModifiersAsync(Guid productId, CancellationToken cancellationToken = default);
}
