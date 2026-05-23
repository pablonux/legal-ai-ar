using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Thesaurus.DTOs;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Thesaurus.Queries.GetThesaurusById;

public class GetThesaurusByIdHandler : IRequestHandler<GetThesaurusByIdQuery, ThesaurusTermDetailDto?>
{
    private readonly IThesaurusRepository _repo;

    public GetThesaurusByIdHandler(IThesaurusRepository repo) => _repo = repo;

    public async Task<ThesaurusTermDetailDto?> Handle(
        GetThesaurusByIdQuery request, CancellationToken cancellationToken)
    {
        var term = await _repo.GetByIdWithRelationsAsync(request.Id, cancellationToken);
        if (term is null) return null;

        var synonyms = MapRelations(term.RelationsAsSource, ThesaurusRelationType.UF);
        var broader = MapRelations(term.RelationsAsSource, ThesaurusRelationType.BT);
        var narrower = MapRelations(term.RelationsAsSource, ThesaurusRelationType.NT);
        var related = MapRelations(term.RelationsAsSource, ThesaurusRelationType.RT);

        return new ThesaurusTermDetailDto(
            term.Id, term.Label, term.BranchName, term.Depth,
            synonyms, broader, narrower, related);
    }

    private static List<ThesaurusRelationDto> MapRelations(
        ICollection<Core.Entities.ThesaurusRelation> relations,
        ThesaurusRelationType type) =>
        relations
            .Where(r => r.RelationType == type)
            .Select(r => new ThesaurusRelationDto(r.TargetTerm.Id, r.TargetTerm.Label))
            .ToList();
}
