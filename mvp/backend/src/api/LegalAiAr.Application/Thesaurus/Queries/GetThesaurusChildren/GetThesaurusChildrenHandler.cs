using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Thesaurus.DTOs;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Thesaurus.Queries.GetThesaurusChildren;

public class GetThesaurusChildrenHandler : IRequestHandler<GetThesaurusChildrenQuery, IReadOnlyList<ThesaurusTermDto>>
{
    private readonly IThesaurusRepository _repo;

    public GetThesaurusChildrenHandler(IThesaurusRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<ThesaurusTermDto>> Handle(
        GetThesaurusChildrenQuery request, CancellationToken cancellationToken)
    {
        var terms = await _repo.GetChildrenAsync(request.TermId, cancellationToken);
        return terms.Select(t => new ThesaurusTermDto(t.Id, t.Label, t.BranchName, t.Depth)).ToList();
    }
}
