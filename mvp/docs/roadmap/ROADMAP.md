# Legal AI AR — Development Roadmap

> **Version**: 2.0 · **Date**: 2026-04-29 · **Scope**: Phases 0–4

---

## Conventions

| Symbol | Meaning |
|---|---|
| `[ ]` | Pending |
| `[x]` | Completed |
| `[~]` | In progress |
| `[!]` | Blocked — see note |

**Checks per deliverable**

Each deliverable has two independent columns:

- **DEV** — development complete, PR merged, tests passing in CI
- **AUD** — audited: code review approved + smoke test in staging + documentation updated if applicable

**Feature closure**: all deliverables with `[x] DEV` and `[x] AUD`.

**Design task**: each feature starts with a mandatory `T-00` task that produces the `.md` documents and `.mermaid` diagrams needed to start development. No other task in the feature may start until `T-00` has `[x] DEV` and `[x] AUD`.

**Deliverable numbering**: canonical range E001–E311. E001–E206 (original), E207–E214 (F0-3), E215–E231 (F1-17), E232–E249 (F1-18), E250–E299 (F1-21 sub-features), E300–E305 (F1-19), E306–E311 (F1-20).

---

## Index

- [Phase 0 — Foundations](#phase-0--foundations)
- [Phase 1 — MVP](#phase-1--mvp)
- [Phase 2 — Expansion](#phase-2--expansion)
- [Phase 3 — Advanced intelligence](#phase-3--advanced-intelligence)
- [Phase 4 — Full coverage](#phase-4--full-coverage)
- [Summary](#summary)
- [Closure criteria](#closure-criteria)

---

## Phase 0 — Foundations

> Base infrastructure, repository structure and shared contracts.
> **Prerequisite for all**: no Phase 1 feature may start until F0-1 and F0-2 are closed.

---

### F0-1 · Repository and project structure

**Objective**: Monorepo configured, .NET projects created with correct references, basic CI working.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E001 | `docs/design/f0-1-repo-structure.md` — monorepo structure decisions, naming conventions, branch and PR policy | `[x]` | `[ ]` |
| E002 | `docs/design/f0-1-dependencies.mermaid` — .NET project dependency diagram (Clean Architecture) | `[x]` | `[ ]` |
| E003 | `docs/design/f0-1-ci-pipeline.mermaid` — CI pipeline diagram (trigger → build → test → lint) | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Create repository with structure `backend/`, `frontend/`, `docs/` (optional: `infra/` for deploy scripts)
- [x] **T-02** Initialize `LegalAiAr.sln` with all .NET projects and layer references
- [x] **T-03** Implement `LegalAiAr.Core`: entities, enums, repository/service interfaces, Service Bus message contracts
- [x] **T-04** Create skeleton of `LegalAiAr.Infrastructure` with folders per provider
- [x] **T-05** Create worker and test projects with correct references
- [x] **T-06** Configure `.editorconfig`, `.gitignore`, branch policy
- [x] **T-07** Configure CI pipeline (build + tests on each PR)

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E004 | Repository initialized with complete structure | `[x]` | `[ ]` |
| E005 | `LegalAiAr.sln` with all projects referenced per Clean Architecture | `[x]` | `[ ]` |
| E006 | `LegalAiAr.Core` complete: entities, enums, interfaces, messages | `[x]` | `[ ]` |
| E007 | CI pipeline working: green build on test PR | `[x]` | `[ ]` |

---

### F0-2 · Azure Infrastructure — existing services

**Objective**: Configure connection to existing Azure services. Functional local environment.

Azure services (SQL, Blob, AI Search, OpenAI, Storage Queues, App Service, Static Web Apps, Container Apps) are **already provisioned**. Entra ID will be deferred to Phase 3. This feature focuses on obtaining credentials, configuring environment variables, and deploying applications on those resources.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E008 | `docs/design/f0-2-infrastructure.md` — Azure resources catalog: service, tier, purpose and key configuration | `[x]` | `[ ]` |
| E009 | `docs/design/f0-2-azure-network.mermaid` — network diagram: VNet, subnets, private access between resources (reference) | `[x]` | `[ ]` |
| E010 | `docs/design/f0-2-environment-variables.md` — exhaustive environment variables table per component (API, each worker) | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Obtain credentials, connection strings and endpoints from existing Azure services
- [x] **T-02** Apply initial schema to existing Azure SQL with EF Core migrations (all Phase 1 tables)
- [x] **T-03** Seed `Sources` table (CSJN=1, SAIJ=2, PJN=3, SCBA=4) and initial seed of `CrawlerConfigs`
- [x] **T-04** Create `rulings-pdfs` container in existing Azure Blob Storage (if it does not exist)
- [x] **T-05** Create `rulings-by-ruling` and `rulings-by-chunk` indexes in existing Azure AI Search (fields and vector dimensions)
- [x] **T-06** Verify gpt-4o and text-embedding-3-large deployments in existing Azure OpenAI
- [x] **T-07** Create existing Storage Queues: `csjn-ruling-crawler`, `csjn-ruling-parser`, `csjn-ruling-enrichment`, `csjn-ruling-indexer` (same Storage Account as Blob)
- [!] **T-08** Configure deploy on App Service, Static Web Apps and Container Apps — *Deferred: will run at Phase 1 closure, before Phase 2. Local environment until then.*
- [x] **T-09** Configure `docker-compose.yml` for local SQL Server (development)
- [x] **T-10** Verify connectivity to all existing Azure services

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E011 | Existing Azure SQL with complete schema and seeds applied | `[x]` | `[ ]` |
| E012 | Existing Azure Blob Storage with `rulings-pdfs` container accessible | `[x]` | `[ ]` |
| E013 | Existing Azure AI Search with both indexes created and verified | `[x]` | `[ ]` |
| E014 | Storage Queues with 4 operational queues (Phase 1: no Service Bus) | `[x]` | `[ ]` |
| E015 | App Service, Static Web Apps and Container Apps configured for deploy | `[ ]` | `[ ]` |
| E016 | `docker-compose.yml` runs SQL Server locally for development | `[x]` | `[ ]` |
| E017 | `.env.example` with all environment variables documented | `[x]` | `[ ]` |
| E018 | Connectivity verification to all existing Azure services | `[x]` | `[ ]` |

---

### F0-3 · Roadmap Dashboard (IDE internal tool)

**Objective**: IDE-internal tool to view roadmap progress after each ROADMAP.md update. Not part of the application — run script, open generated HTML in browser.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E207 | `docs/design/f0-3-roadmap-parser.md` — parser spec: regex/patterns for phases, features, deliverables, DEV/AUD checkboxes in ROADMAP.md | `[x]` | `[ ]` |
| E208 | `docs/design/f0-3-roadmap-dashboard-ui.md` — UX spec: layout, metrics (overall %, phase breakdown, feature list) | `[x]` | `[ ]` |
| E209 | `docs/design/f0-3-roadmap-dashboard-flow.mermaid` — data flow: ROADMAP.md → script → standalone HTML | `[x]` | `[ ]` |
| E210 | `docs/mockups/mockup-roadmap-dashboard.html` — dashboard page mockup per PwC guidelines (Designer deliverable) | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement Node script `scripts/roadmap-dashboard.js` that parses ROADMAP.md and generates standalone HTML
- [x] **T-02** Script outputs `docs/roadmap/roadmap-dashboard.html` with embedded data (no server, open in browser)
- [x] **T-03** Progress metrics: overall % closed, per-phase breakdown, per-feature summary
- [x] **T-04** Add `docs/roadmap/roadmap-dashboard.html` to .gitignore (generated file)

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E211 | `scripts/roadmap-dashboard.js` parsing ROADMAP.md and generating HTML | `[x]` | `[ ]` |
| E212 | Standalone HTML dashboard viewable in browser without server | `[x]` | `[ ]` |
| E213 | Run `node scripts/roadmap-dashboard.js` after each roadmap update | `[x]` | `[ ]` |
| E214 | Generated file in .gitignore | `[x]` | `[ ]` |

---

## Phase 1 — MVP

> First complete functional version. At Phase 1 closure the system can ingest CSJN rulings, index them and expose them via semantic search and agentic RAG chat with function calling (tools) and simulated authentication (logged-in user with admin role).
> **Prerequisite**: Phase 0 (F0-1 and F0-2) closed.

---

### F1-1 · Shared Infrastructure — base repositories and services

**Objective**: `LegalAiAr.Infrastructure` implemented with all providers needed for workers and API to function.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E020 | `docs/design/f1-1-infrastructure.md` — implementation decisions per provider: EF Core, Azure Search, Blob, Service Bus, OpenAI. Graphs in SQL (~~Neo4j in Phase 3~~ — cancelled, SQL permanent per F1-21 ADR) | `[x]` | `[ ]` |
| E021 | `docs/design/f1-1-repositories.mermaid` — class diagram: interfaces in Core vs implementations in Infrastructure | `[x]` | `[ ]` |
| E022 | `docs/design/f1-1-ef-schema.mermaid` — complete ER diagram of Azure SQL (all tables and relationships) | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `AppDbContext` with all `IEntityTypeConfiguration<T>`
- [x] **T-02** Implement all repositories: `RulingRepository`, `CourtRepository`, `JudgeRepository`, `KeywordRepository`, `StatuteRepository`, `CitationRepository`, `CrawlerConfigRepository`
- [x] **T-03** Implement `AzureSearchService` with hybrid search (vector + fulltext)
- [x] **T-04** Implement `AzureBlobStorageService`
- [x] **T-05** Implement `SqlGraphService` for relationship queries from Azure SQL (Rulings, Citations, RulingJudges, etc.). ~~Neo4j in Phase 3~~ — SQL permanent per F1-21 ADR
- [x] **T-06** Implement `ServiceBusQueuePublisher`
- [x] **T-07** Implement `OpenAiEmbeddingService` (`text-embedding-3-large`)
- [x] **T-08** Implement `OpenAiEnrichmentService` with structured output
- [x] **T-09** Register all services in `InfrastructureServiceExtensions`

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E023 | `AppDbContext` with all EF Core configurations | `[x]` | `[ ]` |
| E024 | All repositories implemented and unit tested | `[x]` | `[ ]` |
| E025 | `AzureSearchService` working with hybrid search | `[x]` | `[ ]` |
| E026 | `SqlGraphService` querying relationships from Azure SQL | `[x]` | `[ ]` |
| E027 | `ServiceBusQueuePublisher` publishing and consuming messages | `[x]` | `[ ]` |
| E028 | `OpenAiEmbeddingService` and `OpenAiEnrichmentService` operational | `[x]` | `[ ]` |

---

### F1-2 · Pipeline — CrawlerWorker (CSJN) ✓ DEV complete

**Objective**: CrawlerWorker detects new rulings in CSJN via manual trigger and enqueues them in `queue-parser`.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E029 | `docs/design/f1-2-crawler.md` — CSJN incremental discovery strategy: Selenium (fallos/consulta.html + paginarFallos.html), date pagination, new detection, throttling spec for rate limiting (R-002): intervals, backoff, env var config, defensive versioning strategy for CSJN API breaking changes (R-001) | `[x]` | `[ ]` |
| E030 | `docs/design/f1-2-crawler-flow.mermaid` — CrawlerWorker flow diagram: message receipt → discovery → deduplication → publish → retry/DLQ | `[x]` | `[ ]` |
| E031 | `docs/design/f1-2-deduplicacion.md` — SHA-256 deduplication strategy: when to compute hash, what to include, how to query | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `ICrawlerSource` with `DiscoverAsync()` and `GetContentHashAsync()`
- [x] **T-02** Implement `CsjnCrawlerSource`: pagination over CSJN rulings list with configurable throttling
- [x] **T-09** Implement defensive versioning: response schema validation, handling of new/removed fields, fallback for breaking changes (R-001)
- [x] **T-03** Implement deduplication: query `ContentHash` in `Rulings` before enqueueing
- [x] **T-04** Implement `CrawlerWorkerService` as `IHostedService` consuming `queue-crawler`
- [x] **T-05** Publish `ParserMessage` in `queue-parser` for each new document
- [x] **T-06** Configure retry with Polly (exponential backoff, max 3 attempts → DLQ)
- [x] **T-07** Create worker `Dockerfile`
- [x] **T-08** Write unit tests: `CsjnCrawlerSourceTests` (happy path, duplicates, simulated rate limit)
- [x] **T-10** Implement configurable throttling for rate limiting on CSJN API calls (R-002 mitigation)

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E032 | `CsjnCrawlerSource` discovering new documents correctly | `[x]` | `[ ]` |
| E033 | SHA-256 deduplication functional — duplicate documents discarded | `[x]` | `[ ]` |
| E034 | `CrawlerWorkerService` consuming `queue-crawler` and publishing to `queue-parser` | `[x]` | `[ ]` |
| E035 | Retry + DLQ configured and verified | `[x]` | `[ ]` |
| E036 | Crawler `Dockerfile` buildable and runnable | `[x]` | `[ ]` |
| E037 | Unit tests covering main cases (includes defensive versioning R-001 and throttling R-002) | `[x]` | `[ ]` |

---

### F1-3 · Pipeline — ParserWorker (CSJN)

**Objective**: ParserWorker consumes the 5 CSJN REST endpoints, extracts the PDF and normalizes the text.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E038 | `docs/design/f1-3-parser.md` — field-to-field mapping of each CSJN endpoint to `ExtractedMetadata`: `abrirAnalisis`, `getAllDocumentos`, `getSumariosAnalisis`, `getCitas`, `getCitantes` | `[x]` | `[ ]` |
| E039 | `docs/design/f1-3-parser-flow.mermaid` — API call sequence + PDF download + normalization + publish to `queue-enrichment` | `[x]` | `[ ]` |
| E040 | `docs/design/f1-3-normalizacion.md` — PDF text normalization rules: double spaces, spaced text (`S u p r e m a`), image headers, line breaks | `[x]` | `[ ]` |
| E041 | `docs/design/f1-3-missing-fields.md` — logic to determine `missingFields`: which fields come from API and which require GPT-4o | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `CsjnApiParser` with HTTP client for the 5 endpoints
- [x] **T-02** Map API responses to `ExtractedMetadata` and compute `missingFields`
- [x] **T-03** Implement `PdfTextExtractor` with PdfPig
- [x] **T-04** Implement normalization: collapse spaces, fix spaced text, strip image headers, normalize line breaks
- [x] **T-05** Read PDF from Blob Storage using message `blobPathPdf`
- [x] **T-06** Publish `EnrichmentMessage` in `queue-enrichment`
- [x] **T-07** Implement `ParserWorkerService` as `IHostedService`
- [x] **T-08** Create worker `Dockerfile`
- [x] **T-09** Write unit tests with fixtures of real CSJN responses and test PDFs

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E042 | `CsjnApiParser` mapping the 5 endpoints correctly | `[x]` | `[ ]` |
| E043 | `PdfTextExtractor` with complete normalization | `[x]` | `[ ]` |
| E044 | `missingFields` computed correctly per document | `[x]` | `[ ]` |
| E045 | PDF read from Blob Storage using `blobPathPdf` correctly | `[x]` | `[ ]` |
| E046 | `ParserWorkerService` consuming `queue-parser` and publishing to `queue-enrichment` | `[x]` | `[ ]` |
| E047 | Unit tests passing with real fixtures | `[x]` | `[ ]` |

---

### F1-4 · Pipeline — EnrichmentWorker

**Objective**: EnrichmentWorker extracts with GPT-4o the fields that the CSJN API does not provide.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E048 | `docs/design/f1-4-enrichment.md` — enrichment strategy per source: what GPT-4o extracts for CSJN vs other sources. Structured output decisions (json_schema) | `[x]` | `[ ]` |
| E049 | `docs/design/f1-4-prompts.md` — specification of the 3 prompts: judge extraction, cited statutes extraction, `CitationType` classification. Includes expected input/output examples | `[x]` | `[ ]` |
| E050 | `docs/design/f1-4-enrichment-flow.mermaid` — decision flow: receipt → strategy selection → GPT-4o calls → validation → publish | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `IEnrichmentStrategy` with `EnrichAsync(EnrichmentMessage)`
- [x] **T-02** Implement `CsjnEnrichmentStrategy`: processes only fields in `missingFields`
- [x] **T-03** Implement `JudgesExtractionPrompt`: extracts first/last name of signing judges
- [x] **T-04** Implement `StatutesExtractionPrompt`: extracts statute number and articles
- [x] **T-05** Implement `CitationType` classification by textual context
- [x] **T-06** Configure structured output with `response_format: json_schema` and validate result
- [x] **T-07** Publish complete `IndexerMessage` in `queue-indexer`
- [x] **T-08** Implement `EnrichmentWorkerService` as `IHostedService`
- [x] **T-09** Create worker `Dockerfile`
- [x] **T-10** Write unit tests with mock of `IOpenAiClient`

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E051 | `CsjnEnrichmentStrategy` calling GPT-4o only for `missingFields` | `[x]` | `[ ]` |
| E052 | All 3 prompts implemented with validated structured output | `[x]` | `[ ]` |
| E053 | Complete `IndexerMessage` published in `queue-indexer` | `[x]` | `[ ]` |
| E054 | `EnrichmentWorkerService` operational with `Dockerfile` | `[x]` | `[ ]` |
| E055 | Unit tests with OpenAI mocks | `[x]` | `[ ]` |

---

### F1-5 · Pipeline — IndexerWorker

**Objective**: IndexerWorker persists the complete ruling in all Knowledge Base stores with idempotency and retroactive citation resolution.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E056 | `docs/design/f1-5-indexer.md` — IndexerWorker step sequence, each step contract (input/output), idempotency policy by `ContentHash`, partial error handling | `[x]` | `[ ]` |
| E057 | `docs/design/f1-5-indexer-steps.mermaid` — sequence diagram: message → PersistRuling → UploadBlob → GenerateEmbeddings → IndexSearch → ResolveCitations (graph in SQL via relational tables) | `[x]` | `[ ]` |
| E058 | `docs/design/f1-5-citation-resolution.md` — retroactive resolution algorithm: test cases, match conditions (`ExternalAlias` vs `CaseNumber`), ambiguous citation handling | `[x]` | `[ ]` |
| E059 | `docs/design/f1-5-chunking.md` — chunking strategy: 512 tokens, overlap 50, short paragraph handling, encoding to use, chunk ID generation | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `PersistRulingStep`: upsert in `Rulings`, `Courts`, `Judges`, `RulingJudges`, `Keywords`, `RulingKeywords`, `Statutes`, `RulingStatutes`, `Citations`
- [x] **T-02** Implement idempotency: verify `ContentHash` before persisting, discard if already exists
- [x] **T-03** Implement chunking: split `FullText` into 512-token chunks with overlap 50
- [x] **T-04** Implement `GenerateEmbeddingsStep`: embedding of `Summary+Holding` (ruling level) and each chunk
- [x] **T-05** Implement `UploadBlobStep` (if PDF was not uploaded by Crawler)
- [x] **T-06** Implement `IndexSearchStep`: upsert in `rulings-by-ruling` and `rulings-by-chunk`
- [x] **T-07** Relationships (RulingJudges, RulingKeywords, RulingStatutes, Citations) persisted in SQL via `PersistRulingStep`
- [x] **T-08** Implement `ResolveCitationsStep`: resolve pending incoming and outgoing citations
- [x] **T-09** Implement `IndexerWorkerService` orchestrating steps in sequence
- [x] **T-10** Create worker `Dockerfile`
- [x] **T-11** Write unit tests for `ResolveCitationsStep` and `PersistRulingStep`

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E060 | `PersistRulingStep` with complete upsert and verified idempotency | `[x]` | `[ ]` |
| E061 | `GenerateEmbeddingsStep` producing ruling-level and chunk-level embeddings | `[x]` | `[ ]` |
| E062 | `IndexSearchStep` indexing in both Azure AI Search indexes | `[x]` | `[ ]` |
| E063 | Relationships persisted in SQL (RulingJudges, RulingKeywords, RulingStatutes, Citations) | `[x]` | `[ ]` |
| E064 | `ResolveCitationsStep` resolving retroactive citations correctly | `[x]` | `[ ]` |
| E065 | `IndexerWorkerService` orchestrating the 6 steps with `Dockerfile` | `[x]` | `[ ]` |
| E066 | Unit tests for critical steps passing | `[x]` | `[ ]` |

---

### F1-6 · API — Semantic search and ruling detail

**Objective**: API exposes hybrid search and complete ruling details with CQRS/MediatR.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E067 | `docs/design/f1-6-api-rulings.md` — detailed specification of the 3 endpoints: request/response schemas, available filters, pagination, error codes | `[x]` | `[ ]` |
| E068 | `docs/design/f1-6-search-flow.mermaid` — search flow sequence diagram: Controller → MediatR → Handler → EmbeddingService → SearchService → Mapper → Response | `[x]` | `[ ]` |
| E069 | `docs/design/f1-6-cqrs-structure.mermaid` — CQRS class diagram: Commands, Queries, Handlers, Behaviors, DTOs for Rulings module | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `SearchRulingsQuery` + `SearchRulingsHandler` with `ISearchService` + `IEmbeddingService`
- [x] **T-02** Implement `SearchFilters` (jurisdictionArea, instance, courtId, dateFrom, dateTo, keywords)
- [x] **T-03** Implement pagination (page, pageSize default 10, max 50)
- [x] **T-04** Implement `GetRulingByIdQuery` + `GetRulingByIdHandler`
- [x] **T-05** Implement `GetRelatedRulingsQuery` + `GetRelatedRulingsHandler`
- [x] **T-06** Implement DTOs and AutoMapper mapping profiles
- [x] **T-07** Implement `ValidationBehavior` (FluentValidation) and `LoggingBehavior`
- [x] **T-08** Implement `ExceptionHandlingMiddleware` with ProblemDetails (RFC 7807)
- [x] **T-09** Implement `RulingsController` with the 3 endpoints
- [x] **T-10** Register MediatR, AutoMapper and FluentValidation in `Program.cs`
- [x] **T-11** Write unit tests for handlers

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E070 | `POST /api/rulings/search` with hybrid search and filters working | `[x]` | `[ ]` |
| E071 | `GET /api/rulings/{id}` with complete ruling details (judges, keywords, citations) | `[x]` | `[ ]` |
| E072 | `GET /api/rulings/{id}/related` by semantic similarity | `[x]` | `[ ]` |
| E073 | MediatR pipeline with `ValidationBehavior` and `LoggingBehavior` active | `[x]` | `[ ]` |
| E074 | `ExceptionHandlingMiddleware` returning standardized ProblemDetails | `[x]` | `[ ]` |
| E075 | Handler unit tests passing | `[x]` | `[ ]` |

---

### F1-7 · API — Chat RAG

**Objective**: API answers legal questions with SSE streaming, citing specific rulings from the Knowledge Base.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E076 | `docs/design/f1-7-chat-rag.md` — RAG pipeline design: search parameters (top-K chunks and rulings), context construction strategy, token limit, citation format in response | `[x]` | `[ ]` |
| E077 | `docs/design/f1-7-chat-flow.mermaid` — complete sequence diagram: query → embedding → dual search → context → GPT-4o → SSE stream | `[x]` | `[ ]` |
| E078 | `docs/design/f1-7-rag-prompt.md` — complete legal RAG prompt: system prompt, citation instructions, citation format `{caseTitle, id}`, edge cases (no results, partial response) | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `ChatQueryCommand` + `ChatQueryHandler`
- [x] **T-02** Implement RAG logic: embedding → chunk search (top-K=10) + rulings (top-K=5) → context construction
- [x] **T-03** Implement and tune legal RAG prompt with citation instructions
- [x] **T-04** Implement GPT-4o response streaming to client via SSE
- [x] **T-05** Implement `ChatController` with SSE endpoint (`text/event-stream`)
- [x] **T-06** Write unit tests with search and OpenAI mocks

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E079 | `POST /api/chat` responding with SSE streaming | `[x]` | `[ ]` |
| E080 | Complete RAG pipeline with dual search working | `[x]` | `[ ]` |
| E081 | Response includes explicit citations `{caseTitle, id}` in consistent format | `[x]` | `[ ]` |
| E082 | Handler unit tests passing | `[x]` | `[ ]` |

---

### F1-8 · API — Administration

**Objective**: API exposes admin endpoints for crawlers, DLQ, users and health check.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E083 | `docs/design/f1-8-api-admin.md` — specification of all admin endpoints: request/response schemas, validations, crawler possible states, DLQ message lifecycle | `[x]` | `[ ]` |
| E084 | `docs/design/f1-8-crawler-states.mermaid` — crawler state diagram: idle → running → completed / failed → dlq | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `RunCrawlerCommand` + `RunCrawlerHandler` (validates enabled source, publishes `CrawlerMessage`)
- [x] **T-02** Implement `UpdateCrawlerConfigCommand` + handler
- [x] **T-03** Implement `GetCrawlersQuery` + handler
- [x] **T-04** Implement `GetDlqMessagesQuery` + `RequeueMessageCommand` via Service Bus Management API
- [x] **T-05** Implement user handlers (Create, Update, Delete, Get)
- [x] **T-06** Implement `HealthController` with SQL, Service Bus and Blob checks
- [x] **T-07** Implement admin controllers: CrawlersAdminController, JobsAdminController, DlqAdminController, UsersAdminController. Include GetPipelineStatusQuery + handler and endpoint GET /api/admin/pipeline/status. Include GetJobsQuery + handler and endpoint GET /api/admin/jobs (active/completed/failed pipeline jobs).

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E085 | `POST /api/admin/crawlers/{sourceId}/run` triggering manual crawl; GET /api/admin/pipeline/status and GET /api/admin/jobs implemented | `[x]` | `[ ]` |
| E086 | `GET /api/admin/crawlers` and `PATCH` updating configuration | `[x]` | `[ ]` |
| E087 | `GET /api/admin/dlq` and `POST .../requeue` managing failed messages | `[x]` | `[ ]` |
| E088 | User CRUD working | `[x]` | `[ ]` |
| E089 | `GET /api/health` with real dependency checks | `[x]` | `[ ]` |

---

### F1-9 · Authentication — Simulated (API)

**Objective**: Simulated login with admin role. All endpoints require valid simulated token. CORS configured for SPA. Entra ID will be implemented in Phase 3.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E090 | `docs/design/f1-9-auth.md` — simulated authentication flow: mock token issuance with admin role claim, API validation, simulated login/logout endpoints | `[x]` | `[ ]` |
| E091 | `docs/design/f1-9-auth-flow.mermaid` — sequence diagram: SPA → simulated login → mock token → API → validation → response | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement simulated login endpoint that issues mock JWT with `role: admin` claim
- [x] **T-02** Configure JWT validation to accept tokens from simulated provider
- [x] **T-03** Add `[Authorize]` to all controllers (except `/api/health` and `/api/auth/login`)
- [x] **T-04** Configure CORS for SPA domain
- [x] **T-05** Verify that requests without token return 401 with ProblemDetails

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E092 | API rejects requests without valid token with 401 | `[x]` | `[ ]` |
| E093 | CORS configured correctly for SPA domain | `[x]` | `[ ]` |
| E094 | Simulated login working: user obtains token with admin role and accesses app | `[x]` | `[ ]` |

---

### F1-10 · Frontend — Search and ruling detail ✓ Closed 2026-03-12

**Objective**: User can search rulings in natural language, view paginated results and access complete ruling details.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E095 | `docs/design/f1-10-frontend-search.md` — UX specification of Search, Results and RulingDetail views: fields, states (loading, empty, error), filter behavior | `[x]` | `[ ]` |
| E096 | `docs/design/f1-10-components.mermaid` — Angular component tree and data flow diagram for search module | `[x]` | `[ ]` |
| E097 | `docs/design/f1-10-state-flow.mermaid` — UI state diagram for search: idle → loading → results / empty / error | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Initialize Angular project with standalone components and configure `app.routes.ts`
- [x] **T-02** Implement `RulingService` with `search()`, `getById()`, `getRelated()`
- [x] **T-03** Implement `SearchHomeComponent` with `SearchBarComponent` and collapsible filters
- [x] **T-04** Implement `SearchResultsComponent` with pagination and loading/empty/error states
- [x] **T-05** Implement `RulingDetailComponent` with sections: summary, holding, judges, keywords, citations, full text
- [x] **T-06** Implement reusable `RulingCardComponent`
- [x] **T-07** Implement `LoadingSpinnerComponent` and pipes `RulingDatePipe`, `CitationTypeLabelPipe`
- [x] **T-08** Configure global styles in `styles.scss`

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E098 | `/buscar` functional with natural language search | `[x]` | `[ ]` |
| E099 | `/buscar/resultados` with pagination and relevant fragments | `[x]` | `[ ]` |
| E100 | `/fallos/:id` with complete ruling details rendering all fields | `[x]` | `[ ]` |
| E101 | Reusable `RulingCardComponent` and `RulingService` operational | `[x]` | `[ ]` |

---

### F1-11 · Frontend — Chat RAG

**Objective**: User can ask legal questions and receive streaming responses with navigable citations.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E102 | `docs/design/f1-11-chat-ui.md` — chat UX specification: layout, message states (streaming, complete, error), citation render format, SSE reconnection handling | `[x]` | `[ ]` |
| E103 | `docs/design/f1-11-sse-flow.mermaid` — sequence diagram: user types → `ChatService` opens EventSource → response chunks → citation parsing → render | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `ChatService` using `EventSource` to consume SSE
- [x] **T-02** Implement `ChatViewComponent` with history, auto-scroll and "typing" state
- [x] **T-03** Parse citations `{caseTitle, id}` in response and render as `RouterLink` to `/fallos/:id`
- [x] **T-04** Handle reconnection on stream disconnect
- [x] **T-05** Handle server errors (5xx) within stream

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E104 | `/chat` with SSE streaming working end-to-end | `[x]` | `[ ]` |
| E105 | Citations in response rendered as navigable links to ruling details | `[x]` | `[ ]` |
| E106 | Automatic reconnection on stream disconnect | `[x]` | `[ ]` |

---

### F1-12 · Frontend — Admin panel

**Objective**: Admin can monitor the pipeline and run manual crawls from the SPA.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E107 | `docs/design/f1-12-admin-ui.md` — admin panel UX specification: Dashboard, Crawlers, Jobs, DLQ, Users views; possible states, available actions per view | `[x]` | `[ ]` |
| E108 | `docs/design/f1-12-admin-components.mermaid` — Angular component tree for admin module and services consumed by each | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `CrawlerService`, `DlqService`, `UserService`
- [x] **T-02** Implement `DashboardComponent` with status cards per source
- [x] **T-03** Implement `CrawlersComponent` with table, status, last crawl, trigger button and confirmation modal
- [x] **T-04** Implement `JobsComponent` with 30-second polling
- [x] **T-05** Implement `DeadLetterQueueComponent` with tabs per queue and requeue action
- [x] **T-06** Implement `UsersComponent` with CRUD and create/edit form

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E109 | `/admin` with pipeline metrics per source | `[x]` | `[ ]` |
| E110 | `/admin/crawlers` with manual trigger and confirmation working | `[x]` | `[ ]` |
| E111 | `/admin/jobs` with polling and error details | `[x]` | `[ ]` |
| E112 | `/admin/dlq` with message requeue per queue | `[x]` | `[ ]` |
| E113 | `/admin/users` with full CRUD | `[x]` | `[ ]` |

---

### F1-13 · Frontend — Simulated authentication

**Objective**: Simulated login with admin user (without Entra ID). All routes protected. Entra ID will be implemented in Phase 3.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E114 | `docs/design/f1-13-auth-sim.md` — simulated login specification: login form, mock login API call, token storage, session handling | `[x]` | `[ ]` |
| E115 | `docs/design/f1-13-auth-guard-flow.mermaid` — `AuthGuard` flow diagram: request → valid token? → allow / redirect to login → simulated form → callback → original route | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `LoginComponent` with simple form (simulated username/password)
- [x] **T-02** Implement `AuthService` that calls simulated login endpoint and stores token
- [x] **T-03** Implement `AuthGuard` with `CanActivateFn`
- [x] **T-04** Implement `AuthInterceptor` that attaches Bearer token to all HTTP requests
- [x] **T-05** Implement `ErrorInterceptor` that handles 401 by redirecting to login
- [x] **T-06** Verify logout and session cleanup
- [x] **T-07** Verify redirect to `/buscar` after successful login

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E116 | Simulated login working: admin user accesses app | `[x]` | `[ ]` |
| E117 | `AuthGuard` protecting all routes — unauthenticated user redirected | `[x]` | `[ ]` |
| E118 | `AuthInterceptor` attaching Bearer token to all requests | `[x]` | `[ ]` |
| E119 | Expired session redirects to login without losing destination URL | `[x]` | `[ ]` |

---

### F1-14 · Deploy — Phase 1

**Objective**: Complete system deployed on Azure and verified end-to-end with real CSJN rulings.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E120 | `docs/design/f1-14-deploy.md` — deploy strategy: branching model, staging/production slots, rollback, environment variables per environment | `[x]` | `[ ]` |
| E121 | `docs/design/f1-14-cd-pipeline.mermaid` — CD pipeline diagram: merge to main → build → push container registry → deploy staging → smoke test → promote production | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Configure Azure App Service with deployment slots (staging / production)
- [x] **T-02** Configure Azure Static Web Apps with Angular build
- [x] **T-03** Create Azure Container Registry and push images of the 4 workers
- [ ] **T-04** Configure Container Apps with KEDA + Service Bus scalers for the 4 workers
- [x] **T-05** Configure CD pipeline (GitHub Actions / Azure DevOps)
- [ ] **T-06** Run initial CSJN crawl in staging with at least 100 rulings
- [ ] **T-07** Run end-to-end smoke test: crawl → indexing → search → chat
- [ ] **T-08** Promote to production after successful smoke test

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E122 | API deployed on Azure App Service B2 with variables configured | `[ ]` | `[ ]` |
| E123 | SPA deployed on Azure Static Web Apps publicly accessible | `[ ]` | `[ ]` |
| E124 | 4 workers deployed as Container Apps with KEDA active and scale-to-zero verified | `[ ]` | `[ ]` |
| E125 | CD pipeline deploying automatically to staging on each merge to `main` | `[ ]` | `[ ]` |
| E126 | End-to-end smoke test completed with real CSJN rulings | `[ ]` | `[ ]` |

---

### F1-16 · Ingestion traceability *(ex F2-5)* ✓ DEV complete

**Objective**: Each indexed ruling can be traced to the ingestion job that originated it. Admin sees complete execution history. Unblocks F1-15 Job Visibility (WI-2, WI-3).

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E154 | `docs/design/f1-16-trazabilidad.md` — `IngestionJob` lifecycle: states, counters, `ingestionJobId` propagation through the 4 pipeline messages, traceability queries | `[x]` | `[ ]` |
| E155 | `docs/design/f1-16-job-lifecycle.mermaid` — sequence diagram: Admin triggers crawl → job created → messages propagated → rulings indexed with FK → job closed with metrics | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Create EF Core migration for `IngestionJobs` table and `Rulings.IngestionJobId` column
- [x] **T-02** Update `CrawlerWorkerService`: create `IngestionJob` at start, update counters, close on completion
- [x] **T-03** Propagate `ingestionJobId` in all pipeline messages
- [x] **T-04** Update `PersistRulingStep` to save `IngestionJobId`
- [x] **T-05** Implement `GetIngestionJobsQuery` + handler
- [x] **T-06** Update `JobsComponent` with history and metrics per job

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E156 | `IngestionJobs` table created and `Rulings.IngestionJobId` FK active | `[x]` | `[ ]` |
| E157 | `CrawlerWorkerService` creating and closing jobs with metrics | `[x]` | `[ ]` |
| E158 | `ingestionJobId` propagated through to `PersistRulingStep` | `[x]` | `[ ]` |
| E159 | `GET /api/admin/jobs` returning history with metrics | `[x]` | `[ ]` |
| E160 | `JobsComponent` showing complete history with metrics | `[x]` | `[ ]` |

---

### F1-17 · Chat Tools — Function Calling for Jurisprudential Assistant ✓ DEV complete

**Objective**: Transform the chat assistant from a single-pass RAG pipeline into an agentic assistant with OpenAI function calling. The model dynamically invokes tools (filtered search, ruling detail, citation graph, statute lookup, aggregation) to gather precise information before composing the response. Frontend shows inline tool execution feedback.

**Dependencies**: F1-7 (Chat RAG) `[x] DEV`. No Phase 2+ dependencies.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E215 | `docs/design/f1-17-chat-tools-architecture.md` — agentic loop design: `IAgentChatService` contract, `IToolRegistry` pattern, tool execution context, max-iterations cap, error handling strategy, SSE protocol extension (`tool_start`/`tool_end` events), token budget considerations | `[x]` | `[ ]` |
| E216 | `docs/design/f1-17-agentic-loop.mermaid` — sequence diagram: user query → model invocation with tool definitions → tool_calls in stream → server-side tool execution → tool results appended → re-invoke model → repeat until text response → SSE stream to client | `[x]` | `[ ]` |
| E217 | `docs/design/f1-17-tool-catalog.md` — complete catalog of all tools: name, description, JSON schema, backend service mapping, result format. Tools: `search_rulings`, `get_ruling_detail`, `get_ruling_citations`, `get_related_rulings`, `search_by_statute`, `count_rulings`, `list_courts`, `list_judges` | `[x]` | `[ ]` |

#### Development tasks

- [x] **T-01** Implement `IAgentChatService` interface in Core with tool-aware streaming overload: accepts `IReadOnlyList<ChatTool>` definitions, returns `IAsyncEnumerable` of typed events (text chunk, tool call request, done)
- [x] **T-02** Implement `AzureOpenAiAgentChatService` in Infrastructure: wraps `Azure.AI.OpenAI` `ChatClient` with tool definitions in `ChatCompletionOptions.Tools`, handles `StreamingChatToolCallUpdate` accumulation, returns structured events
- [x] **T-03** Implement `IToolRegistry` and `ToolRegistration` in Application: register tool name → JSON schema + async execution handler. `ToolExecutionContext` carries `CancellationToken`, `IServiceProvider`, and per-request state
- [x] **T-04** Implement agentic loop in refactored `ChatQueryHandler`: invoke model → if tool_calls, resolve from registry, execute tools, append tool results as `ChatMessage` with role `Tool`, re-invoke → repeat until text response or max iterations (configurable, default 5). Log each tool invocation (name, params, duration, result size)
- [x] **T-05** Extend SSE protocol in `ChatController`: emit `event: tool_start\ndata: {"tool":"name","args":{...}}\n\n` when tool execution begins and `event: tool_end\ndata: {"tool":"name","resultCount":N}\n\n` when complete. Text chunks remain `data: {text}\n\n`. Backward compatible: clients ignoring `event:` field still work
- [x] **T-06** Implement `search_rulings` tool: embed query via `IEmbeddingService`, build `SearchFilters` from params (dateFrom, dateTo, jurisdictionArea, instance, courtName, keywords), call `ISearchService.SearchAsync`, format top-K results as structured text with rulingId for citations
- [x] **T-07** Implement `get_ruling_detail` tool: call `IRulingRepository.GetByIdAsync` with navigation properties, format complete metadata (court, judges with participation type, keywords, statutes with articles, summary, holding, dates, rulingDirection, isUnconstitutional)
- [x] **T-08** Implement `get_ruling_citations` tool: call `IGraphService.GetInboundCitationsAsync` and/or `GetOutboundCitationsAsync` based on `direction` param, resolve target ruling metadata, format as list with caseTitle, rulingId, date, citationType, externalAlias
- [x] **T-09** Implement `get_related_rulings` tool: call `ISearchService.SearchRelatedAsync`, format results with caseTitle, rulingId, date, court, summary, score
- [x] **T-10** Implement `search_by_statute` tool: add `FindRulingsByStatuteAsync(statuteName, statuteNumber?, articles?, topK)` to `IStatuteRepository` (joins Statutes → RulingStatutes → Rulings with LIKE matching), implement tool handler that calls it and formats results
- [x] **T-11** Implement `count_rulings` tool: add `CountAsync(filters)` to `IRulingRepository` using SQL COUNT(*) with same filter logic as search (jurisdictionArea, instance, courtName, dateFrom, dateTo, isUnconstitutional, keywords via RulingKeywords join). Tool returns `{"count": N, "filters_applied": {...}}`
- [x] **T-12** Implement `list_courts` and `list_judges` tools: `list_courts` queries Courts table with optional jurisdictionArea/instance filters (cap 50). `list_judges` queries Judges + RulingJudges with optional courtName filter, returns ordered by ruling count desc (cap 50)
- [x] **T-13** Update system prompt: describe available tools, when to use each, maintain existing citation format `{caso: "...", id: "..."}` and Spanish language rules. Instruct model to prefer `search_rulings` over relying on initial context when user asks for filtered/specific results
- [x] **T-14** Frontend: extend `ChatService` SSE parsing to detect `event:` field in SSE stream. Route `tool_start`/`tool_end` events to a new observable or callback. Default (no event field) remains text chunk. Backward compatible with non-tool responses
- [x] **T-15** Frontend: implement `ToolStatusChipComponent` — inline chip with spinner during execution, checkmark on completion, warning on error. Spanish labels per tool name (e.g. `search_rulings` → "Buscando jurisprudencia...")
- [x] **T-16** Frontend: integrate `ToolStatusChipComponent` in `ChatViewComponent` — chips appear inline in message flow between text blocks. Multiple sequential tools show multiple chips. Responsive for mobile
- [x] **T-17** Write unit tests: tool registry registration and resolution, each tool handler with mocked services (ISearchService, IRulingRepository, IGraphService, IStatuteRepository), agentic loop with simulated tool_calls, SSE event serialization

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E218 | `IAgentChatService` interface in Core + `AzureOpenAiAgentChatService` implementation handling tool definitions and `StreamingChatToolCallUpdate` accumulation | `[x]` | `[ ]` |
| E219 | `IToolRegistry` with `ToolRegistration` and `ToolExecutionContext` — tools registered at startup, resolved and executed at runtime | `[x]` | `[ ]` |
| E220 | Agentic loop in `ChatQueryHandler`: tool_calls → execute → re-invoke cycle with configurable max-iteration cap (default 5) and per-tool logging | `[x]` | `[ ]` |
| E221 | SSE protocol extended: `ChatController` emits `tool_start`/`tool_end` typed events. Backward compatible with existing clients | `[x]` | `[ ]` |
| E222 | `search_rulings` tool: filtered hybrid search via `ISearchService.SearchAsync` + `IEmbeddingService`, results formatted with rulingId for model citations | `[x]` | `[ ]` |
| E223 | `get_ruling_detail` tool: full ruling metadata via `IRulingRepository.GetByIdAsync` including judges, keywords, statutes, citations | `[x]` | `[ ]` |
| E224 | `get_ruling_citations` tool: inbound/outbound citation traversal via `IGraphService` with resolved target metadata | `[x]` | `[ ]` |
| E225 | `get_related_rulings` tool: semantic similarity via `ISearchService.SearchRelatedAsync` | `[x]` | `[ ]` |
| E226 | `search_by_statute` tool + `IStatuteRepository.FindRulingsByStatuteAsync` with LIKE matching on statute name/number and optional article filter | `[x]` | `[ ]` |
| E227 | `count_rulings` tool + `IRulingRepository.CountAsync` with SQL COUNT and filter logic. `list_courts` and `list_judges` tools with capped results | `[x]` | `[ ]` |
| E228 | System prompt updated with tool descriptions, usage instructions, and citation format preservation | `[x]` | `[ ]` |
| E229 | Frontend `ChatService` parsing `tool_start`/`tool_end` SSE events with backward compatibility | `[x]` | `[ ]` |
| E230 | `ToolStatusChipComponent` with spinner, completion state, and Spanish labels rendered inline in `ChatViewComponent` | `[x]` | `[ ]` |
| E231 | Unit tests: tool registry, all 8 tool handlers with mocked services, agentic loop, SSE event serialization | `[x]` | `[ ]` |

---

### F1-18 · Chat Pipeline — Multi-Stage Architecture ✓ DEV complete

**Objective**: Wrap the agentic chat executor (F1-17) with safety and quality layers: input moderation, query enrichment, output validation and response normalization. Transform the single-stage agentic loop into a professional-grade legal assistant pipeline with guardrails, citation verification and legal disclaimers.

**Prerequisite**: F1-17 closed.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E232 | `docs/design/f1-18-chat-pipeline-architecture.md` — pipeline stages design: `IChatPipelineStage` interface, `ChatPipelineOrchestrator`, stage ordering, fail-open vs fail-closed semantics, SSE streaming integration (pre-stream / post-stream / chunk-mode), `ChatPipeline` configuration model, GPT-4o-mini deployment decision | `[x]` | `[ ]` |
| E233 | `docs/design/f1-18-chat-pipeline-flow.mermaid` — sequence diagram: user query → input guardrail (classify) → query enricher (intent + entities) → agentic executor (F1-17 loop) → output guardrail (citation validation) → response finalizer (normalize + disclaimer) → SSE stream → client | `[x]` | `[ ]` |
| E234 | `docs/design/f1-18-guardrail-catalog.md` — input guardrail classification categories (`legal_query`, `greeting`, `clarification`, `out_of_scope`, `harmful`), prompt injection patterns, PII patterns, rejection response templates (Spanish), query enricher intent taxonomy (`search`, `detail`, `comparison`, `statistics`, `precedent_exploration`, `statute_research`, `general`), entity categories, output guardrail validation rules and severity levels | `[x]` | `[ ]` |

#### Development tasks

**Pipeline infrastructure**

- [x] **T-01** Implement `IChatPipelineStage` interface in Core with `ProcessAsync` contract. Implement `ChatPipelineOrchestrator` in Application that chains stages around the existing `ChatQueryHandler` agentic loop. Add `ChatPipelineOptions` configuration model with per-stage enabled/disabled toggles. Register pipeline in DI (`Program.cs`)

**Input guardrail (WI: chat-input-guardrail)**

- [x] **T-02** Implement `InputGuardrailStage`: rule-based fast layer (regex patterns for prompt injection detection, PII patterns for DNI/CUIT, scope keywords for legal domain), LLM classifier layer (`IGuardrailClassifier` interface in Core + `AzureOpenAiGuardrailClassifier` in Infrastructure using GPT-4o-mini with focused classification prompt). Two-layer execution: rules first (<10ms), LLM only if rules are inconclusive
- [x] **T-03** Implement rejection template system: `IGuardrailTemplateProvider` with configurable Spanish-language templates per classification category. Templates include explanation + 3-4 capability examples. Greeting handler returns friendly response with capability summary without invoking executor. All rejections streamed via SSE as normal text responses
- [x] **T-04** Handle `AgentFinishReason.ContentFilter` in `ChatQueryHandler`: when Azure returns `ContentFilter`, emit a user-friendly Spanish message ("No puedo procesar esta consulta. Por favor reformulá tu pregunta.") instead of silently ending the stream

**Query enricher (WI: chat-query-enricher)**

- [x] **T-05** Implement `QueryEnricherStage`: rule-based intent classifier (keyword/regex patterns: "¿cuántos" → `statistics`, "art."/"ley" → `statute_research`, "citar"/"citado por" → `precedent_exploration`, etc.) and rule-based entity extractor (regex for date ranges, court abbreviations, law references, "c/" case name patterns). Fast path handles ~60-70% of queries without LLM call
- [x] **T-06** Implement LLM enrichment fallback for ambiguous/complex queries: GPT-4o-mini call with structured output prompt returning JSON `{ intent, entities: { temporal, courts, statutes, cases, topics } }`. Implement context injection: append enriched metadata as additional system message to executor's message list. Graceful degradation: enricher failure → proceed with raw query

**Output guardrail (WI: chat-output-guardrail)**

- [x] **T-07** Implement `CitationParser` shared utility in Application: `Parse(text) → Citation[]` extracting `{caso: "...", id: "..."}` patterns, `Normalize(text) → string` converting citation variants to canonical format, `Validate(citation, knownIds) → ValidationResult`. Align regex with frontend `CITATION_REGEX`
- [x] **T-08** Implement `OutputGuardrailStage`: accumulate response text from `TextChunkEvent` yields, parse citations via `CitationParser`, batch-validate ruling IDs against database (`IRulingRepository` — single `SELECT Id, CaseTitle FROM Rulings WHERE Id IN (@ids)`), check title consistency (fuzzy match), check tool-grounding against `ToolExecutionContext.ResolvedRulingIds`. Produce `CitationValidationResult` with per-citation status
- [x] **T-09** Implement SSE validation event: emit `event: validation\ndata: {"status":"passed"|"warnings","details":[...]}\n\n` after last text chunk and before `[DONE]`. Append warning text to stream when serious issues detected ("Nota: Algunas referencias no pudieron ser verificadas..."). Enforce disclaimer on substantive legal responses. Strictness configurable: `lenient` (log only), `moderate` (append warnings), `strict` (block + warn)

**Response finalizer (WI: chat-response-finalizer)**

- [x] **T-10** Implement `ResponseFinalizerStage`: citation normalization in chunk-mode with 200-char look-ahead buffer for cross-chunk citations (extra whitespace, single quotes → double quotes, broken citations), markdown cleanup (unclosed markers, excessive newlines, orphan headings), empty response fallback ("No pude generar una respuesta. Por favor intentá reformular tu consulta."), conditional disclaimer injection using configurable template text

**Cross-cutting**

- [x] **T-11** Add `ChatPipeline` configuration section to `appsettings.json` with sub-sections: `InputGuardrail` (Enabled, UseLlmClassifier, ClassifierModel), `QueryEnricher` (Enabled, UseLlmFallback), `OutputGuardrail` (Enabled, Strictness, DisclaimerText), `ResponseFinalizer` (Enabled, DisclaimerEnabled, StructureEnforcement). Add `AzureOpenAI:MiniDeploymentName` for GPT-4o-mini deployment
- [x] **T-12** Unit tests: `ChatPipelineOrchestrator` stage chaining and short-circuit on guardrail rejection, `InputGuardrailStage` classification per category (rule-based + LLM mock), rejection template rendering, `QueryEnricherStage` intent detection and entity extraction, `CitationParser` parse/normalize/validate, `OutputGuardrailStage` with valid/invalid/mixed citations, `ResponseFinalizerStage` normalization and disclaimer injection. Mocked `IGuardrailClassifier` and `IRulingRepository`
- [x] **T-13** Frontend: extend `ChatService` to parse `event: validation` SSE event. Extend `ChatViewComponent` message model with optional `validation` field. Render validation status (checkmark for passed, warning icon for warnings with tooltip showing details) below assistant message

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E235 | `IChatPipelineStage` interface, `ChatPipelineOrchestrator` chaining stages around agentic executor, `ChatPipelineOptions` configuration model, DI registration | `[x]` | `[ ]` |
| E236 | Input guardrail rule-based layer: prompt injection detection, PII patterns, scope keywords. Classifying queries into `legal_query`, `greeting`, `clarification`, `out_of_scope`, `harmful` | `[x]` | `[ ]` |
| E237 | Input guardrail LLM classifier: `IGuardrailClassifier` + `AzureOpenAiGuardrailClassifier` using GPT-4o-mini for ambiguous queries | `[x]` | `[ ]` |
| E238 | Rejection templates: configurable Spanish-language responses per category with capability examples. Greeting handler bypassing executor | `[x]` | `[ ]` |
| E239 | `ContentFilter` finish reason handled in `ChatQueryHandler` with user-friendly Spanish message | `[x]` | `[ ]` |
| E240 | Query enricher rule-based layer: intent classification + entity extraction via keyword/regex patterns | `[x]` | `[ ]` |
| E241 | Query enricher LLM fallback: GPT-4o-mini structured output for complex queries. Context injection as system message to executor | `[x]` | `[ ]` |
| E242 | `CitationParser` shared utility: `Parse`, `Normalize`, `Validate` methods aligned with frontend `CITATION_REGEX` | `[x]` | `[ ]` |
| E243 | Output guardrail: citation ID batch validation against database, title consistency check, tool-grounding check via `ResolvedRulingIds` | `[x]` | `[ ]` |
| E244 | SSE `validation` event emitted after response. Warning text appended for unverified citations. Legal disclaimer enforced on substantive responses. Strictness configurable | `[x]` | `[ ]` |
| E245 | Response finalizer: citation normalization (chunk-mode with look-ahead buffer), markdown cleanup, empty response fallback, conditional disclaimer injection | `[x]` | `[ ]` |
| E246 | `ChatPipeline` configuration section in `appsettings.json` with per-stage toggles and `AzureOpenAI:MiniDeploymentName` | `[x]` | `[ ]` |
| E247 | Unit tests: pipeline orchestrator, input guardrail (rules + LLM mock), query enricher (rules + LLM mock), citation parser, output guardrail, response finalizer | `[x]` | `[ ]` |
| E248 | Frontend `ChatService` parsing `validation` SSE event. `ChatViewComponent` rendering validation status with icon and tooltip | `[x]` | `[ ]` |
| E249 | ADR-014 updated: GPT-4o-mini deployment added for lightweight classification tasks (guardrail, enricher) | `[x]` | `[ ]` |

---

### F1-19 · Interactive Legal Ontology Viewer

**Objective**: Interactive graph visualization of the legal ontology (classes, properties, relationships, taxonomies) with live KB instance counts. Standalone page at `/ontologia`.

**Dependencies**: F1-1 (Infrastructure). No Neo4j needed — ontology data is hard-coded class definitions + live SQL counts.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E300 | `docs/design/f1-19-ontology-viewer.md` — UX and API specification: 3 endpoints (`/api/ontology/classes`, `/api/ontology/graph`, `/api/ontology/stats`), Cytoscape.js graph, detail panel, taxonomy browser | `[x]` | `[ ]` |
| E301 | `docs/design/f1-19-ontology-graph-schema.mermaid` — graph schema: node types (Core Class, Subclass, Taxonomy, KB Entity), edge types (`is-a`, `relationship`) | `[x]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Implement `GetOntologyClassesQuery` + handler returning hard-coded class hierarchy with properties and relationships
- [ ] **T-02** Implement `GetOntologyGraphQuery` + handler returning node/edge graph data
- [ ] **T-03** Implement `GetOntologyStatsQuery` + handler returning live instance counts from DB (Rulings, Courts, Persons, Statutes, etc.)
- [ ] **T-04** Implement `OntologyController` with the 3 endpoints
- [ ] **T-05** Implement `OntologyPageComponent` with Cytoscape.js graph, detail panel, and taxonomy browser
- [ ] **T-06** Add `/ontologia` route and sidebar navigation entry

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E302 | `GET /api/ontology/classes` returning class hierarchy with properties and KB mapping | `[ ]` | `[ ]` |
| E303 | `GET /api/ontology/graph` returning graph nodes and edges for visualization | `[ ]` | `[ ]` |
| E304 | `GET /api/ontology/stats` returning live KB entity counts | `[ ]` | `[ ]` |
| E305 | `OntologyPageComponent` with interactive Cytoscape.js graph, detail panel, taxonomy browser | `[ ]` | `[ ]` |

---

### F1-20 · Knowledge Graph Explorer

**Objective**: Interactive graph visualization of real KB entity relationships. Users start from any entity and progressively expand the neighborhood. Entry points: standalone `/explorador` page and "Grafo" tab in ruling detail.

**Dependencies**: F1-1 (Infrastructure), F1-5 (`SqlGraphService`). Uses SQL graph queries — no Neo4j required.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E306 | `docs/design/f1-20-graph-explorer.md` — UX and API specification: neighborhood endpoint, entity search endpoint, node/edge styling, Cytoscape.js layout, layers panel | `[x]` | `[ ]` |
| E307 | `docs/design/f1-20-graph-data-model.mermaid` — data model: node types (Ruling, Court, Judge, Statute, Keyword), edge types (cites, signedBy, issuedBy, citesStatute, hasKeyword, normRelation) | `[x]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Implement `GetGraphNeighborhoodQuery` + handler querying `SqlGraphService` for 1-hop neighborhood
- [ ] **T-02** Implement `GET /api/graph/neighborhood/{entityType}/{entityId}` endpoint
- [ ] **T-03** Implement `GET /api/graph/search?q={query}&types={types}` for entity search within graph
- [ ] **T-04** Implement `GraphExplorerPageComponent` with Cytoscape.js, entity search, layers panel
- [ ] **T-05** Implement "Grafo" tab in `RulingDetailComponent` embedding the graph explorer
- [ ] **T-06** Add `/explorador` route and sidebar navigation entry

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E308 | `GET /api/graph/neighborhood/{entityType}/{entityId}` returning 1-hop neighborhood with progressive expansion | `[ ]` | `[ ]` |
| E309 | `GET /api/graph/search` returning entity matches for graph entry points | `[ ]` | `[ ]` |
| E310 | `GraphExplorerPageComponent` with interactive Cytoscape.js graph, search, and layers | `[ ]` | `[ ]` |
| E311 | "Grafo" tab in `RulingDetailComponent` embedding graph explorer for current ruling | `[ ]` | `[ ]` |

---

### F1-21 · CSJN Ontology with Real Data — Pipeline Enhancement + GraphRAG

**Objective**: Fix critical parser bugs, expand KB model from 5 to 8 CSJN API endpoints, integrate Contextual Retrieval for chunk search, implement GraphRAG with Azure SQL (recursive CTEs, multi-hop traversal), add Fallos Destacados discovery source, and regenerate KB with correct data.

**Dependencies**: F1-18 DEV complete. **Strategic decision**: GraphRAG runs on Azure SQL — **Neo4j deferred indefinitely**. The existing relational schema IS the knowledge graph; `SqlGraphService` with recursive CTEs provides multi-hop traversal without additional infrastructure.

**Full plan**: [`docs/csjn-analysis/PLAN-F1-21.md`](../csjn-analysis/PLAN-F1-21.md) — 50 deliverables (E250–E299), 4 sub-features.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E250 | `docs/csjn-analysis/README.md` — 8-endpoint analysis with population statistics, field audit, bug inventory | `[x]` | `[ ]` |
| E251 | `docs/csjn-analysis/{endpoint}.md` x 8 — per-endpoint field inventory, parser status, KB value | `[x]` | `[ ]` |
| E252 | `docs/csjn-analysis/PLAN-F1-21.md` — full task inventory, deliverable tracking | `[x]` | `[ ]` |
| E253 | `docs/design/f1-21-data-flow.mermaid` — 8 endpoints → Parser → Mapper → Enrichment → Indexer data flow | `[ ]` | `[ ]` |

#### F1-21a · Pipeline Completion + DB Migration (TG-A DONE + TG-B + temporal edges)

- [x] **TG-A** Parser + DTO fixes (E254–E268): all critical bugs fixed, DTOs expanded, mapper and enrichment optimized
- [ ] **T-04** PersistRulingStep + Citation entity expansion (E269–E270)
- [ ] **T-05** EF Core migration: 7 Ruling columns + 3 Citation columns + CreatedAt on 6 relationship tables (E271)
- [ ] **T-06** Search index expansion: new filterable/facetable fields (E272–E275)
- [ ] **T-07** Requeue handlers updated (E276–E277)
- [ ] **T-16** Temporal edges on 6 relationship entities (E291)

#### F1-21b · Contextual Retrieval + Search Quality

- [ ] **T-08** Metadata-prepended chunks — deterministic contextual retrieval (E278–E279)
- [ ] **T-09** Hybrid search on chunk index — BM25 + semantic (E280–E281)
- [ ] **T-10** `search_chunks` chat tool (E282–E283)
- [ ] **T-11** Search quality with real Summary/Holding (E284)
- [ ] **T-12** Chat tools with new fields (E285)

#### F1-21c · GraphRAG with Azure SQL

- [ ] **T-17** `SqlGraphService` real graph queries: citation chains, neighborhoods, shared entities, citation paths, judge networks (E292–E296)
- [ ] **T-18** GraphRAG chat tools: `explore_graph`, `find_connection`, enhanced citations (E297–E299)

#### F1-21d · Fallos Destacados + KB Regeneration

- [ ] **T-13** `CsjnFallosDestacadosSource` crawler source (E286–E288)
- [ ] **T-14** Admin API trigger for fallos destacados crawl (E289)
- [ ] **T-15** KB cleanup and regeneration — single pass with all fixes (E290)

#### Development deliverables (summary — see PLAN-F1-21 for full tracking)

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E254–E268 | TG-A: Parser fixes, DTO expansion, mapper optimization (15 deliverables) | `[x]` | `[ ]` |
| E269–E277 | F1-21a: Pipeline completion, migration, search index, requeue (9 deliverables) | `[ ]` | `[ ]` |
| E278–E285 | F1-21b: Contextual retrieval, hybrid chunk search, chat tools (8 deliverables) | `[ ]` | `[ ]` |
| E286–E290 | F1-21d: Fallos destacados source + KB regeneration (5 deliverables) | `[ ]` | `[ ]` |
| E291–E299 | F1-21c: Temporal edges + GraphRAG queries + chat tools (9 deliverables) | `[ ]` | `[ ]` |

---

## Phase 2 — Expansion

> New data sources, automatic crawler scheduling, user roles.
> **Prerequisite**: Phase 1 complete.
> **Note (v2.0)**: The expansion sequence is under review per the ontology convergence plan. SAIJ legislation (originally Phase 4 F4-1) may be pulled forward to before SAIJ rulings to fill the "Ordenamiento" ontological space.

---

### F2-1 · Pipeline — SAIJ (HTML+PDF)

**Objective**: Ingest SAIJ rulings with full HTML + PDF + GPT-4o enrichment sub-pipeline.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E127 | `docs/design/f2-1-saij.md` — SAIJ HTML structure analysis, field mapping to `ExtractedMetadata`, limitations and gaps covered by GPT-4o | `[ ]` | `[ ]` |
| E128 | `docs/design/f2-1-saij-flow.mermaid` — SAIJ sub-pipeline flow diagram: crawler → HTML scraping → PDF download → full enrichment → indexer | `[ ]` | `[ ]` |
| E129 | `docs/design/f2-1-full-enrichment-prompt.md` — `FullEnrichmentPrompt` specification: all fields to extract, input/output examples, missing field handling | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Analyze SAIJ HTML structure and define CSS/XPath selectors
- [ ] **T-02** Implement `SaijCrawlerSource`
- [ ] **T-03** Implement `HtmlParser` with basic metadata scraping
- [ ] **T-04** Implement `FullEnrichmentPrompt` and `FullEnrichmentStrategy`
- [ ] **T-05** Validate SAIJ PDF quality with PdfPig — update ADR-007 if needed
- [ ] **T-06** Enable SAIJ source in `CrawlerConfigs`
- [ ] **T-07** Smoke test: SAIJ crawl with 50 rulings and indexing verification

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E130 | `SaijCrawlerSource` discovering rulings incrementally | `[ ]` | `[ ]` |
| E131 | `HtmlParser` extracting SAIJ metadata correctly | `[ ]` | `[ ]` |
| E132 | `FullEnrichmentStrategy` with GPT-4o extracting all fields | `[ ]` | `[ ]` |
| E133 | ADR-007 updated per PDF validation result | `[ ]` | `[ ]` |
| E134 | SAIJ rulings indexed and searchable in the system | `[ ]` | `[ ]` |

---

### F2-2 · Pipeline — PJN and SCBA (HTML+PDF)

**Objective**: Ingest PJN and SCBA rulings. Unblocked by R-003 closure.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E135 | `docs/design/f2-2-pjn-scba.md` — PJN and SCBA PDF quality analysis (R-003 closure), HTML structure per source, gaps vs CSJN | `[ ]` | `[ ]` |
| E136 | `docs/design/f2-2-pjn-flow.mermaid` — PJN sub-pipeline flow | `[ ]` | `[ ]` |
| E137 | `docs/design/f2-2-scba-flow.mermaid` — SCBA sub-pipeline flow | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Evaluate PJN PDF sample with PdfPig — decide if Azure Document Intelligence is needed
- [ ] **T-02** Evaluate SCBA PDF sample
- [ ] **T-03** Update ADR-007 with final decision on Document Intelligence
- [ ] **T-04** Implement `PjnCrawlerSource` + specific HTML parser
- [ ] **T-05** Implement `ScbaCrawlerSource` + specific HTML parser
- [ ] **T-06** Enable PJN and SCBA sources in `CrawlerConfigs`

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E138 | R-003 closed with documented decision on PDF quality | `[ ]` | `[ ]` |
| E139 | `PjnCrawlerSource` and parser working | `[ ]` | `[ ]` |
| E140 | `ScbaCrawlerSource` and parser working | `[ ]` | `[ ]` |
| E141 | PJN and SCBA rulings indexed and searchable | `[ ]` | `[ ]` |

---

### F2-3 · Automatic crawler scheduling

**Objective**: Crawlers run automatically according to a cron configurable per source from the admin panel.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E142 | `docs/design/f2-3-scheduling.md` — scheduler design: chosen library (NCrontab / Quartz.NET), integration with `CrawlerWorkerService`, concurrency between sources, overlapping execution handling | `[ ]` | `[ ]` |
| E143 | `docs/design/f2-3-scheduler-states.mermaid` — scheduler state diagram per source: disabled → scheduled → triggered → running → completed / failed | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Enable `CronExpression` field in `CrawlerConfigs` (already in schema)
- [ ] **T-02** Implement scheduler in `CrawlerWorkerService` using NCrontab or Quartz.NET
- [ ] **T-03** Add cron expression validation in `UpdateCrawlerConfigHandler`
- [ ] **T-04** Update `PATCH /api/admin/crawlers/{sourceId}` to accept `cronExpression`
- [ ] **T-05** Add cron editor in `CrawlersComponent` with next execution preview
- [ ] **T-06** Handle overlap: do not trigger crawl if one is already running for the same source

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E144 | Scheduler running crawls automatically per `CronExpression` | `[ ]` | `[ ]` |
| E145 | `PATCH` accepts and validates `cronExpression` | `[ ]` | `[ ]` |
| E146 | UI allows editing cron with next execution preview | `[ ]` | `[ ]` |
| E147 | Execution overlap prevented | `[ ]` | `[ ]` |

---

### F2-4 · User roles

**Objective**: Differentiated access by role `admin`, `lawyer` and `viewer` in API and SPA. In Phase 2 roles are used in simulated token. Entra ID group integration is done in Phase 3 (F3-5).

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E148 | `docs/design/f2-4-roles.md` — permission matrix per role and endpoint, role claim issuance in simulated token, ADR-003 and ADR-013 update | `[ ]` | `[ ]` |
| E149 | `docs/design/f2-4-roles-matrix.mermaid` — access diagram: which routes/endpoints each role can access | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Extend simulated login to support roles `admin`, `lawyer`, `viewer` (parameter or default user)
- [ ] **T-02** Emit `role` claim in simulated token JWT
- [ ] **T-03** Add `[Authorize(Roles = "admin")]` on admin controllers
- [ ] **T-04** Add `[Authorize(Roles = "admin,lawyer")]` on chat and related
- [ ] **T-05** Implement `RoleGuard` in Angular
- [ ] **T-06** Hide navigation to `/admin/*` for non-admin roles
- [ ] **T-07** Update ADR-003 and ADR-013

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E150 | Roles in simulated token (admin, lawyer, viewer) working | `[ ]` | `[ ]` |
| E151 | API rejects out-of-role access with 403 | `[ ]` | `[ ]` |
| E152 | `RoleGuard` in Angular hiding sections according to role | `[ ]` | `[ ]` |
| E153 | ADR-003 and ADR-013 updated | `[ ]` | `[ ]` |

---

## Phase 3 — Advanced intelligence

> ~~Graph migration to Neo4j,~~ Entra ID, graph visualization, analytics and notifications.
> **Prerequisite**: Phase 2 complete.
>
> **ADR (v2.0)**: Neo4j has been **deferred indefinitely**. `SqlGraphService` with recursive CTEs (implemented in F1-21c) provides multi-hop graph traversal, neighborhood queries, citation chain resolution, and path finding on Azure SQL — sufficient for all planned graph features without additional infrastructure. Evaluate Neo4j only if Azure SQL recursive CTEs hit performance limits at >100K rulings. E160–E164 from the original F3-0 feature are **cancelled** (E160 of F1-16 remains valid).

---

### ~~F3-0 · Graph migration to Neo4j~~ — CANCELLED

> **Cancelled in v2.0**. Replaced by F1-21c (GraphRAG with Azure SQL). See ADR note above. Original deliverables E160–E164 (F3-0 numbering, distinct from F1-16 E160) are void.

---

### F3-1 · Interactive visual graph

**Objective**: User can visually explore relationships between rulings, judges and laws. ~~Requires F3-0 (Neo4j).~~ **Superseded by F1-19 + F1-20** (ontology viewer and graph explorer, designed over SQL). Retained for reference; implementation uses F1-19/F1-20 specs.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E165 | `docs/design/f3-1-graph-api.md` — superseded by `f1-20-graph-explorer.md` (neighborhood endpoint, SQL queries via `SqlGraphService`) | `[ ]` | `[ ]` |
| E166 | `docs/design/f3-1-graph-ui.md` — superseded by `f1-20-graph-explorer.md` (Cytoscape.js, layers panel) | `[ ]` | `[ ]` |
| E167 | `docs/design/f3-1-graph-schema.mermaid` — superseded by `f1-20-graph-data-model.mermaid` | `[ ]` | `[ ]` |

#### Development tasks

> **Note (v2.0)**: These tasks are superseded by F1-20 development tasks. See F1-20 for the implementation plan using `SqlGraphService` and Cytoscape.js.

- [ ] ~~**T-01** Implement `GetRulingGraphQuery` + handler querying Neo4j~~ → see F1-20 T-01
- [ ] ~~**T-02** Implement `GET /api/rulings/{id}/graph`~~ → see F1-20 T-02 (`/api/graph/neighborhood`)
- [ ] ~~**T-03** Implement `GraphComponent` with D3.js or Cytoscape.js~~ → see F1-20 T-04
- [ ] ~~**T-04** Implement node navigation with ruling preview in side panel~~ → see F1-20 T-04
- [ ] ~~**T-05** Integrate `GraphComponent` in `RulingDetailComponent`~~ → see F1-20 T-05

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E168 | Superseded by F1-20 E308 (`GET /api/graph/neighborhood` returning subgraph from `SqlGraphService`) | `[ ]` | `[ ]` |
| E169 | Superseded by F1-20 E310 (`GraphExplorerPageComponent` with Cytoscape.js) | `[ ]` | `[ ]` |
| E170 | Superseded by F1-20 E311 ("Grafo" tab in `RulingDetailComponent`) | `[ ]` | `[ ]` |

---

### F3-2 · Jurisprudential timeline

**Objective**: Chronological visualization of the evolution of a jurisprudential line through the citation chain. Uses `SqlGraphService.GetCitationChainAsync` (F1-21c E292) — no Neo4j required.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E171 | `docs/design/f3-2-timeline.md` — citation chain construction algorithm: recursive CTE over Azure SQL Citations table, max depth, cycle handling, chronological ordering | `[ ]` | `[ ]` |
| E172 | `docs/design/f3-2-timeline-ui.md` — timeline UX specification: horizontal/vertical layout, representation by `CitationType`, interaction per node | `[ ]` | `[ ]` |
| E173 | `docs/design/f3-2-timeline-flow.mermaid` — citation chain construction algorithm flow diagram (recursive CTE on Azure SQL) or endpoint call sequence | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Implement `GetRulingTimelineQuery` + handler using `SqlGraphService.GetCitationChainAsync`
- [ ] **T-02** Implement `GET /api/rulings/{id}/timeline`
- [ ] **T-03** Implement `TimelineComponent`
- [ ] **T-04** Integrate timeline in `RulingDetailComponent`

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E174 | `GET /api/rulings/{id}/timeline` returning chronological citation chain | `[ ]` | `[ ]` |
| E175 | `TimelineComponent` rendering jurisprudential evolution | `[ ]` | `[ ]` |

---

### F3-3 · Analytics dashboard

**Objective**: Knowledge Base metrics and jurisprudential trends visible to admin.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E176 | `docs/design/f3-3-analytics.md` — metrics to expose: rulings by source/court/period, most cited rulings, most frequent chat queries. Azure SQL aggregation design | `[ ]` | `[ ]` |
| E177 | `docs/design/f3-3-dashboard-layout.mermaid` — analytics dashboard wireframe with sections and metrics | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Implement `GET /api/admin/analytics` with aggregated metrics
- [ ] **T-02** Implement most cited rulings ranking from Azure SQL (`COUNT` on Citations grouped by `TargetRulingId`)
- [ ] **T-03** Implement chat query logging for usage metrics
- [ ] **T-04** Implement `AnalyticsDashboardComponent`

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E178 | Analytics endpoint with metrics by source, court and period | `[ ]` | `[ ]` |
| E179 | Most cited rulings ranking | `[ ]` | `[ ]` |
| E180 | `AnalyticsDashboardComponent` with interactive charts | `[ ]` | `[ ]` |

---

### F3-4 · New ruling notifications

**Objective**: Users receive email alerts when rulings relevant to their subscriptions are indexed.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E181 | `docs/design/f3-4-notificaciones.md` — subscription model: subscription by keywords / court / judge, matching logic on index, email provider (SendGrid / Azure Communication Services), frequency (immediate / daily) | `[ ]` | `[ ]` |
| E182 | `docs/design/f3-4-notif-flow.mermaid` — flow: IndexerWorker indexes ruling → evaluates subscriptions → enqueues notifications → email service dispatches them | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Design and create schema for `Subscriptions` and `NotificationLog` tables
- [ ] **T-02** Implement subscription matching in `IndexerWorker`
- [ ] **T-03** Integrate email provider
- [ ] **T-04** Implement `GET/POST/DELETE /api/subscriptions`
- [ ] **T-05** Implement `SubscriptionsComponent` in SPA

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E183 | Subscription model created in Azure SQL | `[ ]` | `[ ]` |
| E184 | Notifications sent when indexing rulings that match subscriptions | `[ ]` | `[ ]` |
| E185 | `SubscriptionsComponent` with user subscription management | `[ ]` | `[ ]` |

---

### F3-5 · Authentication — Entra ID

**Objective**: Migrate from simulated authentication to Microsoft Entra ID. App registration, real JWT, MSAL in frontend, groups and roles from Entra ID.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E186 | `docs/design/f3-5-entra-id.md` — migration flow: app registration in Entra ID, scopes, redirect URIs, role issuance from groups, simulated login replacement | `[ ]` | `[ ]` |
| E187 | `docs/design/f3-5-entra-flow.mermaid` — sequence diagram: SPA → Entra ID → token → API → validation | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Register app in Entra ID, configure scopes and redirect URIs
- [ ] **T-02** Create groups in Entra ID: `legal-ai-admin`, `legal-ai-lawyer`, `legal-ai-viewer`
- [ ] **T-03** Configure role issuance in JWT claims from Entra ID
- [ ] **T-04** Replace simulated authentication with `Microsoft.Identity.Web` in API
- [ ] **T-05** Replace simulated login with MSAL (`@azure/msal-angular`) in SPA
- [ ] **T-06** Verify Entra ID tokens are accepted and roles applied correctly

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E188 | Entra ID app registered, test token obtainable | `[ ]` | `[ ]` |
| E189 | API validating Entra ID JWT and applying group roles | `[ ]` | `[ ]` |
| E190 | SPA with MSAL: login with Entra ID, logout, token renewal | `[ ]` | `[ ]` |

---

## Phase 4 — Full coverage

> Corpus expansion beyond jurisprudence.
> **Prerequisite**: Phase 3 complete.
> **Note (v2.0)**: F4-1 (Legislation ingestion) may be executed earlier per the ontology convergence plan (Fase 3b). Judge profiles (F4-2) can leverage `SqlGraphService.GetJudgeRulingNetworkAsync` (F1-21c E296) instead of Neo4j.

---

### F4-1 · Legislation ingestion

**Objective**: Laws and decrees indexed and searchable via search and chat alongside rulings.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E191 | `docs/design/f4-1-legislacion.md` — extended data model for statutes: articles, validity, amendments, relationship with citing rulings. Source: SAIJ legislation | `[ ]` | `[ ]` |
| E192 | `docs/design/f4-1-er-legislacion.mermaid` — ER diagram of new legislation tables and their relationships with existing model | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Extend data model for statutes (articles, validity, amendments)
- [ ] **T-02** Implement crawler for SAIJ legislation
- [ ] **T-03** Extend Azure AI Search with `statutes-by-article` index
- [ ] **T-04** Implement multi-type search: rulings + laws in same result
- [ ] **T-05** Extend RAG prompt to include legislation context

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E193 | SAIJ legislation crawler indexing laws and decrees | `[ ]` | `[ ]` |
| E194 | Multi-type search working with mixed results | `[ ]` | `[ ]` |
| E195 | Chat RAG responding with legislation + jurisprudence context | `[ ]` | `[ ]` |

---

### F4-2 · Judge profiles

**Objective**: Complete profile for each judge with ruling history, positions and frequent topics.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E196 | `docs/design/f4-2-perfiles-jueces-api.md` — GET /api/judges/{id} and GET /api/judges endpoint design: schemas, SQL queries via `SqlGraphService.GetJudgeRulingNetworkAsync` (F1-21c E296) | `[ ]` | `[ ]` |
| E197 | `docs/design/f4-2-perfiles-jueces.md` — data to expose in profile: ruling history, frequent keywords, dissent percentage, frequent co-signers. SQL aggregation queries | `[ ]` | `[ ]` |
| E198 | `docs/design/f4-2-judge-profile-ui.md` — judge profile UX specification: sections, charts, links to rulings | `[ ]` | `[ ]` |
| E199 | `docs/design/f4-2-judge-profile-schema.mermaid` — GET /api/judges/{id} endpoint schema or profile data diagram | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Implement `GET /api/judges/{id}` with history and statistics
- [ ] **T-02** Implement `GET /api/judges` with name search
- [ ] **T-03** Implement `JudgeProfileComponent`
- [ ] **T-04** Link judges from `RulingDetailComponent`

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E200 | `GET /api/judges/{id}` with judge history and metrics | `[ ]` | `[ ]` |
| E201 | `JudgeProfileComponent` with history, positions and frequent keywords | `[ ]` | `[ ]` |

---

### F4-3 · Doctrine and academic articles

**Objective**: Ingestion and search of legal doctrine related to indexed jurisprudence.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E202 | `docs/design/f4-3-doctrina.md` — doctrine ingestion model: manual PDF upload, required metadata (author, year, area), indexing in Azure AI Search, semantic relationship with rulings | `[ ]` | `[ ]` |
| E203 | `docs/design/f4-3-doctrina-search.mermaid` — multi-corpus search flow: query → parallel search in jurisprudence + legislation + doctrine → unified ranking | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Extend data model with `DoctrineDocuments` table
- [ ] **T-02** Implement manual upload of doctrine PDFs from admin
- [ ] **T-03** Extend enrichment pipeline for doctrine documents
- [ ] **T-04** Implement unified multi-corpus semantic search
- [ ] **T-05** Extend RAG prompt to include doctrine context

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E204 | Upload and indexing of doctrine documents from admin | `[ ]` | `[ ]` |
| E205 | Multi-corpus semantic search (jurisprudence + legislation + doctrine) | `[ ]` | `[ ]` |
| E206 | Chat RAG with full context (jurisprudence + legislation + doctrine) | `[ ]` | `[ ]` |

---

## Summary

> Updated in v2.0 to include F1-16, F1-17, F1-18, F1-19, F1-20, F1-21. F3-0 cancelled.

| Phase | Features | Design deliverables | Development deliverables | Total deliverables |
|---|:---:|:---:|:---:|:---:|
| Phase 0 — Foundations | 3 | 10 | 16 | 26 |
| Phase 1 — MVP | 20 | 55 | 155 | 210 |
| Phase 2 — Expansion | 4 | 10 | 17 | 27 |
| Phase 3 — Advanced intelligence | 6 (1 cancelled) | 12 | 14 | 26 |
| Phase 4 — Full coverage | 3 | 8 | 8 | 16 |
| **Total** | **36** | **95** | **210** | **305** |

**Deliverable range**: E001–E311 (E160–E164 from F3-0 are void; F1-16 E160 remains valid).

---

## Closure criteria

### Task `[x]`

- Code written and working locally
- No `TODO` or `FIXME` in delivered code
- Unit tests written if the task requires them

### Deliverable `[x] DEV`

- All tasks of the deliverable completed
- PR merged to feature branch
- CI pipeline green (build + tests)

### Deliverable `[x] AUD`

- Code review approved by at least one team member
- Manual smoke test in staging
- Documentation updated if deliverable affects architecture, public API or data model

### Task T-00 (design) `[x] DEV` + `[x] AUD`

- All `.md` documents written and placed in `docs/design/`
- All `.mermaid` diagrams rendering correctly
- Reviewed by team before starting any development task of the feature

### Feature closed

- Task T-00 closed (`[x] DEV` + `[x] AUD`)
- All development deliverables with `[x] DEV` and `[x] AUD`
- Roadmap entry updated with closure date
