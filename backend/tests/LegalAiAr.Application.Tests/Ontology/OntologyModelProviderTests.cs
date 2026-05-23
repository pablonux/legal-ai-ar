using LegalAiAr.Application.Ontology;
using Xunit;

namespace LegalAiAr.Application.Tests.Ontology;

public class OntologyModelProviderTests
{
    private readonly OntologyModelProvider _sut = new();

    [Fact]
    public void GetClasses_ReturnsCoreClasses()
    {
        var classes = _sut.GetClasses();

        Assert.NotEmpty(classes);

        var core = classes.Where(c => c.Category == "core").ToList();
        Assert.Contains(core, c => c.Id == "NormaJuridica");
        Assert.Contains(core, c => c.Id == "SujetoDeDerecho");
        Assert.Contains(core, c => c.Id == "OrganoEstatal");
        Assert.Contains(core, c => c.Id == "HechoJuridico");
        Assert.Contains(core, c => c.Id == "FuenteDelDerecho");
        Assert.Contains(core, c => c.Id == "Jurisdiccion");
    }

    [Fact]
    public void GetClasses_ReturnsKbEntities()
    {
        var classes = _sut.GetClasses();
        var kbEntities = classes.Where(c => c.Category == "kb-entity").ToList();

        Assert.Contains(kbEntities, c => c.Id == "Sentencia" && c.KbEntity == "Ruling");
        Assert.Contains(kbEntities, c => c.Id == "Tribunal" && c.KbEntity == "Court");
        Assert.Contains(kbEntities, c => c.Id == "PersonaHumana" && c.KbEntity == "Person");
        Assert.Contains(kbEntities, c => c.Id == "PalabraClave" && c.KbEntity == "Keyword");
        Assert.Contains(kbEntities, c => c.Id == "Fuente" && c.KbEntity == "Source");
        Assert.Contains(kbEntities, c => c.Id == "ThesaurusTerm" && c.KbEntity == "ThesaurusTerm");
        Assert.Contains(kbEntities, c => c.Id == "ProcesoJudicial" && c.KbEntity == "JudicialProceeding");
        Assert.Equal(7, kbEntities.Count);
    }

    [Fact]
    public void GetClasses_NormaJuridica_HasCorrectSubclasses()
    {
        var classes = _sut.GetClasses();
        var norma = classes.First(c => c.Id == "NormaJuridica");

        Assert.Contains("Ley", norma.Children);
        Assert.Contains("Decreto", norma.Children);
        Assert.Contains("Constitucion", norma.Children);
        Assert.Contains("Tratado", norma.Children);
    }

    [Fact]
    public void GetClasses_NormaJuridica_HasProperties()
    {
        var classes = _sut.GetClasses();
        var norma = classes.First(c => c.Id == "NormaJuridica");

        Assert.NotEmpty(norma.Properties);
        Assert.Contains(norma.Properties, p => p.Name == "tipo" && p.TaxonomyId == "NormType");
        Assert.Contains(norma.Properties, p => p.Name == "ramaDelDerecho" && p.TaxonomyId == "LegalBranch");
    }

    [Fact]
    public void GetClasses_SubclassesReferenceParent()
    {
        var classes = _sut.GetClasses();
        var ley = classes.First(c => c.Id == "Ley");

        Assert.Equal("NormaJuridica", ley.ParentId);
        Assert.Equal("subclass", ley.Category);
    }

    [Fact]
    public void GetClasses_AllClassesHaveNamespace()
    {
        var classes = _sut.GetClasses();

        foreach (var cls in classes)
        {
            Assert.StartsWith("legar:", cls.Namespace);
        }
    }

    [Fact]
    public void GetGraph_ReturnsNodesAndEdges()
    {
        var (nodes, edges) = _sut.GetGraph();

        Assert.NotEmpty(nodes);
        Assert.NotEmpty(edges);

        Assert.True(nodes.Count > 30);
    }

    [Fact]
    public void GetGraph_ContainsIsAEdges()
    {
        var (_, edges) = _sut.GetGraph();
        var isaEdges = edges.Where(e => e.Type == "is-a").ToList();

        Assert.NotEmpty(isaEdges);
        Assert.Contains(isaEdges, e => e.Source == "NormaJuridica" && e.Target == "Ley");
    }

    [Fact]
    public void GetGraph_ContainsRelationshipEdges()
    {
        var (_, edges) = _sut.GetGraph();
        var relEdges = edges.Where(e => e.Type == "relationship").ToList();

        Assert.NotEmpty(relEdges);
        Assert.Contains(relEdges, e => e.Source == "Sentencia" && e.Target == "Tribunal");
        Assert.Contains(relEdges, e => e.Source == "Sentencia" && e.Target == "PersonaHumana");
        Assert.Contains(relEdges, e => e.Source == "Sentencia" && e.Target == "PalabraClave" && e.Label == "tienePalabraClave");
        Assert.Contains(relEdges, e => e.Source == "Sentencia" && e.Target == "Fuente" && e.Label == "provenienteDe");
        Assert.Contains(relEdges, e => e.Source == "PalabraClave" && e.Target == "ThesaurusTerm" && e.Label == "normalizadoPor");
    }

    [Fact]
    public void GetClasses_NormaJuridica_HasKbRoute()
    {
        var classes = _sut.GetClasses();
        var norma = classes.First(c => c.Id == "NormaJuridica");

        Assert.Equal("/buscar/resultados", norma.KbRoute);
    }

    [Fact]
    public void GetClasses_PalabraClave_HasProperties()
    {
        var classes = _sut.GetClasses();
        var keyword = classes.First(c => c.Id == "PalabraClave");

        Assert.Equal("Keyword", keyword.KbEntity);
        Assert.Equal("/buscar/resultados", keyword.KbRoute);
        Assert.Contains(keyword.Properties, p => p.Name == "descripcion");
    }

    [Fact]
    public void GetClasses_Fuente_HasNoRoute()
    {
        var classes = _sut.GetClasses();
        var source = classes.First(c => c.Id == "Fuente");

        Assert.Equal("Source", source.KbEntity);
        Assert.Null(source.KbRoute);
    }

    [Fact]
    public void GetGraph_ContainsDictaminadoPorEdge()
    {
        var (_, edges) = _sut.GetGraph();
        var relEdges = edges.Where(e => e.Type == "relationship").ToList();

        Assert.Contains(relEdges, e => e.Source == "Sentencia" && e.Target == "Fiscal" && e.Label == "dictaminadoPor");
    }

    [Fact]
    public void GetGraph_ContainsConduceAEdge()
    {
        var (_, edges) = _sut.GetGraph();
        var relEdges = edges.Where(e => e.Type == "relationship").ToList();

        Assert.Contains(relEdges, e => e.Source == "ProcesoJudicial" && e.Target == "Sentencia" && e.Label == "conduceA");
    }

    [Fact]
    public void GetClasses_ProcesoJudicial_IsKbEntity()
    {
        var classes = _sut.GetClasses();
        var proceso = classes.First(c => c.Id == "ProcesoJudicial");

        Assert.Equal("kb-entity", proceso.Category);
        Assert.Equal("JudicialProceeding", proceso.KbEntity);
        Assert.Contains(proceso.Properties, p => p.Name == "expediente");
        Assert.Contains(proceso.Properties, p => p.Name == "cantidadFallos");
    }

    [Fact]
    public void GetGraph_EdgesHaveDefaultZeroInstanceCount()
    {
        var (_, edges) = _sut.GetGraph();

        Assert.All(edges, e => Assert.Equal(0, e.InstanceCount));
    }

    [Fact]
    public void GetTaxonomies_ReturnsAllExpectedTaxonomies()
    {
        var taxonomies = _sut.GetTaxonomies();

        Assert.True(taxonomies.ContainsKey("LegalBranch"));
        Assert.True(taxonomies.ContainsKey("NormType"));
        Assert.True(taxonomies.ContainsKey("NormativeLevel"));
        Assert.True(taxonomies.ContainsKey("CourtType"));
        Assert.True(taxonomies.ContainsKey("PrecedentWeight"));
        Assert.True(taxonomies.ContainsKey("Fuero"));
        Assert.True(taxonomies.ContainsKey("GovernmentLevel"));
    }

    [Fact]
    public void GetTaxonomies_LegalBranch_Has21Values()
    {
        var taxonomies = _sut.GetTaxonomies();
        var legalBranch = taxonomies["LegalBranch"];

        Assert.Equal(21, legalBranch.Values.Count);
        Assert.Contains(legalBranch.Values, v => v.Code == "PUB_CONST");
        Assert.Contains(legalBranch.Values, v => v.Code == "PRIV_CIVIL");
        Assert.Contains(legalBranch.Values, v => v.Code == "DIG_DATOS");
    }

    [Fact]
    public void GetTaxonomies_Values_HaveGroupsForLegalBranch()
    {
        var taxonomies = _sut.GetTaxonomies();
        var legalBranch = taxonomies["LegalBranch"];

        var groups = legalBranch.Values.Select(v => v.Group).Distinct().ToList();
        Assert.Contains("Derecho público", groups);
        Assert.Contains("Derecho privado", groups);
        Assert.Contains("Derecho social", groups);
        Assert.Contains("Derecho digital", groups);
    }
}
