# Chunking Strategy — FullText to Search Chunks

| Field | Value |
|---|---|
| **ID** | E059 |
| **Feature** | F1-5 · Pipeline — IndexerWorker (and EnrichmentWorker) |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the chunking strategy for splitting `FullText` (normalized PDF text) into chunks for the `rulings-by-chunk` index. It covers token size, overlap, handling of short paragraphs, encoding, and chunk ID generation. It serves as the design reference for chunking implementation in the EnrichmentWorker (E048) and IndexerWorker (E056).

**Reference**: ADR-002 (512 tokens, overlap 50, two index levels); Architecture section 4.12 (rulings-by-chunk).

---

## 1. Parameters

| Parameter | Value | Rationale |
|---|---|---|
| **Chunk size** | 512 tokens | ADR-002. Balances context length with retrieval granularity. |
| **Overlap** | 50 tokens | ADR-002. Preserves context across chunk boundaries; avoids splitting mid-sentence. |
| **Encoding** | UTF-8 | Standard for .NET and Azure AI Search. Preserves Spanish characters (ñ, á, etc.). |

---

## 2. Tokenization

### 2.1 Tokenizer

Use a tokenizer compatible with `text-embedding-3-large` and `gpt-4o`:

- **Recommended**: `cl100k_base` (OpenAI cl100k encoding). Available via `Microsoft.ML.Tokenizers` (TiktokenTokenizer) or `SharpToken`.
- **Model**: `text-embedding-3-large` uses the same tokenizer as GPT-4. Use `cl100k_base` for consistency.

### 2.2 Token Counting

- Count tokens per segment to enforce 512-token chunks.
- Use `CountTokens(text)` or equivalent before splitting.

---

## 3. Chunking Algorithm

### 3.1 Basic Flow

```
Input: fullText (string)
Output: chunks [{ index, text }]

1. Split fullText into paragraphs (by \n\n or similar).
2. Accumulate paragraphs into current chunk until token count >= 512.
3. When limit reached:
   - Emit chunk with text (up to 512 tokens).
   - Start next chunk with overlap: last 50 tokens of current chunk + next paragraphs.
4. Repeat until all text is processed.
```

### 3.2 Overlap Implementation

- **Sliding window**: Each new chunk starts 50 tokens before the end of the previous chunk.
- **Boundary**: Prefer splitting at paragraph boundaries. If a paragraph exceeds 512 tokens, split at sentence boundaries. If a sentence exceeds 512 tokens, split at token boundary.

---

## 4. Short Paragraph Handling

| Scenario | Behavior |
|---|---|
| **Paragraph < 512 tokens** | Accumulate with next paragraph(s) until >= 512 tokens or end of text. |
| **Single short document** | Emit as single chunk (even if < 512 tokens). Do not pad. |
| **Last chunk < 512 tokens** | Emit as-is. No minimum chunk size. |
| **Empty or whitespace-only** | Skip. Do not create chunk. |

**Rationale**: Legal rulings may have short sections (e.g. "Por ello, se confirma."). Emitting small chunks preserves retrievability. Padding would add noise.

---

## 5. Chunk ID Generation

| Format | Example |
|---|---|
| **Pattern** | `{rulingId}-{chunkIndex}` |
| **Example** | `a1b2c3d4-e5f6-7890-abcd-ef1234567890-0` |

- `rulingId`: UUID of the ruling (assigned at persist time).
- `chunkIndex`: Zero-based index of the chunk within the ruling.

**Uniqueness**: `rulingId` + `chunkIndex` is unique per ruling. For Azure AI Search document ID, use the combined string.

---

## 6. Output Structure

Each chunk in `IndexerMessage.chunks`:

```json
{
  "index": 0,
  "text": "string (chunk content, up to 512 tokens)"
}
```

For Azure AI Search `rulings-by-chunk` index:

```json
{
  "id": "{rulingId}-{chunkIndex}",
  "rulingId": "string",
  "chunkIndex": 0,
  "text": "string",
  "embedding": [3072 dims]
}
```

---

## 7. Edge Cases

| Scenario | Behavior |
|---|---|
| **fullText < 512 tokens** | Single chunk: index=0, text=fullText. |
| **fullText empty** | No chunks. |
| **Unicode** | Preserve. UTF-8 throughout. |
| **Very long word** | Rare. Split at token boundary if word exceeds 512 tokens. |

---

## 8. References

- `docs/architecture/legal-ai-ar-architecture.md` — ADR-002, section 4.12
- `docs/design/f1-4-enrichment.md` — EnrichmentWorker chunking (E048)
- `docs/design/f1-5-indexer.md` — IndexerWorker steps (E056)
- Microsoft.ML.Tokenizers: https://www.nuget.org/packages/Microsoft.ML.Tokenizers
- SharpToken: https://github.com/dmitry-brazhenko/SharpToken
