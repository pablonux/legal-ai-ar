# Requeue Document — API Specification

| Field | Value |
|---|---|
| **ID** | E220 |
| **Feature** | F1-15 · Job Visibility |
| **Date** | 2026-03-13 |

---

## Purpose

This document specifies the API for re-executing a single document at any pipeline stage. It enables admins to re-run a failed document from Parser, Enrichment, or Indexer without requiring the message to be in the DLQ.

**Endpoint**: `POST /api/admin/pipeline/requeue-document`

**Dependencies**: F1-16 (Ingestion traceability) for document traceability and reconstruction from DB.

---

## 1. Use Cases

| Scenario | Solution |
|---|---|
| Document in DLQ | Use existing `POST /api/admin/dlq/{queue}/{id}/requeue` — re-runs from that stage. |
| Document failed at enrichment, not in DLQ | `POST /api/admin/pipeline/requeue-document` with `rulingId` + `stage: "enrichment"` — backend reconstructs EnrichmentMessage from Ruling. |
| Re-parse after parser fix | `POST /api/admin/pipeline/requeue-document` with `rulingId` + `stage: "parser"` — backend reconstructs ParserMessage from Ruling (BlobPath, ContentHash, etc.). |
| Re-index after embedding fix | `POST /api/admin/pipeline/requeue-document` with `rulingId` + `stage: "indexer"` — backend reconstructs IndexerMessage from Ruling + related entities. |

---

## 2. Request Schema

### 2.1 Option A — By Message (client provides full payload)

When the client has the message (e.g. from DLQ peek or failed-documents API):

```json
{
  "stage": "parser",
  "message": {
    "sourceId": 1,
    "documentId": "8048522",
    "analysisId": "804852",
    "blobPathPdf": "rulings-pdfs/csjn/8048522.pdf",
    "contentHash": "a1b2c3...",
    "apiMetadata": { ... }
  }
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| stage | string | Yes | `parser`, `enrichment`, or `indexer` |
| message | object | Yes | Full message payload for that stage. Must conform to ParserMessage, EnrichmentMessage, or IndexerMessage schema. |

### 2.2 Option B — By Ruling ID (backend reconstructs)

When the client references an existing ruling in the KB:

```json
{
  "stage": "enrichment",
  "rulingId": "550e8400-e29b-41d4-a716-446655440000"
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| stage | string | Yes | `parser`, `enrichment`, or `indexer` |
| rulingId | string (GUID) | Yes | ID of the Ruling in Azure SQL. Backend reconstructs the message from DB. |

**Mutual exclusivity**: Provide either `message` or `rulingId`, not both.

---

## 3. Stage Validation

| Stage | Valid with | Reconstruction logic |
|---|---|---|
| parser | message, rulingId | **From ruling**: Ruling has SourceId, ExternalId (documentId), AnalysisId, ContentHash, BlobPath. Build ParserMessage. Publish to `{prefix}-parser`. |
| enrichment | message, rulingId | **From ruling**: Load Ruling + FullText. Build ExtractedMetadata from Ruling fields (Summary, Holding, Judges, Keywords, etc.). MissingFields = `["judges","cited_statutes","citation_types"]` or empty for full re-enrichment. Publish to `{prefix}-enrichment`. |
| indexer | message, rulingId | **From ruling**: Load Ruling + Judges, Keywords, Statutes, Citations. Build RulingData, JudgeData[], etc. Regenerate Chunks from FullText (chunking logic). Publish to `{prefix}-indexer`. |

---

## 4. Response

### 4.1 Success (200 OK)

```json
{
  "success": true,
  "stage": "enrichment",
  "messageId": "optional-queue-message-id-if-available"
}
```

### 4.2 Validation Error (400 Bad Request)

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Validation Error",
  "status": 400,
  "detail": "Either 'message' or 'rulingId' must be provided, not both.",
  "errors": {
    "stage": ["Stage must be one of: parser, enrichment, indexer."]
  }
}
```

### 4.3 Not Found (404)

When `rulingId` is provided but Ruling does not exist:

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Not Found",
  "status": 404,
  "detail": "Ruling with id '...' was not found."
}
```

### 4.4 Reconstruction Failure (500)

When reconstruction fails (e.g. missing BlobPath, invalid data):

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "Cannot reconstruct EnrichmentMessage: Ruling has no FullText."
}
```

---

## 5. Implementation Notes

### 5.1 Reconstruction from Ruling

| Stage | Required Ruling fields | Notes |
|---|---|---|
| parser | SourceId, ExternalId, ContentHash, BlobPath | AnalysisId nullable. BlobPath must exist. |
| enrichment | FullText, Summary, Holding, + related | Need to build ExtractedMetadata. MissingFields drives what GPT extracts; use full set for re-enrichment. |
| indexer | All ruling data + Judges, Keywords, Statutes, Citations | Chunks regenerated from FullText using chunking strategy (512 tokens, overlap 50). |

### 5.2 Idempotency

Workers apply normal idempotency (ContentHash check). Re-queued documents may be deduplicated if already indexed. For "force re-index" scenarios, consider a separate flag or endpoint in a future iteration.

### 5.3 Authorization

Requires `admin` role. Use `[Authorize(Roles = "admin")]` on the controller.

---

## 6. Documented Decisions

| ID | Decision | Justification |
|---|---|---|
| D1 | Support both message and rulingId | Message: when available (DLQ). RulingId: when doc exists in KB but failed earlier. |
| D2 | No crawler stage | Re-execution starts at parser or later. Crawler discovers; we don't re-run discovery for a single doc. |
| D3 | Chunks regenerated on indexer reconstruction | Chunking is deterministic; regenerate from FullText. |

---

## 7. References

- `docs/architecture/legal-ai-ar-specs.md` — §6.1 message contracts (ParserMessage, EnrichmentMessage, IndexerMessage)
- `docs/architecture/legal-ai-ar-architecture.md` — §4.1 Rulings table, §4.15 IngestionJobs
- `docs/design/f1-15-job-visibility-ui.md` — RequeueDocument modal placement (E219)
- `docs/design/f1-5-chunking.md` — Chunking strategy for indexer
