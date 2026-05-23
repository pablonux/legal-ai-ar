using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class WorkerPauseStateConfiguration : IEntityTypeConfiguration<WorkerPauseState>
{
    public void Configure(EntityTypeBuilder<WorkerPauseState> builder)
    {
        builder.ToTable("WorkerPauseStates");

        builder.HasKey(e => e.WorkerType);

        builder.Property(e => e.WorkerType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.IsPaused)
            .HasDefaultValue(false);

        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
