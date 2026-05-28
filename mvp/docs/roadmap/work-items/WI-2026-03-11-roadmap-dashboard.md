# Work Item — Dynamic Roadmap & Project Progress Dashboard

**Created**: 2026-03-11  
**Status**: Draft  
**Feature**: [Roadmap Dashboard](./FEATURE-roadmap-dashboard.md)

## User Story

As a **project stakeholder or team member**, I want **a dynamic dashboard that displays roadmap visibility and project advance** so that **I can see at a glance how the project is progressing, which phases and features are complete, and the dashboard updates automatically whenever the roadmap file changes**.

## Context

- **Source of truth**: `docs/roadmap/ROADMAP.md` — markdown with phases, features, deliverables (E001–E206), and checkboxes (`[x]`, `[ ]`, `[~]`, `[!]`) for DEV and AUD status.
- **Conventions**: Each deliverable has DEV (development) and AUD (audit) columns. Feature closure requires both `[x] DEV` and `[x] AUD`.
- **Scope**: Mini feature — lightweight, focused on visibility and auto-update when roadmap changes.
- **Tech stack**: Angular 17 frontend, .NET 8 backend.

## Acceptance Criteria

1. **Dashboard view** shows:
   - Overall progress: % of deliverables with `[x] DEV` and `[x] AUD` (closed)
   - Per-phase breakdown: Phase 0–4 with counts (done / total)
   - Per-feature summary: feature ID, name, closure status
   - Optional: list of blocked items `[!]` and in-progress `[~]`

2. **IDE internal tool**: Not part of the application. Run `node scripts/roadmap-dashboard.js` after each ROADMAP.md update. Opens standalone HTML in browser — no server needed.

3. **Access**: Open `docs/roadmap/roadmap-dashboard.html` in browser (generated file, in .gitignore).

## Out of Scope (for this mini feature)

- Editing the roadmap from the dashboard
- Historical trends or charts over time
- User authentication for dashboard access

## Notes

- Parsing logic must handle the existing ROADMAP.md structure: tables with `| # | Deliverable | DEV | AUD |`, task lists `- [x] **T-01**`, and phase/feature headers.
- Consider a lightweight backend endpoint that reads and parses `ROADMAP.md` and returns structured JSON, or a build-time script that generates `docs/roadmap/roadmap-data.json` for static consumption.
