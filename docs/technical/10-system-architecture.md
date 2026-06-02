> ⚠️ **Imported — pending review.** Preserved from the previous documentation set. Not yet revised for
> current naming (`Legal Ai Ar` / `LegalAiAr.*`), the cloud-only environment model, or the platform
> `id_token` auth model. **Do not treat as definitive until reviewed.** Some internal links may still
> be stale.

# Legal AI AR — Technical Architecture v1.3

**Status**: Reviewed and corrected  
**Date**: 2026-03-09  
**Scope**: Phase 1 (with Phase 2 decisions incorporated)  
**Language policy**: User interface in Spanish · Code, identifiers and schemas in English

---

## 1. System overview

Legal AI AR is an internal AI platform for an Argentine law firm. It automates the ingestion of judicial rulings from public sources, indexes them in a hybrid Knowledge Base and exposes them through semantic search and RAG jurisprudential chat.

```
Ingestion pipeline (Storage Queues)
────────────────────────────────
Admin / Scheduler
       │
       ▼
 queue-crawler    ──► CrawlerWorker  (consume; downloads from CSJN, SAIJ, PJN, SCBA)
       │
       ▼
 queue-parser     ──► ParserWorker
       │
       ▼
 queue-enrichment ──► EnrichmentWorker
       │
       ▼
 queue-indexer    ──► IndexerWorker  ──► Knowledge Base
                                         (Azure SQL, Blob, AI Search)

User interfaces
───────────────
Angular SPA  ◄──── ASP.NET Core API ◄──── Knowledge Base
(includes /admin/* routes)
```

### 1.1 Backend .NET projects

| Project                     | Purpose                                                                                               |
| --------------------------- | ----------------------------------------------------------------------------------------------------- |
| LegalAiAr.Core              | Entities, enums, repository/service interfaces, Service Bus message contracts                         |
| LegalAiAr.Infrastructure    | Provider implementations (EF Core, Azure Search, Blob, SqlGraphService, Storage Queues, Azure OpenAI) |
| LegalAiAr.Application       | Use cases, CQRS handlers                                                                              |
| LegalAiAr.Api               | ASP.NET Core Web API                                                                                  |
| LegalAiAr.Worker.Crawler    | Download worker from sources                                                                          |
| LegalAiAr.Worker.Parser     | Extraction and normalization worker                                                                   |
| LegalAiAr.Worker.Enrichment | Enrichment worker with GPT-4o                                                                         |
| LegalAiAr.Worker.Indexer    | Indexing worker in Knowledge Base                                                                     |

---

## 2. Architecture decisions

| ID      | Decision           | Detail                                                                                                                                                                                                                                                                                                                |
| ------- | ------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ADR-001 | Cloud stack        | Azure (Blob, SQL, AI Search, Entra ID, Storage Queues, Azure OpenAI)                                                                                                                                                                                                                                                  |
| ADR-002 | Chunking           | 512 tokens, overlap 50. Two index levels in Azure AI Search                                                                                                                                                                                                                                                           |
| ADR-003 | Authentication     | Azure Entra ID. SSO with Microsoft 365. Phase 1: no roles (all users are admin — see ADR-013). Phase 2: roles `admin`, `lawyer`, `viewer` from Entra ID groups.                                                                                                                                                       |
| ADR-004 | Graph              | Phase 1: Azure SQL (Citations table, recursive CTEs). Phase 2: evaluate Neo4j on VM.                                                                                                                                                                                                                                  |
| ADR-005 | Immutability       | Total. Deduplication by SHA-256 of PDF content                                                                                                                                                                                                                                                                        |
| ADR-006 | Single-tenant      | Final. No organization-level isolation                                                                                                                                                                                                                                                                                |
| ADR-007 | PDF parsing        | PdfPig. No Azure Document Intelligence in Phase 1. Post-extraction normalization                                                                                                                                                                                                                                      |
| ADR-008 | CSJN pipeline      | API-first: consume CSJN REST endpoints before processing PDF. GPT-4o only for gap-filling                                                                                                                                                                                                                             |
| ADR-009 | Messaging          | Phase 1: Azure Storage Queues (same Storage Account as Blob). 4 queues: `queue-crawler`, `queue-parser`, `queue-enrichment`, `queue-indexer`                                                                                                                                                                          |
| ADR-010 | Worker hosting     | Azure Container Apps (consumption plan). 4 container apps in shared environment. Auto-scaling via KEDA + Azure Queue Storage scaler. Scales to 0 when idle.                                                                                                                                                           |
| ADR-011 | Crawler triggers   | Phase 1: manual trigger only from the admin panel via API. Phase 2: add configurable cron schedule per source.                                                                                                                                                                                                        |
| ADR-012 | Admin UI           | The admin panel is integrated in the Angular SPA under `/admin/*`. The separate MVC project is removed.                                                                                                                                                                                                               |
| ADR-013 | Phase 1 roles      | All authenticated users have `admin` role in Phase 1. No role guards. Roles `lawyer` and `viewer` are introduced in Phase 2.                                                                                                                                                                                          |
| ADR-014 | LLM and embeddings | Azure OpenAI Service with two deployments: `gpt-4o` (enrichment, agentic chat, RAG) and `gpt-4o-mini` (input guardrail classification, query enrichment). Embeddings via `text-embedding-3-large`. Used by EnrichmentWorker, IndexerWorker and API (chat pipeline). Data remains in the Azure region of the resource. |

---

## 3. Ingestion pipeline — detailed design

### 3.1 Strategy per source

| Source   | Strategy   | Metadata available via API                                                                                                                                                             |
| -------- | ---------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **CSJN** | API-first  | Case title, jurisdiction, resource type, ruling direction, subject area, thematic voices (summaries), citations to other rulings with `summaryId`, citing rulings, synthesis, opinions |
| **SAIJ** | HTML + PDF | Partial metadata in HTML. Full text in PDF                                                                                                                                             |
| **PJN**  | HTML + PDF | Basic metadata. Text in PDF                                                                                                                                                            |
| **SCBA** | HTML + PDF | Basic metadata. Text in PDF                                                                                                                                                            |

### 3.2 CSJN sub-pipeline (Phase 1 — API-first)

CSJN has two distinct phases: **discovery** (obtaining document and analysis IDs) and **metadata/PDF retrieval** (HTTP endpoints). Discovery requires browser automation because the CSJN web portal does not expose a working REST API for listing rulings.

#### 3.2.1 Discovery (CrawlerWorker — Selenium)

The CrawlerWorker uses Selenium (or equivalent headless browser) to discover rulings. Pure HTTP approaches (e.g. POST to search forms) do not work reliably with the current CSJN portal.

```
1. Navigate to fallos/consulta.html
2. Set fechaDesde and fechaHasta (dd/MM/yyyy) in the search form
3. Click "Buscar"
4. Paginate via fallos/paginarFallos.html?jtStartIndex={page*10} (page size: 10)
5. Parse response (XML or JSON) — each Record contains:
   - idAnalisis  → used for abrirAnalisis, getSumariosAnalisis, getAllDocumentos
   - Codigo      → document ID for PDF download (idDocumento)
6. For incremental: use LastCrawledAt as fechaDesde; for full: use configured date range
```

**Reference**: Validated against `legal-ai-buscar-fallos` (FallosSearchService). The HTTP-only path (SumariosSearchService) does not work with the current portal.

#### 3.2.2 Metadata and PDF retrieval (HTTP endpoints)

Once document and analysis IDs are known, the pipeline consumes REST endpoints:

```
CSJN sjconsulta API
        │
        ├── GET abrirAnalisis?idAnalisis={id}
        │       → caseTitle, jurisdiction, resourceType, rulingDirection,
        │         subjectArea, isUnconstitutional, featuredRuling (summary)
        │
        ├── GET getAllDocumentos?idAnalisis={id}
        │       → document list with download code for PDF
        │
        ├── GET getSumariosAnalisis?idAnalisis={id}
        │       → summaries with structured keywords + holding text
        │
        ├── GET getCitas?idDocumento={id}
        │       → citations to other rulings: alias "Fallos: 328:1883", summaryId linkable
        │
        ├── GET getCitantes?idDocumento={id}
        │       → rulings that cite this document
        │
        └── GET PDF binary (documentos/verDocumentoById.html?idDocumento={codigo})
                → full text for chunking + embeddings (PdfPig + normalization)

GPT-4o intervenes only for:
  - Extracting signing judges (not available in API)
  - Extracting laws cited in text (not available in API)
  - Classifying citation type (UPHOLDS/OVERRULES/DISTINGUISHES/CITES) from textual context
```

### 3.3 SAIJ/PJN/SCBA sub-pipeline (Phase 2 — HTML+PDF)

```
HTML source
    │
    ├── HTML scraping → basic metadata (caseTitle, date, court)
    ├── PDF download → PdfPig → text normalization
    └── Full GPT-4o enrichment (structured output):
            - judges, cited_statutes, keywords
            - jurisdiction_area, instance, summary (~200 words)
            - holding (~100 words)
            - cited_rulings (array: {alias, volume, page, citationType})
```

### 3.4 Workers

#### CrawlerWorker

- **Phase 1 — manual trigger only**: activates when an admin publishes a message to `queue-crawler` from the admin panel.
- **Phase 2**: add configurable cron schedule per source (see section 4.14 (`CrawlerConfigs` table)).
- **CSJN discovery**: uses Selenium (headless Chrome/Chromium) to navigate `sjconsulta.csjn.gov.ar`, filter by date range, and paginate results. Pure HTTP search does not work with the current portal. See section 3.2.1.
- Incremental or full download according to message type received. Detects new documents by SHA-256 of PDF.
- **Responsible for uploading PDF to Blob Storage**: downloads PDF, uploads to Blob, obtains path and includes it in `blobPathPdf` of the message published to `queue-parser`.
- Retry with exponential backoff (max 3 attempts). Persistent failure → DLQ.

#### ParserWorker

- For CSJN: consumes all API endpoints and reads PDF from Blob Storage using `blobPathPdf` from the message (CrawlerWorker already uploaded the PDF).
- For other sources: HTML scraping + PDF download.
- Text normalization: collapse multiple spaces, normalize line breaks, remove image headers.
- Publishes to `queue-enrichment` with `extractedMetadata` and `missingFields`.

#### EnrichmentWorker

- For CSJN: calls GPT-4o only for `missingFields` (judges, statutes, citation types).
- For other sources: full extraction via GPT-4o.
- Model: `gpt-4o` with `response_format: { type: "json_schema" }`.
- Publishes to `queue-indexer` with complete payload.

#### IndexerWorker

- Persists the ruling in Azure SQL + PDF in Blob Storage.
- Generates embeddings:
    - **Ruling level**: `text-embedding-3-large` over `summary + holding`
    - **Chunk level**: `text-embedding-3-large` over each 512-token chunk with overlap 50
- Indexes both levels in Azure AI Search.
- Creates nodes and relationships in Neo4j.
- Executes retroactive citation resolution (see section 4.10).

---

## 4. Data model

### 4.1 Azure SQL — `Rulings` table

| Field                | Type                | Source       | Description                                                        |
| -------------------- | ------------------- | ------------ | ------------------------------------------------------------------ |
| `Id`                 | UNIQUEIDENTIFIER PK | System       | UUID generated when indexing                                       |
| `SourceId`           | INT FK              | System       | CSJN=1, SAIJ=2, PJN=3, SCBA=4                                      |
| `ExternalId`         | VARCHAR(50)         | Source       | Document ID in source (e.g. `8048522`)                             |
| `AnalysisId`         | VARCHAR(50)         | CSJN API     | CSJN document analysis ID (e.g. `804852`). Null for other sources. |
| `ContentHash`        | CHAR(64)            | System       | SHA-256 of PDF. Deduplication key.                                 |
| `CaseTitle`          | NVARCHAR(500)       | Source/API   | Official case title of the ruling                                  |
| `CaseNumber`         | VARCHAR(100)        | Source/API   | E.g. `CAF 9548/2021/CA1-CS1`                                       |
| `RulingDate`         | DATE                | Source/API   | Date of the ruling                                                 |
| `CourtId`            | INT FK              | Source/API   | FK to `Courts` table                                               |
| `JurisdictionArea`   | VARCHAR(100)        | Source/GPT   | E.g. `CONTENCIOSO ADMINISTRATIVO FEDERAL`                          |
| `Instance`           | VARCHAR(50)         | Source/GPT   | E.g. `CSJN`, `Cámara`, `Primera Instancia`                         |
| `Jurisdiction`       | VARCHAR(100)        | CSJN API     | E.g. `APELACION EXTRAORDINARIA`. Null for other sources.           |
| `ResourceType`       | VARCHAR(100)        | CSJN API     | E.g. `RECURSO EXTRAORDINARIO FEDERAL`                              |
| `RulingDirection`    | VARCHAR(50)         | CSJN API/GPT | E.g. `UPHOLDS`, `OVERRULES`, `GRANTS`                              |
| `SubjectArea`        | VARCHAR(100)        | CSJN API     | E.g. `Tributario - Bancario`                                       |
| `IsUnconstitutional` | BIT                 | CSJN API/GPT | Declares unconstitutionality                                       |
| `Summary`            | NVARCHAR(MAX)       | CSJN API/GPT | Ruling summary                                                     |
| `Holding`            | NVARCHAR(MAX)       | CSJN API/GPT | Main holding                                                       |
| `FullText`           | NVARCHAR(MAX)       | PDF          | Extracted and normalized text                                      |
| `BlobPath`           | VARCHAR(500)        | System       | Path in Azure Blob Storage                                         |
| `IndexedAt`          | DATETIME            | System       | Indexing timestamp                                                 |
| `Status`             | VARCHAR(20)         | System       | `indexed`, `error`, `pending`                                      |

### 4.2 Azure SQL — `Courts` table

| Field              | Type          | Description                                         |
| ------------------ | ------------- | --------------------------------------------------- |
| `Id`               | INT PK        |                                                     |
| `Name`             | NVARCHAR(300) | Official court name                                 |
| `JurisdictionArea` | VARCHAR(100)  | E.g. `PENAL`, `CIVIL`, `CONTENCIOSO ADMINISTRATIVO` |
| `Territory`        | VARCHAR(100)  | E.g. `Nacional`, `Buenos Aires`, `Chubut`           |
| `Instance`         | VARCHAR(50)   | E.g. `CSJN`, `Cámara`, `Primera Instancia`          |

### 4.3 Azure SQL — `Judges` table

| Field            | Type          | Description    |
| ---------------- | ------------- | -------------- |
| `Id`             | INT PK        |                |
| `FirstName`      | NVARCHAR(200) |                |
| `LastName`       | NVARCHAR(200) |                |
| `CurrentCourtId` | INT FK        | FK to `Courts` |

### 4.4 Azure SQL — `RulingJudges` table (N:N)

| Field               | Type                | Description                        |
| ------------------- | ------------------- | ---------------------------------- |
| `RulingId`          | UNIQUEIDENTIFIER FK |                                    |
| `JudgeId`           | INT FK              |                                    |
| `ParticipationType` | VARCHAR(50)         | `SIGNATORY`, `DISSENT`, `MAJORITY` |

### 4.5 Azure SQL — `Keywords` table

| Field          | Type          | Description                                                      |
| -------------- | ------------- | ---------------------------------------------------------------- |
| `Id`           | INT PK        |                                                                  |
| `ExternalCode` | INT           | CSJN code (`codigoValor`). Null for other sources.               |
| `Description`  | NVARCHAR(200) | E.g. `IMPUESTO A LAS GANANCIAS` (stored as received from source) |

### 4.6 Azure SQL — `RulingKeywords` table (N:N)

| Field       | Type                | Description     |
| ----------- | ------------------- | --------------- |
| `RulingId`  | UNIQUEIDENTIFIER FK |                 |
| `KeywordId` | INT FK              |                 |
| `SortOrder` | INT                 | Relevance order |

### 4.7 Azure SQL — `Statutes` table

| Field    | Type          | Description                                       |
| -------- | ------------- | ------------------------------------------------- |
| `Id`     | INT PK        |                                                   |
| `Number` | VARCHAR(50)   | E.g. `24.767`, `11.683`                           |
| `Name`   | NVARCHAR(300) | Official law name                                 |
| `Url`    | VARCHAR(500)  | Link to argentina.gob.ar/normativa (if available) |

### 4.8 Azure SQL — `RulingStatutes` table (N:N)

| Field       | Type                | Description                                       |
| ----------- | ------------------- | ------------------------------------------------- |
| `RulingId`  | UNIQUEIDENTIFIER FK |                                                   |
| `StatuteId` | INT FK              |                                                   |
| `Articles`  | NVARCHAR(200)       | Specific articles cited (e.g. `art. 80, art. 64`) |

### 4.9 Azure SQL — `Citations` table

| Field            | Type                         | Description                                                                                                  |
| ---------------- | ---------------------------- | ------------------------------------------------------------------------------------------------------------ |
| `Id`             | INT PK                       |                                                                                                              |
| `SourceRulingId` | UNIQUEIDENTIFIER FK          | Ruling that cites                                                                                            |
| `TargetRulingId` | UNIQUEIDENTIFIER FK nullable | Cited ruling. Null if not yet indexed.                                                                       |
| `ExternalAlias`  | VARCHAR(100)                 | E.g. `Fallos: 328:1883` (original format preserved)                                                          |
| `CsjnSummaryId`  | INT nullable                 | CSJN summary ID for future linking                                                                           |
| `CitationType`   | VARCHAR(50)                  | `UPHOLDS`, `OVERRULES`, `DISTINGUISHES`, `CITES`. Inferred by GPT-4o from textual context. Default: `CITES`. |

### 4.10 Retroactive citation resolution

When `Citations.TargetRulingId` is null (the cited ruling was not indexed at ingestion time), the `IndexerWorker` executes the following process when indexing each new ruling:

```
On indexing NewRuling:
  1. Search Citations where ExternalAlias matches NewRuling's case number or volume/page
  2. For each match: UPDATE Citations SET TargetRulingId = NewRuling.Id
  3. Create [:CITES] relationship in Neo4j between SourceRuling and NewRuling

Additionally, resolve outbound citations of NewRuling:
  4. For each citation from NewRuling: check if TargetRuling already exists in KB
  5. If found: SET TargetRulingId. If not: leave null for future resolution.
```

### 4.11 Graph — Model (Phase 1: SQL; Phase 2: Neo4j)

```cypher
// Nodes
(:Ruling {id, caseTitle, rulingDate, jurisdictionArea, instance, rulingDirection})
(:Judge {id, firstName, lastName})
(:Court {id, name, jurisdictionArea, territory})
(:Keyword {id, description})
(:Statute {number, name})

// Relationships
(:Ruling)-[:SIGNED_BY {participationType: "SIGNATORY|DISSENT|MAJORITY"}]->(:Judge)
(:Ruling)-[:CITES {citationType: "UPHOLDS|OVERRULES|DISTINGUISHES|CITES"}]->(:Ruling)
(:Ruling)-[:HAS_KEYWORD]->(:Keyword)
(:Ruling)-[:CITES_STATUTE]->(:Statute)
(:Ruling)-[:ISSUED_BY]->(:Court)
(:Judge)-[:MEMBER_OF]->(:Court)
```

### 4.12 Azure AI Search — Indexes

**Index `rulings-by-ruling`** (ruling-level search)

```json
{
    "id": "string (key)",
    "rulingId": "string",
    "caseTitle": "string (searchable)",
    "summary": "string (searchable)",
    "holding": "string (searchable)",
    "rulingDate": "date (filterable, sortable)",
    "jurisdictionArea": "string (filterable, facetable)",
    "instance": "string (filterable, facetable)",
    "court": "string (filterable, facetable)",
    "keywords": "Collection(string) (filterable, facetable)",
    "rulingDirection": "string (filterable)",
    "embedding": "Collection(Single) — 3072 dim vector"
}
```

**Index `rulings-by-chunk`** (chunk-level search)

```json
{
    "id": "string (key)",
    "rulingId": "string (filterable)",
    "chunkIndex": "int32",
    "text": "string (searchable)",
    "embedding": "Collection(Single) — 3072 dim vector"
}
```

### 4.13 Azure SQL — `Sources` table

Catalog of judicial sources supported by the system. Referenced as FK from `Rulings`, `CrawlerConfigs` and `IngestionJobs`.

| Field      | Type          | Description                                               |
| ---------- | ------------- | --------------------------------------------------------- |
| `Id`       | INT PK        | CSJN=1, SAIJ=2, PJN=3, SCBA=4                             |
| `Name`     | VARCHAR(50)   | Short name (e.g. `CSJN`)                                  |
| `FullName` | NVARCHAR(200) | Full name (e.g. `Corte Suprema de Justicia de la Nación`) |
| `BaseUrl`  | VARCHAR(500)  | Base URL of the source                                    |
| `Strategy` | VARCHAR(20)   | `api-first`, `html-pdf`                                   |
| `IsActive` | BIT           | Whether the source is active in the system                |

### 4.14 Azure SQL — `CrawlerConfigs` table

Stores configuration per source. In Phase 1 it is only used to enable/disable sources and save the last crawl status. The `CronExpression` field is introduced in Phase 2.

| Field               | Type              | Description                                                            |
| ------------------- | ----------------- | ---------------------------------------------------------------------- |
| `Id`                | INT PK            |                                                                        |
| `SourceId`          | INT FK            | FK to source (CSJN=1, SAIJ=2, PJN=3, SCBA=4)                           |
| `IsEnabled`         | BIT               | Whether the source is active for crawling                              |
| `CronExpression`    | VARCHAR(100)      | **Phase 2** — cron expression for automatic schedule. Null in Phase 1. |
| `LastCrawledAt`     | DATETIME nullable | Timestamp of last completed crawl                                      |
| `LastCrawledStatus` | VARCHAR(20)       | `success`, `partial`, `failed`. Null if never executed.                |
| `LastDocumentCount` | INT nullable      | Number of documents processed in last crawl                            |
| `CreatedAt`         | DATETIME          |                                                                        |
| `UpdatedAt`         | DATETIME          |                                                                        |

### 4.15 Azure SQL — `IngestionJobs` table _(Phase 2)_

Records each crawl execution. Allows tracing which job originated each entity in the KB.

| Field                 | Type                   | Description                                            |
| --------------------- | ---------------------- | ------------------------------------------------------ |
| `Id`                  | UNIQUEIDENTIFIER PK    | UUID generated when job starts                         |
| `SourceId`            | INT FK                 | Crawled source                                         |
| `Type`                | VARCHAR(20)            | `incremental`, `full`                                  |
| `TriggeredBy`         | VARCHAR(100)           | Admin user who triggered it, or `scheduler` in Phase 2 |
| `StartedAt`           | DATETIME               | Start timestamp                                        |
| `CompletedAt`         | DATETIME nullable      | Completion timestamp. Null if still in progress.       |
| `Status`              | VARCHAR(20)            | `running`, `completed`, `partial`, `failed`            |
| `DocumentsDiscovered` | INT                    | New documents detected by `CrawlerWorker`              |
| `DocumentsIndexed`    | INT                    | Documents actually persisted in the KB                 |
| `DocumentsFailed`     | INT                    | Documents that failed at some pipeline stage           |
| `ErrorSummary`        | NVARCHAR(MAX) nullable | Error summary if `Status` is `partial` or `failed`     |

The FK in `Rulings` that links each ruling to the job that originated it:

```sql
-- Additional column in Rulings table (Phase 2):
IngestionJobId  UNIQUEIDENTIFIER FK nullable  -- FK to IngestionJobs
```

This enables direct traceability queries:

```sql
-- Which job originated this ruling?
SELECT j.* FROM IngestionJobs j
JOIN Rulings r ON r.IngestionJobId = j.Id
WHERE r.Id = '...'

-- Which rulings did job X index?
SELECT * FROM Rulings WHERE IngestionJobId = '...'

-- How many rulings did each CSJN job generate in the last month?
SELECT j.Id, j.StartedAt, j.DocumentsIndexed
FROM IngestionJobs j
WHERE j.SourceId = 1
AND j.StartedAt >= DATEADD(month, -1, GETUTCDATE())
ORDER BY j.StartedAt DESC
```

---

## 5. API — ASP.NET Core

> **F0.0-W09:** The API layer uses **Minimal API** with convention-based `IEndpoint` discovery
> (`AddLegalAiArEndpoints` / `MapLegalAiArEndpoints`). Route groups and policies are documented in
> [`22-api-endpoints.md`](22-api-endpoints.md). MVC controllers were removed from `LegalAiAr.Api`.

### Phase 1 endpoints

> **Phase 1**: all authenticated users have full access (ADR-013). The "Roles" column reflects the Phase 2 model to preserve design intent.

| Method   | Route                                 | Description                                            | Roles (Phase 2)             |
| -------- | ------------------------------------- | ------------------------------------------------------ | --------------------------- |
| `POST`   | `/api/rulings/search`                 | Hybrid semantic search (vector + keyword)              | `admin`, `lawyer`, `viewer` |
| `GET`    | `/api/rulings/{id}`                   | Full ruling details                                    | `admin`, `lawyer`, `viewer` |
| `GET`    | `/api/rulings/{id}/related`           | Related rulings by semantic similarity                 | `admin`, `lawyer`           |
| `POST`   | `/api/chat`                           | RAG jurisprudential chat (SSE streaming)               | `admin`, `lawyer`           |
| `GET`    | `/api/admin/pipeline/status`          | Pipeline status per source                             | `admin`                     |
| `GET`    | `/api/admin/jobs`                     | Active/completed/failed jobs                           | `admin`                     |
| `GET`    | `/api/admin/dlq`                      | Dead Letter Queue view per queue                       | `admin`                     |
| `POST`   | `/api/admin/dlq/{queue}/{id}/requeue` | Requeue message to origin queue                        | `admin`                     |
| `GET`    | `/api/admin/crawlers`                 | Configuration and status of all crawlers               | `admin`                     |
| `GET`    | `/api/admin/crawlers/{sourceId}`      | Configuration and status of a specific crawler         | `admin`                     |
| `PATCH`  | `/api/admin/crawlers/{sourceId}`      | Update configuration (enable/disable, cron in Phase 2) | `admin`                     |
| `POST`   | `/api/admin/crawlers/{sourceId}/run`  | Trigger manual crawl                                   | `admin`                     |
| `GET`    | `/api/admin/users`                    | User list                                              | `admin`                     |
| `POST`   | `/api/admin/users`                    | Create user                                            | `admin`                     |
| `PUT`    | `/api/admin/users/{id}`               | Update user / change role                              | `admin`                     |
| `DELETE` | `/api/admin/users/{id}`               | Deactivate user                                        | `admin`                     |
| `GET`    | `/api/health`                         | Health check                                           | public                      |

**Body `POST /api/rulings/search`:**

```json
{
    "query": "string (required)",
    "filters": {
        "jurisdictionArea": "string (optional)",
        "instance": "string (optional)",
        "courtId": "int (optional)",
        "dateFrom": "date (optional)",
        "dateTo": "date (optional)",
        "keywords": ["string"]
    },
    "page": "int (default: 1)",
    "pageSize": "int (default: 10, max: 50)"
}
```

**`POST /api/admin/crawlers/{sourceId}/run` body:**

```json
{
    "type": "incremental|full",
    "since": "date (optional, only for incremental — default: LastCrawledAt)"
}
```

**`PATCH /api/admin/crawlers/{sourceId}` body (Phase 1):**

```json
{
    "isEnabled": "bool"
}
```

### Chat RAG — internal flow

```
1. User sends query
2. API generates query embedding (text-embedding-3-large)
3. Search rulings-by-chunk index in Azure AI Search (top-K=10)
4. Search rulings-by-ruling index in Azure AI Search (top-K=5)
5. Build context from chunks + ruling metadata
6. Call GPT-4o with legal RAG prompt
7. Response includes explicit ruling citations ({caseTitle, id})
8. SSE stream to client
```

---

## 6. Angular SPA — Phase 1

> Routes use Spanish slugs because they are user-visible URLs. Component names and internal code are in English.
> **Phase 1**: no role guards. All authenticated users access all routes, including `/admin/*`.

### Main routes

| Component                                     | Route                | Description                                                                                                               |
| --------------------------------------------- | -------------------- | ------------------------------------------------------------------------------------------------------------------------- |
| `SearchHomeComponent`                         | `/buscar`            | Natural language semantic search                                                                                          |
| `SearchResultsComponent`                      | `/buscar/resultados` | Results list with relevant fragments                                                                                      |
| `RulingDetailComponent`                       | `/fallos/:id`        | Full ruling details                                                                                                       |
| `ChatViewComponent`                           | `/chat`              | RAG jurisprudential chat                                                                                                  |
| `MsalRedirectComponent` (or `LoginComponent`) | `/login`             | Redirect to Entra ID (MSAL). _Note: In Angular implementation, `MsalRedirectComponent` from @azure/msal-angular is used._ |

### Administration routes (`/admin/*`)

| Component                  | Route             | Description                                                         |
| -------------------------- | ----------------- | ------------------------------------------------------------------- |
| `DashboardComponent`       | `/admin`          | Pipeline status: processed documents, errors, last crawl per source |
| `CrawlersComponent`        | `/admin/crawlers` | Source list with status, last crawl and "Run now" button            |
| `JobsComponent`            | `/admin/jobs`     | Active, completed and failed jobs with error details                |
| `DeadLetterQueueComponent` | `/admin/dlq`      | DLQ per queue (4 tabs) with requeue option                          |
| `UsersComponent`           | `/admin/users`    | User CRUD with role assignment                                      |

### Authentication (MSAL Angular)

Login via Microsoft Entra ID. A single `AuthGuard` protects all routes (authenticated vs unauthenticated). In Phase 2, `RoleGuard` is added to restrict `/admin/*` and chat routes to the corresponding role.

---

## 7. Azure infrastructure

| Component           | Service                     | Phase 1 tier                                                                                                                                               |
| ------------------- | --------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Blob storage        | Azure Blob Storage          | LRS Standard                                                                                                                                               |
| Relational database | Azure SQL Database          | General Purpose 2 vCores (serverless)                                                                                                                      |
| Semantic search     | Azure AI Search             | Basic                                                                                                                                                      |
| LLM and embeddings  | Azure OpenAI Service        | gpt-4o + text-embedding-3-large                                                                                                                            |
| Identity            | Azure Entra ID              | Law firm's Microsoft 365 (already available)                                                                                                               |
| Messaging           | Azure Storage Queues        | 4 queues (same Storage Account as Blob)                                                                                                                    |
| API hosting         | Azure App Service           | B2                                                                                                                                                         |
| SPA hosting         | Azure Static Web Apps       | Free / Standard                                                                                                                                            |
| Graph               | Azure SQL (Citations, CTEs) | Included in SQL                                                                                                                                            |
| Workers             | Azure Container Apps        | Consumption plan. 4 container apps in shared environment. KEDA + Azure Queue Storage scaler. Scales to 0 when idle. Max 2 instances per worker in Phase 1. |

### 7.1 Azure OpenAI Service

Azure OpenAI provides the AI models used in the system:

| Use              | Model                              | Consumed by                                                              |
| ---------------- | ---------------------------------- | ------------------------------------------------------------------------ |
| Embeddings       | text-embedding-3-large (3072 dims) | IndexerWorker (chunking + ruling level), API (semantic search, RAG chat) |
| LLM — enrichment | gpt-4o                             | EnrichmentWorker (extraction of judges, laws, citation classification)   |
| LLM — chat RAG   | gpt-4o                             | API (ChatQueryHandler)                                                   |

The Azure OpenAI resource (existing) is used in the same region as the rest of the infrastructure. Data sent remains in the resource's region (compliance). Configuration via endpoint, API key and deployment names.

---

## 8. Messaging — Azure Storage Queues (Phase 1)

### Queue design

```
Scheduler / Admin
       │
       ▼
 queue-crawler    ──► CrawlerWorker
                           │
                           ▼
 queue-parser     ──► ParserWorker
                           │
                           ▼
 queue-enrichment ──► EnrichmentWorker
                           │
                           ▼
 queue-indexer    ──► IndexerWorker
```

DLQ: implement `queue-{name}-dlq` queues for failed messages (Storage Queues has no native DLQ).

### Message schemas

**queue-crawler**

```json
{ "sourceId": 1, "type": "incremental|full", "since": "2026-01-01" }
```

**queue-parser**

> JSON uses camelCase; in C# (ParserMessage) properties are PascalCase (e.g. BlobPathPdf).

```json
{
    "sourceId": 1,
    "documentId": "8048522",
    "analysisId": "804852",
    "blobPathPdf": "csjn/2024/8048522.pdf",
    "contentHash": "abc123...",
    "apiMetadata": {
        "caseTitle": "string",
        "rulingDate": "date",
        "jurisdiction": "string",
        "resourceType": "string",
        "rulingDirection": "string",
        "subjectArea": "string",
        "isUnconstitutional": "bool",
        "summary": "string",
        "holding": "string",
        "keywords": [
            { "externalCode": 2093, "description": "IMPUESTO A LAS GANANCIAS" }
        ],
        "citations": [{ "alias": "Fallos: 328:1883", "summaryId": 56748 }],
        "citedBy": [
            { "analysisId": "818955", "caseNumber": "CAF 003507/2024/CS001" }
        ]
    }
}
```

**queue-enrichment**

```json
{
    "documentId": "8048522",
    "sourceId": 1,
    "normalizedText": "string",
    "extractedMetadata": {
        "caseTitle": "string",
        "rulingDate": "date",
        "court": "string",
        "jurisdictionArea": "string",
        "instance": "string",
        "rulingDirection": "string",
        "summary": "string",
        "holding": "string",
        "keywords": ["string"],
        "citations": [{ "alias": "string" }]
    },
    "missingFields": ["judges", "cited_statutes", "citation_types"]
}
```

**queue-indexer**

```json
{
    "documentId": "8048522",
    "ruling": {
        "caseTitle": "string",
        "rulingDate": "date",
        "jurisdictionArea": "string",
        "instance": "string",
        "jurisdiction": "string",
        "resourceType": "string",
        "rulingDirection": "string",
        "subjectArea": "string",
        "isUnconstitutional": "bool",
        "summary": "string",
        "holding": "string",
        "fullText": "string",
        "blobPath": "string"
    },
    "judges": [
        {
            "firstName": "string",
            "lastName": "string",
            "participationType": "SIGNATORY"
        }
    ],
    "keywords": [
        { "externalCode": 2093, "description": "string", "sortOrder": 1 }
    ],
    "statutes": [
        { "number": "24.767", "name": "string", "articles": "art. 8" }
    ],
    "citations": [
        {
            "externalAlias": "Fallos: 328:1883",
            "csjnSummaryId": 56748,
            "citationType": "CITES"
        }
    ],
    "chunks": [
        { "index": 0, "text": "string" },
        { "index": 1, "text": "string" }
    ]
}
```

### Retry configuration

- **Max delivery count**: 3 · **Lock duration**: 5 min · **Message TTL**: 7 days
- After 3 failed attempts → message automatically moves to corresponding DLQ

---

## 9. Workers — Azure Container Apps

### Environment design

```
Container App Environment (Phase 1)
│
├── crawler-worker
│     Trigger: KEDA — Service Bus scaler on queue-crawler (Phase 1: manual messages only)
│     Resources: 0.5 vCPU / 1GB RAM
│     Scale: min 0, max 1 (sequential crawls per source)
│     Note: CSJN discovery requires Selenium + headless Chrome/Chromium. Dockerfile must include Chrome/Chromium.
│
├── parser-worker
│     Trigger: KEDA — Service Bus scaler on queue-parser
│     Resources: 0.5 vCPU / 1GB RAM
│     Scale: min 0, max 2
│
├── enrichment-worker
│     Trigger: KEDA — Service Bus scaler on queue-enrichment
│     Resources: 1 vCPU / 2GB RAM (blocking calls to GPT-4o)
│     Scale: min 0, max 2
│
└── indexer-worker
      Trigger: KEDA — Service Bus scaler on queue-indexer
      Resources: 1 vCPU / 2GB RAM (embeddings + multi-store writes)
      Scale: min 0, max 2
```

### Key advantages for this system

- **Total API isolation**: enrichment spikes do not affect search latency
- **$0 cost at rest**: scales to 0 instances when queues are empty
- **No timeout on crawls**: Container Apps has no execution duration limit
- **Independent deploy**: each worker updates without touching the API or other workers

---

## 10. Pending / Next decisions

Decisions deferred to Phase 2 or under evaluation:

1. **CronExpression and automatic scheduling** — Cron configuration per source in CrawlerConfigs for automatic triggers (ADR-011).
2. **IngestionJobs and Rulings.IngestionJobId** — Jobs table and FK in Rulings for traceability of which crawl originated each ruling.
3. **Lawyer/viewer roles and RoleGuard** — Roles from Entra ID groups and guards on Angular routes.
4. **PJN and SCBA crawlers** — Activation of HTML+PDF sources after quality validation.
5. **PJN/SCBA PDF quality validation (Phase 2)** — Extraction quality evaluation with PdfPig; possible use of Azure Document Intelligence if quality is insufficient. See ADR-007 and feature F2-2.

---

## 11. Open technical risks

| ID    | Description                                                           | Severity | Status                                                      |
| ----- | --------------------------------------------------------------------- | -------- | ----------------------------------------------------------- |
| R-001 | Breaking changes in CSJN API without notice                           | Medium   | Open — implement defensive versioning in crawler            |
| R-002 | Undocumented rate limiting on CSJN endpoints                          | Medium   | Open — implement configurable throttling                    |
| R-003 | PDF quality in PJN and SCBA not validated                             | High     | Open — validate in Phase 2 before activating those crawlers |
| R-004 | Graph in SQL: recursive CTEs may be slower than Neo4j on large graphs | Low      | Accepted for Phase 1; evaluate Neo4j in Phase 2             |
| R-005 | Worker hosting undefined (ADR-010)                                    | Medium   | ✅ Closed — Container Apps                                  |
