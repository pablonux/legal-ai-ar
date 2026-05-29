# Admin & Pipeline Operations

> The operational surface for running the ingestion pipeline and the Knowledge Base: admin endpoints,
> job tracking and traceability, the SignalR worker-control hub, DLQ handling, and failure recovery.
>
> This document describes the operations layer as currently implemented. Endpoint paths follow the
> code.

---

## 1. Overview

Operators drive ingestion through an **admin API** (consumed by the SPA admin area) and a **SignalR
hub** that pauses/resumes workers and propagates infrastructure incidents. Everything is correlated by
`IngestionJob`, and per-document progress is tracked stage by stage, so a run can be inspected,
reconciled, and recovered.

---

## 2. Admin API

All routes are under `api/admin/*`.

### Crawlers & jobs

| Method · path | Purpose |
|---------------|---------|
| `GET /crawlers` · `GET /crawlers/{sourceId}` | List / get per-source crawler config |
| `POST /crawlers/{sourceId}/run` | Trigger a crawl (enqueues a `CrawlerMessage`) |
| `GET /jobs` | List ingestion jobs |
| `POST /jobs/thesaurus` | Start a SAIJ thesaurus ingest job |
| `GET /jobs/{id}/metrics` · `/audit` · `/documents` | Job metrics, audit trail, document list |
| `POST /jobs/{id}/retry` | Retry a job |
| `GET /pipeline/status` | Per-source status: last crawl, doc count, **approximate queue length** |

### Pipeline & documents

| Method · path | Purpose |
|---------------|---------|
| `POST /pipeline/requeue-document` | Re-enqueue a single document at its stage |
| `POST /pipeline/bulk-requeue` | Re-enqueue many (validated) |
| `POST /pipeline/backfill-kb-quality` | Backfill KB quality fields |

### DLQ, infra & reprocess

| Method · path | Purpose |
|---------------|---------|
| `GET /dlq` | List dead-letter messages |
| `POST /dlq/{queue}/{id}/requeue` | Requeue one DLQ message to its main queue |
| `GET /infra/storage-probe` | Probe Azure Storage (queues/blob) health |
| `GET /ruling-reprocess` | List ruling-reprocess requests |
| `POST /ruling-reprocess/rulings/{rulingId}` | Enqueue a full reprocess of one ruling (Fetcher → Indexer) |
| `POST /ruling-reprocess/{requestId}/retry` | Retry a reprocess request |
| `POST /workers/{workerType}/pause` · `/resume` | Pause/resume a worker type (see §4) |

User administration lives under `api/admin` as well (`UsersAdminController`).

---

## 3. Job tracking & traceability

| Entity | Role |
|--------|------|
| `IngestionJob` | One ingestion run; every document/entity records its originating `IngestionJobId` |
| `IngestionJobDetail` | Per-entity-type metrics for a job |
| `Document` + `DocumentStatus` | The document moving through the 6 stages |
| `DocumentStageLog` | Per-document, per-stage timings (benchmarking + loss analysis) |
| `EntityAuditLog` | Operations on KB entities, linked to the job |
| `FieldProvenance` | Field-level lineage (source endpoint, inference method, AI model) |

This gives end-to-end traceability: from a job, drill into its documents, their stage timings, the
audit tail, and the provenance of each persisted field (see
[17 — KB Data Model §6](17-kb-data-model.md)).

---

## 4. Worker control (SignalR)

Pipeline workers maintain a SignalR connection to `WorkerControlHub` and join a group per worker type
(`JoinWorkerGroup`). The hub client interface `IWorkerControlClient` exposes:

- `PauseAsync()` / `ResumeAsync()` — operator-initiated pause/resume.
- `InfraDegradedAsync(report)` / `InfraRecoveredAsync(jobId, detail)` — infrastructure incident fan-out.

**Pause/resume flow:** `POST /api/admin/workers/{workerType}/pause` upserts a `WorkerPauseState` row
**and** calls `Clients.Group(workerType).PauseAsync()`, so a restarting worker reads its persisted
state and a running worker reacts immediately. Workers also call `ReportInfraIncident(report)` when
they detect storage/network degradation; the hub fans this out so the API and other workers can react.

**Hub authentication:** the hub is guarded by the `WorkerControlHub` authorization policy
(`WorkerControlHubRequirement` + handler). Workers authenticate with a shared secret header
(`X-Worker-Hub-Key`) that must match `WorkerControl:HubAccessKey`; the hub base URL is
`WorkerControl:ApiBaseUrl`.

---

## 5. DLQ & requeue

Each pipeline stage has a dead-letter queue (`{prefix}-{stage}-dlq`). After `MaxDequeueCount` failures
a message lands there. Operators inspect the DLQ (`GET /dlq`) and requeue individual messages
(`POST /dlq/{queue}/{id}/requeue`) or, at the document level, re-drive documents
(`requeue-document`, `bulk-requeue`, `requeue-missing-pipeline-messages`). DLQ handling is implemented
in worker code (Storage Queues has no native DLQ).

---

## 6. Failure recovery & reconciliation

A set of job commands handle the messy realities of a distributed pipeline (especially after Azure
Storage degradation — see the recovery design references):

| Command | What it does |
|---------|--------------|
| `RecoverJobFromInfra` | Bring a job back to a coherent state after an infra incident |
| `ReconcileJobCounters` | Recompute job counters from actual document state |
| `ReconcileJobProcessingDocuments` | Resolve documents stuck in `Processing` |
| `RequeueFetcherPending` / `RequeueMissingPipelineMessages` | Re-enqueue documents whose queue messages were lost |
| `RewindParserFailedToFetcher` | Send parser-failed documents back to the Fetcher |
| `ReprocessNextFailedDocuments` | Reprocess the next batch of failed documents |
| `ResumeDiscovery` | Resume an interrupted discovery phase |
| `RepairJobAuditTail` | Repair a job's audit tail |
| `ProbeStorageHealth` (`/infra/storage-probe`) | Check queue/blob reachability before acting |

The guiding principle is **idempotent, non-duplicating** recovery: re-runs rely on dedup
(`ExternalId` / `ContentHash`) and upsert/`MergeOrUpload` semantics so nothing is double-counted.

---

## 7. Ruling reprocess queue

For targeted fixes, `RulingReprocessRequest` is an admin queue to fully reprocess a **single** ruling
(Fetcher → Indexer): `EnqueueRulingReprocess` adds a request, `RetryRulingReprocess` retries a failed
one, and `ListRulingReprocessRequests` shows the queue with `RulingReprocessRequestStatus`.

---

## 8. Configuration

| Section | Key | Purpose |
|---------|-----|---------|
| `WorkerControl` | `HubAccessKey` | Shared secret for the worker-control hub (header `X-Worker-Hub-Key`) |
| `WorkerControl` | `ApiBaseUrl` | Hub base URL workers connect to |
| `Worker` | `MaxDequeueCount`, `VisibilityTimeoutMinutes`, … | When a message is dead-lettered; per-stage tuning |
| `Pipeline` | `QueuePrefix` | Queue/DLQ name prefix |

---

## 9. Related documentation

- [14 — CSJN Ruling Ingestion](14-csjn-ruling-ingestion.md) — the pipeline these operations control
- [17 — KB Data Model](17-kb-data-model.md) — `IngestionJob`, `DocumentStageLog`, provenance & audit
- [18 — Frontend Architecture](18-frontend-architecture.md) — the admin SPA area
- [07 — Observability & LLMOps](07-observability-llmops.md) — tracing and operational metrics

---

*Admin & Pipeline Operations — Legal Ai Ar*
