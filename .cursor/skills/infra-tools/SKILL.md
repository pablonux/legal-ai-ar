---
name: infra-tools
description: "Infrastructure tools for Legal Ai Ar: Azure configuration helper (appsettings, env vars, Key Vault), and GitHub Actions templates for CI/CD. Use when asked to configure Azure, add an environment variable, create an appsettings, configure Key Vault, create a CI/CD workflow, or any infrastructure task. Also applies with 'appsettings', 'env var', 'Key Vault', 'pipeline', 'CI', 'CD', 'deploy', 'GitHub Actions'."
---

# Infra Tools — Legal Ai Ar

Azure configuration and CI/CD templates and conventions.

---

## 1. Azure Config Helper

### Resource naming

```
{service}-legal-ai-ar-{environment}
```

| Service | Dev example | Prod example |
|---------|-------------|--------------|
| Azure SQL | `sql-legal-ai-ar-dev` | `sql-legal-ai-ar-prod` |
| AI Search | `search-legal-ai-ar-dev` | `search-legal-ai-ar-prod` |
| Storage | `stlegalaiardev` | `stlegalaiarprod` |
| App Service | `app-legal-ai-ar-dev` | `app-legal-ai-ar-prod` |
| OpenAI | `oai-legal-ai-ar-dev` | `oai-legal-ai-ar-prod` |
| SignalR | `sigr-legal-ai-ar-dev` | `sigr-legal-ai-ar-prod` |
| Key Vault | `kv-legal-ai-ar-dev` | `kv-legal-ai-ar-prod` |

Storage accounts do not allow hyphens — use all lowercase with no separators.

### appsettings structure

```
mvp/backend/src/api/LegalAiAr.Api/
├── appsettings.json              # Base config (no secrets)
├── appsettings.Development.json  # Overrides for local dev
└── appsettings.Production.json   # Overrides for prod (structure only)
```

### appsettings.json template

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "AzureOpenAI": {
    "Endpoint": "",
    "DeploymentName": "gpt-5o",
    "EmbeddingDeploymentName": "text-embedding-3-large",
    "EmbeddingDimensions": 3072
  },
  "AzureAISearch": {
    "Endpoint": "",
    "IndexLegalNorms": "idx-legal-norms",
    "IndexCaseLaw": "idx-case-law",
    "IndexChunks": "idx-chunks"
  },
  "AzureStorage": {
    "ConnectionString": "",
    "ContainerDocuments": "legal-documents",
    "ContainerCache": "cache-pdf"
  },
  "AzureSignalR": {
    "ConnectionString": ""
  },
  "AzureEntraId": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "",
    "ClientId": "",
    "Audience": ""
  },
  "Ingestion": {
    "MaxConcurrencyPerStage": 5,
    "RetryCount": 3,
    "RetryDelayMs": 1000
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

### Environment variables

Secret keys are passed as env vars or Key Vault references. Never in appsettings.

```bash
# .env.example — Variables required for local development

# Azure SQL
ConnectionStrings__DefaultConnection=Server=localhost;Database=LegalAiAr;Trusted_Connection=True;

# Azure OpenAI
AzureOpenAI__Endpoint=https://oai-legal-ai-ar-dev.openai.azure.com/
AzureOpenAI__ApiKey=<your-api-key>

# Azure AI Search
AzureAISearch__Endpoint=https://search-legal-ai-ar-dev.search.windows.net
AzureAISearch__ApiKey=<your-api-key>

# Azure Storage
AzureStorage__ConnectionString=UseDevelopmentStorage=true

# Azure SignalR
AzureSignalR__ConnectionString=Endpoint=https://sigr-legal-ai-ar-dev.service.signalr.net;AccessKey=<key>;Version=1.0;

# Azure Entra ID
AzureEntraId__TenantId=<your-tenant-id>
AzureEntraId__ClientId=<your-client-id>
```

### Key Vault in production

In production, env vars are replaced with Key Vault references in App Service:

```
@Microsoft.KeyVault(SecretUri=https://kv-legal-ai-ar-prod.vault.azure.net/secrets/{secret-name}/)
```

### Typed options class

For each appsettings section, create an `IOptions<T>` class:

```csharp
namespace LegalAiAr.Infrastructure.Configuration;

public class AzureOpenAIOptions
{
    public const string SectionName = "AzureOpenAI";

    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = "gpt-5o";
    public string EmbeddingDeploymentName { get; set; } = "text-embedding-3-large";
    public int EmbeddingDimensions { get; set; } = 3072;
}
```

DI registration:
```csharp
services.Configure<AzureOpenAIOptions>(
    configuration.GetSection(AzureOpenAIOptions.SectionName));
```

---

## 2. GitHub Actions

### Location

```
.github/workflows/
├── ci.yml              # Build + test on each PR
├── cd-dev.yml          # Deploy to dev on push to main
└── cd-prod.yml         # Deploy to prod (manual)
```

### CI template — build and test

```yaml
# .github/workflows/ci.yml
name: CI — Build & Test

on:
  pull_request:
    branches: [main]
    paths:
      - 'mvp/backend/**'
      - 'mvp/frontend/**'

jobs:
  backend:
    name: Backend (.NET 10)
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: mvp/backend
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --configuration Release --verbosity normal

  frontend:
    name: Frontend (Angular 19)
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: mvp/frontend
    steps:
      - uses: actions/checkout@v4

      - name: Setup Node
        uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: mvp/frontend/package-lock.json

      - name: Install
        run: npm ci

      - name: Lint
        run: npm run lint

      - name: Build
        run: npm run build -- --configuration production

      - name: Test
        run: npm run test -- --watch=false --browsers=ChromeHeadless
```

### CD template — deploy to App Service

```yaml
# .github/workflows/cd-dev.yml
name: CD — Deploy Dev

on:
  push:
    branches: [main]
    paths:
      - 'mvp/backend/**'

jobs:
  deploy:
    name: Deploy Backend to Dev
    runs-on: ubuntu-latest
    environment: development
    defaults:
      run:
        working-directory: mvp/backend
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Publish
        run: >
          dotnet publish src/api/LegalAiAr.Api/LegalAiAr.Api.csproj
          --configuration Release
          --output ./publish

      - name: Deploy to Azure App Service
        uses: azure/webapps-deploy@v3
        with:
          app-name: app-legal-ai-ar-dev
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_DEV }}
          package: mvp/backend/publish
```

### Conventions

- CI runs on every PR that touches backend or frontend
- CD dev: automatic on push to main
- CD prod: manual (workflow_dispatch) with approval
- GitHub secrets: `AZURE_WEBAPP_PUBLISH_PROFILE_DEV`, `AZURE_WEBAPP_PUBLISH_PROFILE_PROD`
- Working directory always relative to `mvp/`
- NuGet and npm cache to speed up builds
```
