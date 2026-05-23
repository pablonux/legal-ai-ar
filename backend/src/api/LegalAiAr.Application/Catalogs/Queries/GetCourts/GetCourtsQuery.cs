using LegalAiAr.Application.Catalogs.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Catalogs.Queries.GetCourts;

public record GetCourtsQuery(
    string? Search,
    string? JurisdictionArea,
    string? Instance,
    int Limit = 50) : IRequest<IReadOnlyList<CourtListItemDto>>;
