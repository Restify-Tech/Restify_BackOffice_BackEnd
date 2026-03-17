using Restify.Auth.Domain.Common;
using Restify.Auth.Domain.Enums;

namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Representa un Tenant (Restaurante) en el sistema multi-tenant
/// </summary>
public class Tenant : AuditableEntity
{
    /// <summary>
    /// Nombre comercial del restaurante
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Slug único para URLs (ej: "la-parrilla")
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// RUC del restaurante para facturación
    /// </summary>
    public string? Ruc { get; set; }

    /// <summary>
    /// Razón social para facturación
    /// </summary>
    public string? BusinessName { get; set; }

    /// <summary>
    /// Dirección del restaurante
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email de contacto
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// URL del logo del restaurante
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Color primario de la marca (hex)
    /// </summary>
    public string PrimaryColor { get; set; } = "#1976D2";

    /// <summary>
    /// Color secundario de la marca (hex)
    /// </summary>
    public string SecondaryColor { get; set; } = "#FF9800";

    /// <summary>
    /// Código de moneda ISO 4217 (ej: USD, EUR)
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Porcentaje de IVA
    /// </summary>
    public decimal TaxPercentage { get; set; } = 15.00m;

    /// <summary>
    /// Zona horaria del restaurante
    /// </summary>
    public string TimeZone { get; set; } = "America/Guayaquil";

    /// <summary>
    /// Estado del tenant
    /// </summary>
    public TenantStatus Status { get; set; } = TenantStatus.Active;

    /// <summary>
    /// Fecha de expiración del periodo de prueba
    /// </summary>
    public DateTime? TrialExpiresAt { get; set; }

    // --- Delivery & Onboarding (Fase 1) ---

    /// <summary>
    /// Modo de operación de delivery (Standalone, Networked, Hybrid)
    /// </summary>
    public DeliveryOperationMode DeliveryOperationMode { get; set; } = DeliveryOperationMode.Standalone;

    /// <summary>
    /// Zona de delivery asignada (para modos Networked/Hybrid)
    /// </summary>
    public Guid? DeliveryZoneId { get; set; }
    public DeliveryZone? DeliveryZone { get; set; }

    /// <summary>
    /// URL de la firma electrónica del restaurante (imagen PNG/JPG, no obligatoria)
    /// </summary>
    public string? SignatureUrl { get; set; }

    /// <summary>
    /// Dirección completa del restaurante
    /// </summary>
    public string? FullAddress { get; set; }

    /// <summary>
    /// Latitud para Google Maps
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Longitud para Google Maps
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Tipo de identificación del propietario/empresa (Cédula o RUC)
    /// </summary>
    public IdentificationType? IdentificationType { get; set; }

    /// <summary>
    /// Número de cédula o RUC
    /// </summary>
    public string? IdentificationNumber { get; set; }

    /// <summary>
    /// Indica si el tenant completó el wizard de onboarding
    /// </summary>
    public bool OnboardingCompleted { get; set; }

    // Navegación
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
