# DLQ Error Envelope — Format and Contract

| Field | Value |
|---|---|
| **ID** | E217 |
| **Feature** | F1-15 · Job Visibility |
| **Date** | 2026-03-13 |

---

## Purpose

This document specifies the DLQ envelope format, the `PublishToDlqAsync` contract, requeue extraction logic, and backward compatibility. It serves as the design reference for DLQ Error Storage (WI-1) implementation.

**Reference**: `docs/design/f1-15-job-visibility-dlq-error.md` §3 (superseded by this focused spec).

---

## 1. Problem

When a worker fails after max retries, the message is moved to the DLQ. Currently only the **original message** is published. The error (exception message, type) is logged but not stored with the message. Admin viewing DLQ sees `bodyPreview` but not the failure reason.

---

## 2. Envelope Structure

When publishing to DLQ, wrap the original message in an envelope:

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
| `originalMessage` | object | The original message (CrawlerMessage, ParserMessage, EnrichmentMessage, IndexerMessage) |
| `error` | object | Error metadata |
| `error.message` | string | Exception.Message (truncated to 2000 chars) |
| `error.type` | string | Exception type name (e.g. `GetType().Name`) |
| `error.timestamp` | string | ISO 8601 when failure occurred |

**Truncation**: `error.message` must be truncated to 2000 characters to avoid Azure Storage Queue message size limits (64 KB).

---

## 3. PublishToDlqAsync Contract

### 3.1 Interface

```csharp
Task PublishToDlqAsync<T>(
    string dlqQueueName,
    T originalMessage,
    Exception ex,
    CancellationToken ct = default) where T : class;
```

### 3.2 Behavior

1. Serialize `originalMessage` to JSON.
2. Build envelope: `{ "originalMessage": <serialized>, "error": { "message": ex.Message, "type": ex.GetType().Name, "timestamp": DateTime.UtcNow } }`.
3. Truncate `error.message` to 2000 chars.
4. Publish envelope JSON to DLQ queue.

### 3.3 Workers Using PublishToDlqAsync

| Worker | DLQ Queue | Message Type |
|---|---|---|
| CrawlerWorkerService | crawler-dlq | CrawlerMessage |
| ParserWorkerService | parser-dlq | ParserMessage |
| EnrichmentWorkerService | enrichment-dlq | EnrichmentMessage |
| IndexerWorkerService | indexer-dlq | IndexerMessage |

**IndexerWorker**: Receives raw `msg.Body`. Parse to `IndexerMessage`, then call `PublishToDlqAsync` with parsed message and exception.

---

## 4. Requeue Extraction Logic

`StorageDlqService.RequeueMessageAsync`:

1. Receive message from DLQ.
2. Parse body as JSON.
3. **If envelope format** (has `originalMessage` property):
   - Extract `originalMessage`.
   - Serialize it to JSON.
   - Publish to origin queue (e.g. `csjn-ruling-parser`).
4. **Else** (legacy format):
   - Publish raw body to origin queue as-is.

**Detection**: `JObject.Parse(body).ContainsKey("originalMessage")` or equivalent.

---

## 5. DlqMessageInfo and API Response

When peeking DLQ messages, parse body:

- **If envelope**: Populate `DlqMessageInfo.Error` with `{ Message: error.message, Type: error.type }`.
- **Else**: `Error = null`.

API response (`GET /api/admin/dlq?queue=parser`) includes `error` in each message when available:

```json
{
  "messages": [
    {
      "id": "...",
      "insertedOn": "...",
      "dequeueCount": 3,
      "bodyPreview": "...",
      "error": {
        "message": "Connection timeout to CSJN API",
        "type": "HttpRequestException"
      }
    }
  ]
}
```

---

## 6. Backward Compatibility

| Scenario | Behavior |
|---|---|
| **Existing DLQ messages** (no envelope) | Requeue publishes raw body. Workers process as before. |
| **New DLQ messages** (with envelope) | Requeue extracts `originalMessage`, publishes that. |
| **DlqMessageInfo.error** | Null for legacy messages; populated for new ones. |
| **Frontend** | Display "—" or "N/A" when `error` is null. |

---

## 7. Documented Decisions

| ID | Decision | Justification |
|---|---|---|
| D1 | Envelope format | Store error with message; no schema change to Azure Storage Queues |
| D2 | Requeue extracts originalMessage | Backward compatible; legacy messages still work |
| D3 | Error message truncation 2000 chars | Avoid Storage Queue 64 KB limit |

---

## 8. References

- `docs/design/f1-8-api-admin.md` — DLQ API spec
- `docs/design/f1-15-job-visibility-dlq-error.md` — Full Job Visibility design
