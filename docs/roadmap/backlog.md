# Legal Ai Ar — Work Item Backlog

> **Re-scoped 2026-05-29 to the PwC tax-legal product** (see [`features.md`](features.md) and
> [`PIVOT-PWC-TAX.md`](PIVOT-PWC-TAX.md)).
> Stack: Angular 19 + .NET 10 + Azure.

---

## How work items are created now

Detailed work items are **generated per feature when the feature is scheduled**, using the
`work-item-generator` skill (each feature gets a `W01 - Comprehensive Documentation` + `W0n`
implementation tickets, following the standard template and the
[Definition of Done](DEFINITION-OF-DONE.md)). Only **R0.0 (F0.0)** has detailed tickets today.

> The previous litigation-oriented work-item folders (`F01 … F23`, `FT01 … FT.5`) describe the **old**
> roadmap and are being retired with the pivot. Until each new feature is generated, this file is the
> feature-level index.

---

## Release 0.0 — Development Environment & Structure (active)

> **Architecture baseline:** All R0.0 work items must align with [`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md). F0.0-W03 (frontend) and F0.0-W08 (quality gates) are the primary alignment tickets.

| Work Item                                                     | Type     | SP  | Status            |
| ------------------------------------------------------------- | -------- | --- | ----------------- |
| F0.0-W01 Comprehensive Documentation                           | doc      | 5   | ✅ done           |
| F0.0-W02 Monorepo Setup & Backend Scaffolding                  | backend  | 5   | ✅ done (PR #102) |
| F0.0-W03 Angular 19 Frontend Scaffolding                       | frontend | 5   | ✅ done (PR #105) |
| F0.0-W07 Local Environment Setup & Onboarding Guide            | doc      | 2   | ✅ done           |
| F0.0-W08 Code Quality Configuration                            | devops   | 3   | ✅ done           |
| F0.0-W09 Minimal API Endpoint Discovery and Migration          | backend  | 8   | ✅ done (PR #111) |
| F0.0-W10 LegalAiAr.Contracts API DTO Layer                     | backend  | 5   | ✅ done (PR #112) |
| F0.0-W11 Angular Feature Schematics (schema-la)                | frontend | 3   | ✅ done (PR #113) |
| F0.0-W12 Transactional Outbox and Domain Events Infrastructure | backend  | 5   | ✅ done (PR #114) |

**R0.0 (F0.0) total:** 41 SP — **complete** (application track). **W04–W06** moved to **FT.5** (see below).

Completing **F0.0 W01–W03, W07–W12** delivers the application foundation (§4–§6, §9). **§10 Delivery**
is **FT.5** (on hold — DevOps consultation). R1 **F1.1** completes production GCaaS auth (§5); R2 **F2.1**
consumes outbox (§4.5).

### Recommended execution order — F0.0 (application)

| Phase                    | Tickets   | Delivers                         |
| ------------------------ | --------- | -------------------------------- |
| 1 — API + contracts      | W09 → W10 | §4.4 Minimal API + Contracts     |
| 2 — Frontend tooling     | W11       | §6 schematics                    |
| 3 — Event infrastructure | W12       | §4.5 outbox (used by F2.1 in R2) |

### FT.5 — Delivery and Hosting (on hold — DevOps)

> Tickets in [`FT.5 - Delivery and Hosting/`](FT.5%20-%20Delivery%20and%20Hosting/README.md). Former F0.0-W04–W06.

| Work Item                                      | Type   | SP  | Status                                 |
| ---------------------------------------------- | ------ | --- | -------------------------------------- |
| FT.5-W01 GitHub Actions CI Configuration       | devops | 5   | 🟡 in progress (blocked — Actions off) |
| FT.5-W02 Azure Infrastructure with Bicep       | devops | 8   | pending                                |
| FT.5-W03 CD Deployment Pipelines Configuration | devops | 5   | pending (after CI + infra)             |

**FT.5 total:** 18 SP. Resume after DevOps confirms CI platform, Azure landing zone, and CD scope.

---

## Feature catalog (R1.0–R4.0) — work items generated per feature when scheduled

### Release 1.0 — Research & Monitoring

> **Detailed work items generated 2026-06-02.** Folders in `docs/roadmap/F1.x - .../`.

| Work Item | Feature | Type | SP | Status |
| --------- | ------- | ---- | --:| ------ |
| F1.1-W01 | [F1.1 Auth & SSO — Comprehensive Documentation](F1.1%20-%20Authentication%20and%20SSO/F1.1%20-%20W01%20-%20Comprehensive%20Documentation.md) | doc | 3 | pending |
| F1.1-W02 | [Platform Auth Middleware and JWT Validation](F1.1%20-%20Authentication%20and%20SSO/F1.1%20-%20W02%20-%20Platform%20Auth%20Middleware%20and%20JWT%20Validation.md) | backend | 5 | pending |
| F1.1-W03 | [Frontend Auth Service, Interceptor and Guards](F1.1%20-%20Authentication%20and%20SSO/F1.1%20-%20W03%20-%20Frontend%20Auth%20Service%20Interceptor%20and%20Guards.md) | frontend | 5 | pending |
| F1.2-W01 | [F1.2 Home Dashboard — Comprehensive Documentation](F1.2%20-%20Home%20Dashboard/F1.2%20-%20W01%20-%20Comprehensive%20Documentation.md) | doc | 2 | pending |
| F1.2-W02 | [Home API Endpoint and UserActivity](F1.2%20-%20Home%20Dashboard/F1.2%20-%20W02%20-%20Home%20API%20Endpoint%20and%20UserActivity.md) | backend | 3 | pending |
| F1.2-W03 | [Frontend Home Feature](F1.2%20-%20Home%20Dashboard/F1.2%20-%20W03%20-%20Frontend%20Home%20Feature.md) | frontend | 5 | pending |
| F1.3-W01 | [F1.3 Case-Law Search — Comprehensive Documentation](F1.3%20-%20Case-Law%20Search/F1.3%20-%20W01%20-%20Comprehensive%20Documentation.md) | doc | 2 | pending |
| F1.3-W02 | [Case-Law Search Backend Alignment](F1.3%20-%20Case-Law%20Search/F1.3%20-%20W02%20-%20Case-Law%20Search%20Backend%20Alignment.md) | backend | 3 | pending |
| F1.3-W03 | [Frontend Case-Law Search Feature](F1.3%20-%20Case-Law%20Search/F1.3%20-%20W03%20-%20Frontend%20Case-Law%20Search%20Feature.md) | frontend | 5 | pending |
| F1.4-W01 | [F1.4 Legislation Search — Comprehensive Documentation](F1.4%20-%20Legislation%20Search/F1.4%20-%20W01%20-%20Comprehensive%20Documentation.md) | doc | 2 | pending |
| F1.4-W02 | [Legislation Search Backend Alignment](F1.4%20-%20Legislation%20Search/F1.4%20-%20W02%20-%20Legislation%20Search%20Backend%20Alignment.md) | backend | 3 | pending |
| F1.4-W03 | [Frontend Legislation Search Feature](F1.4%20-%20Legislation%20Search/F1.4%20-%20W03%20-%20Frontend%20Legislation%20Search%20Feature.md) | frontend | 5 | pending |
| F1.5-W01 | [F1.5 Tax Sources — Comprehensive Documentation](F1.5%20-%20Tax%20Sources%20Ingestion%20and%20Search/F1.5%20-%20W01%20-%20Comprehensive%20Documentation.md) | doc | 3 | pending |
| F1.5-W02 | [Tax Sources Ingestion Pipeline Strategies](F1.5%20-%20Tax%20Sources%20Ingestion%20and%20Search/F1.5%20-%20W02%20-%20Tax%20Sources%20Ingestion%20Pipeline%20Strategies.md) | backend/worker | 8 | pending |
| F1.5-W03 | [Tax Sources AI Search Index and API Endpoints](F1.5%20-%20Tax%20Sources%20Ingestion%20and%20Search/F1.5%20-%20W03%20-%20Tax%20Sources%20AI%20Search%20Index%20and%20API%20Endpoints.md) | backend | 5 | pending |
| F1.5-W04 | [Frontend Tax Sources Search Feature](F1.5%20-%20Tax%20Sources%20Ingestion%20and%20Search/F1.5%20-%20W04%20-%20Frontend%20Tax%20Sources%20Search%20Feature.md) | frontend | 5 | pending |
| F1.6-W01 | [F1.6 Norm Detail — Comprehensive Documentation](F1.6%20-%20Norm%20and%20Article%20Detail/F1.6%20-%20W01%20-%20Comprehensive%20Documentation.md) | doc | 2 | pending |
| F1.6-W02 | [Norm Detail Endpoints and Graph Traversal](F1.6%20-%20Norm%20and%20Article%20Detail/F1.6%20-%20W02%20-%20Norm%20Detail%20Endpoints%20and%20Graph%20Traversal.md) | backend | 5 | pending |
| F1.6-W03 | [Frontend Norm Detail View and Relationship Graph](F1.6%20-%20Norm%20and%20Article%20Detail/F1.6%20-%20W03%20-%20Frontend%20Norm%20Detail%20View%20and%20Graph.md) | frontend | 5 | pending |
| F1.7-W01 | [F1.7 AI Assistant — Comprehensive Documentation](F1.7%20-%20AI%20Research%20Assistant/F1.7%20-%20W01%20-%20Comprehensive%20Documentation.md) | doc | 3 | pending |
| F1.7-W02 | [Tax-Tuned Agents and Semantic Kernel Plugins](F1.7%20-%20AI%20Research%20Assistant/F1.7%20-%20W02%20-%20Tax-Tuned%20Agents%20and%20Semantic%20Kernel%20Plugins.md) | backend/AI | 8 | pending |
| F1.7-W03 | [Chat Endpoints and Conversation Persistence](F1.7%20-%20AI%20Research%20Assistant/F1.7%20-%20W03%20-%20Chat%20Endpoints%20and%20Conversation%20Persistence.md) | backend | 5 | pending |
| F1.7-W04 | [Frontend Chat Feature](F1.7%20-%20AI%20Research%20Assistant/F1.7%20-%20W04%20-%20Frontend%20Chat%20Feature.md) | frontend | 8 | pending |
| F1.8-W01 | [F1.8 Monitoring — Comprehensive Documentation](F1.8%20-%20Regulatory%20Monitoring%20and%20Alerts/F1.8%20-%20W01%20-%20Comprehensive%20Documentation.md) | doc | 3 | pending |
| F1.8-W02 | [Alert Rules Engine and Notification Worker](F1.8%20-%20Regulatory%20Monitoring%20and%20Alerts/F1.8%20-%20W02%20-%20Alert%20Rules%20Engine%20and%20Notification%20Worker.md) | backend | 8 | pending |
| F1.8-W03 | [Frontend Monitoring Settings and Alerts](F1.8%20-%20Regulatory%20Monitoring%20and%20Alerts/F1.8%20-%20W03%20-%20Frontend%20Monitoring%20Settings%20and%20Alerts.md) | frontend | 5 | pending |

**R1.0 total: 116 SP across 26 work items (8 features)**

### Recommended execution order — R1.0

| Phase | Feature | Tickets | Delivers |
| ----- | ------- | ------- | -------- |
| 1 | **F1.1** Auth & SSO | W02 → W03 | Production auth — unblocks everything |
| 2 | **F1.2** Home Dashboard | W02 → W03 | Landing screen; user activity |
| 3 | **F1.3** Case-law Search | W02 → W03 | Jurisprudence search (MVP-aligned) |
| 4 | **F1.4** Legislation Search | W02 → W03 | Legislation search (MVP-aligned) |
| 5 | **F1.5** Tax Sources | W02 → W03 → W04 | New ingestion + tax search (unblocks F1.7) |
| 6 | **F1.6** Norm & Article Detail | W02 → W03 | Detail + graph (navigates from F1.4/F1.3) |
| 7 | **F1.7** AI Research Assistant | W02 → W03 → W04 | Flagship AI feature (needs F1.5 indexed) |
| 8 | **F1.8** Monitoring & Alerts | W02 → W03 | Regulatory alerts (needs F1.5 as source) |

### Release 2.0 — Professional Productivity

| ID   | Feature                                                                      |
| ---- | ---------------------------------------------------------------------------- |
| F2.1 | Projects / Workspaces (docs, saved research, tasks, deadlines, team & roles) |
| F2.2 | Document review & analysis                                                   |
| F2.3 | Deliverable generation (memos / opinions / reports)                          |
| F2.4 | Project tasks & deadlines                                                    |

### Release 3.0 — Knowledge & Intelligence

| ID   | Feature                                                               |
| ---- | --------------------------------------------------------------------- |
| F3.1 | PwC internal KB (precedents/memos, citable, per-project confidential) |
| F3.2 | Tax agent + regulatory & case-law agents (SK router)                  |
| F3.3 | Legal/tax/compliance risk analysis (+ history)                        |
| F3.4 | Tax controversy (tracking, light)                                     |

### Release 4.0 — Scale & Operations

| ID   | Feature                          |
| ---- | -------------------------------- |
| F4.1 | Practice/project reporting       |
| F4.2 | User & role administration       |
| F4.3 | Legal graph explorer             |
| F4.4 | Assistant feedback & improvement |
| F4.5 | Observability & LLMOps           |

### Cross-cutting (FT)

| ID   | Feature                                                                                                               |
| ---- | --------------------------------------------------------------------------------------------------------------------- |
| FT.1 | Real-time notifications                                                                                               |
| FT.2 | Global omnisearch                                                                                                     |
| FT.3 | Theme & accessibility                                                                                                 |
| FT.4 | Audit & logging                                                                                                       |
| FT.5 | Delivery & hosting (GitHub + GCaaS) — **tickets:** [`FT.5/`](FT.5%20-%20Delivery%20and%20Hosting/README.md) (on hold) |

---

_Work Item Backlog — Legal Ai Ar (PwC Tax-Legal)_
