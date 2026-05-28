---
name: documenter
description: "Generates and maintains technical documentation for Legal Ai Ar: architecture documents, technical decisions, implementation guides, and API documentation. Use when the user asks to create technical documentation, write an ADR (Architecture Decision Record), document an endpoint, create an implementation guide, update docs/technical/, document a design decision, or generate documentation for any system component. Also applies when they say 'document', 'write docs', 'create a guide', 'ADR', 'technical decision', or any documentation request that is not a roadmap work item."
---

# Documenter — Legal Ai Ar

Generates professional technical documentation that integrates with the project's existing structure.

## Existing documentation structure

```
docs/
├── roadmap/          ← Work items and planning (do NOT use this skill, use work-item-generator)
├── technical/        ← Numbered technical documents (01-09)
│   ├── 01-rag-retrieval.md
│   ├── 02-agentic-architecture.md
│   ├── 03-prompt-engineering.md
│   ├── 04-ingestion-processing.md
│   ├── 05-ai-quality-evaluation.md
│   ├── 06-ai-security-compliance.md
│   ├── 07-observability-llmops.md
│   ├── 08-legal-ai-ux.md
│   └── 09-data-knowledge-management.md
└── ontology/         ← Domain model (only entity-analyzer modifies this)
```

## Document types this skill generates

### 1. Technical document (`docs/technical/`)

To add a new technical document, follow the existing numbering convention (next available number).

**Standard structure:**

```markdown
# {NN}-{kebab-case-title}

> Technical document — Legal Ai Ar

## Executive summary

{2-3 paragraphs with the context, problem, and adopted solution}

## Context and motivation

{Why this is needed, what problem it solves}

## Design / Architecture

{Detailed description of the solution}

### {Subsection 1}

{Detail with Mermaid diagrams if applicable}

### {Subsection 2}

## Design decisions

| Decision | Alternatives evaluated | Chosen | Reason |
|----------|------------------------|--------|--------|
| ... | ... | ... | ... |

## System impact

{Which components are affected, what changes it requires}

## References

- {Links to relevant internal or external documents}
```

### 2. Implementation guide

For step-by-step guides aimed at the developer.

```markdown
# Guide: {Title}

> Implementation guide — Legal Ai Ar

## Prerequisites

- {Required tools, configurations, or knowledge}

## Steps

### 1. {First step}

{Description}

```{language}
{Illustrative code}
```

### 2. {Second step}

...

## Verification

{How to confirm everything works correctly}

## Troubleshooting

| Problem | Cause | Solution |
|---------|-------|----------|
| ... | ... | ... |
```

### 3. Architecture Decision Record (ADR)

To document important architectural decisions.

```markdown
# ADR-{NNN}: {Decision title}

> Architecture Decision Record — Legal Ai Ar

## Status

{Proposed | Accepted | Rejected | Superseded by ADR-XXX}

## Context

{Situation that motivates the decision}

## Decision

{The decision made and its implications}

## Consequences

### Positive
- {Benefit 1}

### Negative
- {Trade-off 1}

### Risks
- {Risk 1 and mitigation}

## Alternatives considered

### {Alternative 1}
{Description and reason for rejection}

### {Alternative 2}
{Description and reason for rejection}
```

### 4. API/Endpoint documentation

To document individual endpoints or groups of endpoints.

```markdown
# API: {Endpoint group}

## {METHOD} {/route}

**Description**: {What it does}

**Authentication**: Bearer token (Entra ID)
**Roles**: {allowed roles}

### Request

```json
{
  "field": "type — description"
}
```

### Response ({HTTP code})

```json
{
  "field": "type — description"
}
```

### Errors

| Code | Description |
|------|-------------|
| 400 | {Description} |
| 401 | Not authenticated |
| 403 | No permissions |
```

## Style rules

- **Language**: all technical documentation in English. Spanish only when documenting end-user facing UI strings.
- **Format**: Markdown with Mermaid support for diagrams
- **Tone**: technical but accessible; explain the "why", not just the "what"
- **Code**: include illustrative snippets in C# or TypeScript as appropriate
- **Tables**: use for comparisons and decisions
- **Diagrams**: prefer Mermaid (flowchart, sequence, class) so they are versionable in Git
- **Consistency**: use the project names (LegalAiAr.*), Azure conventions, and ontology terminology

## Before generating

1. Read the existing technical documents in `docs/technical/` related to the topic
2. Check `docs/roadmap/features.md` if the document relates to a roadmap feature
3. Verify you are not duplicating information that already exists in another document
