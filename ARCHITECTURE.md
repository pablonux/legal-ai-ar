# Arquitectura General — Legal AI AR

## 1. Visión del Sistema

Legal AI AR es una plataforma modular diseñada para escalar desde la ingesta de fallos judiciales hasta una base de conocimiento jurídico completa del sistema argentino. La arquitectura sigue principios de **separación de responsabilidades**, **escalabilidad horizontal** y **extensibilidad**, desplegada íntegramente sobre **Microsoft Azure** con backend **.NET 8**.

---

## 2. Diagrama de Alto Nivel

```
                    ┌─────────────────────────────────────────────┐
                    │              FUENTES DE DATOS                │
                    │  SAIJ │ PJN │ SCBA │ CSJN │ Tribunales...   │
                    └──────────────────┬──────────────────────────┘
                                       │ HTTP scraping / APIs
                                       ▼
┌──────────────────────────────────────────────────────────────────┐
│              WORKERS  (.NET 8 Worker Services)                   │
│                                                                  │
│  ┌────────────┐  ┌────────────┐  ┌──────────────┐  ┌─────────┐ │
│  │  Crawler   │  │  Parser    │  │  Enrichment  │  │ Indexer │ │
│  │  Worker    │→ │  Worker    │→ │  Worker      │→ │ Worker  │ │
│  └────────────┘  └────────────┘  └──────────────┘  └─────────┘ │
│                  ◄──── Azure Service Bus (queues) ────────────►  │
└──────────────────────────────────┬───────────────────────────────┘
                                   │
                                   ▼
┌──────────────────────────────────────────────────────────────────┐
│                     KB CORE (Knowledge Base)                     │
│                                                                  │
│  ┌──────────────────┐  ┌─────────────────┐                       │
│  │  Azure Storage   │  │  Azure SQL DB   │                       │
│  │  Account (Blob)  │  │  + EF Core 8    │                       │
│  │  Docs originales │  │  Metadata       │                       │
│  └──────────────────┘  └─────────────────┘                       │
│                                                                  │
│  ┌──────────────────┐  ┌─────────────────┐                       │
│  │  Azure AI Search │  │  Neo4j CE       │                       │
│  │  Vector + FTS    │  │  Graph DB       │                       │
│  └──────────────────┘  └─────────────────┘                       │
│                                                                  │
│              Azure OpenAI Service (embeddings + LLM)            │
└──────────────────────────────────┬───────────────────────────────┘
                                   │
                                   ▼
┌──────────────────────────────────────────────────────────────────┐
│                    API LAYER (ASP.NET Core 8)                    │
│                    REST API — versionada                         │
└─────────┬────────────────────────────────────────┬──────────────┘
          │                                         │
          ▼                                         ▼
┌──────────────────┐                    ┌──────────────────────┐
│  ASP.NET Core    │                    │   Angular 17+ SPA    │
│  MVC (Admin)     │                    │   (TypeScript)       │
└──────────────────┘                    └──────────────────────┘

Todo desplegado en Azure Container Apps
Secretos gestionados con Azure Key Vault
```

---

## 3. Servicios Azure y su Rol

| Servicio | Propósito | Tier recomendado |
|---|---|---|
| **Azure Storage Account** | Blob Storage para documentos originales | Standard LRS / GRS |
| **Azure SQL Database** | Metadata estructurada, entidades, relaciones | General Purpose — 4 vCores |
| **Azure AI Search** | Índice vectorial + full-text sobre fallos | Standard S1 |
| **Azure OpenAI Service** | `text-embedding-3-large` + `gpt-4o` | Pay-per-use |
| **Azure Service Bus** | Queues entre workers del pipeline | Standard |
| **Azure Container Apps** | Hosting de todos los servicios | Consumption plan |
| **Azure Key Vault** | Secretos, connection strings, API keys | Standard |
| **Azure Monitor** | Logs, métricas, alertas | Incluido |

---

## 4. Componentes de Software

### 4.1 KB Core

Abstracción sobre las 4 capas de almacenamiento. Expuesta como librería interna (`LegalAiAr.KbCore`) consumida por Workers y API.

Ver documento dedicado: [KB-CORE.md](KB-CORE.md)

### 4.2 Workers (.NET 8 Worker Services)

Servicios de background que implementan el pipeline de ingesta. Se comunican vía **Azure Service Bus**.

Ver documento dedicado: [WORKERS.md](WORKERS.md)

### 4.3 API (ASP.NET Core 8 Web API)

- Endpoints REST versionados (`/api/v1/...`)
- Autenticación con **Azure AD / JWT Bearer**
- Búsqueda semántica delegada a Azure AI Search
- Búsqueda por grafo delegada a Neo4j
- Documentación con **Swagger / OpenAPI**

### 4.4 MVC Admin (ASP.NET Core 8 MVC)

- Panel de administración server-side
- Razor Pages + Bootstrap
- Gestión de fuentes, workers, entidades
- Monitoreo del pipeline

### 4.5 SPA (Angular 17+)

- Búsqueda inteligente de fallos
- Visualización de grafos de relaciones
- Timeline de jurisprudencia
- Dashboard analítico
- Autenticación con **MSAL (Azure AD)**

---

## 5. Estructura de Solución .NET

```
LegalAiAr.sln
├── src/
│   ├── LegalAiAr.KbCore/              # Librería core KB (abstracciones + clientes)
│   │   ├── Storage/                   # Azure Blob Storage client
│   │   ├── Relational/                # EF Core DbContext + Repositories
│   │   ├── Search/                    # Azure AI Search client
│   │   └── Graph/                     # Neo4j client
│   │
│   ├── LegalAiAr.Domain/              # Entidades de dominio, enums, interfaces
│   ├── LegalAiAr.Application/         # CQRS commands/queries (MediatR)
│   ├── LegalAiAr.Infrastructure/      # Implementaciones concretas
│   │
│   ├── LegalAiAr.Api/                 # ASP.NET Core 8 Web API
│   ├── LegalAiAr.Mvc/                 # ASP.NET Core 8 MVC Admin
│   │
│   └── LegalAiAr.Workers/
│       ├── CrawlerWorker/
│       ├── ParserWorker/
│       ├── EnrichmentWorker/
│       └── IndexerWorker/
│
├── tests/
│   ├── LegalAiAr.UnitTests/
│   └── LegalAiAr.IntegrationTests/
│
└── spa/                               # Angular project
```

---

## 6. Principios de Diseño

1. **KB como fuente única de verdad**: Todos los componentes leen de la KB.
2. **Clean Architecture**: Domain → Application → Infrastructure → Presentation.
3. **CQRS con MediatR**: Commands (escritura) y Queries (lectura) separados.
4. **Idempotencia en ingesta**: Workers pueden re-procesar sin duplicar datos.
5. **Extensibilidad de tipos documentales**: Nuevos tipos sin migraciones destructivas.
6. **Trazabilidad**: Cada entidad mantiene linaje completo (fuente, fecha, versión, worker).
7. **Azure-native**: Aprovechar servicios administrados para reducir operaciones.
