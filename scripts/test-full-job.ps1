<#
.SYNOPSIS
    Prueba E2E completa: login, ejecutar crawl, verificar job.

.DESCRIPTION
    Asume que API y workers están en ejecución (docker-compose o Run and Debug).
    Ejecuta: login -> POST crawlers/1/run -> consulta jobs hasta ver el nuevo job.

.PARAMETER ApiBase
    URL base de la API. Por defecto http://localhost:5088.

.PARAMETER SourceId
    ID de la fuente a crawlear. Por defecto 1 (CSJN).

.PARAMETER CrawlType
    Tipo de crawl: incremental o by-range. Por defecto incremental.

.PARAMETER DateFrom
    Para by-range: fecha inicio (YYYY-MM-DD). Ej: 2024-12-01.

.PARAMETER DateTo
    Para by-range: fecha fin (YYYY-MM-DD). Ej: 2024-12-31.

.PARAMETER MaxWaitSeconds
    Segundos máximos para esperar que aparezca el job. Por defecto 120.

.EXAMPLE
    .\scripts\test-full-job.ps1
    Ejecuta la prueba con valores por defecto.

.EXAMPLE
    .\scripts\test-full-job.ps1 -CrawlType by-range -DateFrom "2024-12-01" -DateTo "2024-12-31" -MaxWaitSeconds 600
    Crawl por rango Dic 2024 (1370 docs esperados).
#>

[CmdletBinding()]
param(
    [string]$ApiBase = "http://localhost:5088",
    [int]$SourceId = 1,
    [ValidateSet("incremental", "by-range")]
    [string]$CrawlType = "incremental",
    [string]$DateFrom = "",
    [string]$DateTo = "",
    [int]$MaxWaitSeconds = 120
)

$ErrorActionPreference = "Stop"

function Write-Success { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Fail { param([string]$Message) Write-Host $Message -ForegroundColor Red }
function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Warn { param([string]$Message) Write-Host $Message -ForegroundColor Yellow }

# --- 1. Verificar que la API responde ---
Write-Info "Verificando API en $ApiBase..."
try {
    $health = Invoke-RestMethod -Uri "$ApiBase/api/health" -Method Get -TimeoutSec 5
    if ($health.Status -eq "Unhealthy") {
        Write-Warn "API responde pero reporta Unhealthy: $($health.Status)"
    } else {
        Write-Success "API OK (Status: $($health.Status))"
    }
} catch {
    Write-Fail "No se pudo conectar a la API. Asegurate de que esté en ejecución (Run and Debug o docker-compose)."
    Write-Host $_.Exception.Message
    exit 1
}

# --- 2. Ejecutar crawl (local API injects platform identity in Development) ---
$headers = @{}
if ($CrawlType -eq "by-range") {
    if (-not $DateFrom -or -not $DateTo) {
        Write-Fail "Para by-range se requieren -DateFrom y -DateTo (ej: -DateFrom '2024-12-01' -DateTo '2024-12-31')"
        exit 1
    }
    $crawlBody = @{ type = $CrawlType; dateFrom = $DateFrom; dateTo = $DateTo } | ConvertTo-Json
} else {
    $crawlBody = @{ type = $CrawlType } | ConvertTo-Json
}
Write-Info "Ejecutando crawl (sourceId=$SourceId, type=$CrawlType)..."
try {
    $crawlResp = Invoke-RestMethod -Uri "$ApiBase/api/admin/crawlers/$SourceId/run" -Method Post -Headers $headers -Body $crawlBody -ContentType "application/json"
    Write-Success "Crawl iniciado: $($crawlResp.message)"
} catch {
    Write-Fail "Error al ejecutar crawl: $($_.Exception.Message)"
    exit 1
}

# --- 4. Esperar y consultar jobs ---
Write-Info "Esperando job (máx. $MaxWaitSeconds s)..."
$start = Get-Date
$found = $false
$job = $null

while (((Get-Date) - $start).TotalSeconds -lt $MaxWaitSeconds) {
    Start-Sleep -Seconds 5
    $jobs = Invoke-RestMethod -Uri "$ApiBase/api/admin/jobs" -Method Get -Headers $headers
    $recent = $jobs | Where-Object { $_.sourceId -eq $SourceId } | Sort-Object -Property startedAt -Descending | Select-Object -First 1
    if ($recent -and $recent.startedAt) {
        $job = $recent
        $found = $true
        Write-Success "Job encontrado: $($job.id)"
        break
    }
    Write-Host "  Esperando job... ($([math]::Round(((Get-Date) - $start).TotalSeconds))s)"
}

if (-not $found) {
    Write-Warn "No se encontró job en el tiempo límite. Revisá que el worker Crawler esté en ejecución."
    Write-Info "Jobs actuales:"
    $jobs | ForEach-Object { Write-Host "  - $($_.id) | $($_.status) | discovered=$($_.documentsDiscovered) indexed=$($_.documentsIndexed) failed=$($_.documentsFailed)" }
    exit 0
}

# --- 5. Mostrar métricas ---
Write-Host ""
Write-Success "=== Job cargado ==="
Write-Host "  ID:              $($job.id)"
Write-Host "  Fuente:          $($job.sourceName) (sourceId=$($job.sourceId))"
Write-Host "  Tipo:            $($job.type)"
Write-Host "  Estado:          $($job.status)"
Write-Host "  Inicio:          $($job.startedAt)"
Write-Host "  Fin:             $($job.completedAt)"
Write-Host "  Descubiertos:    $($job.documentsDiscovered)"
Write-Host "  Indexados:       $($job.documentsIndexed)"
Write-Host "  Fallidos:        $($job.documentsFailed)"
if ($job.errorSummary) { Write-Host "  Error summary:   $($job.errorSummary)" }
Write-Host ""
Write-Info "Ver jobs en la UI: http://localhost:4200/admin/jobs"
