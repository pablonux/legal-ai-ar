using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Caching;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace LegalAiAr.Infrastructure.Tests.Persistence.Repositories;

public class KeywordRepositoryTests
{
    private static readonly EntityCacheService Cache = new(NullLogger<EntityCacheService>.Instance);

    [Fact]
    public async Task GetByIdAsync_WhenKeywordExists_ReturnsKeyword()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var keyword = await CreateAndSaveKeywordAsync(context);
        var repository = new KeywordRepository(context, Cache);

        var result = await repository.GetByIdAsync(keyword.Id);

        Assert.NotNull(result);
        Assert.Equal(keyword.Id, result.Id);
        Assert.Equal(keyword.Description, result.Description);
        Assert.Equal(keyword.ExternalCode, result.ExternalCode);
    }

    [Fact]
    public async Task GetByIdAsync_WhenKeywordDoesNotExist_ReturnsNull()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new KeywordRepository(context, Cache);

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetOrCreateBatchAsync_Empty_ReturnsEmpty()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new KeywordRepository(context, Cache);

        var map = await repository.GetOrCreateBatchAsync(Array.Empty<KeywordLookupKey>());

        Assert.Empty(map);
    }

    [Fact]
    public async Task GetOrCreateBatchAsync_TwoKeysOneRow_ReturnsSameKeyword()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var keyword = new Keyword { ExternalCode = 2093, Description = "IMPUESTO A LAS GANANCIAS" };
        context.Keywords.Add(keyword);
        await context.SaveChangesAsync();

        await using var context2 = TestDbContextFactory.Create(dbName);
        var repository = new KeywordRepository(context2, Cache);

        var map = await repository.GetOrCreateBatchAsync(
        [
            new KeywordLookupKey(2093, "IMPUESTO A LAS GANANCIAS"),
            new KeywordLookupKey(null, "IMPUESTO A LAS GANANCIAS"),
        ]);

        Assert.Equal(2, map.Count);
        Assert.Same(
            map[new KeywordLookupKey(2093, "IMPUESTO A LAS GANANCIAS")],
            map[new KeywordLookupKey(null, "IMPUESTO A LAS GANANCIAS")]);
    }

    [Fact]
    public async Task GetOrCreateAsync_DelegatesToBatch_SingleKey()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var repository = new KeywordRepository(context, Cache);

        var kw = await repository.GetOrCreateAsync(7777, "KEYWORD ÚNICO");
        await context.SaveChangesAsync();

        Assert.NotEqual(0, kw.Id);
        Assert.Equal(7777, kw.ExternalCode);
        Assert.Equal("KEYWORD ÚNICO", kw.Description);
    }

    private static async Task<Keyword> CreateAndSaveKeywordAsync(AppDbContext context)
    {
        var keyword = new Keyword
        {
            ExternalCode = 2093,
            Description = "IMPUESTO A LAS GANANCIAS"
        };
        context.Keywords.Add(keyword);
        await context.SaveChangesAsync();
        return keyword;
    }
}
