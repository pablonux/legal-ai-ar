using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class LegalRepresentationConfiguration : IEntityTypeConfiguration<LegalRepresentation>
{
    public void Configure(EntityTypeBuilder<LegalRepresentation> builder)
    {
        builder.ToTable("LegalRepresentations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => new { e.LawyerPersonId, e.PartyPersonId, e.JudicialProceedingId })
            .IsUnique();

        builder.HasOne(e => e.LawyerPerson)
            .WithMany()
            .HasForeignKey(e => e.LawyerPersonId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.PartyPerson)
            .WithMany()
            .HasForeignKey(e => e.PartyPersonId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.JudicialProceeding)
            .WithMany(e => e.LegalRepresentations)
            .HasForeignKey(e => e.JudicialProceedingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
