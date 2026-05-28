# Legal Ai Ar — Backlog de Work Items

> Generado: 2026-04-02
> Total features: 27
> Stack: Angular 19 + .NET 10 + Azure

---

## Estructura de Carpetas

```
docs/roadmap/
├── features.md              # Roadmap de features (existente)
├── backlog.md               # Este archivo (índice del backlog)
├── F01 - Autenticacion y Autorizacion/
│   ├── F01 - W01 - Documentacion Integral.md
│   ├── F01 - W02 - Backend - Configuracion Entra ID y JWT.md
│   ├── F01 - W03 - Backend - Middleware de Autorizacion por Rol.md
│   ├── F01 - W04 - Frontend - MSAL Angular Setup y AuthService.md
│   ├── F01 - W05 - Frontend - AuthGuard y RoleGuard.md
│   ├── F01 - W06 - Frontend - AuthInterceptor y ErrorInterceptor.md
│   └── F01 - W07 - Testing - Tests de Autenticacion E2E.md
├── F02 - Dashboard Principal/
│   ├── F02 - W01 - Documentacion Integral.md
│   ├── F02 - W02 - Backend - Endpoint Agregador Dashboard.md
│   ├── F02 - W03 - Frontend - Layout Shell Sidebar y Navbar.md
│   ├── F02 - W04 - Frontend - Dashboard Component y Widgets.md
│   ├── F02 - W05 - Frontend - Widget Plazos Proximos.md
│   ├── F02 - W06 - Frontend - Widget Novedades Normativas.md
│   └── F02 - W07 - Testing - Tests de Dashboard.md
├── F03 - Busqueda de Normas/
│   ├── F03 - W01 - Documentacion Integral.md
│   ├── F03 - W02 - Backend - Indice AI Search para Normas.md
│   ├── F03 - W03 - Backend - Scoring Profile Hibrido BM25 y Vectores.md
│   ├── F03 - W04 - Backend - Endpoint POST Buscar Normas.md
│   ├── F03 - W05 - Frontend - SearchBar con Autocompletado.md
│   ├── F03 - W06 - Frontend - Filtros Laterales con Facets.md
│   ├── F03 - W07 - Frontend - Lista de Resultados con Highlight.md
│   └── F03 - W08 - Testing - Tests de Busqueda de Normas.md
├── F04 - Busqueda de Jurisprudencia/
│   ├── F04 - W01 - Documentacion Integral.md
│   ├── F04 - W02 - Backend - Indice AI Search para Jurisprudencia.md
│   ├── F04 - W03 - Backend - Endpoint POST Buscar Jurisprudencia.md
│   ├── F04 - W04 - Frontend - Pagina Busqueda Jurisprudencia.md
│   ├── F04 - W05 - Frontend - Card de Resultado de Fallo.md
│   └── F04 - W06 - Testing - Tests de Busqueda de Jurisprudencia.md
├── F05 - Detalle de Norma/
│   ├── F05 - W01 - Documentacion Integral.md
│   ├── F05 - W02 - Backend - Endpoint GET Norma Detalle.md
│   ├── F05 - W03 - Backend - Endpoint GET Norma Grafo SQL Graph.md
│   ├── F05 - W04 - Backend - Endpoint GET Norma Articulos Paginado.md
│   ├── F05 - W05 - Frontend - Pagina Detalle de Norma con Tabs.md
│   ├── F05 - W06 - Frontend - Visualizacion Grafo de Relaciones.md
│   ├── F05 - W07 - Frontend - Timeline de Modificaciones.md
│   └── F05 - W08 - Testing - Tests de Detalle de Norma.md
├── F06 - Detalle de Articulo/
│   ├── F06 - W01 - Documentacion Integral.md
│   ├── F06 - W02 - Backend - Endpoint GET Articulo y Jurisprudencia Relacionada.md
│   ├── F06 - W03 - Frontend - Pagina Detalle de Articulo.md
│   ├── F06 - W04 - Frontend - Panel Jurisprudencia Relacionada.md
│   └── F06 - W05 - Testing - Tests de Detalle de Articulo.md
├── F07 - Novedades Normativas/
│   ├── F07 - W01 - Documentacion Integral.md
│   ├── F07 - W02 - Backend - Azure Function Timer Trigger Boletin Oficial.md
│   ├── F07 - W03 - Backend - Endpoint GET Novedades y SignalR Push.md
│   ├── F07 - W04 - Frontend - Pagina Feed de Novedades.md
│   ├── F07 - W05 - Frontend - Suscripcion a Alertas por Rama.md
│   └── F07 - W06 - Testing - Tests de Novedades.md
├── F08 - Chat con Agentes IA/
│   ├── F08 - W01 - Documentacion Integral.md
│   ├── F08 - W02 - Backend - Semantic Kernel Setup y Orquestador.md
│   ├── F08 - W03 - Backend - Endpoint POST Chat con SignalR Streaming.md
│   ├── F08 - W04 - Backend - Persistencia de Conversaciones.md
│   ├── F08 - W05 - Frontend - Chat UI con Markdown Rendering.md
│   ├── F08 - W06 - Frontend - SignalR Client para Streaming.md
│   ├── F08 - W07 - Frontend - Panel de Fuentes Citadas.md
│   ├── F08 - W08 - Frontend - Historial de Conversaciones.md
│   └── F08 - W09 - Testing - Tests de Chat E2E.md
├── F09 - Agente Normativo/
│   ├── F09 - W01 - Documentacion Integral.md
│   ├── F09 - W02 - Backend - Plugin AgenteNormativo Functions.md
│   ├── F09 - W03 - Backend - Prompts Semanticos del Agente.md
│   ├── F09 - W04 - Backend - Integracion con AI Search y SQL Graph.md
│   └── F09 - W05 - Testing - Evaluacion con 15 Consultas Tipificadas.md
├── F10 - Agente Jurisprudencial/
│   ├── F10 - W01 - Documentacion Integral.md
│   ├── F10 - W02 - Backend - Plugin AgenteJurisprudencial Functions.md
│   ├── F10 - W03 - Backend - RAG Pipeline sobre Jurisprudencia.md
│   ├── F10 - W04 - Backend - Graph Traversal Fallo a Articulo.md
│   └── F10 - W05 - Testing - Evaluacion con 15 Consultas Tipificadas.md
├── F11 - Agente Procesal/
│   ├── F11 - W01 - Documentacion Integral.md
│   ├── F11 - W02 - Backend - Plugin AgenteProcesal Functions.md
│   ├── F11 - W03 - Backend - Motor de Calculo de Dias Habiles.md
│   ├── F11 - W04 - Backend - Calendario de Feriados Nacionales y Judiciales.md
│   └── F11 - W05 - Testing - Evaluacion con 10 Consultas Tipificadas.md
├── F12 - Gestion de Expedientes/
│   ├── F12 - W01 - Documentacion Integral.md
│   ├── F12 - W02 - Backend - Modelo EF Core Expediente y Migraciones.md
│   ├── F12 - W03 - Backend - CRUD Endpoints Expedientes.md
│   ├── F12 - W04 - Backend - Subrecursos Movimientos y Documentos.md
│   ├── F12 - W05 - Backend - Upload Documentos a Blob Storage.md
│   ├── F12 - W06 - Frontend - Lista de Expedientes con DataTable.md
│   ├── F12 - W07 - Frontend - Formulario Alta y Edicion Expediente.md
│   ├── F12 - W08 - Frontend - Detalle Expediente con Tabs.md
│   ├── F12 - W09 - Frontend - Timeline de Movimientos.md
│   ├── F12 - W10 - Frontend - Gestion de Documentos Adjuntos.md
│   └── F12 - W11 - Testing - Tests CRUD Expedientes.md
├── F13 - Gestion de Plazos/
│   ├── F13 - W01 - Documentacion Integral.md
│   ├── F13 - W02 - Backend - Modelo EF Core Plazo y Migraciones.md
│   ├── F13 - W03 - Backend - CRUD Endpoints Plazos.md
│   ├── F13 - W04 - Backend - Azure Function Evaluacion Diaria de Plazos.md
│   ├── F13 - W05 - Backend - Generacion de Alertas via Storage Queue.md
│   ├── F13 - W06 - Frontend - Lista de Plazos con Semaforo Visual.md
│   ├── F13 - W07 - Frontend - Formulario Alta Plazo con Calculo Habiles.md
│   └── F13 - W08 - Testing - Tests de Plazos y Calculo de Habiles.md
├── F14 - Calendario Legal/
│   ├── F14 - W01 - Documentacion Integral.md
│   ├── F14 - W02 - Backend - Endpoint GET Calendario Agregado.md
│   ├── F14 - W03 - Frontend - Integracion FullCalendar Angular.md
│   ├── F14 - W04 - Frontend - Filtros y Navegacion de Calendario.md
│   └── F14 - W05 - Testing - Tests de Calendario.md
├── F15 - Analisis de Riesgo Legal/
│   ├── F15 - W01 - Documentacion Integral.md
│   ├── F15 - W02 - Backend - Plugin AgenteRiesgo Semantic Kernel.md
│   ├── F15 - W03 - Backend - Modelo de Taxonomia de Riesgos.md
│   ├── F15 - W04 - Backend - Endpoint POST Analizar Riesgo.md
│   ├── F15 - W05 - Backend - Persistencia de Analisis en SQL.md
│   ├── F15 - W06 - Frontend - Formulario de Ingreso de Caso.md
│   ├── F15 - W07 - Frontend - Vista de Resultado con Score Visual.md
│   └── F15 - W08 - Testing - Evaluacion de Analisis de Riesgo.md
├── F16 - Historial de Analisis de Riesgo/
│   ├── F16 - W01 - Documentacion Integral.md
│   ├── F16 - W02 - Backend - Endpoint GET Historial y Re-analisis.md
│   ├── F16 - W03 - Frontend - Lista Historial con DataTable.md
│   ├── F16 - W04 - Frontend - Diff Visual entre Analisis.md
│   └── F16 - W05 - Testing - Tests de Historial.md
├── F17 - Generacion de Informes Legales/
│   ├── F17 - W01 - Documentacion Integral.md
│   ├── F17 - W02 - Backend - Motor de Templates OpenXml.md
│   ├── F17 - W03 - Backend - Templates de Informes en Blob Storage.md
│   ├── F17 - W04 - Backend - Endpoint POST Generar Informe.md
│   ├── F17 - W05 - Frontend - Selector de Tipo de Informe.md
│   ├── F17 - W06 - Frontend - Preview y Descarga de Informe.md
│   └── F17 - W07 - Testing - Tests de Generacion de Informes.md
├── F18 - Reportes Operativos/
│   ├── F18 - W01 - Documentacion Integral.md
│   ├── F18 - W02 - Backend - Endpoints de Reportes Agregados.md
│   ├── F18 - W03 - Frontend - Graficos con ngx-charts.md
│   ├── F18 - W04 - Frontend - Export a PDF y Excel.md
│   └── F18 - W05 - Testing - Tests de Reportes.md
├── F19 - Administracion de Usuarios/
│   ├── F19 - W01 - Documentacion Integral.md
│   ├── F19 - W02 - Backend - CRUD Usuarios y Permisos Custom.md
│   ├── F19 - W03 - Backend - Audit Log en Azure SQL.md
│   ├── F19 - W04 - Frontend - Pagina Admin Usuarios.md
│   ├── F19 - W05 - Frontend - Log de Auditoria.md
│   └── F19 - W06 - Testing - Tests de Admin.md
├── F20 - Configuracion de Alertas Avanzadas/
│   ├── F20 - W01 - Documentacion Integral.md
│   ├── F20 - W02 - Backend - CRUD Configuracion de Alertas.md
│   ├── F20 - W03 - Backend - Azure Function Evaluacion de Condiciones.md
│   ├── F20 - W04 - Backend - Envio de Email con SendGrid o SMTP.md
│   ├── F20 - W05 - Frontend - Wizard Configuracion de Alerta.md
│   ├── F20 - W06 - Frontend - Centro de Alertas Inbox.md
│   └── F20 - W07 - Testing - Tests de Alertas.md
├── F21 - Explorador de Grafo Legal/
│   ├── F21 - W01 - Documentacion Integral.md
│   ├── F21 - W02 - Backend - Endpoint GET Grafo Explorar con Profundidad.md
│   ├── F21 - W03 - Frontend - Componente Grafo Interactivo D3 o Cytoscape.md
│   ├── F21 - W04 - Frontend - Panel Detalle de Nodo Seleccionado.md
│   ├── F21 - W05 - Frontend - Filtros por Tipo de Relacion.md
│   └── F21 - W06 - Testing - Tests de Explorador de Grafo.md
├── F22 - Feedback y Mejora de Agentes/
│   ├── F22 - W01 - Documentacion Integral.md
│   ├── F22 - W02 - Backend - Endpoint POST Feedback y Modelo SQL.md
│   ├── F22 - W03 - Backend - Azure Function Analisis Semanal de Feedback.md
│   ├── F22 - W04 - Frontend - Botones Feedback en Chat.md
│   ├── F22 - W05 - Frontend - Dashboard Metricas de Satisfaccion.md
│   └── F22 - W06 - Testing - Tests de Feedback.md
├── F23 - Modo Offline PWA/
│   ├── F23 - W01 - Documentacion Integral.md
│   ├── F23 - W02 - Backend - ETags y Caching Headers.md
│   ├── F23 - W03 - Frontend - Angular Service Worker Setup.md
│   ├── F23 - W04 - Frontend - IndexedDB para Cache Offline.md
│   ├── F23 - W05 - Frontend - Sync al Reconectar.md
│   └── F23 - W06 - Testing - Tests Offline.md
├── FT01 - Notificaciones en Tiempo Real/
│   ├── FT01 - W01 - Documentacion Integral.md
│   ├── FT01 - W02 - Backend - SignalR Hub NotificacionHub.md
│   ├── FT01 - W03 - Backend - Worker Storage Queue a SignalR.md
│   ├── FT01 - W04 - Frontend - NotificacionService SignalR Client.md
│   ├── FT01 - W05 - Frontend - Badge y Toast Components.md
│   ├── FT01 - W06 - Frontend - Centro de Notificaciones.md
│   └── FT01 - W07 - Testing - Tests de Notificaciones.md
├── FT02 - Busqueda Global Omnisearch/
│   ├── FT02 - W01 - Documentacion Integral.md
│   ├── FT02 - W02 - Backend - Endpoint GET Buscar Global Multi-Index.md
│   ├── FT02 - W03 - Frontend - Modal Omnisearch con Keyboard Nav.md
│   ├── FT02 - W04 - Frontend - Resultados Agrupados por Tipo.md
│   └── FT02 - W05 - Testing - Tests de Omnisearch.md
├── FT03 - Tema y Accesibilidad/
│   ├── FT03 - W01 - Documentacion Integral.md
│   ├── FT03 - W02 - Frontend - Angular Material Theming Claro y Oscuro.md
│   ├── FT03 - W03 - Frontend - Tailwind Config y Design Tokens.md
│   ├── FT03 - W04 - Frontend - Accesibilidad WCAG 2.1 AA.md
│   └── FT03 - W05 - Testing - Audit de Accesibilidad.md
├── FT04 - Auditoria y Logging/
│   ├── FT04 - W01 - Documentacion Integral.md
│   ├── FT04 - W02 - Backend - Middleware de Auditoria NET 10.md
│   ├── FT04 - W03 - Backend - Tabla AuditLog y Retention Policy.md
│   ├── FT04 - W04 - Backend - Application Insights Integration.md
│   └── FT04 - W05 - Testing - Tests de Auditoria.md
```

---

## Release 1.0 (7 features, 47 work items)

| Feature | Nombre | Sprint | Work Items |
|---------|--------|--------|------------|
| F01 | [Autenticacion y Autorizacion](F01%20-%20Autenticacion%20y%20Autorizacion/) | S01 | 7 |
| F02 | [Dashboard Principal](F02%20-%20Dashboard%20Principal/) | S02 | 7 |
| F03 | [Busqueda de Normas](F03%20-%20Busqueda%20de%20Normas/) | S02-S03 | 8 |
| F04 | [Busqueda de Jurisprudencia](F04%20-%20Busqueda%20de%20Jurisprudencia/) | S03 | 6 |
| F05 | [Detalle de Norma](F05%20-%20Detalle%20de%20Norma/) | S03-S04 | 8 |
| F06 | [Detalle de Articulo](F06%20-%20Detalle%20de%20Articulo/) | S04 | 5 |
| F07 | [Novedades Normativas](F07%20-%20Novedades%20Normativas/) | S04 | 6 |

### Detalle de Work Items — Release 1.0

| Feature | Work Item | Tipo | Estimación |
|---------|-----------|------|------------|
| F01 | F01-W01: Documentacion Integral | doc | 3 SP |
| F01 | F01-W02: Backend - Configuracion Entra ID y JWT | backend | 5 SP |
| F01 | F01-W03: Backend - Middleware de Autorizacion por Rol | backend | 5 SP |
| F01 | F01-W04: Frontend - MSAL Angular Setup y AuthService | frontend | 3 SP |
| F01 | F01-W05: Frontend - AuthGuard y RoleGuard | frontend | 3 SP |
| F01 | F01-W06: Frontend - AuthInterceptor y ErrorInterceptor | frontend | 3 SP |
| F01 | F01-W07: Testing - Tests de Autenticacion E2E | testing | 3 SP |
| F02 | F02-W01: Documentacion Integral | doc | 3 SP |
| F02 | F02-W02: Backend - Endpoint Agregador Dashboard | backend | 5 SP |
| F02 | F02-W03: Frontend - Layout Shell Sidebar y Navbar | frontend | 5 SP |
| F02 | F02-W04: Frontend - Dashboard Component y Widgets | frontend | 3 SP |
| F02 | F02-W05: Frontend - Widget Plazos Proximos | frontend | 3 SP |
| F02 | F02-W06: Frontend - Widget Novedades Normativas | frontend | 3 SP |
| F02 | F02-W07: Testing - Tests de Dashboard | testing | 3 SP |
| F03 | F03-W01: Documentacion Integral | doc | 3 SP |
| F03 | F03-W02: Backend - Indice AI Search para Normas | backend | 5 SP |
| F03 | F03-W03: Backend - Scoring Profile Hibrido BM25 y Vectores | backend | 5 SP |
| F03 | F03-W04: Backend - Endpoint POST Buscar Normas | backend | 3 SP |
| F03 | F03-W05: Frontend - SearchBar con Autocompletado | frontend | 3 SP |
| F03 | F03-W06: Frontend - Filtros Laterales con Facets | frontend | 3 SP |
| F03 | F03-W07: Frontend - Lista de Resultados con Highlight | frontend | 3 SP |
| F03 | F03-W08: Testing - Tests de Busqueda de Normas | testing | 3 SP |
| F04 | F04-W01: Documentacion Integral | doc | 3 SP |
| F04 | F04-W02: Backend - Indice AI Search para Jurisprudencia | backend | 5 SP |
| F04 | F04-W03: Backend - Endpoint POST Buscar Jurisprudencia | backend | 5 SP |
| F04 | F04-W04: Frontend - Pagina Busqueda Jurisprudencia | frontend | 3 SP |
| F04 | F04-W05: Frontend - Card de Resultado de Fallo | frontend | 3 SP |
| F04 | F04-W06: Testing - Tests de Busqueda de Jurisprudencia | testing | 3 SP |
| F05 | F05-W01: Documentacion Integral | doc | 3 SP |
| F05 | F05-W02: Backend - Endpoint GET Norma Detalle | backend | 5 SP |
| F05 | F05-W03: Backend - Endpoint GET Norma Grafo SQL Graph | backend | 5 SP |
| F05 | F05-W04: Backend - Endpoint GET Norma Articulos Paginado | backend | 3 SP |
| F05 | F05-W05: Frontend - Pagina Detalle de Norma con Tabs | frontend | 3 SP |
| F05 | F05-W06: Frontend - Visualizacion Grafo de Relaciones | frontend | 3 SP |
| F05 | F05-W07: Frontend - Timeline de Modificaciones | frontend | 3 SP |
| F05 | F05-W08: Testing - Tests de Detalle de Norma | testing | 3 SP |
| F06 | F06-W01: Documentacion Integral | doc | 3 SP |
| F06 | F06-W02: Backend - Endpoint GET Articulo y Jurisprudencia Relacionada | backend | 5 SP |
| F06 | F06-W03: Frontend - Pagina Detalle de Articulo | frontend | 5 SP |
| F06 | F06-W04: Frontend - Panel Jurisprudencia Relacionada | frontend | 3 SP |
| F06 | F06-W05: Testing - Tests de Detalle de Articulo | testing | 3 SP |
| F07 | F07-W01: Documentacion Integral | doc | 3 SP |
| F07 | F07-W02: Backend - Azure Function Timer Trigger Boletin Oficial | backend | 5 SP |
| F07 | F07-W03: Backend - Endpoint GET Novedades y SignalR Push | backend | 5 SP |
| F07 | F07-W04: Frontend - Pagina Feed de Novedades | frontend | 3 SP |
| F07 | F07-W05: Frontend - Suscripcion a Alertas por Rama | frontend | 3 SP |
| F07 | F07-W06: Testing - Tests de Novedades | testing | 3 SP |

---

## Release 2.0 (7 features, 48 work items)

| Feature | Nombre | Sprint | Work Items |
|---------|--------|--------|------------|
| F08 | [Chat con Agentes IA](F08%20-%20Chat%20con%20Agentes%20IA/) | S05-S06 | 9 |
| F09 | [Agente Normativo](F09%20-%20Agente%20Normativo/) | S06 | 5 |
| F10 | [Agente Jurisprudencial](F10%20-%20Agente%20Jurisprudencial/) | S06-S07 | 5 |
| F11 | [Agente Procesal](F11%20-%20Agente%20Procesal/) | S07 | 5 |
| F12 | [Gestion de Expedientes](F12%20-%20Gestion%20de%20Expedientes/) | S05-S06 | 11 |
| F13 | [Gestion de Plazos](F13%20-%20Gestion%20de%20Plazos/) | S06-S07 | 8 |
| F14 | [Calendario Legal](F14%20-%20Calendario%20Legal/) | S07 | 5 |

### Detalle de Work Items — Release 2.0

| Feature | Work Item | Tipo | Estimación |
|---------|-----------|------|------------|
| F08 | F08-W01: Documentacion Integral | doc | 3 SP |
| F08 | F08-W02: Backend - Semantic Kernel Setup y Orquestador | backend | 5 SP |
| F08 | F08-W03: Backend - Endpoint POST Chat con SignalR Streaming | backend | 5 SP |
| F08 | F08-W04: Backend - Persistencia de Conversaciones | backend | 3 SP |
| F08 | F08-W05: Frontend - Chat UI con Markdown Rendering | frontend | 3 SP |
| F08 | F08-W06: Frontend - SignalR Client para Streaming | frontend | 3 SP |
| F08 | F08-W07: Frontend - Panel de Fuentes Citadas | frontend | 3 SP |
| F08 | F08-W08: Frontend - Historial de Conversaciones | frontend | 3 SP |
| F08 | F08-W09: Testing - Tests de Chat E2E | testing | 3 SP |
| F09 | F09-W01: Documentacion Integral | doc | 3 SP |
| F09 | F09-W02: Backend - Plugin AgenteNormativo Functions | backend | 5 SP |
| F09 | F09-W03: Backend - Prompts Semanticos del Agente | backend | 5 SP |
| F09 | F09-W04: Backend - Integracion con AI Search y SQL Graph | backend | 3 SP |
| F09 | F09-W05: Testing - Evaluacion con 15 Consultas Tipificadas | testing | 3 SP |
| F10 | F10-W01: Documentacion Integral | doc | 3 SP |
| F10 | F10-W02: Backend - Plugin AgenteJurisprudencial Functions | backend | 5 SP |
| F10 | F10-W03: Backend - RAG Pipeline sobre Jurisprudencia | backend | 5 SP |
| F10 | F10-W04: Backend - Graph Traversal Fallo a Articulo | backend | 3 SP |
| F10 | F10-W05: Testing - Evaluacion con 15 Consultas Tipificadas | testing | 3 SP |
| F11 | F11-W01: Documentacion Integral | doc | 3 SP |
| F11 | F11-W02: Backend - Plugin AgenteProcesal Functions | backend | 5 SP |
| F11 | F11-W03: Backend - Motor de Calculo de Dias Habiles | backend | 5 SP |
| F11 | F11-W04: Backend - Calendario de Feriados Nacionales y Judiciales | backend | 3 SP |
| F11 | F11-W05: Testing - Evaluacion con 10 Consultas Tipificadas | testing | 3 SP |
| F12 | F12-W01: Documentacion Integral | doc | 3 SP |
| F12 | F12-W02: Backend - Modelo EF Core Expediente y Migraciones | backend | 5 SP |
| F12 | F12-W03: Backend - CRUD Endpoints Expedientes | backend | 5 SP |
| F12 | F12-W04: Backend - Subrecursos Movimientos y Documentos | backend | 3 SP |
| F12 | F12-W05: Backend - Upload Documentos a Blob Storage | backend | 3 SP |
| F12 | F12-W06: Frontend - Lista de Expedientes con DataTable | frontend | 3 SP |
| F12 | F12-W07: Frontend - Formulario Alta y Edicion Expediente | frontend | 3 SP |
| F12 | F12-W08: Frontend - Detalle Expediente con Tabs | frontend | 3 SP |
| F12 | F12-W09: Frontend - Timeline de Movimientos | frontend | 3 SP |
| F12 | F12-W10: Frontend - Gestion de Documentos Adjuntos | frontend | 3 SP |
| F12 | F12-W11: Testing - Tests CRUD Expedientes | testing | 3 SP |
| F13 | F13-W01: Documentacion Integral | doc | 3 SP |
| F13 | F13-W02: Backend - Modelo EF Core Plazo y Migraciones | backend | 5 SP |
| F13 | F13-W03: Backend - CRUD Endpoints Plazos | backend | 5 SP |
| F13 | F13-W04: Backend - Azure Function Evaluacion Diaria de Plazos | backend | 3 SP |
| F13 | F13-W05: Backend - Generacion de Alertas via Storage Queue | backend | 3 SP |
| F13 | F13-W06: Frontend - Lista de Plazos con Semaforo Visual | frontend | 3 SP |
| F13 | F13-W07: Frontend - Formulario Alta Plazo con Calculo Habiles | frontend | 3 SP |
| F13 | F13-W08: Testing - Tests de Plazos y Calculo de Habiles | testing | 3 SP |
| F14 | F14-W01: Documentacion Integral | doc | 3 SP |
| F14 | F14-W02: Backend - Endpoint GET Calendario Agregado | backend | 5 SP |
| F14 | F14-W03: Frontend - Integracion FullCalendar Angular | frontend | 5 SP |
| F14 | F14-W04: Frontend - Filtros y Navegacion de Calendario | frontend | 3 SP |
| F14 | F14-W05: Testing - Tests de Calendario | testing | 3 SP |

---

## Release 3.0 (4 features, 25 work items)

| Feature | Nombre | Sprint | Work Items |
|---------|--------|--------|------------|
| F15 | [Analisis de Riesgo Legal](F15%20-%20Analisis%20de%20Riesgo%20Legal/) | S08-S09 | 8 |
| F16 | [Historial de Analisis de Riesgo](F16%20-%20Historial%20de%20Analisis%20de%20Riesgo/) | S09 | 5 |
| F17 | [Generacion de Informes Legales](F17%20-%20Generacion%20de%20Informes%20Legales/) | S08-S09 | 7 |
| F18 | [Reportes Operativos](F18%20-%20Reportes%20Operativos/) | S09 | 5 |

### Detalle de Work Items — Release 3.0

| Feature | Work Item | Tipo | Estimación |
|---------|-----------|------|------------|
| F15 | F15-W01: Documentacion Integral | doc | 3 SP |
| F15 | F15-W02: Backend - Plugin AgenteRiesgo Semantic Kernel | backend | 5 SP |
| F15 | F15-W03: Backend - Modelo de Taxonomia de Riesgos | backend | 5 SP |
| F15 | F15-W04: Backend - Endpoint POST Analizar Riesgo | backend | 3 SP |
| F15 | F15-W05: Backend - Persistencia de Analisis en SQL | backend | 3 SP |
| F15 | F15-W06: Frontend - Formulario de Ingreso de Caso | frontend | 3 SP |
| F15 | F15-W07: Frontend - Vista de Resultado con Score Visual | frontend | 3 SP |
| F15 | F15-W08: Testing - Evaluacion de Analisis de Riesgo | testing | 3 SP |
| F16 | F16-W01: Documentacion Integral | doc | 3 SP |
| F16 | F16-W02: Backend - Endpoint GET Historial y Re-analisis | backend | 5 SP |
| F16 | F16-W03: Frontend - Lista Historial con DataTable | frontend | 5 SP |
| F16 | F16-W04: Frontend - Diff Visual entre Analisis | frontend | 3 SP |
| F16 | F16-W05: Testing - Tests de Historial | testing | 3 SP |
| F17 | F17-W01: Documentacion Integral | doc | 3 SP |
| F17 | F17-W02: Backend - Motor de Templates OpenXml | backend | 5 SP |
| F17 | F17-W03: Backend - Templates de Informes en Blob Storage | backend | 5 SP |
| F17 | F17-W04: Backend - Endpoint POST Generar Informe | backend | 3 SP |
| F17 | F17-W05: Frontend - Selector de Tipo de Informe | frontend | 3 SP |
| F17 | F17-W06: Frontend - Preview y Descarga de Informe | frontend | 3 SP |
| F17 | F17-W07: Testing - Tests de Generacion de Informes | testing | 3 SP |
| F18 | F18-W01: Documentacion Integral | doc | 3 SP |
| F18 | F18-W02: Backend - Endpoints de Reportes Agregados | backend | 5 SP |
| F18 | F18-W03: Frontend - Graficos con ngx-charts | frontend | 5 SP |
| F18 | F18-W04: Frontend - Export a PDF y Excel | frontend | 3 SP |
| F18 | F18-W05: Testing - Tests de Reportes | testing | 3 SP |

---

## Release 4.0 (5 features, 31 work items)

| Feature | Nombre | Sprint | Work Items |
|---------|--------|--------|------------|
| F19 | [Administracion de Usuarios](F19%20-%20Administracion%20de%20Usuarios/) | S10 | 6 |
| F20 | [Configuracion de Alertas Avanzadas](F20%20-%20Configuracion%20de%20Alertas%20Avanzadas/) | S10 | 7 |
| F21 | [Explorador de Grafo Legal](F21%20-%20Explorador%20de%20Grafo%20Legal/) | S11 | 6 |
| F22 | [Feedback y Mejora de Agentes](F22%20-%20Feedback%20y%20Mejora%20de%20Agentes/) | S11 | 6 |
| F23 | [Modo Offline PWA](F23%20-%20Modo%20Offline%20PWA/) | S11 | 6 |

### Detalle de Work Items — Release 4.0

| Feature | Work Item | Tipo | Estimación |
|---------|-----------|------|------------|
| F19 | F19-W01: Documentacion Integral | doc | 3 SP |
| F19 | F19-W02: Backend - CRUD Usuarios y Permisos Custom | backend | 5 SP |
| F19 | F19-W03: Backend - Audit Log en Azure SQL | backend | 5 SP |
| F19 | F19-W04: Frontend - Pagina Admin Usuarios | frontend | 3 SP |
| F19 | F19-W05: Frontend - Log de Auditoria | frontend | 3 SP |
| F19 | F19-W06: Testing - Tests de Admin | testing | 3 SP |
| F20 | F20-W01: Documentacion Integral | doc | 3 SP |
| F20 | F20-W02: Backend - CRUD Configuracion de Alertas | backend | 5 SP |
| F20 | F20-W03: Backend - Azure Function Evaluacion de Condiciones | backend | 5 SP |
| F20 | F20-W04: Backend - Envio de Email con SendGrid o SMTP | backend | 3 SP |
| F20 | F20-W05: Frontend - Wizard Configuracion de Alerta | frontend | 3 SP |
| F20 | F20-W06: Frontend - Centro de Alertas Inbox | frontend | 3 SP |
| F20 | F20-W07: Testing - Tests de Alertas | testing | 3 SP |
| F21 | F21-W01: Documentacion Integral | doc | 3 SP |
| F21 | F21-W02: Backend - Endpoint GET Grafo Explorar con Profundidad | backend | 5 SP |
| F21 | F21-W03: Frontend - Componente Grafo Interactivo D3 o Cytoscape | frontend | 5 SP |
| F21 | F21-W04: Frontend - Panel Detalle de Nodo Seleccionado | frontend | 3 SP |
| F21 | F21-W05: Frontend - Filtros por Tipo de Relacion | frontend | 3 SP |
| F21 | F21-W06: Testing - Tests de Explorador de Grafo | testing | 3 SP |
| F22 | F22-W01: Documentacion Integral | doc | 3 SP |
| F22 | F22-W02: Backend - Endpoint POST Feedback y Modelo SQL | backend | 5 SP |
| F22 | F22-W03: Backend - Azure Function Analisis Semanal de Feedback | backend | 5 SP |
| F22 | F22-W04: Frontend - Botones Feedback en Chat | frontend | 3 SP |
| F22 | F22-W05: Frontend - Dashboard Metricas de Satisfaccion | frontend | 3 SP |
| F22 | F22-W06: Testing - Tests de Feedback | testing | 3 SP |
| F23 | F23-W01: Documentacion Integral | doc | 3 SP |
| F23 | F23-W02: Backend - ETags y Caching Headers | backend | 5 SP |
| F23 | F23-W03: Frontend - Angular Service Worker Setup | frontend | 5 SP |
| F23 | F23-W04: Frontend - IndexedDB para Cache Offline | frontend | 3 SP |
| F23 | F23-W05: Frontend - Sync al Reconectar | frontend | 3 SP |
| F23 | F23-W06: Testing - Tests Offline | testing | 3 SP |

---

## Release Transversal (4 features, 22 work items)

| Feature | Nombre | Sprint | Work Items |
|---------|--------|--------|------------|
| FT01 | [Notificaciones en Tiempo Real](FT01%20-%20Notificaciones%20en%20Tiempo%20Real/) | S03-S04 | 7 |
| FT02 | [Busqueda Global Omnisearch](FT02%20-%20Busqueda%20Global%20Omnisearch/) | S04 | 5 |
| FT03 | [Tema y Accesibilidad](FT03%20-%20Tema%20y%20Accesibilidad/) | S02 | 5 |
| FT04 | [Auditoria y Logging](FT04%20-%20Auditoria%20y%20Logging/) | S03 | 5 |

### Detalle de Work Items — Release Transversal

| Feature | Work Item | Tipo | Estimación |
|---------|-----------|------|------------|
| FT01 | FT01-W01: Documentacion Integral | doc | 3 SP |
| FT01 | FT01-W02: Backend - SignalR Hub NotificacionHub | backend | 5 SP |
| FT01 | FT01-W03: Backend - Worker Storage Queue a SignalR | backend | 5 SP |
| FT01 | FT01-W04: Frontend - NotificacionService SignalR Client | frontend | 3 SP |
| FT01 | FT01-W05: Frontend - Badge y Toast Components | frontend | 3 SP |
| FT01 | FT01-W06: Frontend - Centro de Notificaciones | frontend | 3 SP |
| FT01 | FT01-W07: Testing - Tests de Notificaciones | testing | 3 SP |
| FT02 | FT02-W01: Documentacion Integral | doc | 3 SP |
| FT02 | FT02-W02: Backend - Endpoint GET Buscar Global Multi-Index | backend | 5 SP |
| FT02 | FT02-W03: Frontend - Modal Omnisearch con Keyboard Nav | frontend | 5 SP |
| FT02 | FT02-W04: Frontend - Resultados Agrupados por Tipo | frontend | 3 SP |
| FT02 | FT02-W05: Testing - Tests de Omnisearch | testing | 3 SP |
| FT03 | FT03-W01: Documentacion Integral | doc | 3 SP |
| FT03 | FT03-W02: Frontend - Angular Material Theming Claro y Oscuro | frontend | 5 SP |
| FT03 | FT03-W03: Frontend - Tailwind Config y Design Tokens | frontend | 5 SP |
| FT03 | FT03-W04: Frontend - Accesibilidad WCAG 2.1 AA | frontend | 3 SP |
| FT03 | FT03-W05: Testing - Audit de Accesibilidad | testing | 3 SP |
| FT04 | FT04-W01: Documentacion Integral | doc | 3 SP |
| FT04 | FT04-W02: Backend - Middleware de Auditoria NET 10 | backend | 5 SP |
| FT04 | FT04-W03: Backend - Tabla AuditLog y Retention Policy | backend | 5 SP |
| FT04 | FT04-W04: Backend - Application Insights Integration | backend | 3 SP |
| FT04 | FT04-W05: Testing - Tests de Auditoria | testing | 3 SP |

---

## Resumen

| Métrica | Valor |
|---------|-------|
| Total features | 27 |
| Total work items | 173 |
| Total story points | 627 SP |
| Sprints estimados (2 semanas c/u) | 11 |
| Velocity esperada (2-3 devs) | ~25-35 SP/sprint |

---

*Legal Ai Ar — Backlog de Work Items — 2026-04-02*
