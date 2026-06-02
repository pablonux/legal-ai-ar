using LegalAiAr.Application.Common.Mappings;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Rulings;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Application.Rulings.Queries.GetRelatedRulings;

/// <summary>
/// Handles retrieval of rulings semantically similar to the given ruling.
/// </summary>
public class GetRelatedRulingsHandler : IRequestHandler<GetRelatedRulingsQuery, IReadOnlyList<RelatedRulingDto>>
{
    private const int MaxLimit = 20;

    private readonly ISearchService _search;
    private readonly IRulingRepository _rulingRepository;

    public GetRelatedRulingsHandler(
        ISearchService search,
        IRulingRepository rulingRepository)
    {
        _search = search;
        _rulingRepository = rulingRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RelatedRulingDto>> Handle(
        GetRelatedRulingsQuery request,
        CancellationToken cancellationToken)
    {
        var rulingExists = await _rulingRepository.GetByIdAsync(request.Id, cancellationToken);
        if (rulingExists is null || rulingExists.Status == RulingStatus.Reprocessing)
            throw new NotFoundException("Ruling not found.");

        var limit = Math.Clamp(request.Limit, 1, MaxLimit);

        var items = await _search.SearchRelatedAsync(request.Id, limit, cancellationToken);

        return RulingMapper.ToRelatedDtos(items);
    }
}
