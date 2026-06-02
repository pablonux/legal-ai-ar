---
name: consistency-checker
description: "Checks consistency across all Legal Ai Ar roadmap documents: features.md, work items, backlog, gap analysis, and technical documents. Use when the user asks to review consistency, detect contradictions, verify that work items match features.md, find broken references, validate work item numbering, or any audit of the project documentation. Also applies when they say 'review docs', 'there are inconsistencies', 'verify roadmap', 'audit documentation', or after making mass changes to the documentation."
---

# Consistency Checker — Legal Ai Ar

Checks coherence across all project planning documents.

## Documents to verify

The main source of truth is `docs/roadmap/features.md`. Everything else must be consistent with this file.

### Document hierarchy

```
features.md (source of truth)
  ├── Each scheduled feature folder (F0.0 today; R1–R4/FT features F{X.Y} as generated)
  │   ├── W01 - Comprehensive Documentation
  │   └── W02..WNN - Specific work items
  ├── backlog.md
  └── docs/technical/*.md (technical references)
```

## Checks to perform

### 1. Features vs. Work Items

For each feature in features.md:
- Verify its folder exists at `docs/roadmap/F{XX} - {Name}/`
- Verify the work items listed in features.md have their corresponding .md
- Verify there are no orphan work items (.md files with no entry in features.md)
- Validate that work item numbering is sequential (W01, W02, W03...)

### 2. Work item metadata

For each work item .md:
- The **Release** field matches the assignment in features.md
- The **Feature** field matches the parent feature name
- The ID in the title matches the file name
- The footer contains the correct ID and title
- **Dependencies** reference work items that exist

### 3. Names and references

- All documents use "Legal Ai Ar" or "LegalAiAr" (never "LegalKB")
- .NET project names are `LegalAiAr.{Layer}` (Api, Application, Core, Infrastructure, Agents, AgentEvals)
- Monorepo folder paths match the actual structure
- Azure resources follow the `{service}-legal-ai-ar-{environment}` convention

### 4. Endpoints and DTOs

- Endpoints mentioned in work items exist in the endpoints table in features.md
- Referenced DTOs are consistent across work items of the same feature
- HTTP methods and routes have no duplicates or conflicts

### 5. Cross-references

- Links between documents (backlog, work items) point to existing files
- Dependencies between work items form a valid DAG (no cycles)
- References to technical documents (`docs/technical/`) are correct

## Verification procedure

1. Read the full `docs/roadmap/features.md`
2. List all feature folders and their files
3. For each feature, compare what is declared in features.md vs. what exists on disk
4. Read each work item and validate metadata
5. Compile the findings report

## Report format

```markdown
## Consistency Report — Legal Ai Ar

**Date**: {date}
**Files analyzed**: {N}

### Summary

| Category | ✅ OK | ⚠️ Warnings | ❌ Errors |
|----------|-------|-------------|-----------|
| Features vs. Folders | X | Y | Z |
| Work Item Metadata | X | Y | Z |
| Names and References | X | Y | Z |
| Endpoints and DTOs | X | Y | Z |
| Cross-references | X | Y | Z |

### Errors (require correction)

1. **{File}**: {Error description}

### Warnings (review manually)

1. **{File}**: {Warning description}

### Suggestions

1. {Improvement suggestion}
```

## Automatic correction

If the user asks to fix the errors found (not just report them), apply the corrections with the Edit tool, prioritizing:
1. Naming errors (LegalKB → LegalAiAr)
2. Incorrect metadata in work item headers
3. Missing or incorrect footers
4. Broken numbering

Never delete content — only fix inconsistencies.
