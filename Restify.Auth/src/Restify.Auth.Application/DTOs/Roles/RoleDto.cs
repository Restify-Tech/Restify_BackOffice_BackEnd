namespace Restify.Auth.Application.DTOs.Roles;

/// <summary>
/// DTO completo de rol
/// </summary>
public record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystem,
    DateTime CreatedAt,
    IEnumerable<PermissionDto> Permissions
);

/// <summary>
/// DTO de permiso
/// </summary>
public record PermissionDto(
    Guid Id,
    string Code,
    string Name,
    string Module
);
