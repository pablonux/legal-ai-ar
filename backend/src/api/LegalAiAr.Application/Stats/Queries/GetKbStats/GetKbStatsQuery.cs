using LegalAiAr.Application.Stats.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Stats.Queries.GetKbStats;

public record GetKbStatsQuery : IRequest<KbStatsDto>;
