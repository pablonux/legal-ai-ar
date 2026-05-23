using LegalAiAr.Application.Chat.Pipeline;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Application.Tests.Chat.Pipeline;

public class RuleBasedQueryEnricherTests
{
    [global::Xunit.Fact]
    public void ClassifyIntent_Statistics_ReturnsStatistics()
    {
        var sut = new RuleBasedQueryEnricher();

        var (intent, confidence) = sut.ClassifyIntent("¿Cuántos fallos hay sobre despido?");

        Assert.Equal("statistics", intent);
        Assert.Equal(0.90f, confidence);
    }

    [global::Xunit.Fact]
    public void ClassifyIntent_StatuteResearch_ReturnsStatuteResearch()
    {
        var sut = new RuleBasedQueryEnricher();

        var (intent, confidence) = sut.ClassifyIntent("Fallos que aplican la Ley 20.744");

        Assert.Equal("statute_research", intent);
        Assert.Equal(0.85f, confidence);
    }

    [global::Xunit.Fact]
    public void ClassifyIntent_PrecedentExploration_ReturnsPrecedent()
    {
        var sut = new RuleBasedQueryEnricher();

        var (intent, confidence) = sut.ClassifyIntent("¿Qué fallos citan a Ekmekdjian?");

        Assert.Equal("precedent_exploration", intent);
        Assert.Equal(0.85f, confidence);
    }

    [global::Xunit.Fact]
    public void ClassifyIntent_Detail_ReturnsDetail()
    {
        var sut = new RuleBasedQueryEnricher();

        var (intent, confidence) = sut.ClassifyIntent("Mostrá los detalles del fallo Arriola");

        Assert.Equal("detail", intent);
        Assert.Equal(0.80f, confidence);
    }

    [global::Xunit.Fact]
    public void ClassifyIntent_General_ReturnsGeneral()
    {
        var sut = new RuleBasedQueryEnricher();

        var (intent, confidence) = sut.ClassifyIntent("Jurisprudencia argentina reciente");

        Assert.Equal("general", intent);
        Assert.Null(confidence);
    }

    [global::Xunit.Fact]
    public void ExtractEntities_YearRange_ExtractsTemporal()
    {
        var sut = new RuleBasedQueryEnricher();

        var enrichment = sut.ExtractEntities("fallos entre 2020 y 2024", "general", null);

        Assert.NotNull(enrichment);
        var t = Assert.Single(enrichment!.Temporal);
        Assert.Equal(2020, t.From);
        Assert.Equal(2024, t.To);
    }

    [global::Xunit.Fact]
    public void ExtractEntities_SinceYear_ExtractsTemporal()
    {
        var sut = new RuleBasedQueryEnricher();

        var enrichment = sut.ExtractEntities("fallos desde 2022", "general", null);

        Assert.NotNull(enrichment);
        var t = Assert.Single(enrichment!.Temporal);
        Assert.Equal(2022, t.From);
        Assert.Null(t.To);
    }

    [global::Xunit.Fact]
    public void ExtractEntities_Court_ExtractsCourt()
    {
        var sut = new RuleBasedQueryEnricher();

        var enrichment = sut.ExtractEntities("fallos de la CSJN", "general", null);

        Assert.NotNull(enrichment);
        Assert.Contains("CSJN", enrichment!.Courts);
    }

    [global::Xunit.Fact]
    public void ExtractEntities_Law_ExtractsStatute()
    {
        var sut = new RuleBasedQueryEnricher();

        var enrichment = sut.ExtractEntities("Ley 26.994", "general", null);

        Assert.NotNull(enrichment);
        var statute = Assert.Single(enrichment!.Statutes);
        Assert.Equal("Ley 26.994", statute.Reference);
    }

    [global::Xunit.Fact]
    public void ExtractEntities_ArticleCN_ExtractsStatuteWithArticle()
    {
        var sut = new RuleBasedQueryEnricher();

        var enrichment = sut.ExtractEntities("art. 14 de la Constitución Nacional", "general", null);

        Assert.NotNull(enrichment);
        var statute = Assert.Single(enrichment!.Statutes);
        Assert.Equal("Constitución Nacional", statute.Reference);
        Assert.Equal(new[] { "14" }, statute.Articles);
    }

    [global::Xunit.Fact]
    public void ExtractEntities_CaseFormat_ExtractsCase()
    {
        var sut = new RuleBasedQueryEnricher();

        var enrichment = sut.ExtractEntities("Ekmekdjian c/ Sofovich", "general", null);

        Assert.NotNull(enrichment);
        Assert.Contains("Ekmekdjian c/ Sofovich", enrichment!.Cases);
    }

    [global::Xunit.Fact]
    public void ExtractEntities_NoEntities_NullConfidence_ReturnsNull()
    {
        var sut = new RuleBasedQueryEnricher();

        var enrichment = sut.ExtractEntities("algo genérico", "general", null);

        Assert.Null(enrichment);
    }
}
