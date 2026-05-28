# F00 - W07 - Local Environment Setup and Onboarding Guide

> **Feature:** F00 - Development Environment and Structure
> **Release:** 0.0 | **Sprint:** S00
> **Type:** doc | **Priority:** High
> **Assignable to:** Backend or Frontend Dev (whoever finishes W02/W03 first)

---

## Description

Create the onboarding guide so any developer can clone the repo and have the project running locally in under 30 minutes. Includes automated setup scripts, docker-compose for local dependencies, and step-by-step documentation.

---

## Tasks

- [ ] Create `docker-compose.yml` at the root with a local SQL Server
- [ ] Create the `infra/scripts/setup-local.ps1` script (Windows)
- [ ] Create the `infra/scripts/setup-local.sh` script (Linux/Mac)
- [ ] Create `infra/scripts/seed-db.sql` with initial sample data
- [ ] Create `docs/onboarding/README.md` with a step-by-step guide
- [ ] Create `docs/onboarding/troubleshooting.md` with common errors and solutions
- [ ] Update the root `README.md` with a quick start
- [ ] Verify that a new dev can follow the guide and bring everything up in < 30 min
- [ ] Add a recommended-extensions section for VS Code and Visual Studio

---

## docker-compose.yml

```yaml
# docker-compose.yml (repo root)
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: legal-ai-ar-sql
    ports:
      - "1433:1433"
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "LocalDev123!"
      MSSQL_PID: "Developer"
    volumes:
      - sqldata:/var/opt/mssql

  azurite:
    image: mcr.microsoft.com/azure-storage/azurite
    container_name: legal-ai-ar-azurite
    ports:
      - "10000:10000"   # Blob
      - "10001:10001"   # Queue
      - "10002:10002"   # Table
    volumes:
      - azuritedata:/data

volumes:
  sqldata:
  azuritedata:
```

---

## Local Setup Script (PowerShell)

```powershell
# infra/scripts/setup-local.ps1
Write-Host "=== Legal Ai Ar - Local Setup ===" -ForegroundColor Cyan

# 1. Check prerequisites
$checks = @(
    @{ Name = ".NET SDK"; Cmd = "dotnet --version"; Min = "10.0" }
    @{ Name = "Node.js"; Cmd = "node --version"; Min = "22.0" }
    @{ Name = "Docker"; Cmd = "docker --version"; Min = "" }
    @{ Name = "Git"; Cmd = "git --version"; Min = "" }
    @{ Name = "Azure CLI"; Cmd = "az --version"; Min = "" }
)

foreach ($check in $checks) {
    try {
        $result = Invoke-Expression $check.Cmd 2>$null
        Write-Host "  ✅ $($check.Name): $result" -ForegroundColor Green
    } catch {
        Write-Host "  ❌ $($check.Name): NOT INSTALLED" -ForegroundColor Red
        exit 1
    }
}

# 2. Start Docker
Write-Host "`n📦 Starting Docker services..." -ForegroundColor Yellow
docker compose up -d

# 3. Backend
Write-Host "`n🔧 Configuring backend..." -ForegroundColor Yellow
Set-Location backend
dotnet restore
dotnet build

# 4. Migrations
Write-Host "`n🗄️ Applying migrations..." -ForegroundColor Yellow
Set-Location src/LegalAiAr.Api
dotnet ef database update
Set-Location ../..

# 5. Seed data
Write-Host "`n🌱 Seed data..." -ForegroundColor Yellow
# sqlcmd -S localhost -U sa -P "LocalDev123!" -d LegalAiAr -i ../infra/scripts/seed-db.sql

# 6. Frontend
Write-Host "`n🎨 Configuring frontend..." -ForegroundColor Yellow
Set-Location ../frontend
npm ci

Write-Host "`n✅ Setup complete!" -ForegroundColor Green
Write-Host "To start:" -ForegroundColor Cyan
Write-Host "  Backend:  cd backend/src/LegalAiAr.Api && dotnet run"
Write-Host "  Frontend: cd frontend && npm start"
Write-Host "  Swagger:  https://localhost:5001/swagger"
Write-Host "  App:      http://localhost:4200"
```

---

## Recommended Extensions

### VS Code (Frontend)
```json
// .vscode/extensions.json
{
  "recommendations": [
    "angular.ng-template",
    "esbenp.prettier-vscode",
    "dbaeumer.vscode-eslint",
    "ms-vscode.vscode-typescript-next",
    "bradlc.vscode-tailwindcss",
    "bierner.markdown-mermaid",
    "eamodio.gitlens"
  ]
}
```

### Visual Studio 2022 (Backend)
- EF Core Power Tools
- Roslynator
- SerilogAnalyzer
- EditorConfig Language Service

---

## Acceptance Criteria

- [ ] `docker compose up -d` starts SQL Server and Azurite with no errors
- [ ] The setup script installs everything and leaves the project ready
- [ ] The onboarding guide covers 100% of the setup
- [ ] A new dev can follow the guide and have everything running in < 30 min
- [ ] The troubleshooting covers at least 5 common errors
- [ ] The recommended extensions are configured

---

## Dependencies

- **Depends on:** F00-W02, F00-W03 (existing backend and frontend projects)
- **Blocks:** None (but it is critical for team onboarding)

---

*F00 - W07 - Local Environment Setup and Onboarding Guide — Legal Ai Ar*
