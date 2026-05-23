using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class ThesaurusRelationConfiguration : IEntityTypeConfiguration<ThesaurusRelation>
{
    public void Configure(EntityTypeBuilder<ThesaurusRelation> builder)
    {
        builder.ToTable("ThesaurusRelations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.RelationType)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => new { e.SourceTermId, e.TargetTermId, e.RelationType })
            .IsUnique();

        builder.HasIndex(e => e.RelationType);

        builder.HasOne(e => e.SourceTerm)
            .WithMany(e => e.RelationsAsSource)
            .HasForeignKey(e => e.SourceTermId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TargetTerm)
            .WithMany(e => e.RelationsAsTarget)
            .HasForeignKey(e => e.TargetTermId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
