# Script Empty KB — Design

**Date**: 2026-03-13  
**Scope**: Tool to reset Knowledge Base to zero for first ingestion job  

---

## 1. Objective

Provide a console tool `LegalAiAr.EmptyKb` that empties all Knowledge Base data stores so the system is ready for a clean first ingestion run. No schema changes; only data deletion and reset.

---

## 2. Knowledge Base Components

| Store | Purpose | Empty action |
|-------|---------|--------------|
| **Azure SQL** | Rulings, metadata, citations, judges, courts, keywords, statutes | Delete ruling-related data; reset CrawlerConfig; keep Sources, Users |
| **Azure Blob** | PDFs in `rulings-pdfs` container | Delete all blobs |
| **Azure AI Search** | `rulings-by-ruling`, `rulings-by-chunk` indexes | Delete all documents |
| **Storage Queues** | crawler, parser, enrichment, indexer + DLQs | Optional: clear all messages |

---

## 3. Azure SQL — Delete Order

Respect FK constraints. Delete in this order:

1. `Citations` (FK to Rulings)
2. `RulingJudges` (FK to Rulings, Judges)
3. `RulingKeywords` (FK to Rulings, Keywords)
4. `RulingStatutes` (FK to Rulings, Statutes)
5. `Rulings` (FK to Courts, Sources)
6. `Judges` (orphaned; optional FK to Courts — set CurrentCourtId=null first)
7. `Courts` (orphaned)
8. `Keywords` (orphaned)
9. `Statutes` (orphaned)

Then reset:

```sql
UPDATE CrawlerConfigs SET LastCrawledAt = NULL, LastCrawledStatus = NULL, LastDocumentCount = NULL;
```

**Preserve**: `Sources`, `CrawlerConfigs` (rows), `Users`.

---

## 4. Azure Blob

Delete all blobs in container `rulings-pdfs` (or configured `AzureBlob__ContainerName`). List and delete in batches.

---

## 5. Azure AI Search

Use Search REST API or SDK to delete all documents from both indexes. Options:

- **Option A**: Delete by query `*` (match all) — batch delete
- **Option B**: Recreate indexes (drops and recreates; schema stays same)

Recommend **Option A** to preserve index schema and avoid recreation.

---

## 6. Storage Queues (Optional)

- `csjn-ruling-crawler`, `csjn-ruling-parser`, `csjn-ruling-enrichment`, `csjn-ruling-indexer`
- DLQs: `csjn-ruling-*-dlq`

Clear all messages. Azure Storage Queues: receive and delete until empty. Use `--clear-queues` flag.

---

## 7. Tool Interface

```
dotnet run --project LegalAiAr.EmptyKb [--dry-run] [--clear-queues]
```

- `--dry-run`: Log what would be done; no actual deletes
- `--clear-queues`: Also clear pipeline queues (default: false)
- Environment: same as other tools (`AzureSql__ConnectionString`, `AzureBlob__ConnectionString`, `AzureSearch__Endpoint`, `AzureSearch__ApiKey`, etc.)

---

## 8. Safety

- Use `--dry-run` first to validate; no confirmation prompt (suitable for CI/scripts)
- Log counts before/after
- Exit code: 0 success, 1 failure

---

## 9. Deliverables

| # | Deliverable | DEV | AUD |
|---|-------------|:---:|:---:|
| E207 | `docs/design/script-empty-kb.md` — design doc | [ ] | [ ] |
| E208 | `LegalAiAr.EmptyKb` tool — SQL, Blob, Search, optional Queues | [ ] | [ ] |
