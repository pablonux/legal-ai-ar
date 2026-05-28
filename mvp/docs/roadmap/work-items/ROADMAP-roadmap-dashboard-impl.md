# Implementation Roadmap — Roadmap Dashboard (F0-3)

**Source**: WI-2026-03-11-roadmap-dashboard · **Feature**: Roadmap Dashboard  
**Date**: 2026-03-11 · **For**: Manager to merge into ROADMAP.md

---

## Analysis Summary

The Roadmap Dashboard is a mini feature that surfaces project progress from `docs/roadmap/ROADMAP.md`. It requires:

1. **Parser**: Extract phases, features, deliverables and their DEV/AUD status from markdown
2. **Data source**: API endpoint or static JSON, updated when roadmap changes
3. **Frontend**: Angular dashboard view with progress metrics, phase/feature breakdown

**Impact**: No changes to data model, pipeline, or existing API. New API endpoint (or static JSON) and new Angular route/component. Can run in parallel with Phase 1 work.

**Placement**: Phase 0 — Foundations, as F0-3 (tooling/visibility). Does not block Phase 1 features.

---

## ROADMAP Merge Block

Insert after F0-2 (before `## Phase 1 — MVP`):

```markdown
---

### F0-3 · Roadmap Dashboard

**Objective**: Dynamic dashboard showing roadmap and project progress, updated when ROADMAP.md changes.

#### T-00 · Design and documentation

| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E207 | `docs/design/f0-3-roadmap-parser.md` — parser spec: regex/patterns for phases, features, deliverables, DEV/AUD checkboxes in ROADMAP.md | `[ ]` | `[ ]` |
| E208 | `docs/design/f0-3-roadmap-dashboard-ui.md` — UX spec: layout, metrics (overall %, phase breakdown, feature list), component structure | `[ ]` | `[ ]` |
| E209 | `docs/design/f0-3-roadmap-dashboard-flow.mermaid` — data flow: ROADMAP.md → parser → API/JSON → Angular dashboard | `[ ]` | `[ ]` |
| E210 | `docs/mockups/mockup-roadmap-dashboard.html` — dashboard page mockup per PwC guidelines (Designer deliverable) | `[ ]` | `[ ]` |

#### Development tasks

- [ ] **T-01** Implement RoadmapParser (C# or Node script) that reads ROADMAP.md and produces structured JSON (phases, features, deliverables with DEV/AUD status)
- [ ] **T-02** Add GET `/api/roadmap` endpoint (or build-time script that writes `docs/roadmap/roadmap-data.json` for static consumption)
- [ ] **T-03** Create Angular RoadmapDashboardComponent with route `/dashboard`
- [ ] **T-04** Implement progress metrics: overall % closed, per-phase breakdown, per-feature summary, optional blocked/in-progress list
- [ ] **T-05** Integrate dashboard with existing shell (header, sidebar, PwC guidelines)

#### Development deliverables

| # | Deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E211 | RoadmapParser producing valid JSON from ROADMAP.md | `[ ]` | `[ ]` |
| E212 | API endpoint or static JSON artifact serving roadmap data | `[ ]` | `[ ]` |
| E213 | RoadmapDashboardComponent with progress metrics and phase/feature breakdown | `[ ]` | `[ ]` |
| E214 | Dashboard accessible at `/dashboard` in Angular SPA | `[ ]` | `[ ]` |

---

```

---

## Designer Deliverables

Designer produces E210 (mockup) after Documenter completes E208 (UX spec). Mockup is required before T-01.

---

## Summary Update

After merge, update Phase 0 row in Summary table:

| Phase | Features | Design deliverables | Development deliverables | Total deliverables |
|---|:---:|:---:|:---:|:---:|
| Phase 0 — Foundations | **3** | **10** | **16** | **26** |

(Add 1 feature, 4 design deliverables including Designer mockup, 4 development deliverables.)
