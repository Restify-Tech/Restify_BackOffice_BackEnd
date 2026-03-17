using System.Text.Json;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.DTOs.General;
using Restify.Core.Application.Interfaces;
using Restify.Core.Application.Mappings;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Services;

public class GeneralValueService : IGeneralValueService
{
    private readonly IGeneralValueRepository _valueRepository;
    private readonly IGeneralTableRepository _tableRepository;

    public GeneralValueService(
        IGeneralValueRepository valueRepository,
        IGeneralTableRepository tableRepository)
    {
        _valueRepository = valueRepository;
        _tableRepository = tableRepository;
    }

    public async Task<Result<GeneralValueDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _valueRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<GeneralValueDto>.Failure("Valor no encontrado");

        return Result<GeneralValueDto>.Success(entity.ToDto());
    }

    public async Task<Result<GeneralValueDto>> GetByCodeAsync(string tableCode, string valueCode, CancellationToken cancellationToken = default)
    {
        var table = await _tableRepository.GetByCodeAsync(tableCode, cancellationToken: cancellationToken);
        if (table == null)
            return Result<GeneralValueDto>.Failure($"No se encontró la tabla con código '{tableCode}'");

        var entity = await _valueRepository.GetByCodeAsync(table.Id, valueCode, cancellationToken);
        if (entity == null)
            return Result<GeneralValueDto>.Failure($"No se encontró el valor con código '{valueCode}'");

        return Result<GeneralValueDto>.Success(entity.ToDto());
    }

    public async Task<Result<List<GeneralValueListDto>>> GetByTableIdAsync(Guid tableId, bool? activeOnly = true, CancellationToken cancellationToken = default)
    {
        var entities = await _valueRepository.GetByTableIdAsync(tableId, activeOnly, cancellationToken);
        var dtos = entities.Select(e => e.ToListDto()).ToList();
        return Result<List<GeneralValueListDto>>.Success(dtos);
    }

    public async Task<Result<List<GeneralValueListDto>>> GetByTableCodeAsync(string tableCode, bool? activeOnly = true, CancellationToken cancellationToken = default)
    {
        var table = await _tableRepository.GetByCodeAsync(tableCode, cancellationToken: cancellationToken);
        if (table == null)
            return Result<List<GeneralValueListDto>>.Failure($"No se encontró la tabla con código '{tableCode}'");

        var entities = await _valueRepository.GetByTableIdAsync(table.Id, activeOnly, cancellationToken);
        var dtos = entities.Select(e => e.ToListDto()).ToList();
        return Result<List<GeneralValueListDto>>.Success(dtos);
    }

    public async Task<Result<GeneralValueDto>> CreateAsync(CreateGeneralValueRequest request, CancellationToken cancellationToken = default)
    {
        var table = await _tableRepository.GetByIdAsync(request.GeneralTableId, cancellationToken: cancellationToken);
        if (table == null)
            return Result<GeneralValueDto>.Failure("La tabla no existe");

        if (await _valueRepository.ExistsAsync(request.GeneralTableId, request.Code, cancellationToken))
            return Result<GeneralValueDto>.Failure($"Ya existe un valor con el código '{request.Code}' en esta tabla");

        // Si es default, quitar el default de los demás
        if (request.IsDefault)
        {
            await _valueRepository.ClearDefaultAsync(request.GeneralTableId, cancellationToken);
        }

        var entity = new GeneralValue
        {
            GeneralTableId = request.GeneralTableId,
            Code = request.Code,
            Content = request.Content,
            ShortDescription = request.ShortDescription,
            NumericValue = request.NumericValue,
            Reference1 = request.Reference1,
            Reference2 = request.Reference2,
            Reference3 = request.Reference3,
            Reference4 = request.Reference4,
            Reference5 = request.Reference5,
            Icon = request.Icon,
            BackgroundColor = request.BackgroundColor,
            TextColor = request.TextColor,
            DisplayOrder = request.DisplayOrder,
            IsLocked = request.IsLocked,
            IsActive = request.IsActive,
            IsDefault = request.IsDefault,
            ExtraConfig = request.ExtraConfig != null ? JsonSerializer.Serialize(request.ExtraConfig) : null
        };

        await _valueRepository.AddAsync(entity, cancellationToken);

        // Recargar con la navegación
        entity = await _valueRepository.GetByIdAsync(entity.Id, cancellationToken);

        return Result<GeneralValueDto>.Success(entity!.ToDto());
    }

    public async Task<Result<GeneralValueDto>> UpdateAsync(UpdateGeneralValueRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _valueRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity == null)
            return Result<GeneralValueDto>.Failure("Valor no encontrado");

        if (entity.IsLocked)
            return Result<GeneralValueDto>.Failure("Este valor está bloqueado y no puede ser modificado");

        // Verificar si está cambiando el código
        if (entity.Code != request.Code && await _valueRepository.ExistsAsync(entity.GeneralTableId, request.Code, cancellationToken))
            return Result<GeneralValueDto>.Failure($"Ya existe un valor con el código '{request.Code}' en esta tabla");

        // Si es default, quitar el default de los demás
        if (request.IsDefault && !entity.IsDefault)
        {
            await _valueRepository.ClearDefaultAsync(entity.GeneralTableId, cancellationToken);
        }

        entity.Code = request.Code;
        entity.Content = request.Content;
        entity.ShortDescription = request.ShortDescription;
        entity.NumericValue = request.NumericValue;
        entity.Reference1 = request.Reference1;
        entity.Reference2 = request.Reference2;
        entity.Reference3 = request.Reference3;
        entity.Reference4 = request.Reference4;
        entity.Reference5 = request.Reference5;
        entity.Icon = request.Icon;
        entity.BackgroundColor = request.BackgroundColor;
        entity.TextColor = request.TextColor;
        entity.DisplayOrder = request.DisplayOrder;
        entity.IsActive = request.IsActive;
        entity.IsDefault = request.IsDefault;
        entity.ExtraConfig = request.ExtraConfig != null ? JsonSerializer.Serialize(request.ExtraConfig) : null;

        await _valueRepository.UpdateAsync(entity, cancellationToken);

        return Result<GeneralValueDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _valueRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Valor no encontrado");

        if (entity.IsLocked)
            return Result<bool>.Failure("Este valor está bloqueado y no puede ser eliminado");

        await _valueRepository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _valueRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Valor no encontrado");

        await _valueRepository.ClearDefaultAsync(entity.GeneralTableId, cancellationToken);

        entity.IsDefault = true;
        await _valueRepository.UpdateAsync(entity, cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ReorderAsync(Guid tableId, List<Guid> orderedIds, CancellationToken cancellationToken = default)
    {
        var values = await _valueRepository.GetByTableIdAsync(tableId, false, cancellationToken);

        for (int i = 0; i < orderedIds.Count; i++)
        {
            var value = values.FirstOrDefault(v => v.Id == orderedIds[i]);
            if (value != null)
            {
                value.DisplayOrder = i;
            }
        }

        await _valueRepository.UpdateRangeAsync(values, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<List<LookupItem>>> GetLookupAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        var entities = await _valueRepository.GetByTableIdAsync(tableId, true, cancellationToken);
        var items = entities.Select(e => new LookupItem
        {
            Id = e.Id,
            Name = e.Content,
            Code = e.Code
        }).ToList();

        return Result<List<LookupItem>>.Success(items);
    }

    public async Task<Result<List<LookupItem>>> GetLookupByTableCodeAsync(string tableCode, CancellationToken cancellationToken = default)
    {
        var table = await _tableRepository.GetByCodeAsync(tableCode, cancellationToken: cancellationToken);
        if (table == null)
            return Result<List<LookupItem>>.Failure($"No se encontró la tabla con código '{tableCode}'");

        return await GetLookupAsync(table.Id, cancellationToken);
    }
}
