using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

public sealed class RulingReprocessRequestConfiguration : IEntityTypeConfiguration<RulingReprocessRequest>
{
    public void Configure(EntityTypeBuilder<RulingReprocessRequest> builder)
    {
        builder.ToTable("RulingReprocessRequests");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(x => x.RequestedBy).HasMaxLength(256);
        builder.Property(x => x.ErrorMessage).HasMaxLength(4000);

        builder.HasIndex(x => new { x.RulingId, x.Status });
        builder.HasIndex(x => x.DocumentId);
        builder.HasIndex(x => x.RequestedAt);

        builder.HasOne(x => x.Ruling)
            .WithMany()
            .HasForeignKey(x => x.RulingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Document)
            .WithMany()
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
