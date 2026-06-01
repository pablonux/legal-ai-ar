# Project Status — Where We Are / What's Next

> **Single source of truth for current state.** Read this first to know what is done and what to pick
> up next. Update it whenever a work item is completed or the next step changes. Committed to the repo
> so it is shared across environments (Cowork / Cursor).
>
> **Last updated:** 2026-05-29 (F00-W02 merged — PR #102)

---

## Current phase

**R0.0 (Preparation) — in progress.** Application code lives at the repo root (`backend/`, `frontend/`,
`infra/`, `deployment/`, `docker-compose.app.yml`). **F00-W02** is **done** (merged to `main` via
[PR #102](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/102)). **Next:** F00-W03.

---

## Done so far (foundation)

- **Roadmap**: [`features.md`](features.md) (releases, features, endpoints, KPIs) + [`backlog.md`](backlog.md) (187 work items, ~650 SP) + per-feature folders `F00…F23`, `FT01…FT05`.
- **Technical docs** (`docs/technical/`, 20 docs): conceptual 01–09, imported-pending-review 10–12, and **authored-from-code** 13–20 (SAIJ thesaurus, CSJN ingestion, SAIJ web ingestion, chat/RAG/agents, KB data model, frontend, admin/operations, data-sources catalog).
- **Ontology** (`docs/ontology/`), **delivery/hosting** (`docs/deployment/`), **onboarding** (`docs/onboarding/`), **AppKit 4** UI reference (`docs/appkit4/`).
- **AI config unified**: `CLAUDE.md` + `.claude/skills/` (Cowork) and `.cursor/rules/` + `.cursor/skills/` (Cursor) — same 13 skills, English-first language rule.
- **MVP docs fully integrated and `mvp/docs/` deleted.** Data sources (incl. MJN magistrados/designaciones) documented in [`docs/technical/20-legal-data-sources.md`](../technical/20-legal-data-sources.md).

---

## Next up — Release 0.0 (Preparation)

Tickets are the files in [`F00 - Development Environment and Structure/`](F00%20-%20Development%20Environment%20and%20Structure/). Pick them in order:

| Order | Ticket file | Status |
|------:|-------------|--------|
| — | F00 - W01 - Comprehensive Documentation | ✅ done (authored) |
| — | F00 - W02 - Monorepo Setup and Backend Scaffolding | ✅ done (merged PR #102) |
| **1** | **F00 - W03 - Angular 19 Frontend Scaffolding** | ⏭ **next** |
| 3 | F00 - W04 - GitHub Actions CI Configuration | pending |
| 4 | F00 - W05 - Azure Infrastructure with Bicep | pending |
| 5 | F00 - W06 - CD Deployment Pipelines Configuration | pending |
| 6 | F00 - W07 - Local Environment Setup and Onboarding Guide | pending |
| 7 | F00 - W08 - Code Quality Configuration | pending |

> ⚠️ **Adapt vs create.** The F00 tickets were drafted greenfield (their diagrams assume code already
> at the repo root and "scaffold from scratch"). In reality the MVP already provides `backend/` and
> `frontend/` at the repo root, and `docs/` was already at the root. So several F00 tickets reduce to
> **"hoist + adapt"** rather than "create new" — call this out in the `architect` step. `features.md`
> and `backlog.md` now list this same W01–W08 set (reconciled). Note W04–W06 (CI/Bicep/CD) overlap
> with feature **FT05** (delivery & hosting) — consolidate when implemented.

After R0.0: **R1.0 — Research & Monitoring**. ⚠️ The product was **re-scoped to PwC tax-legal**
(projects/workspaces, tax focus) — R1.0+ features changed; F00/R0.0 is unaffected. See
[`features.md`](features.md) and [`PIVOT-PWC-TAX.md`](PIVOT-PWC-TAX.md). The old litigation work-item
folders (`F01…F23`, `FT01…FT05`) are being retired.

---

## Recently completed — F00-W02

Monorepo hoist (`mvp/` removed), `LegalAiAr.Agents`, `LegalAiAr.AgentEvals`, docs/skills repoint,
`.github/ISSUE_TEMPLATE/`. Build: 0 errors on `main`; analyzer warnings deferred to **F00-W08**.
Work item: [`F00 - W02 - Monorepo Setup and Backend Scaffolding.md`](F00%20-%20Development%20Environment%20and%20Structure/F00%20-%20W02%20-%20Monorepo%20Setup%20and%20Backend%20Scaffolding.md).

---

## How to pick up work (Cursor)

1. Open the repo root in Cursor — `.cursor/rules/*.mdc` load automatically (project, backend, frontend, work-items rules).
2. **Read this file** and the [Developer Guide](../developer-guide.md) (work-item → PR flow).
3. Pick the next work item (table above) under `docs/roadmap/{Feature}/`.
4. Drive it with the skills: `architect` (impact) → `task-breakdown` (concrete tasks) → `developer` (code) → `reviewer` (pre-PR). The AI proposes code; a human places it.
5. Branch `feature/fXX-wYY-desc`, implement, `dotnet build` / `dotnet test`, open PR (`Closes FXX-WYY`). See [GitHub Delivery](../deployment/github-delivery.md).
6. **Definition of Done (mandatory):** a work item **cannot be closed** until its documentation
   round-trip is done — update this file (progress log + "Next up") **and** sync any affected docs to
   the merged code. See [`DEFINITION-OF-DONE.md`](DEFINITION-OF-DONE.md). The `reviewer` skill and the
   PR template enforce it.

---

## Progress log

| Date | Release / WI | Change |
|------|--------------|--------|
| 2026-05-29 | — | Foundation complete: roadmap, 20 technical docs, ontology, deployment, onboarding, unified skills/rules; `mvp/docs/` integrated & removed. Ready to start R0.0 / F00-W02. |
| 2026-05-29 | F00-W02 | Monorepo hoist (`mvp/` → root); `LegalAiAr.Agents` + `LegalAiAr.AgentEvals`; SK 1.77.0; repoint docs/skills; `.github/ISSUE_TEMPLATE/`; AC: 0 errors (warnings → F00-W08). |
| 2026-05-29 | F00-W02 | **Merged to `main`** — [PR #102](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/102). WI closed; pick up F00-W03. |
| 2026-05-29 | roadmap | **Product re-scoped to PwC tax-legal** (branch `feature/roadmap-pwc-tax-pivot`): `features.md` + `backlog.md` rewritten; R1.0+ features changed (projects/workspaces, tax sources, document review, deliverables, internal KB, tax controversy). F00/R0.0 unchanged. Old `F01…F23`/`FT0x` work-item folders to be retired. |

---

*Project Status — Legal Ai Ar*
