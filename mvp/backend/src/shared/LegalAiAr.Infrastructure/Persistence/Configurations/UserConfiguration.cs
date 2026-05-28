using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Email)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(e => e.DisplayName)
            .HasMaxLength(200);
        builder.Property(e => e.Role)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(e => e.Email)
            .IsUnique();
    }
}
