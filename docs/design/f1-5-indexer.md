# IndexerWorker — Step Sequence and Contracts

| Field | Value |
|---|---|
| **ID** | E056 |
| **Feature** | F1-5 · Pipeline — IndexerWorker |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the sequence of steps in the IndexerWorker, the input/output contract of each step, the idempotency policy by `ContentHash`, and the handling of partial errors. It serves as the design reference for T-01 to T-11 and is consumed by developers implementing the IndexerWorker.

**Reference**: Architecture section 3.4 (IndexerWorker), 4.1–4.10 (Data model); E031 (Deduplication).

---

## 1. Step Sequence

| Order | Step | Responsibility |
|---|---|---|
| 0 | Idempotency check | Verify `ContentHash` not in Rulings; if exists, discard message and exit |
| 1 | PersistRulingStep | Upsert Rulings, Courts, Judges, RulingJudges, Keywords, RulingKeywords, Statutes, RulingStatutes, Citations in Azure SQL |
| 2 | UploadBlobStep | Upload PDF to Blob if not already present (Phase 1 CSJN: typically no-op; CrawlerWorker already uploaded) |
| 3 | GenerateEmbeddingsStep | Generate embeddings for ruling level (summary+holding) and each chunk |
| 4 | IndexSearchStep | Upsert documents in `rulings-by-ruling` and `rulings-by-chunk` (Azure AI Search) |
| 5 | ResolveCitationsStep | Retroactive citation resolution: match Citations with TargetRulingId=null to newly indexed ruling |

**Phase 1**: No Neo4j. Graph relationships are stored in SQL tables (Citations, RulingJudges, RulingKeywords, RulingStatutes). Neo4j is introduced in Phase 3 (F3-0).

---

## 2. Idempotency Policy

### 2.1 Check

Before any step, the IndexerWorker:

1. Reads `contentHash` from the incoming `IndexerMessage` (propagated from CrawlerWorker via pipeline).
2. Calls `IRulingRepository.ExistsByContentHashAsync(contentHash)`.
3. If `true` → discard the message without error. Log at debug level. Do not execute any step.
4. If `false` → proceed with the full pipeline.

### 2.2 Rationale

- Message replay from DLQ must not create duplicates.
- CrawlerWorker deduplication may be bypassed in edge cases; IndexerWorker provides a second guarantee.

**Reference**: E031 (Deduplication strategy).

---

## 3. Step Contracts

### 3.1 PersistRulingStep

| Aspect | Detail |
|---|---|
| **Input** | `IndexerMessage` (ruling, judges, keywords, statutes, citations, documentId, contentHash) |
| **Output** | `RulingId` (UUID of persisted ruling), updated DbContext |
| **Side effects** | Inserts/updates in Rulings, Courts, Judges, RulingJudges, Keywords, RulingKeywords, Statutes, RulingStatutes, Citations |
| **Transaction** | Single transaction. Rollback on any failure. |

**Order of persistence** (to satisfy FKs):

1. Court (upsert by name/jurisdiction; get or create CourtId)
2. Ruling (insert; generate Id)
3. Judges (upsert by firstName+lastName; get or create JudgeId)
4. RulingJudges (insert)
5. Keywords (upsert by externalCode or description; get or create KeywordId)
6. RulingKeywords (insert with SortOrder)
7. Statutes (upsert by number; get or create StatuteId)
8. RulingStatutes (insert with Articles)
9. Citations (insert with SourceRulingId, ExternalAlias, CsjnSummaryId, CitationType; TargetRulingId=null initially)

---

### 3.2 UploadBlobStep

| Aspect | Detail |
|---|---|
| **Input** | `IndexerMessage` (ruling.blobPath, ruling.fullText or PDF bytes if available) |
| **Output** | Confirmed `blobPath` (unchanged if already uploaded) |
| **Side effects** | Upload to Azure Blob Storage if PDF not present |
| **Phase 1** | CrawlerWorker uploads PDF. `blobPath` is in the message. Step is a no-op: verify blob exists or skip. |

**Decision**: For Phase 1, this step verifies the blob exists at `ruling.blobPath`. If missing (edge case), fail and move to DLQ. No upload from IndexerWorker in Phase 1.

---

### 3.3 GenerateEmbeddingsStep

| Aspect | Detail |
|---|---|
| **Input** | Ruling (summary, holding), Chunks (text per chunk) |
| **Output** | Ruling-level embedding (3072 dims), Chunk-level embeddings (3072 dims each) |
| **Model** | `text-embedding-3-large` |
| **Ruling text** | `summary + " " + holding` (or concatenation) |
| **Chunk text** | Each chunk's `text` field from the message |

**Reference**: ADR-002 (chunking); Architecture section 4.12 (index schema).

---

### 3.4 IndexSearchStep

| Aspect | Detail |
|---|---|
| **Input** | Ruling metadata, ruling embedding, chunks with embeddings, RulingId |
| **Output** | Documents upserted in Azure AI Search |
| **Indexes** | `rulings-by-ruling`, `rulings-by-chunk` |
| **Operation** | MergeOrUpload (idempotent upsert) |
| **Document IDs** | `rulings-by-ruling`: e.g. `{rulingId}`; `rulings-by-chunk`: e.g. `{rulingId}-{chunkIndex}` |

**Reference**: Architecture section 4.12 (index schemas).

---

### 3.5 ResolveCitationsStep

| Aspect | Detail |
|---|---|
| **Input** | Newly indexed RulingId, CaseNumber (or volume/page), Citations from the message |
| **Output** | Updated Citations rows (TargetRulingId set where match found) |
| **Inbound** | Search Citations where ExternalAlias matches new ruling; set TargetRulingId |
| **Outbound** | For each citation from the new ruling, check if TargetRuling exists; set TargetRulingId if found |

**Reference**: Architecture section 4.10; E058 (Citation resolution algorithm).

---

## 4. Error Handling

### 4.1 Partial Failures

| Scenario | Behavior |
|---|---|
| **PersistRulingStep fails** | Rollback transaction. Do not proceed. Move message to DLQ after retries. |
| **UploadBlobStep fails** | Fail. DLQ. (Phase 1: blob should exist.) |
| **GenerateEmbeddingsStep fails** | Fail. DLQ. Do not partially index. |
| **IndexSearchStep fails** | Fail. DLQ. Ruling is in SQL but not searchable; consider compensating (mark Status=error) or retry. |
| **ResolveCitationsStep fails** | Log. Do not fail the pipeline. Citations remain with TargetRulingId=null; resolved in a future indexing run. |

### 4.2 Decision: Atomicity

- **PersistRulingStep**: Must succeed entirely or rollback. No partial ruling.
- **Later step failure**: If PersistRulingStep succeeds but a subsequent step fails, set `Ruling.Status = 'error'`. The message goes to DLQ. On retry, idempotency check finds ContentHash and skips — no duplicate, but the ruling remains incomplete (e.g. not in search).
- **Remediation**: Rulings with Status=error may require manual reindex or a future "reindex pending" job. Phase 1 does not implement automatic retry of partial indexing.

---

## 5. References

- `docs/architecture/legal-ai-ar-architecture.md` — sections 3.4, 4.1–4.12
- `docs/design/f1-2-deduplicacion.md` — Idempotency (E031)
- `docs/design/f1-5-citation-resolution.md` — ResolveCitationsStep algorithm (E058)
- `docs/design/f1-5-chunking.md` — Chunking strategy (E059)
</think>

<｜tool▁calls▁begin｜><｜tool▁call▁begin｜>
StrReplace