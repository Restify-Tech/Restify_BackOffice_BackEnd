using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class DeductionTypeService : IDeductionTypeService
{
    private readonly IDeductionTypeRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public DeductionTypeService(
        IDeductionTypeRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<DeductionTypeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<DeductionTypeDto>.Failure("Tipo de deduccion no encontrado");

        return Result<DeductionTypeDto>.Success(entity.ToDto());
    }

    public async Task<Result<IEnumerable<DeductionTypeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<DeductionTypeDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<DeductionTypeDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetActiveAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<DeductionTypeDto>>.Success(dtos);
    }

    public async Task<Result<DeductionTypeDto>> CreateAsync(CreateDeductionTypeRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<DeductionTypeDto>.Failure("El nombre del tipo de deduccion es requerido");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var entity = request.ToEntity(tenantId);

        var created = await _repository.AddAsync(entity, cancellationToken);

        return Result<DeductionTypeDto>.Success(created.ToDto());
    }

    public async Task<Result<DeductionTypeDto>> UpdateAsync(Guid id, UpdateDeductionTypeRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<DeductionTypeDto>.Failure("Tipo de deduccion no encontrado");

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<DeductionTypeDto>.Failure("El nombre del tipo de deduccion es requerido");

        entity.UpdateFrom(request);
        await _repository.UpdateAsync(entity, cancellationToken);

        return Result<DeductionTypeDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Tipo de deduccion no encontrado");

        await _repository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }
}
