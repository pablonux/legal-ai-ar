<#
.SYNOPSIS
    Controlled day-by-day CSJN crawl via the API (most recent first).

.DESCRIPTION
    Iterates from EndDate backwards to StartDate, triggering one by-range crawl per day.
    Waits for each job to complete before moving to the next day.
    Skips days with no acuerdo results (weekends, holidays).
    Logs all results to a CSV file for post-analysis.

.PARAMETER ApiBase
    URL base of the API. Default: http://localhost:5088

.PARAMETER StartDate
    First date to crawl (oldest). Format: YYYY-MM-DD.

.PARAMETER EndDate
    Last date to crawl (most recent). Format: YYYY-MM-DD.

.PARAMETER MaxWaitSeconds
    Max seconds to wait per job before timing out. Default: 600.

.PARAMETER PollSeconds
    Seconds between job status polls. Default: 10.

.PARAMETER PauseOnError
    If set, pauses and asks before continuing after a failed job.

.EXAMPLE
    .\scripts\crawl-by-day.ps1 -StartDate 2025-01-01 -EndDate 2025-12-31
    Crawl all of 2025, one day at a time, most recent first.

.EXAMPLE
    .\scripts\crawl-by-day.ps1 -StartDate 2025-01-01 -EndDate 2025-12-31 -PauseOnError
    Same, but pauses on errors for manual decision.
#>

[CmdletBinding()]
param(
    [string]$ApiBase = "http://localhost:5088",
    [Parameter(Mandatory)][string]$StartDate,
    [Parameter(Mandatory)][string]$EndDate,
    [int]$MaxWaitSeconds = 600,
    [int]$PollSeconds = 10,
    [switch]$PauseOnError
)

$ErrorActionPreference = "Stop"

function Write-Ok   { param([string]$M) Write-Host $M -ForegroundColor Green }
function Write-Err  { param([string]$M) Write-Host $M -ForegroundColor Red }
function Write-Info { param([string]$M) Write-Host $M -ForegroundColor Cyan }
function Write-Warn { param([string]$M) Write-Host $M -ForegroundColor Yellow }

$dtStart = [datetime]::ParseExact($StartDate, "yyyy-MM-dd", $null)
$dtEnd   = [datetime]::ParseExact($EndDate, "yyyy-MM-dd", $null)

if ($dtStart -gt $dtEnd) {
    Write-Err "StartDate ($StartDate) must be <= EndDate ($EndDate)"
    exit 1
}

$totalDays = [int](($dtEnd - $dtStart).TotalDays) + 1
Write-Info "=== CSJN Day-by-Day Crawl ==="
Write-Info "Range: $EndDate (newest) -> $StartDate (oldest) = $totalDays days"
Write-Host ""

# ── 1. Verify API ──────────────────────────────────────────
Write-Info "Checking API at $ApiBase..."
try {
    $null = Invoke-RestMethod -Uri "$ApiBase/api/health" -Method Get -TimeoutSec 5
    Write-Ok "API is running"
} catch {
    Write-Err "API not available at $ApiBase. Start the API and workers first."
    exit 1
}

# ── 2. Prepare log file (local API injects platform identity in Development) ──
$headers = @{}
$logDir = Join-Path (Join-Path $PSScriptRoot "..") "logs"
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }
$logFile = Join-Path $logDir "crawl-by-day-$(Get-Date -Format 'yyyyMMdd-HHmmss').csv"
"Date,Discovered,Published,Skipped,Failed,Status,Duration_s,JobId" | Out-File $logFile -Encoding utf8
Write-Info "Log: $logFile"
Write-Host ""

# ── 4. Crawl loop (newest to oldest) ──────────────────────
$cursor = $dtEnd
$dayNumber = 0
$totalDiscovered = 0
$totalPublished = 0
$totalSkipped = 0
$totalFailed = 0
$daysWithData = 0
$daysEmpty = 0
$scriptStart = Get-Date

while ($cursor -ge $dtStart) {
    $dayNumber++
    $dateStr = $cursor.ToString("yyyy-MM-dd")
    $progress = [math]::Round(($dayNumber / $totalDays) * 100, 1)

    Write-Host ""
    Write-Info "[$dayNumber/$totalDays] ($progress%) Processing $dateStr..."

    # Trigger crawl
    $crawlBody = @{
        type = "by-range"
        dateFrom = $dateStr
        dateTo = $dateStr
    } | ConvertTo-Json

    $jobId = $null
    try {
        $crawlResp = Invoke-RestMethod -Uri "$ApiBase/api/admin/crawlers/1/run" `
            -Method Post -Headers $headers -Body $crawlBody -ContentType "application/json"
        Write-Host "  Crawl triggered: $($crawlResp.message)"
    } catch {
        $errMsg = $_.Exception.Message
        if ($errMsg -match "another job is already active") {
            Write-Warn "  Active job detected, waiting 30s and retrying..."
            Start-Sleep -Seconds 30
            try {
                $crawlResp = Invoke-RestMethod -Uri "$ApiBase/api/admin/crawlers/1/run" `
                    -Method Post -Headers $headers -Body $crawlBody -ContentType "application/json"
            } catch {
                Write-Err "  Failed to trigger crawl for $dateStr : $($_.Exception.Message)"
                "$dateStr,0,0,0,0,trigger_failed,0," | Out-File $logFile -Append -Encoding utf8
                $cursor = $cursor.AddDays(-1)
                continue
            }
        } else {
            Write-Err "  Failed to trigger crawl for $dateStr : $errMsg"
            "$dateStr,0,0,0,0,trigger_failed,0," | Out-File $logFile -Append -Encoding utf8
            $cursor = $cursor.AddDays(-1)
            continue
        }
    }

    # Wait for job to complete
    $waitStart = Get-Date
    $job = $null
    $lastStatus = ""

    while (((Get-Date) - $waitStart).TotalSeconds -lt $MaxWaitSeconds) {
        Start-Sleep -Seconds $PollSeconds

        try {
            $jobs = Invoke-RestMethod -Uri "$ApiBase/api/admin/jobs" -Method Get -Headers $headers
        } catch {
            Write-Warn "  Poll error: $($_.Exception.Message)"
            continue
        }

        $recent = $jobs | Where-Object {
            $_.sourceId -eq 1 -and
            $_.type -eq "by-range" -and
            $_.status -ne "pending"
        } | Sort-Object -Property startedAt -Descending | Select-Object -First 1

        if ($recent) {
            $lastStatus = $recent.status
            if ($lastStatus -in @("success", "completed", "failed", "cancelled", "partial")) {
                $job = $recent
                break
            }
            $elapsed = [math]::Round(((Get-Date) - $waitStart).TotalSeconds)
            Write-Host "  [$elapsed`s] status=$lastStatus discovered=$($recent.documentsDiscovered)" -NoNewline
            Write-Host ""
        }
    }

    $duration = [math]::Round(((Get-Date) - $waitStart).TotalSeconds)

    if ($job) {
        $discovered = if ($job.documentsDiscovered) { [int]$job.documentsDiscovered } else { 0 }
        $published = if ($job.documentsIndexed) { [int]$job.documentsIndexed } elseif ($job.documentsCrawled) { [int]$job.documentsCrawled } else { 0 }
        $failed = if ($job.documentsFailed) { [int]$job.documentsFailed } else { 0 }
        $skipped = [math]::Max(0, $discovered - $published - $failed)
        $jid = $job.id

        $totalDiscovered += $discovered
        $totalPublished += $published
        $totalSkipped += $skipped
        $totalFailed += $failed

        if ($discovered -gt 0) {
            $daysWithData++
            Write-Ok "  Done: discovered=$discovered published=$published skipped=$skipped failed=$failed status=$($job.status) (${duration}s)"
        } else {
            $daysEmpty++
            Write-Host "  No results for $dateStr (${duration}s)" -ForegroundColor DarkGray
        }

        "$dateStr,$discovered,$published,$skipped,$failed,$($job.status),$duration,$jid" | Out-File $logFile -Append -Encoding utf8

        if ($job.status -eq "failed" -and $PauseOnError) {
            Write-Warn "  Job failed. Continue? (Y/N)"
            $answer = Read-Host
            if ($answer -ne "Y" -and $answer -ne "y") {
                Write-Info "Stopping at user request."
                break
            }
        }
    } else {
        Write-Warn "  Timeout waiting for job on $dateStr after ${duration}s"
        "$dateStr,0,0,0,0,timeout,$duration," | Out-File $logFile -Append -Encoding utf8
    }

    $cursor = $cursor.AddDays(-1)
}

# ── 5. Summary ────────────────────────────────────────────
$totalElapsed = (Get-Date) - $scriptStart
Write-Host ""
Write-Info "========================================="
Write-Info "=== Crawl Complete ==="
Write-Info "========================================="
Write-Host "  Days processed:    $dayNumber / $totalDays"
Write-Host "  Days with data:    $daysWithData"
Write-Host "  Days empty:        $daysEmpty"
Write-Host "  Total discovered:  $totalDiscovered"
Write-Host "  Total published:   $totalPublished"
Write-Host "  Total skipped:     $totalSkipped"
Write-Host "  Total failed:      $totalFailed"
Write-Host "  Total time:        $($totalElapsed.ToString('hh\:mm\:ss'))"
Write-Host "  Log file:          $logFile"
Write-Host ""
