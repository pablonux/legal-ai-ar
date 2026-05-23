using System.Collections.Generic;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Worker.Indexer.Steps;
using NSubstitute;

namespace LegalAiAr.Worker.Indexer.Tests.Steps;

public class GenerateEmbeddingsStepTests
{
    private const int EmbeddingDimensions = 3072;

    [Fact]
    public async Task ExecuteAsync_WhenSummaryAndHoldingPresent_UsesThemForRulingEmbedding()
    {
        var embeddingService = Substitute.For<IEmbeddingService>();
        var rulingEmbedding = CreateFakeEmbedding();
        embeddingService.GenerateBatchAsync(Arg.Any<IList<string>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var texts = callInfo.Arg<IList<string>>();
                var list = new List<float[]>(texts.Count);
                list.Add(rulingEmbedding);
                for (var i = 1; i < texts.Count; i++)
                    list.Add(CreateFakeEmbedding());
                return list;
            });

        var sut = new GenerateEmbeddingsStep(embeddingService, Substitute.For<IEnrichmentService>(), Substitute.For<Microsoft.Extensions.Logging.ILogger<GenerateEmbeddingsStep>>());
        var message = CreateIndexerMessage(summary: "Summary text", holding: "Holding text");
        var chunks = new List<ChunkData> { new(0, "Chunk content") };

        var result = await sut.ExecuteAsync(message, chunks);

        Assert.Equal(rulingEmbedding, result.RulingEmbedding);
        await embeddingService.Received(1).GenerateBatchAsync(
            Arg.Is<IList<string>>(l => l.Count == 2 && l[0].Contains("Summary text") && l[0].Contains("Holding text")),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenSummaryAndHoldingEmpty_UsesFirstChunkAsFallback()
    {
        var embeddingService = Substitute.For<IEmbeddingService>();
        var fallbackEmbedding = CreateFakeEmbedding();
        embeddingService.GenerateBatchAsync(Arg.Any<IList<string>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var texts = callInfo.Arg<IList<string>>();
                var list = new List<float[]>(texts.Count);
                list.Add(fallbackEmbedding);
                for (var i = 1; i < texts.Count; i++)
                    list.Add(CreateFakeEmbedding());
                return list;
            });

        var sut = new GenerateEmbeddingsStep(embeddingService, Substitute.For<IEnrichmentService>(), Substitute.For<Microsoft.Extensions.Logging.ILogger<GenerateEmbeddingsStep>>());
        var message = CreateIndexerMessage(summary: null, holding: null, fullText: "Full text");
        var chunks = new List<ChunkData> { new(0, "First chunk text") };

        var result = await sut.ExecuteAsync(message, chunks);

        Assert.Equal(fallbackEmbedding, result.RulingEmbedding);
        await embeddingService.Received(1).GenerateBatchAsync(
            Arg.Is<IList<string>>(l => l.Count >= 1 && l[0] == "First chunk text"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenChunksPresent_GeneratesEmbeddingForEachChunk()
    {
        var embeddingService = Substitute.For<IEmbeddingService>();
        embeddingService.GenerateBatchAsync(Arg.Any<IList<string>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var texts = callInfo.Arg<IList<string>>();
                var list = new List<float[]>(texts.Count);
                for (var i = 0; i < texts.Count; i++)
                    list.Add(CreateFakeEmbedding());
                return list;
            });

        var sut = new GenerateEmbeddingsStep(embeddingService, Substitute.For<IEnrichmentService>(), Substitute.For<Microsoft.Extensions.Logging.ILogger<GenerateEmbeddingsStep>>());
        var message = CreateIndexerMessage(summary: "S", holding: "H");
        var chunks = new List<ChunkData>
        {
            new(0, "Chunk 0"),
            new(1, "Chunk 1")
        };

        var result = await sut.ExecuteAsync(message, chunks);

        Assert.Equal(2, result.ChunkEmbeddings.Count);
        Assert.Equal(0, result.ChunkEmbeddings[0].Index);
        Assert.Equal("Chunk 0", result.ChunkEmbeddings[0].Text);
        Assert.Equal(EmbeddingDimensions, result.ChunkEmbeddings[0].Embedding.Length);
        Assert.Equal(1, result.ChunkEmbeddings[1].Index);
        Assert.Equal("Chunk 1", result.ChunkEmbeddings[1].Text);
    }

    [Fact]
    public async Task ExecuteAsync_WhenChunkTextEmpty_SkipsChunk()
    {
        var embeddingService = Substitute.For<IEmbeddingService>();
        embeddingService.GenerateBatchAsync(Arg.Any<IList<string>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var texts = callInfo.Arg<IList<string>>();
                var list = new List<float[]>(texts.Count);
                for (var i = 0; i < texts.Count; i++)
                    list.Add(CreateFakeEmbedding());
                return list;
            });

        var sut = new GenerateEmbeddingsStep(embeddingService, Substitute.For<IEnrichmentService>(), Substitute.For<Microsoft.Extensions.Logging.ILogger<GenerateEmbeddingsStep>>());
        var message = CreateIndexerMessage(summary: "S", holding: "H");
        var chunks = new List<ChunkData>
        {
            new(0, "Valid chunk"),
            new(1, ""),
            new(2, "   ")
        };

        var result = await sut.ExecuteAsync(message, chunks);

        Assert.Single(result.ChunkEmbeddings);
        Assert.Equal("Valid chunk", result.ChunkEmbeddings[0].Text);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoChunks_ReturnsEmptyChunkEmbeddings()
    {
        var embeddingService = Substitute.For<IEmbeddingService>();
        embeddingService.GenerateBatchAsync(Arg.Any<IList<string>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var texts = callInfo.Arg<IList<string>>();
                var list = new List<float[]>(texts.Count);
                for (var i = 0; i < texts.Count; i++)
                    list.Add(CreateFakeEmbedding());
                return list;
            });

        var sut = new GenerateEmbeddingsStep(embeddingService, Substitute.For<IEnrichmentService>(), Substitute.For<Microsoft.Extensions.Logging.ILogger<GenerateEmbeddingsStep>>());
        var message = CreateIndexerMessage(summary: "S", holding: "H");
        var chunks = new List<ChunkData>();

        var result = await sut.ExecuteAsync(message, chunks);

        Assert.Empty(result.ChunkEmbeddings);
        Assert.Equal(EmbeddingDimensions, result.RulingEmbedding.Length);
    }

    [Fact]
    public async Task ExecuteAsync_WithLlmConfig_UsesLlmContextualization()
    {
        var embeddingService = Substitute.For<IEmbeddingService>();
        embeddingService.GenerateBatchAsync(Arg.Any<IList<string>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var texts = callInfo.Arg<IList<string>>();
                var list = new List<float[]>(texts.Count);
                for (var i = 0; i < texts.Count; i++)
                    list.Add(CreateFakeEmbedding());
                return list;
            });

        var enrichmentService = Substitute.For<IEnrichmentService>();
        enrichmentService.GetStructuredOutputAsync(
                Arg.Any<string>(), Arg.Any<string>(),
                Arg.Is("chunk_context"), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns("""{"context":"Este fragmento trata sobre impuesto a las ganancias."}""");

        var llmConfig = new LegalAiAr.Core.Entities.EmbeddingConfig
        {
            Id = 2,
            Version = "v2-llm-contextual",
            EmbeddingModel = "text-embedding-3-large",
            EmbeddingDimensions = 3072,
            ContextualizationMethod = "llm-contextual",
            ChunkingStrategy = "fixed-512-overlap-50",
            ChunkSize = 512,
            ChunkOverlap = 50,
            IsActive = true
        };

        var sut = new GenerateEmbeddingsStep(embeddingService, enrichmentService,
            Substitute.For<Microsoft.Extensions.Logging.ILogger<GenerateEmbeddingsStep>>());
        var message = CreateIndexerMessage(summary: "S", holding: "H");
        var chunks = new List<ChunkData> { new(0, "Chunk about taxes") };

        var result = await sut.ExecuteAsync(message, chunks, llmConfig);

        Assert.Single(result.ChunkEmbeddings);
        Assert.Contains("Este fragmento trata sobre impuesto", result.ChunkEmbeddings[0].ContextualizedText);
        Assert.Contains("Chunk about taxes", result.ChunkEmbeddings[0].ContextualizedText);
        await enrichmentService.Received(1).GetStructuredOutputAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Is("chunk_context"), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithLlmConfig_FallsBackOnLlmError()
    {
        var embeddingService = Substitute.For<IEmbeddingService>();
        embeddingService.GenerateBatchAsync(Arg.Any<IList<string>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var texts = callInfo.Arg<IList<string>>();
                var list = new List<float[]>(texts.Count);
                for (var i = 0; i < texts.Count; i++)
                    list.Add(CreateFakeEmbedding());
                return list;
            });

        var enrichmentService = Substitute.For<IEnrichmentService>();
        enrichmentService.GetStructuredOutputAsync(
                Arg.Any<string>(), Arg.Any<string>(),
                Arg.Is("chunk_context"), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns<string>(_ => throw new InvalidOperationException("LLM unavailable"));

        var llmConfig = new LegalAiAr.Core.Entities.EmbeddingConfig
        {
            Id = 2, Version = "v2-llm", EmbeddingModel = "x", EmbeddingDimensions = 3072,
            ContextualizationMethod = "llm-contextual", ChunkingStrategy = "s", ChunkSize = 512, ChunkOverlap = 50, IsActive = true
        };

        var sut = new GenerateEmbeddingsStep(embeddingService, enrichmentService,
            Substitute.For<Microsoft.Extensions.Logging.ILogger<GenerateEmbeddingsStep>>());
        var message = CreateIndexerMessage(summary: "S", holding: "H");
        var chunks = new List<ChunkData> { new(0, "Chunk text") };

        var result = await sut.ExecuteAsync(message, chunks, llmConfig);

        Assert.Single(result.ChunkEmbeddings);
        Assert.StartsWith("[", result.ChunkEmbeddings[0].ContextualizedText);
        Assert.Contains("Chunk text", result.ChunkEmbeddings[0].ContextualizedText);
    }

    private static float[] CreateFakeEmbedding()
    {
        var arr = new float[EmbeddingDimensions];
        for (var i = 0; i < EmbeddingDimensions; i++)
            arr[i] = i * 0.001f;
        return arr;
    }

    private static IndexerMessage CreateIndexerMessage(
        string? summary = "Summary",
        string? holding = "Holding",
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
                Summary: summary,
                Holding: holding,
                FullText: fullText,
                BlobPath: "path.pdf"),
            Persons: [],
            Keywords: [],
            Statutes: [],
            Citations: [],
            Chunks: []);
    }
}
