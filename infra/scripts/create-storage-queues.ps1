<#
.SYNOPSIS
    Crea las colas de Azure Storage Queues para el pipeline de ingesta (F0-2 T-07).

.DESCRIPTION
    Crea las 4 colas principales y las 4 colas DLQ en el Storage Account compartido con Blob.
    Usa AzureBlob__ConnectionString. Las colas existentes se omiten (idempotente).

.PARAMETER EnvPath
    Ruta al archivo .env. Por defecto busca .env en el directorio raíz del repositorio.

.PARAMETER SkipDlq
    Si se especifica, no crea las colas DLQ (solo las 4 principales).

.EXAMPLE
    .\create-storage-queues.ps1
    Crea las 8 colas (4 principales + 4 DLQ).

.EXAMPLE
    .\create-storage-queues.ps1 -SkipDlq
    Crea solo las 4 colas principales.
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$EnvPath,

    [Parameter()]
    [switch]$SkipDlq
)

$ErrorActionPreference = "Stop"

function Write-Success { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Fail { param([string]$Message) Write-Host $Message -ForegroundColor Red }
function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Warn { param([string]$Message) Write-Host $Message -ForegroundColor Yellow }

# Cargar variables desde .env
function Get-EnvVars {
    $rootDir = $PWD.Path
    if ($PSScriptRoot) {
        $infraDir = Split-Path $PSScriptRoot -Parent
        $repoRoot = Split-Path $infraDir -Parent
        if (Test-Path (Join-Path $repoRoot ".env")) {
            $rootDir = $repoRoot
        }
    }
    $defaultEnvPath = Join-Path $rootDir ".env"
    $path = if ($EnvPath) { $EnvPath } else { $defaultEnvPath }

    $vars = @{}
    if (Test-Path $path) {
        Write-Info "Cargando variables desde: $path"
        Get-Content $path | ForEach-Object {
            $line = $_.Trim()
            if ($line -and -not $line.StartsWith("#")) {
                $idx = $line.IndexOf("=")
                if ($idx -gt 0) {
                    $key = $line.Substring(0, $idx).Trim()
                    $value = $line.Substring($idx + 1).Trim()
                    $vars[$key] = $value
                }
            }
        }
    } else {
        Write-Info "Archivo .env no encontrado en $path. Usando variables de entorno."
        $key = "AzureBlob__ConnectionString"
        $val = [Environment]::GetEnvironmentVariable($key, "Process")
        if (-not $val) { $val = [Environment]::GetEnvironmentVariable($key, "User") }
        if (-not $val) { $val = [Environment]::GetEnvironmentVariable($key, "Machine") }
        if ($val) { $vars[$key] = $val }
    }

    return $vars
}

function Get-RequiredVar {
    param($vars, [string]$key, [string]$desc)
    $val = $vars[$key]
    if (-not $val -or $val -match "^(DB_SECRET|STORAGE_KEY|SEARCH_KEY|OPENAI_KEY)$") {
        Write-Fail "Falta o es placeholder: $key ($desc). Configurar en .env o variables de entorno."
        exit 1
    }
    return $val
}

function Ensure-Queue-AzStorage {
    param($ctx, [string]$name)
    $existing = Get-AzStorageQueue -Name $name -Context $ctx -ErrorAction SilentlyContinue
    if ($existing) {
        Write-Info "  $name (ya existe)"
        return $false
    }
    New-AzStorageQueue -Name $name -Context $ctx | Out-Null
    Write-Success "  $name (creada)"
    return $true
}

function Ensure-Queue-AzCli {
    param([string]$conn, [string]$name)
    $ErrorActionPreference = "SilentlyContinue"
    $existsOutput = az storage queue exists -n $name --connection-string $conn -o tsv 2>$null
    $ErrorActionPreference = "Stop"
    if ($existsOutput -eq "True") {
        Write-Info "  $name (ya existe)"
        return $false
    }
    $ErrorActionPreference = "SilentlyContinue"
    az storage queue create -n $name --connection-string $conn --only-show-errors 2>$null | Out-Null
    $err = $LASTEXITCODE
    $ErrorActionPreference = "Stop"
    if ($err -eq 0) {
        Write-Success "  $name (creada)"
        return $true
    }
    Write-Fail "  $name (error al crear)"
    return $false
}

# --- Main ---
Write-Info "=== Crear colas Storage Queues (F0-2 T-07) ==="
Write-Info ""

$vars = Get-EnvVars
$connectionString = Get-RequiredVar $vars "AzureBlob__ConnectionString" "Connection string del Storage Account (Blob + Queues)"

$useAzStorage = $false
$useAzCli = $false

if (Get-Module -ListAvailable -Name Az.Storage) {
    $useAzStorage = $true
    Import-Module Az.Storage -ErrorAction SilentlyContinue
    if (-not (Get-Module Az.Storage)) { $useAzStorage = $false }
}

if (-not $useAzStorage) {
    $azPath = Get-Command az -ErrorAction SilentlyContinue
    if ($azPath) {
        $useAzCli = $true
        Write-Info "Usando Azure CLI (Az.Storage no instalado)."
    }
}

if (-not $useAzStorage -and -not $useAzCli) {
    Write-Fail "Se requiere Az.Storage o Azure CLI. Opciones:"
    Write-Host "  1. Install-Module -Name Az.Storage -Scope CurrentUser -Force" -ForegroundColor Yellow
    Write-Host "  2. Instalar Azure CLI: https://aka.ms/install-azure-cli" -ForegroundColor Yellow
    exit 1
}

if ($useAzStorage) {
    $ctx = New-AzStorageContext -ConnectionString $connectionString -ErrorAction Stop
    Write-Info "Storage Account conectado (Az.Storage)."
} else {
    Write-Info "Storage Account: connection string cargada."
}
Write-Info ""

$prefix = if ($env:Pipeline__QueuePrefix) { $env:Pipeline__QueuePrefix } else { "pipeline" }
$mainQueues = @("$prefix-crawler", "$prefix-parser", "$prefix-enrichment", "$prefix-indexer")
$dlqQueues = @("$prefix-crawler-dlq", "$prefix-parser-dlq", "$prefix-enrichment-dlq", "$prefix-indexer-dlq")

$created = 0

Write-Info "Colas principales:"
foreach ($name in $mainQueues) {
    if ($useAzStorage) {
        if (Ensure-Queue-AzStorage -ctx $ctx -name $name) { $created++ }
    } else {
        if (Ensure-Queue-AzCli -conn $connectionString -name $name) { $created++ }
    }
}

if (-not $SkipDlq) {
    Write-Info ""
    Write-Info "Colas DLQ (mensajes fallidos):"
    foreach ($name in $dlqQueues) {
        if ($useAzStorage) {
            if (Ensure-Queue-AzStorage -ctx $ctx -name $name) { $created++ }
        } else {
            if (Ensure-Queue-AzCli -conn $connectionString -name $name) { $created++ }
        }
    }
} else {
    Write-Warn ""
    Write-Warn "Omitiendo colas DLQ (usar sin -SkipDlq para crearlas)."
}

Write-Info ""
Write-Success "=== Completado: $created cola(s) creada(s) ==="
exit 0
