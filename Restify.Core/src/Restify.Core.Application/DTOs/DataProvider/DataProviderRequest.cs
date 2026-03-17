namespace Restify.Core.Application.DTOs.DataProvider;

/// <summary>
/// Request para obtener datos de una entidad
/// </summary>
public class DataProviderQueryRequest
{
    /// <summary>
    /// Nombre de la entidad (ej: "Category", "Product")
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la vista (ej: "INDEX", "DETAIL"). Default: "default"
    /// </summary>
    public string ViewName { get; set; } = "default";

    /// <summary>
    /// Página actual (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamaño de página
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Campo para ordenar
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// Dirección del ordenamiento (asc/desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Filtros dinámicos (campo -> valor)
    /// </summary>
    public Dictionary<string, object?>? Filters { get; set; }

    /// <summary>
    /// Búsqueda global en todos los campos de texto
    /// </summary>
    public string? GlobalSearch { get; set; }
}

/// <summary>
/// Request para obtener metadata de una entidad
/// </summary>
public class DataProviderMetadataRequest
{
    /// <summary>
    /// Nombre de la entidad
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la vista. Default: "default"
    /// </summary>
    public string ViewName { get; set; } = "default";
}
