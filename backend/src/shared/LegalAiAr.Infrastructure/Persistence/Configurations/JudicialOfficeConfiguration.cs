using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class JudicialOfficeConfiguration : IEntityTypeConfiguration<JudicialOffice>
{
    public void Configure(EntityTypeBuilder<JudicialOffice> builder)
    {
        builder.ToTable("JudicialOffices");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Position)
            .HasConversion<string>()
            .HasMaxLength(30);
        builder.Property(e => e.DesignationAuthority)
            .HasMaxLength(300);

        builder.HasIndex(e => new { e.PersonId, e.CourtId, e.StartDate })
            .IsUnique()
            .HasFilter("[StartDate] IS NOT NULL");

        builder.HasOne(e => e.Person)
            .WithMany(e => e.JudicialOffices)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Court)
            .WithMany(e => e.JudicialOffices)
            .HasForeignKey(e => e.CourtId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
