# EF Core Migrations — Legal AI AR

## Aplicar migraciones a Azure SQL

Desde la raíz del repo, con la variable `AzureSql__ConnectionString` configurada (o cargada desde `.env`):

```powershell
cd backend
$env:AzureSql__ConnectionString = "Server=tcp:u2zfnlvisdsp0001.database.windows.net,1433;Database=LegalAI-DEV;User ID=sqladmin;Password=TU_PASSWORD;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;"
dotnet ef database update --project src/shared/LegalAiAr.Infrastructure/LegalAiAr.Infrastructure.csproj --startup-project src/api/LegalAiAr.Api/LegalAiAr.Api.csproj
```

O en bash/Linux:

```bash
cd backend
export AzureSql__ConnectionString="Server=tcp:..."
dotnet ef database update --project src/shared/LegalAiAr.Infrastructure/LegalAiAr.Infrastructure.csproj --startup-project src/api/LegalAiAr.Api/LegalAiAr.Api.csproj
```

## Crear una nueva migración

```powershell
dotnet ef migrations add NombreMigracion --project src/shared/LegalAiAr.Infrastructure/LegalAiAr.Infrastructure.csproj --startup-project src/api/LegalAiAr.Api/LegalAiAr.Api.csproj --output-dir Persistence/Migrations
```

## Revertir la última migración

```powershell
dotnet ef migrations remove --project src/shared/LegalAiAr.Infrastructure/LegalAiAr.Infrastructure.csproj --startup-project src/api/LegalAiAr.Api/LegalAiAr.Api.csproj
```

## Cargar variables de entorno (.env)

Desde la raíz del repo, para cargar `.env` en la sesión actual:

```powershell
.\scripts\load-env.ps1
```

Luego podés ejecutar cualquier comando (las variables ya están cargadas). O combinar en uno:

```powershell
.\scripts\load-env.ps1 dotnet run --project backend/src/api/LegalAiAr.Api/LegalAiAr.Api.csproj
```

## Crear container Blob Storage (T-04)

Con `.env` configurado (o variables manuales):

```powershell
.\scripts\run-setup-blob.ps1
```

O manualmente tras cargar `.env`:

```powershell
.\scripts\load-env.ps1
cd backend
dotnet run --project src/tools/LegalAiAr.SetupBlob/LegalAiAr.SetupBlob.csproj
```

Crea el container si no existe. Usa `AzureBlob__ContainerName` o `rulings-pdfs` por defecto.

## Crear índices Azure AI Search (T-05)

Con `.env` configurado:

```powershell
.\scripts\run-setup-search.ps1
```

O manualmente tras cargar `.env`:

```powershell
.\scripts\load-env.ps1
cd backend
dotnet run --project src/tools/LegalAiAr.SetupSearch/LegalAiAr.SetupSearch.csproj
```

Crea los índices `rulings-by-ruling` y `rulings-by-chunk` si no existen. Usa los nombres por defecto de la arquitectura o los definidos en las variables de entorno.

---

## Desarrollo local (SQL Server en Docker)

Si usas `docker-compose.yml` con SQL Server local:

```powershell
$env:AzureSql__ConnectionString = "Server=localhost,1433;Database=LegalAiAr;User Id=sa;Password=Dev_Password123!;TrustServerCertificate=True;"
dotnet ef database update --project src/shared/LegalAiAr.Infrastructure/LegalAiAr.Infrastructure.csproj --startup-project src/api/LegalAiAr.Api/LegalAiAr.Api.csproj
```
