using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.ScreenPermissions;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de permisos de pantalla
/// </summary>
public class ScreenPermissionService : IScreenPermissionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ScreenPermissionService> _logger;

    public ScreenPermissionService(
        AppDbContext context,
        ILogger<ScreenPermissionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ScreenPermissionDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var screens = await _context.ScreenPermissions
            .Where(sp => sp.IsActive)
            .OrderBy(sp => sp.Module)
            .ThenBy(sp => sp.DisplayOrder)
            .ToListAsync(cancellationToken);

        var result = screens.Select(sp => new ScreenPermissionDto(
            sp.Id,
            sp.ScreenCode,
            sp.ScreenName,
            sp.Module,
            sp.Route,
            sp.RequiredPermission,
            sp.DisplayOrder,
            sp.IsActive
        ));

        return Result<IEnumerable<ScreenPermissionDto>>.Success(result);
    }

    public async Task<Result<IEnumerable<ScreenPermissionsByModuleDto>>> GetByModuleAsync(CancellationToken cancellationToken = default)
    {
        var screens = await _context.ScreenPermissions
            .Where(sp => sp.IsActive)
            .OrderBy(sp => sp.Module)
            .ThenBy(sp => sp.DisplayOrder)
            .ToListAsync(cancellationToken);

        var grouped = screens
            .GroupBy(sp => sp.Module)
            .Select(g => new ScreenPermissionsByModuleDto(
                g.Key,
                g.Select(sp => new ScreenPermissionDto(
                    sp.Id,
                    sp.ScreenCode,
                    sp.ScreenName,
                    sp.Module,
                    sp.Route,
                    sp.RequiredPermission,
                    sp.DisplayOrder,
                    sp.IsActive
                ))
            ));

        return Result<IEnumerable<ScreenPermissionsByModuleDto>>.Success(grouped);
    }

    public async Task<Result<IEnumerable<ScreenPermissionDto>>> GetAccessibleScreensAsync(
        IEnumerable<string> userPermissions,
        CancellationToken cancellationToken = default)
    {
        var permissionSet = userPermissions.ToHashSet();

        var screens = await _context.ScreenPermissions
            .Where(sp => sp.IsActive)
            .OrderBy(sp => sp.Module)
            .ThenBy(sp => sp.DisplayOrder)
            .ToListAsync(cancellationToken);

        var accessible = screens
            .Where(sp => permissionSet.Contains(sp.RequiredPermission))
            .Select(sp => new ScreenPermissionDto(
                sp.Id,
                sp.ScreenCode,
                sp.ScreenName,
                sp.Module,
                sp.Route,
                sp.RequiredPermission,
                sp.DisplayOrder,
                sp.IsActive
            ));

        return Result<IEnumerable<ScreenPermissionDto>>.Success(accessible);
    }
}
