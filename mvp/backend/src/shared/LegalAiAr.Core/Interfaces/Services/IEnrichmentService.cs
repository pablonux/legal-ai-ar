namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Calls the nano model for structured extraction (judges, statutes, prosecutor opinion).
/// Used by EnrichmentWorker strategies.
/// </summary>
public interface IEnrichmentService
{
    /// <summary>
    /// Calls the configured model with system prompt, user content, and json_schema for structured output.
    /// Returns the raw JSON response string.
    /// </summary>
    /// <param name="systemPrompt">System instructions for the model.</param>
    /// <param name="userContent">User message content (e.g. ruling text or citations to classify).</param>
    /// <param name="schemaName">Name for the JSON schema (e.g. "judges_extraction").</param>
    /// <param name="jsonSchema">JSON schema string for response_format.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw JSON string matching the schema.</returns>
    Task<string> GetStructuredOutputAsync(
        string systemPrompt,
        string userContent,
        string schemaName,
        string jsonSchema,
        CancellationToken cancellationToken = default);
}
