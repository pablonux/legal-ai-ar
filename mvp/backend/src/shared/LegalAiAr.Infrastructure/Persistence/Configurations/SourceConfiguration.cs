using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class SourceConfiguration : IEntityTypeConfiguration<Source>
{
    public void Configure(EntityTypeBuilder<Source> builder)
    {
        builder.ToTable("Sources");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.FullName)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(e => e.BaseUrl)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(e => e.Strategy)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.IsActive)
            .IsRequired();

        builder.HasMany(e => e.Rulings)
            .WithOne(e => e.Source)
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.CrawlerConfigs)
            .WithOne(e => e.Source)
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Source { Id = 1, Name = "CSJN", FullName = "Corte Suprema de Justicia de la Nación", BaseUrl = "https://sjconsulta.csjn.gov.ar/sjconsulta/", Strategy = "api-first", IsActive = true },
            new Source { Id = 2, Name = "SAIJ", FullName = "Sistema Argentino de Información Jurídica", BaseUrl = "https://www.saij.gob.ar/", Strategy = "html-pdf", IsActive = true },
            new Source { Id = 3, Name = "PJN", FullName = "Poder Judicial de la Nación", BaseUrl = "https://www.pjn.gov.ar/", Strategy = "html-pdf", IsActive = true },
            new Source { Id = 4, Name = "SCBA", FullName = "Suprema Corte de Justicia de la Provincia de Buenos Aires", BaseUrl = "https://www.scba.gov.ar/", Strategy = "html-pdf", IsActive = true },
            new Source { Id = 6, Name = "SAIJ", FullName = "SAIJ — Tesauro (API vocabularios)", BaseUrl = "http://vocabularios.saij.gob.ar/", Strategy = "thesaurus", IsActive = true });
    }
}
