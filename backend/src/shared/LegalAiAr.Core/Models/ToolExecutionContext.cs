namespace LegalAiAr.Core.Models;

/// <summary>
/// Per-request context passed to each tool during execution.
/// Tools resolve scoped dependencies from <see cref="Services"/> rather than constructor injection.
/// </summary>
public sealed class ToolExecutionContext
{
    /// <summary>
    /// Scoped <see cref="IServiceProvider"/> from the current HTTP request.
    /// Tools resolve their dependencies (e.g. <c>ISearchService</c>, <c>IRulingRepository</c>) from this.
    /// </summary>
    public IServiceProvider Services { get; init; } = null!;

    /// <summary>
    /// Ruling IDs already fetched in this request. Prevents redundant lookups across multiple tool calls.
    /// </summary>
    public HashSet<Guid> ResolvedRulingIds { get; } = new();
}
