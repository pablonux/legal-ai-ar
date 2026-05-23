using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class CitationConfiguration : IEntityTypeConfiguration<Citation>
{
    public void Configure(EntityTypeBuilder<Citation> builder)
    {
        builder.ToTable("Citations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExternalAlias)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(e => e.CitationType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.CitationText)
            .HasMaxLength(500);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => e.CsjnFalloId);
        builder.HasIndex(e => e.TargetRulingId);
        builder.HasIndex(e => e.ExternalAlias);

        builder.HasOne(e => e.SourceRuling)
            .WithMany(e => e.OutboundCitations)
            .HasForeignKey(e => e.SourceRulingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.TargetRuling)
            .WithMany(e => e.InboundCitations)
            .HasForeignKey(e => e.TargetRulingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CitedStatute)
            .WithMany()
            .HasForeignKey(e => e.CitedStatuteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.CitedStatuteId);
    }
}
