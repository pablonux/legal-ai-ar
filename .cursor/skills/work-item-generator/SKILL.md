---
name: work-item-generator
description: "Generates work items (.md) for the Legal Ai Ar roadmap following the project's standard template. Use when the user asks to create a new work item, task, user story, or ticket for any roadmap feature (F00-F23, FT01-FT04). Also use when they say 'create W0X for F0Y', 'add a task to the roadmap', 'I need a work item for...', or any variation that implies creating a new unit of work within the project plan."
---

# Work Item Generator — Legal Ai Ar

Generates work items in Markdown that follow the project's standard template.

## Project context

Legal Ai Ar has a roadmap with ~24 features (F00-F23, FT01-FT04) organized into 5 releases (R0.0-R4.0). Each feature contains work items (W01, W02, ...) which are individual .md documents.

## Before generating

1. Read `docs/roadmap/features.md` to understand the parent feature, its release, and existing work items
2. Read at least one existing work item of the same feature to capture the style and level of detail
3. If the feature has a W01 (Comprehensive Documentation), read it to understand the full technical context

## Work item template

Each work item is saved at: `docs/roadmap/{Feature Folder}/{ID} - {Title}.md`

Where:
- Feature Folder: `F{XX} - {Feature Name}` (e.g., `F08 - AI Agent Chat`)
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

The user says: "Create the work item for the semantic search endpoint in F03"

Steps:
1. Read `docs/roadmap/features.md` → find F03 and its planned endpoints
2. Read the existing work items in `docs/roadmap/F03 - Legal Norm Search/`
3. Determine the next number (W0X)
4. Generate the .md with the template, including the specific endpoint, DTOs, and data flow
5. Save to `docs/roadmap/F03 - Legal Norm Search/F03 - W0X - {Title}.md`
