using LegalAiAr.Core.Messages;
using LegalAiAr.Worker.Indexer.Chunking;

namespace LegalAiAr.Worker.Indexer.Tests.Chunking;

public class TextChunkingServiceTests
{
    private readonly TextChunkingService _sut = new();

    [Fact]
    public void Chunk_WhenEmptyText_ReturnsEmptyList()
    {
        var result = _sut.Chunk("");

        Assert.Empty(result);
    }

    [Fact]
    public void Chunk_WhenWhitespaceOnly_ReturnsEmptyList()
    {
        var result = _sut.Chunk("   \n\t  ");

        Assert.Empty(result);
    }

    [Fact]
    public void Chunk_WhenNull_ReturnsEmptyList()
    {
        var result = _sut.Chunk(null!);

        Assert.Empty(result);
    }

    [Fact]
    public void Chunk_WhenShortText_ReturnsSingleChunk()
    {
        var text = "Por ello, se confirma la sentencia apelada.";

        var result = _sut.Chunk(text);

        Assert.Single(result);
        Assert.Equal(0, result[0].Index);
        Assert.Equal(text, result[0].Text);
    }

    [Fact]
    public void Chunk_WhenTextUnder512Tokens_ReturnsSingleChunk()
    {
        var text = string.Join(" ", Enumerable.Repeat("palabra", 100));

        var result = _sut.Chunk(text);

        Assert.Single(result);
        Assert.Equal(0, result[0].Index);
        Assert.Equal(text, result[0].Text);
    }

    [Fact]
    public void Chunk_WhenTextExceeds512Tokens_ReturnsMultipleChunksWithOverlap()
    {
        var words = Enumerable.Repeat("considerando que la Corte Suprema de Justicia de la Nación tiene competencia", 80);
        var text = string.Join(" ", words);

        var result = _sut.Chunk(text);

        Assert.True(result.Count >= 2);
        Assert.Equal(0, result[0].Index);
        Assert.Equal(1, result[1].Index);
        Assert.All(result, c => Assert.False(string.IsNullOrWhiteSpace(c.Text)));
    }

    [Fact]
    public void Chunk_WhenUnicodeSpanish_PreservesCharacters()
    {
        var text = "La Constitución Nacional establece en su artículo 14 el derecho de asociación. Año 2024.";

        var result = _sut.Chunk(text);

        Assert.Single(result);
        Assert.Contains("Constitución", result[0].Text);
        Assert.Contains("artículo", result[0].Text);
    }

    [Fact]
    public void GetEffectiveChunks_WhenMessageHasChunks_ReturnsMessageChunks()
    {
        var message = CreateIndexerMessage(chunks: [new ChunkData(0, "Existing chunk")]);

        var result = _sut.GetEffectiveChunks(message);

        Assert.Single(result);
        Assert.Equal("Existing chunk", result[0].Text);
        Assert.Equal(0, result[0].Index);
    }

    [Fact]
    public void GetEffectiveChunks_WhenMessageChunksEmpty_ChunksFullText()
    {
        var fullText = "Texto corto del fallo.";
        var message = CreateIndexerMessage(chunks: [], fullText);

        var result = _sut.GetEffectiveChunks(message);

        Assert.Single(result);
        Assert.Equal(fullText, result[0].Text);
        Assert.Equal(0, result[0].Index);
    }

    [Fact]
    public void GetEffectiveChunks_WhenMessageChunksNull_ChunksFullText()
    {
        var fullText = "Otro texto.";
        var message = CreateIndexerMessage(chunks: [], fullText);

        var result = _sut.GetEffectiveChunks(message);

        Assert.Single(result);
        Assert.Equal(fullText, result[0].Text);
    }

    private static IndexerMessage CreateIndexerMessage(
        IReadOnlyList<ChunkData> chunks,
        string fullText = "Full text")
    {
        return new IndexerMessage(
            DocumentId: "123",
            ContentHash: "hash",
            SourceId: 1,
            Ruling: new RulingData(
                CaseTitle: "Test",
                RulingDate: DateOnly.FromDateTime(DateTime.UtcNow),
                CaseNumber: null,
                JurisdictionArea: null,
                Instance: null,
                Jurisdiction: null,
                ResourceType: null,
                RulingDirection: null,
                SubjectArea: null,
                IsUnconstitutional: false,
                Summary: null,
                Holding: null,
                FullText: fullText,
                BlobPath: "path.pdf"),
            Persons: [],
            Keywords: [],
            Statutes: [],
            Citations: [],
            Chunks: chunks);
    }
}
