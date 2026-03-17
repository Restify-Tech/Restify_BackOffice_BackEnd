using Microsoft.EntityFrameworkCore;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Common;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Interfaces;

namespace Restify.Auth.Infrastructure.Persistence;

/// <summary>
/// Contexto de base de datos principal
/// </summary>
public class AppDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly Guid? _currentTenantId;
    private readonly Guid? _currentUserId;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
        _currentTenantId = currentUserService.TenantId;
        _currentUserId = currentUserService.UserId;
    }

    // DbSets
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<ScreenPermission> ScreenPermissions => Set<ScreenPermission>();
    public DbSet<DeliveryZone> DeliveryZones => Set<DeliveryZone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas las configuraciones del assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Filtro global de tenant para entidades que implementan ITenantEntity
        if (_currentTenantId.HasValue)
        {
            modelBuilder.Entity<User>().HasQueryFilter(e => e.TenantId == _currentTenantId);
            modelBuilder.Entity<Role>().HasQueryFilter(e => e.TenantId == _currentTenantId);
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUserId;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
