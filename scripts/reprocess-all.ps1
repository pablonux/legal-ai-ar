<#
.SYNOPSIS
    Reprocess all rulings through the V2 pipeline (parser → enrichment → indexer).
.DESCRIPTION
    1. Starts keep-awake (mouse mover) to prevent screen hibernation.
    2. Queues ParserMessages for all rulings via BulkRequeue (one-time operation).
    3. Monitors queue depths and DB counts until processing completes.

    Workers (parser, enrichment, indexer) must be running separately (Run & Debug or docker-compose).
    If VPN drops, workers will pause on transient errors; messages stay in queue.
    When VPN reconnects, workers resume automatically — no new job is created.

.PARAMETER DryRun
    Queue messages without publishing (count only).
.PARAMETER Limit
    Max rulings to queue (default: all).
.PARAMETER SkipQueue
    Skip the queueing step (if messages were already queued) and go directly to monitoring.
.PARAMETER PollSeconds
    Seconds between progress polls (default: 30).

.EXAMPLE
    .\scripts\reprocess-all.ps1
    Queue all rulings to parser and monitor until complete.

.EXAMPLE
    .\scripts\reprocess-all.ps1 -SkipQueue -PollSeconds 60
    Monitor an already-running reprocess (messages already in queue).
#>

[CmdletBinding()]
param(
    [switch]$DryRun,
    [int]$Limit = 0,
    [switch]$SkipQueue,
    [int]$PollSeconds = 30
)

$ErrorActionPreference = "Stop"
$repoRoot = Join-Path $PSScriptRoot ".."

function Write-Ok   { param([string]$M) Write-Host $M -ForegroundColor Green }
function Write-Err  { param([string]$M) Write-Host $M -ForegroundColor Red }
function Write-Info { param([string]$M) Write-Host $M -ForegroundColor Cyan }
function Write-Warn { param([string]$M) Write-Host $M -ForegroundColor Yellow }

# ── 1. Load environment ─────────────────────────────────────
& "$PSScriptRoot\load-env.ps1" | Out-Null

$sqlConn   = $env:AzureSql__ConnectionString
$blobConn  = $env:AzureBlob__ConnectionString
$prefix    = if ($env:Pipeline__QueuePrefix) { $env:Pipeline__QueuePrefix } else { "pipeline" }

if (-not $sqlConn -or $sqlConn -match "PLACEHOLDER") {
    Write-Err "AzureSql__ConnectionString not configured in .env"
    exit 1
}
if (-not $blobConn) {
    Write-Err "AzureBlob__ConnectionString not configured in .env"
    exit 1
}

# ── 2. Start keep-awake ─────────────────────────────────────
$existingJob = Get-Job -Name 'KeepAwake' -ErrorAction SilentlyContinue | Where-Object { $_.State -eq 'Running' }
if ($existingJob) {
    Write-Info "Keep-awake already running (JobId=$($existingJob.Id))."
} else {
    $awakeJob = Start-Job -Name 'KeepAwake' -FilePath "$PSScriptRoot\keep-awake.ps1"
    Write-Ok "Keep-awake started (JobId=$($awakeJob.Id)). Screen will not hibernate."
}

# ── 3. Queue messages (one-time) ────────────────────────────
if (-not $SkipQueue) {
    Write-Host ""
    Write-Info "=== Queueing ParserMessages for all rulings ==="
    Write-Info "Stage: parser | UseCache: true | VPN retries: 10"
    Write-Host ""

    $bulkArgs = @("run", "--project", "$repoRoot\backend\src\tools\LegalAiAr.BulkRequeue", "--", "--stage", "parser", "--use-cache", "--max-retries", "10")
    if ($Limit -gt 0) { $bulkArgs += "--limit"; $bulkArgs += $Limit.ToString() }
    if ($DryRun) { $bulkArgs += "--dry-run" }

    & dotnet @bulkArgs
    $queueExitCode = $LASTEXITCODE

    if ($queueExitCode -ne 0) {
        Write-Err "BulkRequeue exited with code $queueExitCode. Some messages may have failed."
        Write-Warn "You can re-run with -SkipQueue to only monitor, or re-run without to retry queueing."
    } else {
        Write-Ok "All messages queued successfully."
    }

    if ($DryRun) {
        Write-Info "Dry run complete. No messages were published."
        Stop-Job -Name 'KeepAwake' -ErrorAction SilentlyContinue
        Remove-Job -Name 'KeepAwake' -ErrorAction SilentlyContinue
        exit 0
    }
}

# ── 4. Monitor progress ─────────────────────────────────────
Write-Host ""
Write-Info "=== Monitoring pipeline progress (poll every ${PollSeconds}s) ==="
Write-Info "Workers must be running (parser, enrichment, indexer)."
Write-Info "Press Ctrl+C to stop monitoring (workers continue processing)."
Write-Host ""

$queueNames = @("$prefix-parser", "$prefix-enrichment", "$prefix-indexer")
$dlqNames   = @("$prefix-parser-dlq", "$prefix-enrichment-dlq", "$prefix-indexer-dlq")

function Get-QueueCount {
    param([string]$ConnStr, [string]$QueueName)
    try {
        $result = az storage queue metadata show --name $QueueName --connection-string $ConnStr --query "approximateMessageCount" -o tsv 2>$null
        if ($result) { return [int]$result } else { return -1 }
    } catch {
        return -1
    }
}

$startTime = Get-Date
$lastVpnWarning = $null
$stableZeroCount = 0

while ($true) {
    $now = Get-Date
    $elapsed = $now - $startTime

    # Test connectivity
    $connected = $true
    try {
        $testConn = New-Object Microsoft.Data.SqlClient.SqlConnection($sqlConn)
        $testConn.Open()
        $testConn.Close()
    } catch {
        $connected = $false
    }

    if (-not $connected) {
        if (-not $lastVpnWarning -or ($now - $lastVpnWarning).TotalSeconds -gt 60) {
            Write-Warn "[$(Get-Date -Format 'HH:mm:ss')] VPN/network down. Workers paused. Waiting for reconnection..."
            $lastVpnWarning = $now
        }
        Start-Sleep -Seconds 10
        continue
    }

    if ($lastVpnWarning) {
        Write-Ok "[$(Get-Date -Format 'HH:mm:ss')] Network restored. Workers resuming."
        $lastVpnWarning = $null
    }

    # Query queue depths
    $queueDepths = @{}
    $totalPending = 0
    foreach ($q in $queueNames) {
        $count = Get-QueueCount -ConnStr $blobConn -QueueName $q
        $queueDepths[$q] = $count
        if ($count -gt 0) { $totalPending += $count }
    }

    $dlqDepths = @{}
    $totalDlq = 0
    foreach ($q in $dlqNames) {
        $count = Get-QueueCount -ConnStr $blobConn -QueueName $q
        $dlqDepths[$q] = $count
        if ($count -gt 0) { $totalDlq += $count }
    }

    # Query ruling count from DB
    $rulingCount = 0
    try {
        $conn = New-Object Microsoft.Data.SqlClient.SqlConnection($sqlConn)
        $conn.Open()
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = "SELECT COUNT(*) FROM Rulings WHERE SourceId = 1"
        $rulingCount = $cmd.ExecuteScalar()
        $conn.Close()
    } catch { }

    # Display status
    $ts = $elapsed.ToString("hh\:mm\:ss")
    $parserQ   = $queueDepths["$prefix-parser"]
    $enrichQ   = $queueDepths["$prefix-enrichment"]
    $indexerQ  = $queueDepths["$prefix-indexer"]

    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] elapsed=$ts | parser=$parserQ enrichment=$enrichQ indexer=$indexerQ | dlq=$totalDlq | rulings=$rulingCount"

    if ($totalPending -eq 0 -and $totalDlq -ge 0) {
        $stableZeroCount++
    } else {
        $stableZeroCount = 0
    }

    # All queues empty for 3 consecutive polls = done
    if ($stableZeroCount -ge 3 -and $elapsed.TotalMinutes -gt 2) {
        Write-Host ""
        Write-Ok "=== All queues empty for $($stableZeroCount * $PollSeconds)s. Processing appears complete. ==="
        Write-Host "  Rulings in DB: $rulingCount"
        Write-Host "  DLQ messages:  $totalDlq"
        if ($totalDlq -gt 0) {
            Write-Warn "  There are $totalDlq messages in DLQs. Review and retry if needed."
        }
        break
    }

    Start-Sleep -Seconds $PollSeconds
}

# ── 5. Cleanup ───────────────────────────────────────────────
Write-Host ""
Write-Info "Stopping keep-awake..."
Stop-Job -Name 'KeepAwake' -ErrorAction SilentlyContinue
Remove-Job -Name 'KeepAwake' -ErrorAction SilentlyContinue
Write-Ok "Done. Total elapsed: $((Get-Date) - $startTime)"
