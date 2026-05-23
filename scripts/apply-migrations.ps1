# Apply EF Core migrations to the database.
# Requires: .env with AzureSql__ConnectionString (or ConnectionStrings__DefaultConnection)
#
# IMPORTANT: Stop the API and workers before running (they lock DLLs and prevent build).
#
# Usage:
#   .\scripts\apply-migrations.ps1

$ErrorActionPreference = "Stop"
$repoRoot = Join-Path $PSScriptRoot ".."

& "$PSScriptRoot\load-env.ps1" dotnet ef database update `
  --project "$repoRoot\backend\src\shared\LegalAiAr.Infrastructure\LegalAiAr.Infrastructure.csproj" `
  --startup-project "$repoRoot\backend\src\api\LegalAiAr.Api\LegalAiAr.Api.csproj"
