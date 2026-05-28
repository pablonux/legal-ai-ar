# Work Item — Job Visibility

**Created**: 2026-03-13  
**Status**: Draft  
**Feature**: [F1-15 Job Visibility](./FEATURE-f1-15-job-visibility.md)

## User Story

As an **admin**, I want **to see real-time job status including documents discovered, in progress, indexed, and failed** so that **I can monitor ingestion progress and identify issues early**.

## Context

- **Current state (Phase 1)**: Jobs are synthetic from `CrawlerConfigs` — one job per source = last crawl. No `IngestionJobs` table; no document-level tracking.
- **Target state**: `IngestionJobs` table (F2-5) with `DocumentsDiscovered`, `DocumentsIndexed`, `DocumentsFailed`, `Status` (`running`, `completed`, `partial`, `failed`).
- **Dependency**: F2-5 — `IngestionJobs` table, `ingestionJobId` propagation through pipeline messages, `Rulings.IngestionJobId` FK.
- **Design reference**: `docs/design/f2-6-job-visibility-dlq-error.md` §1.

## Acceptance Criteria

1. **Crawler**: Creates `IngestionJob` at start (status `running`); updates `DocumentsDiscovered` as discovery progresses or at end.
2. **Pipeline stages**: Each stage (Parser, Enrichment, Indexer) updates job counters when documents succeed or fail.
3. **Crawler**: Closes job with final metrics (`DocumentsIndexed`, `DocumentsFailed`, `Status`) when crawl completes.
4. **API**: `GET /api/admin/jobs` returns real data from `IngestionJobs` (history, metrics per job).
5. **Frontend**: `JobsComponent` shows complete history with metrics: discovered, indexed, failed, status, timestamps.

## Out of Scope

- Phase 1 incremental improvements without F2-5 (e.g. queue length only)
- Automatic job cancellation or pause

## Notes

- This WI assumes F2-5 (trazabilidad) is implemented. Architect should sequence after F2-5 tasks.
- Discovery is streaming; `DocumentsDiscovered` may be updated at end of crawl unless crawler emits progress events.
