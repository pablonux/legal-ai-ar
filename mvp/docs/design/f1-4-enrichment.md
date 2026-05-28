# Enrichment Strategy — EnrichmentWorker

| Field | Value |
|---|---|
| **ID** | E048 |
| **Feature** | F1-4 · Pipeline — EnrichmentWorker |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the enrichment strategy by source: what GPT-4o extracts for CSJN vs other sources, how the strategy is selected, and decisions on structured output (`json_schema`). It serves as the design reference for `IEnrichmentStrategy`, `CsjnEnrichmentStrategy`, and `FullEnrichmentStrategy` (T-01, T-02), and is consumed by developers implementing the EnrichmentWorker.

**Reference**: Architecture section 3.2.2, 3.3, 3.4 (EnrichmentWorker); ADR-008 (CSJN API-first, GPT-4o gap-filling only).

---

## 1. Strategy by Source

| Source | SourceId | Strategy | What GPT-4o extracts |
|---|---|---|---|
| **CSJN** | 1 | Gap-filling | Only fields in `missingFields`: judges, cited_statutes, citation_types |
| **SAIJ** | 2 | Full enrichment | All structured fields (judges, statutes, keywords, summary, holding, citations, jurisdiction_area, instance) |
| **PJN** | 3 | Full enrichment | Same as SAIJ |
| **SCBA** | 4 | Full enrichment | Same as SAIJ |

---

## 2. CSJN Strategy (Phase 1)

**Principle**: Minimize GPT-4o usage. The CSJN API provides most metadata; GPT-4o fills only the gaps.

### 2.1 Input

- `EnrichmentMessage` with `extractedMetadata` (from ParserWorker), `normalizedText` (PDF full text), `missingFields` (typically `["judges", "cited_statutes", "citation_types"]`).

### 2.2 Processing

For each field in `missingFields`:

| Field | Prompt | Input context | Output shape |
|---|---|---|---|
| `judges` | JudgesExtractionPrompt | `normalizedText` (or excerpt around signatures) | `{ judges: [{ firstName, lastName, participationType }] }` |
| `cited_statutes` | StatutesExtractionPrompt | `normalizedText` | `{ statutes: [{ number, name, articles }] }` |
| `citation_types` | CitationTypePrompt | `normalizedText` + `extractedMetadata.citations` (alias per citation) | `{ citationTypes: [{ alias, citationType }] }` |

### 2.3 Merge

- Start with `extractedMetadata` from the message.
- Overlay GPT-4o outputs: add `judges`, merge `statutes` into ruling data, set `citationType` per citation in the citations array.
- Build complete `IndexerMessage` with ruling, judges, keywords, statutes, citations (with types), chunks.

### 2.4 Chunking and FullText

The EnrichmentWorker sets `ruling.fullText = normalizedText` and produces `chunks` from it using the strategy in E059 (512 tokens, overlap 50). The IndexerMessage includes both `ruling.fullText` and `chunks`. The IndexerWorker consumes the message and generates embeddings over the provided chunks. Chunking logic may be shared (e.g. `IChunkingService`) between EnrichmentWorker and IndexerWorker if the IndexerWorker also needs to rechunk; for Phase 1, the EnrichmentWorker produces chunks.

---

## 3. Full Enrichment Strategy (Phase 2 — SAIJ, PJN, SCBA)

**Principle**: The parser provides only basic metadata (caseTitle, date, court) from HTML. GPT-4o extracts all structured fields from the PDF and HTML in a single structured output call.

### 3.1 Input

- `EnrichmentMessage` with minimal `extractedMetadata`, `normalizedText`, and `missingFields` containing all fields that require extraction.

### 3.2 Processing

- Single GPT-4o call with `FullEnrichmentPrompt` and a JSON schema covering: judges, cited_statutes, keywords, summary, holding, citations (with alias and citationType), jurisdiction_area, instance.

### 3.3 Output

- Complete structured payload merged into `IndexerMessage`.

**Note**: Full enrichment is implemented in F2-1 (SAIJ). E048 focuses on Phase 1 (CSJN). The interface `IEnrichmentStrategy` is designed to support both strategies.

---

## 4. Strategy Selection

```csharp
// EnrichmentWorkerService or equivalent
IEnrichmentStrategy GetStrategy(int sourceId)
{
    return sourceId switch
    {
        1 => _csjnStrategy,   // CsjnEnrichmentStrategy
        2 => _fullStrategy,   // FullEnrichmentStrategy (Phase 2)
        3 => _fullStrategy,
        4 => _fullStrategy,
        _ => throw new ArgumentException($"Unknown source: {sourceId}")
    };
}
```

**Phase 1**: Only `sourceId = 1` (CSJN) is active. `FullEnrichmentStrategy` can be a stub or throw for Phase 1.

---

## 5. Structured Output (json_schema)

### 5.1 Decision

Use Azure OpenAI `response_format: { "type": "json_schema", "json_schema": { ... } }` for all GPT-4o enrichment calls.

**Rationale**:
- Guarantees valid JSON output.
- Enforces schema (required fields, types).
- Reduces parsing errors and retries.

### 5.2 Schema Definition

Each prompt has an associated JSON schema. Example for judges:

```json
{
  "type": "json_schema",
  "json_schema": {
    "name": "judges_extraction",
    "strict": true,
    "schema": {
      "type": "object",
      "properties": {
        "judges": {
          "type": "array",
          "items": {
            "type": "object",
            "properties": {
              "firstName": { "type": "string" },
              "lastName": { "type": "string" },
              "participationType": { "type": "string", "enum": ["SIGNATORY", "DISSENT", "MAJORITY"] }
            },
            "required": ["firstName", "lastName", "participationType"]
          }
        }
      },
      "required": ["judges"]
    }
  }
}
```

### 5.3 Validation

After receiving the response:
1. Parse JSON.
2. Validate against the expected schema (required fields, types).
3. If invalid: log, retry once with a simplified prompt or fail and move to DLQ.

---

## 6. Interface

```csharp
// LegalAiAr.Core or LegalAiAr.Application
public interface IEnrichmentStrategy
{
    Task<IndexerMessage> EnrichAsync(EnrichmentMessage message, CancellationToken cancellationToken = default);
}
```

Each strategy receives `EnrichmentMessage` and returns `IndexerMessage` ready for `queue-indexer`.

---

## 7. Model and Configuration

| Setting | Value |
|---|---|
| Model | `gpt-4o` |
| Deployment | `AzureOpenAI__ChatDeploymentName` |
| Response format | `json_schema` with strict schema |
| Temperature | 0 (deterministic extraction) |

**Reference**: ADR-014 (Azure OpenAI, gpt-4o).

---

## 8. Documented Decisions

| ID | Decision | Justification |
|---|---|---|
| D1 | json_schema for all enrichment calls | Ensures parseable, typed output. Reduces errors. |
| D2 | Strategy selection by sourceId | Simple, explicit. No heuristic. |
| D3 | Temperature 0 | Extraction is factual; minimize variance. |
| D4 | EnrichmentWorker produces chunks | IndexerMessage includes chunks; IndexerWorker generates embeddings. Separation of concerns. |

---

## 9. References

- `docs/architecture/legal-ai-ar-architecture.md` — sections 3.2.2, 3.3, 3.4, ADR-008, ADR-014
- `docs/design/f1-3-missing-fields.md` — missingFields logic (E041)
- `docs/design/f1-4-prompts.md` — Prompt specifications (E049)
- `docs/design/f1-5-chunking.md` — Chunking strategy (E059)
