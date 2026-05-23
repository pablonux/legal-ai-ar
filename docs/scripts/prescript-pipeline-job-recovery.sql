/*
  Pre-levantamiento: alinear BD del job de prueba tras incidente / antes de probar recuperación.
  Ejecutar en SSMS contra la MISMA base que usa el API (tras backup opcional).

  IMPORTANTE
  - Reemplazar @JobId por el GUID del IngestionJob en processing.
  - Si en Azure Queue siguen mensajes "invisibles" para los mismos DocumentId,
    resetear Pending puede duplicar trabajo al volver visibles. Revisar visibility timeout
    o vaciar/peek la cola Fetcher solo si sabés que no hay duplicados peligrosos.
  - Los estados/etapas deben coincidir con las cadenas que persiste EF (PascalCase típico).
*/

SET NOCOUNT ON;
-- USE [TuBaseLegalAiAr];  -- descomentar y ajustar

DECLARE @JobId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000000'; /* <-- REEMPLAZAR */

IF NOT EXISTS (SELECT 1 FROM dbo.IngestionJobs WHERE Id = @JobId)
BEGIN
    RAISERROR('IngestionJob no encontrado.', 16, 1);
    RETURN;
END;

/* --- 1) Diagnóstico previo --- */
SELECT Id, Status, DocumentsDiscovered, DocumentsSkipped, DocumentsCrawled, DocumentsFailed, ErrorSummary, StartedAt
FROM dbo.IngestionJobs
WHERE Id = @JobId;

SELECT CurrentStage, Status, COUNT(*) AS Cnt
FROM dbo.Documents
WHERE IngestionJobId = @JobId
GROUP BY CurrentStage, Status
ORDER BY CurrentStage, Status;

/* --- 2) Job listo para que el pipeline retome (sin flags nuevos aún) --- */
UPDATE dbo.IngestionJobs
SET Status = N'processing',
    ErrorSummary = NULL
WHERE Id = @JobId;

/*
  Opcional (futuro): cuando existan columnas en IngestionJobs:
  InfrastructureDegraded = 0, DegradedSinceUtc = NULL, DiscoveryBatchPublished = 1, ...
  usar bloques IF EXISTS (sys.columns) como en migraciones idempotentes.
*/

/* --- 3) Huérfanos Fetcher en Processing -> Pending (reintento limpio) --- */
UPDATE dbo.Documents
SET Status = N'Pending',
    RetryCount = 0,
    ErrorMessage = NULL,
    ErrorType = NULL,
    LastUpdatedAt = SYSUTCDATETIME()
WHERE IngestionJobId = @JobId
  AND CurrentStage = N'Fetcher'
  AND Status = N'Processing';

/* Opcional: otros stages si tuviste corte a mitad */
/*
UPDATE dbo.Documents
SET Status = N'Pending', RetryCount = 0, ErrorMessage = NULL, ErrorType = NULL, LastUpdatedAt = SYSUTCDATETIME()
WHERE IngestionJobId = @JobId AND CurrentStage = N'Parser' AND Status = N'Processing';
*/

/* --- 4) Verificación posterior --- */
SELECT CurrentStage, Status, COUNT(*) AS Cnt
FROM dbo.Documents
WHERE IngestionJobId = @JobId
GROUP BY CurrentStage, Status
ORDER BY CurrentStage, Status;

SELECT Id, Status, DocumentsDiscovered, DocumentsSkipped, DocumentsCrawled, ErrorSummary
FROM dbo.IngestionJobs
WHERE Id = @JobId;

PRINT 'Listo. Siguiente: migraciones (si hay), levantar API, luego workers. Si Fetcher no consume, faltan mensajes en cola: usar comando Requeue (cuando exista) o revisar cola.';
