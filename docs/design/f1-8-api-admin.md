# API Admin — Endpoint Specification

| Field | Value |
|---|---|
| **ID** | E083 |
| **Feature** | F1-8 · API — Administración |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies all admin API endpoints: request/response schemas, validations, crawler states, and DLQ message lifecycle. It serves as the design reference for `CrawlersAdminController`, `JobsAdminController`, `DlqAdminController`, `UsersAdminController` and `HealthController`.

**Reference**: Architecture section 5 (API); specs section 7; ADR-009 (Storage Queues).

---

## 1. Crawlers

### 1.1 GET /api/admin/crawlers

List all crawler configurations with status.

**Query parameters**: None (or optional `sourceId` for single crawler — see 1.2).

**Response (200 OK)**:

```json
[
  {
    "sourceId": 1,
    "sourceName": "CSJN",
    "isEnabled": true,
    "lastCrawledAt": "2024-03-15T14:30:00Z",
    "lastCrawledStatus": "success",
    "lastDocumentCount": 42
  }
]
```

| Field | Type | Description |
|---|---|---|
| sourceId | int | FK to Sources (1=CSJN, 2=SAIJ, 3=PJN, 4=SCBA) |
| sourceName | string | Short name from Sources |
| isEnabled | bool | From CrawlerConfigs.IsEnabled |
| lastCrawledAt | datetime? | ISO 8601. Null if never executed |
| lastCrawledStatus | string? | `success`, `partial`, `failed`. Null if never executed |
| lastDocumentCount | int? | Documents processed in last crawl. Null if never executed |

---

### 1.2 GET /api/admin/crawlers/{sourceId}

Configuration and status of a specific crawler.

**Path parameter**: `sourceId` (int, 1–4).

**Response (200 OK)**: Same schema as single element in 1.1 array.

**Errors**: 404 if sourceId not found or no CrawlerConfig for that source.

---

### 1.3 PATCH /api/admin/crawlers/{sourceId}

Update crawler configuration. Phase 1: only `isEnabled`.

**Request body**:

```json
{
  "isEnabled": true
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| isEnabled | bool | Yes | Enable or disable the source for crawling |

**Response (200 OK)**: Updated crawler (same schema as 1.2).

**Errors**: 400 if validation fails; 404 if sourceId not found.

---

### 1.4 POST /api/admin/crawlers/{sourceId}/run

Trigger manual crawl.

**Request body (incremental)**:

```json
{
  "type": "incremental",
  "since": "2024-01-01"
}
```

**Request body (by-range)**:

```json
{
  "type": "by-range",
  "dateFrom": "2024-03-01",
  "dateTo": "2024-03-31"
}
```

| Field | Type | Required | Default | Description |
|---|---|---|---|---|
| type | string | Yes | — | `incremental` or `by-range` |
| since | date | No | LastCrawledAt (incremental) | ISO 8601 (YYYY-MM-DD). Only for incremental. |
| dateFrom | date | Yes (by-range) | — | Start of date range. Required when type is by-range. |
| dateTo | date | Yes (by-range) | — | End of date range. Required when type is by-range. |

**Validation**:
- `type` must be `incremental` or `by-range`
- `since` required only when `type` is `incremental` and no `LastCrawledAt` exists
- `dateFrom` and `dateTo` required when `type` is `by-range`; `dateFrom` must be ≤ `dateTo`
- Source must be enabled (`isEnabled: true`)

**Response (200 OK)**:

```json
{
  "success": true,
  "message": "Crawl triggered for source CSJN"
}
```

**Errors**: 400 if source disabled or validation fails; 404 if sourceId not found.

---

### 1.5 Crawler States

| Status | Description |
|---|---|
| `idle` | No crawl in progress. `lastCrawledStatus` reflects last run. |
| `running` | Crawl in progress (CrawlerWorker processing). Phase 1: inferred from queue activity or worker heartbeat if implemented. |
| `success` | Last crawl completed successfully |
| `partial` | Last crawl completed with some failures |
| `failed` | Last crawl failed |

**Phase 1**: `running` is not persisted. Jobs table (IngestionJobs) is Phase 2. Status comes from `CrawlerConfigs.LastCrawledStatus`.

---

## 2. Pipeline Status and Jobs

### 2.1 GET /api/admin/pipeline/status

Pipeline status per source.

**Response (200 OK)**:

```json
{
  "sources": [
    {
      "sourceId": 1,
      "sourceName": "CSJN",
      "lastCrawledAt": "2024-03-15T14:30:00Z",
      "lastCrawledStatus": "success",
      "lastDocumentCount": 42,
      "queueLength": 0
    }
  ]
}
```

| Field | Type | Description |
|---|---|---|
| queueLength | int | Approximate message count in `{prefix}-crawler` (e.g. `csjn-ruling-crawler`). |

**Phase 1**: Data from CrawlerConfigs. Queue metrics from Azure Storage Queue API (`GetPropertiesAsync` → `ApproximateMessagesCount`). Single crawler queue; `queueLength` is global.

---

### 2.2 GET /api/admin/jobs

Active, completed and failed jobs.

**Phase 1**: No IngestionJobs table. Options:
- Return empty array `[]`
- Return synthetic jobs from CrawlerConfigs (one "job" per source = last crawl)

**Response (200 OK)** (Phase 1 — synthetic):

```json
[
  {
    "id": "synthetic-1",
    "sourceId": 1,
    "sourceName": "CSJN",
    "type": "incremental",
    "triggeredBy": "admin",
    "startedAt": "2024-03-15T14:00:00Z",
    "completedAt": "2024-03-15T14:30:00Z",
    "status": "completed",
    "documentsDiscovered": 50,
    "documentsIndexed": 42,
    "documentsFailed": 8
  }
]
```

**Phase 2**: Real data from IngestionJobs. Phase 1 implementation may use CrawlerConfigs to build a minimal view or return `[]`.

---

## 3. Dead Letter Queue (DLQ)

**Phase 1**: Azure Storage Queues. No native DLQ. Custom DLQ queues: `{prefix}-{stage}-dlq` (e.g. `csjn-ruling-crawler-dlq`, `csjn-ruling-parser-dlq`).

### 3.1 GET /api/admin/dlq

List messages in a DLQ.

**Query parameters**:

| Parameter | Type | Required | Description |
|---|---|---|---|
| queue | string | Yes | `crawler`, `parser`, `enrichment`, `indexer` |

**Validation**: `queue` must be one of the four values.

**Response (200 OK)**:

```json
{
  "queue": "parser",
  "messageCount": 3,
  "messages": [
    {
      "id": "msg-uuid-1",
      "insertedOn": "2024-03-15T10:00:00Z",
      "dequeueCount": 3,
      "bodyPreview": "{\"documentId\":\"8048522\",\"sourceId\":1,...}"
    }
  ]
}
```

| Field | Type | Description |
|---|---|---|
| id | string | Azure Storage Queue message ID (MessageId or base64) |
| insertedOn | datetime | When message was added to DLQ |
| dequeueCount | int | Delivery attempts (typically 3 when in DLQ) |
| bodyPreview | string | First 200 chars of message body (truncated) |

**Implementation**: Use `QueueClient` for `{prefix}-{stage}-dlq`. `PeekMessagesAsync` or `ReceiveMessagesAsync` (with visibility timeout 0 for peek-only). Max 32 messages per request (Storage Queues limit).

---

### 3.2 POST /api/admin/dlq/{queue}/{id}/requeue

Requeue a message from DLQ to the origin queue.

**Path parameters**:

| Parameter | Type | Description |
|---|---|---|
| queue | string | `crawler`, `parser`, `enrichment`, `indexer` |
| id | string | Message ID (as returned by GET /api/admin/dlq) |

**Request body**: None.

**Response (200 OK)**:

```json
{
  "success": true,
  "message": "Message requeued to csjn-ruling-parser"
}
```

**Implementation**:
1. Receive message from `{prefix}-{stage}-dlq` by ID (or iterate and match).
2. Publish message body to origin queue `{prefix}-{stage}`.
3. Delete message from DLQ.

**Errors**: 404 if message not found; 500 if publish fails.

---

### 3.3 DLQ Message Lifecycle

```
Origin queue (e.g. csjn-ruling-parser)
    │
    │  Worker fails processing (max 3 attempts)
    ▼
DLQ (csjn-ruling-parser-dlq)
    │
    │  Admin calls POST .../requeue
    ▼
Origin queue (message re-enters pipeline)
```

**Note**: Requeued messages are processed again from scratch. No retry count reset in Storage Queues (message is new in origin queue).

---

## 4. Users

**Phase 1**: Users table for admin CRUD. Phase 3: sync with Entra ID. Minimal schema for F1-8.

### 4.1 Users Table (Assumed)

| Field | Type | Description |
|---|---|---|
| Id | UNIQUEIDENTIFIER PK | UUID |
| Email | NVARCHAR(256) | Unique. Login identifier |
| DisplayName | NVARCHAR(200) | Optional |
| Role | VARCHAR(20) | `admin`, `lawyer`, `viewer` |
| IsActive | BIT | Soft delete |
| CreatedAt | DATETIME | |
| UpdatedAt | DATETIME | |

⚠️ **ASSUMPTION**: Users table schema not explicitly in architecture. This schema supports Phase 1 admin CRUD. Phase 3 will integrate with Entra ID.

---

### 4.2 GET /api/admin/users

List all users.

**Response (200 OK)**:

```json
[
  {
    "id": "uuid",
    "email": "user@firm.com",
    "displayName": "Juan Pérez",
    "role": "admin",
    "isActive": true
  }
]
```

---

### 4.3 POST /api/admin/users

Create user.

**Request body**:

```json
{
  "email": "user@firm.com",
  "displayName": "Juan Pérez",
  "role": "admin"
}
```

| Field | Type | Required | Validation |
|---|---|---|---|
| email | string | Yes | Valid email format, unique |
| displayName | string | No | Max 200 chars |
| role | string | Yes | `admin`, `lawyer`, `viewer` |

**Response (201 Created)**: Created user (same schema as 4.2 element). Location header: `/api/admin/users/{id}`.

**Errors**: 400 if validation fails or email duplicate.

---

### 4.4 PUT /api/admin/users/{id}

Update user (display name, role).

**Request body**:

```json
{
  "displayName": "Juan Pérez",
  "role": "lawyer"
}
```

| Field | Type | Required |
|---|---|---|
| displayName | string | No |
| role | string | Yes |

**Response (200 OK)**: Updated user.

**Errors**: 400 if validation fails; 404 if user not found.

---

### 4.5 DELETE /api/admin/users/{id}

Deactivate user (soft delete).

**Request body**: None.

**Response (204 No Content)**.

**Implementation**: Set `IsActive = false`. Do not physically delete.

**Errors**: 404 if user not found.

---

## 5. Health Check

### 5.1 GET /api/health

Health check. **Public** (no authentication).

**Response (200 OK)**:

```json
{
  "status": "Healthy",
  "checks": {
    "sql": "Healthy",
    "blob": "Healthy",
    "search": "Unhealthy"
  }
}
```

| Field | Description |
|---|---|
| status | `Healthy` if all checks pass; `Degraded` if some fail; `Unhealthy` if critical (e.g. SQL) fails |
| checks | Per-dependency status |

**Checks**:
- **sql**: Azure SQL connection (e.g. `SELECT 1`)
- **blob**: Azure Blob Storage (e.g. list containers or get container properties)
- **search**: Azure AI Search (e.g. get index stats)

**Response codes**: 200 for Healthy/Degraded; 503 for Unhealthy (optional; some implementations return 200 with status field).

---

## 6. Error Responses

All endpoints use RFC 7807 ProblemDetails for errors.

| HTTP | Scenario |
|---|---|
| 400 | Validation error (body or query params) |
| 401 | Unauthorized (no/invalid token) |
| 404 | Resource not found (sourceId, user, DLQ message) |
| 500 | Internal server error |

---

## 7. References

- `docs/architecture/legal-ai-ar-architecture.md` — section 5 (API), 4.14 (CrawlerConfigs), 8 (Messaging)
- `docs/design/f1-1-infrastructure.md` — Storage Queues, queue names
- `docs/setup/azure-storage-queues-setup.md` — DLQ queue names
