<#
.SYNOPSIS
    Creates Azure Container Registry for Legal AI AR workers (F1-14 T-03).

.DESCRIPTION
    Creates ACR for pushing worker container images.
    Name must be globally unique (5-50 alphanumeric).

.PARAMETER ResourceGroup
    Azure resource group name. Default: legal-ai-ar-rg

.PARAMETER Location
    Azure region. Default: eastus2

.PARAMETER AcrName
    ACR name (globally unique). Default: legalaiaracr

.PARAMETER Sku
    Basic, Standard, or Premium. Default: Basic

.EXAMPLE
    .\create-container-registry.ps1
    Creates ACR legalaiaracr in legal-ai-ar-rg.
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$ResourceGroup = "legal-ai-ar-rg",

    [Parameter()]
    [string]$Location = "eastus2",

    [Parameter()]
    [string]$AcrName = "legalaiaracr",

    [Parameter()]
    [ValidateSet("Basic", "Standard", "Premium")]
    [string]$Sku = "Basic"
)

$ErrorActionPreference = "Stop"

function Write-Success { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Fail { param([string]$Message) Write-Host $Message -ForegroundColor Red }
function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }

$az = Get-Command az -ErrorAction SilentlyContinue
if (-not $az) {
    Write-Fail "Azure CLI required. Install: https://aka.ms/install-azure-cli"
    exit 1
}

Write-Info "=== Create Container Registry (F1-14 T-03) ==="
Write-Info "Resource Group: $ResourceGroup"
Write-Info "ACR Name: $AcrName"
Write-Info ""

$rgExists = az group exists -n $ResourceGroup 2>$null
if ($rgExists -eq "false") {
    Write-Info "Creating resource group..."
    az group create -n $ResourceGroup -l $Location -o none
    Write-Success "Resource group created."
}

Write-Info "Creating Container Registry..."
az acr create `
    -g $ResourceGroup `
    -n $AcrName `
    -l $Location `
    --sku $Sku `
    --admin-enabled true `
    -o none 2>$null

if ($LASTEXITCODE -ne 0) {
    $existing = az acr show -g $ResourceGroup -n $AcrName -o json 2>$null
    if ($existing) {
        Write-Info "Container Registry exists."
    } else {
        Write-Fail "Failed to create ACR. Name may be taken (must be globally unique)."
        exit 1
    }
} else {
    Write-Success "Container Registry created."
}

$loginServer = az acr show -g $ResourceGroup -n $AcrName --query "loginServer" -o tsv 2>$null
Write-Info ""
Write-Success "=== ACR ready ==="
Write-Host "  Login server: $loginServer" -ForegroundColor White
Write-Info "  Login: az acr login -n $AcrName"
Write-Info ""
