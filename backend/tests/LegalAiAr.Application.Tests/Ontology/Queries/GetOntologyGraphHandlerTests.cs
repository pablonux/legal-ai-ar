using LegalAiAr.Application.Ontology;
using LegalAiAr.Application.Ontology.Queries;
using LegalAiAr.Core.Interfaces.Services;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Ontology.Queries;

public class GetOntologyGraphHandlerTests
{
    private readonly OntologyModelProvider _model = new();
    private readonly IOntologyStatsProvider _stats = Substitute.For<IOntologyStatsProvider>();

    public GetOntologyGraphHandlerTests()
    {
        _stats.GetEntityCountsAsync(Arg.Any<CancellationToken>()).Returns(
            new Dictionary<string, int>
            {
                ["Sentencia"] = 1500,
                ["Tribunal"] = 87,
                ["Juez"] = 230,
                ["PalabraClave"] = 4200,
                ["Fuente"] = 3,
            });
        _stats.GetRelationCountsAsync(Arg.Any<CancellationToken>()).Returns(
            new Dictionary<string, int>
            {
                ["citaFallo"] = 3200,
                ["firmadoPor"] = 4500,
                ["citaNorma"] = 6100,
                ["tienePalabraClave"] = 12000,
            });
    }

    [Fact]
    public async Task Handle_MergesInstanceCounts()
    {
        var sut = new GetOntologyGraphHandler(_model, _stats);
        var result = await sut.Handle(new GetOntologyGraphQuery(), CancellationToken.None);

        Assert.NotEmpty(result.Nodes);
        Assert.NotEmpty(result.Edges);

        var sentencia = result.Nodes.First(n => n.Id == "Sentencia");
        Assert.Equal(1500, sentencia.InstanceCount);

        var tribunal = result.Nodes.First(n => n.Id == "Tribunal");
        Assert.Equal(87, tribunal.InstanceCount);

        var ley = result.Nodes.First(n => n.Id == "Ley");
        Assert.Equal(0, ley.InstanceCount);
    }

    [Fact]
    public async Task Handle_MergesRelationCounts()
    {
        var sut = new GetOntologyGraphHandler(_model, _stats);
        var result = await sut.Handle(new GetOntologyGraphQuery(), CancellationToken.None);

        var citaFallo = result.Edges.First(e => e.Label == "citaFallo");
        Assert.Equal(3200, citaFallo.InstanceCount);

        var firmadoPor = result.Edges.First(e => e.Label == "firmadoPor");
        Assert.Equal(4500, firmadoPor.InstanceCount);
    }

    [Fact]
    public async Task Handle_ConceptualEdgesHaveZeroCount()
    {
        var sut = new GetOntologyGraphHandler(_model, _stats);
        var result = await sut.Handle(new GetOntologyGraphQuery(), CancellationToken.None);

        var regula = result.Edges.First(e => e.Label == "regula");
        Assert.Equal(0, regula.InstanceCount);

        var isaEdges = result.Edges.Where(e => e.Type == "is-a").ToList();
        Assert.All(isaEdges, e => Assert.Equal(0, e.InstanceCount));
    }

    [Fact]
    public async Task Handle_NewKbEntitiesHaveCounts()
    {
        var sut = new GetOntologyGraphHandler(_model, _stats);
        var result = await sut.Handle(new GetOntologyGraphQuery(), CancellationToken.None);

        var keyword = result.Nodes.First(n => n.Id == "PalabraClave");
        Assert.Equal(4200, keyword.InstanceCount);

        var source = result.Nodes.First(n => n.Id == "Fuente");
        Assert.Equal(3, source.InstanceCount);
    }
}
