# Environment Variables — Legal AI AR

| Field | Value |
|---|---|
| **ID** | E010 |
| **Feature** | F0-2 · Azure Infrastructure — existing services |
| **Date** | 2026-03-09 |

---

## Purpose

This document defines the exhaustive table of environment variables per component (API and each worker). It serves as a reference for `.env.example` (E018) and E019 (connectivity verification), for configuration in Azure App Service and Container Apps, and for local development. It is consumed by developers and DevOps.

---

## 1. Conventions

- **.NET format**: `Section__Key` (double underscore). Example: `AzureSql__ConnectionString`.
- **Secrets**: Never include real values in the repository. Use environment variables or Azure Key Vault in production.
- **Local development**: Example values in `.env.example`; `appsettings.Development.json` may reference variables with placeholder values.

---

## 2. Matrix by Component

| Variable | API | Crawler | Parser | Enrichment | Indexer |
|---|---|:---:|:---:|:---:|:---:|
| AzureSql__ConnectionString | ✓ | ✓ | | | ✓ |
| AzureBlob__ConnectionString | ✓ | ✓ | ✓ | | ✓ |
| AzureBlob__ContainerName | ✓ | ✓ | ✓ | | ✓ |
| AzureOpenAI__Endpoint | ✓ | | | ✓ | ✓ |
| AzureOpenAI__ApiKey | ✓ | | | ✓ | ✓ |
| AzureOpenAI__ChatDeploymentName | ✓ | | | ✓ | |
| AzureOpenAI__EmbeddingDeploymentName | ✓ | | | | ✓ |
| AzureSearch__Endpoint | ✓ | | | | ✓ |
| AzureSearch__ApiKey | ✓ | | | | ✓ |
| AzureSearch__RulingIndexName | ✓ | | | | ✓ |
| AzureSearch__ChunkIndexName | ✓ | | | | ✓ |
| AzureAd__TenantId | ✓ | | | | |
| AzureAd__ClientId | ✓ | | | | |
| AzureAd__Audience | ✓ | | | | |

---

## 3. Variable Details

### 3.1 Shared (multiple components)

| Variable | Type | Description | Example |
|---|---|---|---|
| `AzureSql__ConnectionString` | string | Azure SQL Database connection string. Includes Server, Database, User ID, Password. | `Server=tcp:legal-ai-sql.database.windows.net,1433;Database=legalaiar;...` |
| `AzureBlob__ConnectionString` | string | Azure Storage Account connection string. Used for Blob (PDFs) and Storage Queues (pipeline). Same account for both. | `DefaultEndpointsProtocol=https;AccountName=legalaiarblob;...` |
| `AzureBlob__ContainerName` | string | PDF container name. | `rulings-pdfs` |

**Phase 1**: Messaging via Azure Storage Queues (same Storage Account as Blob). No Azure Service Bus nor Neo4j. Graph modeled in Azure SQL (Citations table, recursive CTEs).

### 3.2 Azure OpenAI

| Variable | Type | Description | Example |
|---|---|---|---|
| `AzureOpenAI__Endpoint` | string | Azure OpenAI resource endpoint. | `https://legal-ai-openai.openai.azure.com/` |
| `AzureOpenAI__ApiKey` | string | Azure OpenAI resource API key. | *(secret)* |
| `AzureOpenAI__ChatDeploymentName` | string | gpt-4o (chat) deployment name. | `gpt-4o` |
| `AzureOpenAI__EmbeddingDeploymentName` | string | text-embedding-3-large deployment name. | `text-embedding-3-large` |

### 3.3 Azure AI Search

| Variable | Type | Description | Example |
|---|---|---|---|
| `AzureSearch__Endpoint` | string | Azure AI Search endpoint. | `https://legal-ai-search.search.windows.net` |
| `AzureSearch__ApiKey` | string | Azure AI Search API key. | *(secret)* |
| `AzureSearch__RulingIndexName` | string | Ruling-level index name. | `rulings-by-ruling` |
| `AzureSearch__ChunkIndexName` | string | Chunk-level index name. | `rulings-by-chunk` |

### 3.4 Azure Entra ID (API only)

| Variable | Type | Description | Example |
|---|---|---|---|
| `AzureAd__TenantId` | string | Azure Entra ID tenant ID (firm's Microsoft 365). | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `AzureAd__ClientId` | string | Client ID of the app registered in Entra ID. | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `AzureAd__Audience` | string | Expected audience in the JWT token. | `api://legal-ai-ar` |

---

## 4. Variables by Component — Summary

### API (App Service)

All variables except worker-specific ones. Total: 18 variables.

### CrawlerWorker

- AzureSql__ConnectionString (deduplication by ContentHash)
- AzureBlob__ConnectionString (Blob + Storage Queues)
- AzureBlob__ContainerName
- CsjnCrawler__ThrottlingDelayMs (R-002: delay between CSJN requests, default 2000)
- CsjnCrawler__ThrottlingBackoffMultiplier (R-002: exponential backoff on 429, default 2.0)
- CsjnCrawler__ThrottlingMaxRetries (R-002: max retries per request when rate limited, default 3)

### ParserWorker

- AzureBlob__ConnectionString (Storage Queues + Blob)
- AzureBlob__ContainerName

### EnrichmentWorker

- AzureBlob__ConnectionString (Storage Queues)
- AzureOpenAI__Endpoint
- AzureOpenAI__ApiKey
- AzureOpenAI__ChatDeploymentName

### IndexerWorker

- AzureSql__ConnectionString
- AzureBlob__ConnectionString (Blob + Storage Queues)
- AzureBlob__ContainerName
- AzureOpenAI__Endpoint, ApiKey, EmbeddingDeploymentName
- AzureSearch__Endpoint, ApiKey, RulingIndexName, ChunkIndexName

---

## 5. Local Development

For local development, use `docker-compose.yml` for SQL Server. Variables point to local instances:

| Variable | Local value |
|---|---|
| `AzureSql__ConnectionString` | `Server=localhost,1433;Database=LegalAiAr;User Id=sa;Password=Dev_Password123!;TrustServerCertificate=True` |

Azure Blob Storage (includes Storage Queues), AI Search and Azure OpenAI are used directly from Azure (free or basic tiers). Not emulated locally. Neo4j is not used in Phase 1 (graph in SQL).

---

## 6. CrawlerWorker — CSJN Throttling (R-002)

| Variable | Type | Description | Default |
|---|---|---|---|
| `CsjnCrawler__ThrottlingDelayMs` | int | Minimum delay in ms between consecutive CSJN requests (discovery, PDF download). | 2000 |
| `CsjnCrawler__ThrottlingBackoffMultiplier` | double | Multiplier for exponential backoff on 429. Delay = ThrottlingDelayMs × (Multiplier ^ attempt). | 2.0 |
| `CsjnCrawler__ThrottlingMaxRetries` | int | Maximum retries per request when rate limited. | 3 |

**Reference**: E029 (`docs/design/f1-2-crawler.md`).

---

## 7. Future Variables (Phase 2)

*(Reserved for per-source throttling in CrawlerConfigs, Neo4j, etc.)*

---

## References

- `docs/architecture/legal-ai-ar-specs.md` — section 9.1 (Environment variables)
- `docs/design/f0-2-infrastructure.md` — configuration per resource
- `docs/roadmap/ROADMAP.md` — E018 (`.env.example`), E019 (connectivity verification)
