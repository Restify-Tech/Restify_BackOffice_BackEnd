namespace Restify.Auth.Application.DTOs.Roles;

/// <summary>
/// Request para actualizar un rol
/// </summary>
public record UpdateRoleRequest(
    string Name,
    string? Description,
    IEnumerable<Guid> PermissionIds
);
