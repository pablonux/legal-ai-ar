using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Worker.Enrichment.Chunking;
using LegalAiAr.Worker.Enrichment.Strategies;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LegalAiAr.Worker.Enrichment.Tests.Strategies;

public class CsjnEnrichmentStrategyTests
{
    private static CsjnEnrichmentStrategy CreateSut(
        IEnrichmentService? enrichmentService = null,
        TextChunkingService? chunkingService = null)
    {
        var enrichment = enrichmentService ?? Substitute.For<IEnrichmentService>();
        var chunking = chunkingService ?? new TextChunkingService();
        var logger = Substitute.For<ILogger<CsjnEnrichmentStrategy>>();
        return new CsjnEnrichmentStrategy(enrichment, chunking, logger);
    }

    private static EnrichmentMessage CreateMessage(
        IReadOnlyList<string> missingFields,
        string normalizedText = "Sample ruling text for testing.",
        IReadOnlyList<ExtractedCitationDto>? citations = null)
    {
        var meta = new ExtractedMetadata(
            CaseTitle: "Test Case",
            RulingDate: new DateOnly(2024, 3, 15),
            CaseNumber: "CAF 9548/2021/CA1-CS1",
            Court: "CSJN",
            JurisdictionArea: "Tributario",
            Instance: "CSJN",
            RulingDirection: "UPHOLDS",
            Summary: "Summary",
            Holding: "Holding",
            Keywords: [new ExtractedKeywordDto(2093, "IMPUESTO A LAS GANANCIAS")],
            Citations: citations ?? [new ExtractedCitationDto("Fallos: 328:1883", 56748)]);

        return new EnrichmentMessage(
            DocumentId: "8048522",
            SourceId: 1,
            NormalizedText: normalizedText,
            ExtractedMetadata: meta,
            MissingFields: missingFields,
            BlobPath: "csjn/2024/8048522.pdf");
    }

    [Fact]
    public async Task EnrichAsync_WithNoMissingFields_ReturnsIndexerMessageWithEmptyJudgesAndStatutes()
    {
        var sut = CreateSut();
        var message = CreateMessage([]);

        var result = await sut.EnrichAsync(message);

        Assert.NotNull(result);
        Assert.Equal("8048522", result.DocumentId);
        Assert.Equal("Test Case", result.Ruling.CaseTitle);
        Assert.Equal(new DateOnly(2024, 3, 15), result.Ruling.RulingDate);
        Assert.Empty(result.Persons);
        Assert.Empty(result.Statutes);
        Assert.Single(result.Keywords);
        Assert.Equal(2093, result.Keywords[0].ExternalCode);
        Assert.Single(result.Citations);
        Assert.Equal("Fallos: 328:1883", result.Citations[0].ExternalAlias);
        Assert.Equal("CITES", result.Citations[0].CitationType);
        Assert.NotEmpty(result.Chunks);
    }

    [Fact]
    public async Task EnrichAsync_WithJudgesMissing_ReturnsExtractedJudges()
    {
        var enrichment = Substitute.For<IEnrichmentService>();
        enrichment.GetStructuredOutputAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Is("judges_extraction"),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns("""{"judges":[{"firstName":"Ricardo","lastName":"Lorenzetti","participationType":"SIGNATORY"}]}""");

        var sut = CreateSut(enrichment);
        var message = CreateMessage(["judges"]);

        var result = await sut.EnrichAsync(message);

        Assert.Single(result.Persons);
        Assert.Equal("Ricardo", result.Persons[0].FirstName);
        Assert.Equal("Lorenzetti", result.Persons[0].LastName);
        Assert.Equal("SIGNATORY", result.Persons[0].RulingRole);
    }

    [Fact]
    public async Task EnrichAsync_WithCitedStatutesMissing_ReturnsExtractedStatutes()
    {
        var enrichment = Substitute.For<IEnrichmentService>();
        enrichment.GetStructuredOutputAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Is("statutes_extraction"),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns("""{"statutes":[{"number":"24.767","name":"Ley de Concursos","articles":"art. 14"}]}""");

        var sut = CreateSut(enrichment);
        var message = CreateMessage(["cited_statutes"]);

        var result = await sut.EnrichAsync(message);

        Assert.Single(result.Statutes);
        Assert.Equal("24.767", result.Statutes[0].Number);
        Assert.Equal("Ley de Concursos", result.Statutes[0].Name);
        Assert.Equal("art. 14", result.Statutes[0].Articles);
    }

    [Fact]
    public async Task EnrichAsync_WithCitationTypesMissing_ReturnsClassifiedCitationTypes()
    {
        var textWithUpholdsSignal =
            "El tribunal ratifica lo decidido en Fallos: 328:1883 respecto de la cuestión tributaria planteada.";

        var sut = CreateSut();
        var message = CreateMessage(["citation_types"], normalizedText: textWithUpholdsSignal);

        var result = await sut.EnrichAsync(message);

        Assert.Single(result.Citations);
        Assert.Equal("UPHOLDS", result.Citations[0].CitationType);
    }

    [Fact]
    public async Task EnrichAsync_WithNullBlobPath_DerivesBlobPathFromSourceAndDocument()
    {
        var sut = CreateSut();
        var message = CreateMessage([]) with { BlobPath = null };

        var result = await sut.EnrichAsync(message);

        Assert.NotNull(result.Ruling.BlobPath);
        Assert.Contains("csjn", result.Ruling.BlobPath);
        Assert.Contains("8048522", result.Ruling.BlobPath);
    }

    [Fact]
    public async Task EnrichAsync_WithApiVotes_BuildsVoteData()
    {
        var sut = CreateSut();
        var votes = new List<CsjnVoteDto>
        {
            new("UNANIMIDAD", "ROSATTI, LORENZETTI", "1-5",
                [new CsjnMinistroDto(68, "ROSATTI"), new CsjnMinistroDto(1, "LORENZETTI")])
        };
        var meta = new ExtractedMetadata(
            CaseTitle: "Test", RulingDate: new DateOnly(2024, 1, 1),
            CaseNumber: null, Court: "CSJN", JurisdictionArea: null,
            Instance: "CSJN", RulingDirection: null, Summary: null, Holding: null,
            Keywords: [], Citations: [],
            ApiPersons: [new ExtractedPersonDto("ROSATTI", "SIGNATORY", 68)],
            ApiVotes: votes);

        var message = new EnrichmentMessage("123", 1, "text", meta, [], BlobPath: "b.pdf");

        var result = await sut.EnrichAsync(message);

        Assert.NotNull(result.Votes);
        Assert.Single(result.Votes);
        Assert.Equal("UNANIMIDAD", result.Votes[0].VoteType);
        Assert.Equal("1-5", result.Votes[0].Pages);
        Assert.Equal(2, result.Votes[0].Signatories.Count);
    }

    [Fact]
    public async Task EnrichAsync_WithSumarios_PassesThroughSumarioData()
    {
        var sut = CreateSut();
        var sumarios = new List<ExtractedSumarioDto>
        {
            new(176590, "Headnote text", "349", "203", 1,
                [new ExtractedKeywordDto(765, "BENEFICIO DE LITIGAR SIN GASTOS")])
        };
        var meta = new ExtractedMetadata(
            CaseTitle: "Test", RulingDate: new DateOnly(2024, 1, 1),
            CaseNumber: null, Court: "CSJN", JurisdictionArea: null,
            Instance: "CSJN", RulingDirection: null, Summary: null, Holding: null,
            Keywords: [], Citations: [],
            Sumarios: sumarios);

        var message = new EnrichmentMessage("123", 1, "text", meta, [], BlobPath: "b.pdf");

        var result = await sut.EnrichAsync(message);

        Assert.NotNull(result.Sumarios);
        Assert.Single(result.Sumarios);
        Assert.Equal(176590, result.Sumarios[0].ExternalId);
        Assert.Equal("Headnote text", result.Sumarios[0].Text);
        Assert.Single(result.Sumarios[0].Keywords);
    }

    [Fact]
    public async Task EnrichAsync_WithSynthesesAndLinks_PassesThroughData()
    {
        var sut = CreateSut();
        var syntheses = new List<ExtractedSynthesisDto>
        {
            new("Synthesis text", 0)
        };
        var links = new List<ExtractedLinkDto>
        {
            new("http://example.com/dict.pdf", "DICTAMEN", "INTERNAL")
        };
        var meta = new ExtractedMetadata(
            CaseTitle: "Test", RulingDate: new DateOnly(2024, 1, 1),
            CaseNumber: null, Court: "CSJN", JurisdictionArea: null,
            Instance: "CSJN", RulingDirection: null, Summary: null, Holding: null,
            Keywords: [], Citations: [],
            Syntheses: syntheses, Links: links);

        var message = new EnrichmentMessage("123", 1, "text", meta, [], BlobPath: "b.pdf");

        var result = await sut.EnrichAsync(message);

        Assert.NotNull(result.Syntheses);
        Assert.Single(result.Syntheses);
        Assert.Equal("Synthesis text", result.Syntheses[0].Text);

        Assert.NotNull(result.Links);
        Assert.Single(result.Links);
        Assert.Equal("http://example.com/dict.pdf", result.Links[0].Url);
        Assert.Equal("DICTAMEN", result.Links[0].Title);
    }

    [Fact]
    public async Task EnrichAsync_WithProsecutorOpinion_AddsProsecutorPersonAndDocumentUrl()
    {
        var enrichment = Substitute.For<IEnrichmentService>();
        enrichment.GetStructuredOutputAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Is("prosecutor_opinion"),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns("""{"hasDictamen":true,"prosecutorName":"Laura Monti","summary":"Favorable","recommendedDirection":"UPHOLDS","agreedWithCourt":true}""");

        var sut = CreateSut(enrichment);
        var links = new List<ExtractedLinkDto>
        {
            new("http://csjn.gov.ar/dictamen123.pdf", "DICTAMEN DE LA PROCURACION", "INTERNAL")
        };
        var meta = new ExtractedMetadata(
            CaseTitle: "Test", RulingDate: new DateOnly(2024, 1, 1),
            CaseNumber: null, Court: "CSJN", JurisdictionArea: null,
            Instance: "CSJN", RulingDirection: null, Summary: null, Holding: null,
            Keywords: [], Citations: [],
            Links: links);
        var message = new EnrichmentMessage("123", 1, "text", meta, ["prosecutor_opinion"], BlobPath: "b.pdf");

        var result = await sut.EnrichAsync(message);

        Assert.NotNull(result.ProsecutorOpinion);
        Assert.Equal("Laura Monti", result.ProsecutorOpinion.ProsecutorName);
        Assert.Equal("http://csjn.gov.ar/dictamen123.pdf", result.ProsecutorOpinion.DocumentUrl);

        var prosecutor = result.Persons.FirstOrDefault(p => p.RulingRole == "PROSECUTOR");
        Assert.NotNull(prosecutor);
        Assert.Equal("Laura Monti", prosecutor.LastName);
    }

    [Fact]
    public async Task EnrichAsync_WithStructuredArticles_IncludesInStatuteData()
    {
        var sut = CreateSut();
        var apiStatutes = new List<ExtractedStatuteDto>
        {
            new("20.744", "Ley de Contrato de Trabajo", "art. 14 inc. b", "14", "b")
        };
        var meta = new ExtractedMetadata(
            CaseTitle: "Test", RulingDate: new DateOnly(2024, 1, 1),
            CaseNumber: null, Court: "CSJN", JurisdictionArea: null,
            Instance: "CSJN", RulingDirection: null, Summary: null, Holding: null,
            Keywords: [], Citations: [],
            ApiStatutes: apiStatutes);

        var message = new EnrichmentMessage("123", 1, "text", meta, [], BlobPath: "b.pdf");

        var result = await sut.EnrichAsync(message);

        Assert.Single(result.Statutes);
        Assert.NotNull(result.Statutes[0].StructuredArticles);
        Assert.Single(result.Statutes[0].StructuredArticles!);
        Assert.Equal("14", result.Statutes[0].StructuredArticles![0].Article);
        Assert.Equal("b", result.Statutes[0].StructuredArticles![0].Subsection);
    }

    [Fact]
    public async Task EnrichAsync_WithCvCaseTitle_ExtractsPartiesIntoMessage()
    {
        var sut = CreateSut();
        var meta = new ExtractedMetadata(
            CaseTitle: "González, Pedro c/ Estado Nacional s/ amparo",
            RulingDate: new DateOnly(2024, 6, 1),
            CaseNumber: "CAF 1234/2024",
            Court: "CSJN", JurisdictionArea: null,
            Instance: "CSJN", RulingDirection: null, Summary: null, Holding: null,
            Keywords: [], Citations: []);

        var message = new EnrichmentMessage("999", 1, "text", meta, [], BlobPath: "b.pdf");

        var result = await sut.EnrichAsync(message);

        Assert.NotNull(result.Parties);
        Assert.Equal(2, result.Parties.Count);
        Assert.Equal("González, Pedro", result.Parties[0].Name);
        Assert.Equal("PLAINTIFF", result.Parties[0].PartyRole);
        Assert.Equal("Physical", result.Parties[0].PersonType);
        Assert.Equal("Estado Nacional", result.Parties[1].Name);
        Assert.Equal("DEFENDANT", result.Parties[1].PartyRole);
        Assert.Equal("Legal", result.Parties[1].PersonType);
    }
}
