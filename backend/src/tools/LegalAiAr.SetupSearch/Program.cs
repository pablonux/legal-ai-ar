using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Data.SqlClient;

const int EmbeddingDimensions = 3072; // text-embedding-3-large
const string SynonymMapName = "legal-thesaurus";

LoadEnvFile();

var migrate = args.Contains("--migrate");
var updateSynonyms = args.Contains("--synonyms") || args.Contains("--sync");

var endpoint = Environment.GetEnvironmentVariable("AzureSearch__Endpoint")
    ?? throw new InvalidOperationException("AzureSearch__Endpoint environment variable is required.");

var apiKey = Environment.GetEnvironmentVariable("AzureSearch__ApiKey")
    ?? throw new InvalidOperationException("AzureSearch__ApiKey environment variable is required.");

var rulingIndexName = Environment.GetEnvironmentVariable("AzureSearch__RulingIndexName")
    ?? "rulings-by-ruling";

var chunkIndexName = Environment.GetEnvironmentVariable("AzureSearch__ChunkIndexName")
    ?? "rulings-by-chunk";

var credential = new AzureKeyCredential(apiKey);
var indexClient = new SearchIndexClient(new Uri(endpoint), credential);

var vectorSearch = new VectorSearch
{
    Algorithms =
    {
        new HnswAlgorithmConfiguration("hnsw-cosine")
        {
            Parameters = new HnswParameters
            {
                Metric = VectorSearchAlgorithmMetric.Cosine,
                M = 4,
                EfConstruction = 400,
                EfSearch = 500
            }
        }
    },
    Profiles =
    {
        new VectorSearchProfile("vector-profile", "hnsw-cosine")
    }
};

// Create rulings-by-ruling index
var esAnalyzer = LexicalAnalyzerName.EsMicrosoft;

var scoringProfile = new ScoringProfile("relevance-boost")
{
    TextWeights = new TextWeights(new Dictionary<string, double>
    {
        ["caseTitle"] = 5.0,
        ["keywords"] = 4.0,
        ["holding"] = 3.0,
        ["summary"] = 2.5,
        ["caseNumber"] = 2.0,
        ["officialReference"] = 2.0,
        ["judges"] = 1.5,
        ["statutes"] = 1.5
    }),
    Functions =
    {
        new FreshnessScoringFunction("rulingDate", 1.5, new FreshnessScoringParameters(TimeSpan.FromDays(365 * 3)))
        {
            Interpolation = ScoringFunctionInterpolation.Logarithmic
        }
    },
    FunctionAggregation = ScoringFunctionAggregation.Sum
};

var rulingIndex = new SearchIndex(rulingIndexName)
{
    Fields =
    {
        new SimpleField("id", SearchFieldDataType.String) { IsKey = true },
        new SimpleField("rulingId", SearchFieldDataType.String) { IsFilterable = true },

        // Searchable text fields with Spanish analyzer + synonym map
        new SearchField("caseTitle", SearchFieldDataType.String)
            { IsSearchable = true, AnalyzerName = esAnalyzer, SynonymMapNames = { SynonymMapName } },
        new SearchField("summary", SearchFieldDataType.String)
            { IsSearchable = true, AnalyzerName = esAnalyzer, SynonymMapNames = { SynonymMapName } },
        new SearchField("holding", SearchFieldDataType.String)
            { IsSearchable = true, AnalyzerName = esAnalyzer, SynonymMapNames = { SynonymMapName } },
        new SearchField("caseNumber", SearchFieldDataType.String)
            { IsSearchable = true, IsFilterable = true, AnalyzerName = esAnalyzer },

        new SimpleField("rulingDate", SearchFieldDataType.DateTimeOffset)
            { IsFilterable = true, IsSortable = true },

        // Metadata: filterable/facetable only (not in free-text BM25)
        new SimpleField("jurisdictionArea", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SimpleField("instance", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SimpleField("court", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SimpleField("courtType", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SimpleField("fuero", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SimpleField("instanceLevel", SearchFieldDataType.Int32)
            { IsFilterable = true, IsSortable = true },
        new SimpleField("rulingDirection", SearchFieldDataType.String)
            { IsFilterable = true },
        new SimpleField("subjectArea", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SimpleField("legalBranch", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SimpleField("precedentWeight", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SimpleField("isPlenario", SearchFieldDataType.Boolean)
            { IsFilterable = true },
        new SimpleField("isLeadingCase", SearchFieldDataType.Boolean)
            { IsFilterable = true },
        new SimpleField("resourceType", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SimpleField("isUnconstitutional", SearchFieldDataType.Boolean)
            { IsFilterable = true },
        new SimpleField("actionType", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SearchField("officialReference", SearchFieldDataType.String)
            { IsSearchable = true, IsFilterable = true, AnalyzerName = esAnalyzer },
        new SimpleField("federalQuestion", SearchFieldDataType.String)
            { IsFilterable = true, IsFacetable = true },
        new SimpleField("hasDictamen", SearchFieldDataType.Boolean)
            { IsFilterable = true },

        // Collections: searchable + filterable with Spanish analyzer
        new SearchField("keywords", SearchFieldDataType.Collection(SearchFieldDataType.String))
            { IsSearchable = true, IsFilterable = true, IsFacetable = true, AnalyzerName = esAnalyzer,
              SynonymMapNames = { SynonymMapName } },
        new SearchField("judges", SearchFieldDataType.Collection(SearchFieldDataType.String))
            { IsSearchable = true, IsFilterable = true, AnalyzerName = esAnalyzer },
        new SearchField("persons", SearchFieldDataType.Collection(SearchFieldDataType.String))
            { IsSearchable = true, IsFilterable = true, AnalyzerName = esAnalyzer },
        new SearchField("statutes", SearchFieldDataType.Collection(SearchFieldDataType.String))
            { IsSearchable = true, IsFilterable = true, AnalyzerName = esAnalyzer },

        new SearchField("embedding", SearchFieldDataType.Collection(SearchFieldDataType.Single))
        {
            IsSearchable = true,
            VectorSearchDimensions = EmbeddingDimensions,
            VectorSearchProfileName = "vector-profile"
        }
    },
    VectorSearch = vectorSearch,
    ScoringProfiles = { scoringProfile },
    DefaultScoringProfile = "relevance-boost",
    SemanticSearch = new SemanticSearch
    {
        Configurations =
        {
            new SemanticConfiguration("legal-semantic", new SemanticPrioritizedFields
            {
                TitleField = new SemanticField("caseTitle"),
                ContentFields =
                {
                    new SemanticField("summary"),
                    new SemanticField("holding")
                },
                KeywordsFields =
                {
                    new SemanticField("keywords")
                }
            })
        }
    }
};

// Create rulings-by-chunk index
var chunkScoringProfile = new ScoringProfile("chunk-relevance")
{
    TextWeights = new TextWeights(new Dictionary<string, double>
    {
        ["contextualizedText"] = 3.0,
        ["text"] = 1.0
    })
};

var chunkIndex = new SearchIndex(chunkIndexName)
{
    Fields =
    {
        new SimpleField("id", SearchFieldDataType.String) { IsKey = true },
        new SimpleField("rulingId", SearchFieldDataType.String) { IsFilterable = true },
        new SimpleField("chunkIndex", SearchFieldDataType.Int32) { IsFilterable = true },
        new SearchField("text", SearchFieldDataType.String)
            { IsSearchable = true, AnalyzerName = esAnalyzer },
        new SearchField("contextualizedText", SearchFieldDataType.String)
            { IsSearchable = true, AnalyzerName = esAnalyzer, SynonymMapNames = { SynonymMapName } },
        new SearchField("embedding", SearchFieldDataType.Collection(SearchFieldDataType.Single))
        {
            IsSearchable = true,
            VectorSearchDimensions = EmbeddingDimensions,
            VectorSearchProfileName = "vector-profile"
        }
    },
    VectorSearch = vectorSearch,
    ScoringProfiles = { chunkScoringProfile },
    DefaultScoringProfile = "chunk-relevance",
    SemanticSearch = new SemanticSearch
    {
        Configurations =
        {
            new SemanticConfiguration("chunk-semantic", new SemanticPrioritizedFields
            {
                ContentFields =
                {
                    new SemanticField("contextualizedText"),
                    new SemanticField("text")
                }
            })
        }
    }
};

// ── Synonym map from thesaurus ──────────────────────────────
if (updateSynonyms || !migrate)
{
    var solrRules = await BuildSynonymRulesFromDbAsync();
    if (!string.IsNullOrWhiteSpace(solrRules))
    {
        var ruleCount = solrRules.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
        Console.WriteLine($"Uploading synonym map '{SynonymMapName}' with {ruleCount} rules...");
        var synonymMap = new SynonymMap(SynonymMapName, solrRules);
        await indexClient.CreateOrUpdateSynonymMapAsync(synonymMap);
        Console.WriteLine($"Synonym map '{SynonymMapName}' uploaded.");
    }
    else
    {
        Console.WriteLine("No synonym rules found in thesaurus DB. Skipping synonym map.");
    }

    if (updateSynonyms && !migrate)
    {
        // Just sync synonyms + index schema, no migrate
        await indexClient.CreateOrUpdateIndexAsync(rulingIndex);
        Console.WriteLine($"Index '{rulingIndexName}' updated with synonym map references.");
        Console.WriteLine("Synonym sync complete.");
        return 0;
    }
}

if (migrate)
{
    Console.WriteLine("=== MIGRATE MODE: reading all docs from old indexes ===");

    var rulingDocs = await ReadAllDocsAsync(credential, endpoint, rulingIndexName);
    Console.WriteLine($"Read {rulingDocs.Count} ruling docs.");

    var chunkDocs = await ReadAllDocsAsync(credential, endpoint, chunkIndexName);
    Console.WriteLine($"Read {chunkDocs.Count} chunk docs.");

    Console.WriteLine("Deleting old indexes...");
    if (await IndexExistsAsync(indexClient, rulingIndexName))
        await indexClient.DeleteIndexAsync(rulingIndexName);
    if (await IndexExistsAsync(indexClient, chunkIndexName))
        await indexClient.DeleteIndexAsync(chunkIndexName);
    Console.WriteLine("Old indexes deleted.");

    Console.WriteLine("Creating indexes with new schema...");
    await indexClient.CreateOrUpdateIndexAsync(rulingIndex);
    Console.WriteLine($"Index '{rulingIndexName}' created.");
    await indexClient.CreateOrUpdateIndexAsync(chunkIndex);
    Console.WriteLine($"Index '{chunkIndexName}' created.");

    await UploadDocsAsync(credential, endpoint, rulingIndexName, rulingDocs);
    await UploadDocsAsync(credential, endpoint, chunkIndexName, chunkDocs);

    Console.WriteLine("Migration complete.");
}
else
{
    var rulingExisted = await IndexExistsAsync(indexClient, rulingIndexName);
    await indexClient.CreateOrUpdateIndexAsync(rulingIndex);
    Console.WriteLine(rulingExisted
        ? $"Index '{rulingIndexName}' updated (schema synced)."
        : $"Index '{rulingIndexName}' created successfully.");

    var chunkExisted = await IndexExistsAsync(indexClient, chunkIndexName);
    await indexClient.CreateOrUpdateIndexAsync(chunkIndex);
    Console.WriteLine(chunkExisted
        ? $"Index '{chunkIndexName}' updated (schema synced)."
        : $"Index '{chunkIndexName}' created successfully.");

    Console.WriteLine("Setup complete.");
}
return 0;

// ── Helpers ─────────────────────────────────────────────────

static async Task<bool> IndexExistsAsync(SearchIndexClient client, string indexName)
{
    try
    {
        await client.GetIndexAsync(indexName);
        return true;
    }
    catch (RequestFailedException ex) when (ex.Status == 404)
    {
        return false;
    }
}

static async Task<List<SearchDocument>> ReadAllDocsAsync(
    AzureKeyCredential credential, string endpoint, string indexName)
{
    var client = new SearchClient(new Uri(endpoint), indexName, credential);
    var docs = new List<SearchDocument>();
    const int pageSize = 1000;
    var skip = 0;

    while (true)
    {
        var options = new SearchOptions
        {
            Size = pageSize,
            Skip = skip,
            IncludeTotalCount = true
        };
        options.Select.Add("*");

        var response = await client.SearchAsync<SearchDocument>("*", options);
        var page = new List<SearchDocument>();
        await foreach (var result in response.Value.GetResultsAsync())
        {
            if (result.Document != null)
                page.Add(result.Document);
        }

        docs.AddRange(page);
        Console.Write($"\r  Reading {indexName}: {docs.Count} docs...");

        if (page.Count < pageSize)
            break;

        skip += pageSize;
    }
    Console.WriteLine();
    return docs;
}

static async Task UploadDocsAsync(
    AzureKeyCredential credential, string endpoint, string indexName, List<SearchDocument> docs)
{
    if (docs.Count == 0) return;

    var client = new SearchClient(new Uri(endpoint), indexName, credential);
    const int batchSize = 500;

    for (var i = 0; i < docs.Count; i += batchSize)
    {
        var batch = docs.Skip(i).Take(batchSize).ToList();
        var actions = batch.Select(IndexDocumentsAction.Upload).ToArray();
        var indexBatch = IndexDocumentsBatch.Create(actions);
        var response = await client.IndexDocumentsAsync(indexBatch);
        var ok = response.Value.Results.Count(r => r.Succeeded);
        var fail = response.Value.Results.Count(r => !r.Succeeded);
        Console.WriteLine($"  [{Math.Min(i + batchSize, docs.Count)}/{docs.Count}] uploaded {ok}, failed {fail}");
        if (fail > 0)
        {
            foreach (var r in response.Value.Results.Where(r => !r.Succeeded))
                Console.WriteLine($"    FAIL {r.Key}: {r.ErrorMessage}");
        }
    }
}

static async Task<string> BuildSynonymRulesFromDbAsync()
{
    LoadEnvFile();
    var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
        ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

    if (string.IsNullOrWhiteSpace(sqlConn))
    {
        Console.WriteLine("WARNING: AzureSql__ConnectionString not set — skipping synonym generation.");
        return string.Empty;
    }

    const string query = """
        SELECT src.Label AS Preferred, tgt.Label AS NonPreferred
        FROM ThesaurusRelations r
        JOIN ThesaurusTerms src ON src.Id = r.SourceTermId
        JOIN ThesaurusTerms tgt ON tgt.Id = r.TargetTermId
        WHERE r.RelationType = 'UF'
        ORDER BY src.Label
        """;

    var pairs = new List<(string Preferred, string NonPreferred)>();

    await using var conn = new SqlConnection(sqlConn);
    await conn.OpenAsync();
    await using var cmd = new SqlCommand(query, conn);
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        pairs.Add((reader.GetString(0), reader.GetString(1)));
    }

    if (pairs.Count == 0) return string.Empty;

    var groups = pairs
        .GroupBy(p => Normalize(p.Preferred), StringComparer.OrdinalIgnoreCase)
        .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

    var lines = new List<string>();
    foreach (var group in groups)
    {
        var preferred = group.Key;
        var synonyms = group
            .Select(p => Normalize(p.NonPreferred))
            .Where(s => !string.IsNullOrWhiteSpace(s) &&
                        !s.Equals(preferred, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (synonyms.Count == 0) continue;
        lines.Add($"{preferred}, {string.Join(", ", synonyms)}");
    }

    Console.WriteLine($"  Built {lines.Count} synonym rules from {pairs.Count} UF pairs in DB.");
    return string.Join('\n', lines);

    static string Normalize(string term) =>
        term.Trim().Replace(",", " ").Replace("\n", " ").Replace("\r", "");
}

static void LoadEnvFile()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir != null)
    {
        var envFile = Path.Combine(dir.FullName, ".env");
        if (File.Exists(envFile))
        {
            foreach (var line in File.ReadAllLines(envFile))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#')) continue;
                var eqIndex = trimmed.IndexOf('=');
                if (eqIndex <= 0) continue;
                var key = trimmed[..eqIndex].Trim();
                var value = trimmed[(eqIndex + 1)..].Trim().Trim('"');
                Environment.SetEnvironmentVariable(key, value);
            }
            return;
        }
        dir = dir.Parent;
    }
}
