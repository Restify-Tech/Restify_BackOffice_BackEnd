namespace Restify.Auth.Application.DTOs.Roles;

/// <summary>
/// Request para crear un rol
/// </summary>
public record CreateRoleRequest(
    string Name,
    string? Description,
    IEnumerable<Guid> PermissionIds
);
