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

| Work Item | Type | SP | Status |
|-----------|------|----|--------|
| F00-W01 Comprehensive Documentation | doc | 5 | ✅ done |
| F00-W02 Monorepo Setup & Backend Scaffolding | backend | 5 | ✅ done (PR #102) |
| F00-W03 Angular 19 Frontend Scaffolding | frontend | 5 | ⏭ next |
| F00-W04 GitHub Actions CI Configuration | devops | 5 | pending |
| F00-W05 Azure Infrastructure with Bicep | devops | 8 | pending |
| F00-W06 CD Deployment Pipelines Configuration | devops | 5 | pending |
| F00-W07 Local Environment Setup & Onboarding Guide | doc | 2 | pending |
| F00-W08 Code Quality Configuration | devops | 3 | pending |

---

## Feature catalog (R1.0–R4.0) — work items generated per feature when scheduled

### Release 1.0 — Research & Monitoring

| ID | Feature |
|----|---------|
| F1.1 | Authentication & corporate SSO (Entra `id_token`) |
| F1.2 | Home: my projects, tasks & alerts |
| F1.3 | Tax sources ingestion + search (dictámenes, Comisión Arbitral, Tribunal Fiscal, consultas vinculantes) |
| F1.4 | Legislation search |
| F1.5 | Case-law search |
| F1.6 | Norm / article detail (+ graph) |
| F1.7 | AI research assistant (chat over KB) |
| F1.8 | Regulatory/tax monitoring & alerts |

### Release 2.0 — Professional Productivity

| ID | Feature |
|----|---------|
| F2.1 | Projects / Workspaces (docs, saved research, tasks, deadlines, team & roles) |
| F2.2 | Document review & analysis |
| F2.3 | Deliverable generation (memos / opinions / reports) |
| F2.4 | Project tasks & deadlines |

### Release 3.0 — Knowledge & Intelligence

| ID | Feature |
|----|---------|
| F3.1 | PwC internal KB (precedents/memos, citable, per-project confidential) |
| F3.2 | Tax agent + regulatory & case-law agents (SK router) |
| F3.3 | Legal/tax/compliance risk analysis (+ history) |
| F3.4 | Tax controversy (tracking, light) |

### Release 4.0 — Scale & Operations

| ID | Feature |
|----|---------|
| F4.1 | Practice/project reporting |
| F4.2 | User & role administration |
| F4.3 | Legal graph explorer |
| F4.4 | Assistant feedback & improvement |
| F4.5 | Observability & LLMOps |

### Cross-cutting (FT)

| ID | Feature |
|----|---------|
| FT.1 | Real-time notifications |
| FT.2 | Global omnisearch |
| FT.3 | Theme & accessibility |
| FT.4 | Audit & logging |
| FT.5 | Delivery & hosting (GitHub + GCaaS) |

---

*Work Item Backlog — Legal Ai Ar (PwC Tax-Legal)*
