# Missing Fields Logic — ParserWorker

| Field | Value |
|---|---|
| **ID** | E041 |
| **Feature** | F1-3 · Pipeline — ParserWorker (CSJN) |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the logic for determining `missingFields` in the `EnrichmentMessage`. The EnrichmentWorker uses `missingFields` to decide which fields require GPT-4o extraction. It serves as the design reference for the ParserWorker (T-02) and EnrichmentWorker strategy selection, and is consumed by developers implementing both workers.

**Reference**: Architecture section 3.2.2 (GPT-4o intervenes only for judges, statutes, citation types); section 3.4 (ParserWorker, EnrichmentWorker).

---

## 1. Overview

| Field | Available from CSJN API? | Requires GPT-4o? |
|---|---|---|
| `judges` | No | Yes |
| `cited_statutes` | No | Yes |
| `citation_types` | No | Yes |
| `caseTitle` | Yes (abrirAnalisis) | No |
| `rulingDate` | Yes (abrirAnalisis) | No |
| `jurisdiction` | Yes (abrirAnalisis) | No |
| `resourceType` | Yes (abrirAnalisis) | No |
| `rulingDirection` | Yes (abrirAnalisis) | No |
| `subjectArea` | Yes (abrirAnalisis) | No |
| `isUnconstitutional` | Yes (abrirAnalisis) | No |
| `summary` | Yes (abrirAnalisis, getSumariosAnalisis) | No |
| `holding` | Yes (getSumariosAnalisis) | No |
| `keywords` | Yes (getSumariosAnalisis) | No |
| `citations` (alias, summaryId) | Yes (getCitas) | No |
| `citation_types` (UPHOLDS/OVERRULES/etc.) | No | Yes |

---

## 2. CSJN Logic (Phase 1)

For source `SourceId = 1` (CSJN), the API never provides:

1. **judges** — Signing judges (firstName, lastName, participationType) are not in any CSJN endpoint. They must be extracted from the PDF text by GPT-4o.
2. **cited_statutes** — Laws and decrees cited in the ruling (number, name, articles) are not in the API. GPT-4o extracts them from the full text.
3. **citation_types** — The API provides citation aliases (e.g. `Fallos: 328:1883`) but not the semantic type (UPHOLDS, OVERRULES, DISTINGUISHES, CITES). GPT-4o infers the type from textual context.

**Result**: For every CSJN document, `missingFields` is always:

```json
["judges", "cited_statutes", "citation_types"]
```

**Implementation**: No conditional logic. When `sourceId === 1`, set `missingFields = ["judges", "cited_statutes", "citation_types"]`.

---

## 3. Optional Fields — Null/Empty Handling

If the API returns null or empty for a field that is normally available (e.g. `summary` or `holding`), the parser has two options:

| Option | Behavior |
|---|---|
| **A** | Do not add to `missingFields`. Pass through as null/empty. The EnrichmentWorker does not have a prompt to fill these; downstream steps handle missing data. |
| **B** | Add to `missingFields`. Would require an EnrichmentWorker prompt for that field. |

**Decision**: Option A. The EnrichmentWorker for CSJN only has prompts for `judges`, `cited_statutes`, and `citation_types`. If `summary` or `holding` is empty from the API, we do not add them to `missingFields` — we pass the empty value. The IndexerWorker or validation layer may flag incomplete rulings if needed.

---

## 4. Other Sources (Phase 2)

For SAIJ, PJN, SCBA (HTML+PDF strategy), the pipeline uses **full enrichment**. All structured fields are extracted by GPT-4o from the PDF and HTML. The `missingFields` logic for those sources is:

- **All fields** that the Indexer expects are potentially missing: `judges`, `cited_statutes`, `citation_types`, `summary`, `holding`, `keywords`, `citations`, `jurisdiction_area`, `instance`, etc.
- The parser sets `missingFields` to the full list of fields that require extraction, or the EnrichmentWorker uses a different strategy (`FullEnrichmentStrategy`) that does not rely on `missingFields` and instead runs the full extraction prompt.

**Note**: E041 focuses on CSJN (Phase 1). Phase 2 logic is documented in F2-1 and F2-2 design deliverables.

---

## 5. EnrichmentWorker Usage

The EnrichmentWorker receives `EnrichmentMessage` with `missingFields`. For CSJN:

- If `missingFields` contains `"judges"` → call `JudgesExtractionPrompt`.
- If `missingFields` contains `"cited_statutes"` → call `StatutesExtractionPrompt`.
- If `missingFields` contains `"citation_types"` → call citation type classification (per citation in `extractedMetadata.citations`).

The worker merges GPT-4o outputs with `extractedMetadata` and publishes the complete `IndexerMessage`.

---

## 6. Documented Decisions

| ID | Decision | Justification |
|---|---|---|
| D1 | CSJN missingFields is fixed | API never provides judges, statutes, or citation types. No per-document variation. |
| D2 | Empty API fields not in missingFields | EnrichmentWorker has no prompts for them. Keeps scope minimal. |

---

## 7. References

- `docs/architecture/legal-ai-ar-architecture.md` — sections 3.2.2, 3.4
- `docs/design/f1-3-parser.md` — ExtractedMetadata, EnrichmentMessage (E038)
- `docs/design/f1-4-enrichment.md` — Enrichment strategy (E048)
- `docs/design/f1-4-prompts.md` — Prompt specifications (E049)
