# Work Item — DLQ Error Storage

**Created**: 2026-03-13  
**Status**: Draft  
**Feature**: [F1-15 Job Visibility](./FEATURE-f1-15-job-visibility.md)

## User Story

As an **admin**, I want **to see the error reason (message and type) when viewing messages in the Dead Letter Queue** so that **I can understand why a document failed and debug without checking application logs**.

## Context

- **Current state**: Workers publish the original message to DLQ; the exception is logged but not stored with the message. Admin sees only `bodyPreview` (first 200 chars).
- **Solution**: Envelope format — wrap message with `{ "originalMessage": {...}, "error": { "message", "type", "timestamp" } }` when publishing to DLQ.
- **Backward compatibility**: Requeue must handle both envelope format (extract `originalMessage`) and legacy format (publish raw body).
- **Design reference**: `docs/design/f2-6-job-visibility-dlq-error.md` §3.

## Acceptance Criteria

1. **Backend**: `IQueuePublisher` has `PublishToDlqAsync<T>(queue, message, exception, ct)`; `StorageQueuePublisher` implements envelope serialization (error message truncated to 2000 chars).
2. **Backend**: All four workers (Crawler, Parser, Enrichment, Indexer) use `PublishToDlqAsync` when moving failed messages to DLQ.
3. **Backend**: `StorageDlqService.RequeueMessageAsync` parses body; if envelope, extracts and publishes `originalMessage`; else publishes raw body (legacy).
4. **Backend**: `DlqMessageInfo` and API response include optional `error` object (`message`, `type`) when envelope format.
5. **Frontend**: `DeadLetterQueueComponent` displays `error.message` and `error.type` for each DLQ message when available.

## Out of Scope

- Changing retry policy or max attempts
- Storing full stack trace (message only, truncated)

## Notes

- Error message truncation: 2000 chars to stay within Azure Storage Queue 64 KB limit.
- IndexerWorker currently uses `PublishRawAsync(msg.Body)` — must parse to `IndexerMessage` and use envelope like others.
