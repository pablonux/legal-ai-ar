using LegalAiAr.Application.Graph.Queries;
using LegalAiAr.Core.Interfaces.Services;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Graph.Queries;

public class SearchEntitiesHandlerTests
{
    private readonly IGraphExplorerRepository _repo = Substitute.For<IGraphExplorerRepository>();
    private readonly SearchEntitiesHandler _sut;

    public SearchEntitiesHandlerTests() => _sut = new SearchEntitiesHandler(_repo);

    [Fact]
    public async Task Handle_ReturnsMappedResults()
    {
        var hits = new List<GraphSearchHit>
        {
            new("ruling:abc", "ruling", "Arriola", "2009-08-25 · CSJN"),
            new("court:1", "court", "CSJN", "Nacional"),
        };

        _repo.SearchEntitiesAsync("arriola", null, Arg.Any<CancellationToken>())
            .Returns(hits);

        var result = await _sut.Handle(new SearchEntitiesQuery("arriola", null), CancellationToken.None);

        Assert.Equal(2, result.Results.Count);
        Assert.Equal("ruling:abc", result.Results[0].Id);
        Assert.Equal("court", result.Results[1].EntityType);
    }

    [Fact]
    public async Task Handle_WithTypes_PassesTypesFilter()
    {
        _repo.SearchEntitiesAsync("test", "ruling,court", Arg.Any<CancellationToken>())
            .Returns(new List<GraphSearchHit>());

        var result = await _sut.Handle(new SearchEntitiesQuery("test", "ruling,court"), CancellationToken.None);

        Assert.Empty(result.Results);
        await _repo.Received(1).SearchEntitiesAsync("test", "ruling,court", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptySearch_ReturnsEmptyList()
    {
        _repo.SearchEntitiesAsync("xyz", null, Arg.Any<CancellationToken>())
            .Returns(new List<GraphSearchHit>());

        var result = await _sut.Handle(new SearchEntitiesQuery("xyz", null), CancellationToken.None);

        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task Handle_PreservesSubtitle()
    {
        var hits = new List<GraphSearchHit>
        {
            new("judge:5", "judge", "Zaffaroni, Eugenio R.", "CSJN"),
        };

        _repo.SearchEntitiesAsync("zaffaroni", null, Arg.Any<CancellationToken>())
            .Returns(hits);

        var result = await _sut.Handle(new SearchEntitiesQuery("zaffaroni", null), CancellationToken.None);

        Assert.Equal("CSJN", result.Results[0].Subtitle);
    }
}
