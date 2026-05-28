using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Persistence.Repositories;

namespace LegalAiAr.Infrastructure.Tests.Persistence.Repositories;

public class RulingRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WhenRulingExists_ReturnsRuling()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var ruling = await CreateAndSaveRulingAsync(context);
        var repository = new RulingRepository(context);

        var result = await repository.GetByIdAsync(ruling.Id);

        Assert.NotNull(result);
        Assert.Equal(ruling.Id, result.Id);
        Assert.Equal(ruling.ContentHash, result.ContentHash);
        Assert.Equal(ruling.CaseTitle, result.CaseTitle);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRulingDoesNotExist_ReturnsNull()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new RulingRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByContentHashAsync_WhenRulingExists_ReturnsRuling()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var ruling = await CreateAndSaveRulingAsync(context);
        var repository = new RulingRepository(context);

        var result = await repository.GetByContentHashAsync(ruling.ContentHash);

        Assert.NotNull(result);
        Assert.Equal(ruling.Id, result.Id);
        Assert.Equal(ruling.ContentHash, result.ContentHash);
    }

    [Fact]
    public async Task GetByContentHashAsync_WhenRulingDoesNotExist_ReturnsNull()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new RulingRepository(context);

        var result = await repository.GetByContentHashAsync("nonexistent-hash");

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsByContentHashAsync_WhenRulingExists_ReturnsTrue()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var ruling = await CreateAndSaveRulingAsync(context);
        var repository = new RulingRepository(context);

        var result = await repository.ExistsByContentHashAsync(ruling.ContentHash);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByContentHashAsync_WhenRulingDoesNotExist_ReturnsFalse()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new RulingRepository(context);

        var result = await repository.ExistsByContentHashAsync("nonexistent-hash");

        Assert.False(result);
    }

    [Fact]
    public async Task FindRulingIdsByAnalysisIdsAsync_ReturnsOnlyMatchingRows()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var r1 = CreateRuling();
        r1.AnalysisId = "ANA-001";
        var r2 = CreateRuling();
        r2.AnalysisId = "ANA-002";
        r2.ExternalId = "8048523";
        r2.ContentHash = "def456abc123789012345678901234567890123456789012345678901234ab";
        context.Rulings.AddRange(r1, r2);
        await context.SaveChangesAsync();

        var repository = new RulingRepository(context);

        var map = await repository.FindRulingIdsByAnalysisIdsAsync(
            ["ANA-001", "missing", "ANA-002", "ANA-001"]);

        Assert.Equal(2, map.Count);
        Assert.Equal(r1.Id, map["ANA-001"]);
        Assert.Equal(r2.Id, map["ANA-002"]);
        Assert.False(map.ContainsKey("missing"));
    }

    [Fact]
    public async Task FindRulingIdsByAnalysisIdsAsync_WhenEmptyInput_ReturnsEmptyDictionary()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new RulingRepository(context);

        var map = await repository.FindRulingIdsByAnalysisIdsAsync(Array.Empty<string>());

        Assert.Empty(map);
    }

    [Fact]
    public async Task AddAsync_WhenRulingAdded_CanBeRetrieved()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var repository = new RulingRepository(context);
        var ruling = CreateRuling();

        await repository.AddAsync(ruling);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(ruling.Id);
        Assert.NotNull(result);
        Assert.Equal(ruling.ContentHash, result.ContentHash);
    }

    private static Ruling CreateRuling()
    {
        return new Ruling
        {
            Id = Guid.NewGuid(),
            SourceId = 1,
            ExternalId = "8048522",
            ContentHash = "abc123def456",
            CaseTitle = "Test Case Title",
            RulingDate = new DateOnly(2024, 1, 15),
            CourtId = 1,
            IndexedAt = DateTime.UtcNow,
            Status = RulingStatus.Indexed
        };
    }

    private static async Task<Ruling> CreateAndSaveRulingAsync(AppDbContext context)
    {
        var ruling = CreateRuling();
        context.Rulings.Add(ruling);
        await context.SaveChangesAsync();
        return ruling;
    }
}
