using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Proceedings.DTOs;

namespace LegalAiAr.Application.Proceedings.Queries;

public record GetProceedingByIdQuery(int Id) : IRequest<ProceedingDetailDto?>;
