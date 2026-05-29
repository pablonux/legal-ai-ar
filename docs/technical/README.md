# Techniques, Technologies, and Best Practices — Legal Ai Ar

> Technical documentation of the design decisions, patterns, and practices adopted for the Legal Knowledge Base system with AI.

---

## Document Index

| # | Document | Scope |
|---|----------|-------|
| 01 | [RAG & Retrieval](./01-rag-retrieval.md) | Hybrid Search, GraphRAG, Contextual Retrieval, re-ranking, query expansion, retrieval evaluation |
| 02 | [Agentic Architecture](./02-agentic-architecture.md) | Multi-agent patterns, semantic router, tool calling, reasoning, memory, guardrails |
| 03 | [Prompt Engineering & Management](./03-prompt-engineering.md) | Versioned templates, system prompts, few-shot, structured output, anti-hallucination, registry |
| 04 | [Document Ingestion & Processing](./04-ingestion-processing.md) | Metadata enrichment, automatic classification, legal NER, deduplication, validation, versioning |
| 05 | [AI Evaluation & Quality](./05-ai-quality-evaluation.md) | Golden set, metrics, LLM-as-judge, HITL feedback, regression testing, drift monitoring |
| 06 | [AI Security & Compliance](./06-ai-security-compliance.md) | Content filtering, PII, data residency, rate limiting, responsible AI, attorney-client privilege |
| 07 | [Observability & LLMOps](./07-observability-llmops.md) | Distributed tracing, token usage, semantic caching, circuit breaker, model versioning |
| 08 | [Legal AI UX](./08-legal-ai-ux.md) | Streaming, inline citation, confidence scores, explainability, feedback loops |
| 09 | [Data & Knowledge Management](./09-data-knowledge-management.md) | Controlled taxonomy, temporal versioning, graph maintenance, data lineage, consistency |
| 10 | [System Architecture](./10-system-architecture.md) | ⚠️ *Imported, pending review.* System overview, ingestion design, data model, messaging, workers, infra, ADRs, risks |
| 11 | [Technical Specs](./11-technical-specs.md) | ⚠️ *Imported, pending review.* Repository structure, Clean Architecture, CQRS, data model, API, frontend, testing, ADRs |
| 12 | [C4 Diagrams](./12-c4-diagrams.md) | ⚠️ *Imported, pending review.* C4 context/container/component diagrams (sources in [`diagrams/`](./diagrams/)) |
| 13 | [SAIJ Thesaurus Ingestion](./13-saij-thesaurus-ingestion.md) | The SAIJ thesaurus pipeline as implemented: TemaTres crawl, relations, synonym maps, keyword normalization, query expansion, API |
| 14 | [CSJN Ruling Ingestion](./14-csjn-ruling-ingestion.md) | The CSJN *fallos* ingestion pipeline as implemented: discovery sources, sjconsulta API, parsing, enrichment, indexing, config |
| 15 | [SAIJ Web Ingestion](./15-saij-web-ingestion.md) | SAIJ open-data API ingestion of legislation (statutes) and jurisprudence: discovery, self-contained flow (no LLM), data model, classification |
| 16 | [Chat, RAG & Agent Pipeline](./16-chat-rag-agents.md) | Agentic RAG chat as implemented: SSE streaming, guardrail pipeline, tool-calling loop, 13 tools, hybrid + GraphRAG retrieval |
| 17 | [Knowledge Base Data Model](./17-kb-data-model.md) | The ~44 EF Core entities in Azure SQL: legal content, actors, citation/graph layer, ingestion bookkeeping, provenance & audit |
| 18 | [Frontend Architecture](./18-frontend-architecture.md) | The Angular SPA as implemented: structure, routing/navigation, cookie auth, SSE chat client, Cytoscape graph explorer, environments |
| 19 | [Admin & Pipeline Operations](./19-admin-pipeline-operations.md) | Admin API, job tracking & traceability, SignalR worker control (pause/resume), DLQ & requeue, failure recovery, ruling reprocess |
| 20 | [Legal Data Sources Catalog](./20-legal-data-sources.md) | Catalog of known Argentine legal data sources (jurisprudence portals, administrative dictámenes, legislation) with URLs and integration status |

> Docs **10–12** are imported from the previous documentation set and are **drafts pending review** — not yet aligned to current naming, the cloud-only environment, or the platform `id_token` auth model. Do not treat them as definitive until revised.

---

## Conventions

Each document follows this structure:

1. **Description** — What it is and why it is relevant to Legal Ai Ar
2. **Technical Decisions** — What we chose and why (with a comparison table of alternatives)
3. **Architecture** — Mermaid diagrams of the solution
4. **Concrete Example** — Applied to the Argentine legal domain
5. **Items to Define** — Checklist of what is still to be resolved
6. **References** — Links to official documentation and relevant papers

---

## Relationship with the Roadmap

These documents complement the [Comprehensive Documentation (F00-W01)](../roadmap/F00%20-%20Development%20Environment%20and%20Structure/F00%20-%20W01%20-%20Comprehensive%20Documentation.md) and serve as a technical reference for developers when implementing the roadmap features.

---

*Legal Ai Ar — Technical Documentation*
