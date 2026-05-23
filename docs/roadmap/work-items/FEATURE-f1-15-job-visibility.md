# Feature — Job Visibility

**Created**: 2026-03-13  
**Status**: Draft  
**Type**: Admin pipeline enhancements  
**Design Reference**: `docs/design/f1-15-job-visibility-dlq-error.md`

## Summary

Three enhancements for the ingestion pipeline admin experience: (1) persist and display the error reason when messages go to DLQ, (2) improve job visibility with real-time status (documents discovered, indexed, failed), and (3) allow re-execution of a single document at any pipeline stage.

## Scope

- **In scope**: DLQ error envelope and UI display; job visibility (depends on F2-5 IngestionJobs); document re-execution endpoint
- **Out of scope**: Editing pipeline config from UI; automatic retry policies

## Dependencies

| WI | Depends on |
|----|------------|
| WI-2026-03-13-dlq-error-storage | None — implementable in Phase 1 |
| WI-2026-03-13-job-visibility | F2-5 (IngestionJobs table, ingestionJobId propagation) |
| WI-2026-03-13-document-reexecution | F2-5 (trazabilidad, document traceability) |

## Work Items

| WI | Title | Status |
|----|-------|--------|
| [WI-2026-03-13-dlq-error-storage](./WI-2026-03-13-dlq-error-storage.md) | DLQ Error Storage — Persist and display error when message goes to DLQ | Draft |
| [WI-2026-03-13-job-visibility](./WI-2026-03-13-job-visibility.md) | Job Visibility — Real-time status (documents discovered, indexed, failed) | Draft |
| [WI-2026-03-13-document-reexecution](./WI-2026-03-13-document-reexecution.md) | Document Re-execution — Re-run single document at any pipeline stage | Draft |

## Source of Truth

`docs/design/f1-15-job-visibility-dlq-error.md` — technical spec, envelope format, API design, backward compatibility.
