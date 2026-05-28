using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Worker.Discoverer.Strategies;

/// <summary>
/// Routes CSJN discovery to the correct source based on the crawl type.
/// "fallos-destacados" uses the curated-collection source; everything else uses acuerdo-based discovery.
/// </summary>
public sealed class CsjnDiscoverAdapter : IDiscoverStrategy
{
    private readonly CrawlerSourceDiscoverAdapter _acuerdoAdapter;
    private readonly CrawlerSourceDiscoverAdapter _fallosDestacadosAdapter;

    public CsjnDiscoverAdapter(ICrawlerSource acuerdoSource, ICrawlerSource fallosDestacadosSource)
    {
        _acuerdoAdapter = new CrawlerSourceDiscoverAdapter(acuerdoSource);
        _fallosDestacadosAdapter = new CrawlerSourceDiscoverAdapter(fallosDestacadosSource);
    }

    public int? LastTotalSearchResults => ResolveActive(null).LastTotalSearchResults;

    public async Task<int?> GetTotalForRangeAsync(DiscovererMessage message, CancellationToken cancellationToken = default)
        => await ResolveActive(message.Type).GetTotalForRangeAsync(message, cancellationToken);

    public IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> DiscoverAsync(
        DiscovererMessage message,
        CancellationToken cancellationToken = default)
        => ResolveActive(message.Type).DiscoverAsync(message, cancellationToken);

    private CrawlerSourceDiscoverAdapter ResolveActive(string? type)
        => string.Equals(type, "fallos-destacados", StringComparison.OrdinalIgnoreCase)
            ? _fallosDestacadosAdapter
            : _acuerdoAdapter;
}
