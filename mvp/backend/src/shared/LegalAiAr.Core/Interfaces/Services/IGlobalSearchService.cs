namespace LegalAiAr.Core.Interfaces.Services;

public record GlobalSearchItem(
    string EntityType,
    string Id,
    string Title,
    string? Subtitle,
    string Route);

public interface IGlobalSearchService
{
    Task<IReadOnlyList<GlobalSearchItem>> SearchAsync(
        string query, int maxPerEntity = 5, CancellationToken cancellationToken = default);
}
