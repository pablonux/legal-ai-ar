using LegalAiAr.Contracts.Responses.Catalogs;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Catalogs.Queries.GetPersonById;

public class GetPersonByIdHandler : IRequestHandler<GetPersonByIdQuery, PersonDetailDto?>
{
    private readonly IPersonRepository _repo;

    public GetPersonByIdHandler(IPersonRepository repo) => _repo = repo;

    public async Task<PersonDetailDto?> Handle(
        GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var detail = await _repo.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (detail is null) return null;

        var person = detail.Person;

        var offices = person.JudicialOffices
            .OrderByDescending(o => o.IsCurrent)
            .ThenByDescending(o => o.StartDate)
            .Select(o => new PersonOfficeDto(
                o.CourtId, o.Court.Name, o.Position.ToString(),
                o.StartDate?.ToString("yyyy-MM-dd"),
                o.EndDate?.ToString("yyyy-MM-dd"),
                o.IsCurrent))
            .ToList();

        var proceedings = person.ProceedingParties
            .OrderByDescending(pp => pp.JudicialProceeding.LastRulingDate)
            .Select(pp => new PersonProceedingDto(
                pp.JudicialProceedingId,
                pp.JudicialProceeding.CaseNumber,
                pp.JudicialProceeding.DisplayName,
                pp.Role.ToString()))
            .ToList();

        return new PersonDetailDto(
            person.Id,
            person.DisplayName,
            detail.CourtName,
            detail.RulingCount,
            person.PersonType.ToString(),
            person.LegalEntityType?.ToString(),
            detail.RecentRulings.Select(r => new PersonRecentRulingDto(
                r.RulingId, r.CaseTitle, r.RulingDate, r.Instance, r.RulingRole
            )).ToList(),
            offices,
            proceedings);
    }
}
