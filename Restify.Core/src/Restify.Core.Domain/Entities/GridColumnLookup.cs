namespace Restify.Core.Domain.Entities;

/// <summary>
/// Configuración de lookup (relación) para una columna del grid
/// </summary>
public class GridColumnLookup : BaseEntity
{
    /// <summary>
    /// Referencia a la columna
    /// </summary>
    public Guid GridColumnId { get; set; }

    /// <summary>
    /// Nombre de la entidad relacionada
    /// </summary>
    public string TargetEntity { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint API para obtener los datos del lookup
    /// </summary>
    public string ApiEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Campo que contiene el valor (ID)
    /// </summary>
    public string ValueField { get; set; } = "id";

    /// <summary>
    /// Campo que se muestra al usuario
    /// </summary>
    public string DisplayField { get; set; } = "name";

    /// <summary>
    /// Campos adicionales para mostrar (separados por coma)
    /// </summary>
    public string? AdditionalDisplayFields { get; set; }

    /// <summary>
    /// Formato de visualización (ej: "{name} - {code}")
    /// </summary>
    public string? DisplayFormat { get; set; }

    /// <summary>
    /// Permite búsqueda en el lookup
    /// </summary>
    public bool AllowSearch { get; set; } = true;

    /// <summary>
    /// Campo por el cual buscar (si difiere de displayField)
    /// </summary>
    public string? SearchField { get; set; }

    /// <summary>
    /// Mínimo de caracteres para iniciar búsqueda
    /// </summary>
    public int MinSearchLength { get; set; } = 1;

    /// <summary>
    /// Permite agregar nuevos items desde el lookup
    /// </summary>
    public bool AllowAdd { get; set; } = false;

    /// <summary>
    /// Permite limpiar la selección
    /// </summary>
    public bool AllowClear { get; set; } = true;

    /// <summary>
    /// Filtro estático en formato JSON
    /// </summary>
    public string? StaticFilter { get; set; }

    /// <summary>
    /// Ordenamiento del lookup
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Número máximo de items a cargar
    /// </summary>
    public int? MaxItems { get; set; }

    /// <summary>
    /// Cachear los datos del lookup
    /// </summary>
    public bool IsCached { get; set; } = true;

    /// <summary>
    /// Tiempo de caché en segundos
    /// </summary>
    public int CacheDuration { get; set; } = 300;

    /// <summary>
    /// Cargar todos los datos al inicio (para lookups pequeños)
    /// </summary>
    public bool PreloadData { get; set; } = false;

    /// <summary>
    /// Es lookup dependiente de otra columna (cascada)
    /// </summary>
    public bool IsDependentLookup { get; set; } = false;

    /// <summary>
    /// Campo padre del cual depende
    /// </summary>
    public string? ParentField { get; set; }

    /// <summary>
    /// Campo en la entidad relacionada para filtrar por el padre
    /// </summary>
    public string? ParentFilterField { get; set; }

    /// <summary>
    /// Limpiar valor cuando cambia el padre
    /// </summary>
    public bool ClearOnParentChange { get; set; } = true;

    // Navigation property
    public GridColumn GridColumn { get; set; } = null!;
}
