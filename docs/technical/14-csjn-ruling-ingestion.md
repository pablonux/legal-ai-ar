# CSJN Ruling Ingestion

> How judicial rulings (*fallos*) from the **Corte Suprema de Justicia de la Nación (CSJN)** are
> discovered, fetched, parsed, enriched, persisted, and indexed into the Knowledge Base.
>
> This document describes the ingestion **as currently implemented** in the backend. Class names
> follow the current code and may be realigned as features evolve. Argentine legal terms and CSJN
> endpoint names are kept verbatim.

---

## 1. Context

The CSJN is `Source` **Id = 1** (`Strategy = "api-first"`). Its public portal is
`https://sjconsulta.csjn.gov.ar/sjconsulta/`, which exposes a JSON API (the *sjconsulta* service)
plus PDF documents. Every ruling maps to the court **"Corte Suprema de Justicia de la Nación"**,
instance **CSJN**.

Three crawler sources feed the same pipeline, covering complementary parts of the CSJN corpus:

| Source class | Covers | Discovery mechanism |
|--------------|--------|---------------------|
| `CsjnAcuerdoDiscoverySource` | Rulings by *acuerdo* date | `GET consultaAcuerdo.html?fecha=DD/MM/YYYY` → paginate `paginarFallos.html?jtStartIndex=N` (page size 10) |
| `CsjnSumariosDiscoverySource` | Historical *sumarios* (1863–2026), incl. rulings absent from the full-text DB | `POST consultaSumarios/buscar.html` → `GET paginarSumarios.html` (≤ 5000 results/search); each *sumario* yields `idFallo` → `AnalysisId`, `DocumentId` resolved via `getAllDocumentos` |
| `CsjnFallosDestacadosSource` | Curated *Fallos Destacados* collection (~1,811 docs) | `POST fallosDestacados/buscar.html` → paginate `paginarFallos.html` |

All three are HTTP-only (no Selenium), implement `ICrawlerSource`, and reuse the same PDF download +
blob-cache path. A browser `User-Agent` is sent; sessions are initialized per search.

---

## 2. Pipeline overview

Ingestion is a six-stage pipeline (`PipelineStage` enum, in order), each stage a hosted
`BackgroundService` worker consuming its own Azure Storage Queue and emitting to the next:

```
Discoverer → Fetcher → Parser → Enricher → Persister → Indexer
   (0)         (1)       (2)       (3)         (4)         (5)
```

Queue names derive from a single configurable prefix (`PipelineQueueNames`):
`{prefix}-{stage}` and `{prefix}-{stage}-dlq` (e.g. `pipeline-fetcher`, `pipeline-fetcher-dlq`).
Messages carry `EntityType` + `SourceId`, so **one set of queues serves all sources** (CSJN, SAIJ, …);
the correct per-source strategy is selected at each stage via a resolver
(`IStrategyResolver` → `IDiscoverStrategy` / `IFetchStrategy` / `IParseStrategy` / `IEnrichStrategy` /
`IPersistStrategy` / `IIndexStrategy`).

Per-document progress is tracked in `DocumentStageLog`; `DocumentStatus`
(`Pending`/`Processing`/`Completed`/`Failed`/`Discarded`/`Cancelled`) records the outcome at each
stage, and `RulingStatus` (`Indexed`/`Reprocessing`/`Error`/`Pending`) the final KB state. Runs are
grouped under an `IngestionJob`.

### Trigger — `CrawlerMessage`

A crawl run is triggered by a `CrawlerMessage`:

| Field | Meaning |
|-------|---------|
| `SourceId` | `1` for CSJN |
| `DocumentType` | `"ruling"` (default) |
| `Type` | `"incremental"` (since a date) or `"by-range"` (DateFrom/DateTo) |
| `Since` / `DateFrom` / `DateTo` | Date window |
| `IngestionJobId` | Reuse a pre-created job (e.g. from a split) |
| `UseCache` | Check the external-download cache before HTTP (always write-through) |
| `Reprocess` | Skip dedup checks to reprocess existing documents |
| `MaxDocuments` | Optional cap (e.g. first 10 *fallos destacados*) |

**Deduplication:** discovery skips documents already present via `ExistsByExternalId` and
`ExistsByContentHash` (a SHA-based `ContentHash`), unless `Reprocess = true`.

---

## 3. Stage detail

### 3.1 Discoverer

Resolves the crawler source(s) for `SourceId = 1` and enumerates documents in the requested window.
`CsjnAcuerdoDiscoverySource` can process several dates in parallel (`DiscoveryDateConcurrency`,
default 1; each date keeps its own session and paginates sequentially), bounded by an optional global
HTTP bulkhead (`DiscoveryHttpMaxConcurrency`, via `CsjnDiscoveryHttpGate`). Each discovered item
becomes a `DiscoveredDocument` (carrying `ExternalId`, `AnalysisId`, `DocumentId`) and is enqueued for
the Fetcher.

### 3.2 Fetcher

Downloads the ruling PDF (cached in Blob Storage) and **primes the sjconsulta JSON caches**
(`CsjnSjconsultaJsonTransport.PrimeSjconsultaCachesAsync`) so downstream stages can run cache-only.
Output: a `FetchedDocument`.

### 3.3 Parser — sjconsulta JSON (8 endpoints)

`CsjnApiParser` fetches metadata from the **8 CSJN sjconsulta endpoints** and merges them into
`CsjnApiMetadata`:

1. `fallos/abrirAnalisis.html?idAnalisis=` — opens the analysis (must return non-empty JSON)

Then seven endpoints (issued concurrently when `PostAbrirParallelEnabled`, bounded by
`CsjnApiRequestGate` / `PostAbrirHttpMaxConcurrencyGlobal`, default 6):

2. `documentos/getAllDocumentos.html?idAnalisis=` — resolves `DocumentId`
3. `sumarios/getSumariosAnalisis.html?idAnalisis=` — official doctrinal *sumarios*
4. `documentos/getCitas.html?idDocumento=` — outbound citations
5. `documentos/getCitantes.html?idDocumento=` — inbound citations (cited-by)
6. `fallos/getDictamenesAnalisis.html?idAnalisis=` — Procurador General opinions (*dictámenes*)
7. `fallos/getSintesisAnalisis.html?idAnalisis=` — syntheses
8. `fallos/getEnlacesAnalisis.html?idAnalisis=` — related links

`CsjnMetadataMapper` maps `CsjnApiMetadata` → `ExtractedMetadata`, sets the court/instance constants,
maps known judges via `CsjnMinisterDictionary`, and **computes the dynamic `missingFields`** for the
Enricher: when the API already provides structured data (e.g. votes, statutes), the corresponding LLM
call is skipped. PDF text is extracted/normalized by `PdfTextExtractor` + `PdfTextNormalizer`, and the
case caption by `CaratulaParser`.

**Caching & resilience:** all responses are written through to a blob cache under `_cache/csjn/api/`
(`CachePathHelper.ApiCacheKey`). With `PreferBlobApiCacheBeforeHttp = true` (default) cached JSON is
read before HTTP even when `UseCache = false`; the Parser is typically run with
`OutboundHttpEnabled = false` so only the Fetcher calls sjconsulta. Defensive schema validation
(R-001) throws `CsjnSchemaViolationException`; search/session faults raise `CsjnSearchErrorException`
and `CsjnSessionTimeoutException`.

### 3.4 Enricher — LLM gap-filling + chunking

`CsjnRulingEnrichStrategy` (adapter over `CsjnEnrichmentStrategy`) uses Azure OpenAI (GPT-4o) to fill
**only the missing fields**, via structured-output prompts:

| Prompt | Purpose |
|--------|---------|
| `OntologyClassificationPrompt` | Legal branch, *plenario* status, leading-case detection |
| `JudgesExtractionPrompt` | Extract judges / roles |
| `StatutesExtractionPrompt` | Extract referenced statutes and articles |
| `CitationTypePrompt` | Classify each citation: `UPHOLDS` / `OVERRULES` / `DISTINGUISHES` / `CITES` |
| `ProsecutorOpinionPrompt` | Extract the Procurador General opinion |

The full text is then split by `TextChunkingService` into **512-token chunks with 50-token overlap**
(`cl100k_base` encoding, compatible with text-embedding-3-large — ADR-002). The complete enriched
payload (`PersisterPayload`: ruling + persons + keywords + statutes + citations + chunks + sumarios +
syntheses + links + votes + prosecutor opinion) is uploaded to Blob, and the `PersisterMessage`
carries only the blob path.

### 3.5 Persister

Deserializes the blob payload and writes the entity graph to Azure SQL in a single transaction:
`Rulings`, `Courts`, persons + `RulingParticipations`, `Keywords` + `RulingKeywords`, `Statutes` +
`RulingStatutes`, `Citations`, `Sumarios`, `Syntheses`, `Links`, `Votes`, `ProsecutorOpinion`.

### 3.6 Indexer

Composed of ordered steps:

1. `UploadBlobStep` — verifies the PDF blob exists at `ruling.BlobPath` (fails to DLQ if missing).
2. `GenerateEmbeddingsStep` — ruling-level embedding (summary + holding) and per-chunk embeddings,
   **3072-dim** (`text-embedding-3-large`). Chunks are contextualized first
   (`ChunkContextualizationPrompt`, Contextual Retrieval) to improve recall.
3. `ExtractChunkMentionsStep` — string-matches each chunk against the ruling's persons, statutes, and
   keywords, persisting `ChunkEntityMention` records for GraphRAG local search.
4. `IndexSearchStep` — indexes into Azure AI Search (`rulings-by-ruling` and `rulings-by-chunk`) with
   `MergeOrUpload` for idempotency.
5. `PersistRulingStep` — persists/updates the ruling graph in SQL.
6. `ResolveCitationsStep` — retroactive citation resolution: links `Citations` with
   `TargetRulingId = null` to newly indexed rulings (inbound and outbound). Non-fatal.

---

## 4. Data model (key entities)

`Ruling` is the central entity. Selected fields:

- **Identity / provenance:** `Id` (Guid), `SourceId`, `ExternalId`, `AnalysisId`, `ContentHash`, `IngestionJobId`, `BlobPath`, `IndexedAt`, `Status`.
- **Case data:** `CaseTitle`, `CaseNumber`, `RulingDate`, `CourtId`, `Jurisdiction`, `JurisdictionArea`, `Instance`, `ResourceType`, `SubjectArea`.
- **Legal classification:** `LegalBranch`, `PrecedentWeight`, `IsPlenario`, `IsLeadingCase`, `IsUnconstitutional`, `FederalQuestion`, `RatioDecidendi`, `DoctrinaLegal`, `HasDictamen`.
- **Text:** `Summary`, `Holding`, `FullText`.

Related entities: `Source`, `Court`, `Sumario` (official doctrinal extracts — *what lawyers cite*,
from `getSumariosAnalisis`), `RulingParticipation` (judges/parties + roles), `RulingKeyword`,
`RulingStatute` (+ `RulingStatuteArticle`), `Citation` (inbound/outbound, typed), `RulingSynthesis`,
`RulingLink`, `Vote`, `ProsecutorOpinion`, and `ChunkEntityMention` (chunk → entity, for GraphRAG).

---

## 5. Configuration & throttling

| Section | Key (env: `Section__Key`) | Default | Purpose |
|---------|---------------------------|---------|---------|
| `CsjnCrawler` | `BaseUrl` | `https://sjconsulta.csjn.gov.ar/sjconsulta/` | Portal base |
| `CsjnCrawler` | `ThrottlingDelayMs` | 2000 | Min delay between discovery/PDF requests |
| `CsjnCrawler` | `ThrottlingBackoffMultiplier` / `ThrottlingMaxRetries` | 2.0 / 3 | Backoff on 429 |
| `CsjnCrawler` | `DiscoveryDateConcurrency` / `DiscoveryHttpMaxConcurrency` | 1 / 0 | Parallel acuerdo dates + global bulkhead |
| `CsjnApi` | `ThrottlingDelayMs` | 500 | Min delay between API requests |
| `CsjnApi` | `RequestTimeoutSeconds` | 30 | HTTP timeout |
| `CsjnApi` | `PreferBlobApiCacheBeforeHttp` | true | Read blob cache before HTTP |
| `CsjnApi` | `PostAbrirParallelEnabled` / `PostAbrirHttpMaxConcurrencyGlobal` | true / 6 | Parallelize the 7 post-`abrirAnalisis` GETs |
| `CsjnApi` | `OutboundHttpEnabled` | true | Set false on Parser so only the Fetcher hits sjconsulta |
| `Worker` | `BatchSize`, `MaxConcurrency`, `VisibilityTimeoutMinutes`, `MaxDequeueCount` | — | Generic queue worker tuning (per stage) |

HTTP pressure is bounded process-wide by `CsjnApiRequestGate` and `CsjnDiscoveryHttpGate` (bulkheads
+ minimum spacing). After `MaxDequeueCount` failed attempts a message moves to the stage DLQ, which an
admin UI can inspect and requeue.

---

## 6. Operational notes

- **Idempotency** is end-to-end: dedup on discovery (`ExternalId` / `ContentHash`), `MergeOrUpload` in
  AI Search, upsert semantics on persist. Re-running a crawl with `Reprocess = true` bypasses dedup.
- **Caching** lets the pipeline replay from blob (`_cache/csjn/...`) without re-hitting CSJN — useful
  for reprocessing and for debugging document loss between stages.
- **Citation graph** is built incrementally: `getCitas` / `getCitantes` populate `Citation` rows, and
  `ResolveCitationsStep` retro-links them as more rulings are indexed.
- **Cost control:** the Enricher only calls the LLM for fields the API didn't already provide
  (dynamic `missingFields`), so well-structured CSJN rulings incur minimal LLM usage.

---

## 7. Related documentation

- [04 — Document Ingestion & Processing](04-ingestion-processing.md) — the cross-source ingestion framework
- [01 — RAG & Retrieval](01-rag-retrieval.md) — how indexed rulings are searched (hybrid + GraphRAG)
- [13 — SAIJ Thesaurus Ingestion](13-saij-thesaurus-ingestion.md) — the companion thesaurus pipeline
- [Argentine Legal Ontology](../ontology/argentine-legal-ontology.md) — domain model for rulings and citations

---

*CSJN Ruling Ingestion — Legal Ai Ar*
