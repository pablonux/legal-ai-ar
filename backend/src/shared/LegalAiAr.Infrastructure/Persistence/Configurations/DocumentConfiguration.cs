using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.ExternalId).HasMaxLength(200).IsRequired();
        builder.Property(e => e.AnalysisId).HasMaxLength(200);

        builder.Property(e => e.CurrentStage)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.BlobPath).HasMaxLength(500);
        builder.Property(e => e.ContentHash).HasMaxLength(64);
        builder.Property(e => e.ErrorMessage).HasMaxLength(-1);
        builder.Property(e => e.ErrorType).HasMaxLength(200);
        builder.Property(e => e.RetryCount).HasDefaultValue(0);
        builder.Property(e => e.FetchPdfTimeoutSeconds);
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasOne(e => e.IngestionJob)
            .WithMany()
            .HasForeignKey(e => e.IngestionJobId)
            .OnDelete(DeleteBehavior.Restrict);

        // Deduplication: no two documents for same (SourceId, ExternalId) pair
        builder.HasIndex(e => new { e.SourceId, e.ExternalId })
            .IsUnique()
            .HasDatabaseName("IX_Documents_SourceId_ExternalId");

        // Job-level queries: filter by job, stage, status
        builder.HasIndex(e => new { e.IngestionJobId, e.CurrentStage, e.Status })
            .HasDatabaseName("IX_Documents_Job_Stage_Status");

        // Stage-level processing queries
        builder.HasIndex(e => new { e.CurrentStage, e.Status })
            .HasDatabaseName("IX_Documents_Stage_Status");
    }
}
