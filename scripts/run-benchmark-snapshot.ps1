# Ejecuta scripts/sql/benchmark-latest-job-snapshot.sql con AzureSql__ConnectionString del .env raíz.
# Alternativa robusta: dotnet run -c Release --project backend/src/tools/LegalAiAr.VerifyConnectivity/LegalAiAr.VerifyConnectivity.csproj -- benchmark-job-snapshot (desde repo raíz).
$ErrorActionPreference = 'Stop'
$repoRoot = Join-Path $PSScriptRoot '..'
. (Join-Path $PSScriptRoot 'load-env.ps1')
if (-not $env:AzureSql__ConnectionString) {
    throw 'AzureSql__ConnectionString missing after load-env.'
}

$cs = $env:AzureSql__ConnectionString
$pairs = @{}
foreach ($seg in ($cs -split ';')) {
    $seg = $seg.Trim()
    if ($seg -match '^([^=]+)=(.*)$') {
        $pairs[$matches[1].Trim()] = $matches[2].Trim()
    }
}

$S = $pairs['Server'] -replace '^tcp:'
$d = $pairs['Database']
$u = $pairs['User ID']
$p = $pairs['Password']
if (-not $S -or -not $d) {
    throw 'Could not parse Server/Database from AzureSql__ConnectionString.'
}

$inputSql = Join-Path $repoRoot 'scripts\sql\benchmark-latest-job-snapshot.sql'
$outTxt = Join-Path $env:TEMP 'legal-ai-ar-benchmark-snapshot.txt'
& sqlcmd -S $S -d $d -U $u -P $p -C -i $inputSql -o $outTxt -W -s "`t"

if ($LASTEXITCODE -ne 0) {
    throw "sqlcmd failed with exit code $LASTEXITCODE"
}

Get-Content $outTxt
Write-Host "`n_written: $outTxt" -ForegroundColor Green
