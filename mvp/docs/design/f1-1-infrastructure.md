# Decisiones de implementación — LegalAiAr.Infrastructure

| Campo | Valor |
|---|---|
| **ID** | E020 |
| **Feature** | F1-1 · Shared Infrastructure — repositorios y servicios base |
| **Fecha** | 2026-03-10 |

---

## Propósito

Este documento captura las decisiones de implementación por proveedor de infraestructura en `LegalAiAr.Infrastructure`. Define cómo se implementan EF Core, Azure AI Search, Blob Storage, Storage Queues, Azure OpenAI y el servicio de grafo. Sirve como guía para las tareas T-01 a T-09 de F1-1 y como referencia para desarrolladores que extiendan o mantengan los proveedores. Lo consumen el equipo de desarrollo y los implementadores de los workers.

---

## 1. EF Core — Azure SQL

### 1.1 Decisiones

| Decisión | Detalle |
|---|---|
| **DbContext** | `AppDbContext` hereda de `DbContext`. Registrado como `DbContextOptions<AppDbContext>` con connection string desde `AzureSql__ConnectionString`. |
| **Configuraciones** | Cada entidad tiene su `IEntityTypeConfiguration<T>` en `Persistence/Configurations/`. Fluent API para relaciones, índices y constraints. |
| **Migrations** | EF Core Migrations en `Persistence/Migrations/`. Comando `dotnet ef migrations add` desde el proyecto Infrastructure. |
| **Retry** | Habilitar `EnableRetryOnFailure` con max 3 intentos y delay exponencial para resiliencia ante fallos transitorios de red. |
| **Sensitive data** | `EnableSensitiveDataLogging` solo en Development. Nunca en producción. |

### 1.2 Tablas y relaciones

Todas las tablas definidas en la arquitectura (sección 4): `Rulings`, `Courts`, `Judges`, `RulingJudges`, `Keywords`, `RulingKeywords`, `Statutes`, `RulingStatutes`, `Citations`, `Sources`, `CrawlerConfigs`. FK explícitas y índices según modelo de datos.

**Reference**: `docs/architecture/legal-ai-ar-architecture.md` sections 4.1–4.15.

---

## 2. Azure AI Search

### 2.1 Decisiones

| Decisión | Detalle |
|---|---|
| **Cliente** | `SearchClient` de `Azure.Search.Documents`. Un cliente por índice o cliente compartido con índices nombrados. |
| **Búsqueda híbrida** | Vector search (embedding) + keyword search (fulltext). Combinación configurable (ej. 70% vector, 30% keyword o scoring híbrido nativo de Azure AI Search). |
| **Embeddings** | Vectores de 3072 dimensiones (`text-embedding-3-large`). Campo `embedding` como `Collection(Single)` en ambos índices. |
| **Índices** | `rulings-by-ruling` y `rulings-by-chunk`. Nombres configurables vía `AzureSearch__RulingIndexName` y `AzureSearch__ChunkIndexName`. |
| **Upsert** | `IndexDocumentsAsync` con `IndexDocumentsAction.MergeOrUpload` para idempotencia. |

### 2.2 Interfaces

- `ISearchService`: `SearchAsync(embedding, filters, page, pageSize)` → resultados paginados con `RulingSearchResultDto`.
- `SearchService` implementa búsqueda dual (chunks + rulings) según flujo del chat RAG (arquitectura sección 5).

**Referencia**: ADR-002 (chunking), arquitectura sección 4.12 (esquema de índices).

---

## 3. Azure Blob Storage

### 3.1 Decisiones

| Decisión | Detalle |
|---|---|
| **Cliente** | `BlobServiceClient` de `Azure.Storage.Blobs`. Connection string desde `AzureBlob__ConnectionString`. |
| **Container** | Nombre desde `AzureBlob__ContainerName` (ej. `rulings-pdfs`). Crear container si no existe en startup (opcional, según política de aprovisionamiento). |
| **Rutas** | Estructura `{source}/{year}/{documentId}.pdf` (ej. `csjn/2024/8048522.pdf`). Definida por CrawlerWorker al subir. |
| **Operaciones** | `UploadAsync`, `DownloadAsync`, `DeleteAsync` (si aplica). `IBlobStorageService` expone `UploadPdfAsync`, `GetPdfStreamAsync`. |
| **Content type** | `application/pdf` para PDFs. |

**Referencia**: Arquitectura sección 3.2 (CrawlerWorker sube PDF), sección 4.1 (Rulings.BlobPath).

---

## 4. Azure Storage Queues (mensajería)

### 4.1 Decisiones

| Decisión | Detalle |
|---|---|
| **Cliente** | `QueueClient` de `Azure.Storage.Queues`. Mismo Storage Account que Blob (`AzureBlob__ConnectionString`). |
| **Colas** | 4 colas por pipeline: `{prefix}-crawler`, `{prefix}-parser`, `{prefix}-enrichment`, `{prefix}-indexer`. Prefix default: `csjn-ruling`. Configurable via `Pipeline__QueuePrefix`. |
| **Formato** | Mensajes JSON serializados. Contratos en `LegalAiAr.Core/Messages/` (CrawlerMessage, ParserMessage, EnrichmentMessage, IndexerMessage). |
| **DLQ** | Colas `{prefix}-{stage}-dlq` para mensajes fallidos. Storage Queues no tiene DLQ nativa; implementar lógica de reencolado manual tras N intentos. |
| **Interfaz** | `IQueuePublisher`: `PublishAsync(queueName, message, ct)`. Consumidores en cada worker (no en Infrastructure; cada worker tiene su propio loop de consumo). |

### 4.2 Nota sobre Service Bus

Fase 1 usa **Azure Storage Queues** exclusivamente (ADR-009). No se implementa `ServiceBusQueuePublisher`. El nombre histórico en specs puede referirse al patrón de publicación; la implementación concreta es con `QueueClient`.

**Referencia**: Arquitectura sección 8, ADR-009.

---

## 5. Azure OpenAI

### 5.1 Embeddings

| Decisión | Detalle |
|---|---|
| **Cliente** | `Azure.AI.OpenAI` o `OpenAIClient` con endpoint y API key. |
| **Modelo** | `text-embedding-3-large` (3072 dims). Deployment name desde `AzureOpenAI__EmbeddingDeploymentName`. |
| **Interfaz** | `IEmbeddingService`: `GenerateAsync(text, ct)` → `float[]`. |
| **Uso** | API (búsqueda, chat RAG), IndexerWorker (chunking + nivel fallo). |

### 5.2 Enrichment (GPT-4o)

| Decisión | Detalle |
|---|---|
| **Modelo** | `gpt-4o`. Deployment desde `AzureOpenAI__ChatDeploymentName`. |
| **Structured output** | `response_format: { type: "json_schema" }` para extracción de jueces, leyes y clasificación de citas. Validar schema en runtime. |
| **Interfaz** | `IEnrichmentService` o integrado en `EnrichmentWorker` como servicio interno. Según specs, `AzureOpenAiEnrichmentService` en Infrastructure. |
| **Uso** | EnrichmentWorker únicamente. |

**Referencia**: ADR-014, arquitectura sección 7.1.

---

## 6. Grafo — SqlGraphService (Fase 1)

### 6.1 Decisiones

| Decisión | Detalle |
|---|---|
| **Almacenamiento** | Azure SQL. Tablas `Citations`, `RulingJudges`, `RulingKeywords`, `RulingStatutes`. Sin Neo4j en Fase 1. |
| **Interfaz** | `IGraphService`: consultas de relaciones (citas entrantes/salientes, jueces por fallo, etc.). |
| **Implementación** | `SqlGraphService`: consultas SQL con CTEs recursivos para cadenas de citas. JOINs para relaciones N:N. |
| **Neo4j** | Diferido a Fase 3 (F3-0). El modelo Cypher de la arquitectura (sección 4.11) se implementará en `Neo4jGraphService` cuando se migre. |

### 6.2 Operaciones típicas

- Obtener citas de un fallo (SourceRulingId, TargetRulingId).
- Obtener fallos que citan a un fallo dado.
- Resolución retroactiva: actualizar `Citations.TargetRulingId` cuando se indexa un fallo que matchea `ExternalAlias` (IndexerWorker, `ResolveCitationsStep`).

**Referencia**: ADR-004, arquitectura sección 4.10 (retroactive citation resolution).

---

## 7. Registro de servicios

### 7.1 Extension method

`InfrastructureServiceExtensions.cs` en `Extensions/`:

```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
{
    // DbContext
    services.AddDbContext<AppDbContext>(opts => ...);
    
    // Repositories
    services.AddScoped<IRulingRepository, RulingRepository>();
    // ... resto de repositorios
    
    // Services
    services.AddSingleton<ISearchService, AzureSearchService>();
    services.AddSingleton<IBlobStorageService, AzureBlobStorageService>();
    services.AddSingleton<IQueuePublisher, StorageQueuePublisher>();
    services.AddSingleton<IEmbeddingService, AzureOpenAiEmbeddingService>();
    services.AddScoped<IGraphService, SqlGraphService>();
    // IEnrichmentService si se expone desde Infrastructure
    
    return services;
}
```

### 7.2 Ciclo de vida

- **Singleton**: clientes HTTP, Search, Blob, Queue, Embedding (estado mínimo, thread-safe).
- **Scoped**: DbContext, repositorios, SqlGraphService (por request en API; por mensaje en workers).

---

## 8. Decisiones documentadas

| ID | Decisión | Justificación |
|---|---|---|
| D1 | Storage Queues en lugar de Service Bus | ADR-009. Mismo Storage Account que Blob. Costo cero adicional en Fase 1. |
| D2 | SqlGraphService con CTEs en SQL | ADR-004. Evitar dependencia de Neo4j hasta Fase 3. Suficiente para consultas de citas en MVP. |
| D3 | Búsqueda híbrida vector + keyword | Mejor recall y precisión que solo vector. Azure AI Search soporta nativamente. |
| D4 | Structured output con json_schema en GPT-4o | Garantiza formato parseable para enrichment. Reduce errores de parsing. |
| D5 | Retry en EF Core | Resiliencia ante fallos transitorios de red en Azure SQL. |

---

## 9. Supuestos

- ⚠️ **SUPUESTO**: Los nombres de colas Storage Queues siguen el patrón `{prefix}-{stage}` (configurable via `Pipeline__QueuePrefix`, default `csjn-ruling`).
- ⚠️ **SUPUESTO**: `IEnrichmentService` (llamadas a GPT-4o para enrichment) se implementa en Infrastructure. Las specs mencionan `AzureOpenAiEnrichmentService`; el EnrichmentWorker lo consume. Si se decide que la lógica de prompts vive solo en el worker, el servicio en Infrastructure podría exponer solo el cliente OpenAI genérico.

---

## 10. Referencias

| Documento | Sección |
|---|---|
| `docs/architecture/legal-ai-ar-architecture.md` | 4 (data model), 7 (infrastructure), 8 (messaging) |
| `docs/architecture/legal-ai-ar-specs.md` | 2.2 (Infrastructure structure), 5 (data model), 6 (messages) |
| `docs/design/f0-2-infrastructure.md` | Azure resources catalog |
| `docs/design/f0-2-environment-variables.md` | Variables per component |
| ADR-002, ADR-004, ADR-009, ADR-014 | Chunking, grafo, mensajería, LLM |
