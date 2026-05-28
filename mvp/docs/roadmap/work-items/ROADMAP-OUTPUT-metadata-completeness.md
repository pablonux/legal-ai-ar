# ROADMAP Output — F1-19 · CSJN Metadata Completeness

> **Architect output** · 2026-04-07 · For Manager to merge into ROADMAP.md
> **Deliverable range**: E250–E268
> **Placement**: Phase 1, after F1-18

---

## Context

A comprehensive data-flow analysis revealed that the CSJN ingestion pipeline reads substantial metadata from governmental endpoints but drops, hardcodes, or incorrectly maps significant portions before persistence. Specifically:

- **5 fields** available in `abrirAnalisis.html` are forced to NULL/FALSE at `CsjnEnrichmentStrategy`
- **1 field** (`JurisdictionArea`) is erroneously mapped from `SubjectArea` instead of from `Jurisdiction`
- **`Instance`** and **`Court`** are hardcoded to `"CSJN"` instead of resolved from API/catalogs
- **`AnalysisId`** exists in the entity but is never set during indexing
- **`getCitantes.html`** data (reverse citations) is read but completely discarded
- **Discovery metadata** from `paginarFallos.html` (7+ fields) is ignored
- **Search index** uses hardcoded `Instance` for the `Court` facet and lacks `SubjectArea`, `ResourceType`, `IsUnconstitutional` facets

These issues degrade the KB quality: search filters produce incorrect results, facets show only "CSJN", and the relational model loses traceability and richness.

---

## Impact Analysis

### Data model changes

| Entity / Record | Change |
|---|---|
| `ExtractedMetadata` | Add 4 fields: `Jurisdiction`, `ResourceType`, `SubjectArea`, `IsUnconstitutional` |
| `IndexerMessage` | Add `AnalysisId` field. Add `CitedBy` collection |
| `EnrichmentMessage` | Add `AnalysisId` passthrough |
| `ParserMessage` | Already has `AnalysisId` — no change needed |
| `RulingIndexInput` | Add `SubjectArea`, `ResourceType`, `IsUnconstitutional` fields |
| `Ruling` entity | Already has all fields — no schema change needed |
| `Court` entity | No schema change — but resolution logic changes |
| `Citation` entity | No schema change — but reverse citations now created |

### Azure AI Search schema changes

| Index | Change |
|---|---|
| `rulings-by-ruling` | Add `subjectArea` (filterable, facetable), `resourceType` (filterable, facetable), `isUnconstitutional` (filterable). Fix `court` to use actual court name |
| `rulings-by-chunk` | No change |

### Pipeline workers affected

| Worker | Changes |
|---|---|
| **Crawler** | `CsjnPaginationParser` captures additional discovery fields; enriched `ParserMessage` |
| **Parser** | `CsjnMetadataMapper` field mapping corrections; `ExtractedMetadata` extended |
| **Enrichment** | `CsjnEnrichmentStrategy` passes through API fields instead of nullifying |
| **Indexer** | `PersistRulingStep` sets `AnalysisId`, fixes court resolution. `IndexSearchStep` indexes new fields. New reverse citation creation |

### API and Frontend

| Component | Change |
|---|---|
| `SearchFilters` | New optional filters: `subjectArea`, `resourceType`, `isUnconstitutional` |
| `SearchRulingsHandler` | Pass new filters to `ISearchService` |
| `RulingsController` | No change (filters are already dynamic in POST body) |
| Search UI filters | Add SubjectArea, ResourceType, IsUnconstitutional filter controls |
| `rulings-by-ruling` index | Recreate with new fields (one-time setup script) |

---

## F1-19 · CSJN Metadata Completeness — Full Governmental Data Flow

**Objective**: Ensure 100% of metadata available from CSJN governmental endpoints flows through the entire ingestion pipeline to persistence in the KB (relational DB, search index, blob storage). Fix broken mappings, nullified fields, and missing data paths identified in the data-flow audit.

**Dependencies**: F1-2, F1-3, F1-4, F1-5 `[x] DEV`. F0-2 `[x] DEV` (for catalog seed data).

---

### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E250 | `docs/design/f1-19-metadata-completeness.md` — field-by-field audit: all CSJN API fields at each pipeline stage (read → mapped → enriched → indexed), correction specification per field, Court resolution redesign, search index schema delta, ExtractedMetadata/RulingData record changes | `[ ]` | `[ ]` |
| E251 | `docs/design/f1-19-metadata-flow.mermaid` — corrected data flow diagram: every CSJN field from API response through CsjnApiParser → CsjnMetadataMapper → ExtractedMetadata → CsjnEnrichmentStrategy → RulingData → PersistRulingStep + IndexSearchStep, showing field name at each stage | `[ ]` | `[ ]` |

---

### T-01 · Extend `ExtractedMetadata` with missing CSJN fields

Add the 4 fields that `CsjnApiParser` reads from `abrirAnalisis.html` but are currently dropped at `CsjnMetadataMapper` because `ExtractedMetadata` lacks them.

**File**: `LegalAiAr.Core/Messages/EnrichmentMessage.cs`

**Add to `ExtractedMetadata` record**:
- `Jurisdiction` (string?) — e.g. `APELACION EXTRAORDINARIA`
- `ResourceType` (string?) — e.g. `RECURSO EXTRAORDINARIO FEDERAL`
- `SubjectArea` (string?) — e.g. `Tributario - Bancario`
- `IsUnconstitutional` (bool) — default `false`

---

### T-02 · Fix `CsjnMetadataMapper` field mappings

**File**: `LegalAiAr.Worker.Parser/Parsers/CsjnMetadataMapper.cs`

Current broken mappings to fix:

| Field | Current (broken) | Corrected |
|---|---|---|
| `JurisdictionArea` | `api.SubjectArea` (wrong source) | `api.Jurisdiction` (e.g. `APELACION EXTRAORDINARIA`) |
| `Instance` | Hardcoded `"CSJN"` | Hardcoded `"CSJN"` remains valid for CSJN source — but see T-05 for Court resolution |
| `Court` | Hardcoded `"CSJN"` | Use `"Corte Suprema de Justicia de la Nación"` (proper court name from catalogs) |

New mappings to add:

| Field | Source |
|---|---|
| `Jurisdiction` | `api.Jurisdiction` |
| `ResourceType` | `api.ResourceType` |
| `SubjectArea` | `api.SubjectArea` |
| `IsUnconstitutional` | `api.IsUnconstitutional` |

---

### T-03 · Fix `CsjnEnrichmentStrategy` field pass-through

**File**: `LegalAiAr.Worker.Enrichment/Strategies/CsjnEnrichmentStrategy.cs`

Stop nullifying fields that come from the governmental API. Replace:

| Field | Current | Corrected |
|---|---|---|
| `Jurisdiction` | `null` | `meta.Jurisdiction` |
| `ResourceType` | `null` | `meta.ResourceType` |
| `SubjectArea` | `null` | `meta.SubjectArea` |
| `IsUnconstitutional` | `false` | `meta.IsUnconstitutional` |

---

### T-04 · Propagate and persist `AnalysisId`

`AnalysisId` exists on the `Ruling` entity and in `ParserMessage`, but is never carried through to `PersistRulingStep`.

Changes:
1. **`IndexerMessage`** — add `AnalysisId` (string?) field
2. **`EnrichmentMessage`** — add `AnalysisId` (string?) field (or ensure it passes through)
3. **`CsjnEnrichmentStrategy`** — set `AnalysisId` from message context
4. **`PersistRulingStep`** — set `ruling.AnalysisId = message.AnalysisId`

---

### T-05 · Fix Court resolution in `PersistRulingStep`

**File**: `LegalAiAr.Worker.Indexer/Steps/PersistRulingStep.cs`

Current behavior creates/looks-up a Court with `name: "CSJN"` and `instance: "CSJN"`.

Corrected behavior:
1. Use proper court name from `ExtractedMetadata.Court` (e.g. `"Corte Suprema de Justicia de la Nación"`)
2. Resolve `JurisdictionArea` from actual `Ruling.JurisdictionArea` (now correctly sourced from `api.Jurisdiction`)
3. Match against seed catalog courts (from `LegalAiAr.SeedCatalogs`) by name before creating new entries
4. `Instance` on Court entity should reflect the judicial hierarchy level (e.g. `"CSJN"` for Supreme Court, `"Cámara"` for appellate)

---

### T-06 · Persist reverse citations from `getCitantes.html`

Currently `CsjnApiParser` reads `getCitantes.html` and parses `CitedBy` (list of `{analysisId, caseNumber}`), but this data is completely discarded in the pipeline.

Changes:
1. **`CsjnApiMetadata.CitedBy`** — already populated (no change)
2. **`ParserMessage.ApiMetadata.CitedBy`** — already serialized (no change)
3. **`ExtractedMetadata`** — add `CitedBy` collection (new field)
4. **`CsjnMetadataMapper`** — map `api.CitedBy` to `ExtractedMetadata.CitedBy`
5. **`IndexerMessage`** — add `CitedBy` collection
6. **`CsjnEnrichmentStrategy`** — pass through `CitedBy` from metadata
7. **`PersistRulingStep`** — for each `CitedBy` entry, create a `Citation` record where `TargetRulingId` = current ruling and `SourceRulingId` resolved from `analysisId`/`caseNumber` (or left NULL for future resolution, analogous to existing forward citation logic)

This enables bidirectional citation graph construction from governmental data.

---

### T-07 · Capture discovery metadata from `paginarFallos.html`

**File**: `LegalAiAr.Worker.Crawler/Sources/CsjnPaginationParser.cs`

Currently only `Codigo` and `idAnalisis` are extracted. The JSON response contains additional fields:

| Field | Value example | Use |
|---|---|---|
| `identificadorExpediente` | `"CAF 9548/2021/CA1-CS1"` | CaseNumber — avoids relying solely on abrirAnalisis |
| `materiaSecretaria` | `"2"` | Maps to court secretariat/subject area |
| `anioFallo` | `"2024"` | Ruling year — useful for validation |
| `sentenciaArbitraria` | `"N"` | Whether it's an arbitrary sentence appeal |
| `tieneVotos` | `"S"` | Whether individual votes exist |
| `inadmisibleCompetencia` | `"N"` | Whether competence was declared inadmissible |

Changes:
1. **`CsjnPaginationParser`** — extract additional fields into a richer `DiscoveredDocument` model
2. **`ParserMessage`** — add optional `DiscoveryMetadata` object with these fields
3. **`CsjnMetadataMapper`** — use `identificadorExpediente` as fallback for `CaseNumber` when abrirAnalisis doesn't provide it

---

### T-08 · Enrich search index schema

**File**: `LegalAiAr.Worker.Indexer/Steps/IndexSearchStep.cs`

| Change | Detail |
|---|---|
| Fix `Court` field | Use actual court name (from `Court` entity via `PersistRulingStep` result) instead of `ruling.Instance` |
| Add `subjectArea` | String, filterable, facetable |
| Add `resourceType` | String, filterable, facetable |
| Add `isUnconstitutional` | Boolean, filterable |

Also:
1. **`RulingIndexInput`** — add `SubjectArea`, `ResourceType`, `IsUnconstitutional` fields
2. **`RulingSearchDocument`** (Infrastructure) — add corresponding search document fields
3. **Azure AI Search setup script** — update `scripts/run-setup-search.ps1` to include new fields in index definition
4. **`SearchFilters`** — add `SubjectArea`, `ResourceType`, `IsUnconstitutional` optional filter properties
5. **`AzureSearchService`** — apply new filters when building search queries

---

### T-09 · Frontend: add new search filters

Add UI controls for the 3 new filterable fields in the search interface.

1. **SubjectArea** — dropdown/combobox populated from facet values (e.g. `Tributario`, `Penal`, `Civil`)
2. **ResourceType** — dropdown populated from facet values (e.g. `RECURSO EXTRAORDINARIO FEDERAL`)
3. **IsUnconstitutional** — checkbox toggle filter

These integrate into the existing collapsible filter panel in `SearchHomeComponent` / `SearchResultsComponent`.

---

### T-10 · Update unit tests

| Test file | Updates |
|---|---|
| `CsjnMetadataMapperTests` | Verify corrected field mappings: JurisdictionArea from Jurisdiction, SubjectArea from SubjectArea, new fields present |
| `CsjnEnrichmentStrategyTests` | Verify Jurisdiction, ResourceType, SubjectArea, IsUnconstitutional pass through (not null/false) |
| `PersistRulingStepTests` | Verify AnalysisId is set; Court resolved with proper name |
| `IndexSearchStepTests` | Verify new fields indexed; Court uses actual name |
| `CsjnPaginationParserTests` | Verify additional discovery fields extracted |
| `ResolveCitationsStepTests` | Add tests for reverse citation creation from CitedBy |

---

### T-11 · Data migration for existing rulings

One-time migration tool/script to re-enrich existing indexed rulings that have NULL values for Jurisdiction, ResourceType, SubjectArea, and IsUnconstitutional=false:

1. Query `Rulings` where `SourceId = 1` (CSJN) and `Jurisdiction IS NULL`
2. For each ruling with a known `AnalysisId` (or `ExternalId`): call `abrirAnalisis.html` to fetch missing metadata
3. Update the ruling record with the corrected fields
4. Update the search index document

This can be implemented as an extension to the existing `LegalAiAr.BulkDownload` tool or as a new `LegalAiAr.BackfillMetadata` tool.

---

### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E252 | `ExtractedMetadata` extended with `Jurisdiction`, `ResourceType`, `SubjectArea`, `IsUnconstitutional` — pipeline compiles and all existing tests pass | `[ ]` | `[ ]` |
| E253 | `CsjnMetadataMapper` corrected: `JurisdictionArea` mapped from `api.Jurisdiction`, `SubjectArea` mapped from `api.SubjectArea`, `Court` uses proper name, all 4 new fields mapped. Verified with updated unit tests | `[ ]` | `[ ]` |
| E254 | `CsjnEnrichmentStrategy` passes through Jurisdiction, ResourceType, SubjectArea, IsUnconstitutional from `ExtractedMetadata` instead of null/false. Verified with updated unit tests | `[ ]` | `[ ]` |
| E255 | `AnalysisId` propagated through `EnrichmentMessage` → `IndexerMessage` → `PersistRulingStep`. `Ruling.AnalysisId` populated in DB for all new CSJN rulings | `[ ]` | `[ ]` |
| E256 | Court resolution in `PersistRulingStep` uses actual court name from API/catalogs. Court facet in search index shows proper names instead of hardcoded `"CSJN"` | `[ ]` | `[ ]` |
| E257 | Reverse citations from `getCitantes.html` persisted as `Citation` records. Bidirectional citation graph constructed from governmental data | `[ ]` | `[ ]` |
| E258 | `CsjnPaginationParser` extracts discovery metadata (`identificadorExpediente`, `materiaSecretaria`, `anioFallo`, `sentenciaArbitraria`, `tieneVotos`, `inadmisibleCompetencia`). Passed to downstream via `ParserMessage.DiscoveryMetadata` | `[ ]` | `[ ]` |
| E259 | `rulings-by-ruling` search index updated with `subjectArea` (facetable), `resourceType` (facetable), `isUnconstitutional` (filterable). Index setup script updated. `SearchFilters` and `AzureSearchService` support new filters | `[ ]` | `[ ]` |
| E260 | Frontend search filters: SubjectArea dropdown, ResourceType dropdown, IsUnconstitutional checkbox. Populated from search facets | `[ ]` | `[ ]` |
| E261 | Unit tests updated: CsjnMetadataMapper, CsjnEnrichmentStrategy, PersistRulingStep (AnalysisId + Court), IndexSearchStep (new fields), CsjnPaginationParser (discovery fields), reverse citation creation | `[ ]` | `[ ]` |
| E262 | Backfill tool for existing CSJN rulings: re-fetches Jurisdiction, ResourceType, SubjectArea, IsUnconstitutional, AnalysisId from `abrirAnalisis.html` and updates DB + search index | `[ ]` | `[ ]` |

---

## Architecture document updates

| Document | Update |
|---|---|
| `legal-ai-ar-architecture.md` § 3.2.2 | Add note that ALL fields from `abrirAnalisis.html` are now persisted. Remove reference to Selenium as sole discovery method. |
| `legal-ai-ar-architecture.md` § 4.1 | Update Source column for Jurisdiction, ResourceType, SubjectArea, IsUnconstitutional to `CSJN API` (no longer `Source/GPT`) |
| `legal-ai-ar-architecture.md` § 4.12 | Update `rulings-by-ruling` index schema with new fields |
| `legal-ai-ar-specs.md` § 6.0 | Update CSJN discovery section to reflect new HTTP-only sources and captured metadata |
| ADR-008 | Add note: "getCitantes.html now consumed for bidirectional citation graph" |

---

## Recommended task execution order

```
T-00 (design docs)
  │
  ├─► T-01 (ExtractedMetadata extension)
  │     │
  │     ├─► T-02 (CsjnMetadataMapper fix)
  │     │     │
  │     │     └─► T-03 (CsjnEnrichmentStrategy fix)
  │     │           │
  │     │           ├─► T-04 (AnalysisId propagation)
  │     │           ├─► T-05 (Court resolution)
  │     │           └─► T-06 (CitedBy persistence)
  │     │
  │     └─► T-07 (Discovery metadata) — independent
  │
  ├─► T-08 (Search index) — after T-02, T-03, T-05
  │     │
  │     └─► T-09 (Frontend filters) — after T-08
  │
  ├─► T-10 (Unit tests) — alongside each task
  │
  └─► T-11 (Backfill) — after T-01 through T-08 complete
```

**Critical path**: T-00 → T-01 → T-02 → T-03 → T-08 → T-09

**Parallelizable**: T-04, T-05, T-06, T-07 can run in parallel after T-03.

---

## Summary

| Metric | Value |
|---|---|
| Design deliverables | 2 (E250–E251) |
| Development deliverables | 11 (E252–E262) |
| Total deliverables | 13 |
| Pipeline workers modified | 4 (Crawler, Parser, Enrichment, Indexer) |
| New records/fields added | ~12 fields across 4 message records |
| Search index fields added | 3 (subjectArea, resourceType, isUnconstitutional) |
| Search index fields fixed | 1 (court) |
| DB columns affected | 0 new columns (all exist; values now populated) |
| CSJN API fields rescued | 6 (Jurisdiction, ResourceType, SubjectArea, IsUnconstitutional, AnalysisId, CitedBy) |
| Erroneous mappings fixed | 2 (JurisdictionArea ← SubjectArea swap, Court hardcode) |
