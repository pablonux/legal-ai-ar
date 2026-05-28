# Gap Analysis: MVP (legal-ai-ar) vs Legal Ai Ar

> **Date:** May 2026
> **Goal:** Identify what from the existing MVP can be reused, what gaps exist, and which technical decisions must be revisited for the new Legal Ai Ar project.

---

## 1. Executive Summary

The MVP is a **surprisingly mature** project — much more than a prototype. It has a 6-stage ingestion pipeline with a strategy pattern per source, an agentic chat with tool calling and 13 tools, hybrid search, graph community detection, an integrated SAIJ thesaurus, and a complete Angular frontend with ~15 functional views including an admin panel.

Most of the core capabilities Legal Ai Ar plans already exist in some form in the MVP. The main gaps are in **sophistication** (specialized agents vs a generic one, prompt registry vs hardcoded, a nonexistent eval framework) and in **ops** (no observability, IaC, or Key Vault).

**Verdict:** The MVP is a solid base. We recommend migrating and evolving, not rewriting.

---

## 2. Stack Comparison

| Aspect | MVP (legal-ai-ar) | Legal Ai Ar (plan) | Gap |
|---|---|---|---|
| Backend | ASP.NET Core 10, Controllers | .NET 10, Minimal API | Low — refactor controllers to Minimal API |
| Frontend | Angular (standalone components) | Angular 19 (standalone) | Low — same base |
| UI Library | PwC AppKit 4 (partial) | PwC AppKit 4 | Low — already has guidelines |
| ORM | EF Core 10 (Code-First) | EF Core 10 (Code-First) | None |
| AI Orchestration | Azure OpenAI SDK direct | Semantic Kernel | **High** — rewrite of the agent layer |
| Workers | BackgroundService (queue polling) | Azure Functions (event-driven) | **High** — hosting model change |
| Messaging | Azure Storage Queues | Azure Storage Queues | None |
| DB | Azure SQL (relational) | Azure SQL + SQL Graph | Medium — add edge tables |
| Search | Azure AI Search (hybrid) | Azure AI Search (hybrid) | None |
| Embeddings | text-embedding-3-large (3072d) | text-embedding-3-large (3072d) | None |
| LLM | GPT-4o | GPT-4o | None |
| Blob | Azure Blob Storage | Azure Blob Storage | None |
| PDF Parsing | PdfPig | Azure Document Intelligence | Medium — PdfPig works but does no OCR |
| Auth | Entra ID (custom JWT) | Entra ID (MSAL) | Low |
| Secrets | appsettings / env vars | Azure Key Vault | Medium |
| CI/CD | GitHub Actions (CI + CD staging) | GitHub Actions (4 environments) | Medium |
| IaC | None | Azure Bicep | **High** — does not exist |
| Observability | None | OpenTelemetry + App Insights | **High** — does not exist |
| Testing | xUnit + NSubstitute | xUnit + Moq + FluentAssertions | Low — same framework |
| Mediator | Custom IMediator | MediatR | Low — direct replacement |

---

## 3. Data Model — Detailed Mapping

### 3.1 Entities that already exist and can be reused

| Legal Ai Ar Entity | MVP Entity | Coverage | Notes |
|---|---|---|---|
| LegalNorm | `Statute` | **95%** | Has NormType, NormativeLevel, dates, status, issuing body. Very complete. |
| CaseLaw | `Ruling` | **100%** | 40+ fields. Richer than planned: includes Vote, ProsecutorOpinion, Headnote. |
| Article | `RulingStatuteArticle` | **70%** | Has article + subsection but is not a standalone entity with its own text. |
| Doctrine | `LegalDoctrine` | **100%** | Statement, topic, overruled tracking, binding weight. |
| CaseFile | `JudicialProceeding` | **90%** | ProcessType, Status, parties. Missing a movements timeline. |
| Person | `Person` | **100%** | Physical/Legal, verified flag, position, institution. |
| Court | `Court` + `JudicialOffice` | **100%** | With office→court hierarchy. |
| Thesaurus | `ThesaurusTerm` + `ThesaurusRelation` | **100%** | NT/BT/RT/UF relations. SAIJ integrated. |
| Keyword | `Keyword` + `RulingKeyword` + `SumarioKeyword` | **100%** | Normalization included. |
| Citation | `Citation` (6 types) | **90%** | UPHOLDS, OVERRULES, DISTINGUISHES, CITES, FOLLOWS, DISSENTS_FROM. |
| NormRelation | `NormRelation` (4 types) | **80%** | DEROGATES, AMENDS, REGULATES, COMPLEMENTS. Missing Interprets and Applies. |
| Vote | `Vote` | **100%** | Majority/dissent/concurrence with signatories. Not planned in Legal Ai Ar. |
| Prosecutor opinion | `ProsecutorOpinion` | **100%** | Not planned in Legal Ai Ar. |
| Representation | `LegalRepresentation` | **100%** | Lawyer-party. Not planned in Legal Ai Ar. |
| GraphCommunity | `GraphCommunity` + `CommunityMembership` | **100%** | Hierarchical with LLM summaries. Not explicitly planned in Legal Ai Ar. |
| FieldProvenance | `FieldProvenance` | **100%** | Field-by-field traceability with inference method. More granular than planned. |
| DocumentStageLog | `DocumentStageLog` | **100%** | Per-document pipeline tracking. |

### 3.2 Entities missing in the MVP

| Legal Ai Ar Entity | Gap | Impact |
|---|---|---|
| **Clause** | Does not exist. Articles are tracked as a string, not as an entity with text | Medium — needed for granular RAG per clause |
| **Movement** | Does not exist. JudicialProceeding has no event timeline | Medium — required for the procedural agent |
| **Deadline** | Does not exist. There is no procedural deadline calculation | High — core feature of the procedural agent |
| **UserPreferences** | `User` exists but without preferences (branch, alerts) | Low |
| **Conversation / ChatMessage** | No persistence of chat conversations | **High** — chat is stateless in the MVP |
| **RiskAnalysis** | Does not exist | Medium — Release 3.0 feature |
| **ArticleVersion** | No temporal versioning of articles | Medium — needed for temporal queries |
| **PromptTemplate** | Prompts hardcoded in C# | **High** — needed for prompt management |
| **ResponseFeedback** | Does not exist | **High** — needed for continuous improvement |
| **LegalTaxonomy** | Partially covered by ThesaurusTerm | Low — extend what exists |

### 3.3 MVP entities Legal Ai Ar did not plan (and should consider)

| MVP Entity | Value | Recommendation |
|---|---|---|
| `Vote` (judges' votes) | High for case law analysis. Allows knowing each judge's position. | **Incorporate into Legal Ai Ar** |
| `ProsecutorOpinion` (prosecutor opinion) | Medium. Relevant for criminal and administrative law. | **Incorporate into Legal Ai Ar** |
| `Sumario` (doctrinal headnotes) | High. Separating the headnote from the full ruling improves RAG. | **Incorporate into Legal Ai Ar** |
| `GraphCommunity` (communities) | High. Enables "case law line summaries". | **Incorporate into Legal Ai Ar** |
| `CrawlerConfig` (per-source config) | High for operations. Allows managing crawlers from the admin UI. | **Incorporate into Legal Ai Ar** |
| `EmbeddingConfig` (model config) | Medium. Allows changing model/dimensions without a deploy. | **Incorporate into Legal Ai Ar** |
| `FieldProvenance` (per-field provenance) | High. More granular traceability than planned. | **Adopt instead of DataProvenance** |
| `ChunkEntityMention` | High for RAG. Entities mentioned in each chunk. | **Incorporate into Legal Ai Ar** |

---

## 4. Ingestion Pipeline — Comparison

### 4.1 Stages

| # | Legal Ai Ar (plan) | MVP (implemented) | Delta |
|---|---|---|---|
| 1 | Scraper/Collector (Timer Trigger) | **Discoverer** — discovers docs in sources | Equivalent. MVP uses BackgroundService, Legal Ai Ar plans Azure Functions. |
| 2 | — | **Fetcher** — downloads PDFs/HTML | The MVP has an extra dedicated download step. Legal Ai Ar combines it with step 1. |
| 3 | Parser & Normalizer | **Parser** — extracts text and metadata | Equivalent. MVP uses PdfPig; Legal Ai Ar plans Azure Doc Intelligence. |
| 4 | — | **Enrichment** — LLM enrichment | Equivalent to Legal Ai Ar step 4 (chunking + enrichment). |
| 5 | SQL insert + Blob upload | **Persister** — persists entities | Equivalent. |
| 6 | Embedding Generator | Integrated into the **Indexer** (GenerateEmbeddingsStep) | Equivalent. The MVP already implements Contextual Retrieval. |
| 7 | AI Search Indexer | Integrated into the **Indexer** (IndexSearchStep) | Equivalent. |
| 8 | Graph Builder | Integrated into the **Indexer** (ResolveCitationsStep + ExtractChunkMentionsStep) | Equivalent. |

**Key finding:** The MVP has **more stages** than Legal Ai Ar (6 vs 7, but Discoverer + Fetcher are 2 steps that Legal Ai Ar combines into 1). In addition, the MVP implements a **strategy pattern per source** (`IDiscoverStrategy`, `IFetchStrategy`, `IParseStrategy`, `IEnrichStrategy`, `IIndexStrategy`), which is a superior design decision for supporting multiple sources with different logic.

### 4.2 Sources implemented in the MVP

| Source | Strategy | Status |
|---|---|---|
| CSJN — Sumarios | API (sjconsulta.csjn.gov.ar) | Implemented |
| CSJN — Acuerdos | API | Implemented |
| CSJN — Fallos Destacados | API | Implemented |
| SAIJ — Case law | HTML + PDF scraping | Implemented |
| SAIJ — Legislation | HTML scraping | Implemented |

This is a valuable asset: the crawlers are already running and producing real data.

### 4.3 What the MVP has and Legal Ai Ar did not plan

- **DLQ management** with an admin UI to view, retry, and discard failed documents
- **Infra recovery** orchestrator to handle incidents
- **External download cache** in Blob to avoid re-downloads
- **Document stage log** for granular per-document pipeline tracking

---

## 5. AI / RAG / Chat — Comparison

### 5.1 What the MVP already has

| Capability | MVP Status | Notes |
|---|---|---|
| Hybrid Search (BM25 + vector) | **Implemented** | 3 indexes: rulings, chunks, statutes |
| Contextual Retrieval | **Implemented** | `ChunkContextualizationPrompt` generates per-chunk context at ingestion |
| Tool Calling (function calling) | **Implemented** | 13 tools registered for the agent |
| SSE Streaming | **Implemented** | Typed events: text, tool_start, tool_end, validation, done |
| Input Guardrails | **Implemented** | 2 layers: rule-based + LLM classifier |
| Output Guardrails | **Implemented** | Citation validation against the DB post-answer |
| Query Preprocessing | **Implemented** | GPT-4o-mini expands queries, uses the SAIJ thesaurus |
| Community Detection | **Implemented** | Union-Find + clustering by law branch |
| Community Summarization | **Implemented** | LLM-generated summaries |

### 5.2 What is missing to reach Legal Ai Ar

| Legal Ai Ar Capability | MVP Status | Estimated effort |
|---|---|---|
| **3 specialized agents** (Regulatory, Case Law, Procedural) | 1 generic agent with 13 tools | High — requires routing, specialized system prompts, orchestrator |
| **Semantic Kernel** | OpenAI SDK direct | High — rewrite of AzureOpenAiAgentChatService and ChatQueryHandler |
| **Semantic + LLM router** | Does not exist | High — new layer |
| **LLM Re-ranking** (top-20 → top-5) | Does not exist | Medium |
| **Prompt Registry in DB** | Prompts hardcoded in C# | Medium — new table + UI |
| **Prompt A/B testing** | Does not exist | Medium |
| **Evaluation (golden set, LLM-as-judge)** | Does not exist | High — new infrastructure |
| **Feedback loop (thumbs up/down)** | Does not exist | Medium — UI + table + analysis |
| **Per-answer confidence score** | Does not exist (citations are validated but with no global score) | Medium |
| **Conversation persistence** | Stateless chat | Medium — new tables + logic |
| **Semantic caching** | Does not exist | Medium |
| **Circuit breaker (Polly)** | Does not exist | Low — Polly is already a dependency |
| **Memory (short-term + long-term)** | Does not exist | Medium |

---

## 6. Frontend — Comparison

### 6.1 Features implemented in the MVP

> The view names (`estadisticas`, `ordenamiento`, etc.) are the MVP's actual route identifiers and are kept as-is.

| Legal Ai Ar Feature | MVP View | Coverage |
|---|---|---|
| F01 - Auth | Login + guard + interceptor | 90% |
| F02 - Dashboard | `estadisticas` — KB stats | 70% (stats, not a personalized dashboard) |
| F03 - Legal norm search | `ordenamiento` — statutes list + detail | 80% |
| F04 - Case law search | `jurisprudencia` — search + results + detail | **95%** |
| F05 - Legal norm detail | `ordenamiento/:id` — statute detail | 80% |
| F08 - Agent chat | `asistente` — chat with SSE streaming | **90%** |
| F12 - Case file management | `procesos` — proceeding list + detail | 60% (no CRUD, read-only) |
| F19 - User admin | `admin/users` | 70% |
| F21 - Graph explorer | `explorador` — graph explorer | **90%** |
| FT02 - Global search | Command palette (Ctrl+K) | **80%** |
| Admin panel (ingestion) | `admin/*` — jobs, DLQ, reprocess | **95%** — very complete |

### 6.2 Features that do NOT exist in the MVP

| Legal Ai Ar Feature | Gap |
|---|---|
| F06 - Article detail (standalone) | Does not exist — articles are inside the norm |
| F07 - Regulatory updates (feed) | Does not exist |
| F09/F10/F11 - Specialized agents | Only 1 generic chat |
| F13 - Deadline management | Does not exist |
| F14 - Legal calendar | Does not exist |
| F15/F16 - Risk analysis | Does not exist |
| F17 - Report generation | Does not exist |
| F18 - Operational reports | Partial (statistics) |
| F20 - Advanced alerts | Does not exist |
| F22 - Agent feedback | Does not exist |
| F23 - PWA offline | Does not exist |
| FT01 - Real-time notifications | Does not exist |
| FT03 - Theme and accessibility | Partial (PwC AppKit) |

### 6.3 Reusable frontend assets

| Asset | Value |
|---|---|
| Chat component with SSE streaming | **Very high** — core functionality already implemented |
| Graph explorer with Cytoscape | **High** — functional graph visualization |
| Command palette (global search) | **High** — modern UX already implemented |
| Ingestion admin panel | **Very high** — DLQ, jobs, reprocess |
| Skeleton loaders, empty states, breadcrumbs | Medium — generic components |
| API services (rulings, courts, persons, statutes) | High — communication layer ready |
| PwC AppKit 4 guidelines + mockups | **Very high** — design already defined |

---

## 7. Strategic Recommendation

### 7.1 Evolve, do not rewrite

The MVP has ~16,000 functional code files. Rewriting from scratch would mean losing months of work on crawlers, pipeline, data model, and UI. We recommend an **incremental migration** approach:

### 7.2 4-phase migration plan

**Phase A — Foundation (Sprint 0-1):**
- Migrate the repo as the base of the new project
- Add Azure Key Vault, OpenTelemetry, App Insights
- Add IaC (Bicep) for the existing resources
- Add the missing entities (Clause, Movement, Deadline, Conversation, ChatMessage, PromptTemplate, ResponseFeedback)
- Refactor controllers → Minimal API (gradual)

**Phase B — Agent Architecture (Sprint 2-3):**
- Integrate Semantic Kernel, replacing the direct OpenAI SDK
- Create 3 specialized agents from the current generic agent (distribute the 13 tools)
- Implement the semantic + LLM router
- Implement the Prompt Registry and migrate hardcoded prompts
- Add conversation persistence

**Phase C — Quality & Eval (Sprint 4-5):**
- Implement the ResponseFeedback table + feedback UI
- Build the golden set with lawyers
- Implement LLM-as-judge + evaluation pipeline
- Add LLM re-ranking, confidence scores
- Implement semantic caching + circuit breaker (Polly)

**Phase D — New Features (Sprint 6+):**
- Deadline management + legal calendar
- Risk analysis
- Report generation
- Advanced alerts
- Azure Functions (migrate workers gradually)

### 7.3 What should NOT be touched (works well)

- Ingestion pipeline (Discoverer → Fetcher → Parser → Enrichment → Indexer) — only extend it
- CSJN and SAIJ crawlers — they work and produce data
- Core data model (Ruling, Statute, Person, Court, Citation, NormRelation, ThesaurusTerm)
- Strategy pattern per source
- DLQ management
- Hybrid search in Azure AI Search
- Community detection + summarization
- Frontend: chat, graph explorer, search, admin panel

---

## 8. Reuse Metrics

| Component | Estimated files | Reuse | Action |
|---|---|---|---|
| Core/Entities + Enums | ~80 files | **90%** | Migrate + add missing entities |
| Core/Interfaces | ~50 files | **80%** | Migrate + extend for Semantic Kernel |
| Infrastructure/Crawling | ~15 files | **95%** | Migrate as-is |
| Infrastructure/AI | ~8 files | **40%** | Partial rewrite for Semantic Kernel |
| Infrastructure/Graph | ~3 files | **80%** | Migrate + add SQL Graph edges |
| Infrastructure/Search | ~5 files | **90%** | Migrate + add re-ranking |
| Infrastructure/Persistence | ~40 files | **85%** | Migrate + add new configs |
| Workers (5 projects) | ~30 files | **70%** | Migrate, evaluate migration to Functions |
| Application (CQRS) | ~50 files | **75%** | Migrate + add new handlers |
| API Controllers | ~15 files | **60%** | Refactor to Minimal API |
| Frontend components | ~100+ files | **80%** | Migrate + add new views |
| Tests | ~40 files | **80%** | Migrate + expand coverage |

**Global estimate: ~78% of the MVP code is reusable directly or with minor adaptations.**

---

*Gap Analysis — MVP (legal-ai-ar) vs Legal Ai Ar — May 2026*
