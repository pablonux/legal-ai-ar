using LegalAiAr.Core.Messages;
using LegalAiAr.Worker.Indexer.Steps;

namespace LegalAiAr.Worker.Indexer.Tests.Steps;

public class ExtractChunkMentionsStepTests
{
    [Fact]
    public void BuildCandidates_ExtractsPersonsStatutesKeywords()
    {
        var message = new IndexerMessage(
            DocumentId: "123", ContentHash: "h", SourceId: 1,
            Ruling: new RulingData(
                CaseTitle: "Test", RulingDate: new DateOnly(2024, 1, 1),
                CaseNumber: null, JurisdictionArea: null, Instance: null,
                Jurisdiction: null, ResourceType: null, RulingDirection: null,
                SubjectArea: null, IsUnconstitutional: false,
                Summary: null, Holding: null, FullText: "text", BlobPath: "b"),
            Persons: [new PersonData("Ricardo", "Lorenzetti", "SIGNATORY")],
            Keywords: [new KeywordData(2093, "IMPUESTO A LAS GANANCIAS", 0)],
            Statutes: [new StatuteData("20.628", "Ley de Impuesto a las Ganancias", "art. 2")],
            Citations: [],
            Chunks: []);

        var candidates = ExtractChunkMentionsStep.BuildCandidates(message);

        Assert.Contains(candidates, c => c.EntityType == "Person" && c.SearchTerm == "Lorenzetti");
        Assert.Contains(candidates, c => c.EntityType == "Statute" && c.SearchTerm == "20.628");
        Assert.Contains(candidates, c => c.EntityType == "Keyword" && c.SearchTerm == "IMPUESTO A LAS GANANCIAS");
    }

    [Fact]
    public void BuildCandidates_SkipsShortNames()
    {
        var message = new IndexerMessage(
            DocumentId: "123", ContentHash: "h", SourceId: 1,
            Ruling: new RulingData(
                CaseTitle: "Test", RulingDate: new DateOnly(2024, 1, 1),
                CaseNumber: null, JurisdictionArea: null, Instance: null,
                Jurisdiction: null, ResourceType: null, RulingDirection: null,
                SubjectArea: null, IsUnconstitutional: false,
                Summary: null, Holding: null, FullText: "text", BlobPath: "b"),
            Persons: [],
            Keywords: [new KeywordData(1, "AB", 0)],
            Statutes: [new StatuteData("", "Ley", null)],
            Citations: [],
            Chunks: []);

        var candidates = ExtractChunkMentionsStep.BuildCandidates(message);

        Assert.DoesNotContain(candidates, c => c.SearchTerm == "AB");
        Assert.DoesNotContain(candidates, c => c.SearchTerm == "Ley");
    }
}
