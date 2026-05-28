# Work Item — Document Re-execution

**Created**: 2026-03-13  
**Status**: Draft  
**Feature**: [F1-15 Job Visibility](./FEATURE-f1-15-job-visibility.md)

## User Story

As an **admin**, I want **to re-run a single document at a specific pipeline stage (parser, enrichment, or indexer)** so that **I can retry failed documents without re-running the entire crawl**.

## Context

- **Current capability**: `POST /api/admin/dlq/{queue}/{id}/requeue` — requeue from DLQ. Works for documents already in DLQ.
- **Gap**: Re-run a document that is **not** in DLQ. E.g. document succeeded at parser but failed at enrichment; admin wants to re-run from enrichment only.
- **Dependency**: F2-5 — document traceability (reconstruct `EnrichmentMessage` or `IndexerMessage` from DB/Ruling).
- **Design reference**: `docs/design/f2-6-job-visibility-dlq-error.md` §2.

## Acceptance Criteria

1. **API**: `POST /api/admin/pipeline/requeue-document` accepts `stage` (`parser` | `enrichment` | `indexer`) and document reference (`documentId`, `sourceId`, or message payload).
2. **Backend**: Validates input; builds appropriate message (ParserMessage, EnrichmentMessage, IndexerMessage); publishes to `{prefix}-{stage}` (e.g. `csjn-ruling-parser`).
3. **Reconstruction**: For enrichment/indexer, reconstruct message from DB (Ruling, Blob path) when document exists; or accept full message payload for DLQ-origin scenarios.
4. **Frontend**: Jobs or DLQ view offers "Re-ejecutar documento" action that calls the new endpoint (e.g. from job's failed list or from ruling detail).

## Out of Scope

- Bypassing idempotency (workers will deduplicate by ContentHash as today)
- Re-executing from crawler stage (crawler operates at source level, not document level)

## Notes

- Phase 2 deliverable — depends on IngestionJobs and document traceability (F2-5).
- Simpler path: accept `documentId` + `sourceId` + `stage`; backend fetches Ruling/Blob and builds message. Alternative: accept full message JSON for flexibility.
