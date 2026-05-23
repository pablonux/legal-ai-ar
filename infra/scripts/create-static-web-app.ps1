<#
.SYNOPSIS
    Creates Azure Static Web App for Legal AI AR SPA (F1-14 T-02).

.DESCRIPTION
    Creates a Static Web App for the Angular SPA.
    Staging: PR/branch deploys. Production: deploy from main.

.PARAMETER ResourceGroup
    Azure resource group name. Default: legal-ai-ar-rg

.PARAMETER Location
    Azure region. Default: eastus2

.PARAMETER AppName
    Static Web App name. Default: legal-ai-ar

.PARAMETER Sku
    Free or Standard. Default: Free

.EXAMPLE
    .\create-static-web-app.ps1
    Creates Static Web App in legal-ai-ar-rg.
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$ResourceGroup = "legal-ai-ar-rg",

    [Parameter()]
    [string]$Location = "eastus2",

    [Parameter()]
    [string]$AppName = "legal-ai-ar",

    [Parameter()]
    [ValidateSet("Free", "Standard")]
    [string]$Sku = "Free"
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

Write-Info "=== Create Static Web App (F1-14 T-02) ==="
Write-Info "Resource Group: $ResourceGroup"
Write-Info "App Name: $AppName"
Write-Info ""

$rgExists = az group exists -n $ResourceGroup 2>$null
if ($rgExists -eq "false") {
    Write-Info "Creating resource group..."
    az group create -n $ResourceGroup -l $Location -o none
    Write-Success "Resource group created."
}

Write-Info "Creating Static Web App..."
az staticwebapp create `
    -g $ResourceGroup `
    -n $AppName `
    -l $Location `
    --sku $Sku `
    -o json 2>$null | Out-Null

if ($LASTEXITCODE -ne 0) {
    $existing = az staticwebapp show -g $ResourceGroup -n $AppName -o json 2>$null
    if ($existing) {
        Write-Info "Static Web App exists."
    } else {
        Write-Fail "Failed to create Static Web App."
        exit 1
    }
} else {
    Write-Success "Static Web App created."
}

$deployToken = az staticwebapp secrets list -g $ResourceGroup -n $AppName --query "properties.apiKey" -o tsv 2>$null
if ($deployToken) {
    Write-Info ""
    Write-Success "=== Static Web App ready ==="
    Write-Host "  Deploy token (for GitHub Actions): $deployToken" -ForegroundColor Yellow
    Write-Info "  Add as secret AZURE_STATIC_WEB_APPS_API_TOKEN in GitHub repo."
    Write-Info ""
}
