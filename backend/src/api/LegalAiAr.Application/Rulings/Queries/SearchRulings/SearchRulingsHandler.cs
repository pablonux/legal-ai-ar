using LegalAiAr.Application.Common.Mappings;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Contracts.Responses.Rulings;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Application.Rulings.Queries.SearchRulings;

/// <summary>
/// Handles hybrid semantic search over rulings.
/// </summary>
public class SearchRulingsHandler : IRequestHandler<SearchRulingsQuery, SearchRulingsResult>
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;
    private readonly ISearchService _search;
    private readonly IEmbeddingService _embeddings;
    private readonly ISearchQueryPreprocessor _preprocessor;
    private readonly IThesaurusContextProvider _thesaurusContext;
    private readonly ICourtRepository _courtRepository;
    private readonly ILogger<SearchRulingsHandler> _logger;

    public SearchRulingsHandler(
        ISearchService search,
        IEmbeddingService embeddings,
        ISearchQueryPreprocessor preprocessor,
        IThesaurusContextProvider thesaurusContext,
        ICourtRepository courtRepository,
        ILogger<SearchRulingsHandler> logger)
    {
        _search = search;
        _embeddings = embeddings;
        _preprocessor = preprocessor;
        _thesaurusContext = thesaurusContext;
        _courtRepository = courtRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<SearchRulingsResult> Handle(SearchRulingsQuery request, CancellationToken cancellationToken)
    {
        var rawQuery = (request.Query ?? string.Empty).Trim();

        float[]? embedding = null;
        string? keywordInput = null;

        if (!string.IsNullOrEmpty(rawQuery))
        {
            var context = await _thesaurusContext.GetContextAsync(rawQuery, cancellationToken);
            var preprocessed = await _preprocessor.PreprocessAsync(rawQuery, context, cancellationToken);

            var embeddingInput = preprocessed?.SemanticQuery ?? rawQuery;
            keywordInput = preprocessed?.KeywordQuery ?? rawQuery;

            if (preprocessed != null)
                _logger.LogDebug("Query preprocessed — keyword: {Keyword}, semantic: {Semantic}",
                    keywordInput, embeddingInput);

            embedding = await _embeddings.GenerateAsync(embeddingInput, cancellationToken);
        }
        else
        {
            _logger.LogDebug("Filter-only search — no query text provided");
        }

        var filters = await ResolveCourtFilterAsync(request.Filters, cancellationToken);

        var page = Math.Max(DefaultPage, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var pagedResult = await _search.SearchAsync(
            embedding,
            keywordInput,
            filters,
            page,
            pageSize,
            cancellationToken);

        var results = RulingMapper.ToSearchResultDtos(pagedResult.Items);

        return new SearchRulingsResult(
            TotalCount: pagedResult.TotalCount,
            Page: page,
            PageSize: pageSize,
            Results: results);
    }

    private async Task<SearchFilters?> ResolveCourtFilterAsync(SearchFilters? filters, CancellationToken cancellationToken)
    {
        if (filters?.CourtId is not { } courtId)
            return filters;

        var court = await _courtRepository.GetByIdAsync(courtId, cancellationToken);
        if (court is null)
        {
            _logger.LogWarning("Court {CourtId} not found; court filter will be skipped", courtId);
            return filters with { CourtId = null };
        }

        return filters with { CourtName = court.Name };
    }
}
