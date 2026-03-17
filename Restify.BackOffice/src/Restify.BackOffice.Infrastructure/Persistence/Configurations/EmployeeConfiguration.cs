using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.IdentificationNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.Position)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Department)
            .HasMaxLength(200);

        builder.Property(e => e.HireDate)
            .IsRequired();

        builder.Property(e => e.TerminationDate);

        builder.Property(e => e.BaseSalary)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.EmploymentType)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.BankName)
            .HasMaxLength(200);

        builder.Property(e => e.BankAccountNumber)
            .HasMaxLength(50);

        builder.Property(e => e.SocialSecurityNumber)
            .HasMaxLength(50);

        builder.Property(e => e.IsActive)
            .IsRequired();

        builder.Property(e => e.TenantId)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        // Indices
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => new { e.TenantId, e.IdentificationNumber }).IsUnique();
        builder.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();
    }
}
