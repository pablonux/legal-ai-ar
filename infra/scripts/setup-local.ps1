<#
.SYNOPSIS
    Automated local setup for Legal Ai Ar (Windows / PowerShell).

.DESCRIPTION
    Verifies prerequisites, prepares .env, restores/builds backend and frontend.
    Uses the shared cloud DEV Azure services — no local SQL Server or Azurite.
    See docs/onboarding/README.md for the full guide.

.EXAMPLE
    .\infra\scripts\setup-local.ps1
    .\infra\scripts\setup-local.ps1 -VerifyConnectivity
#>

[CmdletBinding()]
param(
    [switch]$VerifyConnectivity,
    [switch]$SkipFrontend
)

$ErrorActionPreference = "Stop"

function Write-Step([string]$Message) { Write-Host "`n=== $Message ===" -ForegroundColor Cyan }
function Write-Ok([string]$Message) { Write-Host "  OK  $Message" -ForegroundColor Green }
function Write-Warn([string]$Message) { Write-Host "  WARN  $Message" -ForegroundColor Yellow }
function Write-Fail([string]$Message) { Write-Host "  FAIL  $Message" -ForegroundColor Red }

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "../..")
Set-Location $RepoRoot

Write-Step "Legal Ai Ar - Local Setup"
Write-Host "Repo root: $RepoRoot"

Write-Step "1. Prerequisites"

function Test-CommandVersion {
    param(
        [string]$Name,
        [scriptblock]$GetVersion,
        [string]$MinVersion = $null,
        [bool]$Required = $true
    )

    try {
        $raw = & $GetVersion 2>$null | Select-Object -First 1
        if (-not $raw) { throw "no output" }
        Write-Ok "$Name : $raw"
        if ($MinVersion) {
            $digits = [regex]::Match($raw, '\d+\.\d+').Value
            if ($digits -and ([version]$digits -lt [version]$MinVersion)) {
                throw "version $digits < required $MinVersion"
            }
        }
        return $true
    }
    catch {
        if ($Required) {
            Write-Fail "$Name : NOT INSTALLED or below minimum ($MinVersion)"
            exit 1
        }
        Write-Warn "$Name : not found (optional)"
        return $false
    }
}

Test-CommandVersion -Name "Git" -GetVersion { git --version } | Out-Null
Test-CommandVersion -Name ".NET SDK" -GetVersion { dotnet --version } -MinVersion "10.0" | Out-Null
Test-CommandVersion -Name "Node.js" -GetVersion { node --version } -MinVersion "22.0" | Out-Null
Test-CommandVersion -Name "Docker" -GetVersion { docker --version } | Out-Null

try {
    $azVer = az --version 2>$null | Select-Object -First 1
    if ($azVer) { Write-Ok "Azure CLI : $azVer" }
    else { Write-Warn "Azure CLI : not found (optional)" }
}
catch {
    Write-Warn "Azure CLI : not found (optional)"
}

Write-Step "2. Environment file (.env)"

$EnvExample = Join-Path $RepoRoot ".env.example"
$EnvFile = Join-Path $RepoRoot ".env"

if (-not (Test-Path $EnvFile)) {
    Copy-Item $EnvExample $EnvFile
    Write-Ok "Created .env from .env.example"
    Write-Warn "Fill DB_SECRET, STORAGE_KEY, SEARCH_KEY, OPENAI_KEY - ask Tech Lead for DEV values."
}
else {
    Write-Ok ".env already exists (not overwritten)"
}

if ((Get-Content $EnvFile -Raw) -match 'DB_SECRET|STORAGE_KEY|SEARCH_KEY|OPENAI_KEY') {
    Write-Warn ".env still contains placeholder secrets - cloud features will fail until replaced."
}

Write-Step "3. Backend (.NET)"

Push-Location (Join-Path $RepoRoot "backend")
try {
    dotnet restore LegalAiAr.sln
    if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed" }
    dotnet build LegalAiAr.sln -c Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }
    Write-Ok "Backend restore + build succeeded"
}
finally {
    Pop-Location
}

Write-Step "4. Frontend (Angular)"

if (-not $SkipFrontend) {
    Push-Location (Join-Path $RepoRoot "frontend")
    try {
        npm ci
        if ($LASTEXITCODE -ne 0) {
            Write-Warn "npm ci failed - if AppKit packages fail, complete JFrog login: docs/onboarding/appkit-npm-access.md"
            exit 1
        }
        Write-Ok "Frontend dependencies installed"
    }
    finally {
        Pop-Location
    }
}
else {
    Write-Warn "Skipped frontend ( -SkipFrontend )"
}

if ($VerifyConnectivity) {
    Write-Step "5. Azure connectivity (optional)"
    $VerifyScript = Join-Path $RepoRoot "infra/scripts/verify-azure-connectivity.ps1"
    if (Test-Path $VerifyScript) {
        & $VerifyScript
    }
    else {
        Write-Warn "verify-azure-connectivity.ps1 not found - skipped"
    }
}

Write-Step "Setup complete"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Fill secrets in .env (if not done) - see docs/onboarding/README.md section 3"
Write-Host "  2. Backend:  cd backend; dotnet run --project src/api/LegalAiAr.Api"
Write-Host "  3. Frontend: cd frontend; npm start"
Write-Host "  4. Verify:   Swagger http://localhost:5088/swagger  |  SPA http://localhost:4200"
Write-Host "  5. Optional: docker compose -f docker-compose.app.yml up -d"
Write-Host ""
Write-Host "Connectivity: .\infra\scripts\setup-local.ps1 -VerifyConnectivity"
Write-Host "Help:         docs/onboarding/troubleshooting.md"
