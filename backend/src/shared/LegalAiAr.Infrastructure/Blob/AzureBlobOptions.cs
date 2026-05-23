namespace LegalAiAr.Infrastructure.Blob;

/// <summary>
/// Configuration options for Azure Blob Storage.
/// </summary>
public class AzureBlobOptions
{
    public const string SectionName = "AzureBlob";

    /// <summary>
    /// Azure Storage Account connection string (Blob + Queue share same account).
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Container name for PDFs (e.g. rulings-pdfs).
    /// </summary>
    public string ContainerName { get; set; } = "rulings-pdfs";
}
