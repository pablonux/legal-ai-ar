# Runbook: degradación y recuperación de infra (pipeline)

Este documento resume el flujo operativo cuando los workers reportan fallos de almacenamiento o red (colas Azure Storage) y cómo volver a un estado coherente con la API y el panel de ingesta.

## Referencias

- Plan de implementación: [pipeline-degradation-recovery-implementation-plan.md](pipeline-degradation-recovery-implementation-plan.md)
- Pre-script SQL (BD antes de reencolar): [../scripts/prescript-pipeline-job-recovery.sql](../scripts/prescript-pipeline-job-recovery.sql)

## Qué observás en el sistema

1. **Workers**: errores `RequestFailedException` u otros al leer o publicar en colas; el worker puede invocar `ReportInfraIncident` al hub (con rate limit en el cliente worker).
2. **API**: el coordinador persiste `InfrastructureDegraded` en el `IngestionJob` cuando el incidente trae `IngestionJobId`, y emite `InfraDegradedAsync` a clientes SignalR (con ventana corta de deduplicación por clave worker/cola/código/job).
3. **Admin (Angular)**: el panel de ingesta se conecta al hub `/hubs/worker-control`, muestra un banner ante incidente en vivo o job degradado en BD, y ofrece acciones sobre el **job activo** (heurística del panel).

## Comprobaciones rápidas

| Acción | Dónde |
|--------|--------|
| Probar solo conectividad de colas Discoverer/Fetcher | `GET /api/admin/infra/storage-probe` o botón **Probar colas** en el banner |
| Ver estado del job y workers | `GET /api/admin/jobs/{id}/audit` (modal **Auditar** en ingesta) |

## Recuperación guiada (un job)

Orden recomendado tras confirmar que la VPN o el storage volvieron a la normalidad:

1. **Probar colas** (`storage-probe`). Si falla, no sigas con requeue masivo hasta resolver conectividad o permisos.
2. **Recuperar job** (`POST /api/admin/jobs/{id}/recover-from-infra` con cuerpo por defecto): prueba de storage (si no la desactivás), limpia flags degradados, broadcast de recuperación, reencola **Fetcher** pendientes, reanuda workers en SignalR y BD.
3. Si hay **Pending en otras etapas** sin mensaje en cola, usá **Recuperar + reencolar todo** (mismo endpoint con `requeueAllPipelineStages: true`) o directamente `POST .../requeue-missing-pipeline-messages`.
4. Si quedaron filas **Processing** huérfanas (workers caídos), usá **Processing stale → Pending** (`POST .../reconcile-processing-documents` con `minAgeMinutes`, por defecto 15 en la UI) y luego reencolado si hace falta.
5. Si el descubrimiento quedó a medias: `POST .../resume-discovery` (no está en el banner por defecto; usá la API o extendé el flujo según necesidad).

## Discoverer y cola Fetcher (encolado incremental)

El worker **Discoverer** publica mensajes a la cola **Fetcher** al cerrar cada batch de discovery (página de resultados), sin esperar a que termine todo el rango de fechas. Efectos operativos:

- Un job puede pasar a **processing** y tener trabajo en Fetcher mientras el discovery sigue en curso.
- Si el job termina en **failed** por un error al persistir un documento en discovery, **los mensajes ya encolados** siguen válidos: el Fetcher puede seguir procesando esos documentos aunque el job figure fallido. Revisá logs del Discoverer (`FetcherEnqueued` / advertencia de batches posteriores) y el audit del job antes de reencolar a ciegas.

## Riesgos

- **Duplicados**: reencolar mientras un mensaje sigue invisible en la cola puede duplicar trabajo. Preferí ventanas con colas vacías o el criterio del audit (`structuralRepairSafety`).
- **Reconcile**: solo resetea `Processing` **stale** según antigüedad; no sustituye analizar por qué el worker dejó de avanzar.

## Endpoints admin relevantes

- `GET /api/admin/infra/storage-probe`
- `POST /api/admin/jobs/{id}/recover-from-infra`
- `POST /api/admin/jobs/{id}/requeue-fetcher-pending`
- `POST /api/admin/jobs/{id}/requeue-missing-pipeline-messages?stage=` (opcional)
- `POST /api/admin/jobs/{id}/reconcile-processing-documents`
- `POST /api/admin/jobs/{id}/resume-discovery`

Cuerpo de ejemplo para recuperación amplia:

```json
{
  "requireStorageProbe": true,
  "clearInfrastructureDegraded": true,
  "broadcastRecovered": true,
  "resumeDiscovery": false,
  "requeueFetcherPending": true,
  "requeueAllPipelineStages": true,
  "resumeAllWorkers": true
}
```
