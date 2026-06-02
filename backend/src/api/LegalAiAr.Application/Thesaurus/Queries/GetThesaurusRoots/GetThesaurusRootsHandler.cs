using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Thesaurus;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Thesaurus.Queries.GetThesaurusRoots;

public class GetThesaurusRootsHandler : IRequestHandler<GetThesaurusRootsQuery, IReadOnlyList<ThesaurusTermDto>>
{
    private readonly IThesaurusRepository _repo;

    public GetThesaurusRootsHandler(IThesaurusRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<ThesaurusTermDto>> Handle(
        GetThesaurusRootsQuery request, CancellationToken cancellationToken)
    {
        var terms = await _repo.GetRootTermsAsync(cancellationToken);
        return terms.Select(t => new ThesaurusTermDto(t.Id, t.Label, t.BranchName, t.Depth)).ToList();
    }
}
