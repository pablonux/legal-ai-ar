namespace LegalAiAr.Core.Models;

/// <summary>
/// Result of a fetch operation: content bytes, hash, and blob path.
/// </summary>
/// <param name="ContentHash">SHA-256 hash of the downloaded content.</param>
/// <param name="BlobPath">Path in Blob Storage where the content was uploaded.</param>
/// <param name="ContentBytes">Raw content bytes (PDF/HTML). Null when streamed directly to blob.</param>
public record FetchedDocument(
    string ContentHash,
    string BlobPath,
    byte[]? ContentBytes = null);
