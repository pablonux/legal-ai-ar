namespace LegalAiAr.Infrastructure.Queue;

/// <summary>
/// Configuration for Azure Storage Queue (replaces Service Bus in Fase 1).
/// Uses the same Storage Account as Blob; connection string is shared.
/// </summary>
public class StorageQueueOptions
{
    public const string SectionName = "AzureStorage";

    /// <summary>
    /// Connection string for Azure Storage Account (Blob + Queue share the same account).
    /// Maps to AzureBlob__ConnectionString or AzureStorage__ConnectionString.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Optional: custom queue name prefix. Default uses {prefix}-crawler, {prefix}-parser, etc. via PipelineQueueNames.
    /// </summary>
    public string? QueuePrefix { get; set; }
}
