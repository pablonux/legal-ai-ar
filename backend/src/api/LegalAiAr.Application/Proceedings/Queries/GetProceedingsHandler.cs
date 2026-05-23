using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.DTOs;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Proceedings.Queries;

public class GetProceedingsHandler : IRequestHandler<GetProceedingsQuery, ProceedingPageDto>
{
    private readonly IJudicialProceedingRepository _repo;

    public GetProceedingsHandler(IJudicialProceedingRepository repo) => _repo = repo;

    public async Task<ProceedingPageDto> Handle(GetProceedingsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repo.SearchAsync(
            request.Search,
            request.ProcessType,
            request.LegalBranch,
            request.CourtId,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(p => new ProceedingListItemDto(
            p.Id, p.CaseNumber, p.DisplayName, p.JurisdictionArea,
            p.ProcessType, p.LegalBranch, p.Status,
            p.Court?.Name,
            p.RulingCount, p.FirstRulingDate, p.LastRulingDate
        )).ToList();

        return new ProceedingPageDto(dtos, totalCount, request.Page, request.PageSize);
    }
}
