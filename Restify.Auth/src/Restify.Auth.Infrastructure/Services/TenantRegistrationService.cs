using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Tenant;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Constants;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Servicio para registro público de nuevos tenants (restaurantes)
/// </summary>
public class TenantRegistrationService : ITenantRegistrationService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly PasswordService _passwordService;
    private readonly ILogger<TenantRegistrationService> _logger;

    public TenantRegistrationService(
        AppDbContext context,
        JwtService jwtService,
        PasswordService passwordService,
        ILogger<TenantRegistrationService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<Result<TenantRegisterResponse>> RegisterAsync(
        TenantRegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validar contraseña mínima (defensa en profundidad)
        if (string.IsNullOrWhiteSpace(request.AdminPassword) || request.AdminPassword.Length < 8)
            return Result<TenantRegisterResponse>.Failure("La contraseña debe tener al menos 8 caracteres.");

        // Verificar unicidad del slug
        var slugExists = await _context.Tenants
            .AnyAsync(t => t.Slug == request.Slug, cancellationToken);

        if (slugExists)
            return Result<TenantRegisterResponse>.Failure("El slug ya está en uso. Elija otro identificador.");

        // Verificar unicidad del email (a nivel global, IgnoreQueryFilters)
        var emailExists = await _context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == request.AdminEmail, cancellationToken);

        if (emailExists)
            return Result<TenantRegisterResponse>.Failure("El email ya está registrado en el sistema.");

        // Usar transacción para garantizar atomicidad
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Crear Tenant
            var tenant = new Tenant
            {
                Name = request.BusinessName,
                Slug = request.Slug,
                BusinessName = request.BusinessName,
                Phone = request.Phone,
                Email = request.AdminEmail,
                Address = request.City,
                IdentificationType = request.IdentificationType,
                IdentificationNumber = request.IdentificationNumber,
                Status = TenantStatus.Active,
                OnboardingCompleted = false
            };

            await _context.Tenants.AddAsync(tenant, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // 2. Crear Rol Admin
            var adminRole = new Role
            {
                TenantId = tenant.Id,
                Name = SystemRoles.Administrador,
                NormalizedName = SystemRoles.Administrador.ToUpperInvariant(),
                Description = "Acceso completo al sistema",
                IsSystem = true
            };

            await _context.Roles.AddAsync(adminRole, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // 3. Asignar permisos base al rol Admin (DB-driven via IsTenantDefault)
            var permissions = await _context.Permissions
                .Where(p => p.IsTenantDefault && p.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var permission in permissions)
            {
                await _context.RolePermissions.AddAsync(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                }, cancellationToken);
            }

            // 4. Crear usuario Admin
            var adminUser = new User
            {
                TenantId = tenant.Id,
                Email = request.AdminEmail,
                PasswordHash = _passwordService.HashPassword(request.AdminPassword),
                FirstName = "Admin",
                LastName = request.BusinessName,
                Phone = request.Phone,
                Status = UserStatus.Active,
                EmailVerified = true
            };

            await _context.Users.AddAsync(adminUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // 5. Asignar rol Admin al usuario
            await _context.UserRoles.AddAsync(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            // 6. Generar tokens JWT
            var roles = new List<string> { adminRole.Name };
            var permissionCodes = permissions.Select(p => p.Code).ToList();

            var accessToken = _jwtService.GenerateAccessToken(adminUser, roles, permissionCodes);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Guardar refresh token
            adminUser.RefreshToken = refreshToken;
            adminUser.RefreshTokenExpiresAt = _jwtService.GetRefreshTokenExpiration();
            adminUser.LastLoginAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Nuevo tenant registrado: {Slug} ({TenantId}) con admin {Email}",
                request.Slug, tenant.Id, request.AdminEmail);

            return Result<TenantRegisterResponse>.Success(new TenantRegisterResponse(
                accessToken,
                refreshToken,
                DateTime.UtcNow.AddMinutes(60),
                tenant.Id,
                adminUser.Id
            ));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error al registrar tenant {Slug}", request.Slug);
            return Result<TenantRegisterResponse>.Failure("Ocurrió un error al registrar el restaurante. Intente nuevamente.");
        }
    }
}
