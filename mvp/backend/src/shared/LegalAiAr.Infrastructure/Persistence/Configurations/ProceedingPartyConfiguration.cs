using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class ProceedingPartyConfiguration : IEntityTypeConfiguration<ProceedingParty>
{
    public void Configure(EntityTypeBuilder<ProceedingParty> builder)
    {
        builder.ToTable("ProceedingParties");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => new { e.JudicialProceedingId, e.PersonId, e.Role })
            .IsUnique();

        builder.HasOne(e => e.JudicialProceeding)
            .WithMany(e => e.Parties)
            .HasForeignKey(e => e.JudicialProceedingId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Person)
            .WithMany(e => e.ProceedingParties)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
