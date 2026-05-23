/*
  Tiempos por etapa para los últimos ~5–10 docs reprocesados (sin correlation id).

  Opción 1 (recomendada): ventana [FromUtc, ToUtc) muy ajustada al momento del POST
    + Etapa donde encolaste (Parser, Fetcher, Enricher, Persister, Indexer).
    + TOP (@Take) documentos ordenados por el primer StartedAt en esa etapa.

  Opción 2 (abajo): comentás la CTE entrada/corrida y usás tabla @ExplicitIds.

  Stage en tabla = string PascalCase igual que enums .NET:
    Discoverer, Fetcher, Parser, Enricher, Persister, Indexer
*/

SET NOCOUNT ON;

/* --- Parámetros --- */
DECLARE @JobId UNIQUEIDENTIFIER = N'00000000-0000-0000-0000-000000000000'; -- reemplazar
DECLARE @EntryStage NVARCHAR(20)  = N'Parser';                           -- donde encolaste
DECLARE @Take INT                   = 10;                                 -- 5 o 10
DECLARE @FromUtc DATETIME2(0)      = '2026-05-07T20:05:00Z';              -- UTC
DECLARE @ToUtc DATETIME2(0)        = '2026-05-07T20:12:00Z';              -- UTC

/* Amplía sólo esta ventana al unir líneas de Downstream stages (persist/index tras ToUtc) */
DECLARE @PipelineHorizonUtc DATETIME2(0) = DATEADD(HOUR, 6, @ToUtc);

------------------------------------------------------------
-- Opción A: corrida por ventana + entrada en @EntryStage
------------------------------------------------------------
;WITH entrada AS (
    SELECT l.DocumentId, MIN(l.StartedAt) AS primera_entrada
    FROM dbo.DocumentStageLogs AS l
    INNER JOIN dbo.Documents AS d ON d.Id = l.DocumentId
    WHERE d.IngestionJobId = @JobId
      AND l.Stage = @EntryStage
      AND l.StartedAt >= @FromUtc
      AND l.StartedAt < @ToUtc
    GROUP BY l.DocumentId
),
corrida AS (
    SELECT TOP (@Take)
        e.DocumentId,
        e.primera_entrada
    FROM entrada AS e
    ORDER BY e.primera_entrada ASC
)
SELECT
    d.ExternalId,
    c.DocumentId,
    l.Stage,
    l.StartedAt,
    l.CompletedAt,
    l.DurationMs,
    l.WorkerInstanceId,
    l.ErrorMessage
FROM corrida AS c
INNER JOIN dbo.Documents AS d ON d.Id = c.DocumentId
INNER JOIN dbo.DocumentStageLogs AS l
    ON l.DocumentId = c.DocumentId
   AND l.StartedAt >= @FromUtc
   AND l.StartedAt < @PipelineHorizonUtc
ORDER BY
    c.primera_entrada,
    c.DocumentId,
    CASE l.Stage
        WHEN N'Discoverer' THEN 1
        WHEN N'Fetcher' THEN 2
        WHEN N'Parser' THEN 3
        WHEN N'Enricher' THEN 4
        WHEN N'Persister' THEN 5
        WHEN N'Indexer' THEN 6
        ELSE 99
    END,
    l.StartedAt;

------------------------------------------------------------
-- Resumen: ms totales por documento en la misma corrida (misma ventana en join)
------------------------------------------------------------
;WITH entrada AS (
    SELECT l.DocumentId, MIN(l.StartedAt) AS primera_entrada
    FROM dbo.DocumentStageLogs AS l
    INNER JOIN dbo.Documents AS d ON d.Id = l.DocumentId
    WHERE d.IngestionJobId = @JobId
      AND l.Stage = @EntryStage
      AND l.StartedAt >= @FromUtc
      AND l.StartedAt < @ToUtc
    GROUP BY l.DocumentId
),
corrida AS (
    SELECT TOP (@Take)
        e.DocumentId,
        e.primera_entrada
    FROM entrada AS e
    ORDER BY e.primera_entrada ASC
)
SELECT
    d.ExternalId,
    c.DocumentId,
    SUM(l.DurationMs) AS total_duration_ms,
    COUNT(*) AS stage_log_rows
FROM corrida AS c
INNER JOIN dbo.Documents AS d ON d.Id = c.DocumentId
INNER JOIN dbo.DocumentStageLogs AS l
    ON l.DocumentId = c.DocumentId
   AND l.StartedAt >= @FromUtc
   AND l.StartedAt < @PipelineHorizonUtc
   AND l.ErrorMessage IS NULL
GROUP BY d.ExternalId, c.DocumentId, c.primera_entrada
ORDER BY c.primera_entrada;

/*
------------------------------------------------------------
-- Opción B: IDs explícitos (si copiaste los GUID del admin)
------------------------------------------------------------
DECLARE @ExplicitIds TABLE (DocumentId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY);
INSERT INTO @ExplicitIds (DocumentId) VALUES
    (N'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'),
    (N'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb');
    -- ...

SELECT
    d.ExternalId,
    i.DocumentId,
    l.Stage,
    l.StartedAt,
    l.CompletedAt,
    l.DurationMs,
    l.WorkerInstanceId,
    l.ErrorMessage
FROM @ExplicitIds AS i
INNER JOIN dbo.Documents AS d ON d.Id = i.DocumentId
INNER JOIN dbo.DocumentStageLogs AS l ON l.DocumentId = i.DocumentId
WHERE d.IngestionJobId = @JobId
  AND l.StartedAt >= @FromUtc
  AND l.StartedAt < @PipelineHorizonUtc
ORDER BY i.DocumentId,
    CASE l.Stage
        WHEN N'Discoverer' THEN 1
        WHEN N'Fetcher' THEN 2
        WHEN N'Parser' THEN 3
        WHEN N'Enricher' THEN 4
        WHEN N'Persister' THEN 5
        WHEN N'Indexer' THEN 6
        ELSE 99
    END,
    l.StartedAt;
*/
