using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class TableReservationConfiguration : IEntityTypeConfiguration<TableReservation>
{
    public void Configure(EntityTypeBuilder<TableReservation> builder)
    {
        builder.ToTable("TableReservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CustomerPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CustomerEmail)
            .HasMaxLength(200);

        builder.Property(x => x.SpecialRequests)
            .HasMaxLength(1000);

        builder.Property(x => x.ConfirmationCode)
            .HasMaxLength(10);

        builder.Property(x => x.CancelledBy)
            .HasMaxLength(200);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(500);

        builder.HasIndex(x => new { x.TenantId, x.ReservationDateTime });
        builder.HasIndex(x => new { x.TenantId, x.Status });

        builder.HasOne(x => x.Table)
            .WithMany()
            .HasForeignKey(x => x.TableId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
