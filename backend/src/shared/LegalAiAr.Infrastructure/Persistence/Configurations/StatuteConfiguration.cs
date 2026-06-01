using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class StatuteConfiguration : IEntityTypeConfiguration<Statute>
{
    public void Configure(EntityTypeBuilder<Statute> builder)
    {
        builder.ToTable("Statutes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Number)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(e => e.Name)
            .HasMaxLength(300)
            .IsRequired();
        builder.Property(e => e.Url)
            .HasMaxLength(500);

        builder.Property(e => e.NormType)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(e => e.NormativeLevel)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(e => e.LegalBranch)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(e => e.IssuingBody)
            .HasMaxLength(200);
        builder.Property(e => e.OfficialUrl)
            .HasMaxLength(500);
        builder.Property(e => e.IssuingBodyName)
            .HasMaxLength(300);
        builder.Property(e => e.SaijId)
            .HasMaxLength(100);
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(e => e.SaijId)
            .IsUnique()
            .HasFilter("[SaijId] IS NOT NULL");

        builder.Ignore(e => e.IsVigente);

        builder.HasMany(e => e.RulingStatutes)
            .WithOne(e => e.Statute)
            .HasForeignKey(e => e.StatuteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
