/*
  Snapshot del último IngestionJob (por StartedAt) para documentar benchmarks.
  Uso: sqlcmd tras load-env.ps1 (misma cuenta que desarrollo).

  Opcional — fijar un job concreto (comentá el DECLARE dinámico y descomenta):
  DECLARE @JobId UNIQUEIDENTIFIER = N'…………';
*/
SET NOCOUNT ON;

DECLARE @JobId UNIQUEIDENTIFIER =
(
    SELECT TOP (1) j.Id
    FROM dbo.IngestionJobs AS j
    ORDER BY j.StartedAt DESC
);

IF @JobId IS NULL OR @JobId = '00000000-0000-0000-0000-000000000000'
BEGIN
    RAISERROR('No ingestion jobs found.', 16, 1);
    RETURN;
END;

PRINT N'Benchmark snapshot JobId=' + CAST(@JobId AS NVARCHAR(36));
PRINT N'--------------------------------------------------------------';

-- ---
SELECT
    @JobId AS JobId,
    j.StartedAt,
    j.CompletedAt,
    j.Status,
    j.Type,
    j.TriggeredBy,
    j.DocumentsDiscovered,
    j.DocumentsCrawled,
    j.DocumentsParsed,
    j.DocumentsEnriched,
    j.DocumentsPersisted,
    j.DocumentsIndexed,
    j.DocumentsFailed,
    j.DocumentsSkipped,
    j.EntityType,
    j.SourceId
FROM dbo.IngestionJobs AS j
WHERE j.Id = @JobId;

-- ---
PRINT N'Documents (cohorte)';
SELECT TOP (100)
    d.Id AS DocumentId,
    d.ExternalId,
    d.CurrentStage,
    d.Status,
    d.RetryCount,
    d.LastUpdatedAt
FROM dbo.Documents AS d
WHERE d.IngestionJobId = @JobId
ORDER BY d.ExternalId;

-- ---
PRINT N'DocumentStageLogs summary by stage (DurationMs)';
SELECT
    l.Stage,
    COUNT(DISTINCT l.DocumentId) AS documents_with_log,
    SUM(l.DurationMs) AS sum_duration_ms,
    AVG(CAST(l.DurationMs AS FLOAT)) AS avg_duration_ms_per_row,
    SUM(CASE WHEN l.ErrorMessage IS NOT NULL THEN 1 ELSE 0 END) AS rows_with_error
FROM dbo.DocumentStageLogs AS l
INNER JOIN dbo.Documents AS d ON d.Id = l.DocumentId AND d.IngestionJobId = @JobId
GROUP BY l.Stage
ORDER BY
    CASE l.Stage
        WHEN N'Discoverer' THEN 1
        WHEN N'Fetcher' THEN 2
        WHEN N'Parser' THEN 3
        WHEN N'Enricher' THEN 4
        WHEN N'Persister' THEN 5
        WHEN N'Indexer' THEN 6
        ELSE 99
    END;

-- ---
PRINT N'Per-document pipeline sum (successful stage rows)';
SELECT TOP (50)
    d.ExternalId,
    d.Id AS DocumentId,
    SUM(ISNULL(l.DurationMs, 0)) AS pipeline_sum_ms_success_rows,
    COUNT(*) AS stage_log_rows
FROM dbo.Documents AS d
INNER JOIN dbo.DocumentStageLogs AS l ON l.DocumentId = d.Id
WHERE d.IngestionJobId = @JobId
  AND l.ErrorMessage IS NULL
GROUP BY d.ExternalId, d.Id
ORDER BY d.ExternalId;

-- ---
PRINT N'Ventana temporal (UTC) de logs para ese job — copiar a job-batch-stage-timing.sql';
SELECT @JobId AS JobId,
       MIN(l.StartedAt) AS LogsFromUtcMin,
       MAX(COALESCE(l.CompletedAt, l.StartedAt)) AS LogsToUtcMax
FROM dbo.DocumentStageLogs AS l
INNER JOIN dbo.Documents AS d ON d.Id = l.DocumentId AND d.IngestionJobId = @JobId;
