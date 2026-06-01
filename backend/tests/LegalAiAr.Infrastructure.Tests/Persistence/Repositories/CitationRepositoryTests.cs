using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Persistence.Repositories;

namespace LegalAiAr.Infrastructure.Tests.Persistence.Repositories;

public class CitationRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WhenCitationExists_ReturnsCitation()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var citation = await CreateAndSaveCitationAsync(context);
        var repository = new CitationRepository(context);

        var result = await repository.GetByIdAsync(citation.Id);

        Assert.NotNull(result);
        Assert.Equal(citation.Id, result.Id);
        Assert.Equal(citation.ExternalAlias, result.ExternalAlias);
        Assert.Equal(citation.CitationType, result.CitationType);
        Assert.NotNull(result.SourceRuling);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCitationDoesNotExist_ReturnsNull()
    {
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var repository = new CitationRepository(context);

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WhenCitationAdded_CanBeRetrieved()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(dbName);
        var ruling = await CreateAndSaveRulingAsync(context);
        var repository = new CitationRepository(context);
        var citation = new Citation
        {
            SourceRulingId = ruling.Id,
            ExternalAlias = "Fallos: 328:1883",
            CitationType = CitationType.UPHOLDS
        };

        await repository.AddAsync(citation);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(citation.Id);
        Assert.NotNull(result);
        Assert.Equal("Fallos: 328:1883", result.ExternalAlias);
        Assert.Equal(CitationType.UPHOLDS, result.CitationType);
    }

    private static async Task<Ruling> CreateAndSaveRulingAsync(AppDbContext context)
    {
        var ruling = new Ruling
        {
            Id = Guid.NewGuid(),
            SourceId = 1,
            ExternalId = "8048522",
            ContentHash = "abc123def456",
            CaseTitle = "Test Case",
            RulingDate = new DateOnly(2024, 1, 15),
            CourtId = 1,
            IndexedAt = DateTime.UtcNow,
            Status = RulingStatus.Indexed
        };
        context.Rulings.Add(ruling);
        await context.SaveChangesAsync();
        return ruling;
    }

    private static async Task<Citation> CreateAndSaveCitationAsync(AppDbContext context)
    {
        var ruling = await CreateAndSaveRulingAsync(context);
        var citation = new Citation
        {
            SourceRulingId = ruling.Id,
            ExternalAlias = "Fallos: 328:1883",
            CitationType = CitationType.CITES
        };
        context.Citations.Add(citation);
        await context.SaveChangesAsync();
        return citation;
    }
}
