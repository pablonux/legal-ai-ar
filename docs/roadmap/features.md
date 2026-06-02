# Legal Ai Ar — Features Roadmap

> Legal Knowledge Base + AI assistant for **PwC tax-legal professionals**, organized by
> **projects/workspaces** (each project serves a client). Daily work: tax research, document review,
> deliverable generation, and regulatory monitoring.
>
> **Base:** evolution of the `legal-ai-ar` MVP (~78% reusable code). **Stack:** Angular 19 + .NET 10 +
> Azure. **Re-scoped 2026-05-29** (see [`PIVOT-PWC-TAX.md`](PIVOT-PWC-TAX.md)).

---

## Positioning

This is **not** a generic litigation tool for any law firm. It is an **internal productivity tool for
PwC's tax-legal practice**, used daily to deliver legal-tax services to corporate clients. The unit of
work is a **Project / Workspace** (for a client), not a judicial case file. The output is **client
deliverables** (memos, opinions, reports), grounded in a legal Knowledge Base (legislation,
jurisprudence, and **tax sources**: dictámenes, Tribunal Fiscal, consultas vinculantes) and an
**internal PwC knowledge base** (precedents/memos).

**Focus:** Tax / Impositivo, advisory-first. **Tax controversy** is included as a light tracking module.

---

## Index

1. [Release 0.0 — Development Environment & Structure](#0-release-00--development-environment--structure)
2. [MVP Baseline — what already exists](#1-mvp-baseline--what-already-exists)
3. [Application overview](#2-application-overview)
4. [Release map & feature catalog](#3-release-map--feature-catalog)
5. [Feature details](#4-feature-details)
6. [Data model — business layer](#5-data-model--business-layer)
7. [Tech stack](#6-tech-stack)
8. [Roles & permissions](#7-roles--permissions)
9. [KPIs](#8-kpis--success-metrics)

---

## 0. Release 0.0 — Development Environment & Structure

> Unchanged by the product re-scope — F0.0 is environment/structure. Tickets are the files in
> `F0.0 - Development Environment and Structure/` (source of truth): **W01** Comprehensive Documentation,
> **W02** Monorepo Setup & Backend Scaffolding (done, merged PR #102), **W03** Angular 19 Frontend
> Scaffolding (done, merged PR #105), **W07** Local Env & Onboarding, **W08** Code Quality,
> **W09** Minimal API (PR #111), **W10** Contracts (PR #112), **W11** schematics (PR #113), **W12** outbox (PR #114) — **F0.0 complete**.
> **CI/Bicep/CD** (former W04–W06) → **[FT.5 — Delivery and Hosting](FT.5%20-%20Delivery%20and%20Hosting/README.md)** (on hold — DevOps).
> All align with
> [`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md).
> See [`STATUS.md`](STATUS.md) for current progress.

### Architecture standard coverage by release

| Standard section                           | R0.0 (F0.0)           | R1.0                          | R2.0+               |
| ------------------------------------------ | -------------------- | ----------------------------- | ------------------- |
| §5 Platform auth (GCaaS cookie)            | dev/simulated        | **F1.1** production           | —                   |
| §6 Frontend (AppKit, monorepo, schematics) | **W03, W11**         | full slice per new feature    | —                   |
| §4.4 Minimal API + `IEndpoint`             | **W09** (complete)   | new endpoints in R1+ features | —                   |
| §4 Contracts DTOs                          | **W10**              | expand per feature            | —                   |
| §4.5 Outbox + domain events                | **W12** (complete)   | —                             | **F2.1** aggregates |
| §9 NetArchTest, quality                    | **W08**              | —                             | —                   |
| §10 Delivery dual path                     | **FT.5** (W01–W03)   | —                             | —                   |
| §8 AI extension                            | exists (Agents)      | **F1.7**, F3.2 tune           | —                   |
| §7 Observability (OTel)                    | App Insights partial | —                             | **F4.5**            |

**R0.0 (F0.0) total:** 41 SP — **application track complete** (W01–W03, W07–W12, merged to `main`). **FT.5** adds 18 SP for delivery (on hold). FT.5 closes §10 when DevOps unblocks; **R1** adds product features; **R2** uses the outbox with transactional workspaces (F2.1).

---

## 1. MVP Baseline — what already exists

The MVP is a solid base (~78% reusable) and is documented in `docs/technical/`:

- **Ingestion pipeline** (6 stages: Discoverer → Fetcher → Parser → Enricher → Persister → Indexer) for **CSJN** rulings and **SAIJ** legislation/jurisprudence + the **SAIJ thesaurus**. See tech docs 13–15. _(Tax sources are a new ingestion target — see F1.5.)_
- **KB data model** (~44 EF Core entities) — rulings, norms, statutes, citation graph, thesaurus. Tech doc 17.
- **RAG / chat** — agentic tool-calling assistant with 13 tools, hybrid search (BM25 + 3072-d vectors) + GraphRAG, SSE streaming. Tech doc 16.
- **Frontend** — Angular SPA with ~15 views (search, detail, chat, graph, admin). Tech doc 18.
- **Admin / pipeline ops** — jobs, DLQ, traceability, SignalR worker control. Tech doc 19.

**Main gaps for the PwC tax-legal product:** project/workspace layer, document review/analysis,
deliverable generation, internal KB, tax-source ingestion, tax-tuned agents, tax controversy tracking.

---

## 2. Application Overview

### 2.1 Objectives

Help a PwC tax-legal professional, within a client **project/workspace**: (1) research tax/legal
questions fast over a curated KB + AI assistant; (2) review and analyze client documents; (3) generate
client deliverables (memos/opinions/reports) with citations; (4) monitor regulatory/tax changes; (5)
optionally track tax controversies and their deadlines.

### 2.2 Unit of work — Project / Workspace

Work is organized in **projects** (each for a client). A workspace holds: documents, saved research
(queries, cited items, chat threads), deliverables & tasks, deadlines/milestones, and the **team**
(several professionals with roles), with comments and assignments. Confidentiality is **per project**.

### 2.3 Roles

| Role                   | Scope                                                                   |
| ---------------------- | ----------------------------------------------------------------------- |
| **Partner / Manager**  | Full access to their projects; assign work; approve deliverables        |
| **Senior / Associate** | Work within assigned projects: research, documents, deliverables, tasks |
| **Admin**              | User & role administration, sources, system config                      |
| _(Viewer)_             | Read-only access to specific projects (optional)                        |

### 2.4 Deployment & hosting

Internal PwC app: **Entra ID SSO** via the platform (`id_token` HTTP-only cookie validated by the API),
hosted on **GCaaS** (Knative/Istio/Helm, Vault secrets). No MSAL / app-owned login. See
`docs/deployment/gcaas-hosting.md` and feature **FT.5**.

---

## 3. Release map & feature catalog

| Release  | Name                      | Focus                                                                  |
| -------- | ------------------------- | ---------------------------------------------------------------------- |
| **R0.0** | Preparation               | Dev environment & structure (F0.0)                                      |
| **R1.0** | Research & Monitoring     | KB research (incl. tax sources) + AI assistant + regulatory monitoring |
| **R2.0** | Professional Productivity | Projects/Workspaces + Document review + Deliverable generation         |
| **R3.0** | Knowledge & Intelligence  | Internal KB + tax/specialized agents + risk + tax controversy          |
| **R4.0** | Scale & Operations        | Reporting, admin, graph, feedback, observability, PWA, delivery        |

### Feature catalog

**R1.0 — Research & Monitoring**

| ID   | Feature                                                                                                                    | MVP coverage                        |
| ---- | -------------------------------------------------------------------------------------------------------------------------- | ----------------------------------- |
| F1.1 | Authentication & corporate SSO (Entra `id_token`)                                                                          | simulated → platform                |
| F1.2 | Home: "my projects", tasks & alerts                                                                                        | dashboard exists → reframe          |
| F1.5 | **Tax sources**: ingest + search of dictámenes (ARCA/ARBA/AGIP), Comisión Arbitral, Tribunal Fiscal, consultas vinculantes | **new** (catalogued in tech doc 20) |
| F1.4 | Legislation search                                                                                                         | ~90%                                |
| F1.3 | Case-law (jurisprudence) search                                                                                            | ~90%                                |
| F1.6 | Norm / article detail (+ relationship graph)                                                                               | ~85%                                |
| F1.7 | AI research assistant (chat over the KB)                                                                                   | ~80% — tax-tune                     |
| F1.8 | Regulatory/tax monitoring & alerts (by industry/project)                                                                   | partial                             |

**R2.0 — Professional Productivity**

| ID   | Feature                                                                                                                                                               | MVP coverage               |
| ---- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------- |
| F2.1 | **Projects / Workspaces** (documents, saved research, tasks, deadlines, team & roles, comments)                                                                       | **new**                    |
| F2.2 | **Document review & analysis** (upload contracts/resolutions/working papers; extract clauses/obligations/risks/tax implications; version compare; tabular extraction) | **new**                    |
| F2.3 | **Deliverable generation** (memos, legal-tax opinions, reports — PwC templates + KB citations; export docx/pdf)                                                       | report-gen exists → expand |
| F2.4 | Project tasks & deadlines (regulatory/tax/contractual)                                                                                                                | deadline logic reusable    |

**R3.0 — Knowledge & Intelligence**

| ID   | Feature                                                                                                                                                                              | MVP coverage               |
| ---- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | -------------------------- |
| F3.1 | **PwC internal KB** (precedents/memos/opinions, searchable & **citable by the assistant**, confidential per project)                                                                 | **new**                    |
| F3.2 | **Tax agent** + regulatory & case-law research agents (Semantic Kernel router)                                                                                                       | generic agent → specialize |
| F3.3 | Legal/tax/compliance **risk analysis** (+ history)                                                                                                                                   | 0%                         |
| F3.4 | **Tax controversy (tracking, light)** — stage/status of claims before ARCA / Tribunal Fiscal, computed deadlines + alerts, linked documents & KB citations (no procedural agent yet) | **new (light)**            |

**R4.0 — Scale & Operations**

| ID   | Feature                                                      | MVP coverage                |
| ---- | ------------------------------------------------------------ | --------------------------- |
| F4.1 | Practice/project reporting (utilization, deliverable status) | reframe operational reports |
| F4.2 | User & role administration                                   | exists                      |
| F4.3 | Legal graph explorer                                         | exists                      |
| F4.4 | Assistant feedback & improvement                             | 0%                          |
| F4.5 | Observability & LLMOps                                       | 0%                          |

**Cross-cutting (FT)**

| ID   | Feature                                                             |
| ---- | ------------------------------------------------------------------- |
| FT.1 | Real-time notifications (SignalR)                                   |
| FT.2 | Global omnisearch                                                   |
| FT.3 | Theme & accessibility (AppKit 4, WCAG 2.1 AA)                       |
| FT.4 | Audit & logging                                                     |
| FT.5 | Delivery & hosting (GitHub CI/CD → Azure staging; GCaaS production) |

> **Dropped** (litigation-only, not applicable to tax advisory): judicial case-file management as the
> unit of work, hearings (_audiencias_), procedural deadline computation, the procedural agent,
> multi-instance court history. The public case-law KB **stays** for research.

---

## 4. Feature details

> Concise scope per feature. Detailed work items (W01 Comprehensive Documentation + W0n implementation
> tickets) are generated per feature with the `work-item-generator` skill when the feature is scheduled.

### R1.0

- **F1.1 Auth & SSO** — Platform Entra `id_token` cookie validated by the API; role-based authorization; SPA `withCredentials`. No MSAL.
- **F1.2 Home** — Per-user landing: my projects, pending tasks/deadlines, regulatory alerts, recent research.
- **F1.5 Tax sources** — Ingest and index dictámenes (ARCA/ARBA/AGIP), Comisión Arbitral resolutions, Tribunal Fiscal, consultas vinculantes (sources in tech doc 20). Each source needs its discovery/parse strategy (reuse the pipeline's strategy pattern).
- **F1.4 / F1.3 Search** — Hybrid search over legislation and jurisprudence with filters; results with highlight.
- **F1.6 Detail** — Norm/article detail with relationship graph (amendments, citations).
- **F1.7 AI assistant** — Agentic chat over the KB with tools; tax-tuned prompts; inline citations; SSE streaming.
- **F1.8 Monitoring & alerts** — Track regulatory/tax changes; subscribe by branch/industry/project; push alerts.

### R2.0

- **F2.1 Projects / Workspaces** — Create a project (for a client), add team members + roles, and within it: documents, saved research, deliverables, tasks, deadlines, comments. Confidentiality per project.
- **F2.2 Document review & analysis** — Upload client documents; AI extracts clauses, obligations, risks, tax implications; compare versions; extract tables. Results saved to the project.
- **F2.3 Deliverable generation** — Draft memos/opinions/reports from PwC templates, grounded in KB (public + internal) with inline citations; export to docx/pdf.
- **F2.4 Tasks & deadlines** — Project tasks with assignees; deadlines (regulatory/tax/contractual) with business-day computation and alerts.

### R3.0

- **F3.1 Internal KB** — Library of PwC precedents/memos/opinions; ingest + search; **citable by the assistant**; confidentiality respected per project.
- **F3.2 Specialized agents** — Semantic Kernel router over a **tax agent**, a regulatory agent, and a case-law agent; ReAct for complex queries.
- **F3.3 Risk analysis** — Assess legal/tax/compliance risk of a client scenario; visual score; history/versioning.
- **F3.4 Tax controversy (light)** — Track each controversy in a project: organism (ARCA/TFN…), tax/period, stage, status, amount in dispute, **computed deadlines + alerts**, linked documents and KB citations. No procedural agent (optional future evolution).

### R4.0

- **F4.1 Reporting** — Practice/project metrics: utilization, deliverable status, alerts; export.
- **F4.2 Admin** — Users, roles, sources, configuration; audit log.
- **F4.3 Graph explorer** — Interactive legal graph (Cytoscape) of citations/relations.
- **F4.4 Feedback** — Thumbs up/down per answer; satisfaction metrics; feeds quality.
- **F4.5 Observability & LLMOps** — Tracing, token usage, model versioning, alerts.

### Cross-cutting (FT)

- **FT.1** Real-time notifications · **FT.2** Omnisearch · **FT.3** Theme & accessibility · **FT.4** Audit & logging · **FT.5** Delivery & hosting (see `docs/deployment/*`).

---

## 5. Data model — business layer

The public KB model (rulings, norms, statutes, thesaurus, citation graph — tech doc 17) **stays**. The
re-scope adds an internal **business layer**:

`Project` / `Workspace` · `ProjectMember` (+ role) · `Document` (client/deliverable) · `Deliverable` ·
`Task` · `ProjectDeadline` · `SavedResearch` (queries, cited items, chat threads) ·
`InternalKnowledgeItem` (internal KB) · `ProjectRiskAssessment` · `TaxControversy`
(+ `ControversyEvent`, `ControversyDeadline`). Plus tax sources added to ingestion.

> Full design: [`docs/technical/21-business-workspace-model.md`](../technical/21-business-workspace-model.md).

---

## 6. Tech stack

| Layer    | Technology                                                                                  |
| -------- | ------------------------------------------------------------------------------------------- |
| Frontend | Angular 19 (standalone), **PwC AppKit 4**, Tailwind CSS 4, Cytoscape.js                     |
| Backend  | .NET 10, Clean Architecture, Minimal API, EF Core 10                                        |
| AI / RAG | Azure OpenAI (GPT-4o/5o), Semantic Kernel, text-embedding-3-large (3072d)                   |
| Data     | Azure SQL (relational + graph tables), Azure AI Search (hybrid), Azure Blob, Storage Queues |
| Realtime | Azure SignalR                                                                               |
| Hosting  | GCaaS (Knative/Istio/Helm, Vault, Entra SSO `id_token`) + GitHub Actions → Azure staging    |

---

## 7. Roles & permissions

| Capability               | Partner/Manager | Senior/Associate | Admin |
| ------------------------ | :-------------: | :--------------: | :---: |
| Research (KB, assistant) |       ✅        |        ✅        |  ✅   |
| Create / manage projects |       ✅        |     ✅ (own)     |  ✅   |
| Documents & deliverables |       ✅        |        ✅        |   —   |
| Approve deliverables     |       ✅        |        —         |   —   |
| Internal KB contribute   |       ✅        |        ✅        |  ✅   |
| User/role & source admin |        —        |        —         |  ✅   |

Project confidentiality: members see only the projects they belong to.

---

## 8. KPIs & Success Metrics

- **Research time** ↓ (question → cited answer) vs manual baseline.
- **Deliverable turnaround** ↓ (draft generated with citations).
- **Adoption**: % of tax-legal team using it weekly; projects/workspaces active.
- **Quality**: citation-validation pass rate; answer feedback (👍/👎) ratio.
- **Coverage**: tax sources ingested; internal KB items indexed.
- **Controversy**: deadlines tracked with zero missed (alerts effectiveness).

---

_Features Roadmap — Legal Ai Ar (PwC Tax-Legal)_
