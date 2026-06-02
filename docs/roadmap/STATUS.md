# Project Status — Where We Are / What's Next

> **Single source of truth for current state.** Read this first to know what is done and what to pick
> up next. Update it whenever a work item is completed or the next step changes. Committed to the repo
> so it is shared across environments (Cowork / Cursor).
>
> **Last updated:** 2026-06-02 (F0.0 application track complete — W11 PR #113, W12 PR #114 merged to `main`)

---

## Current phase

**R0.0 (Preparation) — F0.0 application track complete.** Code at repo root (`backend/`, `frontend/`,
`infra/`, `deployment/`, `docker-compose.app.yml`). All **F0.0 W01–W03, W07–W12** merged to `main`
(latest: [PR #113](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/113) schematics,
[PR #114](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/114) transactional outbox).
**CI/Bicep/CD (former F0.0-W04–W06)** → **[FT.5 — Delivery and Hosting](FT.5%20-%20Delivery%20and%20Hosting/README.md)** — **on hold** pending DevOps consultation.
**Next:** **R1.0 — Research & Monitoring** — generate detailed work items per feature when scheduled; see [`features.md`](features.md) and [`PIVOT-PWC-TAX.md`](PIVOT-PWC-TAX.md).

---

## Done so far (foundation)

- **Roadmap**: [`features.md`](features.md) (PwC tax-legal releases, features, endpoints, KPIs) + [`backlog.md`](backlog.md) (F0.0 detailed tickets + feature-level catalog for R1–R4/FT). Detailed work items are generated per feature when it is scheduled (skill `work-item-generator`); only the **F0.0** folder holds tickets today.
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
Generate detailed work items per feature when scheduled (`work-item-generator` skill). Retired litigation folders (`F01…F23`) are not the active backlog.

---

## Recently completed — F0.0-W09

**Merged** [PR #111](https://github.com/pwc-ar-xlos-argentinaaifactory/legal-ai-ar/pull/111) to `main`.
`IEndpoint` discovery (`AddLegalAiArEndpoints` / `MapLegalAiArEndpoints`), validation filter, `ErrorOr`
ProblemDetails helpers, NetArchTest endpoint rules; **all** former MVC controllers migrated (auth,
catalogs, chat SSE, admin, search, ontology, stats, graph, proceedings). `Controllers/` removed;
`Program.cs` no longer calls `MapControllers`. Docs:
[`22-api-endpoints.md`](../technical/22-api-endpoints.md), [`10-system-architecture.md`](../technical/10-system-architecture.md) §5.
Work item:
[`F0.0 - W09`](F0.0%20-%20Development%20Environment%20and%20Structure/F0.0%20-%20W09%20-%20Minimal%20API%20Endpoint%20Discovery%20and%20Migration.md).

---

## Recently completed — F0.0-W08

**Code quality** — `TreatWarningsAsErrors`, NetAnalyzers + Roslynator, `.globalconfig`,
`LegalAiAr.ArchitectureTests` (NetArchTest, 6 layer rules), root husky/lint-staged/commitlint,
`.github/CODEOWNERS`, VS Code format-on-save settings; CI: `dotne