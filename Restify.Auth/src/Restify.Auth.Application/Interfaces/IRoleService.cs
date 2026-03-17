using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Roles;

namespace Restify.Auth.Application.Interfaces;

/// <summary>
/// Servicio de gestión de roles
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Obtiene un rol por su ID
    /// </summary>
    Task<Result<RoleDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene lista paginada de roles
    /// </summary>
    Task<Result<PagedResponse<RoleListDto>>> GetAllAsync(PagedRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los permisos disponibles
    /// </summary>
    Task<Result<IEnumerable<PermissionDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea un nuevo rol
    /// </summary>
    Task<Result<RoleDto>> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un rol existente
    /// </summary>
    Task<Result<RoleDto>> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un rol
    /// </summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
