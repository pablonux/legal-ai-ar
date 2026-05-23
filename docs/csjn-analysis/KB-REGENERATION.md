# KB Cleanup & Regeneration Procedure

## Context

After implementing F1-21a through F1-21d, the Knowledge Base must be regenerated to:

1. **Populate new fields** — `ActionType`, `InternalSubject`, `OfficialReference`, `Observations`, `FederalQuestion`, `ProceduralFormula`, `HasDictamen` on Rulings; `CsjnFalloId`, `CitationText` on Citations.
2. **Fix contextualized embeddings** — Chunks now carry `ContextualizedText` with metadata prefix for better retrieval.
3. **Rebuild search indexes** — New filterable/facetable fields, `chunk-relevance` scoring profile, hybrid semantic search.
4. **Ingest Fallos Destacados** — The curated ~1,811 rulings from CSJN's highlighted collection.

## Pre-requisites

- [ ] F1-21a migration applied: `dotnet ef database update` on target environment.
- [ ] F1-21b search index recreation: run `LegalAiAr.SetupSearch` tool to update index schemas.
- [ ] All four sub-features deployed to target environment.

## Step 1: Apply Database Migration

```bash
# From backend/src/shared/LegalAiAr.Infrastructure
dotnet ef database update --connection "ConnectionString" --startup-project ../../api/LegalAiAr.Api
```

## Step 2: Recreate Search Indexes

```bash
# From backend/src/tools/LegalAiAr.SetupSearch
dotnet run -- --recreate
```

This will:
- Drop and recreate `rulings-by-ruling` with new filterable fields.
- Drop and recreate `rulings-by-chunk` with `contextualizedText`, scoring profile, and semantic configuration.

> **Warning**: This clears all indexed data. Regeneration (Step 4) will repopulate.

## Step 3: Clear Existing KB Data (Optional — Full Reset)

If doing a full regeneration rather than incremental reprocess:

```sql
-- Truncate dependent tables first (FK order)
DELETE FROM [dbo].[RulingKeywords];
DELETE FROM [dbo].[RulingStatutes];
DELETE FROM [dbo].[RulingJudges];
DELETE FROM [dbo].[Citations];
DELETE FROM [dbo].[NormRelations];
DELETE FROM [dbo].[ThesaurusRelations];
DELETE FROM [dbo].[Rulings];

-- Reset crawler last-crawled checkpoint so incremental starts fresh
UPDATE [dbo].[CrawlerConfigs] SET LastCrawledAt = NULL WHERE SourceId = 1;
```

> If you prefer **reprocessing without deleting** existing data, skip this step and use `Reprocess = true` in Step 4.

## Step 4: Regenerate via Reprocess

### Option A: Bulk Requeue (fastest, uses existing cached data)

```http
POST /api/admin/pipeline/bulk-requeue
Content-Type: application/json

{
  "sourceId": 1,
  "useCache": true,
  "reprocess": true
}
```

This requeues all known rulings through the full pipeline (parser → enrichment → indexer), picking up new field mappings and contextualized embeddings.

### Option B: Re-crawl Incremental + Fallos Destacados

```http
# 1. Re-crawl by date range (all historical rulings)
POST /api/admin/crawlers/1/run
Content-Type: application/json

{
  "type": "by-range",
  "dateFrom": "2015-01-01",
  "dateTo": "2026-04-24",
  "useCache": true,
  "reprocess": true
}

# 2. Crawl Fallos Destacados collection
POST /api/admin/crawlers/1/run
Content-Type: application/json

{
  "type": "fallos-destacados",
  "useCache": true,
  "reprocess": false
}
```

### Option C: Fallos Destacados Only (add curated collection to existing KB)

```http
POST /api/admin/crawlers/1/run
Content-Type: application/json

{
  "type": "fallos-destacados",
  "useCache": false,
  "reprocess": false
}
```

## Step 5: Monitor Progress

```http
GET /api/admin/ingestion-jobs?status=running
```

Check `DocumentsDiscovered`, `DocumentsParsed`, `DocumentsEnriched`, `DocumentsIndexed` counters. For ~1,811 Fallos Destacados expect:
- Crawl: ~3 min (pagination + PDF downloads with cache)
- Parse: ~30 min (PDF extraction + 8 API calls per document)
- Enrichment: ~90 min (LLM calls, batched)
- Indexing: ~20 min (embeddings + search index)

## Step 6: Validate

```sql
-- Check new fields populated
SELECT TOP 10 Id, ActionType, OfficialReference, HasDictamen, FederalQuestion
FROM Rulings WHERE ActionType IS NOT NULL;

-- Check citations enriched
SELECT TOP 10 RulingId, CsjnFalloId, CitationText FROM Citations WHERE CsjnFalloId IS NOT NULL;

-- Check contextualized chunks in search
-- Use Azure Portal > AI Search > rulings-by-chunk index > Search explorer
-- Query: contextualizedText:* to verify non-null values
```

## Rollback

If regeneration produces unexpected results:
1. Restore database from pre-regeneration backup.
2. Re-run `LegalAiAr.SetupSearch --recreate` to clear search indexes.
3. Bulk-requeue from the restored database state.

## Notes

- **Cache**: Both acuerdo and fallos-destacados sources share the same PDF/API response cache in Azure Blob Storage. `UseCache = true` avoids re-downloading from CSJN.
- **Deduplication**: Documents discovered by both sources (date-based and fallos-destacados) share the same `DocumentId`/`AnalysisId`. The dedup check (`ExistsByExternalId` + `ExistsByContentHash`) prevents double-processing. Use `Reprocess = true` to force re-processing.
- **Throttling**: CSJN rate limits at ~600ms between requests. Both sources respect `CsjnCrawlerOptions.ThrottlingDelayMs`.
