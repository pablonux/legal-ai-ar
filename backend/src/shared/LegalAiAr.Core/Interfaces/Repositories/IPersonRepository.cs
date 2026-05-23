using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Core.Interfaces.Repositories;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Person> GetOrCreateAsync(string displayName, string? firstName = null, string? lastName = null, int? csjnMinistroId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists persons ordered by ruling participation count (desc), with optional court filter.
    /// </summary>
    Task<IReadOnlyList<PersonWithCount>> ListWithRulingCountAsync(
        string? courtName = null,
        int maxResults = 50,
        CancellationToken cancellationToken = default);

    /// <summary>Searches persons by display name (LIKE), returns with ruling count and court name.</summary>
    Task<IReadOnlyList<PersonListItem>> SearchAsync(
        string? query = null,
        string? courtName = null,
        int limit = 50,
        PersonListView listView = PersonListView.All,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a person with court, ruling count, and recent rulings.</summary>
    Task<PersonDetail?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    Task<int> BackfillCurrentCourtIdAsync(CancellationToken cancellationToken = default);
}
