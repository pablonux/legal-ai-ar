using LegalAiAr.Application.Ontology;
using LegalAiAr.Application.Ontology.Queries;
using Xunit;

namespace LegalAiAr.Application.Tests.Ontology.Queries;

public class GetOntologyClassesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsClassHierarchy()
    {
        var model = new OntologyModelProvider();
        var sut = new GetOntologyClassesHandler(model);

        var result = await sut.Handle(new GetOntologyClassesQuery(), CancellationToken.None);

        Assert.NotEmpty(result.Classes);
        Assert.Contains(result.Classes, c => c.Id == "NormaJuridica");
        Assert.Contains(result.Classes, c => c.Id == "Sentencia");
    }
}
