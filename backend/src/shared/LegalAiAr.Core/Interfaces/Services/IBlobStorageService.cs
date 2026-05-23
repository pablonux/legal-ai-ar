namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Azure Blob Storage operations for PDFs and documents.
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Uploads content to the specified blob path.
    /// </summary>
    /// <returns>The blob path (same as input if successful).</returns>
    Task<string> UploadAsync(string blobPath, Stream content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads blob content as a stream.
    /// </summary>
    Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a blob exists at the specified path.
    /// </summary>
    Task<bool> ExistsAsync(string blobPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a blob from source to destination (copy + delete). Used when reorganizing by document date.
    /// </summary>
    Task<string> MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);
}
