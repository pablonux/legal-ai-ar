# Gap Analysis: MVP (legal-ai-ar) vs Legal Ai Ar

> **Fecha:** Mayo 2026
> **Objetivo:** Identificar qué del MVP existente se puede reutilizar, qué gaps existen, y qué decisiones técnicas deben revisarse para el nuevo proyecto Legal Ai Ar.

---

## 1. Resumen Ejecutivo

El MVP es un proyecto **sorprendentemente maduro** — mucho más que un prototipo. Tiene un pipeline de ingesta de 6 etapas con strategy pattern por fuente, un chat agéntico con tool calling y 13 herramientas, búsqueda híbrida, detección de comunidades en grafo, tesauro SAIJ integrado, y un frontend Angular completo con ~15 vistas funcionales incluyendo un panel admin.

La mayoría de las capacidades core que Legal Ai Ar planea ya existen en alguna forma en el MVP. Los gaps principales son de **sofisticación** (agentes especializados vs uno genérico, prompt registry vs hardcoded, eval framework inexistente) y de **ops** (no hay observabilidad, IaC, ni Key Vault).

**Veredicto:** El MVP es una base sólida. Recomendamos migrar y evolucionar, no reescribir.

---

## 2. Stack Comparativo

| Aspecto | MVP (legal-ai-ar) | Legal Ai Ar (plan) | Gap |
|---|---|---|---|
| Backend | ASP.NET Core 10, Controllers | .NET 10, Minimal API | Bajo — refactor de controllers a Minimal API |
| Frontend | Angular (standalone components) | Angular 19 (standalone) | Bajo — misma base |
| UI Library | PwC AppKit 4 (parcial) | PwC AppKit 4 | Bajo — ya tiene guidelines |
| ORM | EF Core 10 (Code-First) | EF Core 10 (Code-First) | Nulo |
| AI Orchestration | Azure OpenAI SDK directo | Semantic Kernel | **Alto** — rewrite de capa de agentes |
| Workers | BackgroundService (polling queues) | Azure Functions (event-driven) | **Alto** — cambio de hosting model |
| Messaging | Azure Storage Queues | Azure Storage Queues | Nulo |
| DB | Azure SQL (relacional) | Azure SQL + SQL Graph | Medio — agregar edge tables |
| Search | Azure AI Search (hybrid) | Azure AI Search (hybrid) | Nulo |
| Embeddings | text-embedding-3-large (3072d) | text-embedding-3-large (3072d) | Nulo |
| LLM | GPT-4o | GPT-4o | Nulo |
| Blob | Azure Blob Storage | Azure Blob Storage | Nulo |
| PDF Parsing | PdfPig | Azure Document Intelligence | Medio — PdfPig funciona pero no hace OCR |
| Auth | Entra ID (custom JWT) | Entra ID (MSAL) | Bajo |
| Secrets | appsettings / env vars | Azure Key Vault | Medio |
| CI/CD | GitHub Actions (CI + CD staging) | GitHub Actions (4 ambientes) | Medio |
| IaC | Ninguno | Azure Bicep | **Alto** — no existe |
| Observabilidad | Ninguna | OpenTelemetry + App Insights | **Alto** — no existe |
| Testing | xUnit + NSubstitute | xUnit + Moq + FluentAssertions | Bajo — mismo framework |
| Mediator | Custom IMediator | MediatR | Bajo — reemplazo directo |

---

## 3. Modelo de Datos — Mapeo Detallado

### 3.1 Entidades que ya existen y se pueden reutilizar

| Entidad Legal Ai Ar | Entidad MVP | Cobertura | Notas |
|---|---|---|---|
| NormaJuridica | `Statute` | **95%** | Tiene NormType, NormativeLevel, fechas, status, órgano emisor. Muy completa. |
| Jurisprudencia | `Ruling` | **100%** | 40+ campos. Más rica que lo planeado: incluye Vote, ProsecutorOpinion, Sumario. |
| Articulo | `RulingStatuteArticle` | **70%** | Tiene artículo + subsección pero no es entidad standalone con texto propio. |
| Doctrina | `LegalDoctrine` | **100%** | Statement, topic, overruled tracking, binding weight. |
| Expediente | `JudicialProceeding` | **90%** | ProcessType, Status, parties. Falta timeline de movimientos. |
| Persona | `Person` | **100%** | Physical/Legal, verified flag, position, institution. |
| Tribunal | `Court` + `JudicialOffice` | **100%** | Con jerarquía office→court. |
| Tesauro | `ThesaurusTerm` + `ThesaurusRelation` | **100%** | NT/BT/RT/UF relations. SAIJ integrado. |
| Keyword/Voces | `Keyword` + `RulingKeyword` + `SumarioKeyword` | **100%** | Normalización incluida. |
| Citation | `Citation` (6 tipos) | **90%** | UPHOLDS, OVERRULES, DISTINGUISHES, CITES, FOLLOWS, DISSENTS_FROM. |
| NormRelation | `NormRelation` (4 tipos) | **80%** | DEROGATES, AMENDS, REGULATES, COMPLEMENTS. Faltan Interpreta y Aplica. |
| Voto | `Vote` | **100%** | Majority/dissent/concurrence con firmantes. No planeado en Legal Ai Ar. |
| Dictamen fiscal | `ProsecutorOpinion` | **100%** | No planeado en Legal Ai Ar. |
| Representación | `LegalRepresentation` | **100%** | Abogado-parte. No planeado en Legal Ai Ar. |
| GraphCommunity | `GraphCommunity` + `CommunityMembership` | **100%** | Jerárquica con sumarios LLM. No planeado explícitamente en Legal Ai Ar. |
| FieldProvenance | `FieldProvenance` | **100%** | Trazabilidad campo-por-campo con método de inferencia. Más granular que lo planeado. |
| DocumentStageLog | `DocumentStageLog` | **100%** | Tracking de pipeline por documento. |

### 3.2 Entidades que faltan en el MVP

| Entidad Legal Ai Ar | Gap | Impacto |
|---|---|---|
| **Inciso** | No existe. Artículos se trackean como string, no como entidad con texto | Medio — se necesita para RAG granular por inciso |
| **Movimiento** | No existe. JudicialProceeding no tiene timeline de eventos | Medio — requerido para agente procesal |
| **Plazo** | No existe. No hay cálculo de plazos procesales | Alto — feature core del agente procesal |
| **UsuarioPreferencias** | `User` existe pero sin preferencias (rama, alertas) | Bajo |
| **Conversacion / MensajeChat** | No hay persistencia de conversaciones de chat | **Alto** — chat es stateless en MVP |
| **AnalisisRiesgo** | No existe | Medio — feature de Release 3.0 |
| **ArticuloVersion** | No hay versionado temporal de artículos | Medio — necesario para consultas temporales |
| **PromptTemplate** | Prompts hardcoded en C# | **Alto** — necesario para prompt management |
| **FeedbackRespuesta** | No existe | **Alto** — necesario para mejora continua |
| **TaxonomiaLegal** | Cubierto parcialmente por ThesaurusTerm | Bajo — extender lo existente |

### 3.3 Entidades del MVP que Legal Ai Ar no planeó (y debería considerar)

| Entidad MVP | Valor | Recomendación |
|---|---|---|
| `Vote` (votos de magistrados) | Alto para análisis jurisprudencial. Permite saber posición de cada juez. | **Incorporar a Legal Ai Ar** |
| `ProsecutorOpinion` (dictamen fiscal) | Medio. Relevante para derecho penal y administrativo. | **Incorporar a Legal Ai Ar** |
| `Sumario` (headnotes doctrinarios) | Alto. Separar sumario de fallo completo mejora RAG. | **Incorporar a Legal Ai Ar** |
| `GraphCommunity` (comunidades) | Alto. Permite "resúmenes de líneas jurisprudenciales". | **Incorporar a Legal Ai Ar** |
| `CrawlerConfig` (config por fuente) | Alto para operación. Permite gestionar crawlers desde UI admin. | **Incorporar a Legal Ai Ar** |
| `EmbeddingConfig` (config de modelos) | Medio. Permite cambiar modelo/dimensiones sin deploy. | **Incorporar a Legal Ai Ar** |
| `FieldProvenance` (provenance por campo) | Alto. Trazabilidad más granular que lo planeado. | **Adoptar en lugar de DataProvenance** |
| `ChunkEntityMention` | Alto para RAG. Entidades mencionadas en cada chunk. | **Incorporar a Legal Ai Ar** |

---

## 4. Pipeline de Ingesta — Comparación

### 4.1 Etapas

| # | Legal Ai Ar (plan) | MVP (implementado) | Delta |
|---|---|---|---|
| 1 | Scraper/Collector (Timer Trigger) | **Discoverer** — descubre docs en fuentes | Equivalente. MVP usa BackgroundService, Legal Ai Ar planea Azure Functions. |
| 2 | — | **Fetcher** — descarga PDFs/HTML | MVP tiene un paso extra de descarga dedicado. Legal Ai Ar lo combina con paso 1. |
| 3 | Parser & Normalizer | **Parser** — extrae texto y metadata | Equivalente. MVP usa PdfPig; Legal Ai Ar planea Azure Doc Intelligence. |
| 4 | — | **Enrichment** — LLM enrichment | Equivalente al paso 4 de Legal Ai Ar (chunking + enrichment). |
| 5 | SQL insert + Blob upload | **Persister** — persiste entidades | Equivalente. |
| 6 | Embedding Generator | Integrado en **Indexer** (GenerateEmbeddingsStep) | Equivalente. MVP ya implementa Contextual Retrieval. |
| 7 | AI Search Indexer | Integrado en **Indexer** (IndexSearchStep) | Equivalente. |
| 8 | Graph Builder | Integrado en **Indexer** (ResolveCitationsStep + ExtractChunkMentionsStep) | Equivalente. |

**Hallazgo clave:** El MVP tiene **más stages** que Legal Ai Ar (6 vs 7, pero Discoverer + Fetcher son 2 pasos que Legal Ai Ar combina en 1). Además, el MVP implementa **strategy pattern por fuente** (`IDiscoverStrategy`, `IFetchStrategy`, `IParseStrategy`, `IEnrichStrategy`, `IIndexStrategy`), lo cual es una decisión de diseño superior para soportar múltiples fuentes con lógicas diferentes.

### 4.2 Fuentes implementadas en el MVP

| Fuente | Estrategia | Estado |
|---|---|---|
| CSJN — Sumarios | API (sjconsulta.csjn.gov.ar) | Implementada |
| CSJN — Acuerdos | API | Implementada |
| CSJN — Fallos Destacados | API | Implementada |
| SAIJ — Jurisprudencia | HTML + PDF scraping | Implementada |
| SAIJ — Legislación | HTML scraping | Implementada |

Esto es un asset valioso: los crawlers ya están funcionando y produciendo datos reales.

### 4.3 Lo que el MVP tiene y Legal Ai Ar no planificó

- **DLQ management** con UI admin para ver, reintentar y descartar documentos fallidos
- **Infra recovery** orchestrator para manejar incidentes
- **External download cache** en Blob para evitar re-downloads
- **Document stage log** para tracking granular del pipeline por documento

---

## 5. AI / RAG / Chat — Comparación

### 5.1 Lo que el MVP ya tiene

| Capacidad | Estado en MVP | Notas |
|---|---|---|
| Hybrid Search (BM25 + vector) | **Implementado** | 3 índices: rulings, chunks, statutes |
| Contextual Retrieval | **Implementado** | `ChunkContextualizationPrompt` genera contexto por chunk en ingesta |
| Tool Calling (function calling) | **Implementado** | 13 herramientas registradas para el agente |
| SSE Streaming | **Implementado** | Eventos tipados: text, tool_start, tool_end, validation, done |
| Input Guardrails | **Implementado** | 2 capas: rule-based + LLM classifier |
| Output Guardrails | **Implementado** | Validación de citas contra DB post-respuesta |
| Query Preprocessing | **Implementado** | GPT-4o-mini expande queries, usa tesauro SAIJ |
| Community Detection | **Implementado** | Union-Find + clustering por rama legal |
| Community Summarization | **Implementado** | Sumarios generados por LLM |

### 5.2 Lo que falta para llegar a Legal Ai Ar

| Capacidad Legal Ai Ar | Estado en MVP | Esfuerzo estimado |
|---|---|---|
| **3 agentes especializados** (Normativo, Jurisprudencial, Procesal) | 1 agente genérico con 13 tools | Alto — requiere routing, system prompts especializados, orchestrator |
| **Semantic Kernel** | OpenAI SDK directo | Alto — rewrite de AzureOpenAiAgentChatService y ChatQueryHandler |
| **Router semántico + LLM** | No existe | Alto — nueva capa |
| **LLM Re-ranking** (top-20 → top-5) | No existe | Medio |
| **Prompt Registry en DB** | Prompts hardcoded en C# | Medio — nueva tabla + UI |
| **A/B testing de prompts** | No existe | Medio |
| **Evaluación (golden set, LLM-as-judge)** | No existe | Alto — infraestructura nueva |
| **Feedback loop (thumbs up/down)** | No existe | Medio — UI + tabla + análisis |
| **Confidence score por respuesta** | No existe (citas se validan pero sin score global) | Medio |
| **Persistencia de conversaciones** | Chat stateless | Medio — nuevas tablas + lógica |
| **Semantic caching** | No existe | Medio |
| **Circuit breaker (Polly)** | No existe | Bajo — Polly ya es dependencia |
| **Memory (short-term + long-term)** | No existe | Medio |

---

## 6. Frontend — Comparación

### 6.1 Features implementadas en el MVP

| Feature Legal Ai Ar | Vista MVP | Cobertura |
|---|---|---|
| F01 - Auth | Login + guard + interceptor | 90% |
| F02 - Dashboard | `estadisticas` — KB stats | 70% (stats, no dashboard personalizado) |
| F03 - Búsqueda de normas | `ordenamiento` — statutes list + detail | 80% |
| F04 - Búsqueda de jurisprudencia | `jurisprudencia` — search + results + detail | **95%** |
| F05 - Detalle de norma | `ordenamiento/:id` — statute detail | 80% |
| F08 - Chat con agentes | `asistente` — chat con SSE streaming | **90%** |
| F12 - Gestión de expedientes | `procesos` — proceeding list + detail | 60% (sin CRUD, solo lectura) |
| F19 - Admin usuarios | `admin/users` | 70% |
| F21 - Explorador de grafo | `explorador` — graph explorer | **90%** |
| FT02 - Búsqueda global | Command palette (Ctrl+K) | **80%** |
| Panel admin (ingesta) | `admin/*` — jobs, DLQ, reprocess | **95%** — muy completo |

### 6.2 Features que NO existen en el MVP

| Feature Legal Ai Ar | Gap |
|---|---|
| F06 - Detalle de artículo (standalone) | No existe — artículos están dentro de norma |
| F07 - Novedades normativas (feed) | No existe |
| F09/F10/F11 - Agentes especializados | Solo 1 chat genérico |
| F13 - Gestión de plazos | No existe |
| F14 - Calendario legal | No existe |
| F15/F16 - Análisis de riesgo | No existe |
| F17 - Generación de informes | No existe |
| F18 - Reportes operativos | Parcial (estadísticas) |
| F20 - Alertas avanzadas | No existe |
| F22 - Feedback de agentes | No existe |
| F23 - PWA offline | No existe |
| FT01 - Notificaciones real-time | No existe |
| FT03 - Tema y accesibilidad | Parcial (PwC AppKit) |

### 6.3 Assets del frontend reutilizables

| Asset | Valor |
|---|---|
| Componente de chat con SSE streaming | **Muy alto** — funcionalidad core ya implementada |
| Graph explorer con Cytoscape | **Alto** — visualización de grafo funcional |
| Command palette (búsqueda global) | **Alto** — UX moderna ya implementada |
| Panel admin de ingesta | **Muy alto** — DLQ, jobs, reprocess |
| Skeleton loaders, empty states, breadcrumbs | Medio — componentes genéricos |
| Servicios de API (rulings, courts, persons, statutes) | Alto — capa de comunicación lista |
| PwC AppKit 4 guidelines + mockups | **Muy alto** — diseño ya definido |

---

## 7. Recomendación Estratégica

### 7.1 Evolucionar, no reescribir

El MVP tiene ~16,000 archivos de código funcional. Reescribir desde cero significaría perder meses de trabajo en crawlers, pipeline, modelo de datos, y UI. Recomendamos un approach de **migración incremental**:

### 7.2 Plan de migración en 4 fases

**Fase A — Foundation (Sprint 0-1):**
- Migrar repo como base del nuevo proyecto
- Agregar Azure Key Vault, OpenTelemetry, App Insights
- Agregar IaC (Bicep) para los recursos existentes
- Agregar las entidades faltantes (Inciso, Movimiento, Plazo, Conversacion, MensajeChat, PromptTemplate, FeedbackRespuesta)
- Refactor controllers → Minimal API (gradual)

**Fase B — Agent Architecture (Sprint 2-3):**
- Integrar Semantic Kernel reemplazando OpenAI SDK directo
- Crear 3 agentes especializados desde el agente genérico actual (repartir las 13 tools)
- Implementar router semántico + LLM
- Implementar Prompt Registry y migrar prompts hardcoded
- Agregar persistencia de conversaciones

**Fase C — Quality & Eval (Sprint 4-5):**
- Implementar tabla FeedbackRespuesta + UI de feedback
- Construir golden set con abogados
- Implementar LLM-as-judge + pipeline de evaluación
- Agregar LLM re-ranking, confidence scores
- Implementar semantic caching + circuit breaker (Polly)

**Fase D — New Features (Sprint 6+):**
- Gestión de plazos + calendario legal
- Análisis de riesgo
- Generación de informes
- Alertas avanzadas
- Azure Functions (migrar workers gradualmente)

### 7.3 Lo que NO se debe tocar (funciona bien)

- Pipeline de ingesta (Discoverer → Fetcher → Parser → Enrichment → Indexer) — solo extender
- Crawlers de CSJN y SAIJ — funcionan y producen datos
- Modelo de datos core (Ruling, Statute, Person, Court, Citation, NormRelation, ThesaurusTerm)
- Strategy pattern por fuente
- DLQ management
- Búsqueda híbrida en Azure AI Search
- Community detection + summarization
- Frontend: chat, graph explorer, búsqueda, panel admin

---

## 8. Métricas de Reutilización

| Componente | Archivos estimados | Reutilización | Acción |
|---|---|---|---|
| Core/Entities + Enums | ~80 archivos | **90%** | Migrar + agregar entidades faltantes |
| Core/Interfaces | ~50 archivos | **80%** | Migrar + extender para Semantic Kernel |
| Infrastructure/Crawling | ~15 archivos | **95%** | Migrar tal cual |
| Infrastructure/AI | ~8 archivos | **40%** | Rewrite parcial para Semantic Kernel |
| Infrastructure/Graph | ~3 archivos | **80%** | Migrar + agregar SQL Graph edges |
| Infrastructure/Search | ~5 archivos | **90%** | Migrar + agregar re-ranking |
| Infrastructure/Persistence | ~40 archivos | **85%** | Migrar + agregar configs nuevas |
| Workers (5 proyectos) | ~30 archivos | **70%** | Migrar, evaluar migración a Functions |
| Application (CQRS) | ~50 archivos | **75%** | Migrar + agregar nuevos handlers |
| API Controllers | ~15 archivos | **60%** | Refactor a Minimal API |
| Frontend components | ~100+ archivos | **80%** | Migrar + agregar vistas nuevas |
| Tests | ~40 archivos | **80%** | Migrar + expandir cobertura |

**Estimación global: ~78% del código del MVP es reutilizable directamente o con adaptaciones menores.**

---

*Gap Analysis — MVP (legal-ai-ar) vs Legal Ai Ar — Mayo 2026*
