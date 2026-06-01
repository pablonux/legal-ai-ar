using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class ExternalIdentifierConfiguration : IEntityTypeConfiguration<ExternalIdentifier>
{
    public void Configure(EntityTypeBuilder<ExternalIdentifier> builder)
    {
        builder.ToTable("ExternalIdentifiers");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityType)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.EntityId)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.ExternalCode)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(e => e.Label)
            .HasMaxLength(200);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => new { e.EntityType, e.EntityId, e.SourceId, e.ExternalCode })
            .IsUnique();
        builder.HasIndex(e => new { e.SourceId, e.ExternalCode });

        builder.HasOne(e => e.Source)
            .WithMany()
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
