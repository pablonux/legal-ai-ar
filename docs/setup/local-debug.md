# Local Debug Environment — Front + Back + Workers

Debug frontend (Angular), backend (ASP.NET Core API) and workers in VS Code/Cursor.

## Prerequisites

- **.NET 8 SDK**
- **Node.js 18+** and npm
- **.env** file at repo root (copy from `.env.example`, fill with Azure credentials)

## Quick Start

1. **Configure `.env`** at the repo root with your Azure SQL, Blob, Search, etc. (see `.env.example`).

2. **Launch both apps**:
   - Open **Run and Debug** (Ctrl+Shift+D)
   - Select **Full Stack (Front + Back)** from the dropdown
   - Press **F5** or click the green play button

3. **Verify**:
   - Backend: http://localhost:5088 (Swagger: http://localhost:5088/swagger)
   - Frontend: http://localhost:4200
   - In the frontend, click **"Llamar API"** to test the API connection

## Launch Configurations

| Configuration | Description |
|---|---|
| **Backend (API)** | Runs LegalAiAr.Api with debugger on port 5088. Loads `.env`. |
| **Frontend (Angular)** | Starts `ng serve` and opens Chrome with debugger on port 4200 |
| **Full Stack (Front + Back)** | Launches both; use this for full-stack debugging |
| **Worker: Crawler** | Debugs the crawler worker. Loads `.env`. |
| **Worker: Parser** | Debugs the parser worker. Loads `.env`. |
| **Worker: Enrichment** | Debugs the enrichment worker. Loads `.env`. |
| **Worker: Indexer** | Debugs the indexer worker. Loads `.env`. |

## Debugging Workers

1. Select **Worker: Crawler** (or Parser, Enrichment, Indexer) from the Run and Debug dropdown.
2. Set breakpoints in the worker code (e.g. `CrawlerWorkerService.cs`).
3. Press **F5** or click the green play button.
4. The worker runs and polls its queue; when a message arrives, execution will hit your breakpoints.

Workers use the same `.env` as the API (Azure SQL, Blob, Storage Queues, AI Search, etc.).

## Prueba E2E completa (cargar un job)

Para ejecutar un flujo completo: crawl → parser → enrichment → indexer → ver job:

1. **Preparar infraestructura** (una vez):
   ```powershell
   .\scripts\load-env.ps1
   .\infra\scripts\create-storage-queues.ps1
   ```

2. **Iniciar todos los servicios** (API + 4 workers + frontend):
   - **Opción A (Docker)**: `docker compose -f docker-compose.app.yml up -d`
   - **Opción B (Run and Debug)**: Lanzar en orden: Backend (API), Worker: Crawler, Worker: Parser, Worker: Enrichment, Worker: Indexer, Frontend (Angular)

3. **Ejecutar la prueba**:
   ```powershell
   .\scripts\test-full-job.ps1
   ```
   El script hace login, dispara un crawl incremental en CSJN (sourceId=1), espera el job y muestra métricas.

4. **Verificar en la UI**: http://localhost:4200/admin/jobs

Opciones del script: `-CrawlType by-range` (con DateFrom/DateTo), `-SourceId 2`, `-MaxWaitSeconds 300`.

### Ejecutar job con captura de logs (para análisis de pérdida de docs)

Para analizar si se pierden documentos entre workers, usá `run-job-with-logs.ps1`:

```powershell
# Con Docker (captura logs automáticamente)
.\scripts\run-job-with-logs.ps1 -Mode Docker -DateFrom "2024-12-01" -DateTo "2024-12-31"

# Con Run and Debug (guardá logs manualmente desde cada terminal de worker)
.\scripts\run-job-with-logs.ps1 -Mode Manual -DateFrom "2024-12-01" -DateTo "2024-12-31"
```

Los logs se guardan en `logs/run-YYYYMMDD-HHmmss/`. **Criterio de cierre**: los números cierran si `documentsDiscovered = rulings en BD + mensajes en DLQs`. Si no cierran, hay pérdida de documentos entre workers; revisá los logs para analizar.

## Configuration Details

- **API**: `backend/src/api/LegalAiAr.Api/` — CORS allows `http://localhost:4200`
- **Frontend**: `frontend/` — Angular 18, calls API at `http://localhost:5088`
- **Database**: `AzureSql__ConnectionString` from `.env` (Azure SQL)
