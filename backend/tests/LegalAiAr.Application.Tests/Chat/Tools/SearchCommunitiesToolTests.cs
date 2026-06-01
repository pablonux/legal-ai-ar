using LegalAiAr.Application.Chat.Tools;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LegalAiAr.Application.Tests.Chat.Tools;

public class SearchCommunitiesToolTests
{
    private static ToolExecutionContext CreateContext(IGraphCommunityRepository communityRepo)
    {
        var services = new ServiceCollection();
        services.AddSingleton(communityRepo);
        return new ToolExecutionContext { Services = services.BuildServiceProvider() };
    }

    [Fact]
    public async Task ExecuteAsync_WithMatchingCommunities_ReturnsSummaries()
    {
        var repo = Substitute.For<IGraphCommunityRepository>();
        repo.SearchByKeywordAsync("ambiental", Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new List<GraphCommunity>
            {
                new()
                {
                    Id = 1, Level = 0, Title = "Derecho ambiental - contaminación",
                    Summary = "Cluster de fallos sobre contaminación ambiental",
                    KeyFindings = "La Corte ha ampliado la legitimación activa",
                    EntityCount = 15
                }
            });

        var sut = new SearchCommunitiesTool();
        var context = CreateContext(repo);

        var result = await sut.ExecuteAsync("""{"query":"ambiental"}""", context, CancellationToken.None);

        Assert.Contains("1 communities", result);
        Assert.Contains("Derecho ambiental", result);
        Assert.Contains("contaminación ambiental", result);
        Assert.Contains("legitimación activa", result);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoCommunities_ReturnsNoResults()
    {
        var repo = Substitute.For<IGraphCommunityRepository>();
        repo.SearchByKeywordAsync("xyz", Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new List<GraphCommunity>());

        var sut = new SearchCommunitiesTool();
        var context = CreateContext(repo);

        var result = await sut.ExecuteAsync("""{"query":"xyz"}""", context, CancellationToken.None);

        Assert.Contains("no communities found", result);
    }

    [Fact]
    public async Task ExecuteAsync_WithLevelFilter_FiltersResults()
    {
        var repo = Substitute.For<IGraphCommunityRepository>();
        repo.SearchByKeywordAsync("laboral", Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new List<GraphCommunity>
            {
                new() { Id = 1, Level = 0, Title = "Laboral detalle", Summary = "S1", EntityCount = 5 },
                new() { Id = 2, Level = 1, Title = "Laboral general", Summary = "S2", EntityCount = 30 }
            });

        var sut = new SearchCommunitiesTool();
        var context = CreateContext(repo);

        var result = await sut.ExecuteAsync("""{"query":"laboral","level":1}""", context, CancellationToken.None);

        Assert.Contains("1 communities", result);
        Assert.Contains("Laboral general", result);
        Assert.DoesNotContain("Laboral detalle", result);
    }
}
