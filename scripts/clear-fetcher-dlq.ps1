<#
.SYNOPSIS
    Elimina todos los mensajes de la cola DLQ del Fetcher (Storage Queue).

.DESCRIPTION
    Usa AzureBlob__ConnectionString y Pipeline__QueuePrefix desde .env (load-env.ps1),
    igual que enqueue-fetcher-pending-for-job.ps1.
    Cola por defecto: {Pipeline__QueuePrefix}-fetcher-dlq (ej. pipeline-fetcher-dlq).

    Ejecutá esto antes de reencolar documentos si los intentos fallidos quedaron en la DLQ.

.PARAMETER QueueName
    Nombre completo de la cola. Si se omite, se usa {prefix}-fetcher-dlq.

.PARAMETER DryRun
    Solo muestra servidor/cola; no llama a az clear.

.EXAMPLE
    .\scripts\clear-fetcher-dlq.ps1

.EXAMPLE
    .\scripts\clear-fetcher-dlq.ps1 -DryRun
#>
[CmdletBinding()]
param(
    [string]$QueueName,

    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'load-env.ps1') | Out-Null

if (-not $env:AzureBlob__ConnectionString) {
    Write-Error 'AzureBlob__ConnectionString missing after load-env.'
}

$prefix = if ($env:Pipeline__QueuePrefix) { $env:Pipeline__QueuePrefix } else { 'pipeline' }
$q = if ($QueueName) { $QueueName.Trim() } else { "$prefix-fetcher-dlq" }

Write-Host 'Clear Fetcher DLQ (Azure Storage Queue)'
Write-Host "  Queue:     $q"
Write-Host "  DryRun:    $($DryRun.IsPresent)"
Write-Host ''

if ($DryRun) {
    Write-Host 'DryRun: no se eliminó nada. Quitá -DryRun para ejecutar az storage message clear.'
    exit 0
}

$prevEA = $ErrorActionPreference
$ErrorActionPreference = 'Continue'
$out = az storage message clear --queue-name $q --connection-string $env:AzureBlob__ConnectionString -o none 2>&1
$exit = $LASTEXITCODE
$ErrorActionPreference = $prevEA

if ($exit -ne 0) {
    Write-Error "az storage message clear falló (exit $exit): $out"
}

Write-Host 'Listo: la cola quedó vacía.'
Write-Host 'Podés ejecutar enqueue-fetcher-pending-for-job.ps1 para volver a encolar Pending @ Fetcher.'
