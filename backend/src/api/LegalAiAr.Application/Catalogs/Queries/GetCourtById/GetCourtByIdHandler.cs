using LegalAiAr.Contracts.Responses.Catalogs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Catalogs.Queries.GetCourtById;

public class GetCourtByIdHandler : IRequestHandler<GetCourtByIdQuery, CourtDetailDto?>
{
    private readonly ICourtRepository _repo;

    public GetCourtByIdHandler(ICourtRepository repo) => _repo = repo;

    public async Task<CourtDetailDto?> Handle(
        GetCourtByIdQuery request, CancellationToken cancellationToken)
    {
        var detail = await _repo.GetByIdWithStatsAsync(request.Id, cancellationToken);
        if (detail is null) return null;

        var court = detail.Court;

        var parentDto = court.ParentCourt is { } parent
            ? new CourtHierarchyNodeDto(parent.Id, parent.Name, parent.Instance, parent.InstanceLevel)
            : null;

        var childDtos = court.ChildCourts
            .OrderBy(c => c.Name)
            .Select(c => new CourtHierarchyNodeDto(c.Id, c.Name, c.Instance, c.InstanceLevel))
            .ToList();

        var officeDtos = court.JudicialOffices
            .OrderByDescending(o => o.IsCurrent)
            .ThenByDescending(o => o.StartDate)
            .Select(o => new JudicialOfficeDto(
                o.PersonId,
                o.Person.DisplayName,
                o.Position.ToString(),
                o.StartDate?.ToString("yyyy-MM-dd"),
                o.EndDate?.ToString("yyyy-MM-dd"),
                o.IsCurrent))
            .ToList();

        return new CourtDetailDto(
            court.Id,
            court.Name,
            court.JurisdictionArea,
            court.Territory,
            court.Instance,
            detail.RulingCount,
            court.CourtCategory?.ToString(),
            court.Fuero?.ToString(),
            court.InstanceLevel,
            court.GovernmentLevel?.ToString(),
            parentDto,
            childDtos,
            officeDtos,
            detail.TopPersons.Select(p => new PersonListItemDto(
                p.PersonId,
                $"{p.FirstName} {p.LastName}".Trim(),
                null,
                p.RulingCount
            )).ToList());
    }
}
