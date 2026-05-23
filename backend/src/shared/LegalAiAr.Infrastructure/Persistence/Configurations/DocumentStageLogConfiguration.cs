using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class DocumentStageLogConfiguration : IEntityTypeConfiguration<DocumentStageLog>
{
    public void Configure(EntityTypeBuilder<DocumentStageLog> builder)
    {
        builder.ToTable("DocumentStageLogs");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityColumn();

        builder.Property(e => e.DocumentId).IsRequired();

        builder.Property(e => e.Stage)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.StartedAt).IsRequired();
        builder.Property(e => e.CompletedAt).IsRequired();
        builder.Property(e => e.DurationMs).IsRequired();
        builder.Property(e => e.WorkerInstanceId).HasMaxLength(100);
        builder.Property(e => e.ErrorMessage).HasMaxLength(500);

        builder.HasOne(e => e.Document)
            .WithMany()
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.DocumentId)
            .HasDatabaseName("IX_DocumentStageLogs_DocumentId");

        builder.HasIndex(e => new { e.DocumentId, e.Stage })
            .HasDatabaseName("IX_DocumentStageLogs_DocumentId_Stage");

        builder.HasIndex(e => e.StartedAt)
            .HasDatabaseName("IX_DocumentStageLogs_StartedAt");
    }
}
