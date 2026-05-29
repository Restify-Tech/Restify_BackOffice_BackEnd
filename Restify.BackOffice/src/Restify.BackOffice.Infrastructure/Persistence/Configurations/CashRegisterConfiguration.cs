using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class CashRegisterConfiguration : IEntityTypeConfiguration<CashRegister>
{
    public void Configure(EntityTypeBuilder<CashRegister> builder)
    {
        builder.ToTable("CashRegisters");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cr => cr.Description)
            .HasMaxLength(500);

        builder.Property(cr => cr.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cr => cr.IsActive)
            .IsRequired();

        // Relación con sesión actual (opcional)
        builder.HasOne(cr => cr.CurrentSession)
            .WithMany()
            .HasForeignKey(cr => cr.CurrentSessionId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación con todas las sesiones
        builder.HasMany(cr => cr.Sessions)
            .WithOne(s => s.CashRegister)
            .HasForeignKey(s => s.CashRegisterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacion con sucursal
        builder.HasOne(cr => cr.Branch)
            .WithMany(b => b.CashRegisters)
            .HasForeignKey(cr => cr.BranchId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(cr => cr.TenantId);
        builder.HasIndex(cr => cr.Name);
        builder.HasIndex(cr => cr.Status);
        builder.HasIndex(cr => cr.BranchId);
    }
}

public class CashRegisterSessionConfiguration : IEntityTypeConfiguration<CashRegisterSession>
{
    public void Configure(EntityTypeBuilder<CashRegisterSession> builder)
    {
        builder.ToTable("CashRegisterSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.OpenedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.OpenedAt)
            .IsRequired();

        builder.Property(s => s.OpeningBalance)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.ClosedBy)
            .HasMaxLength(100);

        builder.Property(s => s.ExpectedClosingBalance)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.ActualClosingBalance)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Difference)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.ClosingNotes)
            .HasMaxLength(1000);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<int>();

        // Relación con caja registradora
        builder.HasOne(s => s.CashRegister)
            .WithMany(cr => cr.Sessions)
            .HasForeignKey(s => s.CashRegisterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con movimientos
        builder.HasMany(s => s.Movements)
            .WithOne(m => m.Session)
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Propiedades calculadas ignoradas (no mapear a BD)
        builder.Ignore(s => s.TotalSales);
        builder.Ignore(s => s.TotalWithdrawals);
        builder.Ignore(s => s.TotalDeposits);
        builder.Ignore(s => s.TotalExpenses);

        // Índices
        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => s.CashRegisterId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.OpenedAt);
    }
}

public class CashRegisterMovementConfiguration : IEntityTypeConfiguration<CashRegisterMovement>
{
    public void Configure(EntityTypeBuilder<CashRegisterMovement> builder)
    {
        builder.ToTable("CashRegisterMovements");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(m => m.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.Notes)
            .HasMaxLength(1000);

        builder.Property(m => m.ReferenceType)
            .HasMaxLength(50);

        builder.Property(m => m.MovementDate)
            .IsRequired();

        builder.Property(m => m.RegisteredBy)
            .HasMaxLength(100);

        // Relación con sesión
        builder.HasOne(m => m.Session)
            .WithMany(s => s.Movements)
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(m => m.TenantId);
        builder.HasIndex(m => m.SessionId);
        builder.HasIndex(m => m.Type);
        builder.HasIndex(m => m.MovementDate);
        builder.HasIndex(m => new { m.ReferenceId, m.ReferenceType });
    }
}
