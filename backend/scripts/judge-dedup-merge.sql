-- =============================================================================
-- Judge Deduplication: ALTER TABLE + Merge 523 duplicates to canonical records
-- Source: sjconsulta.csjn.gov.ar (10 titulares) + abrirAnalisis API (33 conjueces)
-- Run against LegalAI-DEV after deploying code changes.
-- =============================================================================

BEGIN TRANSACTION;

-- ─────────────────────────────────────────────────────────────────────────────
-- 1. Add CsjnMinistroId column + unique filtered index
-- ─────────────────────────────────────────────────────────────────────────────
-- Judges: CsjnMinistroId, Role, IsVerified
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Judges') AND name = 'CsjnMinistroId')
    ALTER TABLE dbo.Judges ADD CsjnMinistroId INT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Judges') AND name = 'Role')
    ALTER TABLE dbo.Judges ADD Role NVARCHAR(30) NOT NULL DEFAULT 'Unknown';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Judges') AND name = 'IsVerified')
    ALTER TABLE dbo.Judges ADD IsVerified BIT NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Judges') AND name = 'IX_Judges_CsjnMinistroId')
    CREATE UNIQUE NONCLUSTERED INDEX IX_Judges_CsjnMinistroId
        ON dbo.Judges (CsjnMinistroId)
        WHERE CsjnMinistroId IS NOT NULL;

-- Courts: ExternalCode, IsVerified
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Courts') AND name = 'ExternalCode')
    ALTER TABLE dbo.Courts ADD ExternalCode NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Courts') AND name = 'IsVerified')
    ALTER TABLE dbo.Courts ADD IsVerified BIT NOT NULL DEFAULT 0;

-- JudgeTenures table
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('dbo.JudgeTenures') AND type = 'U')
BEGIN
    CREATE TABLE dbo.JudgeTenures (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        JudgeId INT NOT NULL,
        CourtId INT NOT NULL,
        Role NVARCHAR(30) NOT NULL,
        StartDate DATE NULL,
        EndDate DATE NULL,
        IsCurrent BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_JudgeTenures_Judge FOREIGN KEY (JudgeId) REFERENCES dbo.Judges(Id) ON DELETE CASCADE,
        CONSTRAINT FK_JudgeTenures_Court FOREIGN KEY (CourtId) REFERENCES dbo.Courts(Id) ON DELETE NO ACTION
    );
    CREATE UNIQUE NONCLUSTERED INDEX IX_JudgeTenures_Judge_Court_Start
        ON dbo.JudgeTenures (JudgeId, CourtId, StartDate)
        WHERE StartDate IS NOT NULL;
END;

-- ─────────────────────────────────────────────────────────────────────────────
-- 2. Canonical minister data (from CSJN website + API)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE #Canonical (
    CsjnMinistroId INT PRIMARY KEY,
    FirstName NVARCHAR(200),
    LastName NVARCHAR(200),
    IsTitular BIT
);

-- Titulares (confirmed from CSJN search filter)
INSERT INTO #Canonical VALUES
    (1,   N'Ricardo Luis',    N'Lorenzetti',         1),
    (2,   N'Elena Ines',      N'Highton de Nolasco', 1),
    (3,   N'Carlos Santiago',  N'Fayt',               1),
    (4,   N'Enrique Santiago', N'Petracchi',          1),
    (5,   N'Juan Carlos',     N'Maqueda',             1),
    (6,   N'Carmen Maria',    N'Argibay',             1),
    (7,   N'Eugenio Raul',   N'Zaffaroni',           1),
    (68,  N'Horacio Daniel',  N'Rosatti',             1),
    (69,  N'Carlos Fernando', N'Rosenkrantz',         1),
    (241, N'Manuel Jose',     N'Garcia-Mansilla',     1),
    -- Conjueces (from abrirAnalisis ministros[] in 1811 fallos destacados)
    (42,  N'',  N'Tazza',                  0),
    (72,  N'',  N'Sotelo de Andreau',      0),
    (73,  N'',  N'Irurzun',                0),
    (80,  N'',  N'Montesi',                0),
    (82,  N'',  N'Cossio',                 0),
    (83,  N'',  N'Moran',                  0),
    (85,  N'',  N'Gonzalez',               0),
    (87,  N'',  N'Antelo',                 0),
    (88,  N'',  N'Tyden',                  0),
    (102, N'',  N'Aranguren',              0),
    (163, N'',  N'Amabile',                0),
    (172, N'',  N'Rabbi-Baldi Cabanillas', 0),
    (185, N'',  N'Boldu',                  0),
    (193, N'',  N'Alcala',                 0),
    (194, N'',  N'Bruglia',                0),
    (201, N'',  N'Castellanos',            0),
    (217, N'',  N'Borinsky',               0),
    (219, N'',  N'Bertuzzi',               0),
    (221, N'',  N'Moran',                  0),
    (227, N'',  N'Lozano',                 0),
    (229, N'',  N'Hornos',                 0),
    (230, N'',  N'Perez Tognola',          0),
    (231, N'',  N'Corcuera',               0),
    (232, N'',  N'Sanchez Torres',         0),
    (237, N'',  N'Tazza',                  0),
    (238, N'',  N'Andalaf Casiello',       0),
    (240, N'',  N'Candisano Mera',         0),
    (244, N'',  N'Moltini',                0),
    (245, N'',  N'Llorens',                0),
    (248, N'',  N'Castineira de Dios',     0),
    (250, N'',  N'Catalano',               0),
    (251, N'',  N'Bejas',                  0),
    (253, N'',  N'Perozziello Vizier',     0);

-- ─────────────────────────────────────────────────────────────────────────────
-- 3. For each canonical minister, find the BEST existing Judge record
--    Priority: exact FirstName+LastName match > LastName match > any surname match
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE #CanonicalJudge (
    CsjnMinistroId INT PRIMARY KEY,
    CanonicalJudgeId INT NOT NULL,
    FirstName NVARCHAR(200),
    LastName NVARCHAR(200)
);

-- Titulares: match by exact first+last, then by last name, then by surname anywhere
INSERT INTO #CanonicalJudge
SELECT c.CsjnMinistroId, j.Id, c.FirstName, c.LastName
FROM #Canonical c
CROSS APPLY (
    SELECT TOP 1 Id FROM (
        -- Priority 1: exact match
        SELECT Id, 1 AS Priority FROM dbo.Judges
        WHERE FirstName = c.FirstName AND LastName = c.LastName
        UNION ALL
        -- Priority 2: LastName exact match (LLM may have wrong first name)
        SELECT Id, 2 FROM dbo.Judges
        WHERE LastName = c.LastName AND c.IsTitular = 1
        UNION ALL
        -- Priority 3: surname in FirstName (when LastName is empty, e.g. Id 4126 "Lorenzetti"|"")
        SELECT Id, 3 FROM dbo.Judges
        WHERE LastName = '' AND FirstName = c.LastName AND c.IsTitular = 1
    ) ranked
    ORDER BY Priority, Id
) j
WHERE c.IsTitular = 1;

-- Conjueces: match by LastName only (FirstName is empty for conjueces)
INSERT INTO #CanonicalJudge
SELECT c.CsjnMinistroId, j.Id, c.FirstName, c.LastName
FROM #Canonical c
CROSS APPLY (
    SELECT TOP 1 Id FROM dbo.Judges
    WHERE (LastName = c.LastName)
       OR (LastName = '' AND FirstName = c.LastName)
    ORDER BY Id
) j
WHERE c.IsTitular = 0
  AND c.CsjnMinistroId NOT IN (SELECT CsjnMinistroId FROM #CanonicalJudge);

-- Update canonical records with correct names, CsjnMinistroId, Role, IsVerified
UPDATE j
SET j.FirstName      = cj.FirstName,
    j.LastName       = cj.LastName,
    j.CsjnMinistroId = cj.CsjnMinistroId,
    j.Role           = CASE WHEN c.IsTitular = 1 THEN 'Minister' ELSE 'Conjuez' END,
    j.IsVerified     = 1
FROM dbo.Judges j
INNER JOIN #CanonicalJudge cj ON j.Id = cj.CanonicalJudgeId
INNER JOIN #Canonical c ON c.CsjnMinistroId = cj.CsjnMinistroId;

-- ─────────────────────────────────────────────────────────────────────────────
-- 4. Build duplicate map for TITULARES ONLY
--    (conjueces like "Gonzalez", "Moran" are too common to mass-merge)
-- ─────────────────────────────────────────────────────────────────────────────
CREATE TABLE #DuplicateMap (
    DuplicateJudgeId INT PRIMARY KEY,
    CanonicalJudgeId INT NOT NULL
);

-- Find all judge records that match a titular minister but are NOT the canonical
INSERT INTO #DuplicateMap
SELECT DISTINCT j.Id, cj.CanonicalJudgeId
FROM dbo.Judges j
INNER JOIN #Canonical c ON c.IsTitular = 1
INNER JOIN #CanonicalJudge cj ON cj.CsjnMinistroId = c.CsjnMinistroId
WHERE j.Id <> cj.CanonicalJudgeId
  AND (
      -- LastName matches
      j.LastName = c.LastName
      -- Or LastName is part of compound (e.g. "Highton" matches "Highton de Nolasco")
      OR (c.LastName LIKE '%' + j.LastName + '%' AND j.LastName <> '' AND LEN(j.LastName) >= 4)
      -- Or FirstName contains surname (empty LastName cases)
      OR (j.LastName = '' AND j.FirstName = c.LastName)
      -- Or LastName is a fragment of canonical (e.g. "de Nolasco" -> "Highton de Nolasco")
      OR (c.LastName LIKE '%' + j.LastName + '%' AND j.LastName <> '' AND LEN(j.LastName) >= 5
          AND j.LastName NOT IN ('Luis', 'Carlos', 'Dr.', 'Unknown'))
      -- Or compound typos (e.g. "Luis Lorenzetti" in LastName)
      OR (j.LastName LIKE '%' + c.LastName + '%' AND LEN(c.LastName) >= 5)
  )
  AND j.Id NOT IN (SELECT CanonicalJudgeId FROM #CanonicalJudge);

-- Safety: exclude judges that appear in RulingJudges for non-CSJN rulings (if any exist)
-- (Not needed now since all rulings are from CSJN, but defensive)

-- ─────────────────────────────────────────────────────────────────────────────
-- 5. Preview before merge (comment out COMMIT to review)
-- ─────────────────────────────────────────────────────────────────────────────
SELECT 'CANONICAL JUDGES (will keep)' AS [Step];
SELECT cj.CsjnMinistroId, cj.CanonicalJudgeId, cj.FirstName, cj.LastName, c.IsTitular
FROM #CanonicalJudge cj
INNER JOIN #Canonical c ON c.CsjnMinistroId = cj.CsjnMinistroId
ORDER BY cj.CsjnMinistroId;

SELECT 'DUPLICATES TO MERGE' AS [Step];
SELECT dm.DuplicateJudgeId, j.FirstName AS OldFirst, j.LastName AS OldLast,
       dm.CanonicalJudgeId, cj.FirstName AS NewFirst, cj.LastName AS NewLast
FROM #DuplicateMap dm
INNER JOIN dbo.Judges j ON j.Id = dm.DuplicateJudgeId
INNER JOIN #CanonicalJudge cj ON cj.CanonicalJudgeId = dm.CanonicalJudgeId
ORDER BY cj.LastName, dm.DuplicateJudgeId;

-- ─────────────────────────────────────────────────────────────────────────────
-- 6. Remap RulingJudges and delete orphan duplicates
-- ─────────────────────────────────────────────────────────────────────────────

-- Delete RulingJudge rows that would create PK conflicts after remap
DELETE rj
FROM dbo.RulingJudges rj
INNER JOIN #DuplicateMap dm ON rj.JudgeId = dm.DuplicateJudgeId
WHERE EXISTS (
    SELECT 1 FROM dbo.RulingJudges existing
    WHERE existing.RulingId = rj.RulingId
      AND existing.JudgeId = dm.CanonicalJudgeId
);

-- Remap remaining RulingJudge rows to canonical
UPDATE rj
SET rj.JudgeId = dm.CanonicalJudgeId
FROM dbo.RulingJudges rj
INNER JOIN #DuplicateMap dm ON rj.JudgeId = dm.DuplicateJudgeId;

-- Delete orphaned duplicate Judge records
DELETE j
FROM dbo.Judges j
INNER JOIN #DuplicateMap dm ON j.Id = dm.DuplicateJudgeId
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.RulingJudges rj WHERE rj.JudgeId = j.Id
);

-- Also clean up judges with empty names and no rulings (junk from LLM)
DELETE FROM dbo.Judges
WHERE FirstName IN ('Dr.', 'Unknown', '')
  AND LastName IN ('Dr.', 'Unknown', '')
  AND Id NOT IN (SELECT JudgeId FROM dbo.RulingJudges);

-- ─────────────────────────────────────────────────────────────────────────────
-- 6b. Seed initial JudgeTenures for CSJN titular ministers
-- ─────────────────────────────────────────────────────────────────────────────
DECLARE @CsjnCourtId INT = (SELECT TOP 1 Id FROM dbo.Courts WHERE Name LIKE '%Corte Suprema%');

IF @CsjnCourtId IS NOT NULL
BEGIN
    INSERT INTO dbo.JudgeTenures (JudgeId, CourtId, Role, StartDate, EndDate, IsCurrent)
    SELECT cj.CanonicalJudgeId, @CsjnCourtId, 'Minister',
           CASE cj.CsjnMinistroId
               WHEN 1  THEN '2004-12-20' -- Lorenzetti
               WHEN 2  THEN '2004-06-07' -- Highton de Nolasco
               WHEN 3  THEN '1983-10-09' -- Fayt
               WHEN 4  THEN '1983-10-09' -- Petracchi
               WHEN 5  THEN '2002-12-06' -- Maqueda
               WHEN 6  THEN '2005-02-09' -- Argibay
               WHEN 7  THEN '2003-02-06' -- Zaffaroni
               WHEN 68 THEN '2016-06-29' -- Rosatti
               WHEN 69 THEN '2016-08-22' -- Rosenkrantz
               WHEN 241 THEN '2024-08-13' -- Garcia-Mansilla (en comision)
               ELSE NULL
           END,
           CASE cj.CsjnMinistroId
               WHEN 3  THEN '2015-09-11' -- Fayt
               WHEN 4  THEN '2014-10-31' -- Petracchi
               WHEN 6  THEN '2014-05-22' -- Argibay (fallecida)
               WHEN 7  THEN '2014-12-31' -- Zaffaroni
               WHEN 2  THEN '2021-11-01' -- Highton de Nolasco
               ELSE NULL
           END,
           CASE WHEN cj.CsjnMinistroId IN (1, 5, 68, 69, 241) THEN 1 ELSE 0 END
    FROM #CanonicalJudge cj
    INNER JOIN #Canonical c ON c.CsjnMinistroId = cj.CsjnMinistroId
    WHERE c.IsTitular = 1
      AND NOT EXISTS (
          SELECT 1 FROM dbo.JudgeTenures t
          WHERE t.JudgeId = cj.CanonicalJudgeId AND t.CourtId = @CsjnCourtId
      );
END;

-- ─────────────────────────────────────────────────────────────────────────────
-- 7. Final report
-- ─────────────────────────────────────────────────────────────────────────────
SELECT 'FINAL REPORT' AS [Step];
SELECT
    (SELECT COUNT(*) FROM #CanonicalJudge) AS CanonicalCreated,
    (SELECT COUNT(*) FROM #DuplicateMap)   AS DuplicatesMerged,
    (SELECT COUNT(*) FROM dbo.Judges)      AS RemainingJudges,
    (SELECT COUNT(*) FROM dbo.Judges WHERE CsjnMinistroId IS NOT NULL) AS WithMinistroId;

DROP TABLE #Canonical;
DROP TABLE #CanonicalJudge;
DROP TABLE #DuplicateMap;

COMMIT TRANSACTION;
