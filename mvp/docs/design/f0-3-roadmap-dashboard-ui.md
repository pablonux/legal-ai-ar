# Roadmap Dashboard — UX Specification

| Field | Value |
|---|---|
| **ID** | E208 |
| **Feature** | F0-3 · Roadmap Dashboard (IDE internal tool) |
| **Date** | 2026-03-11 |

---

## Purpose

This document specifies the UX of the Roadmap Dashboard: layout, metrics, and states. **IDE internal tool** — not part of the application. Run `node scripts/roadmap-dashboard.js` after each ROADMAP.md update, open the generated HTML in a browser.

---

## Usage

| Step | Action |
|---|---|
| 1 | Edit `docs/roadmap/ROADMAP.md` |
| 2 | Run `node scripts/roadmap-dashboard.js` |
| 3 | Open `docs/roadmap/roadmap-dashboard.html` in browser |

---

## Layout

### 1. Page Structure

- **Breadcrumb**: Inicio > Dashboard (or equivalent)
- **Eyebrow**: "Proyecto" or "Legal AI AR"
- **Page title**: "Avance del roadmap"
- **Content**: Stat cards (overall progress) + phase breakdown + feature list

### 2. Stat Cards (Top Row)

| Metric | Description |
|---|---|
| **% Cerrado** | Overall % of deliverables with both DEV and AUD `[x]` |
| **Entregables** | Closed / Total (e.g. "42 / 214") |
| **Fases completadas** | Count of phases where all deliverables are closed |

### 3. Phase Breakdown

For each phase (0–4):
- Phase name
- Progress bar (closed / total)
- Count: "X / Y entregables"
- Expandable or always-visible list of features

### 4. Feature Summary (Optional Collapsible)

Per feature:
- Feature ID (e.g. F0-1)
- Feature name
- Status: closed (all done) / in progress / pending
- Deliverables: design count, development count

---

## States

| State | UI |
|---|---|
| **Loading** | Spinner or skeleton while fetching roadmap data |
| **Loaded** | Full dashboard with metrics |
| **Error** | Error message with retry button |
| **Empty** | N/A (roadmap always exists) |

---

## Data Source

- **Script**: `scripts/roadmap-dashboard.js` parses ROADMAP.md and embeds data in HTML
- **Refresh**: Re-run script after roadmap update

---

## Accessibility

- Progress bars: `role="progressbar"`, `aria-valuenow`, `aria-valuemin`, `aria-valuemax`
- Semantic headings: h1 for page title, h2 for phase names
