# Ingestion Traceability — IngestionJob Lifecycle

| Field | Value |
|---|---|
| **ID** | E154 |
| **Feature** | F1-16 · Ingestion traceability |
| **Date** | 2026-03-13 |

---

## Purpose

This document specifies the `IngestionJob` lifecycle, states, counters, `ingestionJobId` propagation through the four pipeline messages, and traceability queries. It serves as the design reference for F1-16 development tasks (T-01 to T-06) and unblocks F1-15 Job Visibility (WI-2, WI-3).

**References**: `docs/architecture/legal-ai-ar-architecture.md` §4.15; `docs/architecture/legal-ai-ar-specs.md` §5.7.

---

## 1. IngestionJobs Table Schema

| Field | Type | Description |
|---|---|---|
| `Id` | UNIQUEIDENTIFIER PK | UUID generated when job starts |
| `SourceId` | INT FK | Crawled source (CSJN=1, SAIJ=2, PJN=3, SCBA=4) |
| `Type` | VARCHAR(20) | `incremental`, `full` |
| `TriggeredBy` | VARCHAR(100) | Admin user who triggered it, or `scheduler` (Phase 2) |
| `StartedAt` | DATETIME | Start timestamp |
| `CompletedAt` | DATETIME nullable | Completion timestamp. Null if still in progress. |
| `Status` | VARCHAR(20) | `running`, `completed`, `partial`, `failed` |
| `DocumentsDiscovered` | INT | New documents detected by CrawlerWorker |
| `DocumentsIndexed` | INT | Documents successfully persisted in KB |
| `DocumentsFailed` | INT | Documents that failed at some pipeline stage |
| `ErrorSummary` | NVARCHAR(MAX) nullable | Error summary if `Status` is `partial` or `failed` |

**Rulings table** — additional column:

| Field | Type | Description |
|---|---|---|
| `IngestionJobId` | UNIQUEIDENTIFIER FK nullable | FK to IngestionJobs. Null for legacy rulings or if job unknown. |

---

## 2. Job Lifecycle and States

### 2.1 State Transitions

```
                    ┌─────────────────────────────────────────────────────────┐
                    │                    CrawlerWorker                         │
                    └─────────────────────────────────────────────────────────┘
                                         │
     Admin triggers crawl                │
     (POST /api/admin/crawlers/{id}/run)  │
              │                           │
              ▼                           │
     ┌─────────────────┐                  │
     │ Create IngestionJob                │
     │ Status: running                    │
     │ Id: new GUID                       │
     └────────┬────────┘                  │
              │                           │
              │ Publish CrawlerMessage    │
              │ with ingestionJobId      │
              ▼                           │
     ┌─────────────────┐                  │
     │ Discovery loop  │                  │
     │ (Selenium)      │                  │
     └────────┬────────┘                  │
              │                           │
              │ For each new document:    │
              │ Publish ParserMessage     │
              │ with ingestionJobId       │
              │                           │
              │ At end:                   │
              │ Update DocumentsDiscovered│
              │ Close job (Status update)  │
              ▼                           │
     ┌─────────────────┐                  │
     │ completed /     │                  │
     │ partial / failed│                  │
     └─────────────────┘                  │
```

### 2.2 Status Values

| Status | Description |
|---|---|
| `running` | Crawl in progress. CrawlerWorker has created the job and is discovering or publishing messages. |
| `completed` | Crawl finished successfully. All discovered documents processed (indexed or failed). |
| `partial` | Crawl finished with some failures. `DocumentsFailed` > 0. |
| `failed` | Crawl failed (e.g. discovery error, worker crash). `ErrorSummary` populated. |

### 2.3 Counter Semantics

| Counter | Updated by | When |
|---|---|---|
| `DocumentsDiscovered` | CrawlerWorkerService | At end of discovery loop — count of new documents published to `queue-parser` |
| `DocumentsIndexed` | Parser, Enrichment, Indexer workers | When a document is successfully persisted in KB (PersistRulingStep completes) |
| `DocumentsFailed` | Parser, Enrichment, Indexer workers | When a message is sent to DLQ (after max retries) |

**Note**: `DocumentsIndexed` and `DocumentsFailed` are updated by workers that process messages carrying `ingestionJobId`. Idempotency (ContentHash already exists) counts as indexed if the ruling was first created by this job; otherwise the worker may skip without updating counters. ⚠️ **ASSUMPTION**: On idempotency skip, we do not increment `DocumentsIndexed` (ruling pre-existed). Only net-new indexed rulings from this job increment the counter.

---

## 3. ingestionJobId Propagation

### 3.1 Message Contract Extensions

Each pipeline message must include an optional `IngestionJobId` (or `ingestionJobId` in JSON) property:

| Message | Queue | New Property |
|---|---|---|
| CrawlerMessage | queue-crawler | N/A — job created by CrawlerWorker before publishing |
| ParserMessage | queue-parser | `IngestionJobId` (Guid?) |
| EnrichmentMessage | queue-enrichment | `IngestionJobId` (Guid?) |
| IndexerMessage | queue-indexer | `IngestionJobId` (Guid?) |

**Flow**:
1. CrawlerWorker creates `IngestionJob` at start; obtains `job.Id`.
2. CrawlerWorker publishes `ParserMessage` with `IngestionJobId = job.Id` for each discovered document.
3. ParserWorker receives message, processes, publishes `EnrichmentMessage` with same `IngestionJobId`.
4. EnrichmentWorker receives message, processes, publishes `IndexerMessage` with same `IngestionJobId`.
5. IndexerWorker receives message; `PersistRulingStep` saves `Ruling.IngestionJobId = message.IngestionJobId`.

### 3.2 Counter Updates

Workers that process messages with `ingestionJobId` must update the job row:

| Worker | On success | On failure (DLQ) |
|---|---|---|
| ParserWorkerService | Increment `DocumentsIndexed` | Increment `DocumentsFailed` |
| EnrichmentWorkerService | Increment `DocumentsIndexed` | Increment `DocumentsFailed` |
| IndexerWorkerService | Increment `DocumentsIndexed` | Increment `DocumentsFailed` |

**Implementation**: Use `IIngestionJobRepository` (or equivalent) with `IncrementDocumentsIndexedAsync(jobId)` and `IncrementDocumentsFailedAsync(jobId)`. Use optimistic concurrency or atomic SQL (`UPDATE IngestionJobs SET DocumentsIndexed = DocumentsIndexed + 1 WHERE Id = @id`) to avoid race conditions.

**CrawlerWorker**: At end of discovery, set `DocumentsDiscovered` = count of ParserMessages published. Set `CompletedAt`, `Status` (`completed` / `partial` / `failed`), and optionally `ErrorSummary`.

---

## 4. Traceability Queries

### 4.1 Which job originated this ruling?

```sql
SELECT j.*
FROM IngestionJobs j
JOIN Rulings r ON r.IngestionJobId = j.Id
WHERE r.Id = @rulingId;
```

### 4.2 Which rulings did job X index?

```sql
SELECT *
FROM Rulings
WHERE IngestionJobId = @jobId
ORDER BY CreatedAt;
```

### 4.3 How many rulings did each CSJN job generate in the last month?

```sql
SELECT j.Id, j.StartedAt, j.DocumentsIndexed, j.DocumentsFailed
FROM IngestionJobs j
WHERE j.SourceId = 1
  AND j.StartedAt >= DATEADD(month, -1, GETUTCDATE())
ORDER BY j.StartedAt DESC;
```

### 4.4 Jobs with failures (for admin review)

```sql
SELECT j.Id, j.SourceId, j.StartedAt, j.CompletedAt, j.DocumentsDiscovered,
       j.DocumentsIndexed, j.DocumentsFailed, j.ErrorSummary
FROM IngestionJobs j
WHERE j.Status IN ('partial', 'failed')
ORDER BY j.CompletedAt DESC;
```

---

## 5. API — GET /api/admin/jobs

With F1-16, `GetJobsQuery` returns real data from `IngestionJobs` instead of synthetic jobs from CrawlerConfigs.

**Response fields** (per job):

| Field | Type | Description |
|---|---|---|
| id | string (GUID) | Job ID |
| sourceId | int | Source FK |
| sourceName | string | Display name (CSJN, SAIJ, etc.) |
| type | string | `incremental`, `full` |
| triggeredBy | string | Admin user or `scheduler` |
| startedAt | string (ISO 8601) | Start time |
| completedAt | string \| null | Completion time |
| status | string | `running`, `completed`, `partial`, `failed` |
| documentsDiscovered | int | |
| documentsIndexed | int | |
| documentsFailed | int | |
| errorSummary | string \| null | |

**Reference**: `docs/design/f1-8-api-admin.md` §2.2.

---

## 6. Documented Decisions

| ID | Decision | Justification |
|---|---|---|
| D1 | Job created at crawl start | Single source of truth; CrawlerWorker owns job lifecycle |
| D2 | ingestionJobId in all pipeline messages | Enables traceability and counter updates by each worker |
| D3 | Atomic counter updates | Avoid race conditions when multiple workers process messages concurrently |
| D4 | Rulings.IngestionJobId nullable | Legacy rulings and edge cases may have no job |

---

## 7. References

- `docs/architecture/legal-ai-ar-architecture.md` — §4.15 IngestionJobs table
- `docs/architecture/legal-ai-ar-specs.md` — §5.7 IngestionJobs, §6.1 message contracts
- `docs/design/f1-8-api-admin.md` — GET /api/admin/jobs
- `docs/design/f1-2-crawler.md` — CrawlerWorker flow
