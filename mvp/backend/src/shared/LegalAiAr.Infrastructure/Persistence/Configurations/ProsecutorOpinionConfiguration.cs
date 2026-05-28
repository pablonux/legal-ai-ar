using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class ProsecutorOpinionConfiguration : IEntityTypeConfiguration<ProsecutorOpinion>
{
    public void Configure(EntityTypeBuilder<ProsecutorOpinion> builder)
    {
        builder.ToTable("ProsecutorOpinions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ProsecutorName)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(e => e.Summary)
            .HasMaxLength(4000);
        builder.Property(e => e.RecommendedDirection)
            .HasMaxLength(500);
        builder.Property(e => e.DocumentBlobPath)
            .HasMaxLength(500);

        builder.HasIndex(e => e.RulingId).IsUnique();

        builder.HasOne(e => e.Ruling)
            .WithOne(e => e.ProsecutorOpinion)
            .HasForeignKey<ProsecutorOpinion>(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Person)
            .WithMany()
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
