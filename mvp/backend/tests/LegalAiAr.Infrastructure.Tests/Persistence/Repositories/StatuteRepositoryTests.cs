using LegalAiAr.Core.Entities;
using LegalAiAr.Infrastructure.Caching;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace LegalAiAr.Infrastructure.Tests.Persistence.Repositories;

public class StatuteRepositoryTests
{
    private static readonly EntityCacheService Cache = new(NullLogger<EntityCacheService>.Instance);

    [Fact]
    public async Task GetByIdAsync_WhenStatuteExists_ReturnsStatute()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var statute = await CreateAndSaveStatuteAsync(context);
        var repository = new StatuteRepository(context, Cache);

        var result = await repository.GetByIdAsync(statute.Id);

        Assert.NotNull(result);
        Assert.Equal(statute.Id, result.Id);
        Assert.Equal(statute.Number, result.Number);
        Assert.Equal(statute.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStatuteDoesNotExist_ReturnsNull()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new StatuteRepository(context, Cache);

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetOrCreateBatchAsync_ReturnsExistingAndCreatesNew()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        context.Statutes.Add(new Statute { Number = "A-1", Name = "Old name" });
        await context.SaveChangesAsync();

        var repository = new StatuteRepository(context, Cache);

        var map = await repository.GetOrCreateBatchAsync(
            new[] { ("A-1", "Updated name"), ("B-2", "Brand new") },
            CancellationToken.None);

        Assert.Equal(2, map.Count);
        Assert.Equal("Updated name", map["A-1"].Name);
        Assert.Equal("Brand new", map["B-2"].Name);

        await context.SaveChangesAsync();
        Assert.NotEqual(0, map["B-2"].Id);
    }

    private static async Task<Statute> CreateAndSaveStatuteAsync(AppDbContext context)
    {
        var statute = new Statute
        {
            Number = "24.767",
            Name = "Ley de Impuesto a las Ganancias",
            Url = "https://argentina.gob.ar/normativa/ley-24767"
        };
        context.Statutes.Add(statute);
        await context.SaveChangesAsync();
        return statute;
    }
}
