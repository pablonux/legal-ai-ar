using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class CourtConfiguration : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.ToTable("Courts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(300)
            .IsRequired();
        builder.Property(e => e.ExternalCode)
            .HasMaxLength(100);
        builder.Property(e => e.JurisdictionArea)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.Territory)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.Instance)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.IsVerified)
            .HasDefaultValue(false);

        builder.Property(e => e.CourtCategory)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(e => e.Fuero)
            .HasConversion<string>()
            .HasMaxLength(30);
        builder.Property(e => e.GovernmentLevel)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(e => e.ParentCourtId);

        builder.HasOne(e => e.ParentCourt)
            .WithMany(e => e.ChildCourts)
            .HasForeignKey(e => e.ParentCourtId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Rulings)
            .WithOne(e => e.Court)
            .HasForeignKey(e => e.CourtId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.JudicialOffices)
            .WithOne(e => e.Court)
            .HasForeignKey(e => e.CourtId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
