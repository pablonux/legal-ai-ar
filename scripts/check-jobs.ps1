$ApiBase = "http://localhost:5088"
# Local API injects platform identity in Development (no login required).
$jobs = Invoke-RestMethod -Uri "$ApiBase/api/admin/jobs" -Method Get
Write-Host "Jobs: $($jobs.Count)"
$jobs | ConvertTo-Json -Depth 5
