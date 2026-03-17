namespace Restify.Core.Application.DTOs.Grid;

/// <summary>
/// DTO de configuración de lookup
/// </summary>
public class GridColumnLookupDto
{
    public Guid Id { get; set; }
    public string TargetEntity { get; set; } = string.Empty;
    public string ApiEndpoint { get; set; } = string.Empty;
    public string ValueField { get; set; } = "id";
    public string DisplayField { get; set; } = "name";
    public string? AdditionalDisplayFields { get; set; }
    public string? DisplayFormat { get; set; }
    public bool AllowSearch { get; set; }
    public string? SearchField { get; set; }
    public int MinSearchLength { get; set; }
    public bool AllowAdd { get; set; }
    public bool AllowClear { get; set; }
    public object? StaticFilter { get; set; }
    public string? OrderBy { get; set; }
    public int? MaxItems { get; set; }
    public bool IsCached { get; set; }
    public int CacheDuration { get; set; }
    public bool PreloadData { get; set; }
    public bool IsDependentLookup { get; set; }
    public string? ParentField { get; set; }
    public string? ParentFilterField { get; set; }
    public bool ClearOnParentChange { get; set; }
}

/// <summary>
/// Request para crear/actualizar lookup
/// </summary>
public class CreateGridColumnLookupRequest
{
    public Guid GridColumnId { get; set; }
    public string TargetEntity { get; set; } = string.Empty;
    public string ApiEndpoint { get; set; } = string.Empty;
    public string ValueField { get; set; } = "id";
    public string DisplayField { get; set; } = "name";
    public bool AllowSearch { get; set; } = true;
    public bool PreloadData { get; set; } = false;
}
