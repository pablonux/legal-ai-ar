namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// RAG chat service for jurisprudential Q&amp;A. Calls GPT-4o with context from the Knowledge Base.
/// Used by the Chat endpoint.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Sends a chat message to GPT-4o with the given system prompt and user content.
    /// Returns the complete response text (non-streaming). Use StreamChatAsync for SSE.
    /// </summary>
    /// <param name="systemPrompt">System instructions (e.g. legal RAG prompt).</param>
    /// <param name="userContent">User message including context and query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Complete response text from the model.</returns>
    Task<string> ChatAsync(
        string systemPrompt,
        string userContent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams chat response from GPT-4o. Yields text chunks as they arrive.
    /// Used for SSE streaming to the client.
    /// </summary>
    /// <param name="systemPrompt">System instructions (e.g. legal RAG prompt).</param>
    /// <param name="userContent">User message including context and query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async sequence of text chunks.</returns>
    IAsyncEnumerable<string> StreamChatAsync(
        string systemPrompt,
        string userContent,
        CancellationToken cancellationToken = default);
}
