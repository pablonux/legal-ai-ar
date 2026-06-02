using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Thesaurus;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Thesaurus.Queries.SearchThesaurus;

public class SearchThesaurusHandler : IRequestHandler<SearchThesaurusQuery, IReadOnlyList<ThesaurusTermDto>>
{
    private readonly IThesaurusRepository _repo;

    public SearchThesaurusHandler(IThesaurusRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<ThesaurusTermDto>> Handle(
        SearchThesaurusQuery request, CancellationToken cancellationToken)
    {
        var terms = await _repo.SearchAsync(
            request.Search, Math.Clamp(request.Limit, 1, 50), cancellationToken);

        return terms.Select(t => new ThesaurusTermDto(t.Id, t.Label, t.BranchName, t.Depth)).ToList();
    }
}
