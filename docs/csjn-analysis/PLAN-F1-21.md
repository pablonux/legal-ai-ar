# F1-21 · CSJN Ontology with Real Data — Implementation Plan

> **Branch**: `feature/f1-21-ontology-real-data`
> **Status**: In progress
> **Date**: 2026-04-24
> **Deliverable range**: E250–E299 (50 deliverables)
> **Prerequisite**: F1-18 DEV complete

---

## Objective

Fix critical parser bugs, expand the KB model from 5 to 8 CSJN API endpoints, add structured API data to replace LLM calls, integrate Contextual Retrieval for dramatically better chunk search, introduce a Fallos Destacados discovery source, implement GraphRAG with Azure SQL (recursive CTEs, multi-hop traversal, graph neighborhood queries, graph-aware chat tools — no Neo4j needed), add temporal edges for future Graph Deep Learning, propagate new fields through the full pipeline, update search index, and regenerate KB with correct data — all in a single re-processing pass.

---

## Context

### Data quality analysis

Analysis of 1,811 samples across 8 CSJN API endpoints revealed:

- **4 critical bugs** in `CsjnApiParser`: CaseNumber always NULL, Summary always null, Holding = "1" (numeric flag), CitedBy always empty
- **1 mitigated bug**: RulingDate fallback uses receipt date instead of ruling date
- **3 unused endpoints** with valuable data: `getDictamenesAnalisis` (61% populated), `getSintesisAnalisis` (78%), `getEnlacesAnalisis` (0.5%)
- **High-value unextracted fields**: votosAnalisisDocumental (structured judges), referenciasNormativas (statutes), tipoAccion, tomo/pagina, observaciones, cuestionesFederales, formulas
- **LLM optimization opportunity**: skip judges/statutes LLM calls when API provides structured data (~60% of cases)

Full analysis: `docs/csjn-analysis/README.md` and per-endpoint `.md` files.

### Retrieval quality analysis

Based on [Anthropic's Contextual Retrieval study](https://www.anthropic.com/engineering/contextual-retrieval), analysis of the current pipeline identified three gaps:

1. **Chunks are context-blind**: 512-token raw text windows with zero metadata prepended before embedding. A chunk saying "La Corte dispone..." doesn't identify which case, court, or legal topic.
2. **Chunk index is vector-only**: No BM25/hybrid search on chunks (ruling index uses hybrid, but chunk index only uses vector). Exact matches for statute numbers ("Ley 26.994 art. 1723") or case references ("Fallos: 330:3725") are invisible to chunk retrieval.
3. **Chunk retrieval unused by chat**: `SearchChunksAsync` exists but has zero callers. Chat relies on ruling-level summaries only, never seeing actual legal reasoning text passages.

**Approach**: Deterministic metadata-prefix Contextual Retrieval (zero LLM cost) leveraging our structured ruling data, combined with hybrid chunk search and a new `search_chunks` tool. Full LLM-generated context deferred until multi-entity corpus (laws, decrees) where structured metadata is insufficient.

---

## Task Groups

### TG-A · Parser + DTO Fixes (DONE)

Already implemented in current branch. Fixes all critical bugs and expands DTOs through the full pipeline.

### TG-B · Indexer + Search + DB Pipeline Completion

Complete the data flow from enrichment output to persisted KB and searchable index.

### TG-C · Contextual Retrieval — Chunk Quality + Hybrid Search + RAG Tool

Metadata-prepended chunks, hybrid search on chunk index, and `search_chunks` chat tool.

### TG-D · Search & AI Quality Improvements

Leverage new fields to improve search faceting, embedding quality, and chat tool responses.

### TG-E · Fallos Destacados Discovery Source

New crawler source for curated CSJN fallos destacados collection (~1,811 documents).

### TG-F · KB Cleanup & Regeneration

Clean existing incorrect data and re-process with corrected pipeline. Must be LAST so everything is processed correctly in a single pass.

### TG-G · GraphRAG with Azure SQL — Temporal Edges + Graph Queries

Add temporal edges to all relationship tables, implement real graph queries in `SqlGraphService` (recursive CTEs, multi-hop traversal, graph neighborhood), and add graph-aware chat tools. Uses existing Azure SQL infrastructure — **Neo4j no longer required for Phase 3 graph capabilities**.

---

## T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E250 | `docs/csjn-analysis/README.md` — comprehensive 8-endpoint analysis with population statistics, field audit, bug inventory | `[x]` | `[ ]` |
| E251 | `docs/csjn-analysis/{endpoint}.md` × 8 — per-endpoint field inventory, parser status, KB value | `[x]` | `[ ]` |
| E252 | `docs/csjn-analysis/PLAN-F1-21.md` — this plan: full task inventory, deliverable tracking | `[x]` | `[ ]` |
| E253 | `docs/design/f1-21-data-flow.mermaid` — data flow: 8 endpoints → CsjnApiParser → CsjnApiMetadata → Mapper → ExtractedMetadata → Enrichment → RulingData → PersistRulingStep → Ruling + Search | `[ ]` | `[ ]` |

---

## TG-A · Parser + DTO Fixes

### T-01 · Expand DTOs across pipeline (DONE)

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E254 | `CsjnApiMetadata` expanded: 9 new fields + `CsjnVoteDto` + `CsjnApiStatuteDto` + `CsjnCitationDto` with FalloId/CitationText | `[x]` | `[ ]` |
| E255 | `ExtractedMetadata` expanded: 7 scalar fields + `ExtractedJudgeDto` + `ExtractedStatuteDto` | `[x]` | `[ ]` |
| E256 | `RulingData` expanded: 7 new fields. `CitationData` with CsjnFalloId/CitationText | `[x]` | `[ ]` |
| E257 | `Ruling` entity expanded: ActionType, InternalSubject, OfficialReference, Observations, FederalQuestion, ProceduralFormula, HasDictamen | `[x]` | `[ ]` |

### T-02 · Fix CsjnApiParser — all bugs + new extraction (DONE)

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E258 | **CaseNumber fix**: reads `identificacionExpediente` + `recursoExpediente.claveRecurso` | `[x]` | `[ ]` |
| E259 | **Summary fix**: `falloDestacado` parsed as object → `resumen` > `cabecilla` (strip HTML) > `titulo` | `[x]` | `[ ]` |
| E260 | **Holding fix**: reads `texto` from getSumariosAnalisis (doctrinal headnote), ignores numeric `holding` flag | `[x]` | `[ ]` |
| E261 | **CitedBy fix**: HTML regex parser for `getCitantes` response (was returning empty for 61% of fallos) | `[x]` | `[ ]` |
| E262 | **RulingDate fix**: fallback chain `rulingDateHint` > `falloDestacado.fecha` > `fechaFallo` (sumarios) > `reciboEntrada` | `[x]` | `[ ]` |
| E263 | **New endpoint**: `getDictamenesAnalisis` — detects prosecutor opinion presence | `[x]` | `[ ]` |
| E264 | **New fields**: votos, referenciasNormativas, tipoAccion, materia, observaciones, cuestionesFederales, formulas, tomo/pagina | `[x]` | `[ ]` |
| E265 | **Enhanced getCitas**: extracts `idFallo` and `textoCita` for direct linking | `[x]` | `[ ]` |

### T-03 · Mapper + Enrichment optimization (DONE)

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E266 | `CsjnMetadataMapper`: vote-to-judge conversion (MAYORIA→MAJORITY, DISIDENCIA→DISSENT, etc.), statute conversion, all new fields mapped | `[x]` | `[ ]` |
| E267 | **Dynamic missingFields**: skips `"judges"` when API votes available, skips `"cited_statutes"` when API normas available, adds `"prosecutor_opinion"` only when `HasDictamen == true` | `[x]` | `[ ]` |
| E268 | `CsjnEnrichmentStrategy`: uses API judges/statutes as primary, LLM as fallback; conditional prosecutor opinion extraction; propagates all 7 new fields to RulingData | `[x]` | `[ ]` |

---

## TG-B · Indexer + Search + DB Pipeline Completion

### T-04 · PersistRulingStep + Citation entity

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E269 | `Citation` entity expanded with `CsjnFalloId` (int?), `CitationText` (nvarchar?), and `CreatedAt` (datetime2, default GETUTCDATE) | `[ ]` | `[ ]` |
| E270 | `PersistRulingStep`: maps 7 new Ruling fields + 2 new Citation fields from IndexerMessage | `[ ]` | `[ ]` |

### T-05 · EF Core migration

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E271 | Migration `ExpandRulingAndCitation`: 7 nullable columns on Rulings + 3 columns on Citations (CsjnFalloId, CitationText, CreatedAt) + `CreatedAt` (datetime2, NOT NULL, DEFAULT GETUTCDATE) on RulingJudges, RulingKeywords, RulingStatutes, NormRelations, ThesaurusRelations + `EffectiveDate` (date, NULL) on NormRelations | `[ ]` | `[ ]` |

### T-06 · Search index expansion

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E272 | `RulingIndexInput`: add `ActionType`, `OfficialReference`, `HasDictamen`, `FederalQuestion` | `[ ]` | `[ ]` |
| E273 | `RulingSearchDocument`: add matching properties with `[JsonPropertyName]` | `[ ]` | `[ ]` |
| E274 | `IndexSearchStep` + `AzureSearchService.IndexRulingAsync`: map new fields | `[ ]` | `[ ]` |
| E275 | SetupSearch index schema: add new fields as filterable/facetable | `[ ]` | `[ ]` |

### T-07 · Requeue handlers

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E276 | `RequeueDocumentHandler`: include 7 new fields + CitationData expansion when rebuilding from stored Ruling | `[ ]` | `[ ]` |
| E277 | `BulkRequeueHandler`: same expansion | `[ ]` | `[ ]` |

---

## TG-C · Contextual Retrieval — Chunk Quality + Hybrid Search + RAG Tool

### T-08 · Metadata-Prepended Chunks (Deterministic Contextual Retrieval)

Based on [Anthropic's Contextual Retrieval](https://www.anthropic.com/engineering/contextual-retrieval): prepend ruling metadata to each chunk before embedding AND before BM25 indexing. Zero LLM cost — uses structured metadata already available from 8 API endpoints.

**Before**: `"La Corte dispone hacer lugar a la demanda interpuesta por la Provincia de Entre Ríos y, en consecuencia, declarar la inconstitucionalidad del decreto 2635/2015..."`

**After**: `"[CSJN | 2023-06-15 | Provincia de Entre Ríos c/ Estado Nacional | Derecho Constitucional | Fallos: 346:456 | Exp: CSJ 001234/2019] La Corte dispone hacer lugar a la demanda interpuesta por la Provincia de Entre Ríos y, en consecuencia, declarar la inconstitucionalidad del decreto 2635/2015..."`

Multi-entity future: prefix naturally disambiguates document types when adding laws/decrees:
- Ruling: `[CSJN | 2023-06-15 | Caso X c/ Y | Fallos: 346:456]`
- Law: `[Ley 26.994 | Código Civil y Comercial | Art. 1723 | Vigente desde 2015-08-01]`
- Decree: `[Decreto 2635/2015 | PEN | Distribución fiscal]`

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E278 | `GenerateEmbeddingsStep`: `BuildContextualizedChunkText(RulingData, chunkText)` prepends `[Court \| Date \| CaseTitle \| SubjectArea \| OfficialReference \| CaseNumber]` to chunk text before embedding. Store contextualized text in `ChunkData` for BM25 indexing | `[ ]` | `[ ]` |
| E279 | `ChunkIndexInput` / `ChunkSearchDocument`: add `contextualizedText` field. `IndexSearchStep` passes contextualized text. `AzureSearchService.IndexChunksAsync` stores it | `[ ]` | `[ ]` |

### T-09 · Hybrid Search on Chunk Index

Currently chunk index is vector-only. Enable BM25 + semantic for exact match recovery (statute numbers, case references).

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E280 | SetupSearch: add `SemanticConfiguration` to `rulings-by-chunk` index, apply `legal-thesaurus` synonym map to `contextualizedText` field, add scoring profile | `[ ]` | `[ ]` |
| E281 | `SearchChunksAsync`: accept optional `searchText`, do hybrid search (vector + text) instead of vector-only. Apply semantic ranking when available | `[ ]` | `[ ]` |

### T-10 · `search_chunks` Chat Tool

New tool so the model can search for specific text passages (complements `search_rulings` for summaries).

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E282 | `SearchChunksTool`: tool name `search_chunks`, params `{ query, rulingId?, topK? }`. Calls `SearchChunksAsync` (hybrid), returns chunk text with ruling metadata prefix. Register in `ToolRegistration` | `[ ]` | `[ ]` |
| E283 | System prompt update: instruct model to use `search_chunks` when needing specific legal reasoning passages, exact quotes, or argument details from rulings | `[ ]` | `[ ]` |

---

## TG-D · Search & AI Quality Improvements

### T-11 · Search quality with new data

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E284 | Embedding quality: With real Summary + Holding (was null/"1"), ruling-level embeddings will be dramatically better. No code change needed — automatic on re-index | `[x]` | `[ ]` |

### T-12 · Chat tools with new fields

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E285 | `get_ruling_detail` tool: expose ActionType, OfficialReference, Observations, FederalQuestion, ProceduralFormula, HasDictamen in tool output. `search_rulings` tool: add OfficialReference, ActionType as optional filters. System prompt: instruct model to cite OfficialReference (e.g. "Fallos: 340:1256") when available | `[ ]` | `[ ]` |

---

## TG-E · Fallos Destacados Discovery Source

### T-13 · CsjnFallosDestacadosSource

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E286 | `CsjnFallosDestacadosSource : ICrawlerSource` — POST buscar.html → session → paginate all fallos destacados → yield DiscoveredDocument batches. Reuses same `GetContentHashAsync` as acuerdo source (verDocumentoById.html) | `[ ]` | `[ ]` |
| E287 | `ICrawlerSourceResolver` expanded: `GetSource(CrawlerMessage)` overload → when `Type == "fallos-destacados"` + `SourceId == 1` returns FallosDestacadosSource, else AcuerdoSource | `[ ]` | `[ ]` |
| E288 | DI registration in `CrawlerServiceExtensions` + HttpClient named registration | `[ ]` | `[ ]` |

### T-14 · Admin API — trigger fallos destacados crawl

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E289 | `RunCrawlerHandler` supports `Type = "fallos-destacados"` for SourceId=1. Frontend admin trigger with type selector | `[ ]` | `[ ]` |

---

## TG-G · GraphRAG with Azure SQL

Strategic decision: **use Azure SQL graph capabilities (recursive CTEs, multi-hop JOINs) instead of Neo4j**. The existing relational schema (Citations, RulingJudges, RulingKeywords, RulingStatutes, NormRelations) IS the knowledge graph — no additional database needed.

Current `SqlGraphService` only does 1-hop `Include().Where()` on Citations. This task group expands it with real graph queries and adds GraphRAG-pattern tools to the chat.

### T-16 · Temporal edges on relationship entities

Add `CreatedAt` (DateTime, default `DateTime.UtcNow`) to all 6 relationship entities and `EffectiveDate` (DateOnly?) to `NormRelation`. Leverages the existing E271 migration — near-zero incremental cost. Enables temporal graph queries and future Temporal GNNs.

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E291 | 6 entity files updated: `Citation`, `RulingJudge`, `RulingKeyword`, `RulingStatute`, `NormRelation`, `ThesaurusRelation` — add `CreatedAt` property (`DateTime`, default `DateTime.UtcNow`). `NormRelation` also gets `EffectiveDate` (`DateOnly?`) for legal-effective dating of norm changes. DB columns added via E271 migration | `[ ]` | `[ ]` |

### T-17 · SqlGraphService — real graph queries

Expand `SqlGraphService` with recursive CTEs and multi-entity JOINs via `FromSqlRaw`. Currently all upsert methods are no-ops and queries are 1-hop only. New capabilities unlock GraphRAG patterns for chat.

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E292 | `GetCitationChainAsync(rulingId, maxDepth)`: recursive CTE on Citations table. Returns N-hop citation chain (default 3 hops). Includes `CitationType` and `RulingDate` on each edge for temporal analysis. Used by `explore_citation_network` tool | `[ ]` | `[ ]` |
| E293 | `GetGraphNeighborhoodAsync(rulingId, depth)`: 1-2 hop neighborhood with ALL entity types. Returns the ruling's judges (via RulingJudges), statutes (via RulingStatutes), keywords (via RulingKeywords), court, prosecutor opinion, PLUS the same for cited/citing rulings. This is the "local context" for GraphRAG | `[ ]` | `[ ]` |
| E294 | `GetSharedEntitiesAsync(rulingIds[])`: given a set of ruling IDs (e.g. from search results), find common judges, statutes, keywords across them. Returns intersection counts and entity details. Enables "these 5 rulings share judge X, statute Y, and keyword Z" | `[ ]` | `[ ]` |
| E295 | `GetCitationPathAsync(sourceRulingId, targetRulingId, maxDepth)`: find shortest citation path between two rulings via recursive CTE with path tracking. Returns the chain of intermediate rulings. Answers "how is ruling A connected to ruling B?" | `[ ]` | `[ ]` |
| E296 | `GetJudgeRulingNetworkAsync(judgeId, topN)`: rulings signed by a judge, grouped by `LegalBranch`, `ParticipationType`, and `CitationType` of their outbound citations. Provides the data for judge profile / voting pattern analysis | `[ ]` | `[ ]` |

### T-18 · GraphRAG chat tools

New chat tools that leverage graph queries to enrich RAG context. These complement `search_rulings` (text) and `search_chunks` (passages) with structural graph intelligence.

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E297 | `ExploreGraphTool`: tool name `explore_graph`, params `{ rulingId, depth?, includeEntities? }`. Calls `GetGraphNeighborhoodAsync`. Returns structured graph context: citation chain + judges + statutes + keywords + court. System prompt instructs model to use when needing to understand a ruling's legal context, precedent relationships, or connected entities | `[ ]` | `[ ]` |
| E298 | `FindConnectionTool`: tool name `find_connection`, params `{ sourceRulingId, targetRulingId }`. Calls `GetCitationPathAsync`. Returns the shortest citation path between two rulings with intermediate case details. Enables "how are these two cases related?" queries | `[ ]` | `[ ]` |
| E299 | Update `GetRulingCitationsTool` to use `GetCitationChainAsync` with configurable depth (default 1 for backward compat, model can request up to 3 hops). Richer output includes `CitationType`, `RulingDate`, `LegalBranch` per hop | `[ ]` | `[ ]` |

---

## TG-F · KB Cleanup & Regeneration

### T-15 · Clean and regenerate

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E290 | **Strategy**: Truncate Rulings + related tables (Citations, RulingJudges, RulingKeywords, RulingStatutes, ProsecutorOpinions). Clear Azure AI Search indexes (ruling + chunk). Run Fallos Destacados crawl (`UseCache=true` to avoid re-downloading from CSJN API). All 1,811+ fallos re-processed through full pipeline with: corrected parser, new fields, contextual chunks, hybrid chunk search | `[ ]` | `[ ]` |

---

## Implementation Order — Sub-features

All changes must be in place BEFORE regeneration so every document is processed correctly in a single pass. Split into 4 sub-features, each independently committable.

### F1-21a · Pipeline Completion + DB Migration (11 deliverables)

Completes the data flow from enrichment output to persisted KB and searchable index. Includes temporal edges in the same migration.

```
T-16    Temporal edges on 6 relationship entities (E291)
T-04    PersistRulingStep + Citation entity (E269, E270)
T-05    EF Core migration (E271) — includes temporal columns
T-06    Search index expansion (E272, E273, E274, E275)
T-07    Requeue handlers (E276, E277)
```

**Depends on**: TG-A (done)
**Result**: Parser fixes flow end-to-end into DB + search index. Can deploy and test with existing crawler.

### F1-21b · Contextual Retrieval + Search Quality (7 deliverables)

Metadata-prepended chunks, hybrid chunk search, `search_chunks` tool, and enhanced existing tools.

```
T-08    Metadata-prepended chunks (E278, E279)
T-09    Hybrid search on chunk index (E280, E281)
T-10    search_chunks chat tool (E282, E283)
T-12    Chat tools with new fields (E285)
```

**Depends on**: F1-21a (search index must have new fields)
**Result**: Chunk retrieval goes from unusable to production-grade. Chat has passage-level search.

### F1-21c · GraphRAG with Azure SQL (8 deliverables)

Real graph queries in `SqlGraphService` and graph-aware chat tools.

```
T-17    SqlGraphService real graph queries (E292, E293, E294, E295, E296)
T-18    GraphRAG chat tools (E297, E298, E299)
```

**Depends on**: F1-21a (temporal edges + data in DB)
**Result**: Chat can traverse citation chains, explore graph neighborhoods, find connections between rulings.

### F1-21d · Fallos Destacados + Regeneration (5 deliverables)

New crawler source and KB regeneration with all changes in place.

```
T-13    CsjnFallosDestacadosSource (E286, E287, E288)
T-14    Admin API trigger (E289)
T-15    KB Cleanup & Regeneration (E290)
```

**Depends on**: F1-21a + F1-21b + F1-21c (all pipeline changes must be ready)
**Pre-requisite for E290**: DB backup + search index snapshot before truncation.
**Result**: Clean KB with correct data, all 1,811+ fallos processed through the complete pipeline.

### Dependency graph

```
TG-A (DONE) ──► F1-21a ──┬──► F1-21b ──┐
                          │              │
                          └──► F1-21c ──┤
                                        │
                                        ▼
                                     F1-21d
```

Note: F1-21b and F1-21c can be developed in parallel after F1-21a.

---

## Impact Summary

| Area | Before | After |
|---|---|---|
| CaseNumber | NULL (100%) | Populated from `identificacionExpediente` |
| Summary | null (100%) | Real summary from `falloDestacado` |
| Holding | `"1"` flag | Real doctrinal text from `texto` |
| CitedBy | Empty (61% should have data) | Parsed from HTML |
| Judges | LLM-only (imprecise) | API structured votes + LLM fallback |
| Statutes | LLM-only (incomplete) | API referenciasNormativas + LLM fallback |
| LLM calls | 4 per document always | 1-2 per document (conditional) |
| Embedding quality | Poor (null summary/holding) | High (real legal content) |
| Chunk retrieval | Vector-only, no context, unused by chat | Contextual hybrid search + `search_chunks` tool |
| Search facets | 6 | 10+ (ActionType, OfficialReference, HasDictamen, FederalQuestion) |
| Discovery sources | 1 (acuerdo by date) | 2 (+ fallos destacados curated set) |
| New KB fields | 0 | 7 (ActionType, InternalSubject, OfficialReference, Observations, FederalQuestion, ProceduralFormula, HasDictamen) |
| Graph temporality | No timestamps on edges | `CreatedAt` on 6 relationship tables + `EffectiveDate` on NormRelation — enables temporal graph queries and future GDL |
| Graph queries | 1-hop citations only (`Include().Where()`) | Multi-hop recursive CTEs, graph neighborhood, citation paths, shared entities — full GraphRAG on Azure SQL |
| Graph chat tools | `get_ruling_citations` (1-hop only) | + `explore_graph` (N-hop neighborhood), `find_connection` (shortest path), enhanced citations (multi-hop + entity context) |

---

## Future: LLM-Generated Contextual Retrieval (Phase 4 corpus)

When adding multi-entity documents (laws, decrees, doctrine — Phase 4 of main ROADMAP), evaluate whether the metadata-prefix approach is sufficient or if LLM-generated chunk-specific context is needed. Estimated cost for full LLM contextual retrieval: ~$0.50-$1.50 for entire KB using GPT-4o-mini with prompt caching. Implement only if evaluation shows the deterministic prefix is insufficient for the expanded corpus.

---

## Future: Graph Deep Learning (Phase 3.5 / Phase 4.5)

**Strategic decision**: GraphRAG runs on Azure SQL (TG-G). Neo4j (F3-0) is **deferred indefinitely** — evaluate only if Azure SQL recursive CTEs hit performance limits at >100K rulings. The existing relational schema IS the knowledge graph; `SqlGraphService` with recursive CTEs provides multi-hop traversal, neighborhood queries, and path finding without additional infrastructure.

Temporal edges (E291) and graph queries (T-17) added in F1-21 are the foundation for GDL.

### Phase 3.5 — Graph Deep Learning Core

| Task | Description |
|---|---|
| Graph embedding training | Python service (PyTorch Geometric / DGL). Periodic batch job trains GraphSAGE/GAT on the legal knowledge graph exported from Azure SQL. Produces 128-256 dim node embeddings capturing structural position |
| Dual embedding store | Azure AI Search index with `graphEmbedding` (128-256 dim) alongside `textEmbedding` (3072 dim) |
| Graph-boosted ranking | `score = alpha * text_similarity + beta * graph_similarity + gamma * graph_centrality` in `SearchService` |
| Link prediction model | TransE/RotatE/ComplEx trained on citation + statute edges. Predicts missing citations, suggests related norms |
| Community detection | Periodic job exports citation graph, runs Leiden/Louvain clustering, stores `CommunityId` on Ruling. LLM generates community summaries for global GraphRAG queries |

### Phase 4 (ROADMAP) — Multi-Entity Corpus

| Task | Description |
|---|---|
| `StatuteArticle` entity | Granular article-level nodes (not just statute-level). Enables multi-hop: Ruling → Article → Statute → Article ← Ruling |
| `DoctrineArticle` entity | Academic articles/books with cross-entity edges (DISCUSSES → Ruling, ANALYZES → Statute) |
| `LegalConcept` entity | Abstract principles ("debido proceso", "igualdad"). Knowledge graph completion fills gaps |

### Phase 4.5 — Advanced GDL

| Task | Description |
|---|---|
| Temporal GNN | TGN/TGAT models on timestamped edges (`CreatedAt` from E291). Tracks doctrine evolution, predicts citation velocity decline, detects doctrines in transition |
| Judge behavior prediction | Heterogeneous GNN on Judge-Ruling bipartite graph. Predicts dissent probability, clusters judges by decisional profile |
| Jurisprudential line detection | Spectral clustering / Louvain on citation subgraph. Auto-detects coherent doctrinal threads without text analysis |
| Multi-hop reasoning engine | R-GCN/CompGCN on full heterogeneous graph. Answers complex cross-entity questions traversing Ruling → Statute → Ruling → Judge subgraphs |
| Neo4j evaluation | If Azure SQL CTEs become a bottleneck at scale (>100K rulings), evaluate Neo4j migration. The `IGraphService` abstraction allows swap without changing consumers |
