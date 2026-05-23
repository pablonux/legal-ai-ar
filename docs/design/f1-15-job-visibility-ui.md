# Job Visibility — UX Specification

| Field | Value |
|---|---|
| **ID** | E219 |
| **Feature** | F1-15 · Job Visibility |
| **Date** | 2026-03-13 |

---

## Purpose

This document specifies the UX enhancements for Job Visibility: DLQ error column (message, type), Jobs metrics (discovered, indexed, failed), and RequeueDocument action placement. It extends `docs/design/f1-12-admin-ui.md` and serves as the design reference for F1-15 UI deliverables.

**Dependencies**: F1-16 (Ingestion traceability) for full Jobs metrics and RequeueDocument.

---

## 1. DLQ — Error Column

### 1.1 Current State

DeadLetterQueueComponent shows: ID, Inserted, Dequeue Count, Body Preview, Actions (Reencolar).

### 1.2 Enhancement (WI-1 — Implemented)

Add **Error** column to the DLQ table.

| Column | Content |
|---|---|
| **Error** | When `message.error` is present: badge with `error.type`, expandable or tooltip with `error.message`. When null (legacy): "—" or "N/A". |

### 1.3 Display Options

| Option | Description |
|---|---|
| **Badge + tooltip** | Badge shows exception type (e.g. `HttpRequestException`). Hover shows full message. |
| **Badge + expandable row** | Badge shows type. Click to expand row with full message text. |
| **Inline truncated** | Show first 80 chars of message; "Ver más" expands. |

**Recommendation**: Badge for type + tooltip for message (simplest). For long messages, truncate in tooltip to ~500 chars with "…".

### 1.4 Badge Styling

- **Color**: Neutral (e.g. `bg-warning` or `bg-danger` light) to indicate error.
- **Text**: `error.type` — use short form if type is long (e.g. `CsjnSchemaViolationException` → "SchemaViolation" or full).

### 1.5 Empty State

When `error` is null (legacy message): Display "—" or "Sin información" in the Error column.

**Reference**: `docs/mockups/mockup-admin-dlq-error.html` (E221).

---

## 2. Jobs — Full Metrics

### 2.1 Current State (Phase 1)

JobsComponent shows synthetic jobs from CrawlerConfigs or empty. Columns: Source, Type, Triggered By, Started, Completed, Status, Discovered, Indexed, Failed, Actions.

### 2.2 Enhancement (WI-2 — After F1-16)

With F1-16, `GET /api/admin/jobs` returns real data from IngestionJobs. JobsComponent displays:

| Column | Description |
|---|---|
| Source | Source name (CSJN, SAIJ, etc.) |
| Type | `incremental`, `full` |
| Triggered By | Admin user or `scheduler` |
| Started | `startedAt` formatted |
| Completed | `completedAt` formatted (or "—" if running) |
| Status | Badge: `running` (blue), `completed` (green), `partial` (yellow), `failed` (red) |
| Discovered | `documentsDiscovered` |
| Indexed | `documentsIndexed` |
| Failed | `documentsFailed` |
| Actions | Link to DLQ (if failed > 0), Re-ejecutar documento (see §3) |

### 2.3 Link to DLQ

When `documentsFailed` > 0, show a link: "Ver en DLQ" or "X fallidos → DLQ". Navigate to `/admin/dlq` with the relevant queue tab pre-selected if possible (e.g. parser-dlq for parser failures). ⚠️ **ASSUMPTION**: Phase 1 — single link to DLQ; per-queue filtering from job may require API extension.

### 2.4 Error Summary

When `status` is `partial` or `failed` and `errorSummary` is present, show a tooltip or expandable section with the summary.

**Reference**: `docs/mockups/mockup-admin-jobs-metrics.html` (E222).

---

## 3. RequeueDocument Action Placement

### 3.1 Entry Points

| Location | Action | Description |
|---|---|---|
| **JobsComponent** | "Re-ejecutar documento" | From a job's failed list. User selects a failed document, chooses stage, confirms. |
| **DeadLetterQueueComponent** | "Reencolar" (existing) | Requeue from DLQ — message goes back to origin queue. No stage selector (re-runs from that stage). |
| **DeadLetterQueueComponent** | "Re-ejecutar desde etapa" (new) | Optional: allow user to choose target stage (parser, enrichment, indexer) when requeuing. |

### 3.2 Recommended Placement

**Primary**: JobsComponent — when viewing a job with `documentsFailed` > 0, show "Re-ejecutar documento" or "Reintentar fallidos". Opens modal to select document (from failed list) and stage.

**Secondary**: DeadLetterQueueComponent — existing "Reencolar" suffices for "re-run from same stage". A separate "Re-ejecutar desde otra etapa" could be added if we support reconstructing messages from DB.

### 3.3 RequeueDocument Modal (JobsComponent)

When user clicks "Re-ejecutar documento" from a job:

1. **Modal title**: "Re-ejecutar documento"
2. **Document selector**: List of failed documents (from job). If API does not return per-document failed list, show "Re-ejecutar todos los fallidos" or require documentId input.
3. **Stage selector**: Dropdown — Parser, Enrichment, Indexer. Default: first stage where document could have failed (or Parser).
4. **Confirm**: "El documento se publicará en la cola {stage} y será procesado de nuevo."
5. **Buttons**: Cancel, Re-ejecutar.

**API**: `POST /api/admin/pipeline/requeue-document` (see E220).

### 3.4 RequeueDocument from DLQ

When document is in DLQ, "Reencolar" already re-runs from that stage. For "re-run from different stage", we need document traceability (reconstruct EnrichmentMessage from Ruling). Defer to Phase 2 or implement with E220.

**Reference**: `docs/mockups/mockup-admin-requeue-document.html` (E223).

---

## 4. Summary of UI Changes

| Component | Change |
|---|---|
| **DeadLetterQueueComponent** | Add Error column (badge + tooltip). Implemented in WI-1. |
| **JobsComponent** | Full metrics (discovered, indexed, failed); link to DLQ; Re-ejecutar documento action. Requires F1-16. |
| **RequeueDocument modal** | New modal in JobsComponent (and optionally DLQ). Requires E220 API. |

---

## 5. References

- `docs/design/f1-12-admin-ui.md` — Base admin UX
- `docs/design/f1-16-trazabilidad.md` — IngestionJob metrics
- `docs/design/f1-15-requeue-document-api.md` — RequeueDocument API (E220)
- `docs/mockups/mockup-admin-dlq-error.html` — DLQ Error mockup (E221)
