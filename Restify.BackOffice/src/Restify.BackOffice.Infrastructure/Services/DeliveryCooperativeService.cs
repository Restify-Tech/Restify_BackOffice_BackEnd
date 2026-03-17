using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

public class DeliveryCooperativeService : IDeliveryCooperativeService
{
    private readonly IDeliveryCooperativeRepository _repository;

    public DeliveryCooperativeService(IDeliveryCooperativeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DeliveryCooperativeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<DeliveryCooperativeDto>.Failure("Cooperativa no encontrada");

        return Result<DeliveryCooperativeDto>.Success(entity.ToDto());
    }

    public async Task<Result<IEnumerable<DeliveryCooperativeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<DeliveryCooperativeDto>>.Success(dtos);
    }

    public async Task<Result<DeliveryCooperativeDto>> CreateAsync(CreateDeliveryCooperativeRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<DeliveryCooperativeDto>.Failure("El nombre es requerido");

        if (string.IsNullOrWhiteSpace(request.Ruc))
            return Result<DeliveryCooperativeDto>.Failure("El RUC es requerido");

        var existing = await _repository.GetByRucAsync(request.Ruc, cancellationToken);
        if (existing != null)
            return Result<DeliveryCooperativeDto>.Failure($"Ya existe una cooperativa con el RUC '{request.Ruc}'");

        var entity = request.ToEntity();
        var created = await _repository.AddAsync(entity, cancellationToken);

        return Result<DeliveryCooperativeDto>.Success(created.ToDto());
    }

    public async Task<Result<DeliveryCooperativeDto>> UpdateAsync(Guid id, UpdateDeliveryCooperativeRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<DeliveryCooperativeDto>.Failure("Cooperativa no encontrada");

        entity.UpdateFrom(request);
        await _repository.UpdateAsync(entity, cancellationToken);

        return Result<DeliveryCooperativeDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Cooperativa no encontrada");

        if (entity.Drivers?.Count > 0)
            return Result<bool>.Failure("No se puede eliminar una cooperativa que tiene motorizados asociados");

        await _repository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }
}
