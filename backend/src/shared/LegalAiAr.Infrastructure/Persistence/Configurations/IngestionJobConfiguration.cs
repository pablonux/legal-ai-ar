using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class IngestionJobConfiguration : IEntityTypeConfiguration<IngestionJob>
{
    public void Configure(EntityTypeBuilder<IngestionJob> builder)
    {
        builder.ToTable("IngestionJobs");

        builder.HasKey(e => e.Id);

        // Sentinel 0: not a defined enum value; lets EF distinguish "unset" from EntityType.Ruling for DB defaults.
        builder.Property(e => e.EntityType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue(Core.Enums.EntityType.Ruling)
            .HasSentinel((Core.Enums.EntityType)0);

        builder.Property(e => e.Type).HasMaxLength(20).IsRequired();
        builder.Property(e => e.DateFrom).HasColumnType("date");
        builder.Property(e => e.DateTo).HasColumnType("date");
        builder.Property(e => e.TriggeredBy).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Status).HasMaxLength(20).IsRequired();
        builder.Property(e => e.TotalSearchResults);
        builder.Property(e => e.DocumentsCrawled).HasDefaultValue(0);
        builder.Property(e => e.DocumentsParsed).HasDefaultValue(0);
        builder.Property(e => e.DocumentsEnriched).HasDefaultValue(0);
        builder.Property(e => e.DocumentsSkipped).HasDefaultValue(0);
        builder.Property(e => e.DiscoveryBatchPublished).HasDefaultValue(true);
        builder.Property(e => e.InfrastructureDegraded).HasDefaultValue(false);
        builder.Property(e => e.DegradedSinceUtc);
        builder.Property(e => e.DegradedReason).HasMaxLength(-1);
        builder.Property(e => e.ErrorSummary).HasMaxLength(-1);
        builder.Property(e => e.CreationLog).HasMaxLength(100);
        builder.Property(e => e.ParentJobId);
        builder.Property(e => e.PartitionIndex);
        builder.Property(e => e.PartitionTotal);

        builder.HasOne(e => e.Source)
            .WithMany(e => e.IngestionJobs)
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ParentJob)
            .WithMany()
            .HasForeignKey(e => e.ParentJobId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(e => new { e.EntityType, e.SourceId })
            .HasDatabaseName("IX_IngestionJobs_EntityType_SourceId");
        builder.HasIndex(e => e.StartedAt);
        builder.HasIndex(e => e.ParentJobId);
    }
}
