<#
.SYNOPSIS
    Creates Azure App Service with staging slot for Legal AI AR API (F1-14 T-01).

.DESCRIPTION
    Creates:
    - App Service Plan (B2)
    - Web App (legal-ai-ar-api)
    - Staging deployment slot
    URLs: legal-ai-ar-api.azurewebsites.net (production), legal-ai-ar-api-staging.azurewebsites.net (staging)

.PARAMETER ResourceGroup
    Azure resource group name. Default: legal-ai-ar-rg

.PARAMETER Location
    Azure region. Default: eastus2

.PARAMETER AppName
    Web App name. Must be globally unique. Default: legal-ai-ar-api

.EXAMPLE
    .\create-app-service.ps1
    Creates App Service and staging slot in legal-ai-ar-rg (eastus2).

.EXAMPLE
    .\create-app-service.ps1 -ResourceGroup my-rg -Location westus2
    Creates in custom resource group and region.
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$ResourceGroup = "legal-ai-ar-rg",

    [Parameter()]
    [string]$Location = "eastus2",

    [Parameter()]
    [string]$AppName = "legal-ai-ar-api"
)

$ErrorActionPreference = "Stop"

function Write-Success { param([string]$Message) Write-Host $Message -ForegroundColor Green }
function Write-Fail { param([string]$Message) Write-Host $Message -ForegroundColor Red }
function Write-Info { param([string]$Message) Write-Host $Message -ForegroundColor Cyan }

# Check Azure CLI
$az = Get-Command az -ErrorAction SilentlyContinue
if (-not $az) {
    Write-Fail "Azure CLI required. Install: https://aka.ms/install-azure-cli"
    exit 1
}

Write-Info "=== Create App Service with staging slot (F1-14 T-01) ==="
Write-Info "Resource Group: $ResourceGroup"
Write-Info "Location: $Location"
Write-Info "App Name: $AppName"
Write-Info ""

# Create resource group if not exists
$rgExists = az group exists -n $ResourceGroup 2>$null
if ($rgExists -eq "false") {
    Write-Info "Creating resource group..."
    az group create -n $ResourceGroup -l $Location -o none
    Write-Success "Resource group created."
} else {
    Write-Info "Resource group exists."
}

# Create App Service Plan (B2)
$planName = "$AppName-plan"
Write-Info "Creating App Service Plan ($planName, B2)..."
az appservice plan create `
    -g $ResourceGroup `
    -n $planName `
    -l $Location `
    --sku B2 `
    --is-linux `
    -o none 2>$null
if ($LASTEXITCODE -ne 0) {
    $existing = az appservice plan show -g $ResourceGroup -n $planName -o json 2>$null
    if ($existing) {
        Write-Info "App Service Plan exists."
    } else {
        Write-Fail "Failed to create App Service Plan."
        exit 1
    }
} else {
    Write-Success "App Service Plan created."
}

# Create Web App (.NET 8)
Write-Info "Creating Web App ($AppName)..."
az webapp create `
    -g $ResourceGroup `
    -n $AppName `
    -p $planName `
    -r "DOTNETCORE:8.0" `
    -o none 2>$null
if ($LASTEXITCODE -ne 0) {
    $existing = az webapp show -g $ResourceGroup -n $AppName -o json 2>$null
    if ($existing) {
        Write-Info "Web App exists."
    } else {
        Write-Fail "Failed to create Web App. App name may be taken (must be globally unique)."
        exit 1
    }
} else {
    Write-Success "Web App created."
}

# Configure production slot
Write-Info "Configuring production slot..."
az webapp config appsettings set `
    -g $ResourceGroup `
    -n $AppName `
    --settings ASPNETCORE_ENVIRONMENT=Production `
    -o none
Write-Success "Production slot configured."

# Create staging slot
$stagingSlot = "staging"
Write-Info "Creating staging slot..."
az webapp deployment slot create `
    -g $ResourceGroup `
    -n $AppName `
    -s $stagingSlot `
    -o none 2>$null
if ($LASTEXITCODE -ne 0) {
    $slotExists = az webapp deployment slot list -g $ResourceGroup -n $AppName -o json 2>$null | ConvertFrom-Json
    if ($slotExists | Where-Object { $_.name -eq $stagingSlot }) {
        Write-Info "Staging slot exists."
    } else {
        Write-Fail "Failed to create staging slot."
        exit 1
    }
} else {
    Write-Success "Staging slot created."
}

# Configure staging slot
Write-Info "Configuring staging slot..."
az webapp config appsettings set `
    -g $ResourceGroup `
    -n $AppName `
    -s $stagingSlot `
    --settings ASPNETCORE_ENVIRONMENT=Staging `
    -o none
Write-Success "Staging slot configured."

Write-Info ""
Write-Success "=== App Service ready ==="
Write-Host "  Production: https://$AppName.azurewebsites.net" -ForegroundColor White
Write-Host "  Staging:    https://$AppName-$stagingSlot.azurewebsites.net" -ForegroundColor White
Write-Info ""
Write-Info "Next: Configure Application settings (connection strings, secrets) in Azure Portal or via:"
Write-Host "  az webapp config appsettings set -g $ResourceGroup -n $AppName --settings @settings.json" -ForegroundColor Yellow
Write-Info ""
