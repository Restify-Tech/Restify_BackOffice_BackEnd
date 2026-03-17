using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class QRCodeConfiguration : IEntityTypeConfiguration<QRCode>
{
    public void Configure(EntityTypeBuilder<QRCode> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(q => q.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(q => new { q.TenantId, q.Code })
            .IsUnique();

        builder.HasOne(q => q.Table)
            .WithMany()
            .HasForeignKey(q => q.TableId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
