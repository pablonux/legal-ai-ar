# Legal Ai Ar — Features Roadmap

> Legal Knowledge Base system with AI Agents for an Argentine law firm
>
> **Stack:** Angular 19 (SPA) + .NET 10 + Azure (SQL, AI Search, OpenAI, Functions, Storage)
>
> **Users:** Lawyers (full access) · Administrative staff (operational management)
>
> **Base:** Evolution of the `legal-ai-ar` MVP (~78% reusable code)
>
> **Document version:** 2.0 — May 2026

---

## Index

0. [Release 0.0 — Phase 0: MVP Migration and Environment](#0-release-00--phase-0-mvp-migration-and-environment)
1. [MVP Baseline — What already exists](#1-mvp-baseline--what-already-exists)
2. [Application Overview](#2-application-overview)
3. [Frontend Architecture](#3-frontend-architecture)
4. [Modules and Features by Release](#4-modules-and-features-by-release)
5. [Feature Details — Release 1.0](#5-feature-details--release-10)
6. [Feature Details — Release 2.0](#6-feature-details--release-20)
7. [Feature Details — Release 3.0](#7-feature-details--release-30)
8. [Feature Details — Release 4.0](#8-feature-details--release-40)
9. [Cross-Cutting Features](#9-cross-cutting-features)
10. [Detailed Tech Stack](#10-detailed-tech-stack)
11. [Permissions Matrix by Role](#11-permissions-matrix-by-role)
12. [API Endpoints by Module](#12-api-endpoints-by-module)
13. [KPIs and Success Metrics](#13-kpis-and-success-metrics)

---

## 0. Release 0.0 — Phase 0: MVP Migration and Environment

> **Sprint:** S00 (Pre-development) | **Team:** 1 Backend + 1 Frontend | **Blocking for the whole project**

### F00 — Development Environment and Structure

The `legal-ai-ar` MVP is a solid base with ~16,000 functional code files. Phase 0 evolves the existing monorepo, incorporates the project documentation, and prepares the data model for the new functionality. Infrastructure aspects (CI/CD, IaC, secrets, containers) are managed outside this roadmap.

The work items are the ticket files in `docs/roadmap/F00 - Development Environment and Structure/`
(the **source of truth**):

| ID | Work Item | Type | Assigned to | Estimate |
|----|-----------|------|-------------|----------|
| F00-W01 | Comprehensive Documentation | doc | Tech Lead | 5 SP |
| F00-W02 | Monorepo Setup and Backend Scaffolding (hoist `mvp/` → root; add `LegalAiAr.Agents` + `LegalAiAr.AgentEvals`) | backend | Backend Dev | 5 SP |
| F00-W03 | Angular 19 Frontend Scaffolding (hoist + align the existing SPA) | frontend | Frontend Dev | 5 SP |
| F00-W04 | GitHub Actions CI Configuration | devops | Backend Dev | 5 SP |
| F00-W05 | Azure Infrastructure with Bicep | devops | Backend Dev | 8 SP |
| F00-W06 | CD Deployment Pipelines Configuration | devops | Backend Dev | 5 SP |
| F00-W07 | Local Environment Setup and Onboarding Guide | doc | Anyone | 2 SP |
| F00-W08 | Code Quality Configuration (analyzers, formatting, pre-commit) | devops | Both | 3 SP |

**Total:** 38 story points (≈28 remaining — W01 authored, W07/W08 largely done) | **Estimated duration:** ~1–2 sprints (S00, 2 weeks)

> **Already partly covered & "adapt vs create".** Given the MVP and the foundation work already done,
> several F00 tickets reduce to *hoist/adapt/verify* rather than *create from scratch*: W01 is authored;
> W07 onboarding lives in `docs/onboarding/`; W08 is partly done (root `.editorconfig` + `.vscode/`);
> W03 frontend already exists (hoist + align to Angular 19). The **comprehensive delivery & hosting**
> model (GitHub → Azure staging + GCaaS) is detailed in feature **FT05** and `docs/deployment/`; treat
> F00-W04–W06 as the initial bootstrap and consolidate them with FT05 when implemented.

#### Data model evolution (handled within feature work — not a single F00 ticket)

The data model evolves **inside the feature that needs each entity**, not as a standalone F00 work
item. Two groups:

**(a) New entities still to add** — created in their target release/feature:

| Entity | Purpose | Required for |
|--------|---------|--------------|
| `Clause` | Sub-article granularity for RAG | R1.0 — Article Detail |
| `ArticleVersion` | Temporal versioning of articles (SQL Temporal Tables) | R1.0 — Amendment history |
| `Conversation` + `ChatMessage` | Chat conversation persistence | R1.0 — Basic chat |
| `PromptTemplate` | Prompt Registry in SQL for dynamic templates | R2.0 — Specialized agents |
| `ResponseFeedback` | Thumbs up/down feedback per agent answer | R2.0 — Feedback |
| `Movement` | Procedural event timeline in case files | R2.0 — Case file management |
| `Deadline` | Procedural deadlines with business-day calculation | R2.0 — Deadline management |
| `UserPreferences` | User preferences (branch, alerts, theme) | R2.0 — Personalization |
| `LegalTaxonomy` | Controlled taxonomy (extends ThesaurusTerm) | R1.0 — Classification |
| `RiskAnalysis` | Persistence of generated risk analyses | R3.0 — Risk analysis |

**(b) MVP entities — already present in the model** (no migration needed; documented in
[`docs/technical/17-kb-data-model.md`](../technical/17-kb-data-model.md)): `Vote`, `ProsecutorOpinion`,
`Sumario`, `GraphCommunity` + `CommunityMembership`, `CrawlerConfig`, `EmbeddingConfig`,
`FieldProvenance` (per-field provenance), `ChunkEntityMention`. These were already implemented in the
MVP and are part of the KB data model.

### Phase 0 Technical Decisions

| Decision | Choice |
|---|---|
| Strategy | Evolve the MVP in-place, do not rewrite (~78% reusable) |
| Repo structure | Existing `legal-ai-ar` monorepo (`/backend` + `/frontend` + `/docs`) |
| Backend | .NET 10 LTS, 4-layer Clean Architecture, Minimal API (gradual refactor from Controllers) |
| Frontend | Angular 19 standalone, PwC AppKit 4 (already configured in the MVP) |
| DB Strategy | Code-First EF Core (migrate MVP schema + new entities) |
| Backend Testing | xUnit + NSubstitute (already in the MVP) + FluentAssertions |
| Frontend Testing | Jest + Angular Testing Library + Playwright (E2E) |
| Mediator | The MVP's custom IMediator (evaluate migration to MediatR in R2.0) |

---

## 1. MVP Baseline — What already exists

> The `legal-ai-ar` MVP is a surprisingly mature project with a 6-stage ingestion pipeline, agentic chat with tool calling and 13 tools, hybrid search, graph community detection, an integrated SAIJ thesaurus, and a complete Angular frontend with ~15 functional views.

### 1.1 Ingestion Pipeline (functional — reuse as-is)

| Stage | Component | Description |
|-------|-----------|-------------|
| 1 | **Discoverer** | Discovers documents in sources with a strategy pattern (`IDiscoverStrategy`) |
| 2 | **Fetcher** | Downloads PDFs/HTML with a Blob Storage cache |
| 3 | **Parser** | Extracts text + metadata (PdfPig for PDFs, regex for HTML) |
| 4 | **Enrichment** | LLM enrichment with GPT-4o-mini (metadata, NER, classification) |
| 5 | **Persister** | Persists entities to Azure SQL via EF Core |
| 6 | **Indexer** | Generates embeddings, indexes in AI Search, resolves citations, extracts mentions |

Implemented sources: CSJN (Sumarios, Acuerdos, Fallos Destacados), SAIJ (Case law, Legislation).

Support components: 5 Azure Storage Queues between stages, DLQ with an admin UI, external download cache, DocumentStageLog for tracking, Contextual Retrieval at ingestion, Community Detection (Union-Find) + Summarization (LLM).

### 1.2 Data Model (44 entities + 30 enums)

Reusable core entities: `Statute`, `Ruling`, `Person`, `Court`, `JudicialOffice`, `Citation` (6 types), `NormRelation` (4 types), `ThesaurusTerm` + `ThesaurusRelation`, `LegalDoctrine`, `JudicialProceeding`, `Vote`, `ProsecutorOpinion`, `Sumario`, `Keyword`, `LegalRepresentation`, `GraphCommunity`, `CommunityMembership`, `FieldProvenance`, `ChunkEntityMention`, `CrawlerConfig`, `EmbeddingConfig`, `DocumentStageLog`.

### 1.3 AI / RAG / Chat (functional — evolve)

| Capability | Status | Notes |
|------------|--------|-------|
| Hybrid Search (BM25 + vector) | ✅ Implemented | 3 indexes: rulings, chunks, statutes |
| Contextual Retrieval | ✅ Implemented | Per-chunk context generated at ingestion |
| Tool Calling (13 tools) | ✅ Implemented | 1 generic agent with function calling |
| SSE Streaming | ✅ Implemented | Events: text, tool_start, tool_end, validation, done |
| Input Guardrails | ✅ Implemented | 2 layers: rule-based + LLM classifier |
| Output Guardrails | ✅ Implemented | Citation validation against the DB |
| Query Preprocessing | ✅ Implemented | GPT-4o-mini + expansion with the SAIJ thesaurus |
| Community Detection | ✅ Implemented | Union-Find + clustering by law branch |
| Community Summarization | ✅ Implemented | LLM-generated summaries |

### 1.4 Frontend (Angular — ~15 functional views)

> The view names (`estadisticas`, `ordenamiento`, etc.) are the MVP's actual route identifiers and are kept as-is.

| MVP View | Legal Ai Ar Coverage | Feature |
|----------|----------------------|---------|
| Login + guard + interceptor | 90% | F01 Auth |
| `estadisticas` — KB stats | 70% | F02 Dashboard |
| `ordenamiento` — statutes list + detail | 80% | F03/F05 Legal Norm Search + Detail |
| `jurisprudencia` — search + results + detail | **95%** | F04 Case Law Search |
| `asistente` — chat with SSE streaming | **90%** | F08 Chat |
| `procesos` — proceeding list + detail | 60% | F12 Case Files (read-only) |
| `explorador` — graph explorer (Cytoscape) | **90%** | F21 Legal Graph |
| Command palette (Ctrl+K) | **80%** | FT02 Omnisearch |
| `admin/*` — jobs, DLQ, reprocess, workers | **95%** | F19 Ingestion Admin |
| `organismos`, `sujetos`, `vocabulario` | 70% | Auxiliary views |
| `ontologia` | 60% | Ontology view |

### 1.5 What does NOT exist in the MVP (main gaps)

| Gap | Impact | Release |
|-----|--------|---------|
| Semantic Kernel (uses the OpenAI SDK directly) | High — rewrite of the agent layer | R2.0 |
| 3 specialized agents (1 generic currently) | High — routing, dedicated prompts | R2.0 |
| Prompt Registry (prompts hardcoded in C#) | High — dynamic prompt management | R2.0 |
| Evaluation (golden set, LLM-as-judge) | High — no quality framework | R2.0 |
| Feedback loop (thumbs up/down) | Medium — no continuous improvement | R2.0 |
| Conversation persistence | High — stateless chat | R1.0 |
| LLM Re-ranking | Medium — ranking by score only | R2.0 |
| Per-answer confidence score | Medium — no confidence indicator | R2.0 |
| Semantic caching | Medium — no semantic cache | R2.0 |
| Deadline / calendar management | High — new core feature | R2.0 |
| Risk analysis | High — R3.0 feature | R3.0 |
| Observability (OpenTelemetry) | High — no tracing or metrics | R4.0 |

---

## 2. Application Overview

Legal Ai Ar is a SPA (Single Page Application) that combines a knowledge base of the Argentine legal system with a system of specialized AI agents. It allows lawyers and administrative staff to search legislation and case law, manage case files and deadlines, consult AI agents, and generate legal risk analyses.

### 2.1 Product Objectives

- Centralize access to norms, case law, and doctrine of the Argentine legal order.
- Reduce legal research time from hours to minutes through semantic search and AI.
- Eliminate missed procedural deadlines with automated alerts and tracking.
- Provide data-driven legal risk analysis to improve decision-making.
- Generate legal documents and reports automatically.

### 2.2 User Roles

| Role | Description | Access |
|------|-------------|--------|
| **Lawyer** | A firm professional with full access to all functionality | Search, AI agents, case files, risk analysis, reports, alert configuration |
| **Administrative** | Support staff with access to operational management | Case files, deadlines, notifications, calendar, operational report generation |

### 2.3 Deployment and Hosting Model

The project uses **two complementary delivery paths** that may share the same Azure data services (SQL, Blob, AI Search, OpenAI) but differ in compute and identity. Full references: [`github-delivery.md`](../deployment/github-delivery.md) and [`gcaas-hosting.md`](../deployment/gcaas-hosting.md).

| Aspect | GitHub → Azure staging | GCaaS (corporate production) |
|--------|------------------------|------------------------------|
| Trigger | Merge to `main` → GitHub Actions (`ci.yml`, `cd.yml`) | PwC GCaaS Helm / platform pipeline (chart in `mvp/deployment/`) |
| Compute | Azure App Service (API, staging slot) + Azure Static Web Apps (SPA) | Knative services (API + SPA containers) behind Istio |
| Identity | Same API with `usePlatformCredentials: false` (no platform cookies) | Entra SSO via Envoy; `id_token` HTTP-only cookie |
| Secrets | GitHub Actions secrets (`AZURE_CREDENTIALS`, SWA token) | HashiCorp Vault keys mapped into the release |
| SPA build config | `staging` | `development` / `production` (Angular) |
| Observability | Application Insights | Optional Datadog via platform labels |

GitHub Actions **does not deploy to GCaaS**; GCaaS releases use their own Helm pipeline. This delivery/hosting work is tracked in feature **FT05 — Delivery and Hosting**.

---

## 3. Frontend Architecture

### 3.1 Angular 19 SPA

| Aspect | Decision | MVP |
|--------|----------|-----|
| **Framework** | Angular 19 with standalone components (no NgModules) | ✅ Already in MVP |
| **State Management** | Angular Signals + NgRx Signal Store for global state | Partial — add Signal Store |
| **Routing** | Lazy loading per feature module with `loadComponent()` | ✅ Already in MVP |
| **UI Library** | PwC AppKit 4 + Tailwind CSS 4 for layout | ✅ Already in MVP (partial) |
| **Forms** | Reactive Forms with strict typing (Typed Forms) | ✅ Already in MVP |
| **HTTP** | `HttpClient` with functional interceptors for auth and error handling | ✅ Already in MVP |
| **Real-time** | SignalR client for push notifications and agent responses | Add |
| **Auth** | Platform Entra SSO via `id_token` cookie (`withCredentials`); no MSAL, no `/login` | ✅ Already in MVP (`auth.service`, interceptors) |
| **i18n** | Spanish (AR) as the single language, with support prepared for expansion | ✅ |
| **Testing** | Jest (unit) + Playwright (e2e) | Partial — add Playwright |
| **Build** | esbuild (default in Angular 19), SSG for the landing page | ✅ |

### 3.2 Angular Project Structure

```
src/
├── app/
│   ├── core/                      # Singleton services, guards, interceptors
│   │   ├── auth/                  # AuthService, AuthGuard, platform session config
│   │   ├── interceptors/          # AuthInterceptor, ErrorInterceptor, LoadingInterceptor
│   │   ├── services/              # ApiService, SignalRService, NotificationService
│   │   └── models/                # Shared interfaces and types
│   ├── shared/                    # Reusable components, pipes, directives
│   │   ├── components/            # SearchBar, DataTable, AlertBadge, ConfirmDialog
│   │   ├── pipes/                 # LegalDatePipe, TruncatePipe, HighlightPipe
│   │   └── directives/            # RoleDirective, TooltipDirective
│   ├── features/                  # Feature modules (lazy loaded)
│   │   ├── dashboard/             # Main dashboard
│   │   ├── search/                # Legal norm and case law search
│   │   ├── case-files/            # Case file and proceeding management
│   │   ├── agents/                # AI agent chat
│   │   ├── risk/                  # Legal risk analysis
│   │   ├── calendar/              # Deadline and due-date calendar
│   │   ├── reports/               # Report and document generation
│   │   ├── legal-norms/           # Legal norm explorer (detail, graph)
│   │   ├── graph/                 # Legal graph explorer
│   │   ├── admin/                 # Administration (users, config, ingestion)
│   │   └── alerts/                # Notification and alert center
│   ├── layout/                    # Shell, sidebar, navbar, footer
│   └── app.config.ts              # Standalone configuration
├── assets/
├── environments/
└── styles/                        # Tailwind config, themes, variables
```

---

## 4. Modules and Features by Release

### Release Map

| Release | Name | Weeks | Focus | Strategy |
|---------|------|-------|-------|----------|
| **0.0** | Preparation | S00 (2 wks) | Restructure repo + data model | Docs + new entities + code quality |
| **1.0** | Foundation | S01-S06 (6 wks) | Search + Basic chat + Graph | Evolve the existing MVP |
| **2.0** | Agents | S07-S12 (6 wks) | AI agents + Case files + Deadlines | SK + specialized agents + case mgmt |
| **3.0** | Risk | S13-S16 (4 wks) | Risk analysis + Reports | New features on top of the agent base |
| **4.0** | Operations | S17-S20 (4 wks) | Observability + Alerts + PWA | Ops, monitoring, hardening |

> **Note:** The existence of the MVP reduces R1.0 from 8 to 6 weeks. Many R1.0 features have 70-95% MVP coverage and only require evolution, not development from scratch.

### Feature by Release (with MVP coverage)

| ID | Feature | Release | MVP | Action |
|----|---------|---------|-----|--------|
| F01 | Authentication and Authorization | 1.0 | 90% | Reconcile: platform Entra `id_token` cookie (no MSAL) |
| F02 | Main Dashboard | 1.0 | 70% | Evolve: stats → personalized widgets |
| F03 | Legal Norm Search | 1.0 | 80% | Evolve: add scoring profile, facets |
| F04 | Case Law Search | 1.0 | **95%** | Polish: almost complete |
| F05 | Legal Norm Detail | 1.0 | 80% | Evolve: add amendment timeline |
| F06 | Article Detail | 1.0 | 0% | **New**: standalone view with clauses |
| F07 | Regulatory Updates | 1.0 | 0% | **New**: feed + subscription by branch |
| F08 | AI Agent Chat (basic) | **1.0** | **90%** | Evolve: add persistence + improved citation |
| F21 | Legal Graph Explorer | **1.0** | **90%** | Polish: already functional with Cytoscape |
| F09 | Regulatory Agent | 2.0 | 0% | **New**: specialized SK plugin |
| F10 | Case Law Agent | 2.0 | 0% | **New**: specialized SK plugin |
| F11 | Procedural Agent | 2.0 | 0% | **New**: specialized SK plugin |
| F12 | Case File Management | 2.0 | 60% | Evolve: add CRUD, movements, docs |
| F13 | Deadline Management | 2.0 | 0% | **New**: business-day calculation + alerts |
| F14 | Legal Calendar | 2.0 | 0% | **New**: FullCalendar |
| F19 | User Administration | **2.0** | 70% | Evolve: add roles, audit |
| F22 | Agent Feedback and Improvement | **2.0** | 0% | **New**: thumbs + corrections |
| F15 | Legal Risk Analysis | 3.0 | 0% | **New**: risk agent |
| F16 | Risk Analysis History | 3.0 | 0% | **New**: re-analysis |
| F17 | Legal Report Generation | 3.0 | 0% | **New**: .docx from templates |
| F18 | Operational Reports | 3.0 | 30% | Evolve: stats → charts + export |
| F20 | Advanced Alert Configuration | 4.0 | 0% | **New**: wizard + email |
| F23 | PWA Offline Mode | 4.0 | 0% | **New**: service worker |
| FT05 | Delivery and Hosting (GitHub + GCaaS) | Cross-cutting | 70% | Document + reconcile: GitHub CI/CD to Azure staging, GCaaS Helm/Knative, platform auth, Vault |

---

## 5. Feature Details — Release 1.0

> **Foundation** — MVP evolution: smart search + basic chat + graph
>
> **Strategy:** Most features in this release already exist in the MVP with 70-95% coverage. The work is evolution, polish, and adding missing functionality — not development from scratch.

### F1.1 — Authentication and Authorization

> **Auth model (corrected):** Production runs on **GCaaS** with platform-managed Microsoft Entra SSO. The platform (Envoy) issues an **`id_token` HTTP-only cookie**; the API validates that cookie's JWT (`Auth:Platform`). There is **no MSAL in the SPA, no `/login` route, and no Bearer tokens** — the SPA sends `withCredentials: true` and refreshes the platform session periodically. The MVP already implements this (`PlatformAuthenticationHandler`, `Auth:Platform`). Azure staging (GitHub CD) runs the same API with `usePlatformCredentials: false`. See [`gcaas-hosting.md`](../deployment/gcaas-hosting.md).

| Field | Detail |
|-------|--------|
| **MVP** | 🟢 90% — Platform auth implemented: `id_token` cookie validation, `Auth:Platform` (TenantId/ValidAudience), dev identity injection, route guards, error interceptor (401 → session gate) |
| **Delta** | Finalize role mapping (Entra/platform roles → app roles Lawyer/Administrative), `sesion-requerida` gate polish, and session-refresh hardening. **Do not** add MSAL or app-owned login — the platform owns the SSO flow. |
| **Description** | Authentication via GCaaS platform Entra SSO. The API authorizes only requests bearing a valid `id_token` cookie. Lawyer and Administrative roles from JWT claims. Route guards by role. |
| **Backend** | .NET 10. `PlatformAuthenticationHandler` validates the `id_token` JWT against Entra OIDC metadata (issuer/audience from Vault `TenantId` + `ValidAudience`). `PlatformRoleResolver` maps claims → app role. No custom login endpoints. |
| **Frontend** | Reuse the MVP's `auth.service.ts` (`bootstrapSession()` → `GET /api/auth/me`), `platformCredentialsInterceptor` (`withCredentials`), `startGcaasSessionRefresh()`, `AuthGuard`, and the `sesion-requerida` SSO gate. No MSAL. |
| **Acceptance** | `GET /api/auth/me` returns 200 with a valid session cookie and 401 without. Routes protected by role. Session refresh runs (~45 min). Logout redirects to the platform logout URL. |

### F1.2 — Main Dashboard

| Field | Detail |
|-------|--------|
| **MVP** | 🟡 70% — `estadisticas` view with KB stats (counts, basic charts) |
| **Delta** | Transform stats into a per-role personalized dashboard with widgets: deadlines, recent searches, alerts, updates. |
| **Description** | Main post-login view with an activity summary: upcoming deadlines, recent searches, active case files, pending alerts, regulatory updates. |
| **Components** | `DashboardComponent` with widgets: `DeadlinesWidgetComponent`, `RecentSearchesComponent`, `AlertsWidgetComponent`, `RegulatoryUpdatesComponent`. |
| **Backend** | Aggregator endpoint: `GET /api/dashboard` that consolidates data from multiple services. |
| **Role differentiation** | Lawyer: all widgets + access to AI agents. Administrative: deadlines, case files, calendar. |

### F1.3 — Legal Norm Search

| Field | Detail |
|-------|--------|
| **MVP** | 🟡 80% — `ordenamiento` view with statutes list and detail, functional hybrid search |
| **Delta** | Add a scoring profile with boost by validity/hierarchy. Add dynamic facets. Improve highlighting. |
| **Description** | Semantic search of Argentine legislation. Free-text search (natural language), filters by law branch, jurisdiction, validity, norm type, date range. Results with highlighting and relevant snippets. |
| **Backend** | `POST /api/search/legal-norms` → Azure AI Search (hybrid: BM25 + vectors). Scoring profile with boost by validity and normative hierarchy. Facets for dynamic filters. |
| **Frontend** | Evolve the MVP's `SearchBarComponent`: add autocomplete with a 300ms debounce. Add `SidebarFiltersComponent` with facet counts. Improve result cards with highlight. |
| **Acceptance** | Search in < 2 seconds. Relevant results in the top 5. Functional filters with counts. Pagination with 20 results/page. |

### F1.4 — Case Law Search

| Field | Detail |
|-------|--------|
| **MVP** | 🟢 **95%** — `jurisprudencia` view with search, results, detail almost complete |
| **Delta** | Minimal: add a filter by keywords (topic descriptors) and clickable cited-article chips. |
| **Description** | Semantic search of court rulings. Free-text search, court, venue, date, keywords (topic descriptors). Result view with a ruling excerpt and interpreted articles. |
| **Backend** | Reuse the existing endpoints. Add a relationship with articles via SQL Graph (Edge `interpretsArticle`). |
| **Frontend** | Reuse `CaseLawSearchComponent`. Add filters: keywords (ThesaurusTerm), instance. Result card: add clickable cited-article chips. |
| **Acceptance** | Search < 2 sec. Functional links to interpreted articles. Filter by court with autocomplete. |

### F1.5 — Legal Norm Detail

| Field | Detail |
|-------|--------|
| **MVP** | 🟡 80% — `ordenamiento/:id` view with statute detail, basic articles |
| **Delta** | Add tabs (Info, Articles, History, Graph). Add an amendment timeline. Improve the relationship graph. |
| **Description** | Complete view of a legal norm: metadata, navigable articles, amendment history, related norms (visual graph). |
| **Backend** | Reuse `GET /api/legal-norms/{id}`. Add `GET /api/legal-norms/{id}/graph` → SQL Graph MATCH query. `GET /api/legal-norms/{id}/articles` with pagination. `GET /api/legal-norms/{id}/history`. |
| **Frontend** | Evolve the detail into `LegalNormDetailComponent` with tabs: General Info, Articles (virtual scroll), Amendment History (timeline), Relationship Graph (reuse the MVP explorer's Cytoscape). |
| **Acceptance** | Articles render correctly. The graph shows relationships up to 2 levels. Functional links between norms. |

### F1.6 — Article Detail

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — Articles only exist as a string inside norms, not as a standalone view |
| **Delta** | Completely new feature. Requires the `Clause` entity (F00-W03). |
| **Description** | View of a specific article with: normative text, clauses, case law that interprets it, the article's amendment history. |
| **Backend** | `GET /api/articles/{id}` → SQL. `GET /api/articles/{id}/case-law` → SQL Graph MATCH (CaseLaw→interpretsArticle→Article). |
| **Frontend** | `ArticleDetailComponent` with sections: text in force, clauses (expandable), a side panel with rulings that interpret it (sorted by relevance and date). |
| **Acceptance** | Readable normative text. List of related case law with functional links. |

### F1.7 — Regulatory Updates

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — No updates feed exists |
| **Delta** | Completely new feature. Can reuse the MVP's ingestion pipeline to detect new norms. |
| **Description** | Chronological feed of new norms and amendments detected by the ingestion pipeline. Filterable by law branch. Subscription to topic alerts. |
| **Backend** | An Azure Functions Timer Trigger scrapes the Official Gazette every 6h. `GET /api/updates?branch=criminal&from=2026-03-01`. SignalR push for real-time updates. |
| **Frontend** | `RegulatoryUpdatesComponent` with a timeline feed. Each item shows: type (new/amendment/repeal), affected norm, summary, date. "Subscribe" button by branch. |
| **Acceptance** | The day's updates visible in < 6 hours from publication in the Official Gazette. Functional push notifications. |

### F1.8 — Basic Chat with AI Agent

| Field | Detail |
|-------|--------|
| **MVP** | 🟢 **90%** — `asistente` view with functional chat: SSE streaming, tool calling (13 tools), input guardrails, output validation |
| **Delta** | Add conversation persistence (Conversation + ChatMessage). Improve inline citation with direct links. Add conversation history. Keep the current generic agent (specialization comes in R2.0). |
| **Description** | Chat interface to interact with the generic AI agent. Answers with streaming, source citation, and validation. Persistent conversation history. |
| **Backend** | Reuse the MVP's `AzureOpenAiAgentChatService` and `ChatQueryHandler`. Add the `Conversation` + `ChatMessage` tables. Keep the existing SSE streaming. Improve the citation format in the answer. |
| **Frontend** | Reuse the MVP's `AsistenteComponent`. Add: a side panel for conversation history, a cited-sources panel with clickable links, a per-message "Copy" button. |
| **UX** | Response streaming (already functional). Sources as chips at the end of each answer. |
| **Acceptance** | The answer starts rendering in < 3 sec. Sources always present and clickable. Persistent history across sessions. |
| **Role** | Lawyers only |

### F1.9 — Legal Graph Explorer

| Field | Detail |
|-------|--------|
| **MVP** | 🟢 **90%** — `explorador` view with functional Cytoscape.js, SQL Graph queries, side panel |
| **Delta** | Add filters by relationship type. Improve the node detail panel. Add configurable depth. Connect to the norm/ruling detail. |
| **Description** | Interactive visualization of the legal relationship graph. Navigate relationships between norms, articles, case law, bodies. Zoom, pan, filters by relationship type, on-click node expansion. |
| **Backend** | Reuse the MVP's `SqlGraphService`. Add `GET /api/graph/explore?nodeId=LEY-26994&depth=2&relations=amends,repeals`. |
| **Frontend** | Reuse the MVP's `ExploradorComponent` with Cytoscape. Add: filters by relationship type (checkboxes), depth slider, links from a node to the norm/ruling detail. |
| **Acceptance** | Renders graphs of up to 200 nodes smoothly. Functional zoom/pan. Clicking a node shows detail. Filters by relationship type. |
| **Role** | Lawyers only |

---

## 6. Feature Details — Release 2.0

> **Agents** — Migration to Semantic Kernel, specialized agents, case file and deadline management
>
> **Strategy:** This is the release with the most new work. The MVP's generic agent is replaced by 3 specialized agents orchestrated with Semantic Kernel, and case management functionality (case files, deadlines, calendar) is added.

### F2.1 — Migration to Semantic Kernel + Specialized Agents

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — The MVP uses the Azure OpenAI SDK directly with `AzureOpenAiAgentChatService` |
| **Delta** | Complete rewrite of the agent layer: replace the OpenAI SDK with Semantic Kernel, create 3 specialized plugins, implement a hybrid router, migrate the 13 existing tools to [KernelFunction]. |
| **Description** | Migrate the agent orchestration from the direct OpenAI SDK to Semantic Kernel. Implement a hybrid router (semantic + LLM) to route queries to the appropriate agent. Implement the ReAct pattern for multi-step reasoning. |
| **Backend** | Semantic Kernel with typed plugins in C#. A 2-layer router: embedding similarity (fast path, confidence > 0.85) + LLM router (fallback). Orchestrator pattern for multi-agent queries. |
| **New components** | `SemanticKernelOrchestrator`, `HybridRouter`, `AgentMemoryService` (short-term SQL + working SK + long-term preferences). |
| **Acceptance** | The router correctly classifies > 90% of queries. Agents use ReAct for complex queries. Latency equal to or lower than the MVP. |

### F2.2 — Regulatory Agent (integrated into chat)

| Field | Detail |
|-------|--------|
| **MVP** | Partial — the `SearchStatutes`, `GetStatuteDetail`, `CheckNormRelations` tools exist but in the generic agent |
| **Delta** | Create a `RegulatoryAgentPlugin` with a specialized system prompt. Migrate relevant MVP tools. Add `CheckValidity()`, `RepealChain()`. |
| **Description** | Answers queries about current legislation. Examples: "What is the statute of limitations for a labor claim?", "Which CCCN articles regulate leases?", "Did Ley 27.742 amend the dismissal regime?". |
| **Backend** | Semantic Kernel plugin `RegulatoryAgentPlugin` with functions: `SearchLegalNorm()`, `CheckValidity()`, `GetArticle()`, `TraceAmendments()`, `RepealChain()`. Uses AI Search + SQL Graph. |
| **Acceptance** | Answers cite specific articles. Correctly detects whether a norm was repealed or amended. |

### F2.3 — Case Law Agent (integrated into chat)

| Field | Detail |
|-------|--------|
| **MVP** | Partial — the `SearchRulings`, `GetRulingDetail`, `SearchChunks`, `GetCommunityInfo` tools exist in the generic agent |
| **Delta** | Create a `CaseLawAgentPlugin`. Migrate tools. Add `AnalyzeDoctrine()`, `CaseLawTrend()`. |
| **Description** | Searches and analyzes court rulings. Examples: "What does the CSJN say about discriminatory dismissal?", "Recent precedents on medical liability in CABA". |
| **Backend** | `CaseLawAgentPlugin` with: `SearchRuling()`, `AnalyzeDoctrine()`, `IdentifyPrecedents()`, `CaseLawTrend()`. RAG over AI Search. Graph traversal for the ruling→article relationship. |
| **Acceptance** | Cites rulings with caption, court, and date. Distinguishes ratio decidendi from obiter dictum. Identifies the trend (favorable/unfavorable). |

### F2.4 — Procedural Agent (integrated into chat)

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — There are no procedural tools in the generic agent |
| **Delta** | Completely new feature. Requires the `Movement` and `Deadline` entities (F00-W03). |
| **Description** | Queries about case files and deadlines. Examples: "How long until the answer deadline expires in case file 12345?", "List the cases with due dates this week". |
| **Backend** | `ProceduralAgentPlugin` with: `GetCaseFile()`, `CalculateDeadline()`, `AlertDueDate()`, `ListActiveCases()`. Queries Azure SQL. A business-day calculation engine. National and court holidays calendar. |
| **Acceptance** | Correct business-day calculation (excludes weekends and national holidays). Functional alerts. |
| **Role** | Lawyers and Administrative |

### F2.5 — Prompt Registry and Prompt Management

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — Prompts hardcoded in C# classes (`ChatSystemPrompt`, `ChunkContextualizationPrompt`, etc.) |
| **Delta** | Completely new feature. Requires the `PromptTemplate` entity (F00-W03). |
| **Description** | Hybrid prompt management system: YAML files for base system prompts (versioned in Git) + a SQL `PromptTemplate` table for dynamic templates with A/B testing. |
| **Backend** | `PromptTemplate` in Azure SQL with: name, version, content, variables, target model, active/inactive, performance metrics. API: `GET/PUT /api/admin/prompts`. |
| **Frontend** | Admin prompts view: list, editor with preview, A/B toggle, per-version performance metrics. |
| **Acceptance** | Prompts editable without a deploy. Functional A/B testing. Rollback to a previous version in < 1 minute. |

### F2.6 — AI Evaluation and Quality

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — No evaluation framework |
| **Delta** | Completely new feature. |
| **Description** | A quality evaluation pipeline for agent answers: a golden set of 200 queries, LLM-as-judge for automatic scoring, regression testing in CI, drift monitoring. |
| **Backend** | Golden set in JSON (200 queries with expected answers, distribution by branch). LLM-as-judge (GPT-4o evaluates answers against criteria). CI pipeline: `dotnet test` runs the eval on each PR. Metrics: Recall@10, MRR, faithfulness, citation accuracy. |
| **Acceptance** | Complete golden set (200 queries). Calibrated LLM-as-judge (Cohen's Kappa ≥ 0.75 vs human). Regression test in CI passes in < 5 minutes. |

### F2.7 — Case File Management

| Field | Detail |
|-------|--------|
| **MVP** | 🟡 60% — `procesos` view with JudicialProceeding list and detail (read-only) |
| **Delta** | Add full CRUD, movements (timeline), attached documents (Blob), linking with deadlines. Requires the `Movement` entity (F00-W03). |
| **Description** | CRUD of the firm's judicial and administrative case files. Each case file has: number, caption, venue, court, status, parties, responsible lawyer, attached documents, movement history. |
| **Backend** | CRUD: `GET/POST/PUT/DELETE /api/case-files`. Search with filters: `GET /api/case-files?courtVenue=labor&status=in_progress&lawyer=jperez`. Subresources: `/api/case-files/{id}/movements`, `/api/case-files/{id}/documents`. |
| **Frontend** | Evolve the MVP's `procesos` view into `CaseFileListComponent` with a DataTable (sort, filter, pagination). Add `CaseFileDetailComponent` with tabs: Information, Movements (timeline), Documents (upload/download), Deadlines, Notes. `CaseFileFormComponent` for create/edit. |
| **Acceptance** | Full CRUD. Search by any field. Document upload to Blob Storage. Chronological movement timeline. |
| **Role** | Lawyers (full CRUD) · Administrative (read + add movements) |

### F2.8 — Deadline Management

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — No deadline management exists |
| **Delta** | Completely new feature. Requires the `Deadline` entity (F00-W03). |
| **Description** | Registration and tracking of procedural deadlines linked to case files. Automatic business-day calculation. Configurable alerts (X days before the due date). Statuses: pending, due soon, overdue, fulfilled. |
| **Backend** | CRUD `/api/deadlines`. Business-day calculation with a national and court holidays calendar (Azure SQL). A daily Azure Functions Timer Trigger evaluates deadlines and generates alerts via Storage Queue → SignalR push. |
| **Frontend** | `DeadlineListComponent` with filters by status and urgency. Color badges: green (>5 days), yellow (2-5 days), red (<2 days), gray (fulfilled). `DeadlineFormComponent` with date pickers and automatic business-day calculation. |
| **Acceptance** | Correct business-day calculation. Push alerts 72h, 48h, and 24h before the due date. Court holidays accounted for. |
| **Role** | Lawyers and Administrative |

### F2.9 — Legal Calendar

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — No calendar view exists |
| **Delta** | Completely new feature. |
| **Description** | Calendar view (month/week/day) with all the firm's deadlines, hearings, and due dates. Filters by lawyer, venue, case file. |
| **Backend** | `GET /api/calendar?from=2026-04-01&to=2026-04-30&lawyer=all`. Aggregates deadlines, hearings, and case file dates. |
| **Frontend** | `CalendarComponent` with FullCalendar (Angular wrapper). Color-coded events by type. Clicking an event opens the deadline/case file detail. Drag & drop to reschedule hearings. |
| **Acceptance** | Correct event visualization. Functional filters. Smooth month/week/day navigation. |
| **Role** | Lawyers and Administrative |

### F2.10 — User Administration

| Field | Detail |
|-------|--------|
| **MVP** | 🟡 70% — `admin/users` view with a list, ingestion admin panel (DLQ, jobs, workers) at 95% |
| **Delta** | Add role and per-module permission management. Add access auditing. Keep the MVP's ingestion admin panel. |
| **Description** | System user management: role assignment, per-module permissions, access auditing. Ingestion admin panel (inherited from the MVP). |
| **Backend** | Entra ID for identities. Azure SQL for custom permissions. `GET/POST/PUT /api/admin/users`. Audit log in Azure SQL. Reuse the MVP's ingestion admin endpoints. |
| **Frontend** | Evolve the MVP's `AdminUsuariosComponent`. Add: a role and permission editing form, an audit log with filters. Keep the ingestion admin panel (DLQ, jobs, reprocess). |
| **Role** | Lawyers with admin permission only |

### F2.11 — Agent Feedback and Improvement

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — No feedback system |
| **Delta** | Completely new feature. Requires the `ResponseFeedback` entity (F00-W03). |
| **Description** | A feedback system to rate agent answers: thumbs up/down, answer correction, source-usefulness rating. The data feeds continuous improvement of prompts and scoring. |
| **Backend** | `POST /api/feedback` with: conversationId, messageId, rating, correction. A weekly Azure Functions batch job analyzes feedback. |
| **Frontend** | Thumbs up/down buttons on each agent answer. Detailed feedback modal. Feedback dashboard for admins. |
| **Acceptance** | Frictionless feedback recording (1 click). Dashboard with satisfaction metrics. Thumbs up rate > 80% as a target. |

---

## 7. Feature Details — Release 3.0

> **Risk** — Risk analysis + automated reports
>
> **Strategy:** Completely new features built on top of the R2.0 agent infrastructure.

### F3.1 — Legal Risk Analysis

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — Does not exist |
| **Description** | The user describes a legal case or situation and the system generates a structured risk analysis. Includes: normative assessment, favorable/unfavorable case law, risk factors, estimated probability of success, recommendations. |
| **Backend** | `POST /api/risk/analyze` → Semantic Kernel `RiskAgentPlugin`. Combines outputs from the regulatory and case law agents. Generates structured JSON with scores. Persists the analysis in Azure SQL. |
| **Frontend** | `RiskAnalysisComponent` with: a case input form (textarea + branch/jurisdiction selection), a result view with collapsible sections (normative, case law, factors, score), a visual gauge of the probability of success, a "Generate .docx report" button. |
| **Acceptance** | Analysis generated in < 30 sec. Risk score consistent with the cited case law. Exportable report. |
| **Role** | Lawyers only |

### F3.2 — Risk Analysis History

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — Does not exist |
| **Description** | List of all risk analyses generated by the firm. Filterable by branch, date, lawyer, risk score. Allows re-running an analysis with updated data. |
| **Backend** | `GET /api/risk/history?branch=labor&from=2026-01-01`. Stored in Azure SQL with a snapshot of the sources used. |
| **Frontend** | `RiskHistoryComponent` with a DataTable. Columns: date, case (summary), branch, score, lawyer. Clicking opens the full analysis. A "Re-analyze" button runs a new analysis with the updated KB. |
| **Acceptance** | Paginated and filterable history. Functional re-analysis with a visual diff vs the previous analysis. |

### F3.3 — Legal Report Generation

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — Does not exist |
| **Description** | Automated generation of .docx documents from templates: risk reports, opinions, case law summaries, legal memos. The AI agents complete the content and the system generates the formatted document. |
| **Backend** | `POST /api/legal-reports/generate` with body: `{type, data, caseFileId?}`. The backend uses DocumentFormat.OpenXml (.NET) to generate the .docx from templates stored in Blob Storage. Output saved in Blob Storage. |
| **Frontend** | `ReportGenerateComponent` with: a report type selector, a dynamic form by type, a preview (rendering the .docx in an iframe or PDF viewer), a download button. `ReportListComponent` with a history of generated reports. |
| **Acceptance** | .docx generated correctly with professional formatting. Functional preview. Direct download. |
| **Role** | Lawyers (all types) · Administrative (operational reports only) |

### F3.4 — Operational Reports

| Field | Detail |
|-------|--------|
| **MVP** | 🟡 30% — `estadisticas` view with counts and basic KB stats |
| **Delta** | Transform into complete dashboards with interactive charts, filters, and export. |
| **Description** | Firm management dashboards and reports: number of case files by status, overdue deadlines, workload by lawyer, average resolution time, AI agent usage statistics. |
| **Backend** | `GET /api/reports/case-files-by-status`, `/api/reports/overdue-deadlines`, `/api/reports/workload-by-lawyer`. Aggregate queries over Azure SQL. |
| **Frontend** | `ReportsComponent` with charts: bars (case files by venue), pie (statuses), line (monthly trend), heatmap (workload by lawyer/week). Library: ngx-charts or Chart.js. Export to PDF/Excel. |
| **Acceptance** | Charts rendered correctly. Data consistent with the database. Functional export. |
| **Role** | Lawyers and Administrative |

---

## 8. Feature Details — Release 4.0

> **Operations** — Observability, advanced alerts, hardening
>
> **Strategy:** Focus on production operation: full observability, configurable alerts, PWA, and gradual migration of workers to Azure Functions.

### F4.1 — Observability and LLMOps

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — No observability |
| **Description** | Implement a complete observability stack: OpenTelemetry for distributed tracing, Application Insights for metrics and alerts, custom telemetry for LLM (tokens, latency, costs), semantic caching, circuit breaker with Polly. |
| **Backend** | OpenTelemetry SDK → Application Insights exporter. Custom counters: `llm.tokens.input`, `llm.tokens.output`, `llm.latency_ms`, `llm.cost_usd`. Semantic cache with TTL per query type. Circuit breaker (Polly v8) for Azure OpenAI. |
| **Dashboards** | 6 panels: Request Overview, LLM Performance, RAG Quality, Pipeline Health, Cost Tracking, Error Analysis. |
| **Alerts** | 6 alerts: P95 latency > 10s, error rate > 5%, daily cost > threshold, circuit breaker open, negative feedback > 15%, drift in eval metrics. |
| **Acceptance** | End-to-end tracing functional (request → agent → tool → search → response). Cost dashboard updated in < 1 hour. Circuit breaker protects against Azure OpenAI downtime. |

### F4.2 — Advanced Alert Configuration

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — Does not exist |
| **Description** | Users configure custom alerts: regulatory changes in specific branches, due dates of assigned case files, new rulings from courts of interest, status changes in cases. |
| **Backend** | CRUD `/api/alerts/configuration`. Azure Functions evaluate conditions and generate notifications via Storage Queue → SignalR. Channel options: in-app push, email. |
| **Frontend** | `AlertConfigComponent` with a wizard: alert type selection, condition configuration (branch, court, case file), notification channel, frequency. `AlertCenterComponent` with an alert inbox (read/unread, archived). |
| **Acceptance** | Alerts generated within 30 minutes of the trigger event. Functional inbox with mark as read. Functional email delivery. |

### F4.3 — PWA Offline Mode

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — Does not exist |
| **Description** | Service worker for limited offline functionality: access to previously consulted norms, cached case files, the day's deadlines. Sync on reconnect. |
| **Backend** | The API supports ETags and caching headers. Manifest.json for the PWA. |
| **Frontend** | Angular PWA with `@angular/service-worker`. Cache strategy: network-first for data, cache-first for assets. IndexedDB for favorite norms and case files. |
| **Acceptance** | Offline access to the last 50 consulted norms and active case files. Sync on reconnect with no data loss. |

### F4.4 — Gradual Migration to Azure Functions

| Field | Detail |
|-------|--------|
| **MVP** | The current workers are BackgroundServices (queue polling) in the API process |
| **Delta** | Evaluate and selectively migrate workers to Azure Functions (event-driven, consumption plan). |
| **Description** | The 5 ingestion pipeline workers (Discoverer, Fetcher, Parser, Enrichment, Indexer) currently run as BackgroundServices. Migrating to Azure Functions enables independent scaling, pay-per-execution, and native Storage Queue triggers. |
| **Backend** | Azure Functions .NET 10 isolated worker. Queue triggers replace polling. Keep the strategy pattern per source. Timer trigger for the Official Gazette. |
| **Criterion** | Only migrate if the ingestion volume justifies the Functions overhead. If the volume is low, the MVP's BackgroundServices are sufficient. |

### F4.5 — Model Versioning and Canary Deploys

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — Does not exist |
| **Description** | Model and prompt versioning with canary deploys: 10% traffic to a new version → evaluate metrics → ramp up or automatic rollback. |
| **Backend** | `ModelVersionConfig` in SQL. Feature flags for traffic routing. Automatic comparative A/B metrics. |
| **Acceptance** | Functional canary deploy. Automatic rollback if metrics degrade > 10%. Rollback time < 5 minutes. |

---

## 9. Cross-Cutting Features

These features apply to the whole application and are implemented progressively:

### FT.1 — Real-Time Notifications

| Field | Detail |
|-------|--------|
| **MVP** | 🔴 0% — Does not exist as a user notification system |
| **Delta** | Implement SignalR for user notifications: deadlines, regulatory updates, alerts. |
| **Description** | In-app push notification system via SignalR. A navbar badge with the unread count. Toast notifications for urgent events (deadlines < 24h). |
| **Backend** | Azure SignalR Service. Hub: `NotificationHub` with methods `SendAlert()`, `UpdateStatus()`. Azure Functions publish to a Storage Queue → a worker reads and pushes via SignalR. |
| **Frontend** | `NotificationService` (singleton) maintains the SignalR connection. `NotificationBadgeComponent` in the navbar. `ToastComponent` for urgent alerts. `NotificationCenterComponent` with the full list. |
| **Implementation** | R1.0: base SignalR infrastructure. R2.0: deadline notifications. R4.0: configurable advanced alerts. |

### FT.2 — Global Search (Omnisearch)

| Field | Detail |
|-------|--------|
| **MVP** | 🟢 **80%** — Functional command palette (Ctrl+K) with multi-type search |
| **Delta** | Add search in case files and conversations. Improve result grouping. |
| **Description** | A unified search accessible with `Ctrl+K` from any view. Searches simultaneously across norms, case law, case files, and agent conversations. Results grouped by type. |
| **Backend** | Reuse the MVP's command palette. Add `GET /api/search/global?q=despido+sin+causa` → AI Search multi-index query + Azure SQL for case files. |
| **Frontend** | Evolve the MVP's `OmnisearchComponent`. Add a result group: Case Files, Conversations. Keep the existing keyboard navigation. |
| **Implementation** | R1.0: polish (already functional). R2.0: add case files and conversations. |

### FT.3 — Theme and Accessibility

| Field | Detail |
|-------|--------|
| **MVP** | 🟡 Partial — PwC AppKit 4 partially configured |
| **Description** | Support for light/dark theme. WCAG 2.1 AA compliance. Responsive design (desktop-first, functional on tablet). |
| **Frontend** | Angular Material theming with CSS custom properties. `prefers-color-scheme` detection. Focus visible, ARIA labels, semantic HTML. Breakpoints: desktop (>1200px), tablet (768-1200px). |
| **Implementation** | Progressive across all releases. |

### FT.4 — Audit and Logging

| Field | Detail |
|-------|--------|
| **MVP** | 🟡 Partial — `DocumentStageLog` for pipeline tracking, basic logging |
| **Delta** | Add audit middleware for user actions. Application Insights for technical telemetry. |
| **Description** | Recording of all significant actions: searches performed, case files consulted, documents downloaded, risk analyses generated. Compliance with Ley 25.326 on personal data. |
| **Backend** | Audit middleware in .NET 10. Events written to Azure SQL (AuditLog table). Application Insights for technical telemetry. Retention: 2 years operational, 5 years archived in Blob Storage (cool tier). |
| **Implementation** | R1.0: base middleware + Application Insights. R4.0: complete audit dashboard. |

### FT.5 — Delivery and Hosting (GitHub + GCaaS)

| Field | Detail |
|-------|--------|
| **MVP** | 🟢 70% — `ci.yml` + `cd.yml` deploy API + SPA to **Azure staging**; GCaaS Helm chart (`mvp/deployment/`) with Knative, Istio, Vault, platform Entra auth already implemented |
| **Delta** | Document and reconcile the dual delivery model; complete the GCaaS production verification/rollback runbook; align worker deploy and production promotion (the implemented `cd.yml` covers API + SPA staging only). |
| **Description** | Two complementary paths (see §2.3): GitHub Actions → Azure staging, and GCaaS Helm → corporate production with Entra SSO (`id_token` cookie) and Vault secrets. Both can share the same Azure data services. |
| **References** | [`github-delivery.md`](../deployment/github-delivery.md), [`gcaas-hosting.md`](../deployment/gcaas-hosting.md) |
| **Work items** | See backlog: FT05-W01..W06 (CI, CD to Azure staging, GCaaS Helm/Knative, platform auth, Vault secrets, verification + rollback). |
| **Note** | This complements the "infrastructure managed outside the feature roadmap" stance: it is documented and tracked, but CI/CD and IaC remain operated by the delivery track, not as application features. |

---

## 10. Detailed Tech Stack

### Frontend

| Technology | Version | Use | MVP |
|------------|---------|-----|-----|
| Angular | 19.x | SPA framework | ✅ |
| PwC AppKit 4 | latest | UI Library + guidelines | ✅ Partial |
| Tailwind CSS | 4.x | Utility-first CSS | Add |
| NgRx Signal Store | 19.x | State management with signals | Add |
| SignalR Client | 9.x | Real-time notifications | Add |
| Platform session (GCaaS) | — | Entra SSO via `id_token` cookie + `withCredentials` (no MSAL) | ✅ In MVP (auth.service, interceptors) |
| ngx-markdown | latest | Markdown rendering (agent answers) | Add |
| Cytoscape.js | 3.x | Graph visualization | ✅ |
| FullCalendar | 6.x | Calendar component | Add (R2.0) |
| ngx-charts / Chart.js | latest | Report charts | Add (R3.0) |
| Jest | 30.x | Unit testing | ✅ |
| Playwright | latest | E2E testing | Migrate from current tests |

### Backend

| Technology | Version | Use | MVP |
|------------|---------|-----|-----|
| .NET | 10 (LTS) | Runtime | ✅ |
| ASP.NET Core | 10 | Minimal API (gradual refactor from Controllers) | ✅ Controllers |
| Entity Framework Core | 10 | ORM + SQL Graph mapping | ✅ |
| Semantic Kernel | latest | AI agent orchestration (R2.0) | Replaces OpenAI SDK |
| Azure Functions | .NET 10 isolated | ETL, jobs, triggers (R4.0 — evaluate) | Workers as BackgroundService |
| Platform auth (`Auth:Platform`) | — | `id_token` cookie JWT validation against Entra OIDC (custom `PlatformAuthenticationHandler`) | ✅ In MVP |
| DocumentFormat.OpenXml | latest | .docx generation (R3.0) | Add |
| Polly | 8.x | Retry + Circuit breaker | ✅ (retry only) |
| FluentValidation | 12.x | Request validation | ✅ |
| PdfPig | 0.1.x | PDF parsing | ✅ (evaluate migration to Azure Doc Intelligence) |
| SharpToken | 2.x | Tokenization for prompts | ✅ |
| xUnit + NSubstitute | latest | Testing | ✅ |
| Custom IMediator | — | CQRS pattern | ✅ (evaluate MediatR in R2.0) |

### Azure Services

| Service | Use | MVP | Phase |
|---------|-----|-----|-------|
| Azure SQL Database | Relational data + Graph Tables | ✅ | Existing |
| Azure AI Search | Hybrid search (BM25 + vectors) | ✅ (3 indexes) | Existing |
| Azure OpenAI Service | Embeddings (3072d) + LLM (GPT-4o, GPT-4o-mini) | ✅ | Existing |
| Azure Storage | Blobs (documents) + Queues (5 pipeline queues) | ✅ | Existing |
| Azure Functions | ETL triggers, timer jobs | ❌ | R4.0 (evaluate) |
| Azure App Service | Hosting API + Angular SPA | ✅ | Existing |
| Application Insights | Monitoring and telemetry | ❌ | R1.0 basic, R4.0 complete |
| Azure SignalR Service | Push notifications | Add | R1.0 |

### Platform and Hosting (GCaaS — corporate production)

See [`gcaas-hosting.md`](../deployment/gcaas-hosting.md). Images are built by the GCaaS platform, not by GitHub Actions.

| Component | Use | MVP | Notes |
|-----------|-----|-----|-------|
| GCaaS (PwC) | Corporate Kubernetes hosting platform | ✅ Helm chart in `mvp/deployment/` | Knative + Istio + Envoy |
| Knative | Serverless container runtime for API + SPA | ✅ `templates/ksvc.yaml` | `backend` (port 8080), `frontend` (port 8081) |
| Istio VirtualServices | Ingress routing (legacy host + Entra host) | ✅ | `*-vs-entra` when `authentication.entra: true` |
| Microsoft Entra ID (Envoy) | Platform SSO → `id_token` cookie | ✅ | API validates the cookie JWT |
| HashiCorp Vault | Secret store mapped into the release | ✅ `templates/secrets.yaml` | `Azure*__*`, `Auth__Platform__*` keys |
| Helm | GCaaS deployment packaging | ✅ `mvp/deployment/` | `Chart.yaml`, `values.yaml`, templates |
| Azure Static Web Apps | SPA hosting (GitHub CD staging path) | Add | Deployed by `cd.yml` |
| Datadog | Optional observability via platform labels | Optional | `gcaas_datadog_enabled` |

---

## 11. Permissions Matrix by Role

| Feature | Lawyer | Administrative |
|---------|:------:|:--------------:|
| Dashboard | Full | No AI agents widget |
| Legal norm search | Full | Full |
| Case law search | Full | Read-only |
| Norm/article detail | Full | Full |
| AI agent chat | Full | No access |
| Graph explorer | Full | No access |
| Case file management | Full CRUD | Read + add movements |
| Deadline management | Full CRUD | Full CRUD |
| Legal calendar | Full | Full |
| Risk analysis | Full | No access |
| Report generation | All types | Operational reports only |
| Operational reports | Full | Full |
| Configurable alerts | Full | Deadlines and case files only |
| User administration | With admin permission only | No access |
| Ingestion administration | With admin permission only | No access |
| Agent feedback | Full | No access |

---

## 12. API Endpoints by Module

### Auth

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/auth/me` | Authenticated user's profile |
| GET | `/api/auth/permissions` | Current user's permissions |

### Dashboard

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/dashboard` | Aggregated dashboard data |
| GET | `/api/dashboard/updates` | Latest regulatory updates |

### Search

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/search/legal-norms` | Semantic search of norms |
| POST | `/api/search/case-law` | Semantic search of rulings |
| GET | `/api/search/global` | Unified omnisearch |
| GET | `/api/search/suggestions` | Search autocomplete |

### Legal Norms

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/legal-norms/{id}` | Norm detail |
| GET | `/api/legal-norms/{id}/articles` | Norm's articles |
| GET | `/api/legal-norms/{id}/graph` | Relationship graph |
| GET | `/api/legal-norms/{id}/history` | Amendment history |
| GET | `/api/articles/{id}` | Article detail |
| GET | `/api/articles/{id}/case-law` | Related case law |

### Graph

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/graph/explore` | Explore the graph from a node with depth |
| GET | `/api/graph/communities` | List detected communities |
| GET | `/api/graph/communities/{id}` | Community detail with summary |

### AI Agents

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/agents/chat` | Send a message to the agent (SSE streaming) |
| GET | `/api/agents/conversations` | Conversation history |
| GET | `/api/agents/conversations/{id}` | Conversation detail |
| DELETE | `/api/agents/conversations/{id}` | Delete conversation |

### Case Files

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/case-files` | List case files (with filters) |
| POST | `/api/case-files` | Create case file |
| GET | `/api/case-files/{id}` | Case file detail |
| PUT | `/api/case-files/{id}` | Update case file |
| DELETE | `/api/case-files/{id}` | Delete case file |
| GET | `/api/case-files/{id}/movements` | Case file movements |
| POST | `/api/case-files/{id}/movements` | Record a movement |
| GET | `/api/case-files/{id}/documents` | Attached documents |
| POST | `/api/case-files/{id}/documents` | Upload a document |

### Deadlines

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/deadlines` | List deadlines (with filters) |
| POST | `/api/deadlines` | Create deadline |
| PUT | `/api/deadlines/{id}` | Update deadline |
| PUT | `/api/deadlines/{id}/complete` | Mark deadline as fulfilled |
| GET | `/api/deadlines/upcoming` | Deadlines due soon |

### Calendar

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/calendar` | Events in a date range |

### Risk Analysis

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/risk/analyze` | Generate a risk analysis |
| GET | `/api/risk/history` | Analysis history |
| GET | `/api/risk/{id}` | Analysis detail |
| POST | `/api/risk/{id}/re-analyze` | Re-run with current data |

### Reports (legal documents)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/legal-reports/generate` | Generate a .docx report |
| GET | `/api/legal-reports` | Report history |
| GET | `/api/legal-reports/{id}/download` | Download a report |

### Operational Reports

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/reports/case-files-by-status` | Case files grouped by status |
| GET | `/api/reports/overdue-deadlines` | Overdue deadlines by period |
| GET | `/api/reports/workload-by-lawyer` | Workload by lawyer |
| GET | `/api/reports/agent-usage` | AI agent usage statistics |

### Alerts

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/alerts` | User's alerts (inbox) |
| PUT | `/api/alerts/{id}/read` | Mark an alert as read |
| GET | `/api/alerts/configuration` | Alert configurations |
| POST | `/api/alerts/configuration` | Create an alert configuration |
| PUT | `/api/alerts/configuration/{id}` | Update a configuration |
| DELETE | `/api/alerts/configuration/{id}` | Delete a configuration |

### Admin

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/users` | List users |
| PUT | `/api/admin/users/{id}/role` | Change a user's role |
| GET | `/api/admin/audit` | Audit log |
| GET | `/api/admin/feedback` | Agent feedback dashboard |
| GET | `/api/admin/prompts` | List prompt templates |
| PUT | `/api/admin/prompts/{id}` | Update a prompt template |
| GET | `/api/admin/ingestion/jobs` | Ingestion job status |
| GET | `/api/admin/ingestion/dlq` | Dead letter queue |
| POST | `/api/admin/ingestion/dlq/{id}/retry` | Retry a failed document |

### Feedback

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/feedback` | Send feedback on an agent answer |

---

## 13. KPIs and Success Metrics

| KPI | Target | Measurement |
|-----|--------|-------------|
| Average legal research time | < 2 minutes (vs. 30+ min manual) | Application Insights: duration of search sessions |
| Missed deadlines without justification | 0 per month | Azure SQL: deadlines with "overdue" status and no fulfilled flag |
| Satisfaction with AI agents | > 80% thumbs up | ResponseFeedback table: positive/total ratio |
| Risk analysis precision | > 70% agreement with the actual outcome | Lawyer feedback after case resolution |
| System adoption | > 90% weekly active users | Application Insights: unique users/week vs total |
| System uptime | > 99.5% | Azure Monitor: App Service availability |
| Agent response time | < 5 sec to first token | Application Insights: latency of `/api/agents/chat` |
| Updated norms | < 24h from publication in the Official Gazette | Delta between publicationDate and ingestionDate in Azure SQL |
| MVP code reuse | > 75% | Migrated files vs total |
| Golden set coverage | > 90% Recall@10 | Evaluation pipeline in CI |
| Monthly LLM cost | < $500/month | Custom telemetry in Application Insights |

---

*Legal Ai Ar — Features Roadmap — v2.0 — May 2026*
*Based on the evolution of the `legal-ai-ar` MVP — ~78% reusable code*
