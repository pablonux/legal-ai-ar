using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class RulingSourceMetadataConfiguration : IEntityTypeConfiguration<RulingSourceMetadata>
{
    public void Configure(EntityTypeBuilder<RulingSourceMetadata> builder)
    {
        builder.ToTable("RulingSourceMetadata");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Key)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.Value)
            .HasMaxLength(4000);

        builder.HasIndex(e => new { e.RulingId, e.SourceId, e.Key })
            .IsUnique();

        builder.HasOne(e => e.Ruling)
            .WithMany(e => e.SourceMetadata)
            .HasForeignKey(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Source)
            .WithMany()
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
