namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Ensures catalog rows for SAIJ thesaurus ingestion jobs (Source 6 + CrawlerConfig) exist.
/// Idempotent: safe when migrations already applied.
/// </summary>
public interface IThesaurusIngestCatalogBootstrap
{
    Task EnsureAsync(CancellationToken cancellationToken = default);
}
