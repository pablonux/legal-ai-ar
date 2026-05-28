# F00 - W07 - Setup Entorno Local y Onboarding Guide

> **Feature:** F00 - Entorno y Estructura de Desarrollo
> **Release:** 0.0 | **Sprint:** S00
> **Tipo:** doc | **Prioridad:** Alta
> **Estimación:** 3 story points
> **Asignable a:** Dev Backend o Frontend (quien termine primero W02/W03)

---

## Descripción

Crear la guía de onboarding para que cualquier desarrollador pueda clonar el repo y tener el proyecto corriendo localmente en menos de 30 minutos. Incluye scripts de setup automatizado, docker-compose para dependencias locales y documentación paso a paso.

---

## Tareas

- [ ] Crear `docker-compose.yml` en la raíz con SQL Server local
- [ ] Crear script `infra/scripts/setup-local.ps1` (Windows)
- [ ] Crear script `infra/scripts/setup-local.sh` (Linux/Mac)
- [ ] Crear `infra/scripts/seed-db.sql` con datos de prueba iniciales
- [ ] Crear `docs/onboarding/README.md` con guía paso a paso
- [ ] Crear `docs/onboarding/troubleshooting.md` con errores comunes y soluciones
- [ ] Actualizar `README.md` raíz con quick start
- [ ] Verificar que un dev nuevo puede seguir la guía y levantar todo en < 30 min
- [ ] Agregar sección de extensiones recomendadas para VS Code y Visual Studio

---

## docker-compose.yml

```yaml
# docker-compose.yml (raíz del repo)
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

## Script Setup Local (PowerShell)

```powershell
# infra/scripts/setup-local.ps1
Write-Host "=== Legal Ai Ar - Setup Local ===" -ForegroundColor Cyan

# 1. Verificar prerequisitos
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

# 2. Levantar Docker
Write-Host "`n📦 Levantando servicios Docker..." -ForegroundColor Yellow
docker compose up -d

# 3. Backend
Write-Host "`n🔧 Configurando backend..." -ForegroundColor Yellow
Set-Location backend
dotnet restore
dotnet build

# 4. Migraciones
Write-Host "`n🗄️ Aplicando migraciones..." -ForegroundColor Yellow
Set-Location src/LegalAiAr.Api
dotnet ef database update
Set-Location ../..

# 5. Seed data
Write-Host "`n🌱 Seed data..." -ForegroundColor Yellow
# sqlcmd -S localhost -U sa -P "LocalDev123!" -d LegalAiAr -i ../infra/scripts/seed-db.sql

# 6. Frontend
Write-Host "`n🎨 Configurando frontend..." -ForegroundColor Yellow
Set-Location ../frontend
npm ci

Write-Host "`n✅ Setup completo!" -ForegroundColor Green
Write-Host "Para iniciar:" -ForegroundColor Cyan
Write-Host "  Backend:  cd backend/src/LegalAiAr.Api && dotnet run"
Write-Host "  Frontend: cd frontend && npm start"
Write-Host "  Swagger:  https://localhost:5001/swagger"
Write-Host "  App:      http://localhost:4200"
```

---

## Extensiones Recomendadas

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

## Criterios de Aceptación

- [ ] `docker compose up -d` levanta SQL Server y Azurite sin errores
- [ ] El script de setup instala todo y deja el proyecto listo
- [ ] La guía de onboarding cubre el 100% del setup
- [ ] Un dev nuevo puede seguir la guía y tener todo corriendo en < 30 min
- [ ] El troubleshooting cubre al menos 5 errores comunes
- [ ] Las extensiones recomendadas están configuradas

---

## Dependencias

- **Depende de:** F00-W02, F00-W03 (proyectos backend y frontend existentes)
- **Bloquea:** Ninguno (pero es crítico para onboarding del equipo)

---

*F00 - W07 - Setup Entorno Local y Onboarding Guide — Legal Ai Ar*
