using Restify.Auth.Domain.Common;

namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Mapeo de pantallas del sistema a permisos requeridos para acceso
/// </summary>
public class ScreenPermission : BaseEntity
{
    /// <summary>
    /// Código único de la pantalla (ej: "dashboard", "orders.list", "kitchen")
    /// </summary>
    public string ScreenCode { get; set; } = string.Empty;

    /// <summary>
    /// Nombre legible de la pantalla
    /// </summary>
    public string ScreenName { get; set; } = string.Empty;

    /// <summary>
    /// Módulo al que pertenece (ej: "Operaciones", "Administracion")
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// Ruta del frontend asociada (ej: "/orders", "/users")
    /// </summary>
    public string Route { get; set; } = string.Empty;

    /// <summary>
    /// Código del permiso requerido para acceder (FK lógica a Permission.Code)
    /// </summary>
    public string RequiredPermission { get; set; } = string.Empty;

    /// <summary>
    /// Orden para mostrar en la UI
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Indica si la pantalla está activa
    /// </summary>
    public bool IsActive { get; set; } = true;
}
