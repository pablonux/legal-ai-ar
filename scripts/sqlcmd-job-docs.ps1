# Loads .env via load-env.ps1 (same folder), then queries Documents for the ingestion job.
param(
    [string]$JobId = '2413835d-1db8-4e4a-abd4-4b7e7bf216a7'
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'load-env.ps1')
if (-not $env:AzureSql__ConnectionString) {
    Write-Error 'AzureSql__ConnectionString missing after load-env.'
}

$cs = $env:AzureSql__ConnectionString
$pairs = @{}
foreach ($seg in ($cs -split ';')) {
    $seg = $seg.Trim()
    if ($seg -match '^([^=]+)=(.*)$') {
        $pairs[$matches[1].Trim()] = $matches[2].Trim()
    }
}

$S = $pairs['Server']
$d = $pairs['Database']
$u = $pairs['User ID']
$p = $pairs['Password']

$sql = @"
SET NOCOUNT ON;
SELECT ExternalId, CurrentStage, Status, RetryCount, LastUpdatedAt,
       LEFT(CAST(ISNULL(ErrorMessage, N'') AS NVARCHAR(MAX)), 140) AS ErrorPreview
FROM Documents
WHERE IngestionJobId = '$JobId'
  AND ExternalId IN (N'7988451', N'8033991')
ORDER BY ExternalId;

SELECT ExternalId, CurrentStage, Status, RetryCount, LastUpdatedAt
FROM Documents
WHERE IngestionJobId = '$JobId'
  AND CurrentStage = N'Indexer'
  AND Status IN (N'Pending', N'Processing', N'Failed', N'Completed')
ORDER BY Status, ExternalId;
"@

& sqlcmd -S $S -d $d -U $u -P $p -C -Q $sql
