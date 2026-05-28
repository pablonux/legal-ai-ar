using LegalAiAr.Core.Entities;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LegalAiAr.Worker.Indexer.Tests.Steps;

public class EmbeddingConfigRepositoryTests
{
    private static AppDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        var ctx = new AppDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    [Fact]
    public async Task EnsureSeededAsync_CreatesV1ConfigWhenNoneExist()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var sut = new EmbeddingConfigRepository(context);

        var config = await sut.EnsureSeededAsync();

        Assert.NotNull(config);
        Assert.Equal("v1-rule-based", config.Version);
        Assert.Equal("text-embedding-3-large", config.EmbeddingModel);
        Assert.Equal(3072, config.EmbeddingDimensions);
        Assert.True(config.IsActive);
    }

    [Fact]
    public async Task EnsureSeededAsync_ReturnsExistingActiveConfig()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        context.EmbeddingConfigs.Add(new EmbeddingConfig
        {
            Version = "v2-llm", EmbeddingModel = "model", EmbeddingDimensions = 3072,
            ContextualizationMethod = "llm-contextual", ChunkingStrategy = "s",
            ChunkSize = 512, ChunkOverlap = 50, IsActive = true
        });
        await context.SaveChangesAsync();

        var sut = new EmbeddingConfigRepository(context);
        var config = await sut.EnsureSeededAsync();

        Assert.Equal("v2-llm", config.Version);
    }

    [Fact]
    public async Task UpsertEmbeddingStateAsync_CreatesNewState()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var sourceId = context.Sources.First().Id;
        var ruling = new Ruling { ExternalId = "1", ContentHash = "h", CaseTitle = "T", FullText = "x", BlobPath = "b", SourceId = sourceId };
        context.Rulings.Add(ruling);
        var config = new EmbeddingConfig
        {
            Version = "v1", EmbeddingModel = "m", EmbeddingDimensions = 3072,
            ContextualizationMethod = "rule-based-prefix", ChunkingStrategy = "s",
            ChunkSize = 512, ChunkOverlap = 50, IsActive = true
        };
        context.EmbeddingConfigs.Add(config);
        await context.SaveChangesAsync();

        var sut = new EmbeddingConfigRepository(context);
        await sut.UpsertEmbeddingStateAsync(ruling.Id, config.Id, 15);

        var state = await context.RulingEmbeddingStates.FirstOrDefaultAsync(s => s.RulingId == ruling.Id);
        Assert.NotNull(state);
        Assert.Equal(config.Id, state.EmbeddingConfigId);
        Assert.Equal(15, state.ChunkCount);
        Assert.False(state.NeedsReembedding);
    }

    [Fact]
    public async Task UpsertEmbeddingStateAsync_UpdatesExistingState()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var sourceId = context.Sources.First().Id;
        var ruling = new Ruling { ExternalId = "1", ContentHash = "h", CaseTitle = "T", FullText = "x", BlobPath = "b", SourceId = sourceId };
        context.Rulings.Add(ruling);
        var config1 = new EmbeddingConfig
        {
            Version = "v1", EmbeddingModel = "m", EmbeddingDimensions = 3072,
            ContextualizationMethod = "rule-based-prefix", ChunkingStrategy = "s",
            ChunkSize = 512, ChunkOverlap = 50, IsActive = false
        };
        var config2 = new EmbeddingConfig
        {
            Version = "v2", EmbeddingModel = "m", EmbeddingDimensions = 3072,
            ContextualizationMethod = "llm-contextual", ChunkingStrategy = "s",
            ChunkSize = 512, ChunkOverlap = 50, IsActive = true
        };
        context.EmbeddingConfigs.AddRange(config1, config2);
        await context.SaveChangesAsync();

        context.RulingEmbeddingStates.Add(new RulingEmbeddingState
        {
            RulingId = ruling.Id, EmbeddingConfigId = config1.Id, ChunkCount = 10,
            EmbeddedAt = DateTime.UtcNow.AddDays(-1), NeedsReembedding = true
        });
        await context.SaveChangesAsync();

        var sut = new EmbeddingConfigRepository(context);
        await sut.UpsertEmbeddingStateAsync(ruling.Id, config2.Id, 20);

        var state = await context.RulingEmbeddingStates.FirstOrDefaultAsync(s => s.RulingId == ruling.Id);
        Assert.NotNull(state);
        Assert.Equal(config2.Id, state.EmbeddingConfigId);
        Assert.Equal(20, state.ChunkCount);
        Assert.False(state.NeedsReembedding);
    }
}
