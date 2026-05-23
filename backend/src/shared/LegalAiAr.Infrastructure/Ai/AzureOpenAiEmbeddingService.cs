using System.ClientModel;
using Azure.AI.OpenAI;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace LegalAiAr.Infrastructure.Ai;

/// <summary>
/// Generates embeddings using Azure OpenAI text-embedding-3-large (3072 dimensions).
/// </summary>
public class AzureOpenAiEmbeddingService : IEmbeddingService
{
    private const int ExpectedDimensions = 3072;
    private readonly EmbeddingClient _embeddingClient;
    private readonly ILogger<AzureOpenAiEmbeddingService> _logger;

    public AzureOpenAiEmbeddingService(IOptions<AzureOpenAiOptions> options, ILogger<AzureOpenAiEmbeddingService> logger)
    {
        _logger = logger;
        var opts = options.Value;

        if (string.IsNullOrWhiteSpace(opts.Endpoint))
            throw new InvalidOperationException("AzureOpenAI:Endpoint is required.");
        if (string.IsNullOrWhiteSpace(opts.ApiKey))
            throw new InvalidOperationException("AzureOpenAI:ApiKey is required.");
        if (string.IsNullOrWhiteSpace(opts.EmbeddingDeploymentName))
            throw new InvalidOperationException("AzureOpenAI:EmbeddingDeploymentName is required.");

        var credential = new ApiKeyCredential(opts.ApiKey);
        var azureClient = new AzureOpenAIClient(new Uri(opts.Endpoint.TrimEnd('/') + "/"), credential);
        _embeddingClient = azureClient.GetEmbeddingClient(opts.EmbeddingDeploymentName);
    }

    /// <inheritdoc />
    public async Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty.", nameof(text));

        var response = await _embeddingClient.GenerateEmbeddingsAsync(
            new[] { text },
            cancellationToken: cancellationToken);

        var embedding = response.Value.FirstOrDefault()
            ?? throw new InvalidOperationException("Azure OpenAI returned no embedding for the input text.");

        var vector = embedding.ToFloats();
        var floats = vector.ToArray();

        if (floats.Length != ExpectedDimensions)
        {
            _logger.LogWarning(
                "Embedding dimensions mismatch: expected {Expected}, got {Actual}",
                ExpectedDimensions, floats.Length);
        }

        return floats;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<float[]>> GenerateBatchAsync(IList<string> texts, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(texts);
        if (texts.Count == 0)
            throw new ArgumentException("At least one text is required.", nameof(texts));

        for (var i = 0; i < texts.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(texts[i]))
                throw new ArgumentException($"Text at index {i} cannot be null or empty.", nameof(texts));
        }

        var response = await _embeddingClient.GenerateEmbeddingsAsync(texts, cancellationToken: cancellationToken);

        if (response.Value.Count != texts.Count)
        {
            throw new InvalidOperationException(
                $"Azure OpenAI returned {response.Value.Count} embeddings for {texts.Count} inputs.");
        }

        var result = new List<float[]>(response.Value.Count);
        foreach (var item in response.Value)
        {
            var floats = item.ToFloats().ToArray();
            if (floats.Length != ExpectedDimensions)
            {
                _logger.LogWarning(
                    "Embedding dimensions mismatch: expected {Expected}, got {Actual}",
                    ExpectedDimensions, floats.Length);
            }

            result.Add(floats);
        }

        return result;
    }
}
