# Legal AI AR

Internal legal AI platform for Argentine law firms. Automates the ingestion of judicial rulings from public sources, indexes them in a hybrid Knowledge Base and exposes them through semantic search and RAG jurisprudential chat.

---

## Documentation

| Document | Location |
|---|---|
| Technical architecture | [`docs/architecture/legal-ai-ar-architecture.md`](docs/architecture/legal-ai-ar-architecture.md) |
| C4 diagrams (context, container, component) | [`docs/architecture/c4-diagrams.md`](docs/architecture/c4-diagrams.md) |
| Development specifications | [`docs/architecture/legal-ai-ar-specs.md`](docs/architecture/legal-ai-ar-specs.md) |
| Roadmap | [`docs/roadmap/ROADMAP.md`](docs/roadmap/ROADMAP.md) |
| Design (T-00 deliverables) | [`docs/design/`](docs/design/) |
| **App design guidelines (PwC)** | [`docs/mockups/legal-ai-ar-pwc-design-guidelines.md`](docs/mockups/legal-ai-ar-pwc-design-guidelines.md) |
| **UI mockups (HTML)** | [`docs/mockups/index.html`](docs/mockups/index.html) |
| Agent prompts | [`docs/prompts/cursor/`](docs/prompts/cursor/) |

---

## Index

- [Overview](#overview)
- [Tech stack](#tech-stack)
- [Architecture](#architecture)
- [Ingestion pipeline](#ingestion-pipeline)
- [Data model](#data-model)
- [API](#api)
- [Frontend](#frontend)
- [Infrastructure](#infrastructure)
- [Development phases](#development-phases)
- [Architecture decisions](#architecture-decisions)

---

## Overview

Legal AI AR enables firm lawyers to:

- **Search rulings** in natural language over an indexed jurisprudence base
- **Query a RAG chat** that answers legal questions citing specific rulings
- **Explore relationships** between rulings, judges, laws and thematic voices via graph
- **Manage the ingestion pipeline** from the panel integrated in the same application

### Supported data sources

| Source | Strategy | Phase |
|---|---|---|
| CSJN — Corte Suprema de Justicia de la Nación | API-first (`sjconsulta.csjn.gov.ar`) | Phase 1 |
| SAIJ — Sistema Argentino de Información Jurídica | HTML + PDF (partial metadata in HTML, full text in PDF) | Phase 2 |
| PJN — Poder Judicial de la Nación | HTML + PDF (basic metadata in HTML) | Phase 2 |
| SCBA — Suprema Corte de Buenos Aires | HTML + PDF (basic metadata in HTML) | Phase 2 |

---

## Tech stack

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core |
| Frontend | Angular (single SPA — includes admin panel) |
| Workers | .NET — Azure Container Apps |
| Messaging | Azure Storage Queues (Phase 1; same Storage Account as Blob) |
| Relational database | Azure SQL Database |
| Semantic search | Azure AI Search |
| File storage | Azure Blob Storage |
| Relationship graph | Azure SQL — Citations table + recursive CTEs (Phase 1) |
| Identity | Azure Entra ID (SSO with Microsoft 365) |
| Embeddings | Azure OpenAI — `text-embedding-3-large` (3072 dims) |
| LLM | Azure OpenAI — `gpt-4o` (enrichment + RAG chat) |
| PDF parsing | PdfPig |

---

## Architecture

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

### Workers

| Worker | Trigger | Responsibility |
|---|---|---|
| `CrawlerWorker` | Manual from Admin (Phase 1) / cron (Phase 2) | Discovers new documents in each source. Detects duplicates by SHA-256. |
| `ParserWorker` | KEDA — `queue-parser` | For CSJN: consumes REST endpoints + downloads PDF. For other sources: HTML scraping + PDF. Normalizes text. |
| `EnrichmentWorker` | KEDA — `queue-enrichment` | For CSJN: extracts with GPT-4o only missing fields (judges, statutes). For other sources: full enrichment. |
| `IndexerWorker` | KEDA — `queue-indexer` | Persists in Azure SQL, Blob Storage, Azure AI Search. Generates embeddings. Resolves retroactive citations. Graph in SQL (Citations table). |

Each worker scales to 0 instances when its queue is empty (consumption plan). With Storage Queues, the DLQ must be implemented manually (queue `queue-{name}-dlq`) if required.

---

## Ingestion pipeline

### CSJN flow (API-first)

CSJN has two phases:

1. **Discovery** — The CrawlerWorker uses Selenium (headless browser) to navigate `sjconsulta.csjn.gov.ar`, filter by date range, and paginate results. Pure HTTP search does not work with the current portal. Discovery yields `idAnalisis` and document ID (`Codigo`) per ruling.

2. **Metadata and PDF** — Once IDs are known, the pipeline consumes REST endpoints before processing the PDF, minimizing GPT-4o usage:

```
GET abrirAnalisis       → caseTitle, jurisdiction, resourceType, rulingDirection
GET getAllDocumentos     → document list for download
GET getSumariosAnalisis → structured keywords + holding
GET getCitas            → citations to other rulings with summaryId
GET getCitantes         → rulings that cite this document
GET PDF                 → full text (PdfPig + normalization)

GPT-4o only for: signing judges · cited statutes · citation classification
```

### SAIJ / PJN / SCBA flow (HTML + PDF)

```
HTML scraping → basic metadata
PDF download  → PdfPig → normalization
GPT-4o        → full enrichment (judges, statutes, keywords, summary, holding, citations)
```

### Citations between rulings

Citations are resolved retroactively: when a new ruling is indexed, the `IndexerWorker` searches in `Citations` for previous rulings that cited it with `TargetRulingId = null` and completes the link. The graph is in the Citations table (queries via recursive CTEs).

---

## Data model

### Azure SQL — main tables

| Table | Description |
|---|---|
| `Sources` | Source catalog (CSJN, SAIJ, PJN, SCBA) |
| `Rulings` | Indexed rulings with full metadata |
| `Courts` | Courts |
| `Judges` | Judges |
| `RulingJudges` | Judge participation in rulings (signatory, dissent, majority) |
| `Keywords` | Thematic voices (CSJN thesaurus + extracted by GPT-4o) |
| `RulingKeywords` | Rulings ↔ keywords association |
| `Statutes` | Cited laws and decrees |
| `RulingStatutes` | Rulings ↔ statutes association with specific articles |
| `Citations` | Citations between rulings with type (`UPHOLDS`, `OVERRULES`, `DISTINGUISHES`, `CITES`) |
| `CrawlerConfigs` | Configuration and status of each crawler per source |
| `IngestionJobs` *(Phase 1)* | Crawl execution log for traceability |

### Graph — relationships (Phase 1: SQL)

Model in tables: `Citations` (SourceRulingId, TargetRulingId, CitationType), `RulingJudges`, `RulingKeywords`, `RulingStatutes`. Timeline and ranking queries via recursive CTEs. Phase 2: evaluate Neo4j.

### Azure AI Search — indexes

| Index | Use |
|---|---|
| `rulings-by-ruling` | Ruling-level semantic search (embedding over `summary + holding`) |
| `rulings-by-chunk` | Chunk-level semantic search (512-token chunks, overlap 50) |

---

## API

Base URL: `/api`. All routes are relative to base `/api`.

### Main endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/rulings/search` | Hybrid semantic search (vector + keyword) with filters |
| `GET` | `/api/rulings/{id}` | Full ruling details |
| `GET` | `/api/rulings/{id}/related` | Related rulings by semantic similarity |
| `POST` | `/api/chat` | RAG jurisprudential chat — responds with SSE streaming |
| `GET` | `/api/health` | Health check |

### Admin endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/admin/pipeline/status` | Pipeline status per source |
| `GET` | `/api/admin/jobs` | Active, completed and failed jobs |
| `GET` | `/api/admin/crawlers` | Status of all crawlers |
| `GET` | `/api/admin/crawlers/{sourceId}` | Configuration and status of a specific crawler |
| `POST` | `/api/admin/crawlers/{sourceId}/run` | Trigger manual crawl (`incremental` or `full`) |
| `PATCH` | `/api/admin/crawlers/{sourceId}` | Enable / disable source |
| `GET` | `/api/admin/dlq` | Dead Letter Queue per queue |
| `POST` | `/api/admin/dlq/{queue}/{id}/requeue` | Requeue failed message |
| `GET` | `/api/admin/users` | User list |
| `POST` | `/api/admin/users` | Create user |
| `PUT` | `/api/admin/users/{id}` | Update user / change role |
| `DELETE` | `/api/admin/users/{id}` | Deactivate user |

### RAG chat flow

```
User query
  → embedding (text-embedding-3-large)
  → hybrid search in Azure AI Search (chunks + rulings)
  → context construction with relevant fragments
  → GPT-4o with legal prompt
  → response with explicit ruling citations
  → SSE streaming to client
```

---

## Frontend

Angular SPA with two sections:

### Main routes

| Route | Component | Description |
|---|---|---|
| `/buscar` | `SearchHomeComponent` | Natural language semantic search |
| `/buscar/resultados` | `SearchResultsComponent` | Results with relevant fragments |
| `/fallos/:id` | `RulingDetailComponent` | Full ruling details |
| `/chat` | `ChatViewComponent` | RAG jurisprudential chat |

### Admin routes

| Route | Component | Description |
|---|---|---|
| `/admin` | `DashboardComponent` | Pipeline status |
| `/admin/crawlers` | `CrawlersComponent` | Crawler management and manual trigger |
| `/admin/jobs` | `JobsComponent` | Active, completed and failed jobs |
| `/admin/dlq` | `DeadLetterQueueComponent` | Dead Letter Queue per queue |
| `/admin/users` | `UsersComponent` | User CRUD |

### Authentication

Login via Microsoft Entra ID (MSAL Angular). Phase 1: all authenticated users have full access. Phase 2: `RoleGuard` introduced with roles `admin`, `lawyer` and `viewer`.

---

## Infrastructure

| Component | Azure Service | Phase 1 Tier |
|---|---|---|
| Database | Azure SQL Database | General Purpose 2 vCores (serverless) |
| Search | Azure AI Search | Basic |
| Files | Azure Blob Storage | LRS Standard |
| Messaging | Azure Storage Queues | 4 queues (same Storage Account as Blob) |
| Identity | Azure Entra ID | Microsoft 365 (already available) |
| API hosting | Azure App Service | B2 |
| SPA hosting | Azure Static Web Apps | Free / Standard |
| Workers | Azure Container Apps | Consumption plan (crawler/parser: 0.5 vCPU, 1GB RAM; enrichment/indexer: 1 vCPU, 2GB RAM) |
| Graph | Azure SQL (Citations, CTEs) | Included in SQL |

---

## Development phases

### Phase 1 — MVP
CSJN crawler (manual trigger) · Parser + Enrichment + Indexer · Complete Knowledge Base · Semantic search · RAG chat · Entra ID auth · Admin panel integrated in SPA

### Phase 2
SAIJ, PJN and SCBA crawlers · Automatic scheduling (cron) · User roles (`admin`, `lawyer`, `viewer`) · Ingestion traceability (`IngestionJobs`) · PJN/SCBA PDF quality validation

### Phase 3
Interactive visual graph · Jurisprudential timeline · Analytics dashboard · Notifications of relevant new rulings

### Phase 4
Law and decree ingestion · Judge profiles · Legal doctrine · Multi-type search

---

## Architecture decisions

| ID | Decision |
|---|---|
| ADR-001 | Cloud stack: Azure |
| ADR-002 | Chunking: 512 tokens, overlap 50, two index levels |
| ADR-003 | Auth: Entra ID. Phase 1 no roles, Phase 2 with roles from Entra ID groups |
| ADR-004 | Graph: Phase 1 in SQL (Citations, CTEs). Phase 2: evaluate Neo4j |
| ADR-005 | Total immutability. SHA-256 deduplication |
| ADR-006 | Single-tenant |
| ADR-007 | PDF parsing: PdfPig without Azure Document Intelligence in Phase 1 (post-extraction normalization) |
| ADR-008 | CSJN pipeline: API-first, GPT-4o only for gap-filling |
| ADR-009 | Messaging: Storage Queues (Phase 1), 4 queues per worker |
| ADR-010 | Worker hosting: Azure Container Apps consumption plan with KEDA |
| ADR-011 | Crawler triggers: manual in Phase 1, cron in Phase 2 |
| ADR-012 | Admin UI integrated in Angular SPA under `/admin/*` (no separate MVC project) |
| ADR-013 | Phase 1 roles: all authenticated users are admin (no role guards) |

---

> Full detail of each ADR: `docs/architecture/legal-ai-ar-architecture.md` section 2.

> Complete technical documentation: `docs/architecture/legal-ai-ar-architecture.md` · Specifications: `docs/architecture/legal-ai-ar-specs.md` · Roadmap: `docs/roadmap/ROADMAP.md`
