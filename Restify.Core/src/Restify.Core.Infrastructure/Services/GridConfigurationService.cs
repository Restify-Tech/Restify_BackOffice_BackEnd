using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.DTOs.Grid;
using Restify.Core.Application.Interfaces;
using Restify.Core.Application.Mappings;
using Restify.Core.Domain.Entities;
using Restify.Core.Domain.Enums;

namespace Restify.Core.Infrastructure.Services;

public class GridConfigurationService : IGridConfigurationService
{
    private readonly IGridConfigurationRepository _configRepository;
    private readonly IGridColumnRepository _columnRepository;
    private readonly IGridColumnValidationRepository _validationRepository;
    private readonly IGridColumnLookupRepository _lookupRepository;

    public GridConfigurationService(
        IGridConfigurationRepository configRepository,
        IGridColumnRepository columnRepository,
        IGridColumnValidationRepository validationRepository,
        IGridColumnLookupRepository lookupRepository)
    {
        _configRepository = configRepository;
        _columnRepository = columnRepository;
        _validationRepository = validationRepository;
        _lookupRepository = lookupRepository;
    }

    public async Task<Result<GridConfigurationDto>> GetByEntityNameAsync(string entityName, CancellationToken cancellationToken = default)
    {
        var entity = await _configRepository.GetByEntityNameAsync(entityName, cancellationToken);
        if (entity == null)
            return Result<GridConfigurationDto>.Failure($"No se encontró configuración para la entidad '{entityName}'");

        return Result<GridConfigurationDto>.Success(entity.ToDto());
    }

    public async Task<Result<GridConfigurationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _configRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<GridConfigurationDto>.Failure("Configuración no encontrada");

        return Result<GridConfigurationDto>.Success(entity.ToDto());
    }

    public async Task<Result<List<GridConfigurationDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _configRepository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto()).ToList();
        return Result<List<GridConfigurationDto>>.Success(dtos);
    }

    public async Task<Result<GridConfigurationDto>> CreateAsync(CreateGridConfigurationRequest request, CancellationToken cancellationToken = default)
    {
        if (await _configRepository.ExistsAsync(request.EntityName, cancellationToken))
            return Result<GridConfigurationDto>.Failure($"Ya existe una configuración para la entidad '{request.EntityName}'");

        var entity = new GridConfiguration
        {
            EntityName = request.EntityName,
            DisplayName = request.DisplayName,
            DisplayNamePlural = request.DisplayNamePlural,
            Description = request.Description,
            Icon = request.Icon,
            ApiEndpoint = request.ApiEndpoint,
            DefaultPageSize = request.DefaultPageSize,
            DefaultSortColumn = request.DefaultSortColumn,
            DefaultSortDescending = request.DefaultSortDescending,
            AllowCreate = true,
            AllowEdit = true,
            AllowDelete = true,
            AllowView = true,
            AllowSearch = true,
            ShowRowActions = true
        };

        await _configRepository.AddAsync(entity, cancellationToken);

        return Result<GridConfigurationDto>.Success(entity.ToDto());
    }

    public async Task<Result<GridConfigurationDto>> UpdateAsync(UpdateGridConfigurationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _configRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity == null)
            return Result<GridConfigurationDto>.Failure("Configuración no encontrada");

        entity.DisplayName = request.DisplayName;
        entity.DisplayNamePlural = request.DisplayNamePlural;
        entity.Description = request.Description;
        entity.Icon = request.Icon;
        entity.ApiEndpoint = request.ApiEndpoint;
        entity.DefaultPageSize = request.DefaultPageSize;
        entity.DefaultSortColumn = request.DefaultSortColumn;
        entity.DefaultSortDescending = request.DefaultSortDescending;

        await _configRepository.UpdateAsync(entity, cancellationToken);

        return Result<GridConfigurationDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _configRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Configuración no encontrada");

        await _configRepository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<GridColumnDto>> AddColumnAsync(CreateGridColumnRequest request, CancellationToken cancellationToken = default)
    {
        var config = await _configRepository.GetByIdAsync(request.GridConfigurationId, cancellationToken);
        if (config == null)
            return Result<GridColumnDto>.Failure("Configuración de grid no encontrada");

        var entity = new GridColumn
        {
            GridConfigurationId = request.GridConfigurationId,
            FieldName = request.FieldName,
            HeaderText = request.HeaderText,
            FormLabel = request.FormLabel ?? request.HeaderText,
            ColumnType = request.ColumnType,
            EditorType = request.EditorType,
            GridOrder = request.GridOrder,
            FormOrder = request.FormOrder,
            IsRequired = request.IsRequired,
            IsVisibleInGrid = request.IsVisibleInGrid,
            IsVisibleInCreate = request.IsVisibleInCreate,
            IsVisibleInEdit = request.IsVisibleInEdit,
            IsVisibleInView = true,
            IsSortable = true,
            IsFilterable = true,
            IsSearchable = true,
            IsExportable = true
        };

        await _columnRepository.AddAsync(entity, cancellationToken);

        return Result<GridColumnDto>.Success(entity.ToDto());
    }

    public async Task<Result<GridColumnDto>> UpdateColumnAsync(Guid columnId, CreateGridColumnRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _columnRepository.GetByIdAsync(columnId, cancellationToken);
        if (entity == null)
            return Result<GridColumnDto>.Failure("Columna no encontrada");

        entity.FieldName = request.FieldName;
        entity.HeaderText = request.HeaderText;
        entity.FormLabel = request.FormLabel ?? request.HeaderText;
        entity.ColumnType = request.ColumnType;
        entity.EditorType = request.EditorType;
        entity.GridOrder = request.GridOrder;
        entity.FormOrder = request.FormOrder;
        entity.IsRequired = request.IsRequired;
        entity.IsVisibleInGrid = request.IsVisibleInGrid;
        entity.IsVisibleInCreate = request.IsVisibleInCreate;
        entity.IsVisibleInEdit = request.IsVisibleInEdit;

        await _columnRepository.UpdateAsync(entity, cancellationToken);

        return Result<GridColumnDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteColumnAsync(Guid columnId, CancellationToken cancellationToken = default)
    {
        var entity = await _columnRepository.GetByIdAsync(columnId, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Columna no encontrada");

        await _columnRepository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<GridColumnValidationDto>> AddValidationAsync(CreateGridColumnValidationRequest request, CancellationToken cancellationToken = default)
    {
        var column = await _columnRepository.GetByIdAsync(request.GridColumnId, cancellationToken);
        if (column == null)
            return Result<GridColumnValidationDto>.Failure("Columna no encontrada");

        if (!Enum.TryParse<ValidationType>(request.ValidationType, true, out var validationType))
            return Result<GridColumnValidationDto>.Failure("Tipo de validación no válido");

        var entity = new GridColumnValidation
        {
            GridColumnId = request.GridColumnId,
            ValidationType = validationType,
            ValidationValue = request.ValidationValue,
            ValidationValue2 = request.ValidationValue2,
            ErrorMessage = request.ErrorMessage,
            Order = request.Order
        };

        await _validationRepository.AddAsync(entity, cancellationToken);

        return Result<GridColumnValidationDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> DeleteValidationAsync(Guid validationId, CancellationToken cancellationToken = default)
    {
        var entity = await _validationRepository.GetByIdAsync(validationId, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Validación no encontrada");

        await _validationRepository.DeleteAsync(entity, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<GridColumnLookupDto>> SetLookupAsync(CreateGridColumnLookupRequest request, CancellationToken cancellationToken = default)
    {
        var column = await _columnRepository.GetByIdAsync(request.GridColumnId, cancellationToken);
        if (column == null)
            return Result<GridColumnLookupDto>.Failure("Columna no encontrada");

        // Eliminar lookup existente si hay uno
        await _lookupRepository.DeleteByColumnIdAsync(request.GridColumnId, cancellationToken);

        var entity = new GridColumnLookup
        {
            GridColumnId = request.GridColumnId,
            TargetEntity = request.TargetEntity,
            ApiEndpoint = request.ApiEndpoint,
            ValueField = request.ValueField,
            DisplayField = request.DisplayField,
            AllowSearch = request.AllowSearch,
            PreloadData = request.PreloadData
        };

        await _lookupRepository.AddAsync(entity, cancellationToken);

        return Result<GridColumnLookupDto>.Success(entity.ToDto());
    }

    public async Task<Result<bool>> RemoveLookupAsync(Guid columnId, CancellationToken cancellationToken = default)
    {
        await _lookupRepository.DeleteByColumnIdAsync(columnId, cancellationToken);
        return Result<bool>.Success(true);
    }
}
