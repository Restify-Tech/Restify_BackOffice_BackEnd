using Restify.Core.Domain.Enums;

namespace Restify.Core.Domain.Entities;

/// <summary>
/// Regla de validación para una columna del grid
/// </summary>
public class GridColumnValidation : BaseEntity
{
    /// <summary>
    /// Referencia a la columna
    /// </summary>
    public Guid GridColumnId { get; set; }

    /// <summary>
    /// Tipo de validación
    /// </summary>
    public ValidationType ValidationType { get; set; }

    /// <summary>
    /// Valor de la validación (ej: "5" para MinLength)
    /// </summary>
    public string? ValidationValue { get; set; }

    /// <summary>
    /// Segundo valor para validaciones de rango
    /// </summary>
    public string? ValidationValue2 { get; set; }

    /// <summary>
    /// Mensaje de error personalizado
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Orden de ejecución de la validación
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Validación activa
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Validar solo en cliente
    /// </summary>
    public bool ClientOnly { get; set; } = false;

    /// <summary>
    /// Validar solo en servidor
    /// </summary>
    public bool ServerOnly { get; set; } = false;

    // Navigation property
    public GridColumn GridColumn { get; set; } = null!;
}
