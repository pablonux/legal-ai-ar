using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventType)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.Payload)
            .IsRequired();

        builder.Property(e => e.OccurredOnUtc)
            .IsRequired();

        builder.Property(e => e.Error)
            .HasMaxLength(4000);

        builder.HasIndex(e => new { e.ProcessedOnUtc, e.OccurredOnUtc });
    }
}
