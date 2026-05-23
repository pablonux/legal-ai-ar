using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class JudicialProceedingConfiguration : IEntityTypeConfiguration<JudicialProceeding>
{
    public void Configure(EntityTypeBuilder<JudicialProceeding> builder)
    {
        builder.ToTable("JudicialProceedings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CaseNumber)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.DisplayName)
            .HasMaxLength(500);
        builder.Property(e => e.JurisdictionArea)
            .HasMaxLength(100);
        builder.Property(e => e.ProcessType)
            .HasConversion<string>()
            .HasMaxLength(30);
        builder.Property(e => e.ProcessSubtype)
            .HasMaxLength(50);
        builder.Property(e => e.LegalBranch)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(e => new { e.CaseNumber, e.JurisdictionArea });

        builder.HasOne(e => e.Court)
            .WithMany()
            .HasForeignKey(e => e.CourtId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(e => e.Rulings)
            .WithOne(e => e.JudicialProceeding)
            .HasForeignKey(e => e.JudicialProceedingId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
