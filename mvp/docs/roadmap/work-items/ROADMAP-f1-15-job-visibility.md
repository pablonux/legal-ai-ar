# Implementation Roadmap — Job Visibility (F1-15)

**Source**: WI-2026-03-13-dlq-error-storage, WI-2026-03-13-job-visibility, WI-2026-03-13-document-reexecution  
**Feature**: F1-15 · Job Visibility  
**Date**: 2026-03-13 · **For**: Manager to merge into ROADMAP.md

---

## Analysis Summary

Feature Job Visibility comprises three Work Items with different dependencies:

| WI | Scope | Dependency |
|----|-------|------------|
| **DLQ Error Storage** | Persist and display error when message goes to DLQ | None — implementable in Phase 1 |
| **Job Visibility** | Real-time job status (discovered, indexed, failed) | F1-16 (IngestionJobs) |
| **Document Re-execution** | Re-run single document at any pipeline stage | F1-16 (trazabilidad) |

**Recommended sequencing**:
1. **DLQ Error Storage** — First (no deps). Documenter → Designer → Developer.
2. **Job Visibility** — After F2-5 closure. Documenter extends F2-5 docs; Designer updates Jobs mockup; Developer implements.
3. **Document Re-execution** — After F2-5 closure. Documenter → Designer → Developer.

**Impact**: IQueuePublisher, StorageDlqService, 4 workers, DlqMessageInfo API, DeadLetterQueueComponent; IngestionJobs (F2-5); new RequeueDocument endpoint and UI.

---

## ROADMAP Merge Block

Insert after F1-14 (before `## Phase 2 — Expansion`):

```markdown
---

### F1-15 · Job Visibility

**Objective**: Improve admin visibility of ingestion pipeline: DLQ error display, job metrics, and document re-execution.

**Dependencies**: WI-1 (DLQ Error) has no deps. WI-2 and WI-3 require IngestionJobs (F1-16 trazabilidad).

#### T-00 · Design and documentation

| # | Design deliverable | Owner | DEV | AUD |
|---|---|:---:|:---:|:---:|
| E217 | `docs/design/f1-15-dlq-error-envelope.md` — DLQ envelope format, `PublishToDlqAsync` contract, requeue extraction logic, backward compatibility | Documenter | `[x]` | `[ ]` |
| E218 | `docs/design/f1-15-dlq-error-flow.mermaid` — sequence: worker fails → envelope publish → DLQ peek with error → requeue extracts originalMessage | Documenter | `[x]` | `[ ]` |
| E219 | `docs/design/f1-15-job-visibility-ui.md` — UX spec: DLQ error column (message, type), Jobs metrics (discovered, indexed, failed), RequeueDocument action placement | Documenter | `[x]` | `[ ]` |
| E220 | `docs/design/f1-15-requeue-document-api.md` — API spec for `POST /api/admin/pipeline/requeue-document`: request schema, stage validation, message reconstruction from DB | Documenter | `[x]` | `[ ]` |

#### Designer deliverables

| # | Design deliverable | Owner | DEV | AUD |
|---|---|:---:|:---:|:---:|
| E221 | `docs/mockups/mockup-admin-dlq-error.html` — DLQ table with Error column (message, type), expandable or tooltip | Designer | `[x]` | `[ ]` |
| E222 | `docs/mockups/mockup-admin-jobs-metrics.html` — Jobs table with full metrics (discovered, indexed, failed, status), link to DLQ | Designer | `[x]` | `[ ]` |
| E223 | `docs/mockups/mockup-admin-requeue-document.html` — Requeue document action: from Jobs failed list or DLQ, stage selector, confirm flow | Designer | `[x]` | `[ ]` |

#### Development tasks — WI-1: DLQ Error Storage

- [x] **T-01** Add `PublishToDlqAsync<T>(queue, message, exception, ct)` to `IQueuePublisher`; implement in `StorageQueuePublisher` (envelope, error truncation 2000 chars)
- [x] **T-02** Update `StorageDlqService.RequeueMessageAsync`: parse body; if envelope, extract `originalMessage` and publish; else publish raw (legacy)
- [x] **T-03** Extend `DlqMessageInfo` with optional `Error` (message, type); update `StorageDlqService.PeekMessagesAsync` to parse envelope and populate
- [x] **T-04** Update CrawlerWorkerService, ParserWorkerService, EnrichmentWorkerService, IndexerWorkerService to use `PublishToDlqAsync` on failure
- [x] **T-05** Update `DeadLetterQueueComponent` to display `error.message` and `error.type` when available

#### Development tasks — WI-2: Job Visibility *(blocked until F1-16)*

- [ ] **T-06** CrawlerWorkerService: create `IngestionJob` at start (status `running`); update `DocumentsDiscovered` at end; close job with metrics
- [ ] **T-07** Parser, Enrichment, Indexer workers: update job counters (`DocumentsIndexed`, `DocumentsFailed`) when processing messages with `ingestionJobId`
- [ ] **T-08** Ensure `GetJobsQuery` returns real data from `IngestionJobs` (F1-16 T-05)
- [ ] **T-09** Update `JobsComponent` with full metrics: discovered, indexed, failed, status, timestamps

#### Development tasks — WI-3: Document Re-execution *(blocked until F1-16)*

- [ ] **T-10** Implement `RequeueDocumentCommand` + handler: validate stage, build message from `documentId` + `sourceId` or from payload, publish to queue
- [ ] **T-11** Add `POST /api/admin/pipeline/requeue-document` endpoint
- [ ] **T-12** Add "Re-ejecutar documento" action in JobsComponent (failed list) and/or DeadLetterQueueComponent

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E224 | `PublishToDlqAsync` with envelope; all 4 workers using it | `[x]` | `[ ]` |
| E225 | Requeue extracts `originalMessage` from envelope; legacy format still works | `[x]` | `[ ]` |
| E226 | `DlqMessageInfo` and API expose `error`; DeadLetterQueueComponent displays it | `[x]` | `[ ]` |
| E227 | CrawlerWorkerService creates/closes IngestionJob; pipeline stages update counters | `[ ]` | `[ ]` |
| E228 | JobsComponent shows full history with discovered, indexed, failed metrics | `[ ]` | `[ ]` |
| E229 | `POST /api/admin/pipeline/requeue-document` and "Re-ejecutar documento" action | `[ ]` | `[ ]` |

---

```

---

## Task Ownership Summary

| Role | Deliverables | Tasks |
|------|--------------|-------|
| **Documenter** | E217, E218, E219, E220 | T-00 design docs |
| **Designer** | E221, E222, E223 | Mockups for DLQ error, Jobs metrics, Requeue document |
| **Developer** | E224–E229 | T-01 to T-12 |

---

## Dependencies and Sequencing

```
F1-16 (IngestionJobs) ─┬─► T-06, T-07, T-08, T-09 (Job Visibility)
                      └─► T-10, T-11, T-12 (Document Re-execution)

E217, E218 ─► T-01, T-02, T-03, T-04, T-05 (DLQ Error — no block)
E219 ─► E221, E222, E223 (Designer mockups)
E220 ─► T-10, T-11, T-12 (Requeue document)
```

---

## Summary Update

After merge, update Phase 1 row in Summary table:

| Phase | Features | Design deliverables | Development deliverables | Total deliverables |
|---|:---:|:---:|:---:|:---:|
| Phase 1 — MVP | **+1** (F1-15) | **+7** (E217–E223) | **+6** (E224–E229) | **+13** |

---

## Notes for Manager

- **E217**: Can reference existing `docs/design/f1-15-job-visibility-dlq-error.md` §3; Documenter may extract/refine into focused `f1-15-dlq-error-envelope.md`.
- **E222**: `mockup-admin-jobs.html` exists; E222 is an update to include full metrics (discovered, indexed, failed).
- **T-06–T-09, T-10–T-12**: Blocked until F1-16 (Ingestion traceability) is complete.
