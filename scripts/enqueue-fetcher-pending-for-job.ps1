<#
.SYNOPSIS
    Encola mensajes FetcherMessage en la cola del Fetcher para todos los documentos
    Pending en etapa Fetcher de un job, con el mismo formato JSON que StorageQueuePublisher / Discoverer.

.DESCRIPTION
    - Lee AzureSql__ConnectionString y AzureBlob__ConnectionString desde .env (via load-env.ps1).
    - Prefijo de colas: Pipeline__QueuePrefix o "pipeline" (igual que appsettings).
    - Consulta Documents WHERE IngestionJobId = JobId AND CurrentStage = 'Fetcher' AND Status = 'Pending'.
    - Serializa cada mensaje en JSON camelCase (PropertyNamingPolicy.CamelCase), igual que LegalAiAr.Infrastructure.Queue.StorageQueuePublisher.
    - entityType numérico: Ruling=1, Statute=2 (LegalAiAr.Core.Enums.EntityType).
    - useCache / reprocess por defecto false (el Discoverer los tomaba del DiscovererMessage; no están en BD).

.PARAMETER JobId
    Guid del IngestionJob.

.PARAMETER DryRun
    Solo lista los documentos que se encolarían; no envía a la cola.

.PARAMETER UseCache
    Pasa useCache=true en cada FetcherMessage (equivale a DiscovererMessage.UseCache).

.PARAMETER Reprocess
    Pasa reprocess=true en cada FetcherMessage.

.EXAMPLE
    .\scripts\enqueue-fetcher-pending-for-job.ps1 -JobId '2413835d-1db8-4e4a-abd4-4b7e7bf216a7'

.EXAMPLE
    .\scripts\enqueue-fetcher-pending-for-job.ps1 -JobId '2413835d-1db8-4e4a-abd4-4b7e7bf216a7' -DryRun
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [guid]$JobId,

    [switch]$DryRun,

    [switch]$UseCache,

    [switch]$Reprocess
)

function Get-SqlConnectionParts {
    param([Parameter(Mandatory = $true)][string]$ConnectionString)
    $parts = @{}
    foreach ($seg in ($ConnectionString -split ';')) {
        $seg = $seg.Trim()
        if (-not $seg) { continue }
        $eq = $seg.IndexOf('=')
        if ($eq -lt 1) { continue }
        $k = $seg.Substring(0, $eq).Trim()
        $v = $seg.Substring($eq + 1).Trim()
        $parts[$k] = $v
    }

    $server = $parts['Server']
    if (-not $server) { $server = $parts['Data Source'] }
    if (-not $server) {
        throw 'La cadena SQL debe incluir Server= o Data Source=.'
    }
    $server = $server -replace '^tcp:', ''

    $database = $parts['Database']
    if (-not $database) { $database = $parts['Initial Catalog'] }
    if (-not $database) {
        throw 'La cadena SQL debe incluir Database= o Initial Catalog= (sin esto sqlcmd suele ir a master y devuelve 0 filas).'
    }

    $user = $parts['User ID']
    if (-not $user) { $user = $parts['UID'] }
    if (-not $user) { $user = $parts['User'] }
    if (-not $user) {
        throw 'La cadena SQL debe incluir User ID= (o UID / User).'
    }

    $password = $parts['Password']
    if ($null -eq $password) {
        throw 'La cadena SQL debe incluir Password=.'
    }

    [pscustomobject]@{
        Server   = $server
        Database = $database
        User     = $user
        Password = $password
    }
}

$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'load-env.ps1') | Out-Null

if (-not $env:AzureSql__ConnectionString) {
    Write-Error 'AzureSql__ConnectionString missing after load-env.'
}
if (-not $env:AzureBlob__ConnectionString) {
    Write-Error 'AzureBlob__ConnectionString missing after load-env.'
}

$prefix = if ($env:Pipeline__QueuePrefix) { $env:Pipeline__QueuePrefix } else { 'pipeline' }
$queueName = "$prefix-fetcher"
$blobConn = $env:AzureBlob__ConnectionString

$conn = Get-SqlConnectionParts -ConnectionString $env:AzureSql__ConnectionString

$jobLiteral = $JobId.ToString()

Write-Host 'Enqueue FetcherMessage (Pending @ Fetcher) for job'
Write-Host "  JobId:     $jobLiteral"
Write-Host "  SQL:       Server=$($conn.Server); Database=$($conn.Database)"
Write-Host "  Queue:     $queueName"
Write-Host "  DryRun:    $($DryRun.IsPresent)"
Write-Host "  UseCache:  $($UseCache.IsPresent)"
Write-Host "  Reprocess: $($Reprocess.IsPresent)"
Write-Host ''

$sqlcmdConn = @(
    '-S', $conn.Server,
    '-d', $conn.Database,
    '-U', $conn.User,
    '-P', $conn.Password,
    '-C'
)
# Nota: sqlcmd no permite combinar -W con -y/-Y. COUNT usa -W; JSON usa -y 0 -Y 0 (sin -W).

# Filtro común a COUNT y JSON (Guid tipado; trim por si hubiera espacios en columnas)
$whereJobStage = @"
d.IngestionJobId = CAST(N'$jobLiteral' AS uniqueidentifier)
  AND LTRIM(RTRIM(d.CurrentStage)) = N'Fetcher'
  AND LTRIM(RTRIM(d.Status)) = N'Pending'
"@

$countSql = @"
SET NOCOUNT ON;
SELECT COUNT_BIG(*) FROM Documents d WHERE $whereJobStage;
"@

Write-Host 'Counting matching documents...'
$countRaw = & sqlcmd @sqlcmdConn -h-1 -W -Q $countSql -b 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "sqlcmd (COUNT) falló con código $LASTEXITCODE : $countRaw"
}

$countLines = @($countRaw | ForEach-Object { "$_" } | Where-Object { $_ -match '^\s*\d+\s*$' })
$countLine = @($countLines | Select-Object -First 1)
if (-not $countLine) {
    Write-Error "No se pudo leer COUNT(*) de sqlcmd. Salida: $countRaw"
}
$docCount = [long]$countLine.Trim()
Write-Host "  COUNT Pending @ Fetcher = $docCount"
Write-Host ''

if ($docCount -eq 0) {
    Write-Host 'Nothing to enqueue (el COUNT devolvió 0 para esta Server/Database y JobId).'
    Write-Host 'Si en SSMS ves filas, comprobá que .env apunta al mismo servidor y Database/Initial Catalog.'
    exit 0
}

# FOR JSON en el nivel superior (evita errores con ORDER BY dentro de subconsulta escalar).
$sqlJson = @"
SET NOCOUNT ON;
SELECT
    CAST(d.Id AS VARCHAR(36)) AS documentId,
    d.EntityType AS entityType,
    d.SourceId AS sourceId,
    d.ExternalId AS externalId,
    ISNULL(d.AnalysisId, N'') AS analysisId,
    CAST(d.IngestionJobId AS VARCHAR(36)) AS ingestionJobId,
    ISNULL(CAST(d.FetchPdfTimeoutSeconds AS VARCHAR(20)), N'') AS fetchPdfTimeoutSeconds
FROM Documents d
WHERE $whereJobStage
ORDER BY d.CreatedAt
FOR JSON PATH;
"@

Write-Host 'Loading document rows (FOR JSON)...'
$tmpJson = [System.IO.Path]::ChangeExtension([System.IO.Path]::GetTempFileName(), 'json')
try {
    # No usar -h-1 junto con -y/-Y (sqlcmd moderno: opciones mutuamente excluyentes).
    # FOR JSON necesita -y 0 para no truncar; sin -h-1 el archivo puede incluir línea de cabecera → la limpiamos abajo.
    $jsonCmdMsg = @(& sqlcmd @sqlcmdConn -y 0 -Y 0 -u -Q $sqlJson -o $tmpJson -b 2>&1 | ForEach-Object { "$_" })
    if ($LASTEXITCODE -ne 0) {
        $detail = ($jsonCmdMsg -join "`n").Trim()
        if (-not $detail) { $detail = '(sin mensaje en stderr; revisá consulta SQL o permisos del archivo temporal)' }
        Write-Error "sqlcmd (JSON) falló con código $LASTEXITCODE. Detalle:`n$detail"
    }
    if (-not (Test-Path -LiteralPath $tmpJson)) {
        Write-Error "sqlcmd no creó el archivo de salida: $tmpJson"
    }
    # sqlcmd -u escribe el archivo en Unicode (UTF-16 LE); hay que leer con la misma codificación.
    $jsonBlob = [System.IO.File]::ReadAllText($tmpJson, [System.Text.Encoding]::Unicode)
}
finally {
    Remove-Item $tmpJson -ErrorAction SilentlyContinue
}

$jsonBlob = $jsonBlob.Trim().TrimStart([char]0xFEFF)
# Si sqlcmd dejó una línea de cabecera antes del array JSON, cortar desde el primer '['
if ($jsonBlob -notmatch '^\s*\[') {
    $i = $jsonBlob.IndexOf('[', [System.StringComparison]::Ordinal)
    if ($i -ge 0) { $jsonBlob = $jsonBlob.Substring($i) }
}
$jsonBlob = (
    @($jsonBlob -split "`r?`n") |
    Where-Object {
        $_ -and
        ($_ -notmatch '^\s*\(\d+ rows affected\)') -and
        ($_ -notmatch '^Sqlcmd:') -and
        ($_ -notmatch '^\-+$')
    }
) -join ''

if ([string]::IsNullOrWhiteSpace($jsonBlob) -or $jsonBlob.Trim() -match '^(NULL|\(NULL\))$') {
    Write-Error "COUNT era $docCount pero el JSON está vacío o NULL. Posible fallo de sqlcmd -o o resultado truncado."
}

try {
    # sqlcmd (-h-1) escribe el array JSON en archivo; unimos líneas por si hubiera basura.
    $docs = @($jsonBlob | ConvertFrom-Json)
}
catch {
    Write-Error "No se pudo interpretar la respuesta JSON de sqlcmd: $_`nSalida (primeros 500 chars): $($jsonBlob.Substring(0, [Math]::Min(500, $jsonBlob.Length)))"
}

if ($docs.Count -ne $docCount) {
    Write-Warning "El COUNT era $docCount pero JSON deserializó $($docs.Count) elemento(s). Podés tener datos cambiando entre COUNT y SELECT."
}

Write-Host "  Found $($docs.Count) document(s)."
Write-Host ''

if ($docs.Count -eq 0) {
    Write-Host 'Nothing to enqueue.'
    exit 0
}

function ConvertTo-FetcherMessageJson {
    param(
        [string]$DocId,
        [string]$EntityTypeStr,
        [string]$SourceIdStr,
        [string]$ExternalId,
        [string]$AnalysisIdRaw,
        [string]$IngestionJobIdStr,
        [string]$FetchTimeoutRaw,
        [bool]$UseCacheVal,
        [bool]$ReprocessVal
    )

    $entityTypeInt = switch ($EntityTypeStr.Trim()) {
        'Ruling' { 1 }
        'Statute' { 2 }
        default { throw "Unknown EntityType '$EntityTypeStr'. Expected Ruling or Statute." }
    }

    # Mismo formato que LegalAiAr.Infrastructure.Queue.StorageQueuePublisher (System.Text.Json, camelCase).
    # No usar ConvertTo-Json de PowerShell: puede emitir JSON no estándar y el Fetcher falla al deserializar.
    $d = [System.Collections.Generic.Dictionary[string, System.Object]]::new()
    $d['documentId'] = [guid]::Parse($DocId.Trim())
    $d['entityType'] = [int]$entityTypeInt
    $d['sourceId'] = [int]$SourceIdStr.Trim()
    $d['externalId'] = $ExternalId.Trim()
    $d['useCache'] = $UseCacheVal
    $d['reprocess'] = $ReprocessVal
    $d['ingestionJobId'] = [guid]::Parse($IngestionJobIdStr.Trim())

    $aid = $AnalysisIdRaw.Trim()
    if ($aid.Length -gt 0) {
        $d['analysisId'] = $aid
    }

    $ft = $FetchTimeoutRaw.Trim()
    if ($ft.Length -gt 0) {
        $d['fetchPdfTimeoutSeconds'] = [int]$ft
    }

    $opts = [System.Text.Json.JsonSerializerOptions]::new()
    $opts.PropertyNamingPolicy = [System.Text.Json.JsonNamingPolicy]::CamelCase
    return [System.Text.Json.JsonSerializer]::Serialize($d, $opts)
}

$published = 0
$errors = 0

# Un archivo UTF-8 por mensaje evita que az/PowerShell rompan comillas en --content "..."
$fetcherMsgTemp = [System.IO.Path]::ChangeExtension([System.IO.Path]::GetTempFileName(), 'json')
$utf8NoBom = [System.Text.UTF8Encoding]::new($false)
try {
    foreach ($row in $docs) {
        $docId = [string]$row.documentId
        $entityTypeStr = [string]$row.entityType
        $sourceIdStr = [string]$row.sourceId
        $externalId = [string]$row.externalId
        $analysisId = [string]$row.analysisId
        $ingestionJobId = [string]$row.ingestionJobId
        $fetchTimeout = if ($null -ne $row.fetchPdfTimeoutSeconds) { [string]$row.fetchPdfTimeoutSeconds } else { '' }

        try {
            $json = ConvertTo-FetcherMessageJson -DocId $docId -EntityTypeStr $entityTypeStr `
                -SourceIdStr $sourceIdStr -ExternalId $externalId -AnalysisIdRaw $analysisId `
                -IngestionJobIdStr $ingestionJobId -FetchTimeoutRaw $fetchTimeout `
                -UseCacheVal $UseCache.IsPresent -ReprocessVal $Reprocess.IsPresent
        }
        catch {
            Write-Host "  ERROR building JSON DocId=$docId : $_"
            $errors++
            continue
        }

        if ($DryRun) {
            Write-Host "[DRY] $externalId -> $json"
            $published++
            continue
        }

        $prevEA = $ErrorActionPreference
        $ErrorActionPreference = 'Continue'
        [System.IO.File]::WriteAllText($fetcherMsgTemp, $json, $utf8NoBom)
        # Sintaxis az: @ruta lee el cuerpo desde archivo (evita cortes por comillas en la línea de comandos).
        $contentArg = '@' + $fetcherMsgTemp
        $azOutput = az storage message put --queue-name $queueName --connection-string $blobConn --content $contentArg -o none 2>&1
        $azExit = $LASTEXITCODE
        $ErrorActionPreference = $prevEA

        if ($azExit -eq 0) {
            $published++
            if ($published % 50 -eq 0) { Write-Host "  Published $published..." }
        }
        else {
            Write-Host "  ERROR DocId=$docId exit=$azExit : $azOutput"
            $errors++
        }
    }
}
finally {
    Remove-Item $fetcherMsgTemp -ErrorAction SilentlyContinue
}

Write-Host ''
Write-Host "Done. Enqueued=$published Errors=$errors DryRun=$($DryRun.IsPresent)"
