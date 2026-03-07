# KB Core — Knowledge Base Híbrida

## 1. Concepto

La KB Core es el **núcleo del sistema Legal AI AR**. Es una base de conocimiento híbrida que combina cuatro tecnologías de almacenamiento sobre Azure, cada una optimizada para un tipo de acceso diferente. Se implementa como la librería `LegalAiAr.KbCore`.

```
┌─────────────────────────────────────────────────────────────────┐
│                          KB CORE                                │
│                    LegalAiAr.KbCore                             │
│                                                                 │
│  ┌──────────────────────┐                                       │
│  │  Azure Storage       │  Documento original (PDF/HTML/TXT)    │
│  │  Account (Blob)      │  Inmutable. Fuente de verdad raw.     │
│  │                      │  SDK: Azure.Storage.Blobs             │
│  └──────────────────────┘                                       │
│                                                                 │
│  ┌──────────────────────┐                                       │
│  │  Azure SQL Database  │  Metadata estructurada + entidades    │
│  │  + EF Core 8         │  Búsqueda filtrada, joins, FTS        │
│  │                      │  SDK: Microsoft.EntityFrameworkCore   │
│  └──────────────────────┘                                       │
│                                                                 │
│  ┌──────────────────────┐                                       │
│  │  Azure AI Search     │  Embeddings + full-text               │
│  │                      │  Búsqueda semántica híbrida           │
│  │                      │  SDK: Azure.Search.Documents          │
│  └──────────────────────┘                                       │
│                                                                 │
│  ┌──────────────────────┐                                       │
│  │  Neo4j CE            │  Relaciones entre entidades           │
│  │                      │  Citas, jueces, tribunales, leyes     │
│  │                      │  SDK: Neo4j.Driver                    │
│  └──────────────────────┘                                       │
│                                                                 │
│              Azure OpenAI Service                               │
│              text-embedding-3-large │ gpt-4o                   │
│              SDK: Azure.AI.OpenAI                               │
└─────────────────────────────────────────────────────────────────┘
```

---

## 2. Modelo de Documento

Toda entidad almacenada en la KB es un **Document** con un tipo específico. El diseño es extensible para futuros tipos documentales.

### 2.1 Tipos de Documento

```csharp
public enum DocumentType
{
    // Fase 1 (actual)
    JudicialCase        = 1,    // Fallos judiciales

    // Fase 2 (futuro)
    Law                 = 10,   // Leyes y códigos
    Regulation          = 11,   // Decretos, resoluciones

    // Fase 3 (futuro)
    LegalThesaurus      = 20,   // Tesauros legales
    TribunalCatalog     = 30,   // Catálogo de tribunales
    JudgeProfile        = 31,   // Perfiles de jueces
    Doctrine            = 40,   // Doctrina jurídica
}
```

### 2.2 Entidad Base (Domain)

```csharp
public abstract class Document
{
    public Guid Id { get; init; }
    public DocumentType DocumentType { get; init; }
    public string SourceId { get; set; }        // ID en sistema fuente (SAIJ, PJN...)
    public string SourceName { get; set; }       // "SAIJ", "PJN", "SCBA"
    public string SourceUrl { get; set; }
    public string BlobPath { get; set; }         // Path en Azure Blob Storage
    public string BlobHash { get; set; }         // SHA-256 del original
    public string Language { get; set; } = "es";
    public DocumentStatus Status { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? IndexedAt { get; set; }
}

public enum DocumentStatus
{
    Pending     = 0,
    Processing  = 1,
    Indexed     = 2,
    Error       = 99
}
```

---

## 3. Azure SQL Database — Modelo Relacional

### 3.1 EF Core DbContext

```csharp
public class LegalAiDbContext : DbContext
{
    public DbSet<JudicialCase> JudicialCases { get; set; }
    public DbSet<Tribunal> Tribunals { get; set; }
    public DbSet<Judge> Judges { get; set; }
    public DbSet<CaseJudge> CaseJudges { get; set; }
    public DbSet<CaseCitation> CaseCitations { get; set; }
    public DbSet<CaseVoice> CaseVoices { get; set; }
}
```

### 3.2 Entidad JudicialCase

```csharp
public class JudicialCase : Document
{
    // Identificación
    public string CaseNumber { get; set; }          // Número de expediente
    public int? CaseYear { get; set; }
    public string Caratula { get; set; }            // "García c/ Estado s/ amparo"

    // Clasificación
    public string Jurisdiction { get; set; }        // federal | provincial | caba
    public string JurisdictionName { get; set; }    // "Córdoba", "Santa Fe"
    public string Fuero { get; set; }               // civil | penal | laboral...
    public string Instance { get; set; }            // primera | camara | suprema

    // Tribunal
    public Guid? TribunalId { get; set; }
    public Tribunal Tribunal { get; set; }
    public string TribunalName { get; set; }

    // Fechas
    public DateOnly? ResolutionDate { get; set; }
    public DateOnly? PublicationDate { get; set; }

    // Contenido
    public string Subject { get; set; }
    public string Summary { get; set; }
    public string Holding { get; set; }             // Decisión principal
    public string FullText { get; set; }

    // Clasificación IA
    public string[] LegalTopics { get; set; }       // ["derecho laboral", "despido"]
    public string[] CitedLaws { get; set; }         // ["Ley 20744", "Art. 245 LCT"]

    // Navegación
    public ICollection<CaseJudge> CaseJudges { get; set; }
    public ICollection<CaseCitation> Citations { get; set; }
    public ICollection<CaseVoice> Voices { get; set; }
}
```

### 3.3 Entidades auxiliares

```csharp
public class Tribunal
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Jurisdiction { get; set; }
    public string Fuero { get; set; }
    public string Instance { get; set; }
    public string Province { get; set; }
    public bool Active { get; set; } = true;
}

public class Judge
{
    public Guid Id { get; init; }
    public string FullName { get; set; }
    public string DocumentNumber { get; set; }
    public bool Active { get; set; } = true;
    public string Bio { get; set; }
}

public class CaseJudge
{
    public Guid CaseId { get; set; }
    public Guid JudgeId { get; set; }
    public string Role { get; set; }    // vocal | presidente | conjuez
    public string Vote { get; set; }    // mayoria | disidencia | concurrencia
}

public class CaseCitation
{
    public Guid CitingCaseId { get; set; }
    public Guid CitedCaseId { get; set; }
    public string CitationType { get; set; }  // confirma | revoca | distingue | cita
}
```

---

## 4. Azure Storage Account — Blob Storage

### Estructura de contenedores y paths

```
Container: legal-documents
└── {document-type}/
    └── {year}/
        └── {month}/
            └── {document-id}/
                ├── original.pdf        ← Inmutable. Jamás se modifica.
                ├── extracted.txt       ← Texto plano extraído por Parser Worker
                └── metadata.json       ← Snapshot de metadata al momento de ingesta
```

### Ejemplo real

```
legal-documents/judicial-case/2024/03/
    550e8400-e29b-41d4-a716-446655440000/
        original.pdf
        extracted.txt
        metadata.json
```

### Client en KbCore

```csharp
public interface IBlobStorageClient
{
    Task<Uri> UploadOriginalAsync(Guid documentId, DocumentType type, Stream content, string extension);
    Task<Stream> DownloadOriginalAsync(string blobPath);
    Task UploadExtractedTextAsync(Guid documentId, DocumentType type, string text);
    Task<string> DownloadExtractedTextAsync(string blobPath);
}
```

---

## 5. Azure AI Search — Búsqueda Vectorial + Full-Text

Azure AI Search reemplaza a un vector DB standalone (como Qdrant), ofreciendo **búsqueda híbrida** (vector + keyword) en un único servicio administrado.

### Índices

```
Índice: judicial-cases
  Campos:
    id                  (String, key)
    caseId              (String, filterable)
    caratula            (String, searchable, analyzer: es.lucene)
    summary             (String, searchable)
    holding             (String, searchable)
    fullText            (String, searchable)
    fuero               (String, filterable, facetable)
    jurisdiction        (String, filterable, facetable)
    instance            (String, filterable, facetable)
    resolutionDate      (DateTimeOffset, filterable, sortable)
    legalTopics         (Collection(String), filterable, facetable)
    citedLaws           (Collection(String), filterable)
    tribunalName        (String, filterable, facetable)

  Campos vectoriales:
    summaryVector       (Collection(Single), dims: 3072)   ← embedding del summary+holding
    fullTextVector      (Collection(Single), dims: 3072)   ← embedding del texto completo

  Perfil de búsqueda semántica:
    Configuración: "legal-semantic"
    Título: caratula
    Contenido: summary, holding
    Keywords: legalTopics, citedLaws
```

### Tipos de búsqueda disponibles

```
1. Full-text search       → keyword sobre texto completo (Lucene, español)
2. Vector search          → ANN sobre embeddings (HNSW)
3. Hybrid search          → Combinación full-text + vector con RRF re-ranking
4. Semantic ranking       → Re-ranking con modelo de lenguaje de Azure
5. Filtered search        → Filtros por fuero, fecha, jurisdicción, etc.
```

### Modelo de embedding

```
Proveedor: Azure OpenAI Service
Modelo:    text-embedding-3-large
Dims:      3072
Chunking:  512 tokens con overlap de 50 tokens (para fullTextVector)
           Sin chunking para summaryVector
```

### Client en KbCore

```csharp
public interface ISearchClient
{
    Task IndexCaseAsync(JudicialCaseSearchDocument document);
    Task<SearchResults> SearchCasesAsync(SearchQuery query);
    Task DeleteCaseAsync(string caseId);
}

public class SearchQuery
{
    public string Text { get; set; }
    public float[] Vector { get; set; }
    public SearchMode Mode { get; set; }  // FullText | Vector | Hybrid | Semantic
    public Dictionary<string, string> Filters { get; set; }
    public int Top { get; set; } = 10;
}
```

---

## 6. Neo4j CE — Graph Database

### Nodos

```cypher
(:JudicialCase {
    id: String,
    caseNumber: String,
    caratula: String,
    resolutionDate: Date,
    fuero: String,
    jurisdiction: String,
    instance: String
})

(:Judge {
    id: String,
    fullName: String
})

(:Tribunal {
    id: String,
    name: String,
    jurisdiction: String,
    fuero: String,
    instance: String
})

(:Law {
    id: String,
    reference: String   // "Ley 20744", "Art. 245 LCT", "Decreto 390/76"
})

(:LegalTopic {
    name: String        // "despido sin causa", "daño moral"
})

(:Province {
    name: String
})
```

### Relaciones

```cypher
(case)-[:RESOLVED_BY]->(tribunal)
(judge)-[:RULED_IN {role: String, vote: String}]->(case)
(case)-[:CITES {type: String}]->(otherCase)
(case)-[:REFERENCES]->(law)
(case)-[:TAGGED_WITH]->(topic)
(tribunal)-[:LOCATED_IN]->(province)
(judge)-[:APPOINTED_TO {from: Date, to: Date}]->(tribunal)
```

### Consultas Cypher típicas

```cypher
// Línea jurisprudencial de un tema
MATCH (t:LegalTopic {name: $topic})<-[:TAGGED_WITH]-(c:JudicialCase)
      -[:CITES]->(prev:JudicialCase)
RETURN c, prev ORDER BY c.resolutionDate DESC LIMIT 20

// Jueces que más fallan sobre un tema
MATCH (j:Judge)-[:RULED_IN]->(c:JudicialCase)
      -[:TAGGED_WITH]->(t:LegalTopic {name: $topic})
RETURN j.fullName, count(c) AS cases ORDER BY cases DESC

// Árbol de citas de un fallo (2 niveles)
MATCH path = (root:JudicialCase {id: $id})-[:CITES*1..2]->(cited)
RETURN path
```

### Client en KbCore

```csharp
public interface IGraphClient
{
    Task MergeJudicialCaseAsync(JudicialCase case_);
    Task CreateCitationAsync(Guid citingId, Guid citedId, string type);
    Task<IEnumerable<RelatedCase>> GetRelatedCasesAsync(Guid caseId, int depth = 2);
    Task<IEnumerable<JurisprudenceLine>> GetJurisprudenceLineAsync(string topic);
}
```

---

## 7. Azure OpenAI Service

```csharp
public interface IEmbeddingService
{
    // Genera embedding para indexación o búsqueda
    Task<float[]> GenerateEmbeddingAsync(string text);
    
    // Genera embeddings en batch (más eficiente)
    Task<float[][]> GenerateEmbeddingsAsync(IEnumerable<string> texts);
}

public interface ILlmService
{
    // Resumen automático de fallo
    Task<string> SummarizeCaseAsync(string fullText);
    
    // Extracción de entidades
    Task<CaseEntities> ExtractEntitiesAsync(string fullText);
    
    // Chat sobre jurisprudencia (RAG)
    Task<string> ChatAsync(string question, IEnumerable<string> context);
}
```

**Configuración recomendada:**

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://{resource}.openai.azure.com/",
    "EmbeddingDeployment": "text-embedding-3-large",
    "ChatDeployment": "gpt-4o",
    "EmbeddingDimensions": 3072
  }
}
```

---

## 8. Flujo de Ingesta (happy path)

```
1. Crawler Worker descarga fallo de SAIJ
   └→ Sube original a Azure Blob Storage
   └→ Crea registro en Azure SQL (status: Pending)
   └→ Publica mensaje en Service Bus queue: "parse"

2. Parser Worker consume queue "parse"
   └→ Descarga blob, extrae texto + metadata
   └→ Sube extracted.txt al blob
   └→ Actualiza JudicialCase en Azure SQL
   └→ Publica mensaje en queue: "enrich"

3. Enrichment Worker consume queue "enrich"
   └→ Llama Azure OpenAI para extracción de entidades
   └→ Actualiza tablas auxiliares en Azure SQL
   └→ Publica mensaje en queue: "index"

4. Indexer Worker consume queue "index"
   └→ Genera embeddings vía Azure OpenAI
   └→ Indexa documento en Azure AI Search
   └→ Crea nodos/relaciones en Neo4j CE
   └→ Actualiza status: Indexed, IndexedAt en Azure SQL
```
