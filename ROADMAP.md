# Roadmap — Legal AI AR

---

## Documentación del proyecto

| Documento | Descripción |
|---|---|
| **ROADMAP.md** | Este archivo. Hoja de ruta, fases y decisiones arquitecturales. |
| **FEATURES-BASELINE.md** | Baseline de funcionalidades: confirmadas del POC, requeridas para v1, descartadas, gaps y priorización de Fase 1. **Punto de partida para todo el diseño de v1.** |
| **ARCHITECTURE.md** | Arquitectura general del sistema. |
| **KB-CORE.md** | Diseño de la Knowledge Base (4 capas de storage). |
| **WORKERS.md** | Pipeline de workers .NET 8. |

---

## Stack Tecnológico Definitivo

| Capa | Tecnología |
|---|---|
| **Backend** | .NET 8 (ASP.NET Core Web API + MVC + Worker Services) |
| **ORM** | Entity Framework Core 8 |
| **Frontend** | Angular 17+ (TypeScript) |
| **Blob Storage** | Azure Storage Account |
| **Base de datos relacional** | Azure SQL Database |
| **Búsqueda vectorial + FTS** | Azure AI Search |
| **LLM + Embeddings** | Azure OpenAI Service (gpt-4o + text-embedding-3-large) |
| **Message Queue** | Azure Service Bus |
| **Graph DB** | Neo4j Community Edition ⚠️ diferido a Fase 2+ |
| **Hosting** | Azure Container Apps |
| **Secretos** | Azure Key Vault |
| **Observabilidad** | Azure Monitor + Application Insights |

---

## Decisiones tomadas en baseline (Marzo 2026)

Las siguientes decisiones surgen del análisis del POC y están documentadas
en `FEATURES-BASELINE.md`. Impactan directamente el alcance de cada fase.

| Decisión | Detalle |
|---|---|
| **Todo el backend en C# / .NET 8** | El POC Python/FastAPI queda archivado como referencia. No se despliega en producción. |
| **Frontend Angular reestructurado** | Se preserva el design system PwC AppKit4 (paleta, tipografía, tokens). Los contratos de API se rediseñan. |
| **Neo4j diferido** | El grafo de conocimiento jurídico se incorpora en Fase 2 o posterior. |
| **Autenticación diferida** | No se implementa auth en Fase 1. Se recomienda definir el modelo de roles como preparación. |
| **Tesauro diferido** | El pipeline de tesauro jurídico se incorpora en Fase 2 o posterior. |
| **Infoleg diferido** | La ingesta de normativa (leyes, decretos) se incorpora en Fase 2 o posterior. |
| **Admin Panel es Must Have** | El panel de administración MVC es requerido en Fase 1, no en Fase 2. |
| **Asistente IA es Must Have** | El chat RAG sobre jurisprudencia es feature central de Fase 1. |
| **Segmentación estructural requiere spike** | El primer enfoque LLM fue descartado. Se debe definir nuevo enfoque antes de implementar. |

> Ver decisiones completas y justificaciones en `docs/FEATURES-BASELINE.md`

---

## Fases del Proyecto

### 🔵 Fase 0 — Diseño y Arquitectura (COMPLETADA)

- [x] Arquitectura general del sistema
- [x] Diseño de KB Core (4 capas de storage)
- [x] Modelo de datos: JudicialCase
- [x] Pipeline de workers (.NET 8)
- [x] Stack tecnológico definitivo (.NET + Azure + Neo4j CE)
- [x] POC validado: ingesta CSJN + búsqueda híbrida + análisis LLM × 5 + SPA Angular
- [x] **FEATURES-BASELINE.md** — baseline de funcionalidades (36 features priorizados, 9 gaps)
- [ ] Diseño de API (endpoints, contratos)
- [ ] Diseño de SPA Angular (flujos, pantallas)
- [ ] ADR-001: estructura de solución .NET
- [ ] ADR-002: estrategia de chunking / segmentación estructural
- [ ] ADR-003: estrategia de autenticación (Azure AD vs API Keys) — para Fase 2
- [ ] Setup del repositorio, CI/CD (Azure DevOps / GitHub Actions)
- [ ] **Spike**: nuevo enfoque de segmentación estructural por tipo de documento
- [ ] **Spike**: análisis de endpoints auxiliares CSJN para enriquecimiento

---

### 🟡 Fase 1 — KB Core + Ingesta + API + Admin + SPA

**Objetivo**: Producto funcional con todas las funcionalidades Must Have del baseline.
Ver priorización completa en `docs/FEATURES-BASELINE.md §6`.

**Infraestructura Azure**:
- [ ] Provisionar Azure SQL Database + migraciones EF Core
- [ ] Provisionar Azure Storage Account + contenedor `legal-documents`
- [ ] Provisionar Azure AI Search + índice `judicial-cases`
- [ ] Provisionar Azure OpenAI Service (deployments: embedding + chat)
- [ ] Provisionar Azure Service Bus + queues
- [ ] Azure Key Vault con todos los secrets

> ⚠️ Neo4j CE **no se provisiona en Fase 1** — diferido a Fase 2 (ver decisiones de baseline).

**KbCore Library** (`LegalAiAr.KbCore`):
- [ ] `IBlobStorageClient` + implementación Azure
- [ ] `LegalAiDbContext` + entidades + migraciones
- [ ] `ISearchClient` + implementación Azure AI Search
- [ ] `IEmbeddingService` + implementación Azure OpenAI

> ⚠️ `IGraphClient` + Neo4j **diferido a Fase 2**.

**Scraper** (migrado del POC .NET):
- [ ] `CrawlerWorker` — CSJN (migrar lógica validada en POC a arquitectura .NET)
- [ ] `CrawlerWorker` — SAIJ (nueva fuente)
- [ ] Ejecución programada automática (sin intervención manual)
- [ ] Carga directa a Azure Blob Storage

**Workers de ingesta**:
- [ ] `ParserWorker`: extracción de texto PDF (PdfPig) + sanitización + split de párrafos
- [ ] `EnrichmentWorker`: análisis con GPT-4o (resumen, clasificación, citas, argumentos, timeline)
- [ ] `IndexerWorker`: embeddings + indexación híbrida Azure AI Search
- [ ] Gestión de jobs y ciclo de vida (PENDING → PROCESSING → COMPLETED / FAILED)
- [ ] File versioning
- [ ] Deduplicación cross-fuente (CSJN / SAIJ)
- [ ] Segmentación estructural por tipo de documento (pendiente resultado de spike)
- [ ] Persistencia de resultados de análisis LLM (evita re-ejecución costosa)
- [ ] Backfill histórico del corpus

**API REST** (`/api/v1`):
- [ ] `GET /cases/{id}` — fallo por ID
- [ ] `GET /cases` — listado paginado con filtros
- [ ] `GET /cases/search` — búsqueda híbrida semántica + keyword
- [ ] `GET /cases/{id}/analysis` — análisis LLM × 5 (resumen, clasificación, citas, argumentos, timeline)
- [ ] `POST /chat` — asistente IA conversacional (RAG multi-turno)
- [ ] `GET /jobs`, `GET /jobs/{id}` — monitoreo de jobs
- [ ] `GET /health` — health checks de todas las capas
- [ ] `GET /metrics` — métricas para Dashboard
- [ ] Documentación Swagger / OpenAPI

**Panel Admin MVC** (ASP.NET Core Razor Pages) — **Must Have**:
- [ ] Dashboard de ingesta (jobs, estados, errores)
- [ ] Reintento de jobs fallidos
- [ ] Gestión de fuentes (habilitar/deshabilitar)
- [ ] Logs de actividad reciente
- [ ] Vista del estado del índice de búsqueda

**SPA Angular** (reestructurada — design system PwC AppKit4 preservado):
- [ ] Welcome page
- [ ] Búsqueda semántica con filtros avanzados (fuente, tribunal, materia, fechas)
- [ ] Case Viewer — lectura de párrafos con virtual scroll
- [ ] Case Detail — metadata + PDF + análisis IA × 5
- [ ] Asistente IA conversacional (chat) — **Must Have**
- [ ] Dashboard de métricas
- [ ] Analytics de jurisprudencia

**Testing**:
- [ ] Unit tests por worker y servicio
- [ ] Integration tests sobre KB con dataset de prueba (CSJN diciembre 2024 — 1.370 fallos)

---

### 🟠 Fase 2 — Grafo + Auth + Enriquecimiento

**Grafo de conocimiento** (Neo4j CE):
- [ ] Setup Neo4j CE (Docker / Azure VM)
- [ ] `IGraphClient` + implementación Neo4j en KbCore
- [ ] `IndexerWorker`: linking a Neo4j (casos, jueces, normas citadas)
- [ ] `GET /cases/{id}/related` — fallos relacionados por grafo de citas
- [ ] Explorador de grafo en SPA (D3.js / Cytoscape)
- [ ] Explorador de grafo básico en Admin MVC

**Autenticación y autorización**:
- [ ] ADR-003 resuelto: Azure AD / Entra ID vs JWT propio
- [ ] Autenticación Azure AD / JWT Bearer en API
- [ ] Autenticación MSAL en SPA Angular
- [ ] Modelo de roles: administrador, analista, lector
- [ ] Rate limiting (ASP.NET Core Rate Limiting middleware)

**API adicional**:
- [ ] CRUD completo de tribunales y jueces
- [ ] Búsqueda facetada (filtros por fuero, jurisdicción, instancia)
- [ ] Endpoint de árbol de citas (grafo)

**Workers adicionales**:
- [ ] Crawler: PJN, SCBA
- [ ] Enrichment: detección y linking de citaciones cruzadas
- [ ] Endpoints auxiliares CSJN (según resultado del spike de Fase 0)

**Tesauro jurídico**:
- [ ] ThesaurusWorker: parseo de PDFs de tesauro (semántico + jerárquico)
- [ ] Persistencia de términos en Azure SQL
- [ ] Integración con búsqueda (sinónimos, términos relacionados)
- [ ] `GET /thesaurus` — consulta de términos

**Feedback y mejora continua**:
- [ ] Mecanismo de thumbs up/down en resultados de búsqueda y análisis
- [ ] Registro de interacciones del asistente para evaluación

**Editor de tablas maestras en Admin MVC**:
- [ ] Tribunales, jueces
- [ ] Monitoreo Dead Letter Queue

---

### 🔴 Fase 3 — Analytics Avanzados + Infoleg

**Analytics**:
- [ ] Timeline jurisprudencial por tema
- [ ] Detección de evolución jurisprudencial en un tema
- [ ] Sugerencias de fallos relacionados

**Infoleg — Normativa**:
- [ ] Crawler Infoleg (leyes, decretos, resoluciones)
- [ ] Parser y worker de ingesta de normativa
- [ ] Integración con búsqueda (búsqueda cross-tipo: fallos + normativa)

---

### ⚪ Fase 4 — Expansión de Tipos Documentales

- [ ] Codes nacionales completos
- [ ] Catálogo completo de tribunales
- [ ] Perfiles de jueces
- [ ] Doctrina jurídica
- [ ] Tesauros legales adicionales

---

## ADRs Pendientes

| # | Decisión | Estado |
|---|---|---|
| ADR-001 | Estructura de solución .NET (Clean Architecture / Vertical Slices) | ⏳ Pendiente |
| ADR-002 | Estrategia de segmentación estructural por tipo de documento (reemplaza chunking) | ⏳ Pendiente — requiere spike |
| ADR-003 | Estrategia de autenticación (Azure AD B2C vs Entra ID) — Fase 2 | ⏳ Pendiente |

---

## Gaps abiertos (heredados del baseline)

Ver detalle completo en `docs/FEATURES-BASELINE.md §4`.

| Gap | Criticidad | Fase sugerida |
|---|---|---|
| 4.1 Persistencia de resultados de análisis LLM | Alta | Fase 1 |
| 4.2 Deduplicación de fallos entre fuentes | Alta | Fase 1 |
| 4.3 Estrategia de actualización del corpus | Media | Fase 1 |
| 4.4 Cobertura histórica del corpus (backfill) | Media | Fase 1 |
| 4.5 Escalabilidad del worker de ingesta | Media | Fase 1 |
| 4.6 Endpoints auxiliares CSJN — spike de enriquecimiento | Media | Fase 0 (spike) → Fase 2 |
| 4.7 Nuevo enfoque de segmentación estructural | Alta | Fase 0 (spike) → Fase 1 |
| 4.8 Modelo de roles de usuario | Media | Fase 1 (diseño) → Fase 2 (impl.) |
| 4.9 Feedback de calidad y mejora continua del LLM | Baja | Fase 2 |
