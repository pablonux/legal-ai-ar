# Project Status — Where We Are / What's Next

> **Single source of truth for current state.** Read this first to know what is done and what to pick
> up next. Update it whenever a work item is completed or the next step changes. Committed to the repo
> so it is shared across environments (Cowork / Cursor).
>
> **Last updated:** 2026-06-02 (R1.0 work items generated — 26 WIs across 8 features; next: F1.1-W02)

---

## Current phase

**R1.0 (Research & Monitoring) — starting.** R0.0 / F0.0 application track is complete (all merged
to `main`). All 26 R1.0 work items generated in `docs/roadmap/F1.x - .../` (see [`backlog.md`](backlog.md)).
**FT.5 (CI/CD)** on hold pending DevOps consultation.
**Next:** branch `feature/f1.1-w02-platform-auth` → implement **[F1.1-W02 — Platform Auth Middleware](F1.1%20-%20Authentication%20and%20SSO/F1.1%20-%20W02%20-%20Platform%20Auth%20Middleware%20and%20JWT%20Validation.md)**.

---

## Done so far (foundation)

- **Roadmap**: [`features.md`](features.md) (PwC tax-legal releases, features, endpoints, KPIs) + [`backlog.md`](backlog.md) (F0.0 detailed tickets + feature-level catalog for R1–R4/FT). **R1.0 work items fully generated** (2026-06-02) — 26 WIs, 116 SP, 8 features in `docs/roadmap/F1.x - .../`.
- **Technical docs** (`docs/technical/`, 23 indexed): conceptual 01–09, imported-pending-review 10–12, **authored-from-code** 13–20, **22** API endpoints & Contracts, **23** transactional outbox.
- **Architecture standard** ([`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md)): single reference guide for any internal PwC app — auth, backend/frontend structure, API patterns, testing, delivery. All F0.0+ work items must align with it (see [Definition of Done](DEFINITION-OF-DONE.md)).
- **Ontology** (`docs/ontology/`), **delivery/hosting** (`docs/deployment/`), **onboarding** (`docs/onboarding/`), **AppKit 4** UI reference (`docs/appkit4/`).
- **AI config unified**: `CLAUDE.md` + `.claude/skills/` (Cowork) and `.cursor/rules/` + `.cursor/skills/` (Cursor) — same 13 skills, English-first language rule.
- **MVP docs fully integrated and `mvp/docs/` deleted.** Data sources (incl. MJN magistrados/designaciones) documented in [`docs/technical/20-legal-data-sources.md`](../technical/20-legal-data-sources.md).

---

## F0.0 — Release 0.0 application track (complete)

All tickets in [`F0.0 - Development Environment and Structure/`](F0.0%20-%20Development%20Environment%20and%20Structure/) are **done**:

| Ticket                                           |  SP | PR                                                                             |
| ------------------------------------------------ | --: | ------------------------------------------------------------------------------ |
| W01 Comprehensive Documentation                  |   5 | —                                                                              |
| W02 Monorepo Setup and Backend Scaffolding       |   5 | [#102](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/102) |
| W03 Angular 19 Frontend Scaffolding              |   5 | [#105](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/105) |
| W07 Local Environment Setup and Onboarding       |   2 | —                                                                              |
| W08 Code Quality Configuration                   |   3 | [#110](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/110) |
| W09 Minimal API Endpoint Discovery and Migration |   8 | [#111](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/111) |
| W10 LegalAiAr.Contracts API DTO Layer            |   5 | [#112](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/112) |
| W11 Angular Feature Schematics (schema-la)       |   3 | [#113](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/113) |
| W12 Transactional Outbox and Domain Events       |   5 | [#114](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/114) |

> **R0.0 (F0.0) = 41 SP — complete** on the application track. **W04–W06** (18 SP) live under **[FT.5](FT.5%20-%20Delivery%20and%20Hosting/README.md)** (on hold). Foundation aligns with [`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md) §4–§6, §9.

---

## Next up — Release 1.0 (Research & Monitoring)

⚠️ Product **re-scoped to PwC tax-legal** — R1.0+ features are in [`features.md`](features.md); F0.0 is unchanged.
All 26 R1.0 work items generated (2026-06-02). Pick up in this order (see [`backlog.md`](backlog.md) §Recommended execution order):

1. **F1.1-W02** — Platform Auth Middleware (branch `feature/f1.1-w02-platform-auth`)
2. **F1.1-W03** — Frontend Auth Service
3. **F1.2-W02 / F1.3-W02** — Home API + Tax Sources ingestion (can start in parallel after auth)

---

## On hold — FT.5 Delivery and Hosting (DevOps track)

**Moved from F0.0** (2026-06-02): former **F0.0-W04 / W05 / W06** → **FT.5-W01 / W02 / W03** in
[`FT.5 - Delivery and Hosting/`](FT.5%20-%20Delivery%20and%20Hosting/README.md). **On hold** until the
team consults **DevOps** on CI platform, Azure subscription/IaC, and alignment with GCaaS production path.

| WI                                                                                                                                |  SP | Status                                                                           |
| --------------------------------------------------------------------------------------------------------------------------------- | --: | -------------------------------------------------------------------------------- |
| [FT.5-W01](FT.5%20-%20Delivery%20and%20Hosting/FT.5%20-%20W01%20-%20GitHub%20Actions%20CI%20Configuration.md) CI (GitHub Actions) |   5 | 🟡 partial — workflow YAML on `main`; **blocked** (Actions disabled at org/repo) |
| [FT.5-W02](FT.5%20-%20Delivery%20and%20Hosting/FT.5%20-%20W02%20-%20Azure%20Infrastructure%20with%20Bicep.md) Azure Bicep         |   8 | pending                                                                          |
| [FT.5-W03](FT.5%20-%20Delivery%20and%20Hosting/FT.5%20-%20W03%20-%20CD%20Deployment%20Pipelines%20Configuration.md) CD pipelines  |   5 | pending — do not start until CI path confirmed                                   |

**FT.5 = 18 SP.** Resume when DevOps confirms: enable GitHub Actions vs alternate CI; Azure landing zone; CD scope (GitHub→Azure staging vs GCaaS). See FT.5-W01 § _Resume checklist_ and [`github-delivery.md`](../deployment/github-delivery.md).

---

## How to pick up work (Cursor)

1. Open the repo root in Cursor — `.cursor/rules/*.mdc` load automatically (project, backend, frontend, work-items rules).
2. **Read this file** and the [Developer Guide](../onboarding/developer-guide.md) (work-item → PR flow).
3. Pick the next feature/work item from the order above and its ticket in `docs/roadmap/{Feature}/`.
4. Drive it with the skills: `architect` (impact) → `task-breakdown` (concrete tasks) → `developer` (code) → `reviewer` (pre-PR). The AI proposes code; a human places it.
5. Branch `feature/fX.Y-wZZ-desc`, implement, `dotnet build` / `dotnet test`, open PR (`Closes FX.Y-WZZ`). See [GitHub Delivery](../deployment/github-delivery.md).
6. **Definition of Done (mandatory):** a work item **cannot be closed** until its documentation
   round-trip is done — update this file (progress log + "Next up") **and** sync any affected docs to
   the merged code. See [`DEFINITION-OF-DONE.md`](DEFINITION-OF-DONE.md). The `reviewer` skill and the
   PR template enforce it.

---

## Progress log

| Date       | Release / WI    | Change                                                                                                                                                                                                                                                                                                                                                                                      |
| ---------- | --------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 2026-05-29 | —               | Foundation complete: roadmap, 20 technical docs, ontology, deployment, onboarding, unified skills/rules; `mvp/docs/` integrated & removed. Ready to start R0.0 / F0.0-W02.                                                                                                                                                                                                                  |
| 2026-05-29 | F0.0-W02        | Monorepo hoist (`mvp/` → root); `LegalAiAr.Agents` + `LegalAiAr.AgentEvals`; SK 1.77.0; repoint docs/skills; `.github/ISSUE_TEMPLATE/`; AC: 0 errors (warnings → F0.0-W08).                                                                                                                                                                                                                |
| 2026-05-29 | F0.0-W02        | **Merged to `main`** — [PR #102](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/102). WI closed; pick up F0.0-W03.                                                                                                                                                                                                                                                      |
| 2026-05-29 | roadmap         | **Product re-scoped to PwC tax-legal** (branch `feature/roadmap-pwc-tax-pivot`): `features.md` + `backlog.md` rewritten; R1.0+ features changed (projects/workspaces, tax sources, document review, deliverables, internal KB, tax controversy). F0.0/R0.0 unchanged. Old `F01…F23`/`FT0x` work-item folders to be retired.                                                                 |
| 2026-06-01 | standards       | Published [`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md) — single PwC internal app reference (auth, layers, API, frontend, testing, delivery). Updated DoD, PR template, technical README, F0.0-W03/W08 alignment.                                                                                                                      |
| 2026-06-01 | F0.0            | Added **W09–W12** (21 SP) to close architecture-standard gaps: Minimal API, Contracts, schematics, outbox. R0.0 now **59 SP** total.                                                                                                                                                                                                                                                        |
| 2026-06-02 | F0.0-W03        | Angular 19 + workspace libs (`core`, `shell`, `ui`, `shared-common`); proxy/Jest/Playwright/Prettier; `18-frontend-architecture.md` updated. Pick up **F0.0-W04**.                                                                                                                                                                                                                          |
| 2026-06-01 | F0.0-W03        | **Merged to `main`** — [PR #105](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/105). WI closed (DoD docs); pick up **F0.0-W04**.                                                                                                                                                                                                                                       |
| 2026-06-01 | F0.0-W04        | CI workflow files merged to `main` (`ci-backend.yml`, `ci-frontend.yml`). **Blocked:** GitHub Actions disabled at org/repo — branch protection + AC verification pending. Resume W04 when Actions enabled or CI path decided; parallel: **W07/W08/W09**.                                                                                                                                    |
| 2026-06-01 | F0.0-W07        | Setup scripts (`setup-local.ps1`/`.sh`); onboarding + troubleshooting + README quick start synced to cloud DEV model. WI closed. **Next:** W08 or W09 (W04 resume when CI unblocked).                                                                                                                                                                                                      |
| 2026-06-01 | F0.0-W08        | Analyzers, NetArchTest, husky/commitlint, CI quality gates; backend 0 warnings. WI closed. **Next:** W09 (W04 resume when CI unblocked).                                                                                                                                                                                                                                                    |
| 2026-06-02 | F0.0-W09        | Full Minimal API migration: infra + all endpoint groups; MVC removed. **Merged to `main`** — [PR #111](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/111). WI closed (DoD docs); pick up **F0.0-W10**.                                                                                                                                                                  |
| 2026-06-02 | FT.5 / F0.0     | **F0.0-W04–W06 moved to FT.5** (Delivery and Hosting). DevOps track **on hold** pending platform consultation. F0.0 **next: W10** (41 SP app track: 28 done · 13 pending).                                                                                                                                                                                                                  |
| 2026-06-02 | F0.0-W10        | **`LegalAiAr.Contracts`** added; wave 1–2 DTOs migrated; docs [`22-api-endpoints.md`](../technical/22-api-endpoints.md).                                                                                                                                                                                                                                                                    |
| 2026-06-02 | F0.0-W10        | **Merged to `main`** — [PR #112](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/112). WI closed (DoD docs); pick up **F0.0-W11**.                                                                                                                                                                                                                                       |
| 2026-06-02 | F0.0-W11        | **`schema-la`** schematics; `frontend/docs/schematics.md`; demo `health-check` at `/salud`. **Merged to `main`** — [PR #113](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/113). WI closed; pick up **F0.0-W12**.                                                                                                                                                      |
| 2026-06-02 | F0.0-W12        | Transactional outbox: `IDomainEvent`, `IAggregateRoot`, `OutboxMessages`, EF interceptor, `OutboxDispatcherWorker` in API, handler scan in Application. Docs [`23-outbox-domain-events.md`](../technical/23-outbox-domain-events.md). **Merged to `main`** — [PR #114](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/114). **F0.0 app track complete** — next **R1.0**. |
| 2026-06-02 | R1.0 roadmap    | **R1.0 work items generated** — 26 WIs, 116 SP across 8 features (F1.1–F1.8) in `docs/roadmap/F1.x - .../`. Feature IDs renumbered to match execution order. **Next: F1.1-W02** (platform auth middleware).                                                                                                                                                                                 |

---

## Recently completed — F0.0-W12

**Merged** [PR #114](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/114) to `main`.
Transactional outbox per architecture §4.5: `OutboxMessages` + migration `AddOutboxMessages`,
`DispatchDomainEventsInterceptor`, `DomainEventsDispatcher`, `OutboxMessageProcessor`,
`OutboxDispatcherWorker` (`BackgroundService` in API), `AddDomainEventHandlersFromAssembly` in Application.
Ingestion queues unchanged. Docs:
[`23-outbox-domain-events.md`](../technical/23-outbox-domain-events.md). Work item:
[`F0.0 - W12`](F0.0%20-%20Development%20Environment%20and%20Structure/F0.0%20-%20W12%20-%20Transactional%20Outbox%20and%20Domain%20Events%20Infrastructure.md).

---

## Recently completed — F0.0-W11

**Merged** [PR #113](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/113) to `main`.
Collection **`schema-la`** in
`frontend/schema-la/` — four generators aligned with architecture §6.2; `npm run link-schemas` compiles
rules; `angular.json` registers the collection. Placeholder **`health-check`** thin feature lazy-loads at
`/salud`. Docs: [`frontend/docs/schematics.md`](../frontend/docs/schematics.md),
[`18-frontend-architecture.md`](../technical/18-frontend-architecture.md) §9. Work item:
[`F0.0 - W11`](<F0.0%20-%20Development%20Environment%20and%20Structure/F0.0%20-%20W11%20-%20Angular%20Feature%20Schematics%20(schema-la).md>).

---

## Recently completed — F0.0-W10

**Merged** [PR #112](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/112) to `main`.
**`LegalAiAr.Contracts`** in `backend/src/shared/` — public HTTP request/response types with no LegalAiAr
project references. Wave 1–2 endpoints (`auth`, `rulings`, `statutes`, `courts`, `persons`, `thesaurus`,
`search`) use Contract types at the boundary; `SearchFacetsMapper`, `SearchRulingsRequestValidator`;
`ContractsArchitectureTests`. Statute enums serialized as `string` in Contracts. Docs:
[`22-api-endpoints.md`](../technical/22-api-endpoints.md). Work item:
[`F0.0 - W10`](F0.0%20-%20Development%20Environment%20and%20Structure/F0.0%20-%20W10%20-%20LegalAiAr.Contracts%20API%20DTO%20Layer.md).

---

_Project Status — Legal Ai Ar_
