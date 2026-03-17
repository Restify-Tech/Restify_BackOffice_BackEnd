using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.Property(c => c.IdentificationNumber)
            .HasMaxLength(20);

        builder.Property(c => c.IdentificationType)
            .HasConversion<int?>();

        builder.Property(c => c.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.Property(c => c.EmailVerified)
            .IsRequired();

        builder.Property(c => c.LastLoginAt);

        builder.Property(c => c.RefreshToken)
            .HasMaxLength(500);

        builder.Property(c => c.RefreshTokenExpiresAt);

        builder.Property(c => c.TenantId)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        // Ignorar propiedad calculada
        builder.Ignore(c => c.FullName);

        // Relaciones
        builder.HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => new { c.TenantId, c.Email }).IsUnique();
    }
}
