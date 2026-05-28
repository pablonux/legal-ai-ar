using LegalAiAr.Core.Entities;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LegalAiAr.Infrastructure.Tests.Persistence;

/// <summary>
/// Creates an in-memory AppDbContext for unit tests.
/// Sources and CrawlerConfigs are seeded via IEntityTypeConfiguration.HasData.
/// </summary>
internal static class TestDbContextFactory
{
    public static AppDbContext Create(string databaseName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        SeedCourtIfNotExists(context);
        return context;
    }

    private static void SeedCourtIfNotExists(AppDbContext context)
    {
        if (context.Courts.Any())
            return;

        context.Courts.Add(new Court
        {
            Id = 1,
            Name = "Corte Suprema de Justicia de la Nación",
            JurisdictionArea = "FEDERAL",
            Territory = "Nacional",
            Instance = "CSJN"
        });
        context.SaveChanges();
    }
}
