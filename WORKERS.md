# Workers — Pipeline de Ingesta (.NET 8 Worker Services)

## 1. Arquitectura del Pipeline

```
┌──────────────────────────────────────────────────────────────────┐
│                   AZURE SERVICE BUS                              │
│                                                                  │
│   Queue: legal-crawl    legal-parse    legal-enrich  legal-index │
└──────┬──────────────────────┬──────────────────┬────────────┬───┘
       │                      │                  │            │
  ┌────▼────┐            ┌────▼────┐       ┌────▼────┐  ┌────▼────┐
  │Crawler  │            │ Parser  │       │Enrich.  │  │Indexer  │
  │Worker   │            │ Worker  │       │Worker   │  │Worker   │
  └─────────┘            └─────────┘       └─────────┘  └─────────┘
  .NET BackgroundService — cada worker es un proyecto independiente
```

Cada worker es un **proyecto `.csproj`** que hereda de `BackgroundService` (.NET 8) y se despliega como un **Azure Container App** independiente.

---

## 2. Workers

### 2.1 CrawlerWorker

**Proyecto**: `LegalAiAr.Workers.CrawlerWorker`

**Responsabilidad**: Descargar documentos de fuentes judiciales y subirlos al blob storage.

**Trigger**: Scheduler interno (cron) o mensaje en queue `legal-crawl`.

```csharp
public class CrawlerWorker : BackgroundService
{
    // Implementa IHostedService via BackgroundService
    // Usa IServiceScopeFactory para resolver scoped services
}

public interface ISourceCrawler
{
    string SourceName { get; }
    Task<IEnumerable<CrawlResult>> CrawlAsync(CrawlJob job, CancellationToken ct);
}

// Implementaciones concretas por fuente:
public class SaijCrawler : ISourceCrawler { }
public class PjnCrawler : ISourceCrawler { }
public class ScbaCrawler : ISourceCrawler { }
public class CsjnCrawler : ISourceCrawler { }
```

**Proceso**:
1. Scheduler dispara `CrawlJob` con source, fecha desde/hasta
2. `ISourceCrawler` correspondiente hace HTTP requests a la fuente
3. Descarga documento (PDF o HTML)
4. Sube original a Azure Blob Storage (path: `{type}/{year}/{month}/{id}/original.*`)
5. Crea registro en Azure SQL (`status: Pending`)
6. Publica mensaje en Service Bus queue `legal-parse`

**Configuración por fuente**:
```json
{
  "Sources": {
    "SAIJ": {
      "BaseUrl": "https://www.saij.gob.ar",
      "RateLimitPerSecond": 1,
      "Format": "html",
      "Enabled": true
    },
    "PJN": {
      "BaseUrl": "https://sj.pjn.gov.ar",
      "RateLimitPerSecond": 0.5,
      "Format": "pdf",
      "Enabled": true
    },
    "SCBA": {
      "BaseUrl": "https://www.scba.gov.ar",
      "RateLimitPerSecond": 1,
      "Format": "pdf",
      "Enabled": true
    },
    "CSJN": {
      "BaseUrl": "https://sjconsulta.csjn.gov.ar",
      "RateLimitPerSecond": 0.5,
      "Format": "pdf",
      "Enabled": true
    }
  }
}
```

---

### 2.2 ParserWorker

**Proyecto**: `LegalAiAr.Workers.ParserWorker`

**Responsabilidad**: Extraer texto estructurado del documento original.

**Trigger**: Mensaje en Service Bus queue `legal-parse`.

```csharp
public interface IDocumentParser
{
    DocumentType SupportedType { get; }
    Task<ParseResult> ParseAsync(Stream document, string format);
}

public class JudicialCasePdfParser : IDocumentParser { }
public class JudicialCaseHtmlParser : IDocumentParser { }
```

**Proceso**:
1. Consume mensaje de `legal-parse` con `documentId`
2. Descarga `original.*` desde Azure Blob Storage
3. Detecta formato y selecciona parser:
   - PDF → **PdfPig** (biblioteca .NET para extracción de texto)
   - HTML → **HtmlAgilityPack** / **AngleSharp**
4. Extrae campos estructurados del fallo:
   - Caratula, número, fecha, tribunal, jueces, texto completo
5. Sube `extracted.txt` a Azure Blob Storage
6. Actualiza `JudicialCase` en Azure SQL con metadata extraída
7. Publica mensaje en queue `legal-enrich`

**Heurísticas de parsing de fallos**:
```
Caratula:    Primera línea en mayúsculas / regex "(.+) c\/ (.+) s\/ (.+)"
Fecha:       regex "Buenos Aires,? \d{1,2} de \w+ de \d{4}"
Número:      regex "(Expte\.|N°|Nro\.)\s*[\w\-\/]+"
Tribunal:    Cabecera del documento (primeras 5 líneas)
Jueces:      Firmas al final / sección "Ante mí:"
Votos:       Secciones "El doctor .+ dijo:" / "Considerando:"
```

**NuGet packages**:
- `PdfPig` — extracción de texto de PDFs
- `AngleSharp` — parsing de HTML
- `HtmlAgilityPack` — alternativa HTML

---

### 2.3 EnrichmentWorker

**Proyecto**: `LegalAiAr.Workers.EnrichmentWorker`

**Responsabilidad**: Enriquecer el documento con entidades, clasificaciones y relaciones, usando Azure OpenAI.

**Trigger**: Mensaje en Service Bus queue `legal-enrich`.

```csharp
public interface IEnricher
{
    Task<EnrichmentResult> EnrichAsync(JudicialCase case_, string extractedText);
}

public class OpenAiEnricher : IEnricher
{
    // Usa Azure OpenAI gpt-4o con structured outputs
    // Extrae: jueces, leyes, temas, citaciones en una sola llamada
}
```

**Proceso**:
1. Consume mensaje de `legal-enrich`
2. Descarga `extracted.txt` desde Blob Storage
3. Llama a **Azure OpenAI (GPT-4o)** con prompt de extracción:
   - Jueces intervinientes + roles
   - Leyes citadas (`"Ley 20744"`, `"Art. 245 LCT"`)
   - Temas jurídicos (voces)
   - Referencias a otros fallos
   - Clasificación: fuero, instancia
4. Resuelve jueces y tribunales contra tablas maestras (o crea nuevas entidades)
5. Actualiza tablas auxiliares en Azure SQL (`CaseJudges`, `CaseCitations`, `CaseVoices`)
6. Publica mensaje en queue `legal-index`

**Prompt de extracción (structured output)**:
```json
{
  "judges": [{"name": "...", "role": "vocal|presidente", "vote": "mayoria|disidencia"}],
  "citedLaws": ["Ley 20744", "Art. 245 LCT"],
  "legalTopics": ["despido sin causa", "indemnización"],
  "caseCitations": ["Expte. 12345/2020", "CSJN Fallos 310:1"],
  "fuero": "laboral",
  "instance": "camara"
}
```

---

### 2.4 IndexerWorker

**Proyecto**: `LegalAiAr.Workers.IndexerWorker`

**Responsabilidad**: Generar embeddings e indexar en Azure AI Search y Neo4j.

**Trigger**: Mensaje en Service Bus queue `legal-index`.

**Proceso**:
1. Consume mensaje de `legal-index`
2. Lee `JudicialCase` completo desde Azure SQL
3. **Azure AI Search**:
   - Genera embedding de `summary + holding` → `summaryVector`
   - Genera embedding de `fullText` (chunking 512 tokens) → `fullTextVector`
   - Indexa o actualiza documento en índice `judicial-cases`
4. **Neo4j CE**:
   - MERGE nodo `(:JudicialCase)`
   - MERGE nodos `(:Judge)`, `(:Tribunal)`, `(:Law)`, `(:LegalTopic)`
   - Crea relaciones `RULED_IN`, `RESOLVED_BY`, `REFERENCES`, `CITES`, `TAGGED_WITH`
5. Actualiza `Status = Indexed`, `IndexedAt = now()` en Azure SQL

```csharp
public class IndexerWorkerService
{
    private readonly ISearchClient _searchClient;
    private readonly IGraphClient _graphClient;
    private readonly IEmbeddingService _embeddingService;
    private readonly LegalAiDbContext _db;
}
```

---

## 3. Azure Service Bus — Queues

```
Queue: legal-crawl
  Mensaje: { "source": "SAIJ", "fromDate": "2024-01-01", "toDate": "2024-01-31" }
  TTL: 1 día
  Lock duration: 5 minutos

Queue: legal-parse
  Mensaje: { "documentId": "uuid", "blobPath": "...", "format": "pdf|html" }
  TTL: 7 días
  Lock duration: 10 minutos

Queue: legal-enrich
  Mensaje: { "documentId": "uuid" }
  TTL: 7 días
  Lock duration: 15 minutos

Queue: legal-index
  Mensaje: { "documentId": "uuid" }
  TTL: 7 días
  Lock duration: 10 minutos

Dead Letter Queue: automático por Service Bus
  Alertas: Azure Monitor → email / Teams
```

---

## 4. Manejo de Errores y Retry

```csharp
// Configuración de Resiliency con Polly
services.AddResiliencePipeline("worker-pipeline", builder =>
{
    builder
        .AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMinutes(1),
            BackoffType = DelayBackoffType.Exponential,
        })
        .AddTimeout(TimeSpan.FromMinutes(10));
});
```

```
Estados de documento:
  Pending → Processing → Indexed
                      ↘ Error
                         └ RetryCount: 1, 2, 3
                         └ ErrorMessage: detalle del error
                         └ Si RetryCount >= 3 → mensaje a Dead Letter Queue
                         └ Alerta a Azure Monitor
```

---

## 5. Scheduler (CrawlerWorker interno)

```csharp
// Usando Quartz.NET dentro del CrawlerWorker
public class SchedulerConfig
{
    public static IEnumerable<CrawlSchedule> Schedules => new[]
    {
        new CrawlSchedule { Source = "SAIJ",  Cron = "0 3 * * *", Strategy = "incremental" },
        new CrawlSchedule { Source = "PJN",   Cron = "0 2 * * 0", Strategy = "incremental" },
        new CrawlSchedule { Source = "SCBA",  Cron = "0 4 * * 1", Strategy = "incremental" },
        new CrawlSchedule { Source = "CSJN",  Cron = "0 5 1 * *", Strategy = "full" },
    };
}
```

---

## 6. Monitoreo

- **Azure Monitor + Application Insights**: logs, métricas, trazas distribuidas
- **Azure Container Apps**: métricas de scaling automático por cantidad de mensajes en queue
- **Métricas clave**:
  - Documentos procesados por worker por hora
  - Mensajes en Dead Letter Queue (alerta crítica)
  - Latencia promedio por etapa del pipeline
  - Tasa de errores por fuente
