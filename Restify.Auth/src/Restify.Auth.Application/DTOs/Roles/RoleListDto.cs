namespace Restify.Auth.Application.DTOs.Roles;

/// <summary>
/// DTO resumido para listado de roles
/// </summary>
public record RoleListDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystem,
    int PermissionCount,
    int UserCount
);
