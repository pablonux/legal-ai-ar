using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Worker.Indexer.Steps;
using NSubstitute;

namespace LegalAiAr.Worker.Indexer.Tests.Steps;

public class IndexSearchStepTests
{
    private const int EmbeddingDimensions = 3072;

    [Fact]
    public async Task ExecuteAsync_IndexesRulingAndChunks()
    {
        var searchIndex = Substitute.For<ISearchIndexService>();
        var sut = new IndexSearchStep(searchIndex, Substitute.For<Microsoft.Extensions.Logging.ILogger<IndexSearchStep>>());

        var rulingId = Guid.NewGuid();
        var message = CreateIndexerMessage();
        var embeddingsResult = new EmbeddingsResult(
            CreateFakeEmbedding(),
            [new ChunkWithEmbedding(0, "Chunk 0", "[CSJN | 2024-03-15 | Test Case] Chunk 0", CreateFakeEmbedding()), new ChunkWithEmbedding(1, "Chunk 1", "[CSJN | 2024-03-15 | Test Case] Chunk 1", CreateFakeEmbedding())]);

        await sut.ExecuteAsync(rulingId, message, embeddingsResult);

        await searchIndex.Received(1).IndexRulingAsync(
            Arg.Is<LegalAiAr.Core.Models.RulingIndexInput>(r =>
                r.RulingId == rulingId &&
                r.CaseTitle == "Test Case" &&
                r.Summary == "Summary" &&
                r.Holding == "Holding" &&
                r.Keywords.Contains("IMPUESTO A LAS GANANCIAS")),
            Arg.Any<CancellationToken>());

        await searchIndex.Received(1).IndexChunksAsync(
            Arg.Is<IReadOnlyList<LegalAiAr.Core.Models.ChunkIndexInput>>(c =>
                c.Count == 2 &&
                c[0].ChunkIndex == 0 &&
                c[0].Text == "Chunk 0" &&
                c[1].ChunkIndex == 1 &&
                c[1].Text == "Chunk 1"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoChunks_OnlyIndexesRuling()
    {
        var searchIndex = Substitute.For<ISearchIndexService>();
        var sut = new IndexSearchStep(searchIndex, Substitute.For<Microsoft.Extensions.Logging.ILogger<IndexSearchStep>>());

        var rulingId = Guid.NewGuid();
        var message = CreateIndexerMessage();
        var embeddingsResult = new EmbeddingsResult(CreateFakeEmbedding(), []);

        await sut.ExecuteAsync(rulingId, message, embeddingsResult);

        await searchIndex.Received(1).IndexRulingAsync(Arg.Any<LegalAiAr.Core.Models.RulingIndexInput>(), Arg.Any<CancellationToken>());
        await searchIndex.DidNotReceive().IndexChunksAsync(Arg.Any<IReadOnlyList<LegalAiAr.Core.Models.ChunkIndexInput>>(), Arg.Any<CancellationToken>());
    }

    private static IndexerMessage CreateIndexerMessage()
    {
        return new IndexerMessage(
            DocumentId: "8048522",
            ContentHash: "hash",
            SourceId: 1,
            Ruling: new RulingData(
                CaseTitle: "Test Case",
                RulingDate: new DateOnly(2024, 3, 15),
                CaseNumber: null,
                JurisdictionArea: "Tributario",
                Instance: "CSJN",
                Jurisdiction: null,
                ResourceType: null,
                RulingDirection: "UPHOLDS",
                SubjectArea: null,
                IsUnconstitutional: false,
                Summary: "Summary",
                Holding: "Holding",
                FullText: "Full text",
                BlobPath: "csjn/2024/8048522.pdf"),
            Persons: [],
            Keywords: [new KeywordData(2093, "IMPUESTO A LAS GANANCIAS", 0)],
            Statutes: [],
            Citations: [],
            Chunks: []);
    }

    private static float[] CreateFakeEmbedding()
    {
        var arr = new float[EmbeddingDimensions];
        for (var i = 0; i < EmbeddingDimensions; i++)
            arr[i] = i * 0.001f;
        return arr;
    }
}
