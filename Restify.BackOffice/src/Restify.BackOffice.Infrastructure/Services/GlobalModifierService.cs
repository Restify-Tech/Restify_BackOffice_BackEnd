using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class GlobalModifierService : IGlobalModifierService
{
    private readonly IGlobalModifierRepository _modifierRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;

    public GlobalModifierService(
        IGlobalModifierRepository modifierRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService)
    {
        _modifierRepository = modifierRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<GlobalModifierDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var modifier = await _modifierRepository.GetByIdAsync(id, cancellationToken);

        if (modifier == null)
            return Result<GlobalModifierDto>.Failure("Modificador global no encontrado");

        return Result<GlobalModifierDto>.Success(modifier.ToDto());
    }

    public async Task<Result<IEnumerable<GlobalModifierDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var modifiers = await _modifierRepository.GetAllAsync(cancellationToken);
        var dtos = modifiers.Select(m => m.ToDto());

        return Result<IEnumerable<GlobalModifierDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<GlobalModifierDto>>> GetByTypeAsync(ModifierType type, CancellationToken cancellationToken = default)
    {
        var modifiers = await _modifierRepository.GetByTypeAsync(type, cancellationToken);
        var dtos = modifiers.Select(m => m.ToDto());

        return Result<IEnumerable<GlobalModifierDto>>.Success(dtos);
    }

    public async Task<Result<GlobalModifierDto>> CreateAsync(CreateGlobalModifierRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<GlobalModifierDto>.Failure("El nombre del modificador es requerido");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var modifier = request.ToEntity(tenantId);

        var created = await _modifierRepository.CreateAsync(modifier, cancellationToken);

        return Result<GlobalModifierDto>.Success(created.ToDto());
    }

    public async Task<Result<GlobalModifierDto>> UpdateAsync(Guid id, UpdateGlobalModifierRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<GlobalModifierDto>.Failure("El nombre del modificador es requerido");

        var modifier = await _modifierRepository.GetByIdAsync(id, cancellationToken);
        if (modifier == null)
            return Result<GlobalModifierDto>.Failure("Modificador global no encontrado");

        modifier.UpdateFromRequest(request);
        var updated = await _modifierRepository.UpdateAsync(modifier, cancellationToken);

        return Result<GlobalModifierDto>.Success(updated.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var modifier = await _modifierRepository.GetByIdAsync(id, cancellationToken);
        if (modifier == null)
            return Result<bool>.Failure("Modificador global no encontrado");

        await _modifierRepository.DeleteAsync(id, cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> AssignToProductAsync(Guid modifierId, Guid productId, AssignGlobalModifierRequest request, CancellationToken cancellationToken = default)
    {
        // Verificar que el modificador existe
        var modifier = await _modifierRepository.GetByIdAsync(modifierId, cancellationToken);
        if (modifier == null)
            return Result<bool>.Failure("Modificador global no encontrado");

        // Verificar que el producto existe
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            return Result<bool>.Failure("Producto no encontrado");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        var assignment = new GlobalModifierProduct
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            GlobalModifierId = modifierId,
            ProductId = productId,
            CustomPriceAdjustment = request.CustomPriceAdjustment,
            IsRequired = request.IsRequired,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _modifierRepository.AssignToProductAsync(modifierId, productId, assignment, cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveFromProductAsync(Guid modifierId, Guid productId, CancellationToken cancellationToken = default)
    {
        await _modifierRepository.RemoveFromProductAsync(modifierId, productId, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<GlobalModifierProductDto>>> GetProductModifiersAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            return Result<IEnumerable<GlobalModifierProductDto>>.Failure("Producto no encontrado");

        var dtos = product.GlobalModifiers.Select(gmp => gmp.ToDto());
        return Result<IEnumerable<GlobalModifierProductDto>>.Success(dtos);
    }
}
