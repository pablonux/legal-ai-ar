using LegalAiAr.Application.Graph.Queries;
using LegalAiAr.Core.Interfaces.Services;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Graph.Queries;

public class GetNeighborhoodHandlerTests
{
    private readonly IGraphExplorerRepository _repo = Substitute.For<IGraphExplorerRepository>();
    private readonly GetNeighborhoodHandler _sut;

    public GetNeighborhoodHandlerTests() => _sut = new GetNeighborhoodHandler(_repo);

    [Fact]
    public async Task Handle_ReturnsMappedNeighborhood()
    {
        var center = new GraphNodeRaw("ruling:abc", "ruling", "Arriola", "2009-08-25 · CSJN", null);
        var nodes = new List<GraphNodeRaw>
        {
            new("court:1", "court", "CSJN", "Nacional", null),
            new("judge:42", "judge", "Lorenzetti, Ricardo L.", "CSJN", null),
        };
        var edges = new List<GraphEdgeRaw>
        {
            new("e1", "ruling:abc", "court:1", "issuedBy", null),
            new("e2", "ruling:abc", "judge:42", "signedBy", "MAJORITY"),
        };

        _repo.GetNeighborhoodAsync("ruling", "abc", Arg.Any<CancellationToken>())
            .Returns(new GraphNeighborhood(center, nodes, edges));

        var result = await _sut.Handle(new GetNeighborhoodQuery("ruling", "abc"), CancellationToken.None);

        Assert.Equal("ruling:abc", result.Center.Id);
        Assert.Equal("ruling", result.Center.EntityType);
        Assert.Equal("Arriola", result.Center.Label);
        Assert.Equal(2, result.Nodes.Count);
        Assert.Equal(2, result.Edges.Count);
        Assert.Equal("issuedBy", result.Edges[0].Type);
        Assert.Equal("MAJORITY", result.Edges[1].Label);
    }

    [Fact]
    public async Task Handle_PreservesNodeProperties()
    {
        var props = new Dictionary<string, string> { ["legalBranch"] = "PUB_PENAL" };
        var center = new GraphNodeRaw("ruling:x", "ruling", "Test", "2024-01-01 · CSJN", props);

        _repo.GetNeighborhoodAsync("ruling", "x", Arg.Any<CancellationToken>())
            .Returns(new GraphNeighborhood(center, [], []));

        var result = await _sut.Handle(new GetNeighborhoodQuery("ruling", "x"), CancellationToken.None);

        Assert.NotNull(result.Center.Properties);
        Assert.Equal("PUB_PENAL", result.Center.Properties!["legalBranch"]);
    }

    [Fact]
    public async Task Handle_EmptyNeighborhood_ReturnsEmptyLists()
    {
        var center = new GraphNodeRaw("keyword:99", "keyword", "Estupefacientes", null, null);

        _repo.GetNeighborhoodAsync("keyword", "99", Arg.Any<CancellationToken>())
            .Returns(new GraphNeighborhood(center, [], []));

        var result = await _sut.Handle(new GetNeighborhoodQuery("keyword", "99"), CancellationToken.None);

        Assert.Empty(result.Nodes);
        Assert.Empty(result.Edges);
    }
}
