using Restify.Core.Application.DTOs.Grid;

namespace Restify.Core.Application.DTOs.DataProvider;

/// <summary>
/// Respuesta de una consulta de datos
/// </summary>
public class DataProviderQueryResponse
{
    /// <summary>
    /// Datos de la consulta
    /// </summary>
    public IEnumerable<Dictionary<string, object?>> Data { get; set; } = [];

    /// <summary>
    /// Total de registros (sin paginación)
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Página actual
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Tamaño de página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// Indica si hay más páginas
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Indica si hay páginas anteriores
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
/// Respuesta de metadata de una entidad
/// </summary>
public class DataProviderMetadataResponse
{
    /// <summary>
    /// Nombre de la entidad
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la vista
    /// </summary>
    public string ViewName { get; set; } = string.Empty;

    /// <summary>
    /// Título para mostrar
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Configuración del grid
    /// </summary>
    public GridConfigurationDto? GridConfiguration { get; set; }

    /// <summary>
    /// Indica si la entidad permite crear
    /// </summary>
    public bool AllowCreate { get; set; } = true;

    /// <summary>
    /// Indica si la entidad permite editar
    /// </summary>
    public bool AllowEdit { get; set; } = true;

    /// <summary>
    /// Indica si la entidad permite eliminar
    /// </summary>
    public bool AllowDelete { get; set; } = true;

    /// <summary>
    /// Campos disponibles para filtrado
    /// </summary>
    public List<FilterFieldInfo> FilterFields { get; set; } = [];
}

/// <summary>
/// Información de un campo para filtrado
/// </summary>
public class FilterFieldInfo
{
    public string Field { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "text"; // text, number, date, boolean, select
    public List<SelectOption>? Options { get; set; } // Para campos tipo select
}

/// <summary>
/// Opción para campos select
/// </summary>
public class SelectOption
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}
