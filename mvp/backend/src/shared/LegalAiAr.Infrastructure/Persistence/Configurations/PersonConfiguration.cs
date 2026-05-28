using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.DisplayName)
            .HasMaxLength(400)
            .IsRequired();
        builder.Property(e => e.FirstName)
            .HasMaxLength(200);
        builder.Property(e => e.LastName)
            .HasMaxLength(200);
        builder.Property(e => e.PersonType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PersonType.Physical);
        builder.Property(e => e.LegalEntityType)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(e => e.CsjnMinistroId);
        builder.Property(e => e.IsVerified)
            .HasDefaultValue(false);

        builder.HasIndex(e => e.CsjnMinistroId)
            .IsUnique()
            .HasFilter("[CsjnMinistroId] IS NOT NULL");
        builder.HasIndex(e => e.DisplayName);

        builder.HasMany(e => e.RulingParticipations)
            .WithOne(e => e.Person)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.JudicialOffices)
            .WithOne(e => e.Person)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.ProceedingParties)
            .WithOne(e => e.Person)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
