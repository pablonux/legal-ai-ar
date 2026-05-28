# Staging Verification Tutorial — E011 to E018

Step-by-step guide to verify F0-2 deliverables (E011–E014, E018) against staging Azure services and update the ROADMAP.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PowerShell](https://docs.microsoft.com/powershell/) (Windows) or [PowerShell Core](https://github.com/PowerShell/PowerShell) (cross-platform)
- Azure credentials for **staging** environment (SQL, Blob, Search, OpenAI)
- [Azure CLI](https://aka.ms/install-azure-cli) or [Az.Storage](https://docs.microsoft.com/powershell/azure/install-az-ps) module (for Storage Queues)

---

## Part 1 — Run Setup & Verification Against Staging

### Step 1.1 — Create `.env.staging`

1. Copy the example file:
   ```powershell
   Copy-Item .env.example .env.staging
   ```

2. Edit `.env.staging` and replace placeholders with **staging** Azure credentials:
   - `DB_SECRET` → Staging Azure SQL password
   - `STORAGE_KEY` → Staging Storage Account key
   - `SEARCH_KEY` → Staging AI Search admin key
   - `OPENAI_KEY` → Azure OpenAI API key

3. Update connection strings if staging uses different endpoints:
   - `AzureSql__ConnectionString` — use staging database name (e.g. `LegalAI-STAGING`)
   - `AzureBlob__ContainerName` — e.g. `legalaiar-staging` (or keep `legalaiar-dev` if shared)
   - `AzureSearch__Endpoint`, `AzureSearch__ApiKey` — staging AI Search

4. Ensure no placeholder values remain (`DB_SECRET`, `STORAGE_KEY`, `SEARCH_KEY`, `OPENAI_KEY` must be real values).

---

### Step 1.2 — E011: Apply schema and seeds to Azure SQL

Run EF Core migrations against staging Azure SQL.

**PowerShell (from repo root):**

```powershell
# Use .env with staging credentials. Copy .env.staging to .env, or configure .env for staging.
.\scripts\load-env.ps1
cd backend
dotnet ef database update `
  --project src/shared/LegalAiAr.Infrastructure/LegalAiAr.Infrastructure.csproj `
  --startup-project src/api/LegalAiAr.Api/LegalAiAr.Api.csproj
cd ..
```

**Expected output:** `Done.` or `Applying migration '...'` for each migration.

**Verify:** Connect to staging SQL (SSMS, Azure Data Studio) and confirm tables exist: `Rulings`, `Courts`, `Judges`, `Sources`, `CrawlerConfigs`, etc. Check `Sources` has rows (CSJN=1, SAIJ=2, PJN=3, SCBA=4).

---

### Step 1.3 — E012: Create Blob container

Create the PDF container in staging Blob Storage (idempotent — skips if exists).

**PowerShell (from repo root):**

```powershell
# Uses .env. Configure .env with staging credentials or copy .env.staging to .env.
.\scripts\run-setup-blob.ps1
```

**Expected output:** `Container '...' created successfully.` or `Container '...' already exists.`

---

### Step 1.4 — E013: Create AI Search indexes

Create `rulings-by-ruling` and `rulings-by-chunk` indexes (idempotent).

**PowerShell (from repo root):**

```powershell
# Uses .env. Configure .env with staging credentials or copy .env.staging to .env.
.\scripts\run-setup-search.ps1
```

**Expected output:** Index creation messages. No error if indexes already exist.

---

### Step 1.5 — E014: Create Storage Queues

Create the 4 pipeline queues (and optional DLQs) in the staging Storage Account.

**PowerShell (from repo root):**

```powershell
.\infra\scripts\create-storage-queues.ps1 -EnvPath ".\.env.staging"
```

**Expected output:** `=== Completado: X cola(s) creada(s) ===` (0 if all exist).

**Optional — skip DLQ creation:**
```powershell
.\infra\scripts\create-storage-queues.ps1 -EnvPath ".\.env.staging" -SkipDlq
```

---

### Step 1.6 — E018: Run connectivity verification

Verify all services are reachable and configured correctly.

**PowerShell (from repo root):**

```powershell
.\infra\scripts\verify-azure-connectivity.ps1 -EnvPath ".\.env.staging"
```

**Expected output:**
```
=== Verificación de conectividad Azure (F0-2 T-10) ===
Cargando variables desde: ...\.env.staging
Azure SQL...
   OK
Azure Blob...
   OK
Storage Queues...
   OK (4 colas)
Azure AI Search...
   OK (ambos índices)
Azure OpenAI...
   OK
=== Todas las verificaciones pasaron ===
```

**If OpenAI check is slow or fails:**
```powershell
.\infra\scripts\verify-azure-connectivity.ps1 -EnvPath ".\.env.staging" -SkipOpenAI
```

---

### Step 1.7 — Checklist before updating ROADMAP

| # | Deliverable | Verified? |
|---|-------------|-----------|
| E011 | Azure SQL schema + seeds | ☐ Migrations applied, tables and seeds present |
| E012 | Blob container | ☐ Container exists and is accessible |
| E013 | AI Search indexes | ☐ Both indexes exist |
| E014 | Storage Queues | ☐ 4 queues exist |
| E018 | Connectivity | ☐ All checks passed |

---

## Part 2 — Update ROADMAP

After all verifications pass, update `docs/roadmap/ROADMAP.md` to mark deliverables as `[x]` DEV.

### Step 2.1 — Open ROADMAP

Open `docs/roadmap/ROADMAP.md` and locate the F0-2 development deliverables table (around lines 111–120).

### Step 2.2 — Update E011 to E014 and E018

Change the DEV column from `[ ]` to `[x]` for each verified deliverable:

**Before:**
```markdown
| E011 | Existing Azure SQL with complete schema and seeds applied | `[ ]` | `[ ]` |
| E012 | Existing Azure Blob Storage with `rulings-pdfs` container accessible | `[ ]` | `[ ]` |
| E013 | Existing Azure AI Search with both indexes created and verified | `[ ]` | `[ ]` |
| E014 | Storage Queues with 4 operational queues (Phase 1: no Service Bus) | `[ ]` | `[ ]` |
...
| E018 | Connectivity verification to all existing Azure services | `[ ]` | `[ ]` |
```

**After (only for deliverables you verified):**
```markdown
| E011 | Existing Azure SQL with complete schema and seeds applied | `[x]` | `[ ]` |
| E012 | Existing Azure Blob Storage with `rulings-pdfs` container accessible | `[x]` | `[ ]` |
| E013 | Existing Azure AI Search with both indexes created and verified | `[x]` | `[ ]` |
| E014 | Storage Queues with 4 operational queues (Phase 1: no Service Bus) | `[x]` | `[ ]` |
...
| E018 | Connectivity verification to all existing Azure services | `[x]` | `[ ]` |
```

**Note:** Only mark `[x]` DEV for deliverables you verified. If a step failed, leave it as `[ ]` and fix the issue first.

### Step 2.3 — Optional: E016 and E017

These can be marked without Azure access:

- **E016** — `docker-compose.yml` runs SQL Server locally: run `docker compose up -d sqlserver` and confirm it works.
- **E017** — `.env.example` is complete and matches `docs/design/f0-2-environment-variables.md`.

### Step 2.4 — Commit

```powershell
git add docs/roadmap/ROADMAP.md
git commit -m "chore: mark E011-E014, E018 as DEV complete after staging verification"
```

---

## Troubleshooting

| Issue | Possible cause | Action |
|-------|----------------|--------|
| `DB_SECRET` / placeholder in connection string | Env still has placeholders | Replace all placeholders in `.env.staging` |
| SQL connection timeout | Firewall or wrong endpoint | Add your IP to Azure SQL firewall; check connection string |
| Blob "container not found" | Wrong container name | Set `AzureBlob__ContainerName` in `.env.staging` |
| Search indexes 404 | Indexes not created | Run Step 1.4 again |
| Queues script fails | Az.Storage or Azure CLI missing | Install `Az.Storage` or Azure CLI |
| VerifyConnectivity fails | One service misconfigured | Run with `-SkipOpenAI` to isolate; check each service in Azure Portal |

---

## References

- [azure-credentials-guide.md](./azure-credentials-guide.md) — How to obtain credentials
- [azure-connectivity-verification.md](./azure-connectivity-verification.md) — Verification details
- [ef-migrations.md](./ef-migrations.md) — EF Core migrations
- [docs/design/f0-2-environment-variables.md](../design/f0-2-environment-variables.md) — Variable reference
