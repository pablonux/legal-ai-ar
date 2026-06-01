---
name: work-item-generator
description: "Generates work items (.md) for the Legal Ai Ar roadmap following the project's standard template. Use when the user asks to create a new work item, task, user story, or ticket for any roadmap feature (F00 or any R1–R4/FT feature in features.md). Also use when they say 'create W0X for FY.Z', 'add a task to the roadmap', 'I need a work item for...', or any variation that implies creating a new unit of work within the project plan."
---

# Work Item Generator — Legal Ai Ar

Generates work items in Markdown that follow the project's standard template.

## Project context

Legal Ai Ar (PwC tax-legal) has a roadmap organized into 5 releases (R0.0–R4.0). **R0.0/F00** (Preparation) has detailed tickets on disk; the **R1–R4** features (`F1.x`, `F2.x`, `F3.x`, `F4.x`) and cross-cutting `FT.x` are catalogued in `features.md` and get detailed work items generated on demand when scheduled. Each feature contains work items (W01, W02, ...) which are individual .md documents.

## Before generating

1. Read `docs/roadmap/features.md` to understand the parent feature, its release, and existing work items
2. Read at least one existing work item of the same feature to capture the style and level of detail
3. If the feature has a W01 (Comprehensive Documentation), read it to understand the full technical context

## Work item template

Each work item is saved at: `docs/roadmap/{Feature Folder}/{ID} - {Title}.md`

Where:
- Feature Folder: `F{X.Y} - {Feature Name}` (e.g., `F2.2 - Document Review and Analysis`)
- ID: `F{XX} - W{YY}` with sequential numbering
- Title: descriptive name in English

### Required structure

```markdown
# {ID} - {Title}

> **Feature:** {Parent feature name}
> **Release:** {X.0} | **Sprint:** {SXX}
> **Type:** {backend|frontend|fullstack|infra|docs} | **Priority:** {Critical|High|Medium|Low} {(blocking) if applicable}
> **Estimate:** {N} story points
> **Assignable to:** Backend Dev | Frontend Dev | Fullstack Dev

---

## Description

{1-3 paragraphs explaining what to do and why. Be specific.}

---

## Tasks

### {Logical section 1}

- [ ] Concrete, verifiable task
- [ ] Concrete, verifiable task

### {Logical section 2}

- [ ] ...

---

## {Relevant technical sections}

{Include depending on the work item type:}
- Endpoints (method, route, request/response)
- Data models or DTOs
- Flow or sequence diagrams (Mermaid)
- Illustrative code snippets
- Required configuration

---

## Acceptance Criteria

- [ ] Verifiable criterion 1
- [ ] Verifiable criterion 2
- [ ] Unit tests cover the main case
- [ ] {Work-item-specific criterion}

---

## Dependencies

- **Blocks:** {Work items that depend on this one}
- **Prerequisites:** {Work items that must be completed first}

---

*{ID} - {Title} — Legal Ai Ar*
```

## Generation rules

- **Language**: work item content in English (title, description, tasks, acceptance criteria, technical sections). Spanish only for end-user facing UI strings referenced in the work item.
- **Project names**: use `LegalAiAr.{Layer}` (never LegalKB)
- **Numbering**: check the last existing W in the feature and continue the sequence
- **Sprint**: assign according to the releases table in features.md
- **Story points**: 1 (trivial), 2 (simple), 3 (moderate), 5 (complex), 8 (very complex)
- **Priority**: mark as "blocking" if other work items depend directly on it
- **Footer**: always include the final italic line with the ID and title
- **Consistency**: the endpoints, DTOs, and entities mentioned must exist or be planned in features.md

## Usage example

The user says: "Create the work item for the document analysis service in F2.2"

Steps:
1. Read `docs/roadmap/features.md` → find F2.2 (Document Review and Analysis) and its planned scope/endpoints
2. Read any existing work items in `docs/roadmap/F2.2 - Document Review and Analysis/` (create the folder if it is the first)
3. Determine the next number (W0X)
4. Generate the .md with the template, including the specific endpoint, DTOs, and data flow
5. Save to `docs/roadmap/F2.2 - Document Review and Analysis/F2.2 - W0X - {Title}.md`
