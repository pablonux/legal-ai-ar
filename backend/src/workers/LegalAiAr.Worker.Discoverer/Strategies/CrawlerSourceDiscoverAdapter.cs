using System.Runtime.CompilerServices;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Worker.Discoverer.Strategies;

/// <summary>
/// Adapts an existing <see cref="ICrawlerSource"/> to the new <see cref="IDiscoverStrategy"/> interface.
/// Converts <see cref="DiscovererMessage"/> to <see cref="CrawlerMessage"/> for the legacy source.
/// </summary>
public sealed class CrawlerSourceDiscoverAdapter : IDiscoverStrategy
{
    private readonly ICrawlerSource _source;

    public CrawlerSourceDiscoverAdapter(ICrawlerSource source) => _source = source;

    public int? LastTotalSearchResults => _source.LastTotalSearchResults;

    public async Task<int?> GetTotalForRangeAsync(DiscovererMessage message, CancellationToken cancellationToken = default)
    {
        return await _source.GetTotalForRangeAsync(ToCrawlerMessage(message), cancellationToken);
    }

    public async IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> DiscoverAsync(
        DiscovererMessage message,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var crawlerMsg = ToCrawlerMessage(message);
        await foreach (var batch in _source.DiscoverAsync(crawlerMsg, cancellationToken))
        {
            yield return batch;
        }
    }

    private static CrawlerMessage ToCrawlerMessage(DiscovererMessage msg) => new(
        SourceId: msg.SourceId,
        DocumentType: msg.EntityType.ToString().ToLowerInvariant(),
        Type: msg.Type,
        Since: msg.Since,
        DateFrom: msg.DateFrom,
        DateTo: msg.DateTo,
        IngestionJobId: msg.IngestionJobId,
        UseCache: msg.UseCache,
        Reprocess: msg.Reprocess,
        MaxDocuments: msg.MaxDocuments);
}
