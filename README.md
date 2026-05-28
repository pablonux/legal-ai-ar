# Legal Ai Ar

> Legal Knowledge Base with AI Agents

Legal Ai Ar is a system that centralizes access to legislation, case law, and legal doctrine of the Argentine legal system, combining a multidimensional knowledge base with specialized AI agents. It is designed to reduce legal research time from hours to minutes and to eliminate missed procedural deadlines.

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

**3. SPA application** — Angular frontend with ~15 functional views enabling semantic search, chat with AI agents (SSE streaming + tool calling), graph exploration (Cytoscape), case file and deadline management, and an administrative panel to control the ingestion pipeline.

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

| Release | Name | Focus | Duration |
|---------|------|-------|----------|
| **R0.0** | Preparation | Restructure monorepo, add missing entities, code quality | 2 weeks |
| **R1.0** | Foundation | Auth, dashboard, search, legal norm/case law detail, basic chat, graph | 6 weeks |
| **R2.0** | Agents | Semantic Kernel, 3 specialized agents, case files, deadlines, calendar, eval | 6 weeks |
| **R3.0** | Risk | Legal risk analysis, report generation, operational reports | 4 weeks |
| **R4.0** | Operations | Observability (OpenTelemetry), advanced alerts, PWA, model versioning | 4 weeks |

### Specialized Agents (R2.0)

- **Regulatory Agent** — Queries about current legislation, validity verification, repeal chains
- **Case Law Agent** — Search and analysis of rulings, precedent identification, case law trends
- **Procedural Agent** — Queries about case files and deadlines, business-day calculation, due-date alerts

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
    │   └── gap-analysis-mvp-vs-plan.md  # MVP vs. plan analysis
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
| [Gap Analysis MVP vs Plan](docs/roadmap/gap-analysis-mvp-vs-plan.md) | Detailed comparison of the existing MVP vs. the plan, with an evolution strategy |
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

---

## Requirements

- .NET 10 SDK
- Node.js 20+ and Angular CLI 19
- Azure subscription with: SQL Database, AI Search, OpenAI Service, Storage Account, App Service
- Azure Entra ID (authentication)
