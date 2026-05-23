using LegalAiAr.Infrastructure.Persistence.Repositories;

namespace LegalAiAr.Infrastructure.Tests.Persistence.Repositories;

public class CrawlerConfigRepositoryTests
{
    [Fact]
    public async Task GetBySourceIdAsync_WhenConfigExists_ReturnsConfig()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new CrawlerConfigRepository(context);

        var result = await repository.GetBySourceIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.SourceId);
        Assert.True(result.IsEnabled);
        Assert.NotNull(result.Source);
        Assert.Equal("CSJN", result.Source.Name);
    }

    [Fact]
    public async Task GetBySourceIdAsync_WhenConfigDoesNotExist_ReturnsNull()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new CrawlerConfigRepository(context);

        var result = await repository.GetBySourceIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllConfigsOrderedBySourceId()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new CrawlerConfigRepository(context);

        var result = await repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        Assert.Equal(1, result[0].SourceId);
        Assert.Equal(2, result[1].SourceId);
        Assert.Equal(3, result[2].SourceId);
        Assert.Equal(4, result[3].SourceId);
        Assert.Equal(6, result[4].SourceId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenConfigExists_ReturnsConfig()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new CrawlerConfigRepository(context);

        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.SourceId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenConfigDoesNotExist_ReturnsNull()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new CrawlerConfigRepository(context);

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }
}
