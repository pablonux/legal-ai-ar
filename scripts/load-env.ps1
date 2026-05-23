# Load .env file into process environment variables
# Usage:
#   . .\scripts\load-env.ps1              # Load .env and keep vars in current shell
#   .\scripts\load-env.ps1                # Load .env (vars persist in same process)
#   .\scripts\load-env.ps1 dotnet run ... # Load .env and run command

param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$Command
)

$repoRoot = Join-Path $PSScriptRoot ".."
$envFile = Join-Path $repoRoot ".env"
if (-not (Test-Path $envFile)) {
    Write-Error ".env not found at $envFile. Copy from .env.example and configure."
    exit 1
}

$count = 0
Get-Content $envFile | ForEach-Object {
    $line = $_.Trim()
    if ($line -and -not $line.StartsWith("#")) {
        if ($line -match '^\s*([^#=][^=]*)=(.*)$') {
            $name = $matches[1].Trim()
            $value = $matches[2].Trim() -replace '^["'']|["'']$'
            if ($name) {
                [Environment]::SetEnvironmentVariable($name, $value, 'Process')
                $count++
            }
        }
    }
}

Write-Host "Loaded $count variables from .env" -ForegroundColor Green

if ($Command.Count -gt 0) {
    $exe = $Command[0]
    $rest = $Command[1..($Command.Count - 1)]
    & $exe @rest
}
