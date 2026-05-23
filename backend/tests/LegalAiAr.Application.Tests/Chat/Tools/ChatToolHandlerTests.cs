using LegalAiAr.Application.Chat.Tools;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LegalAiAr.Application.Tests.Chat.Tools;

public class ChatToolHandlerTests
{
    private static ToolExecutionContext CreateContext(Action<ServiceCollection> configure)
    {
        var services = new ServiceCollection();
        configure(services);
        return new ToolExecutionContext { Services = services.BuildServiceProvider() };
    }

    [Fact]
    public async Task SearchRulings_HappyPath_ReturnsCaseTitleAndId()
    {
        var rulingId = Guid.NewGuid();
        var item = new SearchResultItem(
            rulingId, "Test Case", "Sum", null, null, new DateOnly(2024, 1, 15),
            "Civil", "CSJN", "Court A", Array.Empty<string>(), null, 0.9);
        var embed = Substitute.For<IEmbeddingService>();
        embed.GenerateAsync("negligencia", Arg.Any<CancellationToken>()).Returns(new[] { 1f, 2f });
        var search = Substitute.For<ISearchService>();
        search.SearchAsync(Arg.Any<float[]>(), Arg.Any<string>(), Arg.Any<SearchFilters?>(), 1, 5, Arg.Any<CancellationToken>())
            .Returns(new PagedSearchResult(new[] { item }, 1));

        var tool = new SearchRulingsTool();
        var ctx = CreateContext(s =>
        {
            s.AddSingleton(embed);
            s.AddSingleton(search);
        });

        var result = await tool.ExecuteAsync("""{"query":"negligencia"}""", ctx, CancellationToken.None);

        Assert.Contains("Test Case", result);
        Assert.Contains(rulingId.ToString(), result);
    }

    [Fact]
    public async Task SearchRulings_EmptyQuery_ReturnsError()
    {
        var tool = new SearchRulingsTool();
        var ctx = CreateContext(s =>
        {
            s.AddSingleton(Substitute.For<IEmbeddingService>());
            s.AddSingleton(Substitute.For<ISearchService>());
        });

        var result = await tool.ExecuteAsync("""{"query":""}""", ctx, CancellationToken.None);

        Assert.Equal("Error: query is required.", result);
    }

    [Fact]
    public async Task GetRulingDetail_HappyPath_ReturnsMetadata()
    {
        var id = Guid.NewGuid();
        var ruling = new Ruling
        {
            Id = id,
            CaseTitle = "Detail Case",
            RulingDate = new DateOnly(2023, 6, 1),
            CaseNumber = "S-1",
            JurisdictionArea = "Penal",
            Instance = "CSJN",
            Summary = "Resumen.",
            Holding = "El fallo sostiene X.",
            Court = new Court { Name = "CSJN", JurisdictionArea = "Federal", Territory = "Nac", Instance = "Alta" },
            RulingParticipations = { new RulingParticipation { Role = RulingRole.SIGNATORY, Person = new Person { DisplayName = "Ana Lopez", FirstName = "Ana", LastName = "Lopez" } } },
            RulingKeywords = { new RulingKeyword { SortOrder = 1, Keyword = new Keyword { Description = "kw1" } } },
            RulingStatutes = { new RulingStatute { Articles = "art. 1", Statute = new Statute { Name = "Ley X", Number = "1" } } },
            OutboundCitations = { new Citation { ExternalAlias = "Fallos:1:2", CitationType = CitationType.CITES, TargetRulingId = Guid.NewGuid(), TargetRuling = new Ruling { CaseTitle = "Target", Id = Guid.NewGuid() } } }
        };

        var repo = Substitute.For<IRulingRepository>();
        repo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(ruling);

        var tool = new GetRulingDetailTool();
        var ctx = CreateContext(s => s.AddSingleton(repo));

        var result = await tool.ExecuteAsync($@"{{""rulingId"":""{id}""}}", ctx, CancellationToken.None);

        Assert.Contains("Detail Case", result);
        Assert.Contains("Ana Lopez", result);
        Assert.Contains(id, ctx.ResolvedRulingIds);
    }

    [Fact]
    public async Task GetRulingDetail_InvalidId_ReturnsError()
    {
        var tool = new GetRulingDetailTool();
        var ctx = CreateContext(s => s.AddSingleton(Substitute.For<IRulingRepository>()));

        var result = await tool.ExecuteAsync("""{"rulingId":"not-a-uuid"}""", ctx, CancellationToken.None);

        Assert.Equal("Error: Invalid ruling ID format.", result);
    }

    [Fact]
    public async Task GetRulingCitations_HappyPath_ReturnsOutboundAndInbound()
    {
        var rulingId = Guid.NewGuid();
        var targetId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();

        var graph = Substitute.For<IGraphService>();
        graph.GetOutboundCitationsAsync(rulingId, Arg.Any<CancellationToken>())
            .Returns(new[] { new Citation { SourceRulingId = rulingId, TargetRulingId = targetId, ExternalAlias = "OutAlias", CitationType = CitationType.UPHOLDS } });
        graph.GetInboundCitationsAsync(rulingId, Arg.Any<CancellationToken>())
            .Returns(new[] { new Citation { SourceRulingId = sourceId, TargetRulingId = rulingId, ExternalAlias = "InAlias", CitationType = CitationType.CITES } });

        var repo = Substitute.For<IRulingRepository>();
        repo.GetChatMetadataBatchAsync(Arg.Is<IEnumerable<Guid>>(x => x.Contains(targetId)), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, RulingChatMetadata> { [targetId] = new("Target Case", null, null, new DateOnly(2020, 1, 1), null, null, null) });
        repo.GetChatMetadataBatchAsync(Arg.Is<IEnumerable<Guid>>(x => x.Contains(sourceId)), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, RulingChatMetadata> { [sourceId] = new("Source Case", null, null, new DateOnly(2019, 5, 5), null, null, null) });

        var tool = new GetRulingCitationsTool();
        var ctx = CreateContext(s => { s.AddSingleton(graph); s.AddSingleton(repo); });

        var result = await tool.ExecuteAsync($@"{{""rulingId"":""{rulingId}""}}", ctx, CancellationToken.None);

        Assert.Contains("Target Case", result);
        Assert.Contains("Source Case", result);
    }

    [Fact]
    public async Task GetRulingCitations_InvalidId_ReturnsError()
    {
        var tool = new GetRulingCitationsTool();
        var ctx = CreateContext(s => { s.AddSingleton(Substitute.For<IGraphService>()); s.AddSingleton(Substitute.For<IRulingRepository>()); });

        var result = await tool.ExecuteAsync("""{"rulingId":"x"}""", ctx, CancellationToken.None);

        Assert.Equal("Error: Invalid ruling ID format.", result);
    }

    [Fact]
    public async Task GetRelatedRulings_HappyPath_ReturnsResults()
    {
        var refId = Guid.NewGuid();
        var relatedId = Guid.NewGuid();
        var item = new SearchResultItem(relatedId, "Related Case", "S", null, null, new DateOnly(2022, 3, 3), null, null, "Court B", Array.Empty<string>(), null, 0.88);
        var search = Substitute.For<ISearchService>();
        search.SearchRelatedAsync(refId, 5, Arg.Any<CancellationToken>()).Returns(new[] { item });

        var tool = new GetRelatedRulingsTool();
        var ctx = CreateContext(s => s.AddSingleton(search));

        var result = await tool.ExecuteAsync($@"{{""rulingId"":""{refId}""}}", ctx, CancellationToken.None);

        Assert.Contains("Related Case", result);
        Assert.Contains(relatedId.ToString(), result);
    }

    [Fact]
    public async Task GetRelatedRulings_InvalidId_ReturnsError()
    {
        var tool = new GetRelatedRulingsTool();
        var ctx = CreateContext(s => s.AddSingleton(Substitute.For<ISearchService>()));

        var result = await tool.ExecuteAsync("""{"rulingId":"bad"}""", ctx, CancellationToken.None);

        Assert.Equal("Error: Invalid ruling ID format.", result);
    }

    [Fact]
    public async Task SearchByStatute_HappyPath_ReturnsStatuteAndCase()
    {
        var rid = Guid.NewGuid();
        var row = new StatuteRulingResult(rid, "Statute Case", new DateOnly(2021, 7, 7), "Court Z", "Brief", "Ley Y", "26.994", "art. 5");
        var statuteRepo = Substitute.For<IStatuteRepository>();
        statuteRepo.FindRulingsByStatuteAsync("Ley Y", null, null, 10, Arg.Any<CancellationToken>()).Returns(new[] { row });

        var tool = new SearchByStatuteTool();
        var ctx = CreateContext(s => s.AddSingleton(statuteRepo));

        var result = await tool.ExecuteAsync("""{"statuteName":"Ley Y"}""", ctx, CancellationToken.None);

        Assert.Contains("Statute Case", result);
        Assert.Contains("26.994", result);
    }

    [Fact]
    public async Task SearchByStatute_MissingName_ReturnsError()
    {
        var tool = new SearchByStatuteTool();
        var ctx = CreateContext(s => s.AddSingleton(Substitute.For<IStatuteRepository>()));

        var result = await tool.ExecuteAsync("{}", ctx, CancellationToken.None);

        Assert.Equal("Error: statuteName is required.", result);
    }

    [Fact]
    public async Task CountRulings_HappyPath_ReturnsCountAndFilters()
    {
        var repo = Substitute.For<IRulingRepository>();
        repo.CountAsync(Arg.Any<CountFilters>(), Arg.Any<CancellationToken>()).Returns(12);

        var tool = new CountRulingsTool();
        var ctx = CreateContext(s => s.AddSingleton(repo));

        var result = await tool.ExecuteAsync("""{"jurisdictionArea":"Civil","dateFrom":"2020-01-01"}""", ctx, CancellationToken.None);

        Assert.Contains("[count_rulings: 12 rulings]", result);
        Assert.Contains("jurisdictionArea=Civil", result);
    }

    [Fact]
    public async Task ListCourts_HappyPath_ReturnsCourtLines()
    {
        var courts = new[] { new Court { Id = 1, Name = "C1", JurisdictionArea = "A", Territory = "T", Instance = "I" } };
        var courtRepo = Substitute.For<ICourtRepository>();
        courtRepo.ListAsync("A", null, 50, Arg.Any<CancellationToken>()).Returns(courts);

        var tool = new ListCourtsTool();
        var ctx = CreateContext(s => s.AddSingleton(courtRepo));

        var result = await tool.ExecuteAsync("""{"jurisdictionArea":"A"}""", ctx, CancellationToken.None);

        Assert.Contains("C1", result);
        Assert.Contains("[list_courts: 1 courts]", result);
    }

    [Fact]
    public async Task ListPersons_HappyPath_ReturnsPersonLines()
    {
        var persons = new[] { new PersonWithCount(1, "Juan", "Perez", 3) };
        var personRepo = Substitute.For<IPersonRepository>();
        personRepo.ListWithRulingCountAsync("CSJN", 50, Arg.Any<CancellationToken>()).Returns(persons);

        var tool = new ListPersonsTool();
        var ctx = CreateContext(s => s.AddSingleton(personRepo));

        var result = await tool.ExecuteAsync("""{"courtName":"CSJN"}""", ctx, CancellationToken.None);

        Assert.Contains("Juan Perez", result);
        Assert.Contains("Rulings: 3", result);
    }
}
