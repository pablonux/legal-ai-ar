using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class NormRelationConfiguration : IEntityTypeConfiguration<NormRelation>
{
    public void Configure(EntityTypeBuilder<NormRelation> builder)
    {
        builder.ToTable("NormRelations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.RelationType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => new { e.SourceStatuteId, e.TargetStatuteId, e.RelationType })
            .IsUnique();
        builder.HasIndex(e => e.TargetStatuteId);

        builder.HasOne(e => e.SourceStatute)
            .WithMany(e => e.OutboundNormRelations)
            .HasForeignKey(e => e.SourceStatuteId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.TargetStatute)
            .WithMany(e => e.InboundNormRelations)
            .HasForeignKey(e => e.TargetStatuteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
