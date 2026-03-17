using Microsoft.EntityFrameworkCore;

namespace Restify.Auth.Infrastructure.Persistence;

/// <summary>
/// DbContext para la tabla DeliveryDrivers del schema backoffice.
/// Usado por Auth para autenticar motorizados sin acoplamiento HTTP con BackOffice.
/// </summary>
public class DriverDbContext : DbContext
{
    public DriverDbContext(DbContextOptions<DriverDbContext> options) : base(options) { }

    public DbSet<DriverRecord> Drivers => Set<DriverRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("backoffice");

        modelBuilder.Entity<DriverRecord>(entity =>
        {
            entity.ToTable("DeliveryDrivers");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.TenantId).IsRequired();
            entity.Property(d => d.Email).IsRequired().HasMaxLength(200);
            entity.Property(d => d.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(d => d.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(d => d.LastName).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Phone).IsRequired().HasMaxLength(20);
            entity.Property(d => d.IdentificationNumber).IsRequired().HasMaxLength(20);
            entity.Property(d => d.VehiclePlate).HasMaxLength(20);
            entity.Property(d => d.VehicleDescription).HasMaxLength(200);
            entity.Property(d => d.RefreshToken).HasMaxLength(500);
            entity.HasIndex(d => new { d.TenantId, d.Email }).IsUnique();
        });
    }
}

/// <summary>
/// Registro ligero para operaciones de autenticación de motorizados
/// </summary>
public class DriverRecord
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string? VehiclePlate { get; set; }
    public string? VehicleDescription { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPoolDriver { get; set; }
    public Guid? DeliveryZoneId { get; set; }
    public int? VehicleType { get; set; }
    public int VerificationStatus { get; set; }
}
