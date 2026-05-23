<#
.SYNOPSIS
    Ejecuta un job de crawl con captura de logs de cada worker para análisis posterior.

.DESCRIPTION
    Crea logs/run-YYYYMMDD-HHmmss/ con los logs de cada worker (crawler, parser, enrichment, indexer).
    Soporta dos modos:
    - Docker: captura logs de docker compose (requiere que los contenedores estén en ejecución)
    - Manual: asume workers vía Run and Debug; solo ejecuta el job y da instrucciones para guardar logs

    Criterio de cierre: docs descubiertos = rulings en BD + mensajes en DLQs.
    Si no cierran, hay pérdida de documentos entre workers.

.PARAMETER Mode
    Docker o Manual. Docker captura logs automáticamente; Manual solo ejecuta el job.

.PARAMETER DateFrom
    Para by-range: fecha inicio (YYYY-MM-DD). Ej: 2024-12-01.

.PARAMETER DateTo
    Para by-range: fecha fin (YYYY-MM-DD). Ej: 2024-12-31.

.PARAMETER SourceId
    ID de la fuente. Por defecto 1 (CSJN).

.PARAMETER MaxWaitSeconds
    Segundos máximos para esperar que el job aparezca y complete. Por defecto 900 (15 min).

.PARAMETER ApiBase
    URL base de la API. Por defecto http://localhost:5088.

.EXAMPLE
    .\scripts\run-job-with-logs.ps1 -Mode Docker -DateFrom "2024-12-01" -DateTo "2024-12-31"
    Ejecuta job Dic 2024 (1370 docs esperados) con captura de logs Docker.
#>

[CmdletBinding()]
param(
    [ValidateSet("Docker", "Manual")]
    [string]$Mode = "Docker",
    [string]$DateFrom = "2024-12-01",
    [string]$DateTo = "2024-12-31",
    [int]$SourceId = 1,
    [int]$MaxWaitSeconds = 900,
    [string]$ApiBase = "http://localhost:5088"
)

$ErrorActionPreference = "Stop"
$repoRoot = Join-Path $PSScriptRoot ".."

function Write-Success { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Fail { param([string]$Message) Write-Host $Message -ForegroundColor Red }
function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Warn { param([string]$Message) Write-Host $Message -ForegroundColor Yellow }

# --- 1. Crear directorio de logs ---
$logDir = Join-Path $repoRoot "logs\run-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
New-Item -ItemType Directory -Force $logDir | Out-Null
Write-Success "Logs en: $logDir"

# --- 2. Cargar .env ---
& "$PSScriptRoot\load-env.ps1" | Out-Null

# --- 3. Iniciar captura de logs (solo Docker) ---
$logJobs = @()
if ($Mode -eq "Docker") {
    $composeFile = Join-Path $repoRoot "docker-compose.app.yml"
    if (-not (Test-Path $composeFile)) {
        Write-Fail "No se encontró docker-compose.app.yml"
        exit 1
    }
    # Verificar que los contenedores estén corriendo
    Push-Location $repoRoot
    $containers = docker compose -f docker-compose.app.yml ps -q 2>$null
    Pop-Location
    if (-not $containers) {
        Write-Warn "Docker Compose no parece estar en ejecución."
        Write-Info "Ejecutá en otra terminal: docker compose -f docker-compose.app.yml up -d"
        Write-Info "Luego ejecutá este script de nuevo."
        exit 1
    }
    Write-Info "Iniciando captura de logs de workers..."
    $composePath = (Resolve-Path $composeFile).Path
    $workers = @("worker-crawler", "worker-parser", "worker-enrichment", "worker-indexer")
    foreach ($w in $workers) {
        $logFile = Join-Path $logDir "$w.log"
        $job = Start-Job -ScriptBlock {
            param($repo, $compose, $worker, $file)
            Set-Location $repo
            docker compose -f $compose logs -f $worker 2>&1 | Out-File -FilePath $file -Encoding utf8
        } -ArgumentList $repoRoot, $composePath, $w, $logFile
        $logJobs += $job
        Write-Host "  $w -> $logFile"
    }
    Start-Sleep -Seconds 2
}

if ($Mode -eq "Manual") {
    Write-Warn "Modo Manual: los workers deben estar corriendo (Run and Debug)."
    Write-Info "Para guardar logs: en cada terminal de worker, usá 'Guardar como' o copiá la salida."
}

# --- 4. Ejecutar job ---
Write-Info "Ejecutando crawl by-range $DateFrom .. $DateTo (sourceId=$SourceId)..."
try {
    & "$PSScriptRoot\test-full-job.ps1" -CrawlType by-range -DateFrom $DateFrom -DateTo $DateTo -SourceId $SourceId -MaxWaitSeconds $MaxWaitSeconds -ApiBase $ApiBase
} catch {
    Write-Fail "Error al ejecutar job: $_"
    if ($logJobs.Count -gt 0) { $logJobs | Stop-Job }
    exit 1
}

# --- 5. Esperar a que el job complete (poll) ---
Write-Info "Esperando que el job complete (poll cada 15s, máx ${MaxWaitSeconds}s)..."
# Local API injects platform identity in Development (no login required).
$headers = @{}
$start = Get-Date
$job = $null
$completed = $false
while (((Get-Date) - $start).TotalSeconds -lt $MaxWaitSeconds) {
    Start-Sleep -Seconds 15
    $jobs = Invoke-RestMethod -Uri "$ApiBase/api/admin/jobs" -Method Get -Headers $headers
    $recent = $jobs | Where-Object { $_.sourceId -eq $SourceId } | Sort-Object -Property startedAt -Descending | Select-Object -First 1
    if ($recent) {
        $job = $recent
        if ($recent.status -eq "Completed" -or $recent.status -eq "Failed") {
            $completed = $true
            break
        }
    }
    Write-Host "  Job status: $($recent.status) | discovered=$($recent.documentsDiscovered) indexed=$($recent.documentsIndexed) failed=$($recent.documentsFailed)"
}
if (-not $completed) {
    Write-Warn "Job no completó en el tiempo límite. Revisá manualmente."
}

# --- 6. Detener captura de logs (Docker) ---
if ($logJobs.Count -gt 0) {
    Write-Info "Deteniendo captura de logs..."
    $logJobs | Stop-Job
    $logJobs | Remove-Job
    Write-Success "Logs guardados en $logDir"
}

# --- 7. Verificar balance: docs descubiertos = rulings + DLQs ---
Write-Host ""
Write-Info "=== Verificación de balance ==="
Write-Host "Criterio: documentsDiscovered = rulings en BD + mensajes en DLQs"
Write-Host ""

$reportOutput = & "$PSScriptRoot\load-env.ps1" dotnet run --project "$repoRoot\backend\src\tools\LegalAiAr.EmptyKb\LegalAiAr.EmptyKb.csproj" -- --report 2>$null
$jsonLine = $reportOutput | Where-Object { $_ -match '\{' } | Select-Object -Last 1
$report = if ($jsonLine) { $jsonLine | ConvertFrom-Json } else { $null }
if ($report) {
    $discovered = if ($job) { $job.documentsDiscovered } else { "?" }
    $rulingsPlusDlq = $report.rulingsPlusDlq
    $ok = if ($job -and $discovered -ne "?") { $discovered -eq $rulingsPlusDlq } else { $null }
    Write-Host "  documentsDiscovered (job): $discovered"
    Write-Host "  rulings en BD:              $($report.rulings)"
    Write-Host "  DLQ crawler:                $($report.dlqCrawler)"
    Write-Host "  DLQ parser:                 $($report.dlqParser)"
    Write-Host "  DLQ enrichment:             $($report.dlqEnrichment)"
    Write-Host "  DLQ indexer:                $($report.dlqIndexer)"
    Write-Host "  rulings + DLQs:             $rulingsPlusDlq"
    Write-Host ""
    if ($ok -eq $true) {
        Write-Success "Los números cierran."
    } elseif ($ok -eq $false) {
        Write-Fail "Los números NO cierran. Revisá los logs en $logDir para analizar pérdida de documentos."
    }
} else {
    Write-Warn "No se pudo obtener reporte de conteos."
}

Write-Host ""
Write-Info "Logs en: $logDir"
