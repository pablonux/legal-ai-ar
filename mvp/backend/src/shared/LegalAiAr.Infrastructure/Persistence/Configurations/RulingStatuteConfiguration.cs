using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class RulingStatuteConfiguration : IEntityTypeConfiguration<RulingStatute>
{
    public void Configure(EntityTypeBuilder<RulingStatute> builder)
    {
        builder.ToTable("RulingStatutes");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.RulingId, e.StatuteId })
            .IsUnique();

        builder.Property(e => e.Articles)
            .HasMaxLength(500);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.Ruling)
            .WithMany(e => e.RulingStatutes)
            .HasForeignKey(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Statute)
            .WithMany(e => e.RulingStatutes)
            .HasForeignKey(e => e.StatuteId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.StructuredArticles)
            .WithOne(e => e.RulingStatute)
            .HasForeignKey(e => e.RulingStatuteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
