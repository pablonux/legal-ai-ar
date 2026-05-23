# Azure Infrastructure Catalog — Legal AI AR

| Field | Value |
|---|---|
| **ID** | E008 |
| **Feature** | F0-2 · Azure Infrastructure — existing services |
| **Date** | 2026-03-09 |

---

## Purpose

This document is the catalog of Azure resources that Legal AI AR uses in Phase 1. The services are **already created and available**; no provisioning is orchestrated with Bicep or Terraform, nor are resource groups or new resources created. It defines service, tier, purpose and key configuration of each component. It serves as a reference for tasks T-01 through T-11 of F0-2 (configuration and connection) and for environment variable documentation (E010). It is consumed by DevOps, developers and the infrastructure team.

---

## 1. Summary by Component

| # | Component | Azure Service | Phase 1 Tier |
|---|---|---|---|
| 1 | Relational database | Azure SQL Database | General Purpose 2 vCores (serverless) |
| 2 | File storage | Azure Blob Storage | LRS Standard |
| 3 | Semantic search | Azure AI Search | Basic |
| 4 | LLM and embeddings | Azure OpenAI Service | gpt-4o + text-embedding-3-large |
| 5 | Messaging | Azure Storage Queues (same Storage Account as Blob) | Included in Blob |
| 6 | API hosting | Azure App Service | B2 |
| 7 | SPA hosting | Azure Static Web Apps | Free / Standard |
| 8 | Relationship graph | Azure SQL (Citations table, recursive CTEs) | Included in SQL |
| 9 | Identity | Azure Entra ID | Microsoft 365 (existing) |
| 10 | Workers | Azure Container Apps | Consumption plan |

---

## 2. Resource Details

### 2.1 Azure SQL Database

| Attribute | Value |
|---|---|
| **Service** | Azure SQL Database |
| **Tier** | General Purpose |
| **Compute** | 2 vCores (serverless) |
| **Region** | Defined in existing service |
| **Purpose** | Persistence of relational tables: Rulings, Courts, Judges, Keywords, Statutes, Citations, Sources, CrawlerConfigs. EF Core migrations for schema. |
| **Key configuration** | Connection string via environment variable. Firewall: allow IPs from App Service, Container Apps and development machines. |

**Reference**: Architecture section 4 (data model), ADR-001.

---

### 2.2 Azure Blob Storage

| Attribute | Value |
|---|---|
| **Service** | Azure Blob Storage |
| **Redundancy** | LRS (Locally Redundant Storage) Standard |
| **Region** | Defined in existing service |
| **Purpose** | Storage of ruling PDFs. CrawlerWorker uploads PDFs; IndexerWorker persists references in Rulings.BlobPath. |
| **Key configuration** | Container `rulings-pdfs`. Connection string and container name via environment variables. Access: API, IndexerWorker, ParserWorker (read). |

**Reference**: Architecture section 3.2 (CrawlerWorker uploads PDF to Blob), section 4.1 (Rulings.BlobPath).

---

### 2.3 Azure AI Search

| Attribute | Value |
|---|---|
| **Service** | Azure AI Search |
| **Tier** | Basic |
| **Region** | Defined in existing service |
| **Purpose** | Hybrid semantic search (vector + keyword). Indexes: `rulings-by-ruling` (ruling level) and `rulings-by-chunk` (chunk level). Embeddings: text-embedding-3-large (3072 dims). |
| **Key configuration** | Endpoint, API key, index names via environment variables. Indexes created with EF Core or initialization script. Consumed by: API (search, RAG chat), IndexerWorker (indexing). |

**Reference**: Architecture section 4.12 (index schema), ADR-002 (chunking).

---

### 2.4 Azure Storage Queues (Phase 1 — replaces Service Bus)

| Attribute | Value |
|---|---|
| **Service** | Azure Storage Queues |
| **Location** | Same Storage Account as Blob Storage |
| **Region** | Defined in existing service |
| **Purpose** | Ingestion pipeline queues. 4 queues: `queue-crawler`, `queue-parser`, `queue-enrichment`, `queue-indexer`. No native DLQ; implement `{name}-dlq` queue if required. |
| **Key configuration** | Connection string shared with Blob (`AzureBlob__ConnectionString`). KEDA: use Azure Queue Storage scaler instead of Service Bus. Consumed by: API (publish), all 4 workers. |

**Reference**: Architecture section 8 (messaging), ADR-009 (updated for Phase 1).

---

### 2.5 Azure OpenAI Service

| Attribute | Value |
|---|---|
| **Service** | Azure OpenAI Service |
| **Models** | gpt-4o (LLM), text-embedding-3-large (3072 dims) |
| **Region** | Defined in existing service |
| **Purpose** | Embeddings for indexing and semantic search. LLM for enrichment (judge extraction, statutes, citation classification) and RAG chat. Data remains in the resource's region. |
| **Key configuration** | Endpoint, API key, deployment names (chat and embedding) via environment variables. Consumed by: API (search, RAG chat), EnrichmentWorker, IndexerWorker. |

**Reference**: Architecture section 7.1 (Azure OpenAI Service), ADR-014.

---

### 2.6 Azure App Service

| Attribute | Value |
|---|---|
| **Service** | Azure App Service |
| **Tier** | B2 |
| **Region** | Defined in existing service |
| **Purpose** | ASP.NET Core API hosting. Exposes REST endpoints and SSE for search, RAG chat and administration. |
| **Key configuration** | Staging + production deployment slot. Environment variables for SQL, Search, Blob, Neo4j, Entra ID, Azure OpenAI. CORS configured for SPA domain. |

**Reference**: Architecture section 5 (API).

---

### 2.7 Azure Static Web Apps

| Attribute | Value |
|---|---|
| **Service** | Azure Static Web Apps |
| **Tier** | Free / Standard |
| **Region** | Global (CDN) |
| **Purpose** | Angular SPA hosting. Includes main routes (/search, /rulings, /chat) and admin panel (/admin/*). |
| **Key configuration** | Build from `/frontend`. Entra ID redirect URIs must include the deployed SPA URL. |

**Reference**: Architecture section 6 (Angular SPA), ADR-012.

---

### 2.8 Relationship Graph (Phase 1 — Azure SQL)

| Attribute | Value |
|---|---|
| **Service** | Azure SQL Database (same as section 2.1) |
| **Purpose** | Graph modeled in relational tables: Citations (SourceRulingId, TargetRulingId, CitationType), RulingJudges, RulingKeywords, RulingStatutes. Timeline and ranking queries via recursive CTEs. |
| **Key configuration** | No additional infrastructure. SqlGraphService implements IGraphService as no-op (data already persisted by Indexer via repositories). |

**Phase 2**: Evaluate migration to Neo4j on VM if graph query volume justifies it.

---

### 2.9 Azure Entra ID

| Attribute | Value |
|---|---|
| **Service** | Azure Entra ID (Microsoft Entra ID) |
| **Origin** | Firm's Microsoft 365 (already available) |
| **Purpose** | User authentication. SSO. JWT token issuance for the API. Phase 1: all authenticated users have admin role. |
| **Key configuration** | App registration with redirect URIs for SPA (local and production). Scopes: `api://{client-id}/.default` or equivalent. Tenant ID, Client ID, Audience in environment variables. |

**Reference**: Architecture section 6 (authentication), ADR-003, ADR-013.

---

### 2.10 Azure Container Apps

| Attribute | Value |
|---|---|
| **Service** | Azure Container Apps |
| **Plan** | Consumption |
| **Region** | Defined in existing service |
| **Purpose** | Hosting of the 4 pipeline workers. Shared environment. KEDA + Azure Queue Storage scaler for auto-scaling. Scale to 0 when queues are empty. |
| **Key configuration** | 4 container apps in the same environment. Images from Azure Container Registry. Environment variables per worker. |

**Per-worker details**:

| Worker | Resources | Scale |
|---|---|---|
| crawler-worker | 0.5 vCPU / 1 GB RAM | min 0, max 1 |
| parser-worker | 0.5 vCPU / 1 GB RAM | min 0, max 2 |
| enrichment-worker | 1 vCPU / 2 GB RAM | min 0, max 2 |
| indexer-worker | 1 vCPU / 2 GB RAM | min 0, max 2 |

**Reference**: Architecture section 9 (Workers), ADR-010.

---

## 3. Local Environment (Development)

| Component | Origin | Purpose |
|---|---|---|
| SQL Server | docker-compose (mcr.microsoft.com/mssql/server:2022-latest) | Azure SQL emulation for local development. Port 1433. |

**Note**: Azure Blob Storage (includes Storage Queues), Azure AI Search and Azure OpenAI are used directly from Azure in development (free or basic tiers). Not emulated locally. Neo4j is not used in Phase 1.

**Reference**: Specs section 9.2, roadmap F0-2 T-11.

---

## 4. Summary of Decisions

| # | Decision | Justification |
|---|---|---|
| 1 | Existing Azure services (no provisioning) | Resources are already created; the project only configures connection and deploys applications |
| 2 | Azure OpenAI (not direct OpenAI API) | ADR-014: data in Azure region, compliance, stack integration |
| 3 | Graph in SQL (Phase 1) | No VM for Neo4j; Citations table + recursive CTEs. Phase 2: evaluate Neo4j |
| 4 | Container Apps consumption | $0 cost at rest; auto-scaling by queues (ADR-010) |
| 5 | Existing tenant's Entra ID | No new tenant created; firm's Microsoft 365 is reused |

---

## References

- `docs/architecture/legal-ai-ar-architecture.md` — section 7 (Azure Infrastructure), sections 4, 8, 9
- `docs/architecture/legal-ai-ar-specs.md` — section 9 (Configuration and infrastructure)
- `docs/roadmap/ROADMAP.md` — F0-2, tasks T-01 through T-11
