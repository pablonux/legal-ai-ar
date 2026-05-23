<#
.SYNOPSIS
    Re-queues documents that failed at the Persister stage.
.DESCRIPTION
    1. Queries Documents with CurrentStage='Persister' AND Status='Failed'
    2. Resets their status to 'Pending'
    3. Publishes PersisterMessages to the pipeline-persister queue via az CLI
#>
[CmdletBinding()]
param(
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"
& "$PSScriptRoot\load-env.ps1" | Out-Null

$sqlConn   = $env:AzureSql__ConnectionString
$blobConn  = $env:AzureBlob__ConnectionString
$prefix    = if ($env:Pipeline__QueuePrefix) { $env:Pipeline__QueuePrefix } else { "pipeline" }
$queueName = "$prefix-persister"

$parts = @{}
$sqlConn -split ';' | ForEach-Object {
    $kv = $_ -split '=',2
    if ($kv.Length -eq 2) { $parts[$kv[0].Trim()] = $kv[1].Trim() }
}
$server = $parts['Server'] -replace 'tcp:',''
$db     = $parts['Database']
$user   = $parts['User ID']
$pwd    = $parts['Password']

Write-Host "Requeue Failed Persister Documents"
Write-Host "  Queue:   $queueName"
Write-Host "  DryRun:  $DryRun"
Write-Host ""

# Step 1: Get failed document IDs
Write-Host "Loading failed persister documents..."
$rawOutput = sqlcmd -S $server -d $db -U $user -P $pwd -h-1 -s"~" -Q "SELECT CAST(d.Id AS VARCHAR(36)), d.EntityType, d.SourceId, ISNULL(d.ContentHash,''), ISNULL(CAST(d.IngestionJobId AS VARCHAR(36)),''), ISNULL(d.BlobPath,'') FROM Documents d WHERE d.CurrentStage = 'Persister' AND d.Status IN ('Failed','Pending')" 2>&1

$lines = $rawOutput | Where-Object { $_ -match '~' -and $_ -notmatch 'rows affected' }
Write-Host "  Found $($lines.Count) failed documents"

if ($lines.Count -eq 0) {
    Write-Host "Nothing to requeue."
    exit 0
}

# Step 2: Reset status to Pending
if (-not $DryRun) {
    Write-Host "Resetting status to Pending..."
    sqlcmd -S $server -d $db -U $user -P $pwd -Q "UPDATE Documents SET Status = 'Pending', ErrorMessage = NULL, ErrorType = NULL, RetryCount = 0 WHERE CurrentStage = 'Persister' AND Status IN ('Failed','Pending')" 2>&1 | Write-Host
    Write-Host "  Done."
}

# Step 3: Publish PersisterMessages
Write-Host "Publishing PersisterMessages to $queueName..."
$published = 0
$errors = 0

foreach ($line in $lines) {
    $cols = $line -split '~'
    if ($cols.Length -lt 4) { continue }

    $docId          = $cols[0].Trim()
    $entityType     = $cols[1].Trim()
    $sourceId       = $cols[2].Trim()
    $contentHash    = $cols[3].Trim()
    $ingestionJobId = $cols[4].Trim()
    $payloadBlob    = if ($cols.Length -ge 6) { $cols[5].Trim() } else { "" }

    $entityTypeInt = switch ($entityType) {
        "Ruling"  { 0 }
        "Statute" { 1 }
        default   { 0 }
    }

    $msgObj = [ordered]@{
        documentId  = $docId
        entityType  = [int]$entityTypeInt
        sourceId    = [int]$sourceId
        contentHash = $contentHash
        reprocess   = $true
    }
    if ($ingestionJobId -and $ingestionJobId.Length -gt 0) {
        $msgObj.ingestionJobId = $ingestionJobId
    }
    if ($payloadBlob -and $payloadBlob.Length -gt 0) {
        $msgObj.payloadBlobPath = $payloadBlob
    }

    $json = $msgObj | ConvertTo-Json -Compress

    if ($DryRun) {
        Write-Host "  [DRY] DocId=$docId EntityType=$entityType SourceId=$sourceId"
        $published++
    } else {
        $prevEA = $ErrorActionPreference
        $ErrorActionPreference = "Continue"
        $azOutput = az storage message put --queue-name $queueName --content $json --connection-string $blobConn -o none 2>&1
        $azExit = $LASTEXITCODE
        $ErrorActionPreference = $prevEA
        if ($azExit -eq 0) {
            $published++
            if ($published % 10 -eq 0) { Write-Host "  Published $published..." }
        } else {
            Write-Host "  ERROR DocId=$docId exit=$azExit : $azOutput"
            $errors++
        }
    }
}

Write-Host ""
Write-Host "Done. Published=$published Errors=$errors (DryRun=$DryRun)"
