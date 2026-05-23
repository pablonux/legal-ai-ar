using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class EntityAuditLogConfiguration : IEntityTypeConfiguration<EntityAuditLog>
{
    public void Configure(EntityTypeBuilder<EntityAuditLog> builder)
    {
        builder.ToTable("EntityAuditLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityType)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.EntityId)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.Operation)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.PerformedBy)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(e => e.ChangeSummary)
            .HasMaxLength(2000);
        builder.Property(e => e.Timestamp)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => new { e.EntityType, e.EntityId });
        builder.HasIndex(e => e.IngestionJobId);
        builder.HasIndex(e => e.Timestamp);

        builder.HasOne(e => e.IngestionJob)
            .WithMany()
            .HasForeignKey(e => e.IngestionJobId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
