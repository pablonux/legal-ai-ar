# Roadmap Parser — Specification

| Field | Value |
|---|---|
| **ID** | E207 |
| **Feature** | F0-3 · Roadmap Dashboard |
| **Date** | 2026-03-11 |

---

## Purpose

This document specifies how to parse `docs/roadmap/ROADMAP.md` to extract phases, features, deliverables, and their DEV/AUD status. The output is structured JSON for consumption by the Angular dashboard.

---

## Source Structure

ROADMAP.md uses the following patterns:

### Phase Headers

```
## Phase 0 — Foundations
## Phase 1 — MVP
...
```

**Regex**: `^## Phase (\d+) — (.+)$`

### Feature Headers

```
### F0-1 · Repository and project structure
### F1-10 · Frontend — Search and ruling detail
```

**Regex**: `^### (F\d+-\d+) · (.+)$`

### Design Deliverables Table

```
| # | Design deliverable | DEV | AUD |
|---|---|:---:|:---:|
| E001 | `docs/design/f0-1-repo-structure.md` — ... | `[x]` | `[x]` |
| E002 | ... | `[ ]` | `[ ]` |
```

**Pattern**: Table rows after "Design deliverable" header. Extract:
- Deliverable ID (E001–E999)
- DEV status: `[x]` = done, `[ ]` = pending, `[~]` = in progress, `[!]` = blocked
- AUD status: same values

**Regex for checkbox**: `\|\s*`([x~! ])`\s*\|\s*`([x~! ])`\s*\|`

### Development Deliverables Table

Same structure as design deliverables, with "Deliverable" column (no "Design" prefix).

---

## Checkbox Conventions

| Symbol | Meaning |
|--------|---------|
| `[x]` | Completed |
| `[ ]` | Pending |
| `[~]` | In progress |
| `[!]` | Blocked |

**Closed deliverable**: Both DEV and AUD are `[x]`.

---

## Output JSON Schema

```json
{
  "phases": [
    {
      "id": 0,
      "name": "Foundations",
      "fullName": "Phase 0 — Foundations",
      "features": [
        {
          "id": "F0-1",
          "name": "Repository and project structure",
          "designDeliverables": [
            { "id": "E001", "dev": "x", "aud": "x", "closed": true },
            { "id": "E002", "dev": "x", "aud": "x", "closed": true }
          ],
          "developmentDeliverables": [
            { "id": "E004", "dev": "x", "aud": "x", "closed": true }
          ]
        }
      ],
      "totalDesign": 6,
      "closedDesign": 6,
      "totalDevelopment": 12,
      "closedDevelopment": 0
    }
  ],
  "summary": {
    "totalDeliverables": 214,
    "closedDeliverables": 42,
    "percentClosed": 19.6
  }
}
```

---

## Parsing Algorithm

1. Read ROADMAP.md as text
2. Split by `## Phase` to isolate each phase block
3. For each phase block:
   - Extract phase number and name from first line
   - Split by `### F` to isolate each feature
   - For each feature: extract ID and name from header
   - Find "Design deliverable" table: parse rows with `| E\d+ |` pattern
   - Find "Deliverable" table (development): same pattern
4. For each deliverable row: extract ID, DEV checkbox, AUD checkbox
5. Compute aggregates: closed count = DEV `[x]` AND AUD `[x]`
6. Output JSON

---

## Edge Cases

- **Task lists** (`- [x] **T-01**`): Not parsed as deliverables; only table rows with E-prefix IDs
- **Summary table**: Do not parse; compute from parsed data
- **Escaped backticks**: In table cells, `\`` may appear; strip for display only
