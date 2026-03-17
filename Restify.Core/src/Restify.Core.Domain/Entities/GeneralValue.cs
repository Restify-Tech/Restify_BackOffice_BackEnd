namespace Restify.Core.Domain.Entities;

/// <summary>
/// Valores dentro de un GeneralTable
/// Similar a tbGeneralValues del sistema POS
/// Cada valor pertenece a un grupo y tiene código único dentro del grupo
/// </summary>
public class GeneralValue : TenantEntity
{
    /// <summary>
    /// ID del grupo/tabla al que pertenece
    /// </summary>
    public Guid GeneralTableId { get; set; }

    /// <summary>
    /// Código único del valor dentro del grupo (ej: "PENDIENTE", "PROCESADO")
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Contenido/descripción del valor
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Descripción corta para mostrar en UI
    /// </summary>
    public string? ShortDescription { get; set; }

    /// <summary>
    /// Valor numérico asociado (opcional, para ordenamiento o cálculos)
    /// </summary>
    public decimal? NumericValue { get; set; }

    /// <summary>
    /// Referencia adicional 1 (uso flexible según el grupo)
    /// </summary>
    public string? Reference1 { get; set; }

    /// <summary>
    /// Referencia adicional 2
    /// </summary>
    public string? Reference2 { get; set; }

    /// <summary>
    /// Referencia adicional 3
    /// </summary>
    public string? Reference3 { get; set; }

    /// <summary>
    /// Referencia adicional 4
    /// </summary>
    public string? Reference4 { get; set; }

    /// <summary>
    /// Referencia adicional 5
    /// </summary>
    public string? Reference5 { get; set; }

    /// <summary>
    /// Icono asociado al valor (clase CSS: "fa fa-check green")
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Color de fondo (hex: "#FF0000")
    /// </summary>
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Color de texto (hex: "#FFFFFF")
    /// </summary>
    public string? TextColor { get; set; }

    /// <summary>
    /// Orden de visualización dentro del grupo
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Indica si el valor está bloqueado (no editable)
    /// </summary>
    public bool IsLocked { get; set; } = false;

    /// <summary>
    /// Indica si el valor está activo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Es el valor por defecto del grupo
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Configuración adicional en JSON
    /// </summary>
    public string? ExtraConfig { get; set; }

    // Navigation property
    public GeneralTable GeneralTable { get; set; } = null!;
}
