# Legal AI AR — Infrastructure

Scripts and configuration for Azure deployment (F1-14).

## Prerequisites

- [Azure CLI](https://aka.ms/install-azure-cli) installed and logged in (`az login`)
- For Storage Queues: `Az.Storage` PowerShell module or Azure CLI

## Scripts

| Script | Purpose |
|--------|---------|
| `scripts/create-storage-queues.ps1` | Create Storage Queues for the ingestion pipeline (F0-2 T-07) |
| `scripts/create-app-service.ps1` | Create App Service with staging slot for API (F1-14 T-01) |
| `scripts/verify-azure-connectivity.ps1` | Verify connectivity to Azure resources |
| `scripts/verify-azure-openai.ps1` | Verify Azure OpenAI connectivity |

## F1-14 Deployment

### T-01 — App Service with deployment slots

```powershell
cd infra/scripts
.\create-app-service.ps1
```

Options:
- `-ResourceGroup` — Resource group name (default: `legal-ai-ar-rg`)
- `-Location` — Azure region (default: `eastus2`)
- `-AppName` — Web App name, must be globally unique (default: `legal-ai-ar-api`)

After creation, configure Application settings in Azure Portal with variables from `docs/design/f0-2-environment-variables.md` and `.env.example`.

### T-02 — Static Web Apps

See `docs/design/f1-14-deploy.md` for Static Web Apps setup. Create via Azure Portal or:

```bash
az staticwebapp create --name legal-ai-ar --resource-group legal-ai-ar-rg --source <repo-url>
```

### T-03 — Container Registry

```bash
az acr create --name legalaiaracr --resource-group legal-ai-ar-rg --sku Basic
```

### T-04 — Container Apps

Create Container Apps environment and 4 worker apps. See architecture section 9 and `docs/design/f1-14-deploy.md`.

### T-05 — CD Pipeline

GitHub Actions workflow `.github/workflows/cd.yml` deploys on merge to `main`:

1. **Secrets** (GitHub repo → Settings → Secrets):
   - `AZURE_CREDENTIALS` — JSON from `az ad sp create-for-rbac --scopes /subscriptions/<sub>/resourceGroups/<rg> --role Contributor --sdk-auth`
   - `AZURE_STATIC_WEB_APPS_API_TOKEN` — From Static Web App → Manage deployment token

2. **Environments**: Create `staging` environment in GitHub (Settings → Environments).

3. **Variables** (optional): `APP_SERVICE_NAME` if different from `legal-ai-ar-api`.
