# Project Memory — Legal Ai Ar

## Decisions made

- **2025-05**: Decided to use the existing `legal-ai-ar` monorepo instead of creating a new repo. The MVP initially lived under `mvp/`; **F0.0-W02** hoisted it to the repo root (`backend/`, `frontend/`, etc.).
- **2025-05**: Worker control via SignalR was removed from scope. SignalR is reserved for notifications, chat, and alerts.
- **2025-05**: R0.0 was renamed from "Migration" to "Preparation" to reflect that there is no repo migration.
- **2025-05**: All documentation (roadmap, technical, ontology) was organized under `docs/` at the monorepo root.
- **2025-05**: Project names use `LegalAiAr.*` (not LegalKB). Root namespace: `LegalAiAr`.
- **2026-05**: **Product re-scoped to PwC tax-legal** (advisory-first, organized by client projects/workspaces). Releases: R0.0 Preparation · R1.0 Research & Monitoring · R2.0 Professional Productivity · R3.0 Knowledge & Intelligence · R4.0 Scale & Operations. The old litigation work-item folders (`F01…F23`, `FT0x`) were retired. See `docs/roadmap/PIVOT-PWC-TAX.md` and `features.md`. F0.0/R0.0 unchanged.

## Conventions learned

- Roadmap: F0.0 (Preparation) has detailed tickets; R1–R4/FT features are catalogued in `features.md`/`backlog.md`. Detailed work items are generated per feature when scheduled (skill `work-item-generator`).
- Work items: `F{XX} - W{YY} - {Title}.md` inside each feature folder
- Azure resources: `{service}-legal-ai-ar-{environment}`
- Database: `LegalAiAr` (not LegalKB, not legal_ai_ar)
- The MVP has 21 projects in the .NET solution
- **Language rule (strict)**: Everything in English (code, file names, classes, tables, indexes, storage, URLs, endpoints, DTOs, domain entities, commits, docs, work items, comments). Spanish only for end-user facing content (UI labels, error messages, tooltips). The two exceptions kept in Spanish are the agents' YAML prompt content and the mockup labels. Dev chat can be in either language.

## Current status

- R0.0 (Preparation) in progress — **F0.0-W02 merged** (monorepo hoisted; `LegalAiAr.Agents` + `LegalAiAr.AgentEvals` created). Next: F0.0-W03. See `docs/roadmap/STATUS.md` for live progress.
- Complete documentation: roadmap (PwC tax-legal), 21 technical docs, ontology, deployment, onboarding
- Functional MVP (KB) with .NET 10 backend + Angular frontend at the repo root

## Things to avoid

- Never use "LegalKB" — always "LegalAiAr" or "Legal Ai Ar"
- Do not create separate repos — everything goes in `legal-ai-ar`
- Do not include worker control via SignalR in scope
- Do not create work items without checking the existing numbering
