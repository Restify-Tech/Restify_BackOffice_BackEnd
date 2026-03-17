using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.ScreenPermissions;

namespace Restify.Auth.Application.Interfaces;

/// <summary>
/// Servicio de gestión de permisos de pantalla
/// </summary>
public interface IScreenPermissionService
{
    /// <summary>
    /// Obtiene todas las pantallas con sus permisos requeridos
    /// </summary>
    Task<Result<IEnumerable<ScreenPermissionDto>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene pantallas agrupadas por módulo
    /// </summary>
    Task<Result<IEnumerable<ScreenPermissionsByModuleDto>>> GetByModuleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las pantallas accesibles para los permisos dados
    /// </summary>
    Task<Result<IEnumerable<ScreenPermissionDto>>> GetAccessibleScreensAsync(IEnumerable<string> userPermissions, CancellationToken cancellationToken = default);
}
