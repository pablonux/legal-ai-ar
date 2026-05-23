using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class RulingParticipationConfiguration : IEntityTypeConfiguration<RulingParticipation>
{
    public void Configure(EntityTypeBuilder<RulingParticipation> builder)
    {
        builder.ToTable("RulingParticipations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Role)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => new { e.RulingId, e.PersonId, e.Role })
            .IsUnique();

        builder.HasOne(e => e.Ruling)
            .WithMany(e => e.RulingParticipations)
            .HasForeignKey(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Person)
            .WithMany(e => e.RulingParticipations)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Vote)
            .WithMany(e => e.Participations)
            .HasForeignKey(e => e.VoteId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
