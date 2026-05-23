# Load .env and run LegalAiAr.SetupSearch (creates AI Search indexes)
# Usage: .\scripts\run-setup-search.ps1

& "$PSScriptRoot\load-env.ps1"
Push-Location (Join-Path $PSScriptRoot ".." "backend")
try {
    dotnet run --project src/tools/LegalAiAr.SetupSearch/LegalAiAr.SetupSearch.csproj
} finally {
    Pop-Location
}
