# Tutorial: AI Setup for Legal Ai Ar

> Reference guide for the project's AI assistant configuration. Covers Cowork (Claude Desktop) and Cursor, both configured to behave identically.

---

## 1. The project

The Cowork project is already connected to the `legal-ai-ar/` folder, which contains:

```
legal-ai-ar/
├── backend/                # .NET 10, Clean Architecture
├── frontend/               # Angular 19 SPA
├── infra/                  # Azure provisioning scripts
├── deployment/             # GCaaS Helm chart
├── docs/                   # Project documentation
│   ├── roadmap/            # features.md, backlog.md, STATUS.md + work items by feature
│   ├── technical/          # 9 technical documents (RAG, agents, prompts, etc.)
│   └── ontology/           # Argentine legal domain model
├── README.md
├── CLAUDE.md               # → Instructions for Cowork (section 2)
├── .claude/                # → Cowork configuration
│   ├── memory.md           #   Persistent memory (section 3)
│   └── skills/             #   13 shared skills (section 4)
│       ├── work-item-generator/   entity-analyzer/
│       ├── consistency-checker/   documenter/
│       ├── architect/             developer/
│       ├── designer/              reviewer/
│       ├── task-breakdown/        doc-standards/
│       ├── backend-tools/         ia-tools/
│       └── infra-tools/
└── .cursor/                # → Cursor configuration (section 5)
    ├── rules/              #   Project rules for Cursor
    │   ├── project.mdc     #   General rules (equivalent to CLAUDE.md)
    │   ├── backend-dotnet.mdc    # Backend conventions
    │   ├── frontend-angular.mdc  # Frontend conventions
    │   └── work-items.mdc       # Work item template
    └── skills/             #   The same 13 skills as in .claude/
```

When you open a Cowork session with this folder, Claude automatically reads `CLAUDE.md` and has access to all the repo files, the skills, and the memory.

---

## 2. CLAUDE.md — Project instructions

**File**: `CLAUDE.md` (repo root)

It is the project's "system prompt". Claude reads it at the start of every session. It contains the rules and context Claude needs to work correctly with Legal Ai Ar.

### What it includes

- **Identity**: what Legal Ai Ar is
- **Stack**: Angular 19, .NET 10, Azure OpenAI, Semantic Kernel, etc.
- **Monorepo structure**: map of folders and .NET projects
- **Code conventions**: Clean Architecture, naming, patterns (backend and frontend)
- **Azure naming**: `{service}-legal-ai-ar-{environment}`
- **Releases**: R0.0 (Preparation) → R1.0 (Research & Monitoring) → R2.0 (Professional Productivity) → R3.0 (Knowledge & Intelligence) → R4.0 (Scale & Operations)
- **Rules**: English-first language rule, single monorepo, `LegalAiAr.*` names (never LegalKB)

### How to extend it

Edit the file directly to add new rules. Examples:

- Style preferences: "always use `var` in C#"
- Package restrictions: "do not use Dapper, use EF Core"
- Git conventions: "commits in English, conventional commits format"
- Scope restrictions: "do not touch the MVP workers without asking"

---

## 3. Memory — Persistent context

**File**: `.claude/memory.md`

Stores decisions, conventions, and context that Claude can consult in any future session.

### Current content

- **Decisions**: existing monorepo (do not create a new repo), removal of SignalR for workers, rename of R0.0 to "Preparation"
- **Conventions**: feature numbering, work item format, Azure naming
- **Status**: current release / work item (see `docs/roadmap/STATUS.md` for live progress)
- **Things to avoid**: never "LegalKB", no separate repos, no SignalR for workers

### How to update it

Tell Claude at any time:
- "Remember that we decided to use Azure Functions instead of BackgroundService"
- "Add to memory that the ValidityStatus field is now an enum"
- "Update the memory: R0.0 is complete"

The memory grows over time. Every so often it is worth reviewing it and cleaning up what no longer applies.

---

## 4. Custom skills

The most frequent project tasks are covered by specialized skills. They activate automatically when Claude detects that your request matches one of them.

### 4.1 Work Item Generator

**When it activates**: when you ask to create a work item, task, user story, or ticket for any feature.

**What it does**:
1. Reads `features.md` to understand the parent feature
2. Checks existing work items to continue the numbering
3. Generates the `.md` with the full template: description, tasks with checkboxes, technical sections, acceptance criteria, dependencies
4. Saves it in the correct folder (`docs/roadmap/F{XX} - {Name}/`)

**Examples**:
```
"Create the work items for F2.2 - Document Review and Analysis"
"I need a W03 in F1.3 for the ARCA dictámenes ingestion source"
"Add a roadmap task to implement the projects/workspaces API in F2.1"
```

### 4.2 Entity Analyzer

**When it activates**: when working with domain entities, the data model, the ontology, or the legal graph.

**Capabilities**:
- Validate an entity against the formal ontology (`docs/ontology/`)
- Analyze relationships between two entities
- Propose new entities with properties, relationships, and C# code
- Audit consistency between the code (`LegalAiAr.Core/Entities/`) and the ontology

**Examples**:
```
"Verify that the Project (Workspace) entity in the code matches the model"
"We need a TaxControversy entity. Propose the structure"
"What relationships exist between Ruling and Statute per the ontology?"
```

### 4.3 Consistency Checker

**When it activates**: when you ask for documentation audits, consistency reviews, or after mass changes.

**What it verifies**:
- Features in `features.md` vs. existing folders and files on disk
- Work item metadata (release, feature, ID, footer)
- Correct names (`LegalAiAr`, not `LegalKB`)
- Endpoints and DTOs consistent across work items
- Valid cross-references and dependencies (no cycles)

**Examples**:
```
"Review the consistency of all F1.7 work items"
"Are there inconsistencies between features.md and the folders?"
"Verify that no LegalKB is left in the docs"
```

### 4.4 Documenter

**When it activates**: when you ask for technical documentation, guides, ADRs, or API documentation.

**Document types it generates**:
- Numbered technical document (for `docs/technical/`)
- Step-by-step implementation guide
- Architecture Decision Record (ADR)
- API/endpoint documentation

**Examples**:
```
"Document the ingestion pipeline architecture as a technical doc"
"Create an ADR about the decision to use Reciprocal Rank Fusion"
"Write an implementation guide for the Semantic Kernel setup"
```

---

## 5. Cursor — Configuration for the second dev

The other developer works with Cursor. The configuration has two levels: rules (always active depending on context) and skills (specialized roles invoked explicitly).

### 5.1 Rules (`.cursor/rules/`)

They load automatically depending on the file being edited:

| File | Scope | Cowork equivalent |
|------|-------|-------------------|
| `project.mdc` | Always active | Full CLAUDE.md |
| `backend-dotnet.mdc` | `*.cs` and `*.csproj` files in `backend/` | "Backend (.NET)" section of CLAUDE.md |
| `frontend-angular.mdc` | `*.ts`, `*.html`, `*.scss` files in `frontend/` | "Frontend (Angular)" section of CLAUDE.md |
| `work-items.mdc` | `*.md` files in `docs/roadmap/` | work-item-generator skill |

### 5.2 Skills (`.cursor/skills/`)

The **same 13 skills** are available in Cowork (`.claude/skills/`) and in Cursor (`.cursor/skills/`), with identical names. Either one can invoke any skill; the difference is each platform's focus, not the capabilities.

| Skill | Role |
|-------|------|
| **work-item-generator** | Generates work items following the standard template |
| **consistency-checker** | Verifies roadmap consistency |
| **entity-analyzer** | Validates entities against the ontology |
| **documenter** | Generates technical documentation (ADR, guides, API) |
| **architect** | Analyzes technical impact, produces an implementation plan |
| **developer** | Implements work items with production-ready code |
| **designer** | Creates HTML mockups with PwC design guidelines |
| **reviewer** | Code review against project standards |
| **task-breakdown** | Breaks a work item into concrete tasks |
| **doc-standards** | Markdown, Mermaid, ADR templates |
| **backend-tools** | .http files, EF migrations, CQRS scaffolding |
| **ia-tools** | Prompt templates, golden sets, Semantic Kernel plugins |
| **infra-tools** | Azure config, GitHub Actions |

### 5.3 Distribution of responsibilities

Although both platforms have the 13 skills, in practice each focuses on different stages:

```
                    COWORK (Pablo)              CURSOR (Other dev)
                    ──────────────              ──────────────────
Planning            work-item-generator         (available)
                    consistency-checker
                    entity-analyzer

Technical analysis  (available)                 architect
Design              (available)                 designer
Development         (available)                 developer
Review              reviewer                     reviewer
Documentation       documenter                   (available)
```

Cowork focuses on planning and documentation. Cursor focuses on technical analysis, design, and implementation. Both share the same base rules (CLAUDE.md = project.mdc), the same skills, and the same restriction of not touching code without permission.

### 5.4 What both configurations share

- The same 13 skills with identical names
- The same rule of not touching code without explicit permission
- **Same language**: everything in English (code, names, docs, commits, work items); Spanish only for end-user facing text
- Same naming (LegalAiAr, never LegalKB)
- Same monorepo structure and Clean Architecture
- Same Azure and code conventions
- Same work item template

### 5.5 How to keep parity

When a new rule is added to CLAUDE.md, it must be reflected in `project.mdc` (and vice versa). Both files must be kept in sync.

---

## 6. Full workflow — Cowork + Cursor

Example: implement **F2.2 - Document Review and Analysis**

### Phase 1 — Planning (Cowork)

```
Pablo:  "Create the work items for F2.2 - Document Review and Analysis"
Claude: [skill: work-item-generator → reads features.md → generates W01-W05]

Pablo:  "Review the consistency"
Claude: [skill: consistency-checker → validates metadata, dependencies, numbering]

Pablo:  "We need a Document entity. Propose the structure"
Claude: [skill: entity-analyzer → reads the model → proposes the entity with C# code]
```

### Phase 2 — Technical analysis (Cursor)

```
Dev:    "Analyze the technical impact of F2.2-W02 (document analysis service)"
Cursor: [skill: architect → reads features.md + docs/technical/21-business-workspace-model.md
         + 16-chat-rag-agents.md → produces a plan: files, decisions, risks]
```

### Phase 3 — Design (Cursor)

```
Dev:    "Create the mockup for the document review view"
Cursor: [skill: designer → grounds on frontend/ + AppKit 4 → produces an HTML mockup with
         upload, extracted clauses/risks panel, version compare → asks for approval]
```

### Phase 4 — Development (Cursor)

```
Dev:    "Implement F2.2-W02"
Cursor: [skill: developer → reads W02 + the architect's plan → presents:
         "Create DocumentAnalysisService.cs in
          backend/src/api/LegalAiAr.Application/Documents/ with the following code: ..."
         → waits for approval]

Dev:    "Go ahead, approved"
Cursor: [presents the complete code for each file → asks for final approval]
```

### Phase 5 — Review (Cursor or Cowork)

```
Dev:    "Review the code of the F2.2-W02 PR"
Cursor: [skill: reviewer → reviews against conventions, tests, Clean Architecture,
         and the Definition of Done → approves or reports issues by severity]
```

### Phase 6 — Documentation (Cowork)

```
Pablo:  "Document the document-review pipeline"
Claude: [skill: documenter → updates docs/technical/21-business-workspace-model.md]
```

---

## 7. Tips

- **Be specific**: "Create W03 for F2.2 about the document analysis service" works better than "do something for F2.2"
- **Chain skills**: create a work item and then ask it to verify consistency
- **Update the memory**: when you make an important decision, ask Claude to record it for future sessions
- **Iterate on the skills**: if a skill does not generate what you expect, edit its `SKILL.md` directly
- **Use CLAUDE.md as a contract**: if Claude makes a recurring mistake (e.g., uses "LegalKB"), add a more explicit rule

---

*Tutorial: AI Setup for Legal Ai Ar — Legal Ai Ar*
