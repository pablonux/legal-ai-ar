<#
.SYNOPSIS
    Verifica conectividad a todos los servicios Azure (F0-2 T-10).

.DESCRIPTION
    Comprueba: Azure SQL, Blob Storage, Storage Queues, AI Search y Azure OpenAI.
    Usa el tool LegalAiAr.VerifyConnectivity para SQL, Blob, Queues y Search.
    Azure OpenAI se verifica con verify-azure-openai.ps1.

.PARAMETER EnvPath
    Ruta al archivo .env.

.PARAMETER SkipOpenAI
    Omitir verificación de Azure OpenAI (más lento).

.EXAMPLE
    .\verify-azure-connectivity.ps1
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$EnvPath,

    [Parameter()]
    [switch]$SkipOpenAI
)

$ErrorActionPreference = "Stop"

function Write-Success { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Fail { param([string]$Message) Write-Host $Message -ForegroundColor Red }
function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Warn { param([string]$Message) Write-Host $Message -ForegroundColor Yellow }

function Get-EnvVars {
    $rootDir = $PWD.Path
    if ($PSScriptRoot) {
        $infraDir = Split-Path $PSScriptRoot -Parent
        $repoRoot = Split-Path $infraDir -Parent
        if (Test-Path (Join-Path $repoRoot ".env")) { $rootDir = $repoRoot }
    }
    $path = if ($EnvPath) { $EnvPath } else { Join-Path $rootDir ".env" }
    $vars = @{}
    if (Test-Path $path) {
        Write-Info "Cargando variables desde: $path"
        Get-Content $path | ForEach-Object {
            $line = $_.Trim()
            if ($line -and -not $line.StartsWith("#")) {
                $idx = $line.IndexOf("=")
                if ($idx -gt 0) {
                    $vars[$line.Substring(0, $idx).Trim()] = $line.Substring($idx + 1).Trim()
                }
            }
        }
    } else {
        Write-Fail "Archivo .env no encontrado en $path"
        exit 1
    }
    return $vars
}

# --- Main ---
Write-Info "=== Verificación de conectividad Azure (F0-2 T-10) ==="
Write-Info ""

$vars = Get-EnvVars
$repoRoot = if ($PSScriptRoot) { Split-Path (Split-Path $PSScriptRoot -Parent) -Parent } else { Split-Path $MyInvocation.MyCommand.Path -Parent }
$backendPath = Join-Path $repoRoot "backend"
$verifyProject = Join-Path $backendPath "src\tools\LegalAiAr.VerifyConnectivity\LegalAiAr.VerifyConnectivity.csproj"

if (-not (Test-Path $verifyProject)) {
    Write-Fail "Proyecto LegalAiAr.VerifyConnectivity no encontrado. Ejecutar 'dotnet build' en backend."
    exit 1
}

# Exportar variables al proceso para el tool .NET
foreach ($key in $vars.Keys) {
    [Environment]::SetEnvironmentVariable($key, $vars[$key], "Process")
}

# Ejecutar tool .NET
$ErrorActionPreference = "SilentlyContinue"
$output = & dotnet run --project $verifyProject 2>&1
$toolExit = $LASTEXITCODE
$ErrorActionPreference = "Stop"

$allOk = $true
foreach ($line in $output) {
    if ($line -match "^\s*$") { continue }
    $parts = $line -split "\|", 3
    if ($parts.Count -ge 3) {
        $service = $parts[0].Trim()
        $status = $parts[1].Trim()
        $msg = $parts[2].Trim()
        $ok = ($status -eq "OK")
        if (-not $ok) { $allOk = $false }
        Write-Info "$service..."
        if ($ok) { Write-Success "   $msg" } else { Write-Fail "   $msg" }
    }
}

# Azure OpenAI (opcional)
if (-not $SkipOpenAI) {
    Write-Info "Azure OpenAI..."
    $openAiScript = Join-Path $PSScriptRoot "verify-azure-openai.ps1"
    if (Test-Path $openAiScript) {
        $ErrorActionPreference = "SilentlyContinue"
        & $openAiScript -EnvPath (Join-Path $repoRoot ".env") 2>$null
        $openAiOk = ($LASTEXITCODE -eq 0)
        $ErrorActionPreference = "Stop"
        if ($openAiOk) { Write-Success "   OK" } else { Write-Fail "   Ver .\verify-azure-openai.ps1 para detalles"; $allOk = $false }
    } else {
        Write-Warn "   Script verify-azure-openai.ps1 no encontrado"
    }
} else {
    Write-Warn "Azure OpenAI (omitido, usar sin -SkipOpenAI para verificar)"
}

Write-Info ""
if ($allOk -and $toolExit -eq 0) {
    Write-Success "=== Todas las verificaciones pasaron ==="
    exit 0
} else {
    Write-Fail "=== Una o más verificaciones fallaron ==="
    exit 1
}
