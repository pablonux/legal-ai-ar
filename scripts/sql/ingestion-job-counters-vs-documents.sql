/*
  Ola 9 — Reconciliación: contadores en IngestionJobs vs filas en Documents.

  Uso:
    - Dejar @JobId NULL para listar jobs recientes con posible deriva.
    - Fijar @JobId a un GUID para un solo job.

  Convenciones (pipeline actual):
    - Tras persistir con éxito, el documento pasa a CurrentStage = 'Indexer'.
      DocumentsPersisted debería alinearse con DocsAtIndexer (salvo bugs o edición manual).
    - Tras indexar con éxito, Status = 'Completed' (CurrentStage sigue 'Indexer').
      DocumentsIndexed debería alinearse con DocsCompletedIndexer.
    - DocumentsFailed vs DocsFailed: conteo de documentos en Status = 'Failed' para el job.

  No modifica datos; solo lectura.
*/

DECLARE @JobId uniqueidentifier = NULL;  -- p.ej. '843753e8-eaf3-41b3-b42f-37d8e3c119df'
DECLARE @RecentJobs int = 50;

WITH agg AS (
    SELECT
        d.IngestionJobId AS JobId,
        COUNT(*) AS DocRows,
        SUM(CASE WHEN d.CurrentStage = N'Indexer' THEN 1 ELSE 0 END) AS DocsAtIndexer,
        SUM(CASE WHEN d.CurrentStage = N'Indexer' AND d.Status = N'Completed' THEN 1 ELSE 0 END) AS DocsCompletedIndexer,
        SUM(CASE WHEN d.Status = N'Failed' THEN 1 ELSE 0 END) AS DocsFailed
    FROM dbo.Documents d
    GROUP BY d.IngestionJobId
)
SELECT
    j.Id AS JobId,
    j.Status AS JobStatus,
    j.StartedAt,
    j.DocumentsDiscovered,
    j.DocumentsSkipped,
    j.DocumentsCrawled,
    j.DocumentsParsed,
    j.DocumentsEnriched,
    j.DocumentsPersisted,
    j.DocumentsIndexed,
    j.DocumentsFailed,
    ISNULL(a.DocRows, 0) AS DocRows,
    ISNULL(a.DocsAtIndexer, 0) AS DocsAtIndexer,
    ISNULL(a.DocsCompletedIndexer, 0) AS DocsCompletedIndexer,
    ISNULL(a.DocsFailed, 0) AS DocsFailed,
    j.DocumentsPersisted - ISNULL(a.DocsAtIndexer, 0) AS DriftPersistedVsIndexer,
    j.DocumentsIndexed - ISNULL(a.DocsCompletedIndexer, 0) AS DriftIndexedVsCompleted,
    j.DocumentsFailed - ISNULL(a.DocsFailed, 0) AS DriftFailedVsDocs
FROM dbo.IngestionJobs j
LEFT JOIN agg a ON a.JobId = j.Id
WHERE (@JobId IS NULL OR j.Id = @JobId)
  AND (
        @JobId IS NOT NULL
        OR j.DocumentsPersisted <> ISNULL(a.DocsAtIndexer, 0)
        OR j.DocumentsIndexed <> ISNULL(a.DocsCompletedIndexer, 0)
        OR j.DocumentsFailed <> ISNULL(a.DocsFailed, 0)
        OR (j.DocumentsDiscovered > 0 AND ISNULL(a.DocRows, 0) = 0)
      )
ORDER BY j.StartedAt DESC
OFFSET 0 ROWS FETCH NEXT @RecentJobs ROWS ONLY;
