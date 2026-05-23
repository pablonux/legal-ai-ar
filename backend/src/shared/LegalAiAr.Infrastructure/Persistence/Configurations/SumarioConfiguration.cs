using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class SumarioConfiguration : IEntityTypeConfiguration<Sumario>
{
    public void Configure(EntityTypeBuilder<Sumario> builder)
    {
        builder.ToTable("Sumarios");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Text)
            .IsRequired();
        builder.Property(e => e.Volume)
            .HasMaxLength(50);
        builder.Property(e => e.Page)
            .HasMaxLength(50);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => new { e.RulingId, e.ExternalId });

        builder.HasOne(e => e.Ruling)
            .WithMany(e => e.Sumarios)
            .HasForeignKey(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
