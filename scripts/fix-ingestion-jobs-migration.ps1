# Fix AddIngestionJobsTable migration: creates table if migration was applied with empty Up().
# Requires: .env with AzureSql__ConnectionString
#
# Usage: .\scripts\fix-ingestion-jobs-migration.ps1

$ErrorActionPreference = "Stop"
$repoRoot = Join-Path $PSScriptRoot ".."

& "$PSScriptRoot\load-env.ps1" dotnet run --project "$repoRoot\backend\src\tools\LegalAiAr.EmptyKb\LegalAiAr.EmptyKb.csproj" -- --fix-ingestion-jobs-migration
