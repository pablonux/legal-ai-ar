using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.DTOs;
using LegalAiAr.Core.Interfaces.Repositories;

namespace LegalAiAr.Application.Proceedings.Queries;

public class GetProceedingByIdHandler : IRequestHandler<GetProceedingByIdQuery, ProceedingDetailDto?>
{
    private readonly IJudicialProceedingRepository _repo;

    public GetProceedingByIdHandler(IJudicialProceedingRepository repo) => _repo = repo;

    public async Task<ProceedingDetailDto?> Handle(GetProceedingByIdQuery request, CancellationToken cancellationToken)
    {
        var proc = await _repo.GetWithDetailsAsync(request.Id, cancellationToken);
        if (proc is null) return null;

        var rulings = proc.Rulings
            .OrderByDescending(r => r.RulingDate)
            .Select(r => new ProceedingRulingDto(
                r.Id, r.CaseTitle, r.RulingDate,
                r.Court?.Name, r.Instance))
            .ToList();

        var parties = proc.Parties
            .Select(pp => new ProceedingPartyDto(
                pp.PersonId, pp.Person.DisplayName, pp.Role.ToString()))
            .ToList();

        var reps = proc.LegalRepresentations
            .Select(lr => new LegalRepresentationDto(
                lr.LawyerPersonId, lr.LawyerPerson.DisplayName,
                lr.PartyPersonId, lr.PartyPerson.DisplayName))
            .ToList();

        return new ProceedingDetailDto(
            proc.Id, proc.CaseNumber, proc.DisplayName, proc.JurisdictionArea,
            proc.ProcessType, proc.ProcessSubtype, proc.LegalBranch, proc.Status,
            proc.Court?.Name, proc.CourtId, proc.RulingCount,
            proc.FirstRulingDate, proc.LastRulingDate,
            rulings, parties, reps);
    }
}
