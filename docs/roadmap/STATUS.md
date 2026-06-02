# Project Status — Where We Are / What's Next

> **Single source of truth for current state.** Read this first to know what is done and what to pick
> up next. Update it whenever a work item is completed or the next step changes. Committed to the repo
> so it is shared across environments (Cowork / Cursor).
>
> **Last updated:** 2026-06-02 (F00 application track complete — W11 PR #113, W12 PR #114 merged to `main`)

---

## Current phase

**R0.0 (Preparation) — F00 application track complete.** Code at repo root (`backend/`, `frontend/`,
`infra/`, `deployment/`, `docker-compose.app.yml`). All **F00 W01–W03, W07–W12** merged to `main`
(latest: [PR #113](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/113) schematics,
[PR #114](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/114) transactional outbox).
**CI/Bicep/CD (former F00-W04–W06)** → **[FT05 — Delivery and Hosting](FT05%20-%20Delivery%20and%20Hosting/README.md)** — **on hold** pending DevOps consultation.
**Next:** **R1.0 — Research & Monitoring** — generate detailed work items per feature when scheduled; see [`features.md`](features.md) and [`PIVOT-PWC-TAX.md`](PIVOT-PWC-TAX.md).

---

## Done so far (foundation)

- **Roadmap**: [`features.md`](features.md) (PwC tax-legal releases, features, endpoints, KPIs) + [`backlog.md`](backlog.md) (F00 detailed tickets + feature-level catalog for R1–R4/FT). Detailed work items are generated per feature when it is scheduled (skill `work-item-generator`); only the **F00** folder holds tickets today.
- **Technical docs** (`docs/technical/`, 23 indexed): conceptual 01–09, imported-pending-review 10–12, **authored-from-code** 13–20, **22** API endpoints & Contracts, **23** transactional outbox.
- **Architecture standard** ([`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md)): single reference guide for any internal PwC app — auth, backend/frontend structure, API patterns, testing, delivery. All F00+ work items must align with it (see [Definition of Done](DEFINITION-OF-DONE.md)).
- **Ontology** (`docs/ontology/`), **delivery/hosting** (`docs/deployment/`), **onboarding** (`docs/onboarding/`), **AppKit 4** UI reference (`docs/appkit4/`).
- **AI config unified**: `CLAUDE.md` + `.claude/skills/` (Cowork) and `.cursor/rules/` + `.cursor/skills/` (Cursor) — same 13 skills, English-first language rule.
- **MVP docs fully integrated and `mvp/docs/` deleted.** Data sources (incl. MJN magistrados/designaciones) documented in [`docs/technical/20-legal-data-sources.md`](../technical/20-legal-data-sources.md).

---

## F00 — Release 0.0 application track (complete)

All tickets in [`F00 - Development Environment and Structure/`](F00%20-%20Development%20Environment%20and%20Structure/) are **done**:

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

> **R0.0 (F00) = 41 SP — complete** on the application track. **W04–W06** (18 SP) live under **[FT05](FT05%20-%20Delivery%20and%20Hosting/README.md)** (on hold). Foundation aligns with [`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md) §4–§6, §9.

---

## Next up — Release 1.0 (Research & Monitoring)

⚠️ Product **re-scoped to PwC tax-legal** — R1.0+ features are in [`features.md`](features.md); F00 is unchanged.
Generate detailed work items per feature when scheduled (`work-item-generator` skill). Retired litigation folders (`F01…F23`) are not the active backlog.

---

## Recently completed — F00-W09

**Merged** [PR #111](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/111) to `main`.
`IEndpoint` discovery (`AddLegalAiArEndpoints` / `MapLegalAiArEndpoints`), validation filter, `ErrorOr`
ProblemDetails helpers, NetArchTest endpoint rules; **all** former MVC controllers migrated (auth,
catalogs, chat SSE, admin, search, ontology, stats, graph, proceedings). `Controllers/` removed;
`Program.cs` no longer calls `MapControllers`. Docs:
[`22-api-endpoints.md`](../technical/22-api-endpoints.md), [`10-system-architecture.md`](../technical/10-system-architecture.md) §5.
Work item:
[`F00 - W09`](F00%20-%20Development%20Environment%20and%20Structure/F00%20-%20W09%20-%20Minimal%20API%20Endpoint%20Discovery%20and%20Migration.md).

---

## Recently completed — F00-W08

**Code quality** — `TreatWarningsAsErrors`, NetAnalyzers + Roslynator, `.globalconfig`,
`LegalAiAr.ArchitectureTests` (NetArchTest, 6 layer rules), root husky/lint-staged/commitlint,
`.github/CODEOWNERS`, VS Code format-on-save settings; CI: `dotnet format --verify-no-changes` +
full `npm run lint`. Legacy MVP features under relaxed ESLint overrides. Work item:
[`F00 - W08`](F00%20-%20Development%20Environment%20and%20Structure/F00%20-%20W08%20-%20Code%20Quality%20Configuration.md).

---

## Recently completed — F00-W07

**Local onboarding** — `infra/scripts/setup-local.ps1` and `setup-local.sh` (prerequisite checks,
`.env` bootstrap, backend build, `npm ci`); onboarding hub, troubleshooting, and root README quick
start updated for **cloud DEV** workflow (no local SQL/Azurite). Node **22+**. Extensions already in
`.vscode/extensions.json`. Work item:
[`F00 - W07`](F00%20-%20Development%20Environment%20and%20Structure/F00%20-%20W07%20-%20Local%20Environment%20Setup%20and%20Onboarding%20Guide.md).

---

## On hold — FT05 Delivery and Hosting (DevOps track)

**Moved from F00** (2026-06-02): former **F00-W04 / W05 / W06** → **FT05-W01 / W02 / W03** in
[`FT05 - Delivery and Hosting/`](FT05%20-%20Delivery%20and%20Hosting/README.md). **On hold** until the
team consults **DevOps** on CI platform, Azure subscription/IaC, and alignment with GCaaS production path.

| WI                                                                                                                                |  SP | Status                                                                           |
| --------------------------------------------------------------------------------------------------------------------------------- | --: | -------------------------------------------------------------------------------- |
| [FT05-W01](FT05%20-%20Delivery%20and%20Hosting/FT05%20-%20W01%20-%20GitHub%20Actions%20CI%20Configuration.md) CI (GitHub Actions) |   5 | 🟡 partial — workflow YAML on `main`; **blocked** (Actions disabled at org/repo) |
| [FT05-W02](FT05%20-%20Delivery%20and%20Hosting/FT05%20-%20W02%20-%20Azure%20Infrastructure%20with%20Bicep.md) Azure Bicep         |   8 | pending                                                                          |
| [FT05-W03](FT05%20-%20Delivery%20and%20Hosting/FT05%20-%20W03%20-%20CD%20Deployment%20Pipelines%20Configuration.md) CD pipelines  |   5 | pending — do not start until CI path confirmed                                   |

**FT05 = 18 SP.** Resume when DevOps confirms: enable GitHub Actions vs alternate CI; Azure landing zone; CD scope (GitHub→Azure staging vs GCaaS). See FT05-W01 § _Resume checklist_ and [`github-delivery.md`](../deployment/github-delivery.md).

---

## Recently completed — F00-W03

**Merged** [PR #105](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/105) to `main`.
Angular **19** upgrade; workspace libraries **`core`**, **`shell`**, **`ui`**, **`shared-common`**;
platform auth in `core`; shell chrome in `shell`; environments + build configs (`local`, `dev`, `qa`,
`development`, `staging`, `production`); `proxy.conf.json`; Jest + Playwright smoke + Prettier; npm
scripts per WI. `ng build --configuration=production` passes. AppKit 4 install deferred (Material
placeholder). See [`18-frontend-architecture.md`](../technical/18-frontend-architecture.md) and
[`F00 - W03`](F00%20-%20Development%20Environment%20and%20Structure/F00%20-%20W03%20-%20Angular%2019%20Frontend%20Scaffolding.md).

---

## Recently completed — F00-W02

Monorepo hoist (`mvp/` removed), `LegalAiAr.Agents`, `LegalAiAr.AgentEvals`, docs/skills repoint,
`.github/ISSUE_TEMPLATE/`. Build: 0 errors on `main`; analyzer warnings deferred to **F00-W08**.
Work item: [`F00 - W02 - Monorepo Setup and Backend Scaffolding.md`](F00%20-%20Development%20Environment%20and%20Structure/F00%20-%20W02%20-%20Monorepo%20Setup%20and%20Backend%20Scaffolding.md).

---

## How to pick up work (Cursor)

1. Open the repo root in Cursor — `.cursor/rules/*.mdc` load automatically (project, backend, frontend, work-items rules).
2. **Read this file** and the [Developer Guide](../onboarding/developer-guide.md) (work-item → PR flow).
3. Pick the next feature/work item from [`features.md`](features.md) and `docs/roadmap/{Feature}/` (F00 is complete).
4. Drive it with the skills: `architect` (impact) → `task-breakdown` (concrete tasks) → `developer` (code) → `reviewer` (pre-PR). The AI proposes code; a human places it.
5. Branch `feature/fXX-wYY-desc`, implement, `dotnet build` / `dotnet test`, open PR (`Closes FXX-WYY`). See [GitHub Delivery](../deployment/github-delivery.md).
6. **Definition of Done (mandatory):** a work item **cannot be closed** until its documentation
   round-trip is done — update this file (progress log + "Next up") **and** sync any affected docs to
   the merged code. See [`DEFINITION-OF-DONE.md`](DEFINITION-OF-DONE.md). The `reviewer` skill and the
   PR template enforce it.

---

## Progress log

| Date       | Release / WI | Change                                                                                                                                                                                                                                                                                                                                                                                      |
| ---------- | ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 2026-05-29 | —            | Foundation complete: roadmap, 20 technical docs, ontology, deployment, onboarding, unified skills/rules; `mvp/docs/` integrated & removed. Ready to start R0.0 / F00-W02.                                                                                                                                                                                                                   |
| 2026-05-29 | F00-W02      | Monorepo hoist (`mvp/` → root); `LegalAiAr.Agents` + `LegalAiAr.AgentEvals`; SK 1.77.0; repoint docs/skills; `.github/ISSUE_TEMPLATE/`; AC: 0 errors (warnings → F00-W08).                                                                                                                                                                                                                  |
| 2026-05-29 | F00-W02      | **Merged to `main`** — [PR #102](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/102). WI closed; pick up F00-W03.                                                                                                                                                                                                                                                       |
| 2026-05-29 | roadmap      | **Product re-scoped to PwC tax-legal** (branch `feature/roadmap-pwc-tax-pivot`): `features.md` + `backlog.md` rewritten; R1.0+ features changed (projects/workspaces, tax sources, document review, deliverables, internal KB, tax controversy). F00/R0.0 unchanged. Old `F01…F23`/`FT0x` work-item folders to be retired.                                                                  |
| 2026-06-01 | standards    | Published [`docs/standards/pwc-internal-app-architecture.md`](../standards/pwc-internal-app-architecture.md) — single PwC internal app reference (auth, layers, API, frontend, testing, delivery). Updated DoD, PR template, technical README, F00-W03/W08 alignment.                                                                                                                       |
| 2026-06-01 | F00          | Added **W09–W12** (21 SP) to close architecture-standard gaps: Minimal API, Contracts, schematics, outbox. R0.0 now **59 SP** total.                                                                                                                                                                                                                                                        |
| 2026-06-02 | F00-W03      | Angular 19 + workspace libs (`core`, `shell`, `ui`, `shared-common`); proxy/Jest/Playwright/Prettier; `18-frontend-architecture.md` updated. Pick up **F00-W04**.                                                                                                                                                                                                                           |
| 2026-06-01 | F00-W03      | **Merged to `main`** — [PR #105](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/105). WI closed (DoD docs); pick up **F00-W04**.                                                                                                                                                                                                                                        |
| 2026-06-01 | F00-W04      | CI workflow files merged to `main` (`ci-backend.yml`, `ci-frontend.yml`). **Blocked:** GitHub Actions disabled at org/repo — branch protection + AC verification pending. Resume W04 when Actions enabled or CI path decided; parallel: **W07/W08/W09**.                                                                                                                                    |
| 2026-06-01 | F00-W07      | Setup scripts (`setup-local.ps1`/`.sh`); onboarding + troubleshooting + README quick start synced to cloud DEV model. WI closed. **Next:** W08 or W09 (W04 resume when CI unblocked).                                                                                                                                                                                                       |
| 2026-06-01 | F00-W08      | Analyzers, NetArchTest, husky/commitlint, CI quality gates; backend 0 warnings. WI closed. **Next:** W09 (W04 resume when CI unblocked).                                                                                                                                                                                                                                                    |
| 2026-06-02 | F00-W09      | Full Minimal API migration: infra + all endpoint groups; MVC removed. **Merged to `main`** — [PR #111](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/111). WI closed (DoD docs); pick up **F00-W10**.                                                                                                                                                                  |
| 2026-06-02 | FT05 / F00   | **F00-W04–W06 moved to FT05** (Delivery and Hosting). DevOps track **on hold** pending platform consultation. F00 **next: W10** (41 SP app track: 28 done · 13 pending).                                                                                                                                                                                                                    |
| 2026-06-02 | F00-W10      | **`LegalAiAr.Contracts`** added; wave 1–2 DTOs migrated; docs [`22-api-endpoints.md`](../technical/22-api-endpoints.md).                                                                                                                                                                                                                                                                    |
| 2026-06-02 | F00-W10      | **Merged to `main`** — [PR #112](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/112). WI closed (DoD docs); pick up **F00-W11**.                                                                                                                                                                                                                                        |
| 2026-06-02 | F00-W11      | **`schema-la`** schematics; `frontend/docs/schematics.md`; demo `health-check` at `/salud`. **Merged to `main`** — [PR #113](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/113). WI closed; pick up **F00-W12**.                                                                                                                                                       |
| 2026-06-02 | F00-W12      | Transactional outbox: `IDomainEvent`, `IAggregateRoot`, `OutboxMessages`, EF interceptor, `OutboxDispatcherWorker` in API, handler scan in Application. Docs [`23-outbox-domain-events.md`](../technical/23-outbox-domain-events.md). **Merged to `main`** — [PR #114](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/114). **F00 app track complete** — next **R1.0**. |

---

## Recently completed — F00-W12

**Merged** [PR #114](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/114) to `main`.
Transactional outbox per architecture §4.5: `OutboxMessages` + migration `AddOutboxMessages`,
`DispatchDomainEventsInterceptor`, `DomainEventsDispatcher`, `OutboxMessageProcessor`,
`OutboxDispatcherWorker` (`BackgroundService` in API), `AddDomainEventHandlersFromAssembly` in Application.
Ingestion queues unchanged. Docs:
[`23-outbox-domain-events.md`](../technical/23-outbox-domain-events.md). Work item:
[`F00 - W12`](F00%20-%20Development%20Environment%20and%20Structure/F00%20-%20W12%20-%20Transactional%20Outbox%20and%20Domain%20Events%20Infrastructure.md).

---

## Recently completed — F00-W11

**Merged** [PR #113](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/113) to `main`.
Collection **`schema-la`** in
`frontend/schema-la/` — four generators aligned with architecture §6.2; `npm run link-schemas` compiles
rules; `angular.json` registers the collection. Placeholder **`health-check`** thin feature lazy-loads at
`/salud`. Docs: [`frontend/docs/schematics.md`](../frontend/docs/schematics.md),
[`18-frontend-architecture.md`](../technical/18-frontend-architecture.md) §9. Work item:
[`F00 - W11`](<F00%20-%20Development%20Environment%20and%20Structure/F00%20-%20W11%20-%20Angular%20Feature%20Schematics%20(schema-la).md>).

---

## Recently completed — F00-W10

**Merged** [PR #112](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/112) to `main`.
**`LegalAiAr.Contracts`** in `backend/src/shared/` — public HTTP request/response types with no LegalAiAr
project references. Wave 1–2 endpoints (`auth`, `rulings`, `statutes`, `courts`, `persons`, `thesaurus`,
`search`) use Contract types at the boundary; `SearchFacetsMapper`, `SearchRulingsRequestValidator`;
`ContractsArchitectureTests`. Statute enums serialized as `string` in Contracts. Docs:
[`22-api-endpoints.md`](../technical/22-api-endpoints.md). Work item:
[`F00 - W10`](F00%20-%20Development%20Environment%20and%20Structure/F00%20-%20W10%20-%20LegalAiAr.Contracts%20API%20DTO%20Layer.md).

---

_Project Status — Legal Ai Ar_
