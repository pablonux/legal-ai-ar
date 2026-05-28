# Job Visibility, Document Re-execution, and DLQ Error Storage

| Field | Value |
|---|---|
| **ID** | E207 |
| **Feature** | F1-15 · Job Visibility |
| **Date** | 2026-03-13 |

---

## Purpose

This document specifies three enhancements for the ingestion pipeline admin experience:

1. **Job visibility** — Show real-time status: documents discovered, in progress, indexed, failed.
2. **Document re-execution** — Re-run a single document at any pipeline stage (parser, enrichment, indexer).
3. **DLQ error storage** — Persist the error reason when a message is moved to a Dead Letter Queue.

---

## 1. Job Visibility

### 1.1 Current State (Phase 1)

- Jobs are **synthetic** from `CrawlerConfigs`: one job per source = last crawl.
- No `IngestionJobs` table; no document-level tracking.
- Visibility: `LastCrawledAt`, `LastCrawledStatus`, `LastDocumentCount` only.

### 1.2 Target State (Phase 2 with F2-5)

The `IngestionJobs` table (F2-5) provides:

| Field | Description |
|---|---|
| `DocumentsDiscovered` | Total documents found by crawler during discovery |
| `DocumentsIndexed` | Documents successfully persisted in KB |
| `DocumentsFailed` | Documents that failed at some stage |
| `Status` | `running`, `completed`, `partial`, `failed` |

**Running job visibility** requires:

- Crawler creates `IngestionJob` at start (status `running`).
- Crawler updates `DocumentsDiscovered` as discovery progresses (or at end).
- Each pipeline stage updates counters when documents succeed/fail.
- Job closes with final metrics when crawler finishes.

**Dependency**: F2-5 — `IngestionJobs` table and `ingestionJobId` propagation through the pipeline.

### 1.3 Incremental Improvement (Phase 1)

Without full F2-5, we can improve visibility by:

- **Pipeline status**: `GET /api/admin/pipeline/status` already returns `queueLength` per queue.
- **Queue length**: Indicates documents in progress (parser, enrichment, indexer).
- **Crawler**: `LastDocumentCount` = documents published to parser (not discovered total).

**Gap**: No "documents discovered" count until crawler completes. Discovery is streaming; we don't know total until done.

---

## 2. Document Re-execution

### 2.1 Use Case

Admin sees a failed document (e.g. in DLQ or in a job's failed list). They want to re-run that document only, at a specific pipeline stage:

- **Parser**: Re-parse a PDF (e.g. after fixing parser bug).
- **Enrichment**: Re-enrich (e.g. after fixing GPT extraction).
- **Indexer**: Re-index (e.g. after fixing embedding step).

### 2.2 API Design

**Endpoint**: `POST /api/admin/pipeline/requeue-document`

**Request body**:

```json
{
  "stage": "parser",
  "message": { ... }
}
```

| Field | Type | Description |
|---|---|---|
| stage | string | `parser`, `enrichment`, or `indexer` |
| message | object | The full message payload for that stage |

**Alternative** (simpler): `POST /api/admin/dlq/{queue}/{id}/requeue` already exists. Admin can requeue from DLQ; the message is re-processed from that stage.

**New requirement**: Re-execute a document **without** it being in DLQ. E.g. document succeeded at parser but failed at enrichment; we want to re-run from enrichment only.

**Proposed endpoint**: `POST /api/admin/pipeline/requeue-document`

| Request | Description |
|---|---|---|
| `stage: "parser"` | Requires `sourceId`, `documentId`, `blobPathPdf`, `contentHash` (or derive from existing ruling) |
| `stage: "enrichment"` | Requires full `EnrichmentMessage` |
| `stage: "indexer"` | Requires full `IndexerMessage` |

**Implementation**: Validate input, build the appropriate message, publish to `{prefix}-{stage}` (e.g. `csjn-ruling-parser`). No idempotency bypass — the workers will deduplicate by `ContentHash` if needed.

**Simpler approach**: Accept a message ID from DLQ or a document reference. For "re-run from enrichment" we need a way to reconstruct `EnrichmentMessage` — e.g. from a ruling that exists in DB (re-parsed, re-enriched) or from a stored failed message.

**Recommendation**: Phase 1 — DLQ requeue is sufficient for "re-run failed document". Phase 2 — Add `POST /api/admin/pipeline/requeue-document` with `stage` + `documentId` + `sourceId`; backend reconstructs message from DB (e.g. Ruling + Blob path) or from a stored failed message.

---

## 3. DLQ Error Storage

### 3.1 Problem

When a worker fails, it moves the message to the DLQ. Currently we publish the **original message** only. The error (exception message, type) is logged but not stored with the message. Admin viewing DLQ sees `bodyPreview` (first 200 chars of message) but not the error reason.

### 3.2 Solution: Envelope with Error

**Envelope structure** when publishing to DLQ:

```json
{
  "originalMessage": { ... },
  "error": {
    "message": "The actual exception message",
    "type": "CsjnSchemaViolationException",
    "timestamp": "2026-03-13T14:30:00Z"
  }
}
```

| Field | Type | Description |
|---|---|---|
| originalMessage | object | The original message (CrawlerMessage, ParserMessage, EnrichmentMessage, IndexerMessage) |
| error | object | Error metadata |
| error.message | string | Exception.Message (truncated to e.g. 2000 chars) |
| error.type | string | Exception type name |
| error.timestamp | string | ISO 8601 when failure occurred |

### 3.3 Implementation

**New interface** (or extend `IQueuePublisher`):

```csharp
Task PublishToDlqAsync<T>(string dlqQueueName, T originalMessage, Exception ex, CancellationToken ct = default) where T : class;
```

**Behavior**:

1. Serialize `originalMessage` to JSON.
2. Build envelope: `{ "originalMessage": <originalMessage>, "error": { "message": ex.Message, "type": ex.GetType().Name, "timestamp": DateTime.UtcNow } }`.
3. Publish envelope JSON to DLQ queue.

**Requeue** (existing `RequeueMessageAsync`):

1. Receive message from DLQ.
2. Parse body as JSON.
3. If envelope format (has `originalMessage`): extract `originalMessage`, serialize it, publish to origin queue.
4. Else: legacy format — publish raw body to origin (backward compatible).

**DlqMessageInfo** (API response):

Add optional field:

| Field | Type | Description |
|---|---|---|
| error | object \| null | Error info if envelope format |
| error.message | string | Exception message |
| error.type | string | Exception type |

When peeking, parse body; if envelope, populate `error`; else `error: null`.

### 3.4 Workers to Update

| Worker | Current DLQ publish | Change |
|---|---|---|
| CrawlerWorkerService | `PublishAsync(QueueCrawlerDlq, crawlerMessage, ct)` | Use `PublishToDlqAsync(QueueCrawlerDlq, crawlerMessage, ex, ct)` |
| ParserWorkerService | `PublishAsync(QueueParserDlq, parserMessage, ct)` | Use `PublishToDlqAsync(QueueParserDlq, parserMessage, ex, ct)` |
| EnrichmentWorkerService | `PublishAsync(QueueEnrichmentDlq, enrichmentMessage, ct)` | Use `PublishToDlqAsync(QueueEnrichmentDlq, enrichmentMessage, ex, ct)` |
| IndexerWorkerService | `PublishRawAsync(QueueIndexerDlq, msg.Body, ct)` | Parse body to IndexerMessage, use `PublishToDlqAsync(QueueIndexerDlq, indexerMessage, ex, ct)` — or wrap raw body in envelope with error |

**Indexer** uses `msg.Body` (raw string) because it already has the serialized message. We can wrap it: `{ "originalMessage": <parsed JSON>, "error": {...} }` or use a raw envelope `{ "originalMessageRaw": "<escaped body>", "error": {...} }`. Simpler: parse `msg.Body` to `IndexerMessage`, then use same envelope as others.

### 3.5 Backward Compatibility

- **Existing DLQ messages** (no envelope): Requeue works as today — body is the original message, publish as-is.
- **New DLQ messages** (with envelope): Requeue extracts `originalMessage`, publishes that.
- **DlqMessageInfo.error**: Null for legacy messages; populated for new ones.

---

## 4. Summary of Changes

| Area | Change |
|---|---|
| **IQueuePublisher** | Add `PublishToDlqAsync<T>(queue, message, exception, ct)` |
| **StorageQueuePublisher** | Implement envelope serialization |
| **StorageDlqService.RequeueMessageAsync** | Parse body; if envelope, extract and publish `originalMessage` |
| **DlqMessageInfo** | Add optional `Error` property |
| **DlqPeekResult / API** | Expose `error` in DLQ message response |
| **CrawlerWorkerService** | Use `PublishToDlqAsync` with exception |
| **ParserWorkerService** | Use `PublishToDlqAsync` with exception |
| **EnrichmentWorkerService** | Use `PublishToDlqAsync` with exception |
| **IndexerWorkerService** | Parse body, use `PublishToDlqAsync` with exception |
| **Frontend DeadLetterQueueComponent** | Display `error.message` and `error.type` when available |

---

## 5. Documented Decisions

| ID | Decision | Justification |
|---|---|---|
| D1 | Envelope format for DLQ | Store error with message; no schema change to Azure Storage Queues |
| D2 | Requeue extracts originalMessage | Backward compatible; legacy messages (no envelope) still work |
| D3 | Error message truncation | Limit to 2000 chars to avoid Storage Queue message size limits (64 KB) |
| D4 | Document re-execution | Phase 2; depends on IngestionJobs and document traceability |

---

## 6. References

- `docs/architecture/legal-ai-ar-architecture.md` — IngestionJobs table (4.15)
- `docs/design/f1-8-api-admin.md` — DLQ API spec
- `docs/roadmap/ROADMAP.md` — F2-5 trazabilidad (IngestionJobs)
