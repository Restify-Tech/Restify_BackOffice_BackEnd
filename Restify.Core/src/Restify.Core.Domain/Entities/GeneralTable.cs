namespace Restify.Core.Domain.Entities;

/// <summary>
/// Catálogo de grupos/tipos para GeneralValues
/// Similar a tbGeneralTables del sistema POS
/// Permite crear grupos jerárquicos de valores configurables
/// </summary>
public class GeneralTable : TenantEntity
{
    /// <summary>
    /// Código único del grupo (ej: "ESTADOS_PEDIDO", "TIPOS_PAGO")
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nombre descriptivo del grupo
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del grupo
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// ID del grupo padre (para jerarquías)
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Código de aplicación/módulo al que pertenece
    /// </summary>
    public string? ApplicationCode { get; set; }

    /// <summary>
    /// Indica si es un grupo interno del sistema (no editable por usuarios)
    /// </summary>
    public bool IsInternal { get; set; } = false;

    /// <summary>
    /// Indica si el grupo está activo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Orden de visualización
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Icono del grupo (clase CSS o nombre de icono)
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Configuración adicional en JSON
    /// </summary>
    public string? ExtraConfig { get; set; }

    // Navigation properties
    public GeneralTable? Parent { get; set; }
    public ICollection<GeneralTable> Children { get; set; } = new List<GeneralTable>();
    public ICollection<GeneralValue> Values { get; set; } = new List<GeneralValue>();
}
