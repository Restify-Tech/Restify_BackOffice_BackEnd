using Restify.Auth.Domain.Common;

namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Plan de suscripción del SaaS Restify
/// </summary>
public class Plan : AuditableEntity
{
    /// <summary>
    /// Nombre del plan (ej: "Básico", "Pro", "Enterprise")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del plan
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Precio mensual en USD
    /// </summary>
    public decimal MonthlyPrice { get; set; }

    /// <summary>
    /// Precio anual en USD
    /// </summary>
    public decimal AnnualPrice { get; set; }

    /// <summary>
    /// Máximo de usuarios permitidos (0 = ilimitado)
    /// </summary>
    public int MaxUsers { get; set; } = 5;

    /// <summary>
    /// Máximo de sucursales permitidas (0 = ilimitado)
    /// </summary>
    public int MaxBranches { get; set; } = 1;

    /// <summary>
    /// Indica si el plan está activo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Orden de presentación en la UI
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Color del badge en la UI (hex)
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Plan por defecto al crear un nuevo tenant
    /// </summary>
    public bool IsDefault { get; set; } = false;

    // Navegación
    public virtual ICollection<PlanScreenPermission> PlanScreenPermissions { get; set; } = new List<PlanScreenPermission>();
    public virtual ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
}
