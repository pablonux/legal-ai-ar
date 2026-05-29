# Developer Guide — Legal Ai Ar

> How to work with the AI assistants (Claude in Cowork / Cursor) to implement project work items.

---

## 1. Before you start

### Development environment

Full step-by-step setup — for both supported environments (**VS Code + Claude + Docker** and
**Cursor + Docker**) — is in the **[Developer Onboarding guide](onboarding/README.md)**. Get the
project running locally there first, then come back here for the day-to-day workflow. In short, you
need:

- .NET 10 SDK
- Node.js 20 LTS+ (Angular CLI installed locally via `npm ci`)
- Docker Desktop (run the app — API, SPA, workers — in containers)
- Access to the shared cloud DEV services (Azure SQL, AI Search, OpenAI, Storage) — there is no local stack
- Git + access to the `legal-ai-ar` repo

> The original planning context for the environment lives in work item **F00 - W07 - Local
> Environment Setup and Onboarding Guide**; the onboarding guide above is the live, dev-facing version.

### AI tools

We work with two AI assistance tools. Both are configured to behave identically — same rules, same naming, same conventions:

| Tool | Who uses it | Main focus |
|------|-------------|------------|
| **Cowork** (Claude Desktop) | Pablo | Planning, documentation, consistency |
| **Cursor** | Developers | Technical analysis, design, implementation, review |

The AIs **never create or modify code directly**. They always tell you which file to create, in which path, and give you the code for you to place. This is a project rule.

### Repo structure

```
legal-ai-ar/
├── mvp/
│   ├── backend/src/
│   │   ├── api/LegalAiAr.Api/              # Minimal API, endpoints
│   │   ├── api/LegalAiAr.Application/      # CQRS, handlers, DTOs
│   │   ├── shared/LegalAiAr.Core/          # Entities, interfaces
│   │   ├── shared/LegalAiAr.Infrastructure/ # EF Core, Azure, AI
│   │   ├── shared/LegalAiAr.Agents/        # Semantic Kernel
│   │   ├── workers/                         # BackgroundServices
│   │   └── tools/                           # Auxiliary CLIs
│   ├── backend/tests/                       # xUnit + NSubstitute
│   └── frontend/                            # Angular 19 SPA
├── docs/
│   ├── roadmap/                             # Work items by feature
│   ├── technical/                           # 9 technical docs
│   └── ontology/                            # Domain model
├── CLAUDE.md                                # AI instructions (Cowork)
└── .cursor/
    ├── rules/                               # AI instructions (Cursor)
    └── skills/                              # Cursor skills
```

---

## 2. Anatomy of a work item

Each work item is a `.md` file in `docs/roadmap/{Feature}/`. This is the structure:

```
Header          → Feature, Release, Sprint, Type, Priority, Estimate
Description     → What to do and why
Tasks           → Checklist (generic at creation, detailed after the breakdown)
Technical notes → Stack, patterns, configuration
Files           → What to create/modify
Criteria        → When it is "done"
Dependencies    → What it blocks and what it needs
```

When a work item is assigned to you, the first thing you do is **break down the generic tasks into concrete implementation tasks** using the `task-breakdown` skill.

---

## 3. Skills available in Cursor

These are the skills available to you in Cursor. You invoke them from the chat by writing what you need — Cursor automatically detects which one to use.

### task-breakdown — Break down tasks

**When**: upon receiving an assigned work item, before starting to code.

**What it does**: reads the work item, the relevant technical documentation, and replaces the generic tasks with a detailed checklist of files to create/modify, with exact paths, class names, and methods.

```
You:    "Break down the tasks of work item F08-W03"
Cursor: [reads W03, W01, features.md, relevant technical docs
         → generates a checklist: 1. Model (DTOs), 2. Services,
         3. CQRS, 4. Validation, 5. Endpoints, 6. Tests, 7. Verification
         → updates the work item's Tasks section]
```

### architect — Technical analysis

**When**: before the breakdown, when you need to understand the technical impact of a work item on the system.

**What it does**: analyzes which layers, services, and components are affected. Produces a plan with technical decisions, dependencies, and risks.

```
You:    "Analyze the technical impact of F09-W02"
Cursor: [reads W02 + technical docs → plan: files to create,
         decisions (Strategy pattern vs direct), risks, suggested order]
```

### developer — Implementation

**When**: after the breakdown, when you are ready to code.

**What it does**: for each task in the breakdown, it presents the complete code for each file with its path. You place it.

```
You:    "Let's implement task 1 of F08-W03: create the DTOs"
Cursor: [presents: "Create LegalNormSearchRequestDto.cs in
         backend/src/api/LegalAiAr.Application/DTOs/LegalNorms/
         with the following code: ..."]
```

### designer — Mockups

**When**: when you need HTML mockups before implementing a frontend component.

```
You:    "Create the mockup for the legal norm search view"
Cursor: [reads design guidelines → produces HTML mockup]
```

### reviewer — Code review

**When**: before opening a PR, to verify your code meets the standards.

```
You:    "Review the files I created for F08-W03"
Cursor: [reviews against conventions → report with issues by severity]
```

---

## 4. Full flow: from work item to PR

This is the step-by-step flow to implement an assigned work item.

### Step 1 — Understand the context

Read the work item and its W01 (Comprehensive Documentation). If something is unclear, ask the AI:

```
You:    "Explain what F08-W03 needs and how it fits with the rest of the feature"
```

### Step 2 — Technical analysis (optional but recommended)

If the work item is complex (5+ story points), ask for a technical analysis first:

```
You:    "Analyze the technical impact of F08-W03"
Cursor: [skill: architect → produces an implementation plan]
```

Review the plan. If there are technical decisions to make, discuss them now.

### Step 3 — Break down tasks

Ask for the task breakdown. This replaces the work item's generic tasks:

```
You:    "Break down the tasks of F08-W03"
Cursor: [skill: task-breakdown → generates a detailed checklist in the work item]
```

Review the checklist. If something is missing or does not make sense, ask for adjustments.

### Step 4 — Create a branch

```bash
git checkout main
git pull
git checkout -b feature/f08-w03-chat-endpoint
```

### Step 5 — Implement task by task

Follow the checklist in order. For each task, ask for the code:

```
You:    "Implement task 1: create the chat DTOs"
Cursor: [skill: developer → presents complete code with path]
```

Create the file in the indicated path and paste the code. Repeat for each task.

As you complete each task, mark it with `[x]` in the work item.

### Step 6 — Verification

Once all tasks are complete:

```bash
dotnet build
dotnet test
```

Verify the work item's acceptance criteria one by one.

### Step 7 — Review

Before the PR, ask for a review:

```
You:    "Review everything I implemented for F08-W03"
Cursor: [skill: reviewer → issue report]
```

Fix the critical and important issues.

### Step 8 — Documentation round-trip (mandatory — Definition of Done)

**A work item cannot be closed until its documentation is updated.** Before opening the PR:

1. Mark the work item's **Tasks** and **Acceptance Criteria** as `[x]`.
2. Update **`docs/roadmap/STATUS.md`**: add a row to the progress log and advance "Next up".
3. **Sync any affected docs** to match what you actually built — `docs/technical/*` (architecture, ingestion, chat, data model, frontend, admin, data sources), `docs/ontology/*`, `docs/deployment/*`, `features.md`, or onboarding — whichever the change touched.
4. If you discovered a new public data source, document it in `docs/technical/20-legal-data-sources.md` (don't commit static data files).

See the full checklist in [`docs/roadmap/DEFINITION-OF-DONE.md`](roadmap/DEFINITION-OF-DONE.md). The AI (`reviewer` skill) and the PR template both verify this; **do not merge with documentation pending.**

### Step 9 — PR

```bash
git add .
git commit -m "feat(F08): implement POST chat endpoint with SignalR streaming"
git push -u origin feature/f08-w03-chat-endpoint
```

Create the PR to `main` with:
- Title: `feat(F08): implement POST chat endpoint with SignalR streaming`
- Description: list of created/modified files and the referenced work item
- Work item reference: `Closes F08-W03`

---

## 5. Conventions the AI applies (and you too)

These rules are configured in the AI and applied automatically, but it is good to know them:

| Topic | Convention |
|-------|------------|
| **Language** | Everything in English: code, names, comments, documentation, commits, work items. Spanish only for end-user facing text (UI labels, error messages, tooltips) |
| **Names** | `LegalAiAr.*` for .NET projects (never "LegalKB") |
| **Architecture** | Clean Architecture: Core → Application → Infrastructure → Api |
| **Core** | NEVER references another project |
| **API** | Minimal API (no Controllers) |
| **CQRS** | Commands and Queries separated, handlers with MediatR |
| **DI** | Constructor injection only |
| **async** | All I/O async with CancellationToken |
| **Logging** | `ILogger<T>` structured, never `Console.WriteLine` |
| **Config** | `IOptions<T>`, no secrets in appsettings |
| **Tests** | xUnit + NSubstitute + FluentAssertions. Naming: `{Method}_{Scenario}_{Result}` |
| **Angular** | Standalone components, signals, no `any`, interfaces in `core/models/` |
| **Git** | Branch: `feature/{fXX}-{desc}`. Commits: `feat/fix/refactor(F{XX}): description` |
| **Azure** | Resources: `{service}-legal-ai-ar-{environment}` |

---

## 6. Reference documentation

These are the documents the AI consults and that you can also read when you need context:

| Document | What for |
|----------|----------|
| `docs/roadmap/features.md` | Full roadmap, endpoints, KPIs |
| `docs/roadmap/{Feature}/` | Work items of each feature |
| `docs/technical/01-rag-retrieval.md` | Hybrid search, embeddings, re-ranking |
| `docs/technical/02-agentic-architecture.md` | Agents, router, tool calling |
| `docs/technical/03-prompt-engineering.md` | Templates, system prompts |
| `docs/technical/04-ingestion-processing.md` | Ingestion pipeline |
| `docs/technical/05-ai-quality-evaluation.md` | Metrics and evaluation |
| `docs/technical/06-ai-security-compliance.md` | Security and compliance |
| `docs/technical/07-observability-llmops.md` | Observability |
| `docs/technical/08-legal-ai-ux.md` | Chat UX and citation |
| `docs/technical/09-data-knowledge-management.md` | Data management |
| `docs/ontology/argentine-legal-ontology.md` | Legal domain model |

---

## 7. Frequently asked questions

**Can I ask the AI to write the code directly into the files?**
No. The project rule is that the AI indicates what to create and where, and you place it. This is intentional so you always review what goes into the repo.

**What do I do if the AI suggests something that does not make sense?**
Tell it. "That doesn't seem right because X". The AI adjusts. If it is an architecture decision, check with Pablo.

**Can I skip the task-breakdown?**
For simple work items (1-2 story points) you can go straight to the developer. For 3+ story points, the breakdown saves you time and errors.

**How do I know which technical docs to read?**
You don't need to read them yourself — the AI reads them automatically when you ask it to analyze or implement something. But if you want context, they are in `docs/technical/`.

**What if I need an endpoint that is not in features.md?**
Let Pablo know. There may be a missing work item or the roadmap may need adjusting.

---

*Developer Guide — Legal Ai Ar*
