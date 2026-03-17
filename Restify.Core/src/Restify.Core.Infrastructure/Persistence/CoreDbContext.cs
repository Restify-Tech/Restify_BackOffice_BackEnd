using Microsoft.EntityFrameworkCore;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence;

public class CoreDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly Guid? _currentTenantId;

    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
    {
    }

    public CoreDbContext(DbContextOptions<CoreDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
        _currentTenantId = currentUserService.TenantId;
    }

    public DbSet<GridConfiguration> GridConfigurations => Set<GridConfiguration>();
    public DbSet<GridColumn> GridColumns => Set<GridColumn>();
    public DbSet<GridColumnValidation> GridColumnValidations => Set<GridColumnValidation>();
    public DbSet<GridColumnLookup> GridColumnLookups => Set<GridColumnLookup>();

    // GeneralTables y GeneralValues
    public DbSet<GeneralTable> GeneralTables => Set<GeneralTable>();
    public DbSet<GeneralValue> GeneralValues => Set<GeneralValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Esquema separado para Core
        modelBuilder.HasDefaultSchema("core");

        // Aplicar configuraciones
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);

        // Query filter para multi-tenancy
        modelBuilder.Entity<GridConfiguration>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<GeneralTable>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<GeneralValue>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService?.Email;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUserService?.Email;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
            {
                entry.Entity.TenantId = _currentTenantId ?? Guid.Empty;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
