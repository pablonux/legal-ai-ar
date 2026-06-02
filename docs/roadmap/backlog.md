# Legal Ai Ar — Work Item Backlog

> **Re-scoped 2026-05-29 to the PwC tax-legal product** (see [`features.md`](features.md) and
> [`PIVOT-PWC-TAX.md`](PIVOT-PWC-TAX.md)).
> Stack: Angular 19 + .NET 10 + Azure.

---

## How work items are created now

Detailed work items are **generated per feature when the feature is scheduled**, using the
`work-item-generator` skill (each feature gets a `W01 - Comprehensive Documentation` + `W0n`
implementation tickets, following the standard template and the
[Definition of Done](DEFINITION-OF-DONE.md)). Only **R0.0 (F00)** has detailed tickets today.

> The previous litigation-oriented work-item folders (`F01 … F23`, `FT01 … FT05`) describe the **old**
> roadmap and are being retired with the pivot. Until each new feature is generated, this file is the
> feature-level index.

---

## Release 0.0 — Development Environment & Structure (active)

> **Architecture baseline:** All R0.0 work items must align with [`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md). F00-W03 (frontend) and F00-W08 (quality gates) are the primary alignment tickets.

| Work Item                                                     | Type     | SP  | Status            |
| ------------------------------------------------------------- | -------- | --- | ----------------- |
| F00-W01 Comprehensive Documentation                           | doc      | 5   | ✅ done           |
| F00-W02 Monorepo Setup & Backend Scaffolding                  | backend  | 5   | ✅ done (PR #102) |
| F00-W03 Angular 19 Frontend Scaffolding                       | frontend | 5   | ✅ done (PR #105) |
| F00-W07 Local Environment Setup & Onboarding Guide            | doc      | 2   | ✅ done           |
| F00-W08 Code Quality Configuration                            | devops   | 3   | ✅ done           |
| F00-W09 Minimal API Endpoint Discovery and Migration          | backend  | 8   | ✅ done (PR #111) |
| F00-W10 LegalAiAr.Contracts API DTO Layer                     | backend  | 5   | ✅ done (PR #112) |
| F00-W11 Angular Feature Schematics (schema-la)                | frontend | 3   | ✅ done (PR #113) |
| F00-W12 Transactional Outbox and Domain Events Infrastructure | backend  | 5   | ✅ done (PR #114) |

**R0.0 (F00) total:** 41 SP — **complete** (application track). **W04–W06** moved to **FT05** (see below).

Completing **F00 W01–W03, W07–W12** delivers the application foundation (§4–§6, §9). **§10 Delivery**
is **FT05** (on hold — DevOps consultation). R1 **F1.1** completes production GCaaS auth (§5); R2 **F2.1**
consumes outbox (§4.5).

### Recommended execution order — F00 (application)

| Phase                    | Tickets   | Delivers                         |
| ------------------------ | --------- | -------------------------------- |
| 1 — API + contracts      | W09 → W10 | §4.4 Minimal API + Contracts     |
| 2 — Frontend tooling     | W11       | §6 schematics                    |
| 3 — Event infrastructure | W12       | §4.5 outbox (used by F2.1 in R2) |

### FT05 — Delivery and Hosting (on hold — DevOps)

> Tickets in [`FT05 - Delivery and Hosting/`](FT05%20-%20Delivery%20and%20Hosting/README.md). Former F00-W04–W06.

| Work Item                                      | Type   | SP  | Status                                 |
| ---------------------------------------------- | ------ | --- | -------------------------------------- |
| FT05-W01 GitHub Actions CI Configuration       | devops | 5   | 🟡 in progress (blocked — Actions off) |
| FT05-W02 Azure Infrastructure with Bicep       | devops | 8   | pending                                |
| FT05-W03 CD Deployment Pipelines Configuration | devops | 5   | pending (after CI + infra)             |

**FT05 total:** 18 SP. Resume after DevOps confirms CI platform, Azure landing zone, and CD scope.

---

## Feature catalog (R1.0–R4.0) — work items generated per feature when scheduled

### Release 1.0 — Research & Monitoring

| ID   | Feature                                                                                                |
| ---- | ------------------------------------------------------------------------------------------------------ |
| F1.1 | Authentication & corporate SSO (Entra `id_token`)                                                      |
| F1.2 | Home: my projects, tasks & alerts                                                                      |
| F1.3 | Tax sources ingestion + search (dictámenes, Comisión Arbitral, Tribunal Fiscal, consultas vinculantes) |
| F1.4 | Legislation search                                                                                     |
| F1.5 | Case-law search                                                                                        |
| F1.6 | Norm / article detail (+ graph)                                                                        |
| F1.7 | AI research assistant (chat over KB)                                                                   |
| F1.8 | Regulatory/tax monitoring & alerts                                                                     |

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
| FT.5 | Delivery & hosting (GitHub + GCaaS) — **tickets:** [`FT05/`](FT05%20-%20Delivery%20and%20Hosting/README.md) (on hold) |

---

_Work Item Backlog — Legal Ai Ar (PwC Tax-Legal)_
