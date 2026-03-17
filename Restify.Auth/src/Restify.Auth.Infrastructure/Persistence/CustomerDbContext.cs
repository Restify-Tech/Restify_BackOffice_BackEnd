using Microsoft.EntityFrameworkCore;

namespace Restify.Auth.Infrastructure.Persistence;

/// <summary>
/// DbContext de solo lectura/escritura para la tabla de Customers del schema backoffice.
/// Usado por el servicio Auth para autenticar clientes sin acoplamiento HTTP con BackOffice.
/// </summary>
public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }

    public DbSet<CustomerRecord> Customers => Set<CustomerRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("backoffice");

        modelBuilder.Entity<CustomerRecord>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.TenantId).IsRequired();
            entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
            entity.Property(c => c.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Phone).HasMaxLength(20);
            entity.Property(c => c.RefreshToken).HasMaxLength(500);
            entity.HasIndex(c => new { c.TenantId, c.Email }).IsUnique();
        });
    }
}

/// <summary>
/// Registro ligero para operaciones de autenticación de clientes (mapea a backoffice.Customers)
/// </summary>
public class CustomerRecord
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
