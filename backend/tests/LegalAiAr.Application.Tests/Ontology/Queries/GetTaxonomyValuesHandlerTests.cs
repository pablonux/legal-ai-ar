using LegalAiAr.Application.Ontology;
using LegalAiAr.Application.Ontology.Queries;
using LegalAiAr.Core.Interfaces.Services;
using NSubstitute;
using Xunit;

namespace LegalAiAr.Application.Tests.Ontology.Queries;

public class GetTaxonomyValuesHandlerTests
{
    [Fact]
    public async Task Handle_ValidTaxonomy_ReturnsTaxonomyWithCounts()
    {
        var model = new OntologyModelProvider();
        var stats = Substitute.For<IOntologyStatsProvider>();
        stats.GetTaxonomyCountsAsync("LegalBranch", Arg.Any<CancellationToken>()).Returns(
            new Dictionary<string, int>
            {
                ["PUB_CONST"] = 245,
                ["PRIV_CIVIL"] = 412,
            });

        var sut = new GetTaxonomyValuesHandler(model, stats);

        var result = await sut.Handle(new GetTaxonomyValuesQuery("LegalBranch"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("LegalBranch", result!.Id);
        Assert.Equal("Rama del derecho", result.Name);
        Assert.NotEmpty(result.Values);

        var pubConst = result.Values.First(v => v.Code == "PUB_CONST");
        Assert.Equal(245, pubConst.Count);
        Assert.Equal("Derecho constitucional", pubConst.Label);
        Assert.Equal("Derecho público", pubConst.Group);
    }

    [Fact]
    public async Task Handle_UnknownTaxonomy_ReturnsNull()
    {
        var model = new OntologyModelProvider();
        var stats = Substitute.For<IOntologyStatsProvider>();

        var sut = new GetTaxonomyValuesHandler(model, stats);

        var result = await sut.Handle(new GetTaxonomyValuesQuery("Unknown"), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_MissingCountsDefaultToZero()
    {
        var model = new OntologyModelProvider();
        var stats = Substitute.For<IOntologyStatsProvider>();
        stats.GetTaxonomyCountsAsync("CourtType", Arg.Any<CancellationToken>()).Returns(
            new Dictionary<string, int>());

        var sut = new GetTaxonomyValuesHandler(model, stats);

        var result = await sut.Handle(new GetTaxonomyValuesQuery("CourtType"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.All(result!.Values, v => Assert.Equal(0, v.Count));
    }
}
