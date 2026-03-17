namespace Restify.Auth.Application.DTOs.ScreenPermissions;

/// <summary>
/// DTO de mapeo pantalla-permiso
/// </summary>
public record ScreenPermissionDto(
    Guid Id,
    string ScreenCode,
    string ScreenName,
    string Module,
    string Route,
    string RequiredPermission,
    int DisplayOrder,
    bool IsActive
);

/// <summary>
/// DTO para listar pantallas agrupadas por módulo
/// </summary>
public record ScreenPermissionsByModuleDto(
    string Module,
    IEnumerable<ScreenPermissionDto> Screens
);
