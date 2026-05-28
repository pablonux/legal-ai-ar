---
name: entity-analyzer
description: "Analyzes and validates Argentine legal domain entities against the Legal Ai Ar ontology. Use when the user asks to review entities, verify the data model, analyze relationships between entities, validate model consistency against the ontology, add new entities, or any task involving the legal domain classes (LegalNorm, Ruling, Article, CaseFile, Court, etc.). Also applies when 'ontology', 'domain model', 'legal entities', 'legal graph', or 'relationships between norms/rulings' are mentioned."
---

# Entity Analyzer — Legal Ai Ar

Analyzes, validates, and extends the Argentine legal domain entity model.

## Sources of truth

Before any analysis, read these files in order:

1. `docs/ontology/argentine-legal-ontology.md` — Formal specification: classes, properties, relationships, cardinalities
2. `docs/ontology/ontology-data-sources.md` — Primary and secondary sources by class
3. `docs/technical/09-data-knowledge-management.md` — Taxonomy, temporal versioning, graph maintenance

## Main domain entities

The model has ~44 entities organized into these groups:

- **Regulatory**: LegalNorm, Article, Clause, NormType, LawBranch, ValidityStatus
- **Case law**: Ruling, Headnote, Opinion, Court, Judge
- **Procedural**: CaseFile, Deadline, ProceduralAct, DeadlineType, Jurisdiction
- **People/Org**: Person, Organization, CaseParty
- **Relationships**: CitedNorm, CitedRuling, AppliedNorm, Repeal, Amendment
- **AI/RAG**: Chunk, Embedding, Community, DocumentStageLog

## Skill capabilities

### 1. Validate an entity against the ontology

Given an entity name or a C# code snippet (a Core class), verify:
- That the entity exists in the ontology
- That its properties match (name, type, required/optional)
- That relationships are correctly defined
- That enums reference valid values

### 2. Analyze relationships

Given two entities, map all relationship paths between them according to the ontology. Useful for designing queries, endpoints, or agent tools.

### 3. Propose a new entity

If the user needs an entity that does not exist, generate the proposal following the ontology format:
- Class name (PascalCase)
- Properties with types and cardinalities
- Relationships with existing entities
- Suggested data source
- Impact on AI Search indexes

### 4. Generate C# entity code

Produce the C# class for `LegalAiAr.Core/Entities/` following the conventions:

```csharp
namespace LegalAiAr.Core.Entities;

public class EntityName
{
    public Guid Id { get; set; }
    // Properties per the ontology
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public virtual ICollection<RelatedEntity> Related { get; set; } = [];
}
```

### 5. Verify model-ontology consistency

Compare all entities in `mvp/backend/src/shared/LegalAiAr.Core/Entities/` against the ontology and report:
- Entities in code that are not in the ontology
- Entities in the ontology that are not in code
- Discrepancies in properties or relationships

## Report format

When reporting analysis results, use this format:

```
## Analysis of {Entity/Relationship}

**Status**: ✅ Consistent | ⚠️ Discrepancies | ❌ Missing

### Findings
- {Finding 1}
- {Finding 2}

### Suggested actions
- {Action 1}
- {Action 2}
```
