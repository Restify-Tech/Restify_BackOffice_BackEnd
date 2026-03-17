using Restify.Core.Domain.Enums;

namespace Restify.Core.Application.DTOs.Grid;

/// <summary>
/// DTO de columna de grid (para enviar al frontend)
/// </summary>
public class GridColumnDto
{
    public Guid Id { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string HeaderText { get; set; } = string.Empty;
    public string? FormLabel { get; set; }
    public string? HeaderTooltip { get; set; }
    public string ColumnType { get; set; } = "string";
    public string EditorType { get; set; } = "textbox";
    public int GridOrder { get; set; }
    public int FormOrder { get; set; }
    public string? FormGroup { get; set; }
    public string? Width { get; set; }
    public string? MinWidth { get; set; }
    public string? MaxWidth { get; set; }
    public string Alignment { get; set; } = "left";

    // Visibilidad
    public bool IsVisibleInGrid { get; set; }
    public bool IsVisibleInCreate { get; set; }
    public bool IsVisibleInEdit { get; set; }
    public bool IsVisibleInView { get; set; }
    public bool IsExportable { get; set; }

    // Comportamiento
    public bool IsSortable { get; set; }
    public bool IsFilterable { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsInlineEditable { get; set; }
    public bool IsRequired { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsFrozen { get; set; }
    public string? FrozenPosition { get; set; }

    // Formato
    public string? DisplayFormat { get; set; }
    public string? DefaultValue { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public int FormColumnSpan { get; set; }
    public string? CssClass { get; set; }

    // Templates
    public string? CellTemplate { get; set; }
    public string? EditorTemplate { get; set; }

    // Select/Enum options
    public List<SelectOptionDto>? SelectOptions { get; set; }

    // Condiciones
    public object? VisibilityCondition { get; set; }
    public object? EnabledCondition { get; set; }

    // Validaciones
    public List<GridColumnValidationDto> Validations { get; set; } = new();

    // Lookup
    public GridColumnLookupDto? Lookup { get; set; }

    // Extra
    public Dictionary<string, object>? ExtraConfig { get; set; }
}

public class SelectOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsDisabled { get; set; }
}

/// <summary>
/// Request para crear/actualizar columna
/// </summary>
public class CreateGridColumnRequest
{
    public Guid GridConfigurationId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string HeaderText { get; set; } = string.Empty;
    public string? FormLabel { get; set; }
    public GridColumnType ColumnType { get; set; } = GridColumnType.String;
    public EditorType EditorType { get; set; } = EditorType.TextBox;
    public int GridOrder { get; set; }
    public int FormOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVisibleInGrid { get; set; } = true;
    public bool IsVisibleInCreate { get; set; } = true;
    public bool IsVisibleInEdit { get; set; } = true;
}
