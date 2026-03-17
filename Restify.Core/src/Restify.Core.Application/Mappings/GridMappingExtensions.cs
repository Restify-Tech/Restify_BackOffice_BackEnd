using System.Text.Json;
using Restify.Core.Application.DTOs.Grid;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Application.Mappings;

/// <summary>
/// Extensiones de mapeo para entidades de Grid
/// </summary>
public static class GridMappingExtensions
{
    public static GridConfigurationDto ToDto(this GridConfiguration entity)
    {
        return new GridConfigurationDto
        {
            Id = entity.Id,
            EntityName = entity.EntityName,
            DisplayName = entity.DisplayName,
            DisplayNamePlural = entity.DisplayNamePlural,
            Description = entity.Description,
            Icon = entity.Icon,
            ApiEndpoint = entity.ApiEndpoint,
            DefaultPageSize = entity.DefaultPageSize,
            PageSizeOptions = ParseIntList(entity.PageSizeOptions),
            DefaultSortColumn = entity.DefaultSortColumn,
            DefaultSortDescending = entity.DefaultSortDescending,
            AllowCreate = entity.AllowCreate,
            AllowEdit = entity.AllowEdit,
            AllowDelete = entity.AllowDelete,
            AllowView = entity.AllowView,
            AllowExport = entity.AllowExport,
            ExportFormats = ParseStringList(entity.ExportFormats),
            AllowImport = entity.AllowImport,
            AllowSearch = entity.AllowSearch,
            AllowAdvancedFilter = entity.AllowAdvancedFilter,
            AllowInlineEdit = entity.AllowInlineEdit,
            AllowMultiSelect = entity.AllowMultiSelect,
            AllowColumnReorder = entity.AllowColumnReorder,
            AllowColumnResize = entity.AllowColumnResize,
            AllowColumnToggle = entity.AllowColumnToggle,
            FormMode = entity.FormMode,
            ShowRowActions = entity.ShowRowActions,
            RowActionsPosition = entity.RowActionsPosition,
            Columns = entity.Columns?.OrderBy(c => c.GridOrder).Select(c => c.ToDto()).ToList() ?? new(),
            ExtraConfig = ParseJsonObject(entity.ExtraConfig)
        };
    }

    public static GridColumnDto ToDto(this GridColumn entity)
    {
        return new GridColumnDto
        {
            Id = entity.Id,
            FieldName = entity.FieldName,
            HeaderText = entity.HeaderText,
            FormLabel = entity.FormLabel,
            HeaderTooltip = entity.HeaderTooltip,
            ColumnType = entity.ColumnType.ToString().ToLowerInvariant(),
            EditorType = entity.EditorType.ToString().ToLowerInvariant(),
            GridOrder = entity.GridOrder,
            FormOrder = entity.FormOrder,
            FormGroup = entity.FormGroup,
            Width = entity.Width,
            MinWidth = entity.MinWidth,
            MaxWidth = entity.MaxWidth,
            Alignment = entity.Alignment.ToString().ToLowerInvariant(),
            IsVisibleInGrid = entity.IsVisibleInGrid,
            IsVisibleInCreate = entity.IsVisibleInCreate,
            IsVisibleInEdit = entity.IsVisibleInEdit,
            IsVisibleInView = entity.IsVisibleInView,
            IsExportable = entity.IsExportable,
            IsSortable = entity.IsSortable,
            IsFilterable = entity.IsFilterable,
            IsSearchable = entity.IsSearchable,
            IsInlineEditable = entity.IsInlineEditable,
            IsRequired = entity.IsRequired,
            IsPrimaryKey = entity.IsPrimaryKey,
            IsReadOnly = entity.IsReadOnly,
            IsFrozen = entity.IsFrozen,
            FrozenPosition = entity.FrozenPosition,
            DisplayFormat = entity.DisplayFormat,
            DefaultValue = entity.DefaultValue,
            Placeholder = entity.Placeholder,
            HelpText = entity.HelpText,
            Prefix = entity.Prefix,
            Suffix = entity.Suffix,
            FormColumnSpan = entity.FormColumnSpan,
            CssClass = entity.CssClass,
            CellTemplate = entity.CellTemplate,
            EditorTemplate = entity.EditorTemplate,
            SelectOptions = ParseSelectOptions(entity.SelectOptions),
            VisibilityCondition = ParseJsonObject(entity.VisibilityCondition),
            EnabledCondition = ParseJsonObject(entity.EnabledCondition),
            Validations = entity.Validations?.OrderBy(v => v.Order).Select(v => v.ToDto()).ToList() ?? new(),
            Lookup = entity.Lookup?.ToDto(),
            ExtraConfig = ParseJsonObject(entity.ExtraConfig)
        };
    }

    public static GridColumnValidationDto ToDto(this GridColumnValidation entity)
    {
        return new GridColumnValidationDto
        {
            Id = entity.Id,
            ValidationType = entity.ValidationType.ToString().ToLowerInvariant(),
            ValidationValue = entity.ValidationValue,
            ValidationValue2 = entity.ValidationValue2,
            ErrorMessage = entity.ErrorMessage,
            Order = entity.Order,
            ClientOnly = entity.ClientOnly,
            ServerOnly = entity.ServerOnly
        };
    }

    public static GridColumnLookupDto ToDto(this GridColumnLookup entity)
    {
        return new GridColumnLookupDto
        {
            Id = entity.Id,
            TargetEntity = entity.TargetEntity,
            ApiEndpoint = entity.ApiEndpoint,
            ValueField = entity.ValueField,
            DisplayField = entity.DisplayField,
            AdditionalDisplayFields = entity.AdditionalDisplayFields,
            DisplayFormat = entity.DisplayFormat,
            AllowSearch = entity.AllowSearch,
            SearchField = entity.SearchField,
            MinSearchLength = entity.MinSearchLength,
            AllowAdd = entity.AllowAdd,
            AllowClear = entity.AllowClear,
            StaticFilter = ParseJsonObject(entity.StaticFilter),
            OrderBy = entity.OrderBy,
            MaxItems = entity.MaxItems,
            IsCached = entity.IsCached,
            CacheDuration = entity.CacheDuration,
            PreloadData = entity.PreloadData,
            IsDependentLookup = entity.IsDependentLookup,
            ParentField = entity.ParentField,
            ParentFilterField = entity.ParentFilterField,
            ClearOnParentChange = entity.ClearOnParentChange
        };
    }

    private static List<int> ParseIntList(string? json)
    {
        if (string.IsNullOrEmpty(json)) return new List<int> { 10, 25, 50, 100 };
        try
        {
            return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int> { 10, 25, 50, 100 };
        }
        catch
        {
            return new List<int> { 10, 25, 50, 100 };
        }
    }

    private static List<string> ParseStringList(string? json)
    {
        if (string.IsNullOrEmpty(json)) return new List<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static List<SelectOptionDto>? ParseSelectOptions(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<List<SelectOptionDto>>(json);
        }
        catch
        {
            return null;
        }
    }

    private static Dictionary<string, object>? ParseJsonObject(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
        catch
        {
            return null;
        }
    }
}
