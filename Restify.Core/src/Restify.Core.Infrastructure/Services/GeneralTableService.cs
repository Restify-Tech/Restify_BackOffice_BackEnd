using System.Text.Json;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.DTOs.General;
using Restify.Core.Application.Interfaces;
using Restify.Core.Application.Mappings;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Services;

public class GeneralTableService : IGeneralTableService
{
    private readonly IGeneralTableRepository _tableRepository;

    public GeneralTableService(IGeneralTableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<Result<GeneralTableDto>> GetByCodeAsync(string code, bool includeValues = false, CancellationToken cancellationToken = default)
    {
        var entity = await _tableRepository.GetByCodeAsync(code, includeValues, cancellationToken);
        if (entity == null)
            return Result<GeneralTableDto>.Failure($"No se encontró la tabla con código '{code}'");

        return Result<GeneralTableDto>.Success(entity.ToDto());
    }

    public async Task<Result<GeneralTableDto>> GetByIdAsync(Guid id, bool includeValues = false, CancellationToken cancellationToken = default)
    {
        var entity = await _tableRepository.GetByIdAsync(id, includeValues, cancellationToken);
        if (entity == null)
            return Result<GeneralTableDto>.Failure("Tabla no encontrada");

        return Result<GeneralTableDto>.Success(entity.ToDto());
    }

    public async Task<Result<List<GeneralTableListDto>>> GetAllAsync(bool? activeOnly = true, CancellationToken cancellationToken = default)
    {
        var entities = await _tableRepository.GetAllAsync(activeOnly, cancellationToken);
        var dtos = entities.Select(e => e.ToListDto()).ToList();
        return Result<List<GeneralTableListDto>>.Success(dtos);
    }

    public async Task<Result<List<GeneralTableListDto>>> GetByApplicationCodeAsync(string applicationCode, CancellationToken cancellationToken = default)
    {
        var entities = await _tableRepository.GetByApplicationCodeAsync(applicationCode, cancellationToken);
        var dtos = entities.Select(e => e.ToListDto()).ToList();
        return Result<List<GeneralTableListDto>>.Success(dtos);
    }

    public async Task<Result<List<GeneralTableListDto>>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        var entities = await _tableRepository.GetChildrenAsync(parentId, cancellationToken);
        var dtos = entities.Select(e => e.ToListDto()).ToList();
        return Result<List<GeneralTableListDto>>.Success(dtos);
    }

    public async Task<Result<List<GeneralTableDto>>> GetTreeAsync(string? applicationCode = null, CancellationToken cancellationToken = default)
    {
        var entities = await _tableRepository.GetRootTablesAsync(applicationCode, cancellationToken);
        var dtos = entities.Select(e => e.ToDto()).ToList();
        return Result<List<GeneralTableDto>>.Success(dtos);
    }

    public async Task<Result<GeneralTableDto>> CreateAsync(CreateGeneralTableRequest request, CancellationToken cancellationToken = default)
    {
        if (await _tableRepository.ExistsAsync(request.Code, cancellationToken))
            return Result<GeneralTableDto>.Failure($"Ya existe una tabla con el código '{request.Code}'");

        if (request.ParentId.HasValue)
        {
            var parent = await _tableRepository.GetByIdAsync(request.ParentId.Value, cancellationToken: cancellationToken);
            if (parent == null)
                return Result<GeneralTableDto>.Failure("La tabla padre no existe");
        }

        var entity = new GeneralTable
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            ParentId = request.ParentId,
            ApplicationCode = request.ApplicationCode,
            IsInternal = request.IsInternal,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
            Icon = request.Icon,
            ExtraConfig = request.ExtraConfig != null ? JsonSerializer.Serialize(request.ExtraConfig) : null
        };

        await _tableRepository.AddAsync(entity, cancellationToken);

        return Result<GeneralTableDto>.Success(entity.ToDto());
    }

    public async Task<Result<GeneralTableDto>> UpdateAsync(UpdateGeneralTableRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _tableRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        if (entity == null)
            return Result<GeneralTableDto>.Failure("Tabla no encontrada");

        // Verificar si está cambiando el código
        if (entity.Code != request.Code && await _tableRepository.ExistsAsync(request.Code, cancellationToken))
            return Result<GeneralTableDto>.Failure($"Ya existe una tabla con el código '{request.Code}'");

        // Verificar que no se asigne como padre a sí misma
        if (request.ParentId == request.Id)
            return Result<GeneralTableDto>.Failure("Una tabla no puede ser su propio padre");

        if (request.ParentId.HasValue && request.ParentId != entity.ParentId)
        {
            var parent = await _tableRepository.GetByIdAsync(request.ParentId.Value, cancellationToken: cancellationToken);
            if (parent == null)
                return Result<GeneralTableDto>.Failure("La tabla padre no existe");
        }

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.ParentId = request.ParentId;
        entity.ApplicationCode = request.ApplicationCode;
        entity.IsInternal = request.IsInternal;
        entity.IsActive = request.IsActive;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Icon = request.Icon;
        entity.ExtraConfig = request.ExtraConfig != null ? JsonSerializer.Serialize(request.ExtraConfig) : null;

        await _tableRepository.UpdateAsync(entity, cancellationToken);

        return Result<GeneralTableDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _tableRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Tabla no encontrada");

        if (entity.IsInternal)
            return Result<bool>.Failure("No se puede eliminar una tabla interna del sistema");

        if (await _tableRepository.HasChildrenAsync(id, cancellationToken))
            return Result<bool>.Failure("No se puede eliminar una tabla que tiene tablas hijas");

        if (await _tableRepository.HasValuesAsync(id, cancellationToken))
            return Result<bool>.Failure("No se puede eliminar una tabla que tiene valores. Elimine los valores primero");

        await _tableRepository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<List<LookupItem>>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _tableRepository.GetAllAsync(true, cancellationToken);
        var items = entities.Select(e => new LookupItem
        {
            Id = e.Id,
            Name = e.Name,
            Code = e.Code
        }).ToList();

        return Result<List<LookupItem>>.Success(items);
    }
}
