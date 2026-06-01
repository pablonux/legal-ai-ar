using LegalAiAr.Core.Exceptions;
using LegalAiAr.Infrastructure.Crawling.Sources;

namespace LegalAiAr.Infrastructure.Tests.Crawling;

public sealed class CsjnPaginationParserTests
{
    [Fact]
    public void ParseRecordsWithMetadata_ValidRecords_ReturnsMetadata()
    {
        const string json = """
            {"Records":[
              {"Codigo":"1","idAnalisis":"A1","identificadorExpediente":"EXP-1","materiaSecretaria":"M","anioFallo":"2020","tieneVotos":"S"}
            ]}
            """;

        var list = CsjnPaginationParser.ParseRecordsWithMetadata(json, pageIndex: 0);

        Assert.Single(list);
        Assert.Equal("1", list[0].DocumentId);
        Assert.Equal("A1", list[0].AnalysisId);
        Assert.Equal("EXP-1", list[0].CaseNumber);
        Assert.True(list[0].HasVotes);
    }

    [Fact]
    public void ParseRecordsWithMetadata_CodigoAsNumber_Works()
    {
        const string json = """{"Records":[{"Codigo":7962861,"idAnalisis":"xyz"}]}""";

        var list = CsjnPaginationParser.ParseRecordsWithMetadata(json, 0);

        Assert.Single(list);
        Assert.Equal("7962861", list[0].DocumentId);
        Assert.Equal("xyz", list[0].AnalysisId);
    }

    [Fact]
    public void ParseRecordsWithMetadata_MissingCodigo_ThrowsCsjnSchemaViolation()
    {
        const string json = """{"Records":[{"idAnalisis":"A"}]}""";

        var ex = Assert.Throws<CsjnSchemaViolationException>(() => CsjnPaginationParser.ParseRecordsWithMetadata(json, 0));
        Assert.Contains("Codigo", ex.Message);
    }

    [Fact]
    public void ParseRecordsWithMetadata_SessionTimeout_ThrowsCsjnSessionTimeout()
    {
        const string html = "<html>Ha excedido el tiempo de inactividad</html>";

        Assert.Throws<CsjnSessionTimeoutException>(() => CsjnPaginationParser.ParseRecordsWithMetadata(html, 0));
    }

    [Fact]
    public void ParseRecordsWithMetadata_NonJson_ReturnsEmpty()
    {
        var list = CsjnPaginationParser.ParseRecordsWithMetadata("<html>not json</html>", 0);
        Assert.Empty(list);
    }
}
