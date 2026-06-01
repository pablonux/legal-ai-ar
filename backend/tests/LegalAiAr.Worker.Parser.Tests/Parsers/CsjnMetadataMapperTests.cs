using LegalAiAr.Core.Messages;
using LegalAiAr.Worker.Parser.Parsers;

namespace LegalAiAr.Worker.Parser.Tests.Parsers;

public class CsjnMetadataMapperTests
{
    [Fact]
    public void ToExtractedMetadata_MapsAllFieldsCorrectly()
    {
        var api = new CsjnApiMetadata(
            CaseTitle: "Test Case",
            RulingDate: new DateOnly(2024, 3, 15),
            CaseNumber: "CAF 9548/2021/CA1-CS1",
            Jurisdiction: "APELACION EXTRAORDINARIA",
            ResourceType: "RECURSO EXTRAORDINARIO FEDERAL",
            RulingDirection: "UPHOLDS",
            SubjectArea: "Tributario - Bancario",
            IsUnconstitutional: true,
            Summary: "Summary text",
            Holding: "Holding text",
            Keywords: [new CsjnKeywordDto(2093, "IMPUESTO A LAS GANANCIAS")],
            Citations: [new CsjnCitationDto("Fallos: 328:1883", 56748)],
            CitedBy: [new CsjnCitedByDto("818955", "CAF 003507/2024/CS001")]);

        var result = CsjnMetadataMapper.ToExtractedMetadata(api);

        Assert.Equal("Test Case", result.CaseTitle);
        Assert.Equal(new DateOnly(2024, 3, 15), result.RulingDate);
        Assert.Equal("CAF 9548/2021/CA1-CS1", result.CaseNumber);
        Assert.Equal("Corte Suprema de Justicia de la Nación", result.Court);
        Assert.Equal("APELACION EXTRAORDINARIA", result.JurisdictionArea);
        Assert.Equal("CSJN", result.Instance);
        Assert.Equal("UPHOLDS", result.RulingDirection);
        Assert.Equal("Summary text", result.Summary);
        Assert.Equal("Holding text", result.Holding);
        Assert.Single(result.Keywords);
        Assert.Equal(2093, result.Keywords[0].ExternalCode);
        Assert.Equal("IMPUESTO A LAS GANANCIAS", result.Keywords[0].Description);
        Assert.Single(result.Citations);
        Assert.Equal("Fallos: 328:1883", result.Citations[0].Alias);
        Assert.Equal(56748, result.Citations[0].SummaryId);

        Assert.Equal("APELACION EXTRAORDINARIA", result.Jurisdiction);
        Assert.Equal("RECURSO EXTRAORDINARIO FEDERAL", result.ResourceType);
        Assert.Equal("Tributario - Bancario", result.SubjectArea);
        Assert.True(result.IsUnconstitutional);
        Assert.NotNull(result.CitedBy);
        Assert.Single(result.CitedBy!);
        Assert.Equal("818955", result.CitedBy![0].AnalysisId);
        Assert.Equal("CAF 003507/2024/CS001", result.CitedBy![0].CaseNumber);
    }

    [Fact]
    public void ToExtractedMetadata_EmptyCollections_ReturnsEmptyLists()
    {
        var api = new CsjnApiMetadata(
            CaseTitle: "Minimal",
            RulingDate: new DateOnly(2024, 1, 1),
            CaseNumber: null,
            Jurisdiction: null,
            ResourceType: null,
            RulingDirection: null,
            SubjectArea: null,
            IsUnconstitutional: false,
            Summary: null,
            Holding: null,
            Keywords: [],
            Citations: [],
            CitedBy: []);

        var result = CsjnMetadataMapper.ToExtractedMetadata(api);

        Assert.Empty(result.Keywords);
        Assert.Empty(result.Citations);
    }

    [Fact]
    public void GetMissingFields_ForCsjnSourceId_ReturnsJudgesCitedStatutesCitationTypes()
    {
        var result = CsjnMetadataMapper.GetMissingFields(1);

        Assert.Equal(3, result.Count);
        Assert.Contains("judges", result);
        Assert.Contains("cited_statutes", result);
        Assert.Contains("citation_types", result);
    }

    [Fact]
    public void GetMissingFields_ForNonCsjnSourceId_ReturnsEmptyList()
    {
        var result = CsjnMetadataMapper.GetMissingFields(2);

        Assert.Empty(result);
    }
}
