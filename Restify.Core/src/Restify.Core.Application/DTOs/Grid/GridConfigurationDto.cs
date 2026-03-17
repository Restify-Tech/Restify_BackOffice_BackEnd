namespace Restify.Core.Application.DTOs.Grid;

/// <summary>
/// DTO completo de configuración de grid (para enviar al frontend)
/// </summary>
public class GridConfigurationDto
{
    public Guid Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DisplayNamePlural { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string ApiEndpoint { get; set; } = string.Empty;
    public int DefaultPageSize { get; set; }
    public List<int> PageSizeOptions { get; set; } = new();
    public string? DefaultSortColumn { get; set; }
    public bool DefaultSortDescending { get; set; }

    // Permisos
    public bool AllowCreate { get; set; }
    public bool AllowEdit { get; set; }
    public bool AllowDelete { get; set; }
    public bool AllowView { get; set; }
    public bool AllowExport { get; set; }
    public List<string> ExportFormats { get; set; } = new();
    public bool AllowImport { get; set; }
    public bool AllowSearch { get; set; }
    public bool AllowAdvancedFilter { get; set; }
    public bool AllowInlineEdit { get; set; }
    public bool AllowMultiSelect { get; set; }
    public bool AllowColumnReorder { get; set; }
    public bool AllowColumnResize { get; set; }
    public bool AllowColumnToggle { get; set; }

    // UI
    public string FormMode { get; set; } = "modal";
    public bool ShowRowActions { get; set; }
    public string RowActionsPosition { get; set; } = "end";

    // Columnas
    public List<GridColumnDto> Columns { get; set; } = new();

    // Extra
    public Dictionary<string, object>? ExtraConfig { get; set; }
}

/// <summary>
/// Request para crear/actualizar configuración de grid
/// </summary>
public class CreateGridConfigurationRequest
{
    public string EntityName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DisplayNamePlural { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string ApiEndpoint { get; set; } = string.Empty;
    public int DefaultPageSize { get; set; } = 10;
    public string? DefaultSortColumn { get; set; }
    public bool DefaultSortDescending { get; set; }
}

public class UpdateGridConfigurationRequest : CreateGridConfigurationRequest
{
    public Guid Id { get; set; }
}
