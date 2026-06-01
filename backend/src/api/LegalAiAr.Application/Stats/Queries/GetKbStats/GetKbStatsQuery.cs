using LegalAiAr.Application.Mediation;
using LegalAiAr.Application.Stats.DTOs;

namespace LegalAiAr.Application.Stats.Queries.GetKbStats;

public record GetKbStatsQuery : IRequest<KbStatsDto>;
