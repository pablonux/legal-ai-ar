# Plan de implementación completo + pre‑script de BD

Documentos base:

- [pipeline-degradation-recovery-master-plan.md](pipeline-degradation-recovery-master-plan.md) (fases A–F).
- [f1-pipeline-infra-health-signalr.md](f1-pipeline-infra-health-signalr.md) (SignalR / API).

Script SQL pre‑levantamiento: [`docs/scripts/prescript-pipeline-job-recovery.sql`](../scripts/prescript-pipeline-job-recovery.sql).

---

## 1. Objetivo de esta entrega

Implementar degradación/recuperación según el plan maestro y poder **levantar API + workers** sobre el **job actual en `processing`**, con BD alineada a un estado “sano para retomar” usando el script previo (y, cuando exista código, comandos de requeue).

---

## 2. Orden de trabajo (recomendado)

| Orden | Acción |
|-------|--------|
| 1 | **Parar** workers → **parar** API. |
| 2 | **Backup** ligero o anotar `JobId` + resultados de los `SELECT` del script. |
| 3 | Ejecutar **prescript SQL** (ajustar `@JobId` y `USE`). |
| 4 | **Implementar** código + **migraciones** (flags en `IngestionJobs`, hub, comandos). |
| 5 | `dotnet ef database update` (según [ef-migrations.md](../setup/ef-migrations.md)). |
| 6 | Si agregaste columnas nuevas: **opcional** segundo bloque SQL (ver §4) para simular “post‑degradación ya resuelta”. |
| 7 | **Levantar API** → verificar hub / health / admin. |
| 8 | **Levantar workers** → observar Fetcher; si la cola está vacía para ese job, usar **Requeue missing** (cuando implementado) o prueba controlada con VPN en **otro** job. |

---

## 3. Fases de implementación (resumen ejecutable)

### Fase A — Incidente + SignalR

- [x] NuGet / contratos: método hub `ReportInfraIncident` + DTO; rate limit.
- [x] `StorageQueueReceiver` (o capa común): clasificar `RequestFailedException` y reportar.
- [x] API: agregador + broadcast (`InfraIncidentCoordinator`, dedupe breve en `InfraDegradedAsync`).
- [x] Angular admin: banner + suscripción (`PipelineInfraHubService`, panel ingesta).
- [x] Reglas de **salida** de degradado (probe `GET /api/admin/infra/storage-probe`, ack vía `POST .../recover-from-infra` + broadcast `InfraRecoveredAsync`).

### Fase B — Flags en `IngestionJobs`

- [x] Migración: columnas acordadas (p. ej. `InfrastructureDegraded`, `DegradedSinceUtc`, `DegradedReason`, `DiscoveryBatchPublished`, …).
- [x] EF `IngestionJob` + configuración + mapeos en handlers que lean/escriban el job.

### Fase C — Resume discovery

- [x] Comando + handler: `ResumeDiscovery` / payload desde `DiscovererMessage` + skip/cursor (`PreserveProgressCounters`, solape excluye mismo `jobId`).
- [ ] Tests por tipo de crawl relevante.

### Fase D — Requeue desde BD

- [x] Comando requeue Fetcher pending por `jobId` (`POST .../requeue-fetcher-pending`). Requeue unificado multi-etapa: `POST .../requeue-missing-pipeline-messages`.
- [ ] Idempotencia / dedupe documentado.

### Fase E — Reconcile `Processing`

- [x] Política tiempo o comando admin (`POST .../reconcile-processing-documents`); integrar con auditoría existente (operador usa audit + esta acción).

### Fase F — UI + runbook

- [x] Botones / estados en ingesta; runbook operativo: [pipeline-infra-recovery-runbook.md](pipeline-infra-recovery-runbook.md).

---

## 4. Post‑migración: simular “degradación ya resuelta” (opcional)

Cuando las columnas existan, podés **setear** explícitamente el estado “feliz” para probar solo la parte downstream **sin** haber pasado por incidente real, por ejemplo:

```sql
-- Ejemplo: ajustar nombres de columnas al diseño final
UPDATE dbo.IngestionJobs
SET InfrastructureDegraded = 0,
    DegradedSinceUtc = NULL,
    DegradedReason = NULL,
    DiscoveryBatchPublished = 1
WHERE Id = @JobId;
```

(Mantener bloques `IF COL EXISTS` si conviven entornos con/sin migración.)

---

## 5. Qué hace exactamente el pre‑script **hoy** (sin flags nuevos)

El archivo [`prescript-pipeline-job-recovery.sql`](../scripts/prescript-pipeline-job-recovery.sql):

1. Valida que exista el `IngestionJob`.
2. Muestra contadores por `(CurrentStage, Status)` en `Documents`.
3. Pone el job en **`processing`** y limpia `ErrorSummary`.
4. Pasa **`Fetcher` + `Processing` → `Pending`** (huérfanos típicos tras caída), limpiando errores y `RetryCount`.
5. Vuelve a mostrar contadores.

**No crea mensajes en Azure Queue.** Si el problema real era “BD con Pending en Fetcher pero **sin** mensajes publicados”, **solo este script no alcanza** para que el Fetcher avance: necesitás la **Fase D** (requeue) o publicación manual hasta tener el comando.

---

## 6. Pruebas en dos capas (acordado)

| Capa | Qué valida |
|------|------------|
| **Fixture** | BD + script + flags + requeue/reconcile **sin** VPN. |
| **E2E** | Otro job, VPN mala → incidente → VPN buena → recuperación automática o guiada. |

---

## 7. Criterio de “listo para demo”

- Degradación **visible** en UI y/o logs centralizados.
- Salida de degradación **coherente** con reglas.
- Con job de prueba: tras script + requeue (cuando exista), **sube** `DocumentsCrawled` o al menos se consume la cola Fetcher sin 403.
- Reconcile deja **cero** `Processing` eternos en el escenario de prueba.

---

## 8. Riesgos a recordar al ejecutar el script

- **Duplicados** si un mensaje sigue en cola con visibility pendiente y re‑encolás el mismo trabajo.
- **Cadenas** `CurrentStage`/`Status` deben coincidir con EF (revisar una fila real en tu BD si algo no actualiza filas).
