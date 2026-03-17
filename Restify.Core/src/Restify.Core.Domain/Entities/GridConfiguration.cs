namespace Restify.Core.Domain.Entities;

/// <summary>
/// Configuración general de un grid para una entidad específica
/// </summary>
public class GridConfiguration : TenantEntity
{
    /// <summary>
    /// Nombre único de la entidad (ej: "Category", "Product")
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Título que se muestra en el grid
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Plural del nombre para mostrar (ej: "Categorías")
    /// </summary>
    public string DisplayNamePlural { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del grid
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Icono del módulo (nombre de icono o clase CSS)
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Endpoint API base para CRUD
    /// </summary>
    public string ApiEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Número de registros por página por defecto
    /// </summary>
    public int DefaultPageSize { get; set; } = 10;

    /// <summary>
    /// Opciones de tamaño de página disponibles (JSON array)
    /// </summary>
    public string PageSizeOptions { get; set; } = "[10, 25, 50, 100]";

    /// <summary>
    /// Columna por la cual ordenar por defecto
    /// </summary>
    public string? DefaultSortColumn { get; set; }

    /// <summary>
    /// Dirección de ordenamiento por defecto
    /// </summary>
    public bool DefaultSortDescending { get; set; } = false;

    /// <summary>
    /// Permite crear nuevos registros
    /// </summary>
    public bool AllowCreate { get; set; } = true;

    /// <summary>
    /// Permite editar registros
    /// </summary>
    public bool AllowEdit { get; set; } = true;

    /// <summary>
    /// Permite eliminar registros
    /// </summary>
    public bool AllowDelete { get; set; } = true;

    /// <summary>
    /// Permite ver detalle del registro
    /// </summary>
    public bool AllowView { get; set; } = true;

    /// <summary>
    /// Permite exportar datos
    /// </summary>
    public bool AllowExport { get; set; } = true;

    /// <summary>
    /// Formatos de exportación permitidos (JSON array: ["xlsx", "csv", "pdf"])
    /// </summary>
    public string ExportFormats { get; set; } = "[\"xlsx\", \"csv\"]";

    /// <summary>
    /// Permite importar datos
    /// </summary>
    public bool AllowImport { get; set; } = false;

    /// <summary>
    /// Permite búsqueda global
    /// </summary>
    public bool AllowSearch { get; set; } = true;

    /// <summary>
    /// Permite filtros avanzados
    /// </summary>
    public bool AllowAdvancedFilter { get; set; } = true;

    /// <summary>
    /// Permite edición inline en el grid
    /// </summary>
    public bool AllowInlineEdit { get; set; } = false;

    /// <summary>
    /// Permite selección múltiple
    /// </summary>
    public bool AllowMultiSelect { get; set; } = false;

    /// <summary>
    /// Permite reordenar columnas
    /// </summary>
    public bool AllowColumnReorder { get; set; } = true;

    /// <summary>
    /// Permite redimensionar columnas
    /// </summary>
    public bool AllowColumnResize { get; set; } = true;

    /// <summary>
    /// Permite ocultar/mostrar columnas
    /// </summary>
    public bool AllowColumnToggle { get; set; } = true;

    /// <summary>
    /// Modo de apertura del formulario (modal, drawer, page)
    /// </summary>
    public string FormMode { get; set; } = "modal";

    /// <summary>
    /// Mostrar acciones en cada fila
    /// </summary>
    public bool ShowRowActions { get; set; } = true;

    /// <summary>
    /// Posición de las acciones de fila (start, end)
    /// </summary>
    public string RowActionsPosition { get; set; } = "end";

    /// <summary>
    /// Configuración activa
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Orden de visualización en el menú
    /// </summary>
    public int MenuOrder { get; set; } = 0;

    /// <summary>
    /// Ruta del menú (ej: "catalogo/categorias")
    /// </summary>
    public string? MenuPath { get; set; }

    /// <summary>
    /// Permiso requerido para ver este grid
    /// </summary>
    public string? RequiredPermission { get; set; }

    /// <summary>
    /// Configuración JSON adicional
    /// </summary>
    public string? ExtraConfig { get; set; }

    // Navigation properties
    public ICollection<GridColumn> Columns { get; set; } = new List<GridColumn>();
}
