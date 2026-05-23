# Plan maestro: degradación por infra, recuperación y continuidad del pipeline

## Objetivo global

Cuando falle el acceso compartido a **Azure Storage (colas/blob)** o red correlacionada:

1. **Detectar y exponer** degradación (workers → API → UI / otros workers).
2. **Marcar estado** en dominio (`IngestionJob` / flags) para operación y automatización.
3. Al **restablecer** servicios, **reanudar** trabajo de forma segura: discovery, colas y documentos **sin duplicar** ni dejar **`Processing`** huérfano sin política.

Documento relacionado: [f1-pipeline-infra-health-signalr.md](f1-pipeline-infra-health-signalr.md) (canal SignalR y rol de la API).

---

## Supuestos de producto (acordados en conversación)

- **Un solo job activo** por fuente/escenario relevante (no hay mezcla de mensajes de dos jobs en la misma ventana operativa).
- **`Job.DocumentsSkipped`** refleja los omitidos en discovery; se usa para **reanudar** el `DiscovererMessage` coherentemente.
- Paginación típica CSJN **10 por página** en fuentes que usan `PageSize = 10`; la **página aproximada** se deriva de **`DocumentsDiscovered`** con validación cruzada (**último `ExternalId`** dado de alta vs página esperada).
- Hoy el Discoverer **solo publica** mensajes a Fetcher **al final** del barrido exitoso ([`DiscovererWorkerService`](../../backend/src/workers/LegalAiAr.Worker.Discoverer/DiscovererWorkerService.cs)); eso obliga a tratar el caso **BD poblada / cola vacía** de forma explícita.

---

## Fase A — Degradación: detección y visibilidad

| Paso | Entregable |
|------|------------|
| A.1 | Workers: al detectar fallos **clasificables** (403 `AuthorizationFailure`, timeouts de red hacia Storage), **invocar** al hub (p. ej. `ReportInfraIncident`) con **rate limit** por proceso. |
| A.2 | API: **agregador** (ventana + umbral + deduplicación por categoría/código). |
| A.3 | API → clientes: evento **broadcast** (extender `IWorkerControlClient` o hub dedicado) + opcional persistencia mínima de incidente. |
| A.4 | UI admin: **banner** / chip de estado degradado + enlace a auditoría / runbook. |
| A.5 | Salida de degradado: reglas (**probe** desde API, primer `Receive` exitoso reportado por worker, o **ack manual**); **histéresis** para evitar flapping. |

**Criterio de éxito:** operador y UI ven el mismo “modo degradado” que los logs; no se asume recuperación sin evidencia.

---

## Fase B — Persistencia en `IngestionJob` (banderas y cursor)

Añadir (nombres orientativos; ajustar al modelo real):

| Campo | Uso |
|-------|-----|
| `InfrastructureDegraded` (bool) o estado derivado | Job “congelado” por política mientras infra no OK. |
| `DegradedSinceUtc` / `DegradedReason` | Auditoría y UI. |
| `DiscoveryBatchPublished` (bool) | `false` si hubo documentos `Fetcher`/`Pending` **sin** haber ejecutado `PublishBatchAsync` para ese run. |
| `DiscoveryResumeHint` (opcional: JSON o columnas) | Última **página** o **cursor** acordado con la fuente; o confiar en recomputo desde `DocumentsDiscovered` + último `ExternalId`. |
| `LastPipelineProgressAtUtc` (opcional) | Detección de **staleness** para alertas. |

**Criterio de éxito:** la API puede decidir políticas sin escanear solo logs; el job “sabe” si el gate Fetcher ya pasó.

---

## Fase C — Reanudar discovery sin duplicar

| Paso | Entregable |
|------|------------|
| C.1 | Comando admin o worker: **“ResumeDiscovery”** que construye `DiscovererMessage` con **`SkipDocuments` / cursor** alineados con `DocumentsDiscovered`, `DocumentsSkipped` y validación del **último documento** (`ExternalId`, fecha). |
| C.2 | Estrategia por tipo de crawl (`by-range`, destacados, etc.): mapear **página ↔ índice remoto** revisando el código de cada `DiscoverAsync` (p. ej. `jtStartIndex`). |
| C.3 | Si `DiscoveryBatchPublished == false` y ya existen filas `Documents` para el job: **o bien** publicar batch **pendiente** (mensajes aún en lista en memoria no aplica tras crash) → en la práctica **re-generar** `FetcherMessage` desde BD **o** re-ejecutar discovery con skip hasta el último `ExternalId` confirmado. |
| C.4 | Tests: reanudación con skip, sin duplicar `ExternalId`, última página &lt; 10. |

**Criterio de éxito:** tras incidente a mitad de discovery, un solo comando deja el pipeline equivalente a “habría seguido solo”.

---

## Fase D — Reconciliar colas con `Documents` (Fetcher → …)

| Paso | Entregable |
|------|------------|
| D.1 | Definir **invariante**: para cada etapa, qué pares (`CurrentStage`, `Status`) **deben** tener mensaje en cola (p. ej. `Fetcher`+`Pending` → cola Fetcher). |
| D.2 | Comando **“RequeueMissingQueueMessages(jobId)”** (o por etapa): para documentos que cumplen invariante y **no** tienen mensaje “en vuelo” deducible, **publicar** el mensaje correspondiente (`FetcherMessage`, `ParserMessage`, …). Idempotencia: mismo `documentId` no debe duplicar trabajo peligroso (usar contenido del mensaje o dedupe en cola si aplica). |
| D.3 | Integrar con auditoría existente / métricas. |

**Criterio de éxito:** BD y colas vuelven a alinearse sin intervención manual en Storage Explorer.

---

## Fase E — `Processing` huérfano

| Paso | Entregable |
|------|------------|
| E.1 | Política: **tiempo máximo** en `Processing` por etapa sin heartbeat de mensaje, o regla al **arranque** del worker. |
| E.2 | Comando **reconcile** (ya hay líneas en auditoría): reset selectivo a `Pending` o `Failed` con log. |
| E.3 | Coordinar con **visibility timeout** de colas para no competir con un worker vivo. |

**Criterio de éxito:** tras reinicio de red/workers, no quedan filas `Processing` eternas sin consumidor.

---

## Fase F — UI y operación

| Paso | Entregable |
|------|------------|
| F.1 | Panel: mostrar **degradado**, **DiscoveryBatchPublished**, acciones **Resume discovery**, **Requeue missing**, **Reconcile processing**. |
| F.2 | Runbook corto en `docs/` (enlace desde banner): VPN/firewall, orden de botones, cuándo cancelar job. |

---

## Orden de implementación sugerido

1. **A** (visibilidad) — reduce tiempo de diagnóstico ya solo.  
2. **B** (flags mínimos) — habilita política en API.  
3. **E** (processing) — mitiga el daño más frecuente post-corte.  
4. **D** (colas desde BD) — desbloquea Fetcher+ sin re-discovery completo.  
5. **C** (discovery resume) — el caso más delicado; conviene después de tener B+D claros.  
6. **F** (UI + runbook).

---

## Riesgos y decisiones abiertas

- **Idempotencia** al republicar mensajes (mismo documento dos veces en cola).
- **Autenticación** del hub para eventos de incidente vs admin UI.
- **Fuentes no CSJN** con `PageSize` distinto: el resume debe ser **por estrategia**, no una fórmula global ÷10.

---

## Definición de “terminado” para este plan

- Incidente de infra: **visible** y **acotado** en tiempo.
- Job: puede **salir** de degradado con reglas claras y **continuar** sin duplicados masivos ni colas desalineadas.
- Operador: tiene **acciones** en UI o API documentadas para los casos borde (discovery a mitad, cola vacía, `Processing` colgado).
