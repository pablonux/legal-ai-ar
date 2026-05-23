<#
.SYNOPSIS
    Downloads all Fallos Destacados (2015-2026) from the CSJN and fetches 8 API endpoints per fallo.

.DESCRIPTION
    Phase 1 - Index: Paginates fallosDestacados, saves consolidated index.json with all idAnalisis + codigo pairs.
    Phase 2 - API:   For each fallo downloads 8 endpoints (5 parser + 3 frontend-only):
      Parser:   abrirAnalisis, getAllDocumentos, getSumariosAnalisis, getCitas, getCitantes
      Frontend: getSintesisAnalisis, getDictamenesAnalisis, getEnlacesAnalisis

    Supports resume: existing files are skipped. Safe to re-run after interruption.

.PARAMETER ThrottleMs
    Delay between API requests in ms. Default: 600.
.PARAMETER OutputDir
    Root output directory.
.PARAMETER SkipIndex
    Skip Phase 1 if index.json exists.
.PARAMETER IndexOnly
    Run Phase 1 only.
#>
[CmdletBinding()]
param(
    [int]$ThrottleMs = 600,
    [string]$OutputDir = '',
    [switch]$SkipIndex,
    [switch]$IndexOnly
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

$baseUrl = 'https://sjconsulta.csjn.gov.ar/sjconsulta'
$pageSize = 10
$maxRetries = 3
$backoffMultiplier = 2.0
$requestTimeoutSec = 30

if (-not $OutputDir) {
    $scriptDir = $PSScriptRoot
    if (-not $scriptDir) { $scriptDir = Get-Location }
    $OutputDir = Join-Path $scriptDir 'samples'
}

$indexDir  = Join-Path $OutputDir 'fallosDestacados'
$indexFile = Join-Path $indexDir 'index.json'

$endpointDefs = @(
    @{ Name = 'abrirAnalisis';        Path = 'fallos/abrirAnalisis.html';           Param = 'idAnalisis';  UseAnalysisId = $true }
    @{ Name = 'getAllDocumentos';      Path = 'documentos/getAllDocumentos.html';     Param = 'idAnalisis';  UseAnalysisId = $true }
    @{ Name = 'getSumariosAnalisis';   Path = 'sumarios/getSumariosAnalisis.html';   Param = 'idAnalisis';  UseAnalysisId = $true }
    @{ Name = 'getSintesisAnalisis';   Path = 'fallos/getSintesisAnalisis.html';     Param = 'idAnalisis';  UseAnalysisId = $true }
    @{ Name = 'getDictamenesAnalisis'; Path = 'fallos/getDictamenesAnalisis.html';   Param = 'idAnalisis';  UseAnalysisId = $true }
    @{ Name = 'getEnlacesAnalisis';    Path = 'enlaces/getEnlacesAnalisis.html';     Param = 'idAnalisis';  UseAnalysisId = $true }
    @{ Name = 'getCitas';              Path = 'documentos/getCitas.html';            Param = 'idDocumento'; UseAnalysisId = $false }
    @{ Name = 'getCitantes';           Path = 'documentos/getCitantes.html';         Param = 'idDocumento'; UseAnalysisId = $false }
)

$subDirs = @('fallosDestacados') + ($endpointDefs | ForEach-Object { $_.Name })
foreach ($sub in $subDirs) {
    $p = Join-Path $OutputDir $sub
    if (-not (Test-Path $p)) { New-Item -ItemType Directory -Path $p -Force | Out-Null }
}

function Invoke-WithRetry {
    param(
        [string]$Uri,
        [string]$Method = 'GET',
        [string]$Body = $null,
        [string]$ContentType = $null,
        [Microsoft.PowerShell.Commands.WebRequestSession]$Session = $null
    )
    $delay = $ThrottleMs
    for ($attempt = 1; $attempt -le $maxRetries; $attempt++) {
        try {
            $params = @{ Uri = $Uri; Method = $Method; UseBasicParsing = $true; TimeoutSec = $requestTimeoutSec }
            if ($Session)     { $params['WebSession']  = $Session }
            if ($Body)        { $params['Body']        = $Body }
            if ($ContentType) { $params['ContentType']  = $ContentType }

            $response = Invoke-WebRequest @params
            Start-Sleep -Milliseconds $ThrottleMs
            return $response
        }
        catch {
            $status = $null
            if ($_.Exception.Response) { $status = [int]$_.Exception.Response.StatusCode }
            if ($status -eq 429 -or $status -ge 500) {
                $backoff = [int]($delay * $backoffMultiplier)
                Write-Warning ('  Attempt {0}/{1} failed (HTTP {2}) - retrying in {3}ms' -f $attempt, $maxRetries, $status, $backoff)
                Start-Sleep -Milliseconds $backoff
                $delay = $backoff
            }
            else {
                if ($attempt -eq $maxRetries) { throw }
                Write-Warning ('  Attempt {0}/{1} failed - {2}' -f $attempt, $maxRetries, $_.Exception.Message)
                Start-Sleep -Milliseconds $delay
            }
        }
    }
    throw ('All {0} attempts failed for {1}' -f $maxRetries, $Uri)
}

# ── Phase 1: Collect index ──────────────────────────────────────────────────

$records = @()

if ($SkipIndex -and (Test-Path $indexFile)) {
    Write-Host ('[Phase 1] Loading existing index from {0}' -f $indexFile)
    $records = @(Get-Content $indexFile -Raw | ConvertFrom-Json)
}
else {
    Write-Host '[Phase 1] Establishing session with fallosDestacados...'
    $buscarUrl = $baseUrl + '/fallosDestacados/buscar.html'
    $formBody = 'materia=&cabecilla=&fechaDesde=&fechaHasta=&g-recaptcha-response=dummy'
    $sessionResponse = Invoke-WebRequest -Uri $buscarUrl -Method POST -Body $formBody `
        -ContentType 'application/x-www-form-urlencoded' -UseBasicParsing -SessionVariable webSession

    $totalMatch = [regex]::Match($sessionResponse.Content, 'totalResultados\s*=\s*"(\d+)"')
    if (-not $totalMatch.Success) {
        Write-Error 'Could not extract totalResultados from session response'
        exit 1
    }
    $total = [int]$totalMatch.Groups[1].Value
    $totalPages = [Math]::Ceiling($total / $pageSize)
    Write-Host ('[Phase 1] Total fallos destacados: {0} ({1} pages)' -f $total, $totalPages)

    for ($page = 0; $page -lt $totalPages; $page++) {
        $indice = $page * $pageSize
        $pageNum = $page + 1
        Write-Host ('  Page {0}/{1}...' -f $pageNum, $totalPages) -NoNewline

        try {
            $paginateUrl = $baseUrl + '/fallosDestacados/paginarFallos.html?indice=' + $indice + '&ordenRelevancia=false'
            $pageResponse = Invoke-WithRetry -Uri $paginateUrl -Session $webSession
            $pageData = $pageResponse.Content | ConvertFrom-Json

            if ($pageData.Result -ne 'OK' -or -not $pageData.Records) {
                Write-Warning ' unexpected response - skipping'
                continue
            }

            foreach ($r in $pageData.Records) {
                $records += [PSCustomObject]@{
                    idAnalisis = $r.idAnalisis
                    codigo     = $r.codigo
                    fecha      = $r.fecha
                    caratula   = $r.caratula
                    materia    = $r.materia
                    tomoPagina = $r.tomoPagina
                    titulo     = $r.titulo
                    expediente = $r.identificadorExpediente
                }
            }
            Write-Host (' {0} records' -f $pageData.Records.Count)
        }
        catch {
            Write-Warning (' FAILED page {0} - {1}' -f $pageNum, $_.Exception.Message)
        }
    }

    Write-Host ('[Phase 1] Collected {0} records. Saving index...' -f $records.Count)
    $records | ConvertTo-Json -Depth 5 | Out-File -Encoding utf8 $indexFile -Force
    Write-Host ('[Phase 1] Saved: {0}' -f $indexFile)
}

if ($IndexOnly) {
    Write-Host 'IndexOnly flag set - done.'
    exit 0
}

# ── Phase 2: Download 8 API endpoints per fallo ─────────────────────────────

$totalRecords = $records.Count
$epCount = $endpointDefs.Count
$totalRequests = $totalRecords * $epCount
$downloaded = 0
$skipped = 0
$failed = 0
$sw = [System.Diagnostics.Stopwatch]::StartNew()

Write-Host ''
Write-Host ('[Phase 2] {0} fallos x {1} endpoints = {2} requests' -f $totalRecords, $epCount, $totalRequests)
Write-Host ''

for ($i = 0; $i -lt $totalRecords; $i++) {
    $record = $records[$i]
    $num = $i + 1
    $analysisId = $record.idAnalisis
    $documentId = $record.codigo

    foreach ($ep in $endpointDefs) {
        $id = if ($ep.UseAnalysisId) { $analysisId } else { $documentId }
        $outFile = Join-Path (Join-Path $OutputDir $ep.Name) ('{0}.json' -f $id)

        if (Test-Path $outFile) {
            $skipped++
            continue
        }

        $url = '{0}/{1}?{2}={3}' -f $baseUrl, $ep.Path, $ep.Param, $id
        try {
            $response = Invoke-WithRetry -Uri $url
            $response.Content | Out-File -Encoding utf8 $outFile -Force
            $downloaded++
        }
        catch {
            $failed++
            Write-Warning ('  FAILED [{0}] id={1} - {2}' -f $ep.Name, $id, $_.Exception.Message)
        }
    }

    if ($num % 50 -eq 0 -or $num -eq $totalRecords) {
        $elapsedSec = $sw.Elapsed.TotalSeconds
        $rps = if ($elapsedSec -gt 0) { [Math]::Round($downloaded / $elapsedSec, 1) } else { 0 }
        $pct = [Math]::Round(($num / $totalRecords) * 100, 1)
        $processed = $downloaded + $skipped + $failed
        $eta = '?'
        if ($rps -gt 0) {
            $left = $totalRequests - $processed
            $etaSec = [Math]::Round($left / $rps)
            $eta = [TimeSpan]::FromSeconds($etaSec).ToString('hh\:mm\:ss')
        }
        Write-Host ('  [{0}%] {1}/{2} | dl:{3} skip:{4} fail:{5} | {6} req/s | ETA {7}' -f `
            $pct, $num, $totalRecords, $downloaded, $skipped, $failed, $rps, $eta)
    }
}

$sw.Stop()
$finalTime = $sw.Elapsed.ToString('hh\:mm\:ss')
Write-Host ''
Write-Host '======================================================='
Write-Host ('  Done in {0}' -f $finalTime)
Write-Host ('  Downloaded: {0}' -f $downloaded)
Write-Host ('  Skipped:    {0}' -f $skipped)
Write-Host ('  Failed:     {0}' -f $failed)
Write-Host ('  Output:     {0}' -f $OutputDir)
Write-Host '======================================================='
