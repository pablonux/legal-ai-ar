# Legal Ai Ar

> PwC tax-legal knowledge base and productivity platform, with AI agents

Legal Ai Ar is an internal productivity tool for **PwC tax-legal professionals**. It centralizes access to legislation, case law, tax sources, and legal doctrine of the Argentine legal system, combining a multidimensional knowledge base with specialized AI agents, and organizes daily advisory work by **client projects/workspaces**. It is designed to reduce legal-tax research time from hours to minutes and to keep regulatory and tax deadlines under control.

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| **Frontend** | Angular 19 (standalone components) · PwC AppKit 4 · Tailwind CSS 4 · Cytoscape.js |
| **Backend** | .NET 10 · Clean Architecture (4 layers) · Minimal API · EF Core 10 |
| **AI / RAG** | Azure OpenAI (GPT-5o, GPT-5o-mini) · Semantic Kernel · text-embedding-3-large (3072d) |
| **Data** | Azure SQL (relational + Graph Tables) · Azure AI Search (hybrid search) · Azure Blob Storage |
| **Messaging** | Azure Storage Queues · Azure SignalR Service |
| **Hosting** | Azure App Service |

---

## Overall Architecture

The system is composed of three major blocks:

**1. Legal information sources** — Automated crawlers that collect documents from CSJN (Sumarios, Acuerdos, Fallos Destacados), SAIJ (Case Law, Legislation), and the Official Gazette (Boletín Oficial). Each source implements a strategy pattern (`IDiscoverStrategy`) to adapt to its particular structure.

**2. Multidimensional knowledge base** — Documents go through a 6-stage ingestion pipeline (Discovery → Fetch → Parse → Enrichment → Persist → Index) that transforms them into a 44-entity relational data model, semantic vectors in AI Search (3 indexes), and a legal graph with community detection. Contextual Retrieval enriches each chunk with LLM-generated context at ingestion time.

**3. SPA application** — Angular frontend with ~15 functional views enabling semantic search, chat with AI agents (SSE streaming + tool calling), graph exploration (Cytoscape), deadline and task management, and an administrative panel to control the ingestion pipeline.

---

## AI and RAG Capabilities

| Capability | Description |
|------------|-------------|
| **Hybrid Search** | BM25 + vectors (3072d) with Reciprocal Rank Fusion over 3 indexes in AI Search |
| **Contextual Retrieval** | Each chunk is enriched with LLM-generated context at ingestion time |
| **Tool Calling** | 13 tools available to the agent (legal norm search, rulings, graphs, communities, etc.) |
| **SSE Streaming** | Typed events: `text`, `tool_start`, `tool_end`, `validation`, `done` |
| **Input Guardrails** | 2 layers: rule-based + LLM classifier |
| **Output Guardrails** | Citation validation against the database |
| **Query Preprocessing** | GPT-5o-mini + expansion with the SAIJ thesaurus |
| **Community Detection** | Union-Find + clustering by law branch with LLM summarization |

---

## Ingestion Pipeline

```
Discoverer → Fetcher → Parser → Enrichment → Persister → Indexer
    ↓           ↓         ↓          ↓            ↓           ↓
 Discovers   Downloads  Extracts  Enriches     Persists    Generates
 docs from   PDFs/HTML  text +    with GPT-5o   to Azure   embeddings,
 sources     with cache metadata  -mini (NER,   SQL via    indexes in AI
             in Blob              metadata,     EF Core    Search, resolves
                                  classific.)              citations, extracts
                                                           mentions
```

Each stage communicates via Azure Storage Queues (5 queues). It includes a DLQ with an admin UI for retries and `DocumentStageLog` for full pipeline tracking.

---

## Roadmap

The project evolves from a mature MVP (~78% reusable code) through 5 releases:

| Release | Name | Focus |
|---------|------|-------|
| **R0.0** | Preparation | Restructure monorepo, scaffolding, CI/CD, infra, code quality |
| **R1.0** | Research & Monitoring | Auth, home, legislation/case-law search + tax sources, AI research assistant, regulatory/tax monitoring |
| **R2.0** | Professional Productivity | Projects/workspaces, document review & analysis, deliverable generation, project tasks & deadlines |
| **R3.0** | Knowledge & Intelligence | PwC internal KB, tax/regulatory/case-law agents, risk analysis, tax controversy (light tracking) |
| **R4.0** | Scale & Operations | Reporting, administration, graph explorer, assistant feedback, notifications, observability, PWA, delivery |

> The product is scoped as an **internal productivity tool for PwC tax-legal professionals** (advisory-first,
> organized by client projects/workspaces). See [`docs/roadmap/features.md`](docs/roadmap/features.md) for the
> canonical roadmap and [`docs/roadmap/PIVOT-PWC-TAX.md`](docs/roadmap/PIVOT-PWC-TAX.md) for the re-scope.

### Specialized Agents (R3.0)

- **Tax Agent** — Tax research and advisory queries: applicable regime, dictámenes/consultas vinculantes, computations and references
- **Regulatory Agent** — Queries about current legislation, validity verification, repeal chains
- **Case Law Agent** — Search and analysis of rulings, precedent identification, case-law trends

Orchestrated by a Hybrid Router (embedding similarity + LLM fallback) on Semantic Kernel with the ReAct pattern.

---

## Repository Structure

```
legal-ai-ar/
├── backend/
│   ├── src/
│   │   ├── api/
│   │   │   ├── LegalAiAr.Api/          # ASP.NET Core (Controllers → Minimal API)
│   │   │   └── LegalAiAr.Application/  # CQRS, handlers, services
│   │   ├── shared/
│   │   │   ├── LegalAiAr.Core/         # Entities, enums, interfaces
│   │   │   └── LegalAiAr.Infrastructure/ # EF Core, Azure services, AI
│   │   ├── workers/                     # 6 BackgroundService workers
│   │   └── tools/                       # 10 auxiliary CLI tools
│   ├── tests/                           # 8 test projects (xUnit + NSubstitute)
│   ├── LegalAiAr.sln
│   ├── Directory.Packages.props         # Central Package Management
│   └── global.json                      # .NET 10
├── frontend/
│   └── src/
│       └── app/
│           ├── core/                    # Auth, interceptors, singleton services
│           ├── features/                # Lazy-loaded feature modules
│           └── shared/                  # Common components, pipes, directives
└── docs/
    ├── roadmap/                          # Project planning
    │   ├── features.md                  # Full roadmap (v2.0)
    │   └── backlog.md                   # Feature/work-item inventory
    ├── technical/                        # 9 technical documents
    │   ├── 01-rag-retrieval.md
    │   ├── 02-agentic-architecture.md
    │   ├── 03-prompt-engineering.md
    │   ├── 04-ingestion-processing.md
    │   ├── 05-ai-quality-evaluation.md
    │   ├── 06-ai-security-compliance.md
    │   ├── 07-observability-llmops.md
    │   ├── 08-legal-ai-ux.md
    │   └── 09-data-knowledge-management.md
    └── ontology/                       # Legal domain model
        ├── argentine-legal-ontology.md
        ├── argentine-legal-ontology.mermaid
        └── ontology-data-sources.md
```

---

## Documentation

| Document | Description |
|----------|-------------|
| [Features Roadmap](docs/roadmap/features.md) | Full plan of releases, features, endpoints, KPIs, and tech stack |
| [Backlog](docs/roadmap/backlog.md) | Feature and work-item inventory with totals |
| [Argentine Legal Ontology](docs/ontology/argentine-legal-ontology.md) | Formal specification of the Argentine legal domain (classes, properties, relationships) |
| [Data Sources by Class](docs/ontology/ontology-data-sources.md) | Primary and secondary sources to populate each ontology class |
| [RAG & Retrieval](docs/technical/01-rag-retrieval.md) | Hybrid Search, GraphRAG, Contextual Retrieval, re-ranking, evaluation |
| [Agentic Architecture](docs/technical/02-agentic-architecture.md) | Multi-agent, semantic router, tool calling, reasoning, memory, guardrails |
| [Prompt Engineering](docs/technical/03-prompt-engineering.md) | Versioned templates, system prompts, few-shot, structured output |
| [Ingestion & Processing](docs/technical/04-ingestion-processing.md) | Metadata enrichment, classification, legal NER, deduplication, versioning |
| [AI Quality & Evaluation](docs/technical/05-ai-quality-evaluation.md) | Golden set, metrics, LLM-as-judge, HITL feedback, regression testing |
| [Security & Compliance](docs/technical/06-ai-security-compliance.md) | Content filtering, PII, data residency, attorney-client privilege |
| [Observability & LLMOps](docs/technical/07-observability-llmops.md) | Distributed tracing, token usage, semantic caching, circuit breaker |
| [Legal AI UX](docs/technical/08-legal-ai-ux.md) | Streaming, inline citation, confidence scores, feedback loops |
| [Data & Knowledge Management](docs/technical/09-data-knowledge-management.md) | Taxonomy, temporal versioning, graph maintenance, data lineage |

### Onboarding & Workflow

| Document | Description |
|----------|-------------|
| [Developer Onboarding](docs/onboarding/README.md) | Starting point: local setup for VS Code + Claude and Cursor (both with Docker) |
| [Recommended Extensions](docs/onboarding/recommended-extensions.md) | IDE extensions for VS Code and Cursor |
| [Troubleshooting](docs/onboarding/troubleshooting.md) | Common local-setup errors and fixes |
| [Developer Guide](docs/developer-guide.md) | Work-item → PR workflow, AI skills, conventions |
| [AppKit 4 (UI library)](docs/appkit4/README.md) | PwC AppKit 4 reference: components, design tokens, icons, patterns |
| [AI Setup Tutorial](docs/cowork-setup-tutorial.md) | How Cowork (Claude) and Cursor are configured |

### Delivery & Hosting

| Document | Description |
|----------|-------------|
| [GitHub Delivery](docs/deployment/github-delivery.md) | Source control, CI quality gates, CD to Azure staging (API + SPA) |
| [GCaaS Hosting](docs/deployment/gcaas-hosting.md) | PwC corporate hosting: Knative/Istio Helm deploy, Entra SSO (`id_token` cookie), Vault secrets, session model |

> Both delivery paths and the platform auth model are summarized in [features.md §2.3](docs/roadmap/features.md) and tracked as feature **FT05 — Delivery and Hosting**.

---

## Getting Started (Developers)

New to the project? Start with the **[Developer Onboarding guide](docs/onboarding/README.md)** — a
single starting point that takes you from a fresh clone to the app running locally, for either
supported environment:

- **VS Code + Claude + Docker**
- **Cursor + Docker**

It covers prerequisites, Docker-based local dependencies, backend/frontend setup, IDE configuration,
[recommended extensions](docs/onboarding/recommended-extensions.md), and
[troubleshooting](docs/onboarding/troubleshooting.md). For the day-to-day work-item → PR flow, see the
[Developer Guide](docs/developer-guide.md).

---

## Requirements

- .NET 10 SDK
- Node.js 20 LTS+ (Angular CLI installed locally via `npm ci`)
- Docker Desktop (run the app — API, SPA, workers — in containers)
- Access to the shared **cloud DEV** services (Azure SQL, AI Search, OpenAI, Storage) — there is no local stack
- Azure Entra ID (platform-managed SSO; see [GCaaS Hosting](docs/deployment/gcaas-hosting.md))

> Full step-by-step setup is in the [Developer Onboarding guide](docs/onboarding/README.md).
