using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class ProceduralRemedyConfiguration : IEntityTypeConfiguration<ProceduralRemedy>
{
    public void Configure(EntityTypeBuilder<ProceduralRemedy> builder)
    {
        builder.ToTable("ProceduralRemedies");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.RemedyType)
            .HasConversion<string>()
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(e => e.Outcome)
            .HasMaxLength(500);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.ResolvingRuling)
            .WithMany()
            .HasForeignKey(e => e.ResolvingRulingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.AppealedRuling)
            .WithMany()
            .HasForeignKey(e => e.AppealedRulingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.CourtAQuo)
            .WithMany()
            .HasForeignKey(e => e.CourtAQuoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.CourtAdQuem)
            .WithMany()
            .HasForeignKey(e => e.CourtAdQuemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.JudicialProceeding)
            .WithMany()
            .HasForeignKey(e => e.JudicialProceedingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.ResolvingRulingId);
        builder.HasIndex(e => e.AppealedRulingId);
        builder.HasIndex(e => e.JudicialProceedingId);
    }
}
