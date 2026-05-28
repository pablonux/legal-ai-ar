using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Pipeline.Strategies;

/// <summary>
/// Discovers documents from a source for a given entity type.
/// Yields batches of discovered documents; each batch corresponds to one page from the source.
/// </summary>
public interface IDiscoverStrategy
{
    /// <summary>
    /// Total results reported by the source after the last call to <see cref="DiscoverAsync"/>.
    /// Null when the source does not provide this information.
    /// </summary>
    int? LastTotalSearchResults { get; }

    /// <summary>
    /// Runs a search for the given range and returns only the total count (no pagination).
    /// Used for pre-flight split resolution.
    /// </summary>
    Task<int?> GetTotalForRangeAsync(DiscovererMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers documents for the given message parameters.
    /// Yields pages (batches) of documents as they are fetched from the source.
    /// </summary>
    IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> DiscoverAsync(
        DiscovererMessage message,
        CancellationToken cancellationToken = default);
}
