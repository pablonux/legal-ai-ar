# Load .env and run LegalAiAr.SetupBlob (creates Blob container)
# Usage: .\scripts\run-setup-blob.ps1

& "$PSScriptRoot\load-env.ps1"
Push-Location (Join-Path $PSScriptRoot ".." "backend")
try {
    dotnet run --project src/tools/LegalAiAr.SetupBlob/LegalAiAr.SetupBlob.csproj
} finally {
    Pop-Location
}
