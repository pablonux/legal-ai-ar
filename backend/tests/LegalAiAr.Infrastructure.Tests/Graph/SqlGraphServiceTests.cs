using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Infrastructure.Graph;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Tests.Persistence;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LegalAiAr.Infrastructure.Tests.Graph;

public class SqlGraphServiceTests
{
    private readonly ILogger<SqlGraphService> _logger = Substitute.For<ILogger<SqlGraphService>>();

    private static SqlGraphService CreateSut(AppDbContext? context = null)
    {
        var ctx = context ?? TestDbContextFactory.Create(Guid.NewGuid().ToString());
        return new SqlGraphService(ctx, Substitute.For<ILogger<SqlGraphService>>());
    }

    [Fact]
    public async Task UpsertRulingNodeAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        await sut.UpsertRulingNodeAsync(
            Guid.NewGuid(),
            "Test Case",
            new DateOnly(2024, 1, 15),
            "FEDERAL",
            "CSJN",
            "UPHOLDS");
    }

    [Fact]
    public async Task UpsertPersonNodeAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        await sut.UpsertPersonNodeAsync(1, "Juan Pérez");
    }

    [Fact]
    public async Task UpsertCourtNodeAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        await sut.UpsertCourtNodeAsync(1, "CSJN", "FEDERAL", "Nacional", "CSJN");
    }

    [Fact]
    public async Task UpsertKeywordNodeAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        await sut.UpsertKeywordNodeAsync(1, "IMPUESTO A LAS GANANCIAS");
    }

    [Fact]
    public async Task UpsertStatuteNodeAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        await sut.UpsertStatuteNodeAsync("24.767", "Ley de Impuesto a las Ganancias");
    }

    [Fact]
    public async Task CreateSignedByRelationshipAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        await sut.CreateSignedByRelationshipAsync(Guid.NewGuid(), 1, RulingRole.SIGNATORY);
    }

    [Fact]
    public async Task CreateCitesRelationshipAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        var sourceId = Guid.NewGuid();
        var targetId = Guid.NewGuid();
        await sut.CreateCitesRelationshipAsync(sourceId, targetId, CitationType.UPHOLDS);
    }

    [Fact]
    public async Task CreateHasKeywordRelationshipAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        await sut.CreateHasKeywordRelationshipAsync(Guid.NewGuid(), 1);
    }

    [Fact]
    public async Task CreateCitesStatuteRelationshipAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        await sut.CreateCitesStatuteRelationshipAsync(Guid.NewGuid(), 1, "art. 80, art. 64");
    }

    [Fact]
    public async Task CreateIssuedByRelationshipAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        await sut.CreateIssuedByRelationshipAsync(Guid.NewGuid(), 1);
    }

    [Fact]
    public async Task CreateMemberOfRelationshipAsync_CompletesWithoutException()
    {
        var sut = CreateSut();
        await sut.CreateMemberOfRelationshipAsync(1, 1);
    }

    [Fact]
    public async Task GetOutboundCitationsAsync_WhenNoCitations_ReturnsEmpty()
    {
        var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var sut = CreateSut(context);

        var result = await sut.GetOutboundCitationsAsync(Guid.NewGuid());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetOutboundCitationsAsync_WhenCitationsExist_ReturnsMatchingCitations()
    {
        var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var sourceRuling = await CreateAndSaveRulingAsync(context);
        var targetRuling = await CreateAndSaveRulingAsync(context);
        var citation = await CreateAndSaveCitationAsync(context, sourceRuling.Id, targetRuling.Id);
        var sut = CreateSut(context);

        var result = await sut.GetOutboundCitationsAsync(sourceRuling.Id);

        Assert.Single(result);
        Assert.Equal(citation.Id, result[0].Id);
        Assert.Equal(sourceRuling.Id, result[0].SourceRulingId);
        Assert.Equal(targetRuling.Id, result[0].TargetRulingId);
    }

    [Fact]
    public async Task GetInboundCitationsAsync_WhenNoCitations_ReturnsEmpty()
    {
        var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var sut = CreateSut(context);

        var result = await sut.GetInboundCitationsAsync(Guid.NewGuid());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetInboundCitationsAsync_WhenCitationsExist_ReturnsMatchingCitations()
    {
        var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var sourceRuling = await CreateAndSaveRulingAsync(context);
        var targetRuling = await CreateAndSaveRulingAsync(context);
        var citation = await CreateAndSaveCitationAsync(context, sourceRuling.Id, targetRuling.Id);
        var sut = CreateSut(context);

        var result = await sut.GetInboundCitationsAsync(targetRuling.Id);

        Assert.Single(result);
        Assert.Equal(citation.Id, result[0].Id);
        Assert.Equal(sourceRuling.Id, result[0].SourceRulingId);
        Assert.Equal(targetRuling.Id, result[0].TargetRulingId);
    }

    private static async Task<Ruling> CreateAndSaveRulingAsync(AppDbContext context)
    {
        var ruling = new Ruling
        {
            Id = Guid.NewGuid(),
            SourceId = 1,
            ExternalId = Guid.NewGuid().ToString("N")[..12],
            ContentHash = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
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

    private static async Task<Citation> CreateAndSaveCitationAsync(AppDbContext context, Guid sourceRulingId, Guid targetRulingId)
    {
        var citation = new Citation
        {
            SourceRulingId = sourceRulingId,
            TargetRulingId = targetRulingId,
            ExternalAlias = "Fallos: 328:1883",
            CitationType = CitationType.CITES
        };
        context.Citations.Add(citation);
        await context.SaveChangesAsync();
        return citation;
    }
}
