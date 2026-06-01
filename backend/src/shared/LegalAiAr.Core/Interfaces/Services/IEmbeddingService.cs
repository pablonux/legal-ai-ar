namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Generates embeddings using text-embedding-3-large (3072 dimensions).
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Generates an embedding vector for the given text.
    /// </summary>
    /// <param name="text">Text to embed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Embedding vector of 3072 dimensions.</returns>
    Task<float[]> GenerateAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embedding vectors for all texts in a single request.
    /// </summary>
    /// <param name="texts">Texts to embed, in order; must not be null or contain null/empty entries.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Embeddings in the same order as <paramref name="texts"/>.</returns>
    Task<IReadOnlyList<float[]>> GenerateBatchAsync(IList<string> texts, CancellationToken cancellationToken = default);
}
