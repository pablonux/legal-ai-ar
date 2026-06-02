using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Statutes;

namespace LegalAiAr.Application.Statutes.Queries.GetStatuteById;

public record GetStatuteByIdQuery(int Id) : IRequest<StatuteDetailDto?>;
