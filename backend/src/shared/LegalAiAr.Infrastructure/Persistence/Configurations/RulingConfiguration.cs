using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class RulingConfiguration : IEntityTypeConfiguration<Ruling>
{
    public void Configure(EntityTypeBuilder<Ruling> builder)
    {
        builder.ToTable("Rulings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExternalId)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.AnalysisId)
            .HasMaxLength(50);
        builder.Property(e => e.ContentHash)
            .HasMaxLength(64)
            .IsRequired();
        builder.Property(e => e.CaseTitle)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(e => e.CaseNumber)
            .HasMaxLength(100);
        builder.Property(e => e.JurisdictionArea)
            .HasMaxLength(100);
        builder.Property(e => e.Instance)
            .HasMaxLength(50);
        builder.Property(e => e.Jurisdiction)
            .HasMaxLength(100);
        builder.Property(e => e.ResourceType)
            .HasMaxLength(100);
        builder.Property(e => e.RulingDirection)
            .HasMaxLength(50);
        builder.Property(e => e.SubjectArea)
            .HasMaxLength(100);
        builder.Property(e => e.LegalBranch)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(e => e.PrecedentWeight)
            .HasConversion<string>()
            .HasMaxLength(30);
        builder.Property(e => e.BlobPath)
            .HasMaxLength(500);
        builder.Property(e => e.ActionType)
            .HasMaxLength(100);
        builder.Property(e => e.InternalSubject)
            .HasMaxLength(500);
        builder.Property(e => e.OfficialReference)
            .HasMaxLength(200);
        builder.Property(e => e.FederalQuestion)
            .HasMaxLength(500);
        builder.Property(e => e.ProceduralFormula)
            .HasMaxLength(500);
        builder.Property(e => e.RatioDecidendi);
        builder.Property(e => e.DoctrinaLegal);
        builder.Property(e => e.HasDictamen)
            .HasDefaultValue(false);
        builder.Property(e => e.Status)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => (RulingStatus)Enum.Parse(typeof(RulingStatus), v, true))
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(e => e.ContentHash)
            .IsUnique();
        builder.HasIndex(e => new { e.SourceId, e.ExternalId });

        builder.HasOne(e => e.Source)
            .WithMany(e => e.Rulings)
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Court)
            .WithMany(e => e.Rulings)
            .HasForeignKey(e => e.CourtId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.OutboundCitations)
            .WithOne(e => e.SourceRuling)
            .HasForeignKey(e => e.SourceRulingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.InboundCitations)
            .WithOne(e => e.TargetRuling)
            .HasForeignKey(e => e.TargetRulingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.IngestionJob)
            .WithMany()
            .HasForeignKey(e => e.IngestionJobId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
