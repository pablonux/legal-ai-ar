# Workers / pipeline — protocolo de benchmark por olas

**Objetivo:** medir impacto de cambios en el pipeline (Parser, Persister, etc.) con cohortes repetibles, antes de mergear o descartar.

**Cómo trabajamos (Cursor + vos):**

1. El asistente te indica **qué ola** correr, **comandos** y **cómo medir**.
2. Vos ejecutás, anotás resultados en la sección **Registro de ejecuciones** (o pegás texto en el chat).
3. El asistente **actualiza este documento** con lo que trajiste y marca la ola siguiente.

**Última actualización:** 2026-05-07 — **Olas 7–9:** reset `Processing→Pending` en Persister e Indexer al arranque; uploads texto+metadata en paralelo en `ParserProcessor`; script SQL `ingestion-job-counters-vs-documents.sql` + runbook en README (Ola 9). Ola 6 omitida.

---

## Convenciones globales

| Elemento | Notas |
|---------|--------|
| **Cohorte** | Idealmente los **mismos 10 documentos** entre baseline y cambio (ExternalId o lista de GUID). |
| **Snapshot** | `dotnet run -c Release --project backend/src/tools/LegalAiAr.VerifyConnectivity/LegalAiAr.VerifyConnectivity.csproj -- benchmark-job-snapshot` |
| **Resumen por job** | `… VerifyConnectivity.csproj -- benchmark-job-stages <JobId-GUID>` (mismo agregado por `Stage` que el snapshot, sin listar documentos). |
| **SQL timing** | `scripts/sql/job-batch-stage-timing.sql` — preferir `@ExplicitIds` cuando la cohorte ya está fija. |
| **Reprocess** | `POST /api/admin/jobs/{jobId}/documents/reprocess-next` (body `stage`, `take`). |
| **UTC** | Ventanas `@FromUtc` / `@ToUtc` siempre en **UTC**. |
| **Git** | Anotar **rama + commit corto** en cada corrida. |
| **Reconciliación contadores** | `scripts/sql/ingestion-job-counters-vs-documents.sql` — ver Ola 9. |

### Qué traer al chat (plantilla corta)

```
Ola: N
Rama/commit: ...
JobId: ...
Cohorte: (10 ExternalId o “mismos IDs que ola anterior”)
EntryStage / ventana: ...
Resultado SQL: (pegar tabla resumida o screenshot en texto)
Observaciones: (errores, 429, workers viejos, VPN, etc.)
```

---

## Ola 0 — Medición estable (sin cambio de producto)

**Propósito:** cohorte fija + procedimiento SQL + notas de entorno para que las olas siguientes sean comparables.

### A) No tenés `IngestionJobId` todavía (p. ej. KB recién vaciada)

Sí: **lanzá un crawl de fallos destacados con tope 10**. El job **no viene en la respuesta HTTP**; el `IngestionJob` se crea cuando el **worker Discoverer** procesa el mensaje en la cola.

1. **API** (CSJN = `sourceId` **1**, ajustá si usás otra fuente):  
   `POST /api/admin/crawlers/1/run`  
   Body JSON ejemplo:
   ```json
   {
     "type": "fallos-destacados",
     "documentType": "ruling",
     "maxDocuments": 10,
     "useCache": true
   }
   ```
   También podés dispararlo desde la **UI admin** equivalente, si ya la tenés.

2. Asegurate de que el worker **Discoverer** esté levantado y consumiendo `pipeline-discoverer`.

3. Obtener el **JobId**: en la UI de jobs o en SQL, p. ej. el más reciente:
   ```sql
   SELECT TOP 3 Id, StartedAt, Type, Status
   FROM dbo.IngestionJobs
   ORDER BY StartedAt DESC;
   ```

4. Lista de documentos de ese job:
   ```sql
   SELECT Id, ExternalId, CurrentStage, Status
   FROM dbo.Documents
   WHERE IngestionJobId = @JobId
   ORDER BY ExternalId;
   ```
   Esos (hasta 10) son tu **cohorte inicial** para timings por `IngestionJobId` en el SQL.

5. **`reprocess-next` con `take: 10`** solo aplica a documentos **`Failed`** de **ese** job. Si los 10 terminaron `Indexed`/`Pending` en otra etapa, no habrá 10 fallidos hasta que algo falle o uses otra estrategia (medir la **primera** corrida con ventana + `@EntryStage`, o forzar fallos de prueba).

### B) Ya tenés job y fallidos (o cohorte fija)

| Paso | Qué hacer |
|------|-----------|
| 1 | Anotar `IngestionJobId` y **10** documentos (idealmente `Failed` para reprocesos). Exportar **`ExternalId` + `Id` (GUID)**. |
| 2 | Ejecutar `job-batch-stage-timing.sql` usando **Opción 2 `@ExplicitIds`** con esos 10 GUID (comentarios al final del script). Capturar filas por etapa (`DurationMs`, errores si hay). |
| 3 | (Opcional) Un `reprocess-next` de prueba y volver a correr el SQL con ventana acotada para validar. |

**Resultado esperado:** texto o tabla guardada + lista de IDs **congelada** como “cohorte golden”.

**Estado:** baseline **completado** (2026-05-07 UTC, ver tabla de registro abajo).

**Notas / resultados (Ola 0 — primera corrida sin errores):**

- **Rama@commit:** `feature/pipeline-improvements` @ `0047a158`
- **JobId:** `843753e8-eaf3-41b3-b42f-37d8e3c119df`
- **Job:** `type=fallos-destacados`, `StartedAt` → `CompletedAt` ≈ 2026-05-07 **22:33:31Z** → **22:37:15Z**, `Status=success`, `DocumentsFailed=0`, `SourceId=1`, `EntityType=Ruling`
- **Cohorte golden (ExternalId → DocumentId):**

| ExternalId | DocumentId |
|------------|------------|
| 8180461 | `151b2b83-c365-4895-b1a9-a23de0f35521` |
| 8254701 | `745a7aa5-29cf-4da4-8490-ff4addb20cc2` |
| 8259641 | `46f425dd-a809-4270-ab99-44cf04fd9150` |
| 8259831 | `dd9c3ba1-82ad-4803-ba01-ef2aa5e2609f` |
| 8262491 | `373ff040-1ea9-4abc-b6cb-62030c79b780` |
| 8263391 | `a8b66568-ea94-40b1-9e24-63c9c00f155d` |
| 8265211 | `eb64cfb6-91b7-4641-91c1-9d34ecdfdd24` |
| 8266181 | `3d6e6ed7-040d-4c82-8db4-a8eeb09c33fd` |
| 8266271 | `004b9d5b-1ba5-4240-a7b7-7b8c9f945052` |
| 8266301 | `7b7c9daa-d892-4584-995b-1abe36a418af` |

- **Todos los docs:** etapa final `Indexer`, `Completed`, `RetryCount=0`.

- **`DocumentStageLogs` — sumas de `DurationMs` por etapa (10 documentos cada una; valores son sumas de filas exitosas, no wall-clock de job):**

| Stage | docs | sum_duration_ms | avg_ms por fila de log |
|-------|------|-----------------|-------------------------|
| Fetcher | 10 | 39654 | ~3965 |
| Parser | 10 | 76441 | ~7644 |
| Enricher | 10 | 19545 | ~1955 |
| Persister | 10 | 140825 | ~14083 |
| Indexer | 10 | 54700 | ~5470 |

- **Suma por documento (solo filas sin error en logs):** mín. ~22534 ms · máx. ~59067 ms (`pipeline_sum_ms_success_rows`), 5 pasos registrados por documento (Fetcher → Indexer).

- **Ventana útil para `job-batch-stage-timing.sql` (UTC):** `LogsFromUtcMin` **`2026-05-07T22:33:54Z`** — `LogsToUtcMax` **`2026-05-07T22:37:16Z`** (margen recomendado: `@ToUtc` unos segundos después y `@PipelineHorizonUtc` como en el script).

**Próximo benchmark:** después de limpiar la KB y re-ejecutar el mismo crawl de 10 fallos destacados obtendrás **otro `JobId`**; repetí el snapshot (`benchmark-job-snapshot`) o **`@ExplicitIds`** con esta cohorte golden si querés reprocesos comparables sobre los mismos `DocumentId`/contenidos.

---

## Ola 1 — Métricas alineadas (Fetcher `DocumentsFailed`)

**Propósito:** que `IngestionJobs.DocumentsFailed` refleje fallos del Fetcher; no mide tiempo de pipeline pero evita decisiones equivocadas al elegir lotes.

**Depende de:** Ola 0 (sabés qué estás midiendo).

| Paso | Qué hacer |
|------|-----------|
| 1 | Sin cambio de código: anotar discrepancia actual (job vs filas `Documents` en Failed) si la ves en UI/SQL. |
| 2 | Tras implementar fix: repetir mismo escenario de fallo Fetcher y verificar contador. |

**Medición:** consulta SQL (reconciliación) o UI admin; **tests:** `dotnet test backend/tests/LegalAiAr.Worker.Fetcher.Tests -c Release`.

**Estado:** **completada** — en el `catch` del Fetcher se llama a **`IncrementDocumentsFailedAsync`** cuando el mensaje trae **`IngestionJobId`**:

```csharp
// FetcherWorkerService.cs (catch después de SetFailedAsync)
if (fetcherMessage.IngestionJobId.HasValue)
    await jobRepo.IncrementDocumentsFailedAsync(fetcherMessage.IngestionJobId.Value, CancellationToken.None);
```

**Notas / resultados:**

- **Tests unitarios** (`LegalAiAr.Worker.Fetcher.Tests`): fallo simulado en `FetchAsync` con `IngestionJobId` → se verifica `IncrementDocumentsFailedAsync` una vez; sin job id → no se incrementa.
- **Reconciliación manual (SQL)** tras un job con fallos solo en Fetcher:

```sql
DECLARE @JobId UNIQUEIDENTIFIER = N'…';

SELECT j.DocumentsFailed AS job_counter,
       COUNT(*) AS documents_failed_fetcher
FROM dbo.IngestionJobs j
INNER JOIN dbo.Documents d ON d.IngestionJobId = j.Id
WHERE j.Id = @JobId
  AND d.CurrentStage = N'Fetcher'
  AND d.Status = N'Failed'
GROUP BY j.DocumentsFailed;
```

  (`job_counter` debería coincidir con `documents_failed_fetcher` si todos los fallidos del job son de Fetcher y no hubo decrementos por requeue; si hay fallas en otras etapas, comparar por etapa o sumar solo Fetcher vs el delta esperado.)

---

## Ola 2 — Pausa real en workers

**Propósito:** que `WaitIfPaused` (o equivalente) detenga consumo en **todos** los workers; impacto operativo y menos ruido en benchmarks.

**Depende de:** convención de deploy (sabés qué pods/procesos son “nuevos”).

| Paso | Qué hacer |
|------|-----------|
| 1 | Pausar desde el mecanismo que usen (admin/API). |
| 2 | Verificar que **ninguna** cola principal baja mensajes (Discoverer/Fetcher/Parser/Enricher/Persister/Indexer). |
| 3 | Reanudar y confirmar que vuelve a consumir. |

**Medición:** principalmente **comportamiento** (sí/no); tiempos opcionales.

**Estado:** completada (implementación).

**Notas / resultados:**

- Cada worker de pipeline registra `AddWorkerControlGate` y, al inicio de cada ciclo del bucle principal, llama `await _workerGate.WaitIfPausedAsync(...)` antes de `ReceiveAsync` (Discoverer, Fetcher, Parser, Enricher, Persister, Indexer).
- Los procesos deben poder alcanzar la misma base URL que expone el hub SignalR del API. Si el API no está en `http://localhost:5088`, configurar **`WorkerControl__ApiBaseUrl`** (o `WorkerControl:ApiBaseUrl` en appsettings), por ejemplo `https://api.mi-entorno.example`.
- Panel admin: «Pausar todo» / estado por worker incluye **`Discoverer`** alineado con el tipo que usa el backend.

---

## Ola 3 — Artefactos en blob / menos round-trips a CSJN

**Propósito:** bajar varianza y carga en Parser antes de paralelizar llamadas CSJN.

**Depende de:** Ola 0 baseline de tiempos por etapa en Parser (si aplica).

**Medición:** mismo cohorte + `job-batch-stage-timing` con `@EntryStage` acorde (p. ej. `Parser`).

**Estado:** completada (implementación + post-medición en BBDD).

**Notas / resultados:**

- Las respuestas JSON de los 8 endpoints CSJN ya se **escribían** en blob (`_cache/csjn/api/...`) tras cada GET exitoso. Con `UseCache=false` el Parser **no las leía** antes de HTTP; ahora, por defecto, **`CsjnApi:PreferBlobApiCacheBeforeHttp=true`** hace que se **consulte blob antes que HTTP** aunque el mensaje de cola lleve `UseCache=false` (p. ej. crawl por defecto). Así reprocesos y re-ejecuciones reutilizan artefactos y reducen idas a sjconsulta.
- Para volver al comportamiento anterior (solo leer caché si `UseCache=true` en el mensaje): `CsjnApi__PreferBlobApiCacheBeforeHttp=false` en entorno o appsettings.

**Post-medición (2026-05-08 UTC, BBDD actual):**

- Comando: `benchmark-job-snapshot` → último job; `benchmark-job-stages <JobId>` para tablas por etapa.
- **Job medido:** `7f6d5b23-7ef1-4d40-8e69-95dd6fa11a87` — `fallos-destacados`, 10 docs, **mismos ExternalId** que la cohorte golden Ola 0; `StartedAt`≈**2026-05-07T23:47:40Z** → `CompletedAt`≈**2026-05-07T23:51:25Z**; `DocumentsFailed=0`.
- **Ventana `DocumentStageLogs`:** `2026-05-07T23:47:59Z` → `2026-05-07T23:51:25Z`.
- **`DocumentStageLogs` — sumas `DurationMs` por etapa (10 docs, sin errores en filas agregadas):**

| Stage | docs | sum_duration_ms | avg_ms por fila |
|-------|------|-----------------|-----------------|
| Fetcher | 10 | 55193 | ~5519 |
| Parser | 10 | 114290 | ~11429 |
| Enricher | 10 | 21576 | ~2158 |
| Persister | 10 | 142897 | ~14290 |
| Indexer | 10 | 59166 | ~5917 |

- **Suma pipeline por documento (filas sin `ErrorMessage`):** mín. **24324** ms · máx. **76709** ms (`pipeline_sum_ms_success_rows` en snapshot), 5 filas de log por doc.

**Comparación con Ola 0 (tabla fija en este README, job `843753e8-…`):**

| Stage | Ola 0 sum ms | Esta corrida sum ms | Δ |
|-------|----------------|---------------------|---|
| Fetcher | 39654 | 55193 | +39% |
| Parser | 76441 | 114290 | +50% |
| Enricher | 19545 | 21576 | +10% |
| Persister | 140825 | 142897 | +1% |
| Indexer | 54700 | 59166 | +8% |

**Interpretación:** en esta instancia el job baseline **ya no devolvía filas** en `benchmark-job-stages 843753e8-eaf3-41b3-b42f-37d8e3c119df` (sin logs unidos a documentos de ese job), así que la tabla Ola 0 es **referencia histórica** del doc, no re-query al mismo tiempo. La corrida medida es un **crawl completo nuevo** (nuevos `DocumentId`); variación de red/CSJN y **primera pasada** (caché API en blob aún fría) pueden subir `DurationMs` en `DocumentStageLogs` aunque el producto **reduzca llamadas HTTP** (Ola 3). Para aislar el efecto: repetir **reprocess** sobre la cohorte con `_cache` ya poblado y ventana corta en `job-batch-stage-timing.sql` (`@EntryStage = Parser`).

---

## Ola 4 — Throttle + paralelismo CSJN (post `abrirAnalisis`)

**Propósito:** throughput Parser con concurrencia acotada y menos 429.

**Depende de:** Ola 3 recomendado (menos varianza externa).

**Medición:** `job-batch-stage-timing`; comparar sumas/mediana `DurationMs` en `Parser`; anotar errores HTTP/throttle.

**Estado:** implementación + **post-medición** en BBDD (job `5c00923c-a968-452e-946a-ba622c06a678`, 2026-05-08).

**Implementación (2026-05-08):**

- Tras `abrirAnalisis`, los **7** GET restantes se ejecutan en **paralelo** cuando `CsjnApi:PostAbrirParallelEnabled=true` (default). Con `false`, se mantiene el orden secuencial anterior.
- Cada intento HTTP pasa por **`CsjnApiRequestGate`** (singleton en el proceso del Parser): espaciado mínimo entre **inicios** de petición según `ThrottlingDelayMs` y tope de **concurrencia global** `PostAbrirHttpMaxConcurrencyGlobal` (default **6**). Sustituye el antiguo `Task.Delay` fijo solo *después* de cada respuesta.
- Configuración: `appsettings.json` del worker Parser y variables de entorno `CsjnApi__*` (ver `.env.example`).

**Post-medición (2026-05-08, BBDD DEV — `benchmark-job-snapshot` / `benchmark-job-stages`):**

- **Rama@commit:** `766e0f08` (medición ejecutada desde entorno local contra la misma cuenta SQL que `.env`).
- **JobId:** `5c00923c-a968-452e-946a-ba622c06a678`
- **Job:** `type=fallos-destacados`, `StartedAt` → `CompletedAt` ≈ **2026-05-08T00:31:21Z** → **2026-05-08T00:35:38Z** (valores según fila `IngestionJobs` del snapshot), `Status=success`, `DocumentsFailed=0`, **mismos 10 `ExternalId`** que cohorte golden Ola 0 (nuevos `DocumentId`).
- **Ventana `DocumentStageLogs`:** `2026-05-08T00:31:54Z` → `2026-05-08T00:35:39Z`.

**`DocumentStageLogs` — sumas `DurationMs` por etapa (10 docs, `rows_with_error=0`):**

| Stage | docs | sum_duration_ms | avg_ms por fila |
|-------|------|-----------------|-----------------|
| Fetcher | 10 | 61190 | ~6119 |
| Parser | 10 | 130695 | ~13070 |
| Enricher | 10 | 24098 | ~2410 |
| Persister | 10 | 151103 | ~15110 |
| Indexer | 10 | 73995 | ~7400 |

- **Suma pipeline por documento (`pipeline_sum_ms_success_rows`, 5 filas sin error por doc):** mín. **28381** ms · máx. **85538** ms.

**Comparación con Ola 3 (job `7f6d5b23-7ef1-4d40-8e69-95dd6fa11a87`, misma cohorte de ExternalId, README):**

| Stage | Ola 3 sum ms | Ola 4 sum ms | Δ |
|-------|----------------|--------------|---|
| Fetcher | 55193 | 61190 | +11% |
| Parser | 114290 | 130695 | +14% |
| Enricher | 21576 | 24098 | +12% |
| Persister | 142897 | 151103 | +6% |
| Indexer | 59166 | 73995 | +25% |

**Interpretación:** en esta corrida los **sumatorios SQL por etapa subieron** respecto al job Ola 3 medido antes en el mismo doc; no implica regresión del paralelismo CSJN de forma aislada (variación de red, carga de workers, estado del blob `_cache`, y **KB recién reinicializada** según operación local previa). Para comparación más limpia: repetir con **cache caliente** y ventana fija en `job-batch-stage-timing.sql` o varias corridas y mediana. No se observaron filas con error en estos agregados; **429** no aparecen en `DocumentStageLogs` (no sustituyen telemetría HTTP del Parser).

---

## Ola 5 — `PersistCitedByAsync`: batch SaveChanges

**Propósito:** menos round-trips SQL en jobs con citas/cited-by densos.

**Depende de:** cohorte con **muchas** relaciones cited-by si se quiere efecto grande; si no, efecto puede ser modesto.

**Medición:** `job-batch-stage-timing` con `@EntryStage` `Persister` (o ventana centrada en persist).

**Estado:** implementada en código (**un** `SaveChangesAsync` al final de `PersistCitedByAsync` tras acumular citas inversas); medición Persister opcional con cohorte citante densa.

**Implementación (2026-05-08):** `RulingPersistStrategy.PersistCitedByAsync` ya no guarda por cada ítem; ante `DbUpdateException` en el lote se hace `DetachAddedCitationsOnly()` y se registra warning (antes se podía omitir un duplicado y conservar el resto).

**Notas / resultados (benchmark):** _pendiente — medir `@EntryStage = Persister` o sumas en job con muchos cited-by._

---

## Ola 6 — Réplicas Persister / afinado CPU (PdfPig, etc.)

**Propósito:** escalar **después** de optimizar persistencia; PdfPig aislado si el perfil lo muestra.

**Depende de:** Ola 5.

**Medición:** mismo protocolo SQL + notas de infra (N réplicas).

**Estado:** omitida (no se implementará en este roadmap).

**Notas / resultados:** —

---

## Ola 7 — Reset `Processing → Pending` (Persister/Indexer al arranque)

**Propósito:** recuperación tras caídas; medible con simulación de kill/restart si se desea.

**Depende de:** Ola 2 útil para pruebas controladas.

**Estado:** implementada en código (mismo patrón que Parser/Enricher/Fetcher): al arrancar, `IDocumentRepository.ResetProcessingToPendingAsync` para `PipelineStage.Persister` y `PipelineStage.Indexer` en `PersisterWorkerService` e `IndexerWorkerService`. Si hay filas reseteadas, se loguea warning con el conteo.

**Notas / resultados:** _benchmark opcional — dejar un doc en `Processing` en Persister o Indexer, reiniciar solo ese worker y comprobar vuelta a `Pending` + reproceso._

---

## Ola 8 — Mejoras menores (`Task.WhenAll` uploads, etc.)

**Propósito:** solo si el perfil muestra tiempo en blobs pequeños.

**Estado:** implementada en código: en `ParserProcessor` (CSJN), subida a blob del `.txt` normalizado y del JSON de metadata en **paralelo** con `Task.WhenAll` (dos `MemoryStream` independientes).

**Notas / resultados:** _medición opcional con `DocumentStageLogs` / timings Parser si se desea cuantificar._

---

## Ola 9 — Soporte (reconciliación contadores, runbooks)

**Propósito:** analítica e incidentes; fuera del bucle principal de timing.

**Estado:** script SQL de solo lectura `scripts/sql/ingestion-job-counters-vs-documents.sql` — compara `IngestionJobs.DocumentsPersisted` / `DocumentsIndexed` / `DocumentsFailed` con agregados derivados de `Documents` (etapa `Indexer`, completados, fallidos). Parámetros: `@JobId` (NULL = jobs recientes **con posible deriva**), `@RecentJobs`.

**Runbook (resumen):**

1. Ante job “atascado” o contadores sospechosos, ejecutar el script en SSMS/Azure Data Studio con `@JobId` del job.
2. Columnas `Drift*`: distancia entre contador de job y hechos en `Documents`; investigar DLQ, reinicios sin reset (antes de Ola 7), o edición manual.
3. Tras incidente, preferir cohorte nueva o reprocess admin según política del equipo.

**Notas / resultados:** _no sustituye telemetría de workers; lectura puntual._

---

## Registro de ejecuciones

| Fecha (UTC) | Ola | Rama@commit | JobId | Cohorte | Métrica principal | Δ vs anterior | Observaciones |
|-------------|-----|-------------|-------|---------|-------------------|---------------|---------------|
| 2026-05-08 | 4 post | 766e0f08 | `5c00923c-a968-452e-946a-ba622c06a678` | Mismos 10 ExternalId Ola 0; nuevos GUID | Ver tabla Ola 4 vs Ola 3 en README | Parser +14% sum SQL vs job `7f6d5b23…` | `benchmark-job-snapshot` + `benchmark-job-stages`; KB reinicializada antes (terminal local); variación crawl/cache |
| 2026-05-08 | 3 post | aedff4a1 | `7f6d5b23-7ef1-4d40-8e69-95dd6fa11a87` | Mismos 10 ExternalId Ola 0; nuevos GUID | Ver tabla Ola 3 vs Ola 0 en README | Parser +50% sum SQL\* | `benchmark-job-snapshot` + `benchmark-job-stages`; job baseline `843753…` sin logs en esta BBDD; variación crawl/cache |
| 2026-05-07 | 0 | feature/pipeline-improvements @ 0047a158 | `843753e8-eaf3-41b3-b42f-37d8e3c119df` | 10 CSJN destacados (tabla arriba) | Persister sum 140825 ms (10 docs); Parser 76441 | — baseline | Crawl OK; contadores job alineados; sin fallos (fix Persister FK Court) |

*(Añadir filas hacia arriba o abajo según prefieran.)*

---

## Próximo paso (lo indica el asistente en chat)

**Acción actual:** **Ola 5 post-medición (opcional)** — job con muchas citas + `job-batch-stage-timing` (`Persister`). **Ola 6** omitida. **Olas 7–9** cerradas en código/README; siguiente foco de producto según roadmap (p. ej. mejoras pipeline fuera de este doc) o nuevas olas si se definen.
