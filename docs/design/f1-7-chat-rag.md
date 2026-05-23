# Chat RAG — Pipeline Design

| Field | Value |
|---|---|
| **ID** | E076 |
| **Feature** | F1-7 · API — Chat RAG |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the RAG pipeline for the jurisprudential chat endpoint (`POST /api/chat`): search parameters (top-K chunks and rulings), context construction strategy, token limits, and citation format in the response. It serves as the design reference for `ChatQueryCommand`, `ChatQueryHandler` and the legal RAG prompt (E078).

**Reference**: Architecture section 5 (Chat RAG flow); specs section 7.2; ADR-002 (chunking); E059 (chunking strategy).

---

## 1. Pipeline Overview

```
User query
    │
    ├── 1. Generate query embedding (text-embedding-3-large)
    ├── 2. Dual search: rulings-by-chunk (top-K chunks) + rulings-by-ruling (top-K rulings)
    ├── 3. Build context from chunks + ruling metadata
    ├── 4. Call GPT-4o with legal RAG prompt (system + context + user query)
    └── 5. SSE stream response with explicit citations {caseTitle, id}
```

---

## 2. Search Parameters

| Parameter | Value | Rationale |
|---|---|---|
| **Top-K chunks** | 10 | Architecture §5. Granular text excerpts for context. Balances relevance vs. token budget. |
| **Top-K rulings** | 5 | Architecture §5. Ruling-level metadata (summary, holding, caseTitle) for broader coverage. |
| **Chunk index** | `rulings-by-chunk` | Vector search on chunk embeddings (3072 dims). |
| **Ruling index** | `rulings-by-ruling` | Vector search on ruling-level embedding (summary+holding). |

**Configurability**: Values are fixed in Phase 1. Phase 2 may expose `ChatRag__TopKChunks` and `ChatRag__TopKRulings` via configuration if tuning is needed.

---

## 3. Context Construction Strategy

### 3.1 Dual Search

| Search | Index | Returns |
|---|---|---|
| Chunk search | `rulings-by-chunk` | Top 10 chunks: `{ id, rulingId, chunkIndex, text }` |
| Ruling search | `rulings-by-ruling` | Top 5 rulings: `{ rulingId, caseTitle, summary, holding, rulingDate, jurisdictionArea, instance, court }` |

Both searches use the same query embedding. No filters applied (chat is broad jurisprudential Q&A).

### 3.2 Context Assembly

1. **Ruling metadata resolution**: Collect all unique `rulingId` from chunks and ruling search. For each:
   - If present in ruling search results → use that metadata.
   - Otherwise → fetch from `IRulingRepository.GetByIdAsync` (caseTitle, summary, holding, rulingDate, jurisdictionArea, instance, court). Required because chunks may reference rulings not in the top-5 ruling search.

2. **Chunk excerpts**: For each chunk, include:
   - `[Ruling: {caseTitle} ({rulingId})]\n{chunk text}\n`
   - `caseTitle` comes from the resolved ruling metadata.

3. **Ruling metadata blocks**: For each unique ruling (from chunks + ruling search, deduplicated by `rulingId`), include:
   - `Case: {caseTitle} | ID: {rulingId} | Date: {rulingDate} | {jurisdictionArea} / {instance}\nSummary: {summary}\nHolding: {holding}\n`

4. **Order**: Chunks first (by relevance score), then ruling metadata for rulings not yet represented by chunks. Rationale: chunks provide the most relevant text; ruling metadata adds case-level context.

5. **Deduplication**: If a ruling appears in both chunk and ruling results, include it once. Prefer chunk representation when the ruling has chunks in the top 10 (chunk text is more specific).

### 3.3 Context Format (for prompt)

```
## Referenced rulings and excerpts

[Ruling: Smith v. Jones (a1b2c3d4-e5f6-7890-abcd-ef1234567890)]
El demandante alega que la norma aplicada viola el principio de igualdad...

[Ruling: Doe c/ Estado (b2c3d4e5-f6a7-8901-bcde-f12345678901)]
Por ello, se confirma la sentencia de grado...

## Ruling metadata

Case: Smith v. Jones | ID: a1b2c3d4-e5f6-7890-abcd-ef1234567890 | Date: 2024-03-15 | Civil / Primera Instancia
Summary: ...
Holding: ...
```

---

## 4. Token Limit

| Limit | Value | Rationale |
|---|---|---|
| **Max context tokens** | 6,000 | Leaves room for system prompt (~500), user query (~500), and response (~2,000). GPT-4o supports 128K; 6K context is sufficient for 10 chunks + 5 ruling metadata blocks. |
| **Max response tokens** | 2,048 | Typical legal answer length. Configurable via `AzureOpenAI__ChatMaxTokens` (or equivalent). |

**Truncation**: If assembled context exceeds 6,000 tokens, truncate from the end (least relevant chunks/metadata). Preserve chunk order by relevance.

**Token counting**: Use the same tokenizer as the model (`cl100k_base`). E059 references `Microsoft.ML.Tokenizers` or `SharpToken`.

---

## 5. Citation Format

### 5.1 Inline Citation

The model must cite rulings explicitly in the response. Format:

```
{caso: caseTitle, id: rulingId}
```

**Example** (Spanish):

> En el fallo {caso: "Smith v. Jones", id: "a1b2c3d4-e5f6-7890-abcd-ef1234567890"} la Corte sostuvo que...

**Example** (alternative inline):

> ...según lo establecido en *Smith v. Jones* ({id: a1b2c3d4-e5f6-7890-abcd-ef1234567890}).

The exact format will be specified in the RAG prompt (E078). The handler/controller does not parse citations; they are streamed as plain text. The frontend may parse `{caso: ..., id: ...}` to render links to `/fallos/{id}`.

### 5.2 Citation Requirements

| Requirement | Detail |
|---|---|
| **Every referenced ruling** | Must include at least one citation with `caseTitle` and `id`. |
| **Id format** | UUID (lowercase, with hyphens). |
| **caseTitle** | Exact string from the Knowledge Base. Preserve original casing. |

---

## 6. Edge Cases

| Scenario | Behavior |
|---|---|
| **No chunks found** | Use ruling search results only. If no rulings either, return a message indicating no relevant jurisprudence was found. Do not call GPT-4o. |
| **No rulings found** | Use chunk results only. Resolve `caseTitle` from `IRulingRepository.GetByIdAsync` for each chunk's `rulingId`. |
| **Empty query** | Validation error (400). Do not proceed. |
| **Query too long** | Truncate to 1,000 characters before embedding. Log warning. |
| **GPT-4o error** | Return 500 with ProblemDetails. Do not stream partial response. |
| **Stream interrupted** | Client disconnects; handler cancels `CancellationToken`. No retry. |

---

## 7. References

- `docs/architecture/legal-ai-ar-architecture.md` — section 5 (Chat RAG flow), 7.1 (Azure OpenAI)
- `docs/architecture/legal-ai-ar-specs.md` — section 7.2 (Chat RAG internal flow)
- `docs/design/f1-5-chunking.md` — tokenization (E059)
- `docs/design/f1-5-indexer.md` — index structure (E056)
- `docs/design/f1-7-rag-prompt.md` — legal RAG prompt (E078)
