# Deduplication Strategy — SHA-256 Content Hash

| Field | Value |
|---|---|
| **ID** | E031 |
| **Feature** | F1-2 · Pipeline — CrawlerWorker (CSJN) |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the SHA-256 deduplication strategy for the ingestion pipeline: when to compute the hash, what content to include, and how to query for duplicates. It serves as the design reference for CrawlerWorker deduplication (T-03) and IndexerWorker idempotency (F1-5 T-02). It is consumed by developers implementing `CsjnCrawlerSource`, `CrawlerWorkerService` and `PersistRulingStep`.

---

## 1. When to Compute the Hash

| Stage | Component | Moment |
|---|---|---|
| **CrawlerWorker** | `CsjnCrawlerSource` | Immediately after downloading the PDF from the source, before uploading to Blob Storage and before publishing to `queue-parser`. |
| **IndexerWorker** | `PersistRulingStep` | Before persisting to Azure SQL. The message already contains `contentHash` (computed by CrawlerWorker and propagated via `ParserMessage`). The IndexerWorker re-validates by querying `Rulings` for existence. |

**Rationale**: The CrawlerWorker computes the hash as soon as the PDF is available. This allows skipping duplicate documents before any downstream work (Blob upload, queue publish, parser processing). The IndexerWorker performs a second check for idempotency: if a message is reprocessed from the DLQ or if the CrawlerWorker did not perform the check, the IndexerWorker still avoids creating duplicates.

**Reference**: Architecture section 3.4 (CrawlerWorker); specs section 6.3 (Idempotency).

---

## 2. What to Include in the Hash

| Aspect | Decision |
|---|---|
| **Input** | Raw PDF binary content — the entire file as received from the source. |
| **Algorithm** | SHA-256. |
| **Exclusions** | None. Do not exclude metadata, headers, or any bytes. Hash the complete file. |
| **Normalization** | None. Do not normalize line endings, trim whitespace, or alter the binary stream. |

**Rationale**: Hashing the raw PDF ensures that identical files (same ruling, same scan) produce the same hash regardless of when or how they were downloaded. Different encodings or metadata changes would produce different hashes, which is correct — we only deduplicate exact duplicates.

**Reference**: ADR-005 (Total immutability. Deduplication by SHA-256 of PDF content).

---

## 3. Hash Format and Storage

| Aspect | Specification |
|---|---|
| **Output format** | 64-character hexadecimal string, lowercase (e.g. `a1b2c3d4e5f6...`). |
| **Database column** | `Rulings.ContentHash` — `CHAR(64)` — stores the hex string. |
| **Message propagation** | `ParserMessage.ContentHash`, `EnrichmentMessage` (inherited), `IndexerMessage` (in ruling data). |

---

## 4. How to Query for Duplicates

### 4.1 CrawlerWorker

Before uploading to Blob and publishing `ParserMessage`:

1. Compute SHA-256 of the downloaded PDF bytes.
2. Call `IRulingRepository.ExistsByContentHashAsync(contentHash, cancellationToken)`.
3. If `true` → skip: do not upload to Blob, do not publish to `queue-parser`. Continue to the next document.
4. If `false` → proceed: upload PDF to Blob, publish `ParserMessage` with `contentHash` included.

### 4.2 IndexerWorker

Before persisting the ruling:

1. Read `contentHash` from the incoming message.
2. Call `IRulingRepository.ExistsByContentHashAsync(contentHash, cancellationToken)`.
3. If `true` → discard the message without error. Do not persist, do not index, do not update any store. Log at debug level.
4. If `false` → proceed with the full persistence pipeline.

### 4.3 Repository Contract

```csharp
// LegalAiAr.Core/Interfaces/Repositories/IRulingRepository.cs
Task<bool> ExistsByContentHashAsync(string contentHash, CancellationToken cancellationToken = default);
```

**Implementation**: `SELECT 1 FROM Rulings WHERE ContentHash = @contentHash`. Return `true` if any row exists.

### 4.4 Index

Create a non-clustered index on `Rulings.ContentHash` for fast lookups:

```sql
CREATE NONCLUSTERED INDEX IX_Rulings_ContentHash ON Rulings (ContentHash);
```

**Reference**: Architecture section 4.1 (Rulings table); `ContentHash` is the deduplication key.

---

## 5. Edge Cases

| Scenario | Behavior |
|---|---|
| **Hash collision** | SHA-256 collision is cryptographically negligible. No special handling. |
| **Empty or corrupted PDF** | Hash is computed regardless. If the same corrupted file is re-downloaded, it will deduplicate. Different corruptions produce different hashes. |
| **Case sensitivity** | Store and compare hashes in lowercase. Hex comparison is case-insensitive in practice, but consistency avoids bugs. |
| **Null or missing hash** | The CrawlerWorker always computes the hash before publishing. The IndexerWorker should reject messages without `contentHash` (validation error). |

---

## 6. Documented Decisions

| ID | Decision | Justification |
|---|---|---|
| D1 | Hash raw PDF bytes only | Ensures identical files deduplicate; no false positives from metadata differences. |
| D2 | Two-level check (Crawler + Indexer) | Crawler avoids unnecessary work; Indexer guarantees idempotency on message replay. |
| D3 | Index on ContentHash | Lookups are frequent; index avoids full table scans. |

---

## 7. References

- `docs/architecture/legal-ai-ar-architecture.md` — ADR-005, section 4.1 (Rulings.ContentHash), section 3.4 (CrawlerWorker)
- `docs/architecture/legal-ai-ar-specs.md` — section 6.3 (Idempotency)
- `docs/design/f1-2-crawler.md` — section 4 (Deduplication), section 5 (Flow)
- `docs/design/f1-5-indexer.md` — idempotency policy (E056)
