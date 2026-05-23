# Docker Compose — Desarrollo local

| Campo | Valor |
|---|---|
| **Feature** | F0-2 · Infraestructura Azure |
| **Tarea** | T-09 |
| **Fecha** | 2026-03-10 |

---

## Propósito

El archivo `docker-compose.yml` en la raíz del repositorio levanta SQL Server para desarrollo local. Permite ejecutar la API, workers y migraciones EF Core contra una base de datos local sin depender de Azure SQL.

**Nota**: Azure Blob Storage, Storage Queues, AI Search y Azure OpenAI se usan directamente desde Azure en desarrollo (no se emulan localmente).

---

## Servicios

### SQL Server (puerto 1433)

| Atributo | Valor |
|---|---|
| Imagen | `mcr.microsoft.com/mssql/server:2022-latest` |
| Usuario | `sa` |
| Contraseña | `Dev_Password123!` |
| Base de datos | `LegalAiAr` (se crea al ejecutar migraciones) |

### Neo4j (opcional, perfil `neo4j`)

| Atributo | Valor |
|---|---|
| Imagen | `neo4j:5-community` |
| Usuario | `neo4j` |
| Contraseña | `dev_password` |
| Puertos | 7474 (HTTP), 7687 (Bolt) |

Neo4j no se usa en Fase 1 (grafo en SQL). Incluido para Fase 3. Se inicia con `--profile neo4j`.

---

## Uso

### Levantar SQL Server

```powershell
docker compose up -d sqlserver
```

### Verificar que está corriendo

```powershell
docker compose ps
```

### Aplicar migraciones

Con SQL Server en ejecución:

```powershell
$env:AzureSql__ConnectionString = "Server=localhost,1433;Database=LegalAiAr;User Id=sa;Password=Dev_Password123!;TrustServerCertificate=True;"
cd backend
dotnet ef database update --project src/shared/LegalAiAr.Infrastructure/LegalAiAr.Infrastructure.csproj --startup-project src/api/LegalAiAr.Api/LegalAiAr.Api.csproj
```

O cargar `.env` con la connection string local:

```powershell
# En .env o .env.local (para desarrollo):
# AzureSql__ConnectionString=Server=localhost,1433;Database=LegalAiAr;User Id=sa;Password=Dev_Password123!;TrustServerCertificate=True;
.\scripts\load-env.ps1
cd backend
dotnet ef database update --project src/shared/LegalAiAr.Infrastructure/LegalAiAr.Infrastructure.csproj --startup-project src/api/LegalAiAr.Api/LegalAiAr.Api.csproj
```

### Detener

```powershell
docker compose down
```

### Levantar SQL Server + Neo4j (Fase 3)

```powershell
docker compose --profile neo4j up -d
```

---

## Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows o Mac)
- [Docker](https://docs.docker.com/engine/install/) (Linux)

---

## Referencias

- `docs/design/f0-2-infrastructure.md` — section 3 (Local environment)
- `docs/design/f0-2-environment-variables.md` — section 5 (Local development)
- `docs/setup/ef-migrations.md` — Desarrollo local (SQL Server en Docker)
