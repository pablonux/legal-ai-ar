# Project Status — Where We Are / What's Next

> **Single source of truth for current state.** Read this first to know what is done and what to pick
> up next. Update it whenever a work item is completed or the next step changes. Committed to the repo
> so it is shared across environments (Cowork / Cursor).
>
> **Last updated:** 2026-05-29

---

## Current phase

**Planning & documentation foundation — ✅ complete. Code work not started yet (about to begin R0.0).**

The MVP code has **not** been migrated yet: it still lives under `mvp/` (`mvp/backend` .NET 10 Clean
Architecture, `mvp/frontend` Angular, `mvp/infra`, `mvp/deployment`, `docker-compose*.yml`). The plan
is to **evolve the MVP in place** (~78% reusable), not rewrite it.

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
| **1** | **F00 - W02 - Monorepo Setup and Backend Scaffolding** | ⏭ **next** |
| 2 | F00 - W03 - Angular 19 Frontend Scaffolding | pending |
| 3 | F00 - W04 - GitHub Actions CI Configuration | pending |
| 4 | F00 - W05 - Azure Infrastructure with Bicep | pending |
| 5 | F00 - W06 - CD Deployment Pipelines Configuration | pending |
| 6 | F00 - W07 - Local Environment Setup and Onboarding Guide | pending |
| 7 | F00 - W08 - Code Quality Configuration | pending |

> ⚠️ **Adapt vs create.** The F00 tickets were drafted greenfield (their diagrams assume code already
> at the repo root and "scaffold from scratch"). In reality the MVP already provides `backend/` and
> `frontend/` **under `mvp/`**, and `docs/` is already at the root. So several F00 tickets reduce to
> **"hoist + adapt"** rather than "create new" — call this out in the `architect` step. `features.md`
> and `backlog.md` now list this same W01–W08 set (reconciled). Note W04–W06 (CI/Bicep/CD) overlap
> with feature **FT05** (delivery & hosting) — consolidate when implemented.

After R0.0: **R1.0 Foundation** (F01–F07). See [`features.md`](features.md) for the full release map.

---

## ▶ First ticket walkthrough — F00-W02 (Monorepo Setup & Backend Scaffolding)

**Goal:** hoist the code out of `mvp/` to the repo root and add the two new .NET projects
(`LegalAiAr.Agents` for Semantic Kernel, `LegalAiAr.AgentEvals` for evals). After this, `mvp/` is gone.

**Starting reality:** code is under `mvp/backend`, `mvp/frontend`, `mvp/infra`, `mvp/deployment` (+ `mvp/docker-compose*.yml`); `docs/` is already at the root; `mvp/docs/` no longer exists.

### Step by step (what the dev does)

1. **Get oriented.** Read this file and `docs/roadmap/F00 - Development Environment and Structure/F00 - W02 - Monorepo Setup and Backend Scaffolding.md`, plus the [Developer Guide](../developer-guide.md).
2. **Branch:** `git checkout main && git pull && git checkout -b feature/f00-w02-monorepo-hoist`.
3. **Hoist the code to root** (preserve history with `git mv`):
   - `git mv mvp/backend backend`
   - `git mv mvp/frontend frontend`
   - `git mv mvp/infra infra`
   - `git mv mvp/deployment deployment`
   - `git mv mvp/docker-compose.yml .` and `git mv mvp/docker-compose.app.yml .`
   - move/keep `mvp/.env.example` and `mvp/.gitignore` as needed, then remove the empty `mvp/`.
4. **Repoint `mvp/` references** in the operational docs (now that paths changed): `onboarding/*`, `deployment/github-delivery.md`, `gcaas-hosting.md`, `developer-guide.md`, `cowork-setup-tutorial.md`, `features.md`, `FT05` work items → `mvp/backend` → `backend`, `mvp/frontend` → `frontend`, etc.
5. **Add `LegalAiAr.Agents`** (Class Library) in `backend/src/shared/` with `Plugins/ Prompts/ Orchestration/`; reference `Application` + `Core`; reference it from `Api`; install Semantic Kernel NuGet; add to `LegalAiAr.sln`.
6. **Add `LegalAiAr.AgentEvals`** in `backend/tests/` (golden-set/eval structure); add to `LegalAiAr.sln`.
7. **Verify:** `dotnet build` (all projects, 0 warnings/errors) and `dotnet test` pass; Clean Architecture references respected (`Core` references nothing).
8. **Close out:** update this STATUS (mark W02 done, set W03 next, add a log row), commit, and open a PR `Closes F00-W02`.

### Example — what to type to Cursor to drive it

Paste these in order in the Cursor chat (it auto-loads the rules and discovers the skills):

```
1) "Leé docs/roadmap/STATUS.md y el ticket F00-W02 (Monorepo Setup and Backend
    Scaffolding). Resumime en qué consiste y qué hay que hacer dado que el código
    todavía está bajo mvp/."

2) "Usá el skill architect: analizá el impacto de F00-W02. Tené en cuenta que el
    código real está en mvp/backend y mvp/frontend (no en la raíz como asume el
    ticket) y que docs/ ya está migrado. Decime el plan de hoist + los 2 proyectos
    nuevos (Agents, AgentEvals), riesgos y orden."

3) "Desglosá F00-W02 en tareas concretas (skill task-breakdown): comandos git mv del
    hoist, archivos de docs a repointar, creación de LegalAiAr.Agents y
    LegalAiAr.AgentEvals con sus referencias y NuGet, y los pasos de verificación.
    Actualizá la sección Tasks del work item."

4) "Implementemos la tarea 1: dame los comandos exactos para el hoist con git mv y
    qué referencias de mvp/ hay que actualizar en los docs operativos."

5) (por cada proyecto nuevo) "Implementemos LegalAiAr.Agents: decime archivos, rutas,
    .csproj con sus ProjectReference y los paquetes NuGet de Semantic Kernel, para que
    yo los cree."

6) "Revisá lo que implementé para F00-W02 (skill reviewer): build sin warnings, tests,
    y que Core no referencie a nadie."
```

> Remember the project rule: the AI **proposes** files/paths/code; the human places them. After the PR
> merges, update this STATUS file.

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

---

*Project Status — Legal Ai Ar*
