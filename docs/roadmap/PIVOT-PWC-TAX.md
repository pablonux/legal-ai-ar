# Roadmap Pivot — PwC Tax-Legal

> 🟡 **Partially applied** on branch `feature/roadmap-pwc-tax-pivot` (after F00-W02 merged).
> **Done:** `features.md` and `backlog.md` rewritten to the PwC tax-legal scope; `STATUS.md` noted.
> **Pending:** (a) retire the old litigation work-item folders (`F01…F23`, `FT01…FT05`); (b) add the
> business-layer entities (Project/Workspace, Document, Deliverable, …) to the data model / ontology;
> (c) generate detailed work items per feature when scheduled.
>
> **Decided & applied:** 2026-05-29. This document records the agreed re-scope; the canonical roadmap
> is now [`features.md`](features.md).

---

## 1. New positioning

The app is **not** a generic litigation tool for any law firm handling court cases. It is an **internal
productivity tool for PwC tax-legal professionals** doing daily advisory work for corporate clients,
organized by **projects/workspaces** (each project serves a client).

**Captured decisions**

- **Focus:** Tax / Impositivo, advisory-first.
- **Core daily tasks:** tax research · document review · deliverable generation · regulatory monitoring (all four).
- **Unit of work:** **Project / Workspace** (for a client) with documents, saved research, deliverables + tasks, and **team/collaboration**.
- **Internal KB:** key — PwC precedents/memos/opinions, searchable and **citable by the assistant**, confidential per project.
- **Tax controversy:** **light module, core** (tracking only — stage/status, computed deadlines, documents, KB citations; **no** procedural agent yet).
- **Dropped:** judicial case files as the unit of work, hearings (*audiencias*), procedural deadlines, the procedural agent, multi-instance court history. (The public case-law KB stays for research.)

---

## 2. Proposed feature list (re-scoped & re-prioritized)

### R1 — Research & Monitoring (mostly existing)

| New ID | Feature | From |
|--------|---------|------|
| F01 | Authentication & corporate SSO (Entra `id_token`) | = F01 |
| F02 | Home: "my projects", tasks and alerts | reframe F02 |
| F03 | Legislation search | = F03 |
| F04 | Case-law (jurisprudence) search | = F04 |
| F05 | Norm / article detail (+ relationship graph) | merge F05/F06 |
| **F06** | **Tax sources**: ingest + search of dictámenes (ARCA/ARBA/AGIP), Comisión Arbitral, Tribunal Fiscal, consultas vinculantes | **new** (catalogued in `docs/technical/20-legal-data-sources.md`) |
| F07 | AI research assistant (chat over the KB) | = F08 |
| F08 | Regulatory/tax monitoring & alerts (by industry/project) | merge F07/F20 |

### R2 — Professional productivity (the big shift)

| New ID | Feature | From |
|--------|---------|------|
| **F09** | **Projects / Workspaces**: documents, saved research, tasks, deadlines, team & roles, comments | **new** — replaces F12 (case files) |
| **F10** | **Document review & analysis**: upload contracts/resolutions/working papers; extract clauses/obligations/risks/tax implications; version compare; tabular extraction | **new** |
| **F11** | **Deliverable generation**: memos, legal-tax opinions and reports with PwC templates + KB citations (export docx/pdf) | reframe/expand F17 |
| F12 | Project tasks & deadlines (regulatory/tax/contractual) | reframe F13/F14 |

### R3 — Knowledge & intelligence

| New ID | Feature | From |
|--------|---------|------|
| **F13** | **PwC internal KB**: precedents/memos/opinions library, searchable and **citable by the assistant**, confidential per project | **new** |
| F14 | **Tax agent** + regulatory & case-law research agents (router) | reframe F09/F10; procedural agent removed |
| F15 | Legal/tax/compliance risk analysis (+ history) | reframe F15/F16 |
| **F16** | **Tax controversy (tracking, light)**: stage/status of claims before ARCA / Tribunal Fiscal, computed deadlines + alerts, linked documents and KB citations — built on Workspaces (F09), deadlines (F12), document review (F10). **No procedural agent** (optional future evolution). | **new (light)** |

### R4 — Scale, governance & operations

| New ID | Feature | From |
|--------|---------|------|
| F17 | Practice/project reporting (utilization, deliverable status) | reframe F18 |
| F18 | User & role administration | = F19 |
| F19 | Legal graph explorer | = F21 |
| F20 | Assistant feedback | = F22 |
| F21 | Cross-cutting: notifications, omnisearch, theme/accessibility, audit, PWA, delivery | = FT01–FT05 |

> Numbering above is indicative; final IDs assigned when applying. Litigation-only features
> (judicial case files as unit of work, hearings, procedural deadlines, procedural agent) are removed.

---

## 3. Data model implications

The public KB (rulings, norms, statutes, thesaurus, graph) **stays**. A new **internal business layer**
is added:

`Project`/`Workspace` · `ProjectMember` (+ role) · `Document` (client/deliverable) · `Deliverable` ·
`Task` / `ProjectDeadline` · `SavedResearch` (queries, cited items, chat threads) ·
`InternalKnowledgeItem` (internal KB) · `ProjectRiskAssessment` · `TaxControversy` (+ `ControversyEvent`,
`ControversyDeadline`). Plus tax sources in ingestion (dictámenes, Tribunal Fiscal, consultas vinculantes).

---

## 4. Apply plan (after F00-W02 merges)

On a dedicated branch (`feature/roadmap-pwc-tax-pivot`), docs-only, respecting the
[Definition of Done](DEFINITION-OF-DONE.md):

1. Rewrite `features.md` with the re-scoped releases.
2. Regenerate `backlog.md`.
3. Restructure the work-item folders: create the new features (tax sources, workspaces, document review,
   deliverables, internal KB, tax controversy) and retire the litigation ones.
4. Update the ontology / data model (new business-layer entities) and add a `STATUS.md` log entry.
5. Update this file's banner to "applied".

**Coordination:** does not touch the `F00` folder, code, or the dev's `STATUS.md` F00 progress. Merge
**after** F00-W02 so the rewrite sits on the post-hoist paths.

---

*Roadmap Pivot — PwC Tax-Legal (proposal) — Legal Ai Ar*
