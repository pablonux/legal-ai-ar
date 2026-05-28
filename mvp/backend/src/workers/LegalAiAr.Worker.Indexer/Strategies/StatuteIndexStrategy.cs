using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Messages;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Indexer.Strategies;

/// <summary>
/// Indexes a statute entity. Currently a pass-through since statutes
/// are persisted by the Persister worker and don't require search indexing yet.
/// When statute search is added, this will generate embeddings and index to search.
/// </summary>
public sealed class StatuteIndexStrategy : IIndexStrategy
{
    private readonly ILogger<StatuteIndexStrategy> _logger;

    public StatuteIndexStrategy(ILogger<StatuteIndexStrategy> logger)
    {
        _logger = logger;
    }

    public Task IndexAsync(IndexerMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Statute indexing complete for document {DocumentId} (pass-through)",
            message.DocumentId);
        return Task.CompletedTask;
    }
}
