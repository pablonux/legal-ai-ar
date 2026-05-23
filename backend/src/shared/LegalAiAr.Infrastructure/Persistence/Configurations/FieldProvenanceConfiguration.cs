using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class FieldProvenanceConfiguration : IEntityTypeConfiguration<FieldProvenance>
{
    public void Configure(EntityTypeBuilder<FieldProvenance> builder)
    {
        builder.ToTable("FieldProvenance");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityType)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.EntityId)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.FieldName)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.Value)
            .HasMaxLength(4000);
        builder.Property(e => e.PreviousValue)
            .HasMaxLength(4000);
        builder.Property(e => e.SourceEndpoint)
            .HasMaxLength(100);
        builder.Property(e => e.SourceField)
            .HasMaxLength(100);
        builder.Property(e => e.InferenceMethod)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.AiModel)
            .HasMaxLength(100);
        builder.Property(e => e.AiPrompt)
            .HasMaxLength(200);
        builder.Property(e => e.ChangeType)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();
        builder.Property(e => e.RecordedAt)
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(e => e.IsCurrent)
            .HasDefaultValue(true);

        builder.HasIndex(e => new { e.EntityType, e.EntityId, e.FieldName, e.IsCurrent })
            .HasFilter("[IsCurrent] = 1");
        builder.HasIndex(e => e.IngestionJobId);

        builder.HasOne(e => e.Source)
            .WithMany()
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.IngestionJob)
            .WithMany()
            .HasForeignKey(e => e.IngestionJobId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
