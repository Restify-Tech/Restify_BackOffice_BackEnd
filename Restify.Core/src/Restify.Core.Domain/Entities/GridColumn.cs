using Restify.Core.Domain.Enums;

namespace Restify.Core.Domain.Entities;

/// <summary>
/// Configuración de una columna del grid
/// </summary>
public class GridColumn : BaseEntity
{
    /// <summary>
    /// Referencia al grid padre
    /// </summary>
    public Guid GridConfigurationId { get; set; }

    /// <summary>
    /// Nombre del campo en la entidad/DTO
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Título que se muestra en el header del grid
    /// </summary>
    public string HeaderText { get; set; } = string.Empty;

    /// <summary>
    /// Label que se muestra en formularios (si difiere del header)
    /// </summary>
    public string? FormLabel { get; set; }

    /// <summary>
    /// Tooltip del header
    /// </summary>
    public string? HeaderTooltip { get; set; }

    /// <summary>
    /// Tipo de dato de la columna
    /// </summary>
    public GridColumnType ColumnType { get; set; } = GridColumnType.String;

    /// <summary>
    /// Tipo de editor para formularios
    /// </summary>
    public EditorType EditorType { get; set; } = EditorType.TextBox;

    /// <summary>
    /// Orden de la columna en el grid (izquierda a derecha)
    /// </summary>
    public int GridOrder { get; set; }

    /// <summary>
    /// Orden del campo en el formulario
    /// </summary>
    public int FormOrder { get; set; }

    /// <summary>
    /// Grupo/sección del formulario donde aparece este campo
    /// </summary>
    public string? FormGroup { get; set; }

    /// <summary>
    /// Ancho de la columna (px, %, auto)
    /// </summary>
    public string? Width { get; set; }

    /// <summary>
    /// Ancho mínimo de la columna
    /// </summary>
    public string? MinWidth { get; set; }

    /// <summary>
    /// Ancho máximo de la columna
    /// </summary>
    public string? MaxWidth { get; set; }

    /// <summary>
    /// Alineación del contenido
    /// </summary>
    public GridColumnAlignment Alignment { get; set; } = GridColumnAlignment.Left;

    /// <summary>
    /// Columna visible en el grid
    /// </summary>
    public bool IsVisibleInGrid { get; set; } = true;

    /// <summary>
    /// Campo visible en el formulario de creación
    /// </summary>
    public bool IsVisibleInCreate { get; set; } = true;

    /// <summary>
    /// Campo visible en el formulario de edición
    /// </summary>
    public bool IsVisibleInEdit { get; set; } = true;

    /// <summary>
    /// Campo visible en la vista de detalle
    /// </summary>
    public bool IsVisibleInView { get; set; } = true;

    /// <summary>
    /// Campo incluido en exportación
    /// </summary>
    public bool IsExportable { get; set; } = true;

    /// <summary>
    /// Permite ordenar por esta columna
    /// </summary>
    public bool IsSortable { get; set; } = true;

    /// <summary>
    /// Permite filtrar por esta columna
    /// </summary>
    public bool IsFilterable { get; set; } = true;

    /// <summary>
    /// Columna incluida en búsqueda global
    /// </summary>
    public bool IsSearchable { get; set; } = true;

    /// <summary>
    /// Permite edición inline
    /// </summary>
    public bool IsInlineEditable { get; set; } = true;

    /// <summary>
    /// Es campo requerido
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Es clave primaria
    /// </summary>
    public bool IsPrimaryKey { get; set; } = false;

    /// <summary>
    /// Es campo de solo lectura
    /// </summary>
    public bool IsReadOnly { get; set; } = false;

    /// <summary>
    /// Columna fija (frozen)
    /// </summary>
    public bool IsFrozen { get; set; } = false;

    /// <summary>
    /// Posición de columna fija (left/right)
    /// </summary>
    public string? FrozenPosition { get; set; }

    /// <summary>
    /// Formato de visualización (ej: "dd/MM/yyyy", "#,##0.00")
    /// </summary>
    public string? DisplayFormat { get; set; }

    /// <summary>
    /// Valor por defecto para nuevos registros
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Placeholder para inputs
    /// </summary>
    public string? Placeholder { get; set; }

    /// <summary>
    /// Texto de ayuda para el campo
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Prefijo para el valor (ej: "$")
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// Sufijo para el valor (ej: "%")
    /// </summary>
    public string? Suffix { get; set; }

    /// <summary>
    /// Número de columnas en el grid del formulario (1-12)
    /// </summary>
    public int FormColumnSpan { get; set; } = 6;

    /// <summary>
    /// Clases CSS personalizadas para la columna
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Nombre del componente personalizado para renderizar la celda
    /// </summary>
    public string? CellTemplate { get; set; }

    /// <summary>
    /// Nombre del componente personalizado para el editor
    /// </summary>
    public string? EditorTemplate { get; set; }

    /// <summary>
    /// Opciones para select/enum (JSON array)
    /// </summary>
    public string? SelectOptions { get; set; }

    /// <summary>
    /// Expresión condicional para visibilidad (JSON)
    /// </summary>
    public string? VisibilityCondition { get; set; }

    /// <summary>
    /// Expresión condicional para habilitado (JSON)
    /// </summary>
    public string? EnabledCondition { get; set; }

    /// <summary>
    /// Configuración JSON adicional
    /// </summary>
    public string? ExtraConfig { get; set; }

    // Navigation properties
    public GridConfiguration GridConfiguration { get; set; } = null!;
    public ICollection<GridColumnValidation> Validations { get; set; } = new List<GridColumnValidation>();
    public GridColumnLookup? Lookup { get; set; }
}
