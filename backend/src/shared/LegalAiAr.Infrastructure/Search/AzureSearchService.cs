using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Search;

/// <summary>
/// Azure AI Search implementation of hybrid (vector + keyword) search and document indexing.
/// </summary>
public class AzureSearchService : ISearchService, ISearchIndexService
{
    private const int EmbeddingDimensions = 3072;
    private readonly SearchClient? _rulingClient;
    private readonly SearchClient? _chunkClient;
    private readonly SearchClient? _statuteClient;
    private readonly AzureSearchOptions _options;
    private readonly ILogger<AzureSearchService> _logger;

    public AzureSearchService(
        IOptions<AzureSearchOptions> options,
        ILogger<AzureSearchService> logger)
    {
        _options = options.Value;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_options.Endpoint) || string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogWarning("AzureSearch Endpoint/ApiKey not configured — search operations will fail at runtime");
            return;
        }

        var credential = new AzureKeyCredential(_options.ApiKey);
        _rulingClient = new SearchClient(new Uri(_options.Endpoint), _options.RulingIndexName, credential);
        _chunkClient = new SearchClient(new Uri(_options.Endpoint), _options.ChunkIndexName, credential);
        _statuteClient = new SearchClient(new Uri(_options.Endpoint), _options.StatuteIndexName, credential);
    }

    private bool IsConfigured => _rulingClient is not null;

    private SearchClient RulingClient =>
        _rulingClient ?? throw new InvalidOperationException("AzureSearch Endpoint and ApiKey must be configured.");

    private SearchClient ChunkClient =>
        _chunkClient ?? throw new InvalidOperationException("AzureSearch Endpoint and ApiKey must be configured.");

    /// <inheritdoc />
    public async Task<PagedSearchResult> SearchAsync(
        float[]? queryEmbedding,
        string? searchText,
        SearchFilters? filters,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("SearchAsync called but AzureSearch is not configured — returning empty results");
            return new PagedSearchResult([], 0);
        }

        if (queryEmbedding != null && queryEmbedding.Length != EmbeddingDimensions)
            throw new ArgumentException($"Embedding must have {EmbeddingDimensions} dimensions.", nameof(queryEmbedding));

        var isFilterOnly = queryEmbedding == null && string.IsNullOrWhiteSpace(searchText);
        var searchOptions = isFilterOnly
            ? BuildFilterOnlySearchOptions(filters, page, pageSize)
            : BuildSearchOptions(filters, page, pageSize);

        if (queryEmbedding != null)
        {
            var vectorQuery = new VectorizedQuery(new ReadOnlyMemory<float>(queryEmbedding))
            {
                KNearestNeighborsCount = Math.Min(50, pageSize * 5),
                Fields = { "embedding" }
            };
            searchOptions.VectorSearch = new VectorSearchOptions
            {
                Queries = { vectorQuery }
            };
        }

        var searchTextValue = !string.IsNullOrWhiteSpace(searchText) ? searchText : "*";

        Response<SearchResults<RulingSearchDocument>> response;
        try
        {
            response = await RulingClient.SearchAsync<RulingSearchDocument>(
                searchTextValue, searchOptions, cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == "FeatureNotSupportedInService")
        {
            _logger.LogWarning("Semantic search not available — retrying without semantic ranker");
            StripSemanticOptions(searchOptions);
            response = await RulingClient.SearchAsync<RulingSearchDocument>(
                searchTextValue, searchOptions, cancellationToken);
        }
        var results = response.Value;

        var items = new List<SearchResultItem>();
        var totalCount = (int)(results.TotalCount ?? 0);

        await foreach (var result in results.GetResultsAsync())
        {
            var doc = result.Document;
            if (doc == null) continue;

            if (!Guid.TryParse(doc.RulingId, out var rulingId))
                continue;

            var rulingDate = doc.RulingDate.HasValue
                ? DateOnly.FromDateTime(doc.RulingDate.Value.DateTime)
                : default;

            var highlight = ExtractSemanticCaption(result.SemanticSearch)
                          ?? ExtractHighlight(result.Highlights);

            items.Add(new SearchResultItem(
                RulingId: rulingId,
                CaseTitle: doc.CaseTitle ?? string.Empty,
                Summary: doc.Summary,
                Holding: doc.Holding,
                Highlight: highlight,
                RulingDate: rulingDate,
                JurisdictionArea: doc.JurisdictionArea,
                Instance: doc.Instance,
                Court: doc.Court,
                Keywords: (doc.Keywords ?? Array.Empty<string>()).ToArray(),
                RulingDirection: doc.RulingDirection,
                Score: result.Score ?? 0,
                LegalBranch: doc.LegalBranch,
                PrecedentWeight: doc.PrecedentWeight,
                IsPlenario: doc.IsPlenario,
                IsLeadingCase: doc.IsLeadingCase));
        }

        if (_options.MinRelevanceScore > 0)
        {
            var before = items.Count;
            items.RemoveAll(item => item.Score < _options.MinRelevanceScore);
            if (items.Count < before)
            {
                _logger.LogDebug(
                    "Filtered {Removed} results below MinRelevanceScore {Threshold}",
                    before - items.Count, _options.MinRelevanceScore);
            }
        }

        var skip = (page - 1) * pageSize;
        if (items.Count < pageSize)
            totalCount = skip + items.Count;
        else if (totalCount < skip + items.Count)
            totalCount = skip + items.Count;

        return new PagedSearchResult(items, totalCount);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SearchResultItem>> SearchRelatedAsync(
        Guid rulingId,
        int topK,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("SearchRelatedAsync called but AzureSearch is not configured — returning empty results");
            return [];
        }

        var rulingIdStr = rulingId.ToString();
        var getResult = await RulingClient.GetDocumentAsync<RulingSearchDocument>(
            rulingIdStr,
            cancellationToken: cancellationToken);

        var rulingDoc = getResult.Value;
        if (rulingDoc?.Embedding == null || !rulingDoc.Embedding.Any())
        {
            _logger.LogWarning("Ruling {RulingId} not found in search index or has no embedding", rulingId);
            return Array.Empty<SearchResultItem>();
        }

        var embedding = rulingDoc.Embedding!.ToArray();
        if (embedding.Length != EmbeddingDimensions)
        {
            _logger.LogWarning("Ruling {RulingId} has invalid embedding dimensions: {Count}", rulingId, embedding.Length);
            return Array.Empty<SearchResultItem>();
        }

        var vectorQuery = new VectorizedQuery(new ReadOnlyMemory<float>(embedding))
        {
            KNearestNeighborsCount = topK + 1,
            Fields = { "embedding" }
        };

        var searchOptions = new SearchOptions
        {
            Filter = $"rulingId ne '{rulingIdStr}'",
            Size = topK,
            VectorSearch = new VectorSearchOptions { Queries = { vectorQuery } }
        };
        searchOptions.Select.Add("rulingId");
        searchOptions.Select.Add("caseTitle");
        searchOptions.Select.Add("summary");
        searchOptions.Select.Add("holding");
        searchOptions.Select.Add("rulingDate");
        searchOptions.Select.Add("jurisdictionArea");
        searchOptions.Select.Add("instance");
        searchOptions.Select.Add("court");
        searchOptions.Select.Add("keywords");
        searchOptions.Select.Add("rulingDirection");

        var response = await RulingClient.SearchAsync<RulingSearchDocument>(
            searchOptions,
            cancellationToken);
        var results = response.Value;

        var items = new List<SearchResultItem>();
        await foreach (var result in results.GetResultsAsync())
        {
            var doc = result.Document;
            if (doc == null) continue;

            if (!Guid.TryParse(doc.RulingId, out var id))
                continue;

            var rulingDate = doc.RulingDate.HasValue
                ? DateOnly.FromDateTime(doc.RulingDate.Value.DateTime)
                : default;

            items.Add(new SearchResultItem(
                RulingId: id,
                CaseTitle: doc.CaseTitle ?? string.Empty,
                Summary: doc.Summary,
                Holding: doc.Holding,
                Highlight: null,
                RulingDate: rulingDate,
                JurisdictionArea: doc.JurisdictionArea,
                Instance: doc.Instance,
                Court: doc.Court,
                Keywords: (doc.Keywords ?? Array.Empty<string>()).ToArray(),
                RulingDirection: doc.RulingDirection,
                Score: result.Score ?? 0,
                LegalBranch: doc.LegalBranch,
                PrecedentWeight: doc.PrecedentWeight,
                IsPlenario: doc.IsPlenario,
                IsLeadingCase: doc.IsLeadingCase));
        }

        return items;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChatChunkResult>> SearchChunksAsync(
        float[] queryEmbedding,
        int topK,
        string? searchText = null,
        Guid? rulingId = null,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("SearchChunksAsync called but AzureSearch is not configured — returning empty results");
            return [];
        }

        if (queryEmbedding.Length != EmbeddingDimensions)
            throw new ArgumentException($"Embedding must have {EmbeddingDimensions} dimensions.", nameof(queryEmbedding));

        var vectorQuery = new VectorizedQuery(new ReadOnlyMemory<float>(queryEmbedding))
        {
            KNearestNeighborsCount = topK,
            Fields = { "embedding" }
        };

        var searchOptions = new SearchOptions
        {
            Size = topK,
            VectorSearch = new VectorSearchOptions { Queries = { vectorQuery } },
            SemanticSearch = !string.IsNullOrWhiteSpace(searchText)
                ? new SemanticSearchOptions
                {
                    SemanticConfigurationName = "chunk-semantic",
                    QueryCaption = new QueryCaption(QueryCaptionType.Extractive)
                }
                : null
        };
        searchOptions.Select.Add("rulingId");
        searchOptions.Select.Add("chunkIndex");
        searchOptions.Select.Add("text");
        searchOptions.Select.Add("contextualizedText");

        if (rulingId.HasValue)
            searchOptions.Filter = $"rulingId eq '{rulingId.Value}'";

        var response = await ChunkClient.SearchAsync<ChunkSearchDocument>(
            searchText,
            searchOptions,
            cancellationToken);
        var results = response.Value;

        var items = new List<ChatChunkResult>();
        await foreach (var result in results.GetResultsAsync())
        {
            var doc = result.Document;
            if (doc == null) continue;

            if (!Guid.TryParse(doc.RulingId, out var parsedRulingId))
                continue;

            items.Add(new ChatChunkResult(
                RulingId: parsedRulingId,
                ChunkIndex: doc.ChunkIndex,
                Text: doc.ContextualizedText ?? doc.Text ?? string.Empty,
                Score: result.Score ?? 0));
        }

        return items;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChatRulingResult>> SearchRulingsForRagAsync(
        float[] queryEmbedding,
        int topK,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("SearchRulingsForRagAsync called but AzureSearch is not configured — returning empty results");
            return [];
        }

        if (queryEmbedding.Length != EmbeddingDimensions)
            throw new ArgumentException($"Embedding must have {EmbeddingDimensions} dimensions.", nameof(queryEmbedding));

        var vectorQuery = new VectorizedQuery(new ReadOnlyMemory<float>(queryEmbedding))
        {
            KNearestNeighborsCount = topK,
            Fields = { "embedding" }
        };

        var searchOptions = new SearchOptions
        {
            Size = topK,
            VectorSearch = new VectorSearchOptions { Queries = { vectorQuery } }
        };
        searchOptions.Select.Add("rulingId");
        searchOptions.Select.Add("caseTitle");
        searchOptions.Select.Add("summary");
        searchOptions.Select.Add("holding");
        searchOptions.Select.Add("rulingDate");
        searchOptions.Select.Add("jurisdictionArea");
        searchOptions.Select.Add("instance");
        searchOptions.Select.Add("court");

        var response = await RulingClient.SearchAsync<RulingSearchDocument>(
            searchOptions,
            cancellationToken);
        var results = response.Value;

        var items = new List<ChatRulingResult>();
        await foreach (var result in results.GetResultsAsync())
        {
            var doc = result.Document;
            if (doc == null) continue;

            if (!Guid.TryParse(doc.RulingId, out var rulingId))
                continue;

            var rulingDate = doc.RulingDate.HasValue
                ? DateOnly.FromDateTime(doc.RulingDate.Value.DateTime)
                : default;

            items.Add(new ChatRulingResult(
                RulingId: rulingId,
                CaseTitle: doc.CaseTitle ?? string.Empty,
                Summary: doc.Summary,
                Holding: doc.Holding,
                RulingDate: rulingDate,
                JurisdictionArea: doc.JurisdictionArea,
                Instance: doc.Instance,
                Court: doc.Court,
                Score: result.Score ?? 0));
        }

        return items;
    }

    /// <inheritdoc />
    public async Task<SearchFacets> GetFacetsAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("GetFacetsAsync called but AzureSearch is not configured — returning empty facets");
            return SearchFacets.Empty;
        }

        var options = new SearchOptions { Size = 0, IncludeTotalCount = false };
        var facetFields = new[] { "jurisdictionArea", "instance", "court", "courtType", "fuero", "subjectArea", "legalBranch", "precedentWeight", "resourceType" };
        foreach (var f in facetFields)
            options.Facets.Add($"{f},count:100,sort:count");

        var response = await RulingClient.SearchAsync<SearchDocument>("*", options, cancellationToken);
        var facets = response.Value.Facets;

        return new SearchFacets(
            JurisdictionAreas: ExtractFacetValues(facets, "jurisdictionArea"),
            Instances: ExtractFacetValues(facets, "instance"),
            Courts: ExtractFacetValues(facets, "court"),
            CourtTypes: ExtractFacetValues(facets, "courtType"),
            Fueros: ExtractFacetValues(facets, "fuero"),
            SubjectAreas: ExtractFacetValues(facets, "subjectArea"),
            LegalBranches: ExtractFacetValues(facets, "legalBranch"),
            PrecedentWeights: ExtractFacetValues(facets, "precedentWeight"),
            ResourceTypes: ExtractFacetValues(facets, "resourceType"));
    }

    private static IReadOnlyList<FacetValue> ExtractFacetValues(
        IDictionary<string, IList<FacetResult>>? facets, string fieldName)
    {
        if (facets == null || !facets.TryGetValue(fieldName, out var results))
            return [];

        return results
            .Where(r => r.Value != null)
            .Select(r => new FacetValue(r.Value.ToString()!, r.Count ?? 0))
            .ToList();
    }

    private static SearchOptions BuildSearchOptions(SearchFilters? filters, int page, int pageSize)
    {
        var filterParts = new List<string>();

        if (filters != null)
        {
            if (!string.IsNullOrWhiteSpace(filters.JurisdictionArea))
                filterParts.Add($"jurisdictionArea eq '{EscapeFilterValue(filters.JurisdictionArea)}'");
            if (!string.IsNullOrWhiteSpace(filters.Instance))
                filterParts.Add($"instance eq '{EscapeFilterValue(filters.Instance)}'");
            if (!string.IsNullOrWhiteSpace(filters.CourtName))
                filterParts.Add($"court eq '{EscapeFilterValue(filters.CourtName)}'");
            if (filters.DateFrom.HasValue)
                filterParts.Add($"rulingDate ge {filters.DateFrom.Value:yyyy-MM-dd}T00:00:00Z");
            if (filters.DateTo.HasValue)
                filterParts.Add($"rulingDate le {filters.DateTo.Value:yyyy-MM-dd}T23:59:59Z");
            AppendKeywordFilter(filterParts, filters.Keywords);
            if (!string.IsNullOrWhiteSpace(filters.SubjectArea))
                filterParts.Add($"subjectArea eq '{EscapeFilterValue(filters.SubjectArea)}'");
            if (!string.IsNullOrWhiteSpace(filters.ResourceType))
                filterParts.Add($"resourceType eq '{EscapeFilterValue(filters.ResourceType)}'");
            if (filters.IsUnconstitutional.HasValue)
                filterParts.Add($"isUnconstitutional eq {(filters.IsUnconstitutional.Value ? "true" : "false")}");
            if (!string.IsNullOrWhiteSpace(filters.CourtType))
                filterParts.Add($"courtType eq '{EscapeFilterValue(filters.CourtType)}'");
            if (!string.IsNullOrWhiteSpace(filters.Fuero))
                filterParts.Add($"fuero eq '{EscapeFilterValue(filters.Fuero)}'");
            if (!string.IsNullOrWhiteSpace(filters.LegalBranch))
                filterParts.Add($"legalBranch eq '{EscapeFilterValue(filters.LegalBranch)}'");
            if (!string.IsNullOrWhiteSpace(filters.PrecedentWeight))
                filterParts.Add($"precedentWeight eq '{EscapeFilterValue(filters.PrecedentWeight)}'");
            if (!string.IsNullOrWhiteSpace(filters.ActionType))
                filterParts.Add($"actionType eq '{EscapeFilterValue(filters.ActionType)}'");
            if (!string.IsNullOrWhiteSpace(filters.OfficialReference))
                filterParts.Add($"search.ismatch('{EscapeFilterValue(filters.OfficialReference)}', 'officialReference')");
            if (filters.HasDictamen.HasValue)
                filterParts.Add($"hasDictamen eq {(filters.HasDictamen.Value ? "true" : "false")}");
        }

        var filter = filterParts.Count > 0 ? string.Join(" and ", filterParts) : null;
        var skip = (page - 1) * pageSize;

        var options = new SearchOptions
        {
            Filter = filter,
            Size = pageSize,
            Skip = skip,
            IncludeTotalCount = true,
            SearchMode = SearchMode.Any,
            ScoringProfile = "relevance-boost",
            QueryType = SearchQueryType.Semantic,
            SemanticSearch = new SemanticSearchOptions
            {
                SemanticConfigurationName = "legal-semantic",
                QueryCaption = new QueryCaption(QueryCaptionType.Extractive)
                {
                    HighlightEnabled = true
                }
            },
            HighlightPreTag = "<em>",
            HighlightPostTag = "</em>"
        };
        options.HighlightFields.Add("caseTitle");
        options.HighlightFields.Add("summary");
        options.HighlightFields.Add("holding");
        options.Select.Add("rulingId");
        options.Select.Add("caseTitle");
        options.Select.Add("summary");
        options.Select.Add("holding");
        options.Select.Add("rulingDate");
        options.Select.Add("jurisdictionArea");
        options.Select.Add("instance");
        options.Select.Add("court");
        options.Select.Add("keywords");
        options.Select.Add("rulingDirection");
        return options;
    }

    private static SearchOptions BuildFilterOnlySearchOptions(SearchFilters? filters, int page, int pageSize)
    {
        var filterParts = new List<string>();

        if (filters != null)
        {
            if (!string.IsNullOrWhiteSpace(filters.JurisdictionArea))
                filterParts.Add($"jurisdictionArea eq '{EscapeFilterValue(filters.JurisdictionArea)}'");
            if (!string.IsNullOrWhiteSpace(filters.Instance))
                filterParts.Add($"instance eq '{EscapeFilterValue(filters.Instance)}'");
            if (!string.IsNullOrWhiteSpace(filters.CourtName))
                filterParts.Add($"court eq '{EscapeFilterValue(filters.CourtName)}'");
            if (filters.DateFrom.HasValue)
                filterParts.Add($"rulingDate ge {filters.DateFrom.Value:yyyy-MM-dd}T00:00:00Z");
            if (filters.DateTo.HasValue)
                filterParts.Add($"rulingDate le {filters.DateTo.Value:yyyy-MM-dd}T23:59:59Z");
            AppendKeywordFilter(filterParts, filters.Keywords);
            if (!string.IsNullOrWhiteSpace(filters.SubjectArea))
                filterParts.Add($"subjectArea eq '{EscapeFilterValue(filters.SubjectArea)}'");
            if (!string.IsNullOrWhiteSpace(filters.ResourceType))
                filterParts.Add($"resourceType eq '{EscapeFilterValue(filters.ResourceType)}'");
            if (filters.IsUnconstitutional.HasValue)
                filterParts.Add($"isUnconstitutional eq {(filters.IsUnconstitutional.Value ? "true" : "false")}");
            if (!string.IsNullOrWhiteSpace(filters.CourtType))
                filterParts.Add($"courtType eq '{EscapeFilterValue(filters.CourtType)}'");
            if (!string.IsNullOrWhiteSpace(filters.Fuero))
                filterParts.Add($"fuero eq '{EscapeFilterValue(filters.Fuero)}'");
            if (!string.IsNullOrWhiteSpace(filters.LegalBranch))
                filterParts.Add($"legalBranch eq '{EscapeFilterValue(filters.LegalBranch)}'");
            if (!string.IsNullOrWhiteSpace(filters.PrecedentWeight))
                filterParts.Add($"precedentWeight eq '{EscapeFilterValue(filters.PrecedentWeight)}'");
            if (!string.IsNullOrWhiteSpace(filters.ActionType))
                filterParts.Add($"actionType eq '{EscapeFilterValue(filters.ActionType)}'");
            if (!string.IsNullOrWhiteSpace(filters.OfficialReference))
                filterParts.Add($"search.ismatch('{EscapeFilterValue(filters.OfficialReference)}', 'officialReference')");
            if (filters.HasDictamen.HasValue)
                filterParts.Add($"hasDictamen eq {(filters.HasDictamen.Value ? "true" : "false")}");
        }

        var filter = filterParts.Count > 0 ? string.Join(" and ", filterParts) : null;
        var skip = (page - 1) * pageSize;

        var options = new SearchOptions
        {
            Filter = filter,
            Size = pageSize,
            Skip = skip,
            IncludeTotalCount = true,
            QueryType = SearchQueryType.Simple
        };
        options.OrderBy.Add("rulingDate desc");
        options.Select.Add("rulingId");
        options.Select.Add("caseTitle");
        options.Select.Add("summary");
        options.Select.Add("holding");
        options.Select.Add("rulingDate");
        options.Select.Add("jurisdictionArea");
        options.Select.Add("instance");
        options.Select.Add("court");
        options.Select.Add("keywords");
        options.Select.Add("rulingDirection");
        return options;
    }

    private static void StripSemanticOptions(SearchOptions options)
    {
        options.QueryType = SearchQueryType.Full;
        options.SemanticSearch = null;
    }

    private static string? ExtractSemanticCaption(SemanticSearchResult? semanticSearch)
    {
        var captions = semanticSearch?.Captions;
        if (captions == null || captions.Count == 0)
            return null;

        var best = captions[0];
        if (string.IsNullOrWhiteSpace(best.Text))
            return null;

        return !string.IsNullOrWhiteSpace(best.Highlights)
            ? best.Highlights
            : best.Text;
    }

    private static string? ExtractHighlight(IDictionary<string, IList<string>>? highlights)
    {
        if (highlights == null || highlights.Count == 0)
            return null;

        foreach (var field in new[] { "holding", "summary", "caseTitle" })
        {
            if (highlights.TryGetValue(field, out var fragments) && fragments.Count > 0)
                return string.Join(" … ", fragments);
        }

        return null;
    }

    private static void AppendKeywordFilter(List<string> filterParts, IReadOnlyList<string>? keywords)
    {
        if (keywords is not { Count: > 0 })
            return;

        var clauses = keywords
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(k => $"keywords/any(kw: kw eq '{EscapeFilterValue(k!.Trim().ToUpperInvariant())}')")
            .ToList();

        if (clauses.Count > 0)
            filterParts.Add($"({string.Join(" or ", clauses)})");
    }

    private static string EscapeFilterValue(string value)
    {
        return value.Replace("'", "''");
    }

    /// <inheritdoc />
    public async Task IndexRulingAsync(RulingIndexInput input, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("IndexRulingAsync called but AzureSearch is not configured — skipping");
            return;
        }

        if (input.Embedding.Length != EmbeddingDimensions)
            throw new ArgumentException($"Embedding must have {EmbeddingDimensions} dimensions.", nameof(input));

        var doc = new RulingSearchDocument
        {
            Id = input.RulingId.ToString(),
            RulingId = input.RulingId.ToString(),
            CaseTitle = input.CaseTitle,
            Summary = input.Summary,
            Holding = input.Holding,
            CaseNumber = input.CaseNumber,
            RulingDate = new DateTimeOffset(input.RulingDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero),
            JurisdictionArea = input.JurisdictionArea,
            Instance = input.Instance,
            Court = input.Court,
            CourtType = input.CourtType,
            Fuero = input.Fuero,
            InstanceLevel = input.InstanceLevel,
            Keywords = input.Keywords.ToList(),
            Persons = input.Persons.ToList(),
            Statutes = input.Statutes.ToList(),
            RulingDirection = input.RulingDirection,
            SubjectArea = input.SubjectArea,
            LegalBranch = input.LegalBranch,
            PrecedentWeight = input.PrecedentWeight,
            IsPlenario = input.IsPlenario,
            IsLeadingCase = input.IsLeadingCase,
            ResourceType = input.ResourceType,
            IsUnconstitutional = input.IsUnconstitutional,
            ActionType = input.ActionType,
            OfficialReference = input.OfficialReference,
            FederalQuestion = input.FederalQuestion,
            HasDictamen = input.HasDictamen,
            Embedding = input.Embedding
        };

        var actions = new[] { IndexDocumentsAction.MergeOrUpload(doc) };
        await RulingClient.IndexDocumentsAsync(IndexDocumentsBatch.Create(actions), cancellationToken: cancellationToken);
        _logger.LogDebug("Indexed ruling {RulingId} in rulings-by-ruling", input.RulingId);
    }

    /// <inheritdoc />
    public async Task IndexChunksAsync(IReadOnlyList<ChunkIndexInput> inputs, CancellationToken cancellationToken = default)
    {
        if (inputs.Count == 0)
            return;

        if (!IsConfigured)
        {
            _logger.LogWarning("IndexChunksAsync called but AzureSearch is not configured — skipping");
            return;
        }

        var actions = inputs.Select(input =>
        {
            if (input.Embedding.Length != EmbeddingDimensions)
                throw new ArgumentException($"Embedding must have {EmbeddingDimensions} dimensions.", nameof(inputs));

            var doc = new ChunkSearchDocument
            {
                Id = $"{input.RulingId}-{input.ChunkIndex}",
                RulingId = input.RulingId.ToString(),
                ChunkIndex = input.ChunkIndex,
                Text = input.Text,
                ContextualizedText = input.ContextualizedText,
                Embedding = input.Embedding
            };
            return IndexDocumentsAction.MergeOrUpload(doc);
        }).ToArray();

        await ChunkClient.IndexDocumentsAsync(IndexDocumentsBatch.Create(actions), cancellationToken: cancellationToken);
        _logger.LogDebug("Indexed {Count} chunks for ruling {RulingId} in rulings-by-chunk", inputs.Count, inputs[0].RulingId);
    }

    /// <inheritdoc />
    public async Task DeleteRulingAsync(Guid rulingId, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("DeleteRulingAsync called but AzureSearch is not configured — skipping");
            return;
        }

        var rulingKey = rulingId.ToString();
        await RulingClient.DeleteDocumentsAsync("id", [rulingKey], cancellationToken: cancellationToken);

        var chunkIds = new List<string>();
        var searchOptions = new SearchOptions
        {
            Filter = $"rulingId eq '{EscapeFilterValue(rulingKey)}'",
            Select = { "id" },
            Size = 500,
            IncludeTotalCount = false
        };

        while (true)
        {
            var response = await ChunkClient.SearchAsync<ChunkSearchDocument>(
                "*", searchOptions, cancellationToken);
            var batch = new List<string>();
            await foreach (var result in response.Value.GetResultsAsync())
            {
                if (!string.IsNullOrEmpty(result.Document.Id))
                    batch.Add(result.Document.Id);
            }

            if (batch.Count == 0)
                break;

            chunkIds.AddRange(batch);
            if (batch.Count < searchOptions.Size)
                break;
        }

        if (chunkIds.Count > 0)
        {
            for (var i = 0; i < chunkIds.Count; i += 100)
            {
                var slice = chunkIds.Skip(i).Take(100).ToArray();
                await ChunkClient.DeleteDocumentsAsync("id", slice, cancellationToken: cancellationToken);
            }
        }

        _logger.LogInformation(
            "Deleted ruling {RulingId} from search indexes ({ChunkCount} chunks)",
            rulingId, chunkIds.Count);
    }

    /// <inheritdoc />
    public async Task MergeOntologyFieldsAsync(
        Guid rulingId,
        string? legalBranch, string? precedentWeight, bool isPlenario, bool isLeadingCase,
        string? courtType, string? fuero, int? instanceLevel,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("MergeOntologyFieldsAsync called but AzureSearch is not configured — skipping");
            return;
        }

        var partial = new SearchDocument
        {
            ["id"] = rulingId.ToString(),
            ["legalBranch"] = legalBranch ?? "",
            ["precedentWeight"] = precedentWeight ?? "",
            ["isPlenario"] = isPlenario,
            ["isLeadingCase"] = isLeadingCase,
            ["courtType"] = courtType ?? "",
            ["fuero"] = fuero ?? "",
            ["instanceLevel"] = instanceLevel
        };

        var batch = IndexDocumentsBatch.Merge(new[] { partial });
        await RulingClient.IndexDocumentsAsync(batch, cancellationToken: cancellationToken);
        _logger.LogDebug("Merged ontology fields for ruling {RulingId}", rulingId);
    }

    private SearchClient StatuteClient =>
        _statuteClient ?? throw new InvalidOperationException("AzureSearch Endpoint and ApiKey must be configured.");

    /// <inheritdoc />
    public async Task IndexStatuteAsync(StatuteIndexInput input, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("IndexStatuteAsync called but AzureSearch is not configured — skipping");
            return;
        }

        var doc = new StatuteSearchDocument
        {
            Id = input.StatuteId.ToString(),
            StatuteId = input.StatuteId,
            Number = input.Number,
            Name = input.Name,
            NormType = input.NormType,
            NormativeLevel = input.NormativeLevel,
            LegalBranch = input.LegalBranch,
            IssuingBody = input.IssuingBody,
            SanctionDate = input.SanctionDate.HasValue
                ? new DateTimeOffset(input.SanctionDate.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero)
                : null,
            PublicationDate = input.PublicationDate.HasValue
                ? new DateTimeOffset(input.PublicationDate.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero)
                : null,
            Status = input.Status,
            IsVigente = input.IsVigente,
            FullText = input.FullText,
            SaijId = input.SaijId,
            RulingCount = input.RulingCount,
        };

        var actions = new[] { IndexDocumentsAction.MergeOrUpload(doc) };
        await StatuteClient.IndexDocumentsAsync(IndexDocumentsBatch.Create(actions), cancellationToken: cancellationToken);
        _logger.LogDebug("Indexed statute {StatuteId} ({Number}) in statutes index", input.StatuteId, input.Number);
    }
}
