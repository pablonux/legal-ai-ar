using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Statutes.DTOs;

namespace LegalAiAr.Application.Statutes.Queries.GetStatuteById;

public record GetStatuteByIdQuery(int Id) : IRequest<StatuteDetailDto?>;
