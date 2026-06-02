using LegalAiAr.Contracts.Requests.Rulings;
using LegalAiAr.Application.Rulings.Queries.SearchRulings;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Api.Mapping;

public static class SearchRulingsRequestMapper
{
    public static SearchRulingsQuery ToQuery(SearchRulingsRequest request)
    {
        var filters = MapFilters(request.Filters);
        return new SearchRulingsQuery(
            request.Query,
            filters,
            request.Page,
            request.PageSize);
    }

    private static SearchFilters? MapFilters(SearchFiltersRequest? request)
    {
        if (request is null)
            return null;

        return new SearchFilters(
            JurisdictionArea: request.JurisdictionArea,
            Instance: request.Instance,
            CourtId: request.CourtId,
            CourtName: request.Court,
            DateFrom: request.DateFrom,
            DateTo: request.DateTo,
            Keywords: request.Keywords,
            SubjectArea: request.SubjectArea,
            ResourceType: request.ResourceType,
            IsUnconstitutional: request.IsUnconstitutional);
    }
}
