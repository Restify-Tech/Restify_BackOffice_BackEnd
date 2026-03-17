using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Roles;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de gestión de roles
/// </summary>
public class RoleService : IRoleService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        AppDbContext context,
        ICurrentUserService currentUser,
        ILogger<RoleService> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<RoleDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role == null)
            return Result<RoleDto>.Failure("Rol no encontrado");

        return Result<RoleDto>.Success(MapToDto(role));
    }

    public async Task<Result<PagedResponse<RoleListDto>>> GetAllAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Roles
            .Include(r => r.RolePermissions)
            .Include(r => r.UserRoles)
            .AsQueryable();

        // Filtrar por búsqueda
        if (!string.IsNullOrEmpty(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(r =>
                r.Name.ToLower().Contains(search) ||
                (r.Description != null && r.Description.ToLower().Contains(search)));
        }

        // Ordenar
        query = request.OrderBy?.ToLower() switch
        {
            "name" => request.Descending ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
            _ => query.OrderBy(r => r.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var roles = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = roles.Select(r => new RoleListDto(
            r.Id,
            r.Name,
            r.Description,
            r.IsSystem,
            r.RolePermissions.Count,
            r.UserRoles.Count
        ));

        return Result<PagedResponse<RoleListDto>>.Success(new PagedResponse<RoleListDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount,
            totalPages
        ));
    }

    public async Task<Result<IEnumerable<PermissionDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _context.Permissions
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);

        var result = permissions.Select(p => new PermissionDto(
            p.Id,
            p.Code,
            p.Name,
            p.Module
        ));

        return Result<IEnumerable<PermissionDto>>.Success(result);
    }

    public async Task<Result<RoleDto>> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        // Verificar nombre único
        var nameExists = await _context.Roles
            .AnyAsync(r => r.NormalizedName == request.Name.ToUpperInvariant(), cancellationToken);

        if (nameExists)
            return Result<RoleDto>.Failure("Ya existe un rol con ese nombre");

        // Verificar permisos existen
        var permissions = await _context.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (permissions.Count != request.PermissionIds.Count())
            return Result<RoleDto>.Failure("Uno o más permisos no existen");

        var role = new Role
        {
            TenantId = _currentUser.TenantId!.Value,
            Name = request.Name,
            NormalizedName = request.Name.ToUpperInvariant(),
            Description = request.Description,
            IsSystem = false
        };

        await _context.Roles.AddAsync(role, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Asignar permisos
        foreach (var permission in permissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            }, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Rol creado: {RoleName}", role.Name);

        // Recargar con permisos
        var createdRole = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstAsync(r => r.Id == role.Id, cancellationToken);

        return Result<RoleDto>.Success(MapToDto(createdRole));
    }

    public async Task<Result<RoleDto>> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role == null)
            return Result<RoleDto>.Failure("Rol no encontrado");

        if (role.IsSystem)
            return Result<RoleDto>.Failure("No se puede modificar un rol del sistema");

        // Verificar nombre único si cambió
        if (role.NormalizedName != request.Name.ToUpperInvariant())
        {
            var nameExists = await _context.Roles
                .AnyAsync(r => r.NormalizedName == request.Name.ToUpperInvariant() && r.Id != id, cancellationToken);

            if (nameExists)
                return Result<RoleDto>.Failure("Ya existe un rol con ese nombre");
        }

        // Verificar permisos existen
        var permissions = await _context.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (permissions.Count != request.PermissionIds.Count())
            return Result<RoleDto>.Failure("Uno o más permisos no existen");

        role.Name = request.Name;
        role.NormalizedName = request.Name.ToUpperInvariant();
        role.Description = request.Description;

        // Actualizar permisos
        _context.RolePermissions.RemoveRange(role.RolePermissions);

        foreach (var permission in permissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            }, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Rol actualizado: {RoleId}", id);

        // Recargar con permisos
        var updatedRole = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstAsync(r => r.Id == id, cancellationToken);

        return Result<RoleDto>.Success(MapToDto(updatedRole));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role == null)
            return Result.Failure("Rol no encontrado");

        if (role.IsSystem)
            return Result.Failure("No se puede eliminar un rol del sistema");

        if (role.UserRoles.Any())
            return Result.Failure("No se puede eliminar un rol asignado a usuarios");

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Rol eliminado: {RoleId}", id);

        return Result.Success();
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto(
            role.Id,
            role.Name,
            role.Description,
            role.IsSystem,
            role.CreatedAt,
            role.RolePermissions.Select(rp => new PermissionDto(
                rp.Permission.Id,
                rp.Permission.Code,
                rp.Permission.Name,
                rp.Permission.Module
            ))
        );
    }
}
