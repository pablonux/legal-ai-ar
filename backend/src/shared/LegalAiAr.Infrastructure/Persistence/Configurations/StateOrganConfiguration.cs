using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class StateOrganConfiguration : IEntityTypeConfiguration<StateOrgan>
{
    public void Configure(EntityTypeBuilder<StateOrgan> builder)
    {
        builder.ToTable("StateOrgans");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.Property(e => e.Abbreviation).HasMaxLength(30);
        builder.Property(e => e.Branch).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(e => e.GovernmentLevel).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.JurisdictionArea).HasMaxLength(100);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.ParentOrgan)
            .WithMany(e => e.ChildOrgans)
            .HasForeignKey(e => e.ParentOrganId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.IssuedStatutes)
            .WithOne(s => s.IssuingOrgan)
            .HasForeignKey(s => s.IssuingOrganId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.Abbreviation).HasFilter("[Abbreviation] IS NOT NULL");
    }
}
