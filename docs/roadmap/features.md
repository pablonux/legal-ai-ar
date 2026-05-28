# Legal Ai Ar — Roadmap de Features

> Sistema de Knowledge Base Legal con Agentes IA para Estudio Jurídico Argentino
>
> **Stack:** Angular 19 (SPA) + .NET 10 + Azure (SQL, AI Search, OpenAI, Functions, Storage)
>
> **Usuarios:** Abogados (acceso completo) · Administrativos (gestión operativa)
>
> **Base:** Evolución del MVP `legal-ai-ar` (~78% código reutilizable)
>
> **Versión del documento:** 2.0 — Mayo 2026

---

## Índice

0. [Release 0.0 — Fase 0: Migración MVP y Entorno](#0-release-00--fase-0-migración-mvp-y-entorno)
1. [MVP Baseline — Qué ya existe](#1-mvp-baseline--qué-ya-existe)
2. [Visión General de la Aplicación](#2-visión-general-de-la-aplicación)
3. [Arquitectura del Frontend](#3-arquitectura-del-frontend)
4. [Módulos y Features por Release](#4-módulos-y-features-por-release)
5. [Feature Details — Release 1.0](#5-feature-details--release-10)
6. [Feature Details — Release 2.0](#6-feature-details--release-20)
7. [Feature Details — Release 3.0](#7-feature-details--release-30)
8. [Feature Details — Release 4.0](#8-feature-details--release-40)
9. [Features Transversales](#9-features-transversales)
10. [Stack Técnico Detallado](#10-stack-técnico-detallado)
11. [Matriz de Permisos por Rol](#11-matriz-de-permisos-por-rol)
12. [API Endpoints por Módulo](#12-api-endpoints-por-módulo)
13. [KPIs y Métricas de Éxito](#13-kpis-y-métricas-de-éxito)

---

## 0. Release 0.0 — Fase 0: Migración MVP y Entorno

> **Sprint:** S00 (Pre-desarrollo) | **Equipo:** 1 Backend + 1 Frontend | **Bloqueante para todo el proyecto**

### F00 — Migración del MVP y Estructura de Desarrollo

El MVP `legal-ai-ar` es una base sólida con ~16.000 archivos de código funcional. La Fase 0 evoluciona el monorepo existente, incorpora la documentación del proyecto y prepara el modelo de datos para las nuevas funcionalidades. Los aspectos de infraestructura (CI/CD, IaC, secrets, contenedores) se gestionan por fuera de este roadmap.

| ID | Work Item | Tipo | Asignado a | Estimación |
|----|-----------|------|------------|------------|
| F00-W01 | Documentación Integral | doc | Tech Lead | 5 SP |
| F00-W02 | Reestructurar monorepo: agregar `docs/`, proyecto `Agents`, scaffolding | backend | Dev Backend | 5 SP |
| F00-W03 | Agregar entidades faltantes al modelo de datos (migraciones EF Core) | backend | Dev Backend | 5 SP |
| F00-W04 | Incorporar entidades del MVP no planificadas (Vote, Sumario, etc.) | backend | Dev Backend | 3 SP |
| F00-W05 | Configuración Calidad de Código (analyzers, formatting, pre-commit) | devops | Ambos | 3 SP |
| F00-W06 | Onboarding Guide y documentación de migración | doc | Cualquiera | 2 SP |

**Total:** 23 story points | **Duración estimada:** 1 sprint (2 semanas)

> **Nota:** CI/CD, IaC (Bicep), Azure Key Vault, branching strategy y contenedores de desarrollo se gestionan por fuera de este roadmap.

#### F00-W03 — Entidades nuevas a agregar

| Entidad | Propósito | Requerida para |
|---------|-----------|----------------|
| `Inciso` | Granularidad sub-artículo para RAG | R1.0 — Detalle de Artículo |
| `ArticuloVersion` | Versionado temporal de artículos (SQL Temporal Tables) | R1.0 — Historial de modificaciones |
| `Conversacion` + `MensajeChat` | Persistencia de conversaciones de chat | R1.0 — Chat básico |
| `PromptTemplate` | Prompt Registry en SQL para templates dinámicos | R2.0 — Agentes especializados |
| `FeedbackRespuesta` | Feedback thumbs up/down por respuesta de agente | R2.0 — Feedback |
| `Movimiento` | Timeline de eventos procesales en expedientes | R2.0 — Gestión de expedientes |
| `Plazo` | Plazos procesales con cálculo de días hábiles | R2.0 — Gestión de plazos |
| `UsuarioPreferencias` | Preferencias del usuario (rama, alertas, tema) | R2.0 — Personalización |
| `TaxonomiaLegal` | Taxonomía controlada (extiende ThesaurusTerm) | R1.0 — Clasificación |
| `AnalisisRiesgo` | Persistencia de análisis de riesgo generados | R3.0 — Análisis de riesgo |

#### F00-W04 — Entidades del MVP a incorporar al plan Legal Ai Ar

El MVP tiene entidades valiosas que no estaban planificadas originalmente:

| Entidad MVP | Valor | Acción |
|-------------|-------|--------|
| `Vote` (votos de magistrados) | Alto — posición de cada juez en fallos | Incorporar al modelo |
| `ProsecutorOpinion` (dictamen fiscal) | Medio — relevante para penal y administrativo | Incorporar al modelo |
| `Sumario` (headnotes doctrinarios) | Alto — mejora RAG al separar sumario de fallo completo | Incorporar al modelo |
| `GraphCommunity` + `CommunityMembership` | Alto — resúmenes de líneas jurisprudenciales | Incorporar al modelo |
| `CrawlerConfig` (config por fuente) | Alto — gestión de crawlers desde UI admin | Incorporar al modelo |
| `EmbeddingConfig` (config de modelos) | Medio — cambiar modelo/dimensiones sin deploy | Incorporar al modelo |
| `FieldProvenance` (provenance por campo) | Alto — trazabilidad más granular que DataProvenance | Adoptar en lugar de DataProvenance |
| `ChunkEntityMention` | Alto — entidades mencionadas en cada chunk para RAG | Incorporar al modelo |

### Decisiones Técnicas de Fase 0

| Decisión | Elección |
|---|---|
| Estrategia | Evolucionar MVP in-place, no reescribir (~78% reutilizable) |
| Estructura de repo | Monorepo existente `legal-ai-ar` (`/backend` + `/frontend` + `/docs`) |
| Backend | .NET 10 LTS, Clean Architecture 4 capas, Minimal API (refactor gradual desde Controllers) |
| Frontend | Angular 19 standalone, PwC AppKit 4 (ya configurado en MVP) |
| DB Strategy | Code-First EF Core (migrar esquema MVP + nuevas entidades) |
| Testing Backend | xUnit + NSubstitute (ya en MVP) + FluentAssertions |
| Testing Frontend | Jest + Angular Testing Library + Playwright (E2E) |
| Mediator | Custom IMediator del MVP (evaluar migración a MediatR en R2.0) |

---

## 1. MVP Baseline — Qué ya existe

> El MVP `legal-ai-ar` es un proyecto sorprendentemente maduro con un pipeline de ingesta de 6 etapas, chat agéntico con tool calling y 13 herramientas, búsqueda híbrida, detección de comunidades en grafo, tesauro SAIJ integrado, y un frontend Angular completo con ~15 vistas funcionales.

### 1.1 Pipeline de Ingesta (funcional — reutilizar tal cual)

| Stage | Componente | Descripción |
|-------|------------|-------------|
| 1 | **Discoverer** | Descubre documentos en fuentes con strategy pattern (`IDiscoverStrategy`) |
| 2 | **Fetcher** | Descarga PDFs/HTML con cache en Blob Storage |
| 3 | **Parser** | Extrae texto + metadata (PdfPig para PDFs, regex para HTML) |
| 4 | **Enrichment** | LLM enrichment con GPT-4o-mini (metadata, NER, clasificación) |
| 5 | **Persister** | Persiste entidades en Azure SQL vía EF Core |
| 6 | **Indexer** | Genera embeddings, indexa en AI Search, resuelve citas, extrae menciones |

Fuentes implementadas: CSJN (Sumarios, Acuerdos, Fallos Destacados), SAIJ (Jurisprudencia, Legislación).

Componentes de soporte: 5 Azure Storage Queues entre stages, DLQ con UI admin, external download cache, DocumentStageLog para tracking, Contextual Retrieval en ingesta, Community Detection (Union-Find) + Summarization (LLM).

### 1.2 Modelo de Datos (44 entidades + 30 enums)

Entidades core reutilizables: `Statute`, `Ruling`, `Person`, `Court`, `JudicialOffice`, `Citation` (6 tipos), `NormRelation` (4 tipos), `ThesaurusTerm` + `ThesaurusRelation`, `LegalDoctrine`, `JudicialProceeding`, `Vote`, `ProsecutorOpinion`, `Sumario`, `Keyword`, `LegalRepresentation`, `GraphCommunity`, `CommunityMembership`, `FieldProvenance`, `ChunkEntityMention`, `CrawlerConfig`, `EmbeddingConfig`, `DocumentStageLog`.

### 1.3 AI / RAG / Chat (funcional — evolucionar)

| Capacidad | Estado | Notas |
|-----------|--------|-------|
| Hybrid Search (BM25 + vector) | ✅ Implementado | 3 índices: rulings, chunks, statutes |
| Contextual Retrieval | ✅ Implementado | Contexto por chunk generado en ingesta |
| Tool Calling (13 herramientas) | ✅ Implementado | 1 agente genérico con function calling |
| SSE Streaming | ✅ Implementado | Eventos: text, tool_start, tool_end, validation, done |
| Input Guardrails | ✅ Implementado | 2 capas: rule-based + LLM classifier |
| Output Guardrails | ✅ Implementado | Validación de citas contra DB |
| Query Preprocessing | ✅ Implementado | GPT-4o-mini + expansión con tesauro SAIJ |
| Community Detection | ✅ Implementado | Union-Find + clustering por rama legal |
| Community Summarization | ✅ Implementado | Sumarios generados por LLM |

### 1.4 Frontend (Angular — ~15 vistas funcionales)

| Vista MVP | Cobertura Legal Ai Ar | Feature |
|-----------|-------------------|---------|
| Login + guard + interceptor | 90% | F01 Auth |
| `estadisticas` — KB stats | 70% | F02 Dashboard |
| `ordenamiento` — statutes list + detail | 80% | F03/F05 Búsqueda + Detalle Normas |
| `jurisprudencia` — search + results + detail | **95%** | F04 Búsqueda Jurisprudencia |
| `asistente` — chat con SSE streaming | **90%** | F08 Chat |
| `procesos` — proceeding list + detail | 60% | F12 Expedientes (solo lectura) |
| `explorador` — graph explorer (Cytoscape) | **90%** | F21 Grafo Legal |
| Command palette (Ctrl+K) | **80%** | FT02 Omnisearch |
| `admin/*` — jobs, DLQ, reprocess, workers | **95%** | F19 Admin Ingesta |
| `organismos`, `sujetos`, `vocabulario` | 70% | Vistas auxiliares |
| `ontologia` | 60% | Vista de ontología |

### 1.5 Lo que NO existe en el MVP (gaps principales)

| Gap | Impacto | Release |
|-----|---------|---------|
| Semantic Kernel (usa OpenAI SDK directo) | Alto — rewrite capa de agentes | R2.0 |
| 3 agentes especializados (1 genérico actual) | Alto — routing, prompts dedicados | R2.0 |
| Prompt Registry (prompts hardcoded en C#) | Alto — gestión dinámica de prompts | R2.0 |
| Evaluación (golden set, LLM-as-judge) | Alto — sin framework de calidad | R2.0 |
| Feedback loop (thumbs up/down) | Medio — sin mejora continua | R2.0 |
| Persistencia de conversaciones | Alto — chat stateless | R1.0 |
| LLM Re-ranking | Medio — solo ranking por score | R2.0 |
| Confidence score por respuesta | Medio — sin indicador de confianza | R2.0 |
| Semantic caching | Medio — sin cache semántico | R2.0 |
| Gestión de plazos / calendario | Alto — feature core nuevo | R2.0 |
| Análisis de riesgo | Alto — feature de R3.0 | R3.0 |
| Observabilidad (OpenTelemetry) | Alto — sin tracing ni métricas | R4.0 |

---

## 2. Visión General de la Aplicación

Legal Ai Ar es una aplicación SPA (Single Page Application) que combina una knowledge base del sistema legal argentino con un sistema de agentes IA especializados. Permite a abogados y personal administrativo buscar legislación y jurisprudencia, gestionar expedientes y plazos, consultar agentes IA y generar análisis de riesgo legal.

### 2.1 Objetivos del Producto

- Centralizar el acceso a normas, jurisprudencia y doctrina del ordenamiento jurídico argentino.
- Reducir el tiempo de búsqueda legal de horas a minutos mediante búsqueda semántica e IA.
- Eliminar la pérdida de plazos procesales con alertas y seguimiento automatizado.
- Proveer análisis de riesgo legal basado en datos para mejorar la toma de decisiones.
- Generar documentos e informes legales de forma automatizada.

### 2.2 Roles de Usuario

| Rol | Descripción | Acceso |
|-----|-------------|--------|
| **Abogado** | Profesional del estudio con acceso completo a todas las funcionalidades | Búsqueda, agentes IA, expedientes, análisis de riesgo, informes, configuración de alertas |
| **Administrativo** | Personal de soporte con acceso a gestión operativa | Expedientes, plazos, notificaciones, calendario, generación de reportes operativos |

---

## 3. Arquitectura del Frontend

### 3.1 Angular 19 SPA

| Aspecto | Decisión | MVP |
|---------|----------|-----|
| **Framework** | Angular 19 con standalone components (sin NgModules) | ✅ Ya en MVP |
| **State Management** | Angular Signals + NgRx Signal Store para estado global | Parcial — agregar Signal Store |
| **Routing** | Lazy loading por feature module con `loadComponent()` | ✅ Ya en MVP |
| **UI Library** | PwC AppKit 4 + Tailwind CSS 4 para layout | ✅ Ya en MVP (parcial) |
| **Formularios** | Reactive Forms con tipado estricto (Typed Forms) | ✅ Ya en MVP |
| **HTTP** | `HttpClient` con interceptors funcionales para auth y error handling | ✅ Ya en MVP |
| **Real-time** | SignalR client para notificaciones push y respuestas de agentes | Agregar |
| **Auth** | MSAL Angular (Microsoft Authentication Library) con Entra ID | Parcial — migrar de JWT custom |
| **i18n** | Español (AR) como idioma único, con soporte preparado para expansión | ✅ |
| **Testing** | Jest (unit) + Playwright (e2e) | Parcial — agregar Playwright |
| **Build** | esbuild (default en Angular 19), SSG para landing page | ✅ |

### 3.2 Estructura del Proyecto Angular

```
src/
├── app/
│   ├── core/                      # Servicios singleton, guards, interceptors
│   │   ├── auth/                  # AuthService, AuthGuard, MSAL config
│   │   ├── interceptors/          # AuthInterceptor, ErrorInterceptor, LoadingInterceptor
│   │   ├── services/              # ApiService, SignalRService, NotificationService
│   │   └── models/                # Interfaces y tipos compartidos
│   ├── shared/                    # Componentes reutilizables, pipes, directivas
│   │   ├── components/            # SearchBar, DataTable, AlertBadge, ConfirmDialog
│   │   ├── pipes/                 # FechaLegalPipe, TruncatePipe, HighlightPipe
│   │   └── directives/            # RoleDirective, TooltipDirective
│   ├── features/                  # Feature modules (lazy loaded)
│   │   ├── dashboard/             # Dashboard principal
│   │   ├── busqueda/              # Búsqueda de normas y jurisprudencia
│   │   ├── expedientes/           # Gestión de expedientes y causas
│   │   ├── agentes/               # Chat con agentes IA
│   │   ├── riesgo/                # Análisis de riesgo legal
│   │   ├── calendario/            # Calendario de plazos y vencimientos
│   │   ├── informes/              # Generación de informes y reportes
│   │   ├── normas/                # Explorador de normas (detalle, grafo)
│   │   ├── grafo/                 # Explorador de grafo legal
│   │   ├── admin/                 # Administración (usuarios, config, ingesta)
│   │   └── alertas/               # Centro de notificaciones y alertas
│   ├── layout/                    # Shell, sidebar, navbar, footer
│   └── app.config.ts              # Configuración standalone
├── assets/
├── environments/
└── styles/                        # Tailwind config, themes, variables
```

---

## 4. Módulos y Features por Release

### Release Map

| Release | Nombre | Semanas | Foco | Estrategia |
|---------|--------|---------|------|------------|
| **0.0** | Preparación | S00 (2 sem) | Reestructurar repo + modelo de datos | Docs + entidades nuevas + calidad de código |
| **1.0** | Foundation | S01-S06 (6 sem) | Búsqueda + Chat básico + Grafo | Evolucionar MVP existente |
| **2.0** | Agents | S07-S12 (6 sem) | Agentes IA + Expedientes + Plazos | SK + agentes especializados + case mgmt |
| **3.0** | Risk | S13-S16 (4 sem) | Análisis de riesgo + Informes | Features nuevas sobre base de agentes |
| **4.0** | Operations | S17-S20 (4 sem) | Observabilidad + Alertas + PWA | Ops, monitoreo, hardening |

> **Nota:** La existencia del MVP reduce R1.0 de 8 a 6 semanas. Muchas features de R1.0 tienen 70-95% de cobertura del MVP y solo requieren evolución, no desarrollo desde cero.

### Feature por Release (con cobertura MVP)

| ID | Feature | Release | MVP | Acción |
|----|---------|---------|-----|--------|
| F01 | Autenticación y Autorización | 1.0 | 90% | Evolucionar: JWT custom → MSAL |
| F02 | Dashboard Principal | 1.0 | 70% | Evolucionar: stats → widgets personalizados |
| F03 | Búsqueda de Normas | 1.0 | 80% | Evolucionar: agregar scoring profile, facets |
| F04 | Búsqueda de Jurisprudencia | 1.0 | **95%** | Polish: ya casi completa |
| F05 | Detalle de Norma | 1.0 | 80% | Evolucionar: agregar timeline de modificaciones |
| F06 | Detalle de Artículo | 1.0 | 0% | **Nuevo**: vista standalone con incisos |
| F07 | Novedades Normativas | 1.0 | 0% | **Nuevo**: feed + suscripción por rama |
| F08 | Chat con Agentes IA (básico) | **1.0** | **90%** | Evolucionar: agregar persistencia + citación mejorada |
| F21 | Explorador de Grafo Legal | **1.0** | **90%** | Polish: ya funcional con Cytoscape |
| F09 | Agente Normativo | 2.0 | 0% | **Nuevo**: plugin SK especializado |
| F10 | Agente Jurisprudencial | 2.0 | 0% | **Nuevo**: plugin SK especializado |
| F11 | Agente Procesal | 2.0 | 0% | **Nuevo**: plugin SK especializado |
| F12 | Gestión de Expedientes | 2.0 | 60% | Evolucionar: agregar CRUD, movimientos, docs |
| F13 | Gestión de Plazos | 2.0 | 0% | **Nuevo**: cálculo hábiles + alertas |
| F14 | Calendario Legal | 2.0 | 0% | **Nuevo**: FullCalendar |
| F19 | Administración de Usuarios | **2.0** | 70% | Evolucionar: agregar roles, auditoría |
| F22 | Feedback y Mejora de Agentes | **2.0** | 0% | **Nuevo**: thumbs + correcciones |
| F15 | Análisis de Riesgo Legal | 3.0 | 0% | **Nuevo**: agente de riesgo |
| F16 | Historial de Análisis de Riesgo | 3.0 | 0% | **Nuevo**: re-análisis |
| F17 | Generación de Informes Legales | 3.0 | 0% | **Nuevo**: .docx desde templates |
| F18 | Reportes Operativos | 3.0 | 30% | Evolucionar: stats → charts + export |
| F20 | Configuración de Alertas Avanzadas | 4.0 | 0% | **Nuevo**: wizard + email |
| F23 | Modo Offline (PWA) | 4.0 | 0% | **Nuevo**: service worker |

---

## 5. Feature Details — Release 1.0

> **Foundation** — Evolución del MVP: búsqueda inteligente + chat básico + grafo
>
> **Estrategia:** La mayoría de features en este release ya existen en el MVP con 70-95% de cobertura. El trabajo es de evolución, polish y agregado de funcionalidades faltantes — no de desarrollo desde cero.

### F1.1 — Autenticación y Autorización

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟢 90% — Login funcional con JWT custom, guards de ruta, interceptor de auth |
| **Delta** | Migrar de JWT custom a MSAL Angular + Microsoft.Identity.Web. Agregar role claims desde Entra ID. |
| **Descripción** | Login con Microsoft Entra ID. Soporte para roles Abogado y Administrativo. Guards de ruta por rol. |
| **Backend** | .NET 10 Minimal API con Microsoft.Identity.Web. JWT validation. Role claims desde Entra ID. |
| **Frontend** | MSAL Angular 4.x. Reutilizar `AuthInterceptor` y `AuthGuard` del MVP, adaptar a MSAL. |
| **Aceptación** | Login SSO funcional. Rutas protegidas por rol. Token refresh automático. Logout limpio. |

### F1.2 — Dashboard Principal

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟡 70% — Vista `estadisticas` con stats de la KB (conteos, gráficos básicos) |
| **Delta** | Transformar stats en dashboard personalizado por rol con widgets: plazos, búsquedas recientes, alertas, novedades. |
| **Descripción** | Vista principal post-login con resumen de actividad: plazos próximos a vencer, últimas búsquedas, expedientes activos, alertas pendientes, novedades normativas. |
| **Componentes** | `DashboardComponent` con widgets: `PlazosWidgetComponent`, `BusquedasRecientesComponent`, `AlertasWidgetComponent`, `NovedadesNormativasComponent`. |
| **Backend** | Endpoint agregador: `GET /api/dashboard` que consolida data de múltiples servicios. |
| **Diferencial por rol** | Abogado: todos los widgets + acceso a agentes IA. Administrativo: plazos, expedientes, calendario. |

### F1.3 — Búsqueda de Normas

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟡 80% — Vista `ordenamiento` con listado y detalle de statutes, búsqueda híbrida funcional |
| **Delta** | Agregar scoring profile con boost por vigencia/jerarquía. Agregar facets dinámicos. Mejorar highlighting. |
| **Descripción** | Buscador semántico de legislación argentina. Búsqueda por texto libre (lenguaje natural), filtros por rama del derecho, jurisdicción, vigencia, tipo de norma, rango de fechas. Resultados con highlighting y snippets relevantes. |
| **Backend** | `POST /api/buscar/normas` → Azure AI Search (hybrid: BM25 + vectores). Scoring profile con boost por vigencia y jerarquía normativa. Facets para filtros dinámicos. |
| **Frontend** | Evolucionar `SearchBarComponent` del MVP: agregar autocompletado con debounce 300ms. Agregar `FiltrosLateralesComponent` con facet counts. Mejorar cards de resultado con highlight. |
| **Aceptación** | Búsqueda en < 2 segundos. Resultados relevantes en top 5. Filtros funcionales con conteo. Paginación con 20 resultados/página. |

### F1.4 — Búsqueda de Jurisprudencia

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟢 **95%** — Vista `jurisprudencia` con search, results, detail casi completa |
| **Delta** | Mínimo: agregar filtro por voces (descriptores temáticos) y chips de artículos citados clickeables. |
| **Descripción** | Buscador semántico de fallos judiciales. Búsqueda por texto libre, tribunal, fuero, fecha, voces (descriptores temáticos). Vista de resultado con extracto del fallo y artículos interpretados. |
| **Backend** | Reutilizar endpoints existentes. Agregar relación con artículos via SQL Graph (Edge `interpretaArticulo`). |
| **Frontend** | Reutilizar `BusquedaJurisprudenciaComponent`. Agregar filtros: voces (ThesaurusTerm), instancia. Card de resultado: agregar chips de artículos citados clickeables. |
| **Aceptación** | Búsqueda < 2 seg. Links a artículos interpretados funcionales. Filtro por tribunal con autocompletado. |

### F1.5 — Detalle de Norma

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟡 80% — Vista `ordenamiento/:id` con detalle de statute, articulado básico |
| **Delta** | Agregar tabs (Info, Articulado, Historial, Grafo). Agregar timeline de modificaciones. Mejorar grafo de relaciones. |
| **Descripción** | Vista completa de una norma jurídica: metadata, articulado navegable, historial de modificaciones, normas relacionadas (grafo visual). |
| **Backend** | Reutilizar `GET /api/normas/{id}`. Agregar `GET /api/normas/{id}/grafo` → SQL Graph MATCH query. `GET /api/normas/{id}/articulos` con paginación. `GET /api/normas/{id}/historial`. |
| **Frontend** | Evolucionar detalle a `NormaDetalleComponent` con tabs: Info General, Articulado (virtual scroll), Historial de Modificaciones (timeline), Grafo de Relaciones (reutilizar Cytoscape del explorador MVP). |
| **Aceptación** | Articulado renderiza correctamente. Grafo muestra relaciones hasta 2 niveles. Links entre normas funcionales. |

### F1.6 — Detalle de Artículo

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — Artículos solo existen como string dentro de normas, no como vista standalone |
| **Delta** | Feature completamente nuevo. Requiere entidad `Inciso` (F00-W03). |
| **Descripción** | Vista de un artículo específico con: texto normativo, incisos, jurisprudencia que lo interpreta, historial de modificaciones del artículo. |
| **Backend** | `GET /api/articulos/{id}` → SQL. `GET /api/articulos/{id}/jurisprudencia` → SQL Graph MATCH (Jurisprudencia→interpretaArticulo→Articulo). |
| **Frontend** | `ArticuloDetalleComponent` con secciones: texto vigente, incisos (expandibles), panel lateral con fallos que lo interpretan (ordenados por relevancia y fecha). |
| **Aceptación** | Texto normativo legible. Lista de jurisprudencia relacionada con links funcionales. |

### F1.7 — Novedades Normativas

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No existe feed de novedades |
| **Delta** | Feature completamente nuevo. Puede reutilizar pipeline de ingesta del MVP para detectar nuevas normas. |
| **Descripción** | Feed cronológico de nuevas normas y modificaciones detectadas por el pipeline de ingesta. Filtrable por rama del derecho. Suscripción a alertas por tema. |
| **Backend** | Azure Functions Timer Trigger scrapea Boletín Oficial cada 6hs. `GET /api/novedades?rama=penal&desde=2026-03-01`. SignalR push para novedades en tiempo real. |
| **Frontend** | `NovedadesComponent` con feed tipo timeline. Cada item muestra: tipo (nueva/modificación/derogación), norma afectada, resumen, fecha. Botón "Suscribirse" por rama. |
| **Aceptación** | Novedades del día visibles en < 6 horas desde publicación en Boletín Oficial. Push notifications funcionales. |

### F1.8 — Chat Básico con Agente IA

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟢 **90%** — Vista `asistente` con chat funcional: SSE streaming, tool calling (13 tools), input guardrails, output validation |
| **Delta** | Agregar persistencia de conversaciones (Conversacion + MensajeChat). Mejorar citación inline con links directos. Agregar historial de conversaciones. Mantener agente genérico actual (la especialización viene en R2.0). |
| **Descripción** | Interfaz de chat para interactuar con el agente IA genérico. Respuestas con streaming, citación de fuentes y validación. Historial de conversaciones persistente. |
| **Backend** | Reutilizar `AzureOpenAiAgentChatService` y `ChatQueryHandler` del MVP. Agregar tablas `Conversacion` + `MensajeChat`. Mantener SSE streaming existente. Mejorar formato de citaciones en respuesta. |
| **Frontend** | Reutilizar `AsistenteComponent` del MVP. Agregar: panel lateral de historial de conversaciones, panel de fuentes citadas con links clickeables, botón "Copiar" por mensaje. |
| **UX** | Streaming de respuesta (ya funcional). Fuentes como chips al final de cada respuesta. |
| **Aceptación** | Respuesta comienza a renderizar en < 3 seg. Fuentes siempre presentes y clickeables. Historial persistente entre sesiones. |
| **Rol** | Solo Abogados |

### F1.9 — Explorador de Grafo Legal

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟢 **90%** — Vista `explorador` con Cytoscape.js funcional, SQL Graph queries, panel lateral |
| **Delta** | Agregar filtros por tipo de relación. Mejorar panel de detalle del nodo. Agregar profundidad configurable. Conectar con detalle de norma/fallo. |
| **Descripción** | Visualización interactiva del grafo de relaciones legales. Navegar relaciones entre normas, artículos, jurisprudencia, órganos. Zoom, pan, filtros por tipo de relación, expansión de nodos on-click. |
| **Backend** | Reutilizar `SqlGraphService` del MVP. Agregar `GET /api/grafo/explorar?nodoId=LEY-26994&profundidad=2&relaciones=modificaA,derogaA`. |
| **Frontend** | Reutilizar `ExploradorComponent` con Cytoscape del MVP. Agregar: filtros por tipo de relación (checkboxes), slider de profundidad, links desde nodo a detalle de norma/fallo. |
| **Aceptación** | Renderiza grafos de hasta 200 nodos fluido. Zoom/pan funcional. Click en nodo muestra detalle. Filtros por tipo de relación. |
| **Rol** | Solo Abogados |

---

## 6. Feature Details — Release 2.0

> **Agents** — Migración a Semantic Kernel, agentes especializados, gestión de expedientes y plazos
>
> **Estrategia:** Este es el release con mayor trabajo nuevo. Se reemplaza el agente genérico del MVP por 3 agentes especializados orquestados con Semantic Kernel, y se agregan las funcionalidades de gestión de casos (expedientes, plazos, calendario).

### F2.1 — Migración a Semantic Kernel + Agentes Especializados

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — MVP usa Azure OpenAI SDK directo con `AzureOpenAiAgentChatService` |
| **Delta** | Rewrite completo de la capa de agentes: reemplazar OpenAI SDK por Semantic Kernel, crear 3 plugins especializados, implementar router híbrido, migrar las 13 tools existentes a [KernelFunction]. |
| **Descripción** | Migrar la orquestación de agentes de OpenAI SDK directo a Semantic Kernel. Implementar router híbrido (semántico + LLM) para derivar consultas al agente adecuado. Implementar patrón ReAct para reasoning multi-paso. |
| **Backend** | Semantic Kernel con plugins tipados en C#. Router de 2 capas: embedding similarity (fast path, confianza > 0.85) + LLM router (fallback). Orchestrator pattern para consultas multi-agente. |
| **Componentes nuevos** | `SemanticKernelOrchestrator`, `HybridRouter`, `AgentMemoryService` (short-term SQL + working SK + long-term preferences). |
| **Aceptación** | Router clasifica correctamente > 90% de consultas. Agentes usan ReAct para consultas complejas. Latencia equivalente o menor al MVP. |

### F2.2 — Agente Normativo (integrado en chat)

| Campo | Detalle |
|-------|---------|
| **MVP** | Parcial — las tools `SearchStatutes`, `GetStatuteDetail`, `CheckNormRelations` existen pero en agente genérico |
| **Delta** | Crear plugin `AgenteNormativoPlugin` con system prompt especializado. Migrar tools relevantes del MVP. Agregar `VerificarVigencia()`, `CadenaDerogaciones()`. |
| **Descripción** | Responde consultas sobre legislación vigente. Ejemplos: "¿Cuál es el plazo de prescripción para un reclamo laboral?", "¿Qué artículos del CCCN regulan la locación?", "¿La ley 27.742 modificó el régimen de despido?". |
| **Backend** | Semantic Kernel plugin `AgenteNormativoPlugin` con functions: `BuscarNorma()`, `VerificarVigencia()`, `ObtenerArticulo()`, `RastrearModificaciones()`, `CadenaDerogaciones()`. Usa AI Search + SQL Graph. |
| **Aceptación** | Respuestas con cita de artículos específicos. Detecta correctamente si una norma fue derogada o modificada. |

### F2.3 — Agente Jurisprudencial (integrado en chat)

| Campo | Detalle |
|-------|---------|
| **MVP** | Parcial — tools `SearchRulings`, `GetRulingDetail`, `SearchChunks`, `GetCommunityInfo` existen en agente genérico |
| **Delta** | Crear plugin `AgenteJurisprudencialPlugin`. Migrar tools. Agregar `AnalizarDoctrina()`, `TendenciaJurisprudencial()`. |
| **Descripción** | Busca y analiza fallos judiciales. Ejemplos: "¿Qué dice la CSJN sobre el despido discriminatorio?", "Precedentes recientes sobre responsabilidad médica en CABA". |
| **Backend** | Plugin `AgenteJurisprudencialPlugin` con: `BuscarFallo()`, `AnalizarDoctrina()`, `IdentificarPrecedentes()`, `TendenciaJurisprudencial()`. RAG sobre AI Search. Graph traversal para relación fallo→artículo. |
| **Aceptación** | Cita fallos con carátula, tribunal y fecha. Distingue ratio decidendi de obiter dictum. Identifica tendencia (favorable/desfavorable). |

### F2.4 — Agente Procesal (integrado en chat)

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No hay tools procesales en el agente genérico |
| **Delta** | Feature completamente nuevo. Requiere entidades `Movimiento` y `Plazo` (F00-W03). |
| **Descripción** | Consultas sobre expedientes y plazos. Ejemplos: "¿Cuánto falta para que venza el plazo de contestación en el expediente 12345?", "Listame las causas con vencimientos esta semana". |
| **Backend** | Plugin `AgenteProcesalPlugin` con: `ConsultarExpediente()`, `CalcularPlazo()`, `AlertarVencimiento()`, `ListarCausasActivas()`. Consulta Azure SQL. Motor de cálculo de días hábiles. Calendario de feriados nacionales y judiciales. |
| **Aceptación** | Cálculo correcto de plazos hábiles (excluye fines de semana y feriados nacionales). Alertas funcionales. |
| **Rol** | Abogados y Administrativos |

### F2.5 — Prompt Registry y Gestión de Prompts

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — Prompts hardcoded en clases C# (`ChatSystemPrompt`, `ChunkContextualizationPrompt`, etc.) |
| **Delta** | Feature completamente nuevo. Requiere entidad `PromptTemplate` (F00-W03). |
| **Descripción** | Sistema híbrido de gestión de prompts: YAML files para system prompts base (versionados en Git) + tabla SQL `PromptTemplate` para templates dinámicos con A/B testing. |
| **Backend** | `PromptTemplate` en Azure SQL con: nombre, versión, contenido, variables, modelo target, activo/inactivo, métricas de performance. API: `GET/PUT /api/admin/prompts`. |
| **Frontend** | Vista admin de prompts: listado, editor con preview, toggle A/B, métricas de performance por versión. |
| **Aceptación** | Prompts editables sin deploy. A/B testing funcional. Rollback a versión anterior en < 1 minuto. |

### F2.6 — Evaluación y Calidad de IA

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — Sin framework de evaluación |
| **Delta** | Feature completamente nuevo. |
| **Descripción** | Pipeline de evaluación de calidad de las respuestas de agentes: golden set de 200 queries, LLM-as-judge para scoring automático, regression testing en CI, drift monitoring. |
| **Backend** | Golden set en JSON (200 queries con respuestas esperadas, distribución por rama). LLM-as-judge (GPT-4o evalúa respuestas contra criterios). Pipeline CI: `dotnet test` ejecuta eval en cada PR. Métricas: Recall@10, MRR, faithfulness, citation accuracy. |
| **Aceptación** | Golden set completo (200 queries). LLM-as-judge calibrado (Cohen's Kappa ≥ 0.75 vs. humano). Regression test en CI pasa en < 5 minutos. |

### F2.7 — Gestión de Expedientes

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟡 60% — Vista `procesos` con listado y detalle de JudicialProceeding (solo lectura) |
| **Delta** | Agregar CRUD completo, movimientos (timeline), documentos adjuntos (Blob), vinculación con plazos. Requiere entidad `Movimiento` (F00-W03). |
| **Descripción** | CRUD de expedientes judiciales y administrativos del estudio. Cada expediente tiene: número, carátula, fuero, juzgado, estado, partes, abogado responsable, documentos adjuntos, historial de movimientos. |
| **Backend** | CRUD: `GET/POST/PUT/DELETE /api/expedientes`. Búsqueda con filtros: `GET /api/expedientes?fuero=laboral&estado=en_tramite&abogado=jperez`. Subrecursos: `/api/expedientes/{id}/movimientos`, `/api/expedientes/{id}/documentos`. |
| **Frontend** | Evolucionar vista `procesos` del MVP a `ExpedientesListComponent` con DataTable (sort, filter, paginación). Agregar `ExpedienteDetalleComponent` con tabs: Información, Movimientos (timeline), Documentos (upload/download), Plazos, Notas. `ExpedienteFormComponent` para alta/edición. |
| **Aceptación** | CRUD completo. Búsqueda por cualquier campo. Upload de documentos a Blob Storage. Timeline de movimientos cronológico. |
| **Rol** | Abogados (CRUD completo) · Administrativos (lectura + alta de movimientos) |

### F2.8 — Gestión de Plazos

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No existe gestión de plazos |
| **Delta** | Feature completamente nuevo. Requiere entidad `Plazo` (F00-W03). |
| **Descripción** | Registro y seguimiento de plazos procesales vinculados a expedientes. Cálculo automático de días hábiles. Alertas configurables (X días antes del vencimiento). Estados: pendiente, próximo a vencer, vencido, cumplido. |
| **Backend** | CRUD `/api/plazos`. Cálculo de hábiles con calendario de feriados nacionales y judiciales (Azure SQL). Azure Functions Timer Trigger diario evalúa plazos y genera alertas via Storage Queue → SignalR push. |
| **Frontend** | `PlazosListComponent` con filtros por estado y urgencia. Badges de color: verde (>5 días), amarillo (2-5 días), rojo (<2 días), gris (cumplido). `PlazoFormComponent` con date pickers y cálculo automático de hábiles. |
| **Aceptación** | Cálculo correcto de hábiles. Alertas push 72hs, 48hs y 24hs antes del vencimiento. Feriados judiciales contemplados. |
| **Rol** | Abogados y Administrativos |

### F2.9 — Calendario Legal

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No existe vista calendario |
| **Delta** | Feature completamente nuevo. |
| **Descripción** | Vista calendario (mes/semana/día) con todos los plazos, audiencias y vencimientos del estudio. Filtros por abogado, fuero, expediente. |
| **Backend** | `GET /api/calendario?desde=2026-04-01&hasta=2026-04-30&abogado=all`. Agrega plazos, audiencias y fechas de expedientes. |
| **Frontend** | `CalendarioComponent` con FullCalendar (Angular wrapper). Eventos color-coded por tipo. Click en evento abre detalle del plazo/expediente. Drag & drop para reprogramar audiencias. |
| **Aceptación** | Visualización correcta de eventos. Filtros funcionales. Navegación mes/semana/día fluida. |
| **Rol** | Abogados y Administrativos |

### F2.10 — Administración de Usuarios

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟡 70% — Vista `admin/users` con listado, panel admin de ingesta (DLQ, jobs, workers) al 95% |
| **Delta** | Agregar gestión de roles y permisos por módulo. Agregar auditoría de acceso. Mantener panel admin de ingesta del MVP. |
| **Descripción** | Gestión de usuarios del sistema: asignación de rol, permisos por módulo, auditoría de acceso. Panel admin de ingesta (heredado del MVP). |
| **Backend** | Entra ID para identidades. Azure SQL para permisos custom. `GET/POST/PUT /api/admin/usuarios`. Audit log en Azure SQL. Reutilizar endpoints admin de ingesta del MVP. |
| **Frontend** | Evolucionar `AdminUsuariosComponent` del MVP. Agregar: formulario de edición de rol y permisos, log de auditoría con filtros. Mantener panel admin de ingesta (DLQ, jobs, reprocess). |
| **Rol** | Solo Abogados con permiso de admin |

### F2.11 — Feedback y Mejora de Agentes

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — Sin sistema de feedback |
| **Delta** | Feature completamente nuevo. Requiere entidad `FeedbackRespuesta` (F00-W03). |
| **Descripción** | Sistema de feedback para calificar respuestas de agentes: thumbs up/down, corrección de respuesta, rating de utilidad de fuentes. Los datos alimentan mejora continua de prompts y scoring. |
| **Backend** | `POST /api/feedback` con: conversationId, messageId, rating, corrección. Azure Functions batch job semanal analiza feedback. |
| **Frontend** | Botones de thumbs up/down en cada respuesta del agente. Modal de feedback detallado. Dashboard de feedback para admins. |
| **Aceptación** | Feedback registrado sin fricción (1 click). Dashboard con métricas de satisfacción. Tasa de thumbs up > 80% como target. |

---

## 7. Feature Details — Release 3.0

> **Risk** — Análisis de riesgo + Informes automatizados
>
> **Estrategia:** Features completamente nuevos que se construyen sobre la infraestructura de agentes de R2.0.

### F3.1 — Análisis de Riesgo Legal

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No existe |
| **Descripción** | El usuario describe un caso o situación legal y el sistema genera un análisis de riesgo estructurado. Incluye: evaluación normativa, jurisprudencia favorable/desfavorable, factores de riesgo, probabilidad de éxito estimada, recomendaciones. |
| **Backend** | `POST /api/riesgo/analizar` → Semantic Kernel `AgenteRiesgoPlugin`. Combina outputs de agentes normativo y jurisprudencial. Genera JSON estructurado con scores. Persiste análisis en Azure SQL. |
| **Frontend** | `RiesgoAnalisisComponent` con: formulario de ingreso del caso (textarea + selección de rama/jurisdicción), vista de resultado con secciones colapsables (normativa, jurisprudencia, factores, score), gauge visual de probabilidad de éxito, botón "Generar informe .docx". |
| **Aceptación** | Análisis generado en < 30 seg. Score de riesgo coherente con jurisprudencia citada. Informe exportable. |
| **Rol** | Solo Abogados |

### F3.2 — Historial de Análisis de Riesgo

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No existe |
| **Descripción** | Listado de todos los análisis de riesgo generados por el estudio. Filtrable por rama, fecha, abogado, score de riesgo. Permite re-ejecutar un análisis con datos actualizados. |
| **Backend** | `GET /api/riesgo/historial?rama=laboral&desde=2026-01-01`. Almacenados en Azure SQL con snapshot de las fuentes usadas. |
| **Frontend** | `RiesgoHistorialComponent` con DataTable. Columnas: fecha, caso (resumen), rama, score, abogado. Click abre el análisis completo. Botón "Re-analizar" ejecuta nuevo análisis con KB actualizada. |
| **Aceptación** | Historial paginado y filtrable. Re-análisis funcional con diff visual vs. análisis anterior. |

### F3.3 — Generación de Informes Legales

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No existe |
| **Descripción** | Generación automatizada de documentos .docx a partir de templates: informes de riesgo, dictámenes, resúmenes de jurisprudencia, memos legales. Los agentes IA completan el contenido y el sistema genera el documento formateado. |
| **Backend** | `POST /api/informes/generar` con body: `{tipo, datos, expedienteId?}`. Backend usa DocumentFormat.OpenXml (.NET) para generar .docx desde templates almacenados en Blob Storage. Output guardado en Blob Storage. |
| **Frontend** | `InformesGenerarComponent` con: selector de tipo de informe, formulario dinámico según tipo, preview (rendering del .docx en iframe o PDF viewer), botón de descarga. `InformesListComponent` con historial de informes generados. |
| **Aceptación** | .docx generado correctamente con formato profesional. Preview funcional. Descarga directa. |
| **Rol** | Abogados (todos los tipos) · Administrativos (solo reportes operativos) |

### F3.4 — Reportes Operativos

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟡 30% — Vista `estadisticas` con conteos y stats básicos de la KB |
| **Delta** | Transformar en dashboards completos con gráficos interactivos, filtros y exportación. |
| **Descripción** | Dashboards y reportes de gestión del estudio: cantidad de expedientes por estado, plazos vencidos, carga de trabajo por abogado, tiempo promedio de resolución, estadísticas de uso de agentes IA. |
| **Backend** | `GET /api/reportes/expedientes-por-estado`, `/api/reportes/plazos-vencidos`, `/api/reportes/carga-por-abogado`. Queries agregadas sobre Azure SQL. |
| **Frontend** | `ReportesComponent` con gráficos: barras (expedientes por fuero), pie (estados), line (evolución mensual), heatmap (carga por abogado/semana). Librería: ngx-charts o Chart.js. Exportar a PDF/Excel. |
| **Aceptación** | Gráficos renderizados correctamente. Datos consistentes con la base. Export funcional. |
| **Rol** | Abogados y Administrativos |

---

## 8. Feature Details — Release 4.0

> **Operations** — Observabilidad, alertas avanzadas, hardening
>
> **Estrategia:** Foco en operación productiva: observabilidad completa, alertas configurables, PWA, y migración gradual de workers a Azure Functions.

### F4.1 — Observabilidad y LLMOps

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — Sin observabilidad |
| **Descripción** | Implementar stack completo de observabilidad: OpenTelemetry para distributed tracing, Application Insights para métricas y alertas, telemetría custom para LLM (tokens, latencia, costos), semantic caching, circuit breaker con Polly. |
| **Backend** | OpenTelemetry SDK → Application Insights exporter. Custom counters: `llm.tokens.input`, `llm.tokens.output`, `llm.latency_ms`, `llm.cost_usd`. Semantic cache con TTL por tipo de consulta. Circuit breaker (Polly v8) para Azure OpenAI. |
| **Dashboards** | 6 paneles: Request Overview, LLM Performance, RAG Quality, Pipeline Health, Cost Tracking, Error Analysis. |
| **Alertas** | 6 alertas: latencia P95 > 10s, error rate > 5%, costo diario > umbral, circuit breaker open, feedback negativo > 15%, drift en métricas de eval. |
| **Aceptación** | Tracing end-to-end funcional (request → agente → tool → search → response). Dashboard de costos actualizado en < 1 hora. Circuit breaker protege contra downtime de Azure OpenAI. |

### F4.2 — Configuración de Alertas Avanzadas

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No existe |
| **Descripción** | Los usuarios configuran alertas personalizadas: cambios normativos en ramas específicas, vencimientos de expedientes asignados, nuevos fallos de tribunales de interés, cambios de estado en causas. |
| **Backend** | CRUD `/api/alertas/configuracion`. Azure Functions evalúa condiciones y genera notificaciones via Storage Queue → SignalR. Opciones de canal: push in-app, email. |
| **Frontend** | `AlertasConfigComponent` con wizard: selección de tipo de alerta, configuración de condiciones (rama, tribunal, expediente), canal de notificación, frecuencia. `AlertasCentroComponent` con inbox de alertas (leídas/no leídas, archivadas). |
| **Aceptación** | Alertas generadas dentro de los 30 minutos del evento trigger. Inbox funcional con mark as read. Email delivery funcional. |

### F4.3 — Modo Offline (PWA)

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No existe |
| **Descripción** | Service worker para funcionalidad offline limitada: acceso a normas previamente consultadas, expedientes en caché, plazos del día. Sincronización al reconectar. |
| **Backend** | API soporta ETags y caching headers. Manifest.json para PWA. |
| **Frontend** | Angular PWA con `@angular/service-worker`. Cache strategy: network-first para datos, cache-first para assets. IndexedDB para normas y expedientes favoritos. |
| **Aceptación** | Acceso offline a últimas 50 normas consultadas y expedientes activos. Sync al reconectar sin pérdida de datos. |

### F4.4 — Migración Gradual a Azure Functions

| Campo | Detalle |
|-------|---------|
| **MVP** | Los workers actuales son BackgroundService (polling queues) en el proceso del API |
| **Delta** | Evaluar y migrar selectivamente workers a Azure Functions (event-driven, consumption plan). |
| **Descripción** | Los 5 workers del pipeline de ingesta (Discoverer, Fetcher, Parser, Enrichment, Indexer) actualmente corren como BackgroundService. Migrar a Azure Functions permite scaling independiente, pago por ejecución, y triggers nativos de Storage Queue. |
| **Backend** | Azure Functions .NET 10 isolated worker. Queue triggers reemplazan polling. Mantener strategy pattern por fuente. Timer trigger para Boletín Oficial. |
| **Criterio** | Solo migrar si el volumen de ingesta justifica el overhead de Functions. Si el volumen es bajo, los BackgroundService del MVP son suficientes. |

### F4.5 — Model Versioning y Canary Deploys

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No existe |
| **Descripción** | Versionado de modelos y prompts con canary deploys: 10% tráfico a nueva versión → evaluar métricas → ramp up o rollback automático. |
| **Backend** | `ModelVersionConfig` en SQL. Feature flags para routing de tráfico. Métricas A/B comparativas automáticas. |
| **Aceptación** | Canary deploy funcional. Rollback automático si métricas degradan > 10%. Tiempo de rollback < 5 minutos. |

---

## 9. Features Transversales

Estas features aplican a toda la aplicación y se implementan progresivamente:

### FT.1 — Notificaciones en Tiempo Real

| Campo | Detalle |
|-------|---------|
| **MVP** | 🔴 0% — No existe como sistema de notificaciones de usuario |
| **Delta** | Implementar SignalR para notificaciones de usuario: plazos, novedades normativas, alertas. |
| **Descripción** | Sistema de notificaciones push in-app via SignalR. Badge en navbar con conteo de no leídas. Toast notifications para eventos urgentes (plazos < 24hs). |
| **Backend** | Azure SignalR Service. Hub: `NotificacionHub` con métodos `EnviarAlerta()`, `ActualizarEstado()`. Azure Functions publica en Storage Queue → worker lee y pushea via SignalR. |
| **Frontend** | `NotificacionService` (singleton) mantiene conexión SignalR. `NotificacionBadgeComponent` en navbar. `ToastComponent` para alertas urgentes. `NotificacionCentroComponent` con listado completo. |
| **Implementación** | R1.0: infraestructura base SignalR. R2.0: notificaciones de plazos. R4.0: alertas avanzadas configurables. |

### FT.2 — Búsqueda Global (Omnisearch)

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟢 **80%** — Command palette (Ctrl+K) funcional con búsqueda multi-tipo |
| **Delta** | Agregar búsqueda en expedientes y conversaciones. Mejorar agrupación de resultados. |
| **Descripción** | Buscador unificado accesible con `Ctrl+K` desde cualquier vista. Busca simultáneamente en normas, jurisprudencia, expedientes y conversaciones con agentes. Resultados agrupados por tipo. |
| **Backend** | Reutilizar command palette del MVP. Agregar `GET /api/buscar/global?q=despido+sin+causa` → AI Search multi-index query + Azure SQL para expedientes. |
| **Frontend** | Evolucionar `OmnisearchComponent` del MVP. Agregar grupo de resultados: Expedientes, Conversaciones. Mantener keyboard navigation existente. |
| **Implementación** | R1.0: polish (ya funcional). R2.0: agregar expedientes y conversaciones. |

### FT.3 — Tema y Accesibilidad

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟡 Parcial — PwC AppKit 4 parcialmente configurado |
| **Descripción** | Soporte para tema claro/oscuro. Cumplimiento WCAG 2.1 AA. Responsive design (desktop-first, funcional en tablet). |
| **Frontend** | Angular Material theming con CSS custom properties. `prefers-color-scheme` detection. Focus visible, ARIA labels, semantic HTML. Breakpoints: desktop (>1200px), tablet (768-1200px). |
| **Implementación** | Progresivo a lo largo de todos los releases. |

### FT.4 — Auditoría y Logging

| Campo | Detalle |
|-------|---------|
| **MVP** | 🟡 Parcial — `DocumentStageLog` para tracking de pipeline, logging básico |
| **Delta** | Agregar middleware de auditoría para acciones de usuario. Application Insights para telemetría técnica. |
| **Descripción** | Registro de todas las acciones significativas: búsquedas realizadas, expedientes consultados, documentos descargados, análisis de riesgo generados. Compliance con Ley 25.326 de datos personales. |
| **Backend** | Middleware de auditoría en .NET 10. Eventos escritos en Azure SQL (tabla AuditLog). Application Insights para telemetría técnica. Retención: 2 años operativo, 5 años archivo en Blob Storage (cool tier). |
| **Implementación** | R1.0: middleware base + Application Insights. R4.0: dashboard de auditoría completo. |

---

## 10. Stack Técnico Detallado

### Frontend

| Tecnología | Versión | Uso | MVP |
|------------|---------|-----|-----|
| Angular | 19.x | Framework SPA | ✅ |
| PwC AppKit 4 | latest | UI Library + guidelines | ✅ Parcial |
| Tailwind CSS | 4.x | Utility-first CSS | Agregar |
| NgRx Signal Store | 19.x | State management con signals | Agregar |
| SignalR Client | 9.x | Real-time notifications | Agregar |
| MSAL Angular | 4.x | Autenticación Entra ID | Migrar desde JWT custom |
| ngx-markdown | latest | Rendering de markdown (respuestas agentes) | Agregar |
| Cytoscape.js | 3.x | Visualización de grafos | ✅ |
| FullCalendar | 6.x | Componente calendario | Agregar (R2.0) |
| ngx-charts / Chart.js | latest | Gráficos de reportes | Agregar (R3.0) |
| Jest | 30.x | Unit testing | ✅ |
| Playwright | latest | E2E testing | Migrar desde tests actuales |

### Backend

| Tecnología | Versión | Uso | MVP |
|------------|---------|-----|-----|
| .NET | 10 (LTS) | Runtime | ✅ |
| ASP.NET Core | 10 | Minimal API (refactor gradual desde Controllers) | ✅ Controllers |
| Entity Framework Core | 10 | ORM + SQL Graph mapping | ✅ |
| Semantic Kernel | latest | Orquestación de agentes IA (R2.0) | Reemplaza OpenAI SDK |
| Azure Functions | .NET 10 isolated | ETL, jobs, triggers (R4.0 — evaluar) | Workers como BackgroundService |
| Microsoft.Identity.Web | latest | Auth con Entra ID | Migrar desde JWT custom |
| DocumentFormat.OpenXml | latest | Generación de .docx (R3.0) | Agregar |
| Polly | 8.x | Retry + Circuit breaker | ✅ (solo retry) |
| FluentValidation | 12.x | Validación de requests | ✅ |
| PdfPig | 0.1.x | PDF parsing | ✅ (evaluar migración a Azure Doc Intelligence) |
| SharpToken | 2.x | Tokenización para prompts | ✅ |
| xUnit + NSubstitute | latest | Testing | ✅ |
| Custom IMediator | — | CQRS pattern | ✅ (evaluar MediatR en R2.0) |

### Azure Services

| Servicio | Uso | MVP | Fase |
|----------|-----|-----|------|
| Azure SQL Database | Datos relacionales + Graph Tables | ✅ | Existente |
| Azure AI Search | Búsqueda híbrida (BM25 + vectores) | ✅ (3 índices) | Existente |
| Azure OpenAI Service | Embeddings (3072d) + LLM (GPT-4o, GPT-4o-mini) | ✅ | Existente |
| Azure Storage | Blobs (documentos) + Queues (5 colas de pipeline) | ✅ | Existente |
| Azure Functions | ETL triggers, timer jobs | ❌ | R4.0 (evaluar) |
| Azure App Service | Hosting API + SPA Angular | ✅ | Existente |
| Application Insights | Monitoreo y telemetría | ❌ | R1.0 básico, R4.0 completo |
| Azure SignalR Service | Notificaciones push | Agregar | R1.0 |

---

## 11. Matriz de Permisos por Rol

| Feature | Abogado | Administrativo |
|---------|:-------:|:--------------:|
| Dashboard | Completo | Sin widget de agentes IA |
| Búsqueda de normas | Completo | Completo |
| Búsqueda de jurisprudencia | Completo | Solo lectura |
| Detalle de norma/artículo | Completo | Completo |
| Chat con agentes IA | Completo | Sin acceso |
| Explorador de grafo | Completo | Sin acceso |
| Gestión de expedientes | CRUD completo | Lectura + alta movimientos |
| Gestión de plazos | CRUD completo | CRUD completo |
| Calendario legal | Completo | Completo |
| Análisis de riesgo | Completo | Sin acceso |
| Generación de informes | Todos los tipos | Solo reportes operativos |
| Reportes operativos | Completo | Completo |
| Alertas configurables | Completo | Solo plazos y expedientes |
| Administración de usuarios | Solo con permiso admin | Sin acceso |
| Administración de ingesta | Solo con permiso admin | Sin acceso |
| Feedback de agentes | Completo | Sin acceso |

---

## 12. API Endpoints por Módulo

### Auth

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/auth/me` | Perfil del usuario autenticado |
| GET | `/api/auth/permisos` | Permisos del usuario actual |

### Dashboard

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/dashboard` | Datos agregados del dashboard |
| GET | `/api/dashboard/novedades` | Últimas novedades normativas |

### Búsqueda

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/buscar/normas` | Búsqueda semántica de normas |
| POST | `/api/buscar/jurisprudencia` | Búsqueda semántica de fallos |
| GET | `/api/buscar/global` | Omnisearch unificado |
| GET | `/api/buscar/sugerencias` | Autocompletado de búsqueda |

### Normas

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/normas/{id}` | Detalle de norma |
| GET | `/api/normas/{id}/articulos` | Articulado de la norma |
| GET | `/api/normas/{id}/grafo` | Grafo de relaciones |
| GET | `/api/normas/{id}/historial` | Historial de modificaciones |
| GET | `/api/articulos/{id}` | Detalle de artículo |
| GET | `/api/articulos/{id}/jurisprudencia` | Jurisprudencia relacionada |

### Grafo

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/grafo/explorar` | Explorar grafo desde nodo con profundidad |
| GET | `/api/grafo/comunidades` | Listar comunidades detectadas |
| GET | `/api/grafo/comunidades/{id}` | Detalle de comunidad con sumario |

### Agentes IA

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/agentes/chat` | Enviar mensaje al agente (SSE streaming) |
| GET | `/api/agentes/conversaciones` | Historial de conversaciones |
| GET | `/api/agentes/conversaciones/{id}` | Detalle de conversación |
| DELETE | `/api/agentes/conversaciones/{id}` | Eliminar conversación |

### Expedientes

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/expedientes` | Listar expedientes (con filtros) |
| POST | `/api/expedientes` | Crear expediente |
| GET | `/api/expedientes/{id}` | Detalle de expediente |
| PUT | `/api/expedientes/{id}` | Actualizar expediente |
| DELETE | `/api/expedientes/{id}` | Eliminar expediente |
| GET | `/api/expedientes/{id}/movimientos` | Movimientos del expediente |
| POST | `/api/expedientes/{id}/movimientos` | Registrar movimiento |
| GET | `/api/expedientes/{id}/documentos` | Documentos adjuntos |
| POST | `/api/expedientes/{id}/documentos` | Subir documento |

### Plazos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/plazos` | Listar plazos (con filtros) |
| POST | `/api/plazos` | Crear plazo |
| PUT | `/api/plazos/{id}` | Actualizar plazo |
| PUT | `/api/plazos/{id}/cumplir` | Marcar plazo como cumplido |
| GET | `/api/plazos/proximos` | Plazos próximos a vencer |

### Calendario

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/calendario` | Eventos en rango de fechas |

### Análisis de Riesgo

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/riesgo/analizar` | Generar análisis de riesgo |
| GET | `/api/riesgo/historial` | Historial de análisis |
| GET | `/api/riesgo/{id}` | Detalle de análisis |
| POST | `/api/riesgo/{id}/re-analizar` | Re-ejecutar con datos actuales |

### Informes

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/informes/generar` | Generar informe .docx |
| GET | `/api/informes` | Historial de informes |
| GET | `/api/informes/{id}/descargar` | Descargar informe |

### Reportes

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/reportes/expedientes-por-estado` | Expedientes agrupados por estado |
| GET | `/api/reportes/plazos-vencidos` | Plazos vencidos por período |
| GET | `/api/reportes/carga-por-abogado` | Carga de trabajo por abogado |
| GET | `/api/reportes/uso-agentes` | Estadísticas de uso de agentes IA |

### Alertas

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/alertas` | Alertas del usuario (inbox) |
| PUT | `/api/alertas/{id}/leer` | Marcar alerta como leída |
| GET | `/api/alertas/configuracion` | Configuraciones de alertas |
| POST | `/api/alertas/configuracion` | Crear configuración de alerta |
| PUT | `/api/alertas/configuracion/{id}` | Actualizar configuración |
| DELETE | `/api/alertas/configuracion/{id}` | Eliminar configuración |

### Admin

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/admin/usuarios` | Listar usuarios |
| PUT | `/api/admin/usuarios/{id}/rol` | Cambiar rol de usuario |
| GET | `/api/admin/auditoria` | Log de auditoría |
| GET | `/api/admin/feedback` | Dashboard de feedback de agentes |
| GET | `/api/admin/prompts` | Listar prompt templates |
| PUT | `/api/admin/prompts/{id}` | Actualizar prompt template |
| GET | `/api/admin/ingesta/jobs` | Estado de jobs de ingesta |
| GET | `/api/admin/ingesta/dlq` | Dead letter queue |
| POST | `/api/admin/ingesta/dlq/{id}/retry` | Reintentar documento fallido |

### Feedback

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/feedback` | Enviar feedback de respuesta de agente |

---

## 13. KPIs y Métricas de Éxito

| KPI | Target | Medición |
|-----|--------|----------|
| Tiempo promedio de búsqueda legal | < 2 minutos (vs. 30+ min manual) | Application Insights: duración de sesiones de búsqueda |
| Plazos vencidos sin justificación | 0 por mes | Azure SQL: plazos con estado "vencido" sin flag de cumplido |
| Satisfacción con agentes IA | > 80% thumbs up | Tabla FeedbackRespuesta: ratio positivos/total |
| Precisión de análisis de riesgo | > 70% concordancia con resultado real | Feedback de abogados post-resolución de caso |
| Adopción del sistema | > 90% usuarios activos semanales | Application Insights: usuarios únicos/semana vs. total |
| Uptime del sistema | > 99.5% | Azure Monitor: disponibilidad del App Service |
| Tiempo de respuesta de agentes | < 5 seg hasta primer token | Application Insights: latencia de `/api/agentes/chat` |
| Normas actualizadas | < 24hs desde publicación en Boletín Oficial | Delta entre fechaPublicacion y fechaIngesta en Azure SQL |
| Reutilización de código MVP | > 75% | Archivos migrados vs. totales |
| Cobertura de golden set | > 90% Recall@10 | Pipeline de evaluación en CI |
| Costo mensual de LLM | < $500/mes | Telemetría custom en Application Insights |

---

*Legal Ai Ar — Roadmap de Features — v2.0 — Mayo 2026*
*Basado en evolución del MVP `legal-ai-ar` — ~78% código reutilizable*
