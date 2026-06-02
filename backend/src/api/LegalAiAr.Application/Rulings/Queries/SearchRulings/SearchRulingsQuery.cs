using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Rulings;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Application.Rulings.Queries.SearchRulings;

/// <summary>
/// Query for hybrid semantic search over rulings.
/// </summary>
public record SearchRulingsQuery(
    string? Query,
    SearchFilters? Filters,
    int Page = 1,
    int PageSize = 10) : IRequest<SearchRulingsResult>;
