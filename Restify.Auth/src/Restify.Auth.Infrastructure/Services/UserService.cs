using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Users;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de gestión de usuarios
/// </summary>
public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly PasswordService _passwordService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UserService> _logger;

    public UserService(
        AppDbContext context,
        PasswordService passwordService,
        ICurrentUserService currentUser,
        ILogger<UserService> logger)
    {
        _context = context;
        _passwordService = passwordService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return Result<UserDto>.Failure("Usuario no encontrado");

        return Result<UserDto>.Success(MapToDto(user));
    }

    public async Task<Result<PagedResponse<UserListDto>>> GetAllAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsQueryable();

        // Filtrar por búsqueda
        if (!string.IsNullOrEmpty(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(search) ||
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search));
        }

        // Ordenar
        query = request.OrderBy?.ToLower() switch
        {
            "email" => request.Descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "name" => request.Descending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            "lastlogin" => request.Descending ? query.OrderByDescending(u => u.LastLoginAt) : query.OrderBy(u => u.LastLoginAt),
            _ => query.OrderByDescending(u => u.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var users = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = users.Select(u => new UserListDto(
            u.Id,
            u.Email,
            u.Username,
            $"{u.FirstName} {u.LastName}",
            u.Status,
            u.LastLoginAt,
            u.UserRoles.Select(ur => ur.Role.Name)
        ));

        return Result<PagedResponse<UserListDto>>.Success(new PagedResponse<UserListDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount,
            totalPages
        ));
    }

    public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Verificar email único
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
            return Result<UserDto>.Failure("El email ya está registrado");

        // Verificar roles existen
        var roles = await _context.Roles
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        if (roles.Count != request.RoleIds.Count())
            return Result<UserDto>.Failure("Uno o más roles no existen");

        // Verificar username único si se proporciona
        if (!string.IsNullOrEmpty(request.Username))
        {
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == request.Username, cancellationToken);

            if (usernameExists)
                return Result<UserDto>.Failure("El nombre de usuario ya está registrado");
        }

        var user = new User
        {
            TenantId = _currentUser.TenantId!.Value,
            Email = request.Email,
            Username = request.Username,
            PasswordHash = _passwordService.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Status = UserStatus.Active,
            EmailVerified = true
        };

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Asignar roles
        foreach (var role in roles)
        {
            await _context.UserRoles.AddAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            }, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Usuario creado: {Email}", user.Email);

        // Recargar con roles
        var createdUser = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstAsync(u => u.Id == user.Id, cancellationToken);

        return Result<UserDto>.Success(MapToDto(createdUser));
    }

    public async Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return Result<UserDto>.Failure("Usuario no encontrado");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;

        if (request.Status.HasValue)
            user.Status = request.Status.Value;

        // Actualizar roles si se proporcionan
        if (request.RoleIds != null)
        {
            var roles = await _context.Roles
                .Where(r => request.RoleIds.Contains(r.Id))
                .ToListAsync(cancellationToken);

            if (roles.Count != request.RoleIds.Count())
                return Result<UserDto>.Failure("Uno o más roles no existen");

            // Eliminar roles actuales
            _context.UserRoles.RemoveRange(user.UserRoles);

            // Agregar nuevos roles
            foreach (var role in roles)
            {
                await _context.UserRoles.AddAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                }, cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Usuario actualizado: {UserId}", id);

        // Recargar con roles
        var updatedUser = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstAsync(u => u.Id == id, cancellationToken);

        return Result<UserDto>.Success(MapToDto(updatedUser));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return Result.Failure("Usuario no encontrado");

        // Soft delete
        user.Status = UserStatus.Inactive;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Usuario eliminado (soft): {UserId}", id);

        return Result.Success();
    }

    public async Task<Result<string>> ResetPasswordAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return Result<string>.Failure("Usuario no encontrado");

        var tempPassword = GenerateTemporaryPassword();
        user.PasswordHash = _passwordService.HashPassword(tempPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contraseña reseteada para usuario: {UserId}", id);

        return Result<string>.Success(tempPassword);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto(
            user.Id,
            user.Email,
            user.Username,
            user.FirstName,
            user.LastName,
            user.Phone,
            user.AvatarUrl,
            user.Status,
            user.EmailVerified,
            user.LastLoginAt,
            user.CreatedAt,
            user.UpdatedAt,
            user.UserRoles.Select(ur => ur.Role.Name)
        );
    }

    private static string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
