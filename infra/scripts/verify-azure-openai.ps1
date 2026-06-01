<#
.SYNOPSIS
    Verifica que los deployments de Azure OpenAI (gpt-4o y text-embedding-3-large) existan y respondan correctamente.

.DESCRIPTION
    Script de verificación para F0-2 T-06. Lee la configuración desde .env o variables de entorno,
    realiza una llamada mínima a cada deployment y reporta éxito o error con mensajes descriptivos.

.PARAMETER EnvPath
    Ruta al archivo .env. Por defecto busca .env en el directorio raíz del repositorio.

.EXAMPLE
    .\verify-azure-openai.ps1
    Ejecuta desde la raíz del repo, carga .env automáticamente.

.EXAMPLE
    .\verify-azure-openai.ps1 -EnvPath "C:\proyecto\.env"
    Especifica ruta explícita al .env.
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$EnvPath
)

$ErrorActionPreference = "Stop"

# Colores para output
function Write-Success { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Fail { param([string]$Message) Write-Host $Message -ForegroundColor Red }
function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }

# Cargar variables desde .env
function Get-EnvVars {
    $rootDir = $PWD.Path
    if ($PSScriptRoot) {
        $scriptDir = $PSScriptRoot
        $infraDir = Split-Path $scriptDir -Parent
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
        foreach ($key in @(
            "AzureOpenAI__Endpoint", "AzureOpenAI__ApiKey",
            "AzureOpenAI__ChatDeploymentName", "AzureOpenAI__EmbeddingDeploymentName"
        )) {
            $val = [Environment]::GetEnvironmentVariable($key, "Process")
            if (-not $val) { $val = [Environment]::GetEnvironmentVariable($key, "User") }
            if (-not $val) { $val = [Environment]::GetEnvironmentVariable($key, "Machine") }
            if ($val) { $vars[$key] = $val }
        }
    }

    return $vars
}

# Obtener valor requerido
function Get-RequiredVar {
    param($vars, [string]$key, [string]$desc)
    $val = $vars[$key]
    if (-not $val -or $val -match "^(DB_SECRET|STORAGE_KEY|SEARCH_KEY|OPENAI_KEY)$") {
        Write-Fail "Falta o es placeholder: $key ($desc). Configurar en .env o variables de entorno."
        exit 1
    }
    return $val
}

# Normalizar endpoint (eliminar trailing slash)
function Get-NormalizedEndpoint {
    param([string]$endpoint)
    return $endpoint.TrimEnd("/")
}

# Verificar deployment de chat (gpt-4o)
function Test-ChatDeployment {
    param($endpoint, $apiKey, $deploymentName)
    $url = "$endpoint/openai/deployments/$deploymentName/chat/completions?api-version=2024-06-01"
    $body = @{
        messages = @(
            @{ role = "user"; content = "Responda solo: OK" }
        )
        max_tokens = 5
    } | ConvertTo-Json -Depth 4

    $headers = @{
        "api-key" = $apiKey
        "Content-Type" = "application/json"
    }

    try {
        $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body -TimeoutSec 30
        if ($response.choices -and $response.choices.Count -gt 0) {
            return $true
        }
        Write-Fail "Chat: respuesta sin choices."
        return $false
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        $errorBody = ""
        if ($_.ErrorDetails.Message) { $errorBody = $_.ErrorDetails.Message }
        Write-Fail "Chat deployment '$deploymentName' falló: $($_.Exception.Message)"
        if ($statusCode) { Write-Fail "  HTTP $statusCode" }
        if ($errorBody) { Write-Fail "  $errorBody" }
        return $false
    }
}

# Verificar deployment de embeddings (text-embedding-3-large)
function Test-EmbeddingDeployment {
    param($endpoint, $apiKey, $deploymentName)
    $url = "$endpoint/openai/deployments/$deploymentName/embeddings?api-version=2024-06-01"
    $body = @{ input = "test" } | ConvertTo-Json

    $headers = @{
        "api-key" = $apiKey
        "Content-Type" = "application/json"
    }

    try {
        $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body -TimeoutSec 30
        if ($response.data -and $response.data.Count -gt 0 -and $response.data[0].embedding) {
            return $true
        }
        Write-Fail "Embeddings: respuesta sin datos de embedding."
        return $false
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        $errorBody = ""
        if ($_.ErrorDetails.Message) { $errorBody = $_.ErrorDetails.Message }
        Write-Fail "Embedding deployment '$deploymentName' falló: $($_.Exception.Message)"
        if ($statusCode) { Write-Fail "  HTTP $statusCode" }
        if ($errorBody) { Write-Fail "  $errorBody" }
        return $false
    }
}

# --- Main ---
Write-Info "=== Verificación de Azure OpenAI deployments (F0-2 T-06) ==="
Write-Info ""

$vars = Get-EnvVars
$endpoint = Get-NormalizedEndpoint (Get-RequiredVar $vars "AzureOpenAI__Endpoint" "URL del recurso Azure OpenAI")
$apiKey = Get-RequiredVar $vars "AzureOpenAI__ApiKey" "API key"
$chatDeployment = Get-RequiredVar $vars "AzureOpenAI__ChatDeploymentName" "Deployment de gpt-4o"
$embeddingDeployment = Get-RequiredVar $vars "AzureOpenAI__EmbeddingDeploymentName" "Deployment de text-embedding-3-large"

Write-Info "Endpoint: $endpoint"
Write-Info "Chat deployment: $chatDeployment"
Write-Info "Embedding deployment: $embeddingDeployment"
Write-Info ""

$chatOk = Test-ChatDeployment -endpoint $endpoint -apiKey $apiKey -deploymentName $chatDeployment
$embedOk = Test-EmbeddingDeployment -endpoint $endpoint -apiKey $apiKey -deploymentName $embeddingDeployment

Write-Info ""
if ($chatOk) { Write-Success "Chat (gpt-4o): OK" } else { Write-Fail "Chat (gpt-4o): FALLO" }
if ($embedOk) { Write-Success "Embeddings (text-embedding-3-large): OK" } else { Write-Fail "Embeddings (text-embedding-3-large): FALLO" }

if ($chatOk -and $embedOk) {
    Write-Info ""
    Write-Success "=== Verificación completada: todos los deployments están operativos ==="
    exit 0
} else {
    Write-Info ""
    Write-Fail "=== Verificación fallida: revisar configuración y crear deployments en Azure OpenAI Studio si no existen ==="
    exit 1
}
