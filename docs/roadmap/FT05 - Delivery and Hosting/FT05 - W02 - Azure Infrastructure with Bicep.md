# FT05 - W02 - Azure Infrastructure with Bicep (IaC)

> **Feature:** FT05 - Delivery and Hosting (formerly F00-W05)
> **Release:** Cross-cutting (FT) | **Sprint:** —
> **Track status:** ⏸ **On hold** — pending DevOps consultation (Azure subscription / IaC ownership)
> **Type:** devops | **Priority:** High
> **Estimate:** 8 story points
> **Assignable to:** Backend Dev

---

## Description

Define all Azure infrastructure as code using Bicep. Create reusable modules for each service and per-environment parameter files (dev, qa, staging, prod). Provision at least the DEV environment as validation.

---

## Tasks

- [ ] Create the `infra/bicep/` structure with `main.bicep` and a `modules/` folder
- [ ] Create the `sql.bicep` module (Azure SQL Server + Database)
- [ ] Create the `search.bicep` module (Azure AI Search)
- [ ] Create the `openai.bicep` module (Azure OpenAI)
- [ ] Create the `keyvault.bicep` module (Key Vault with access policies)
- [ ] Create the `storage.bicep` module (Storage Account: blobs, queues, tables)
- [ ] Create the `appservice.bicep` module (App Service Plan + Web App .NET 10)
- [ ] Create the `staticwebapp.bicep` module (Static Web App for Angular)
- [ ] Create the `functions.bicep` module (Function App)
- [ ] Create the `signalr.bicep` module (SignalR Service)
- [ ] Create the `appinsights.bicep` module (Application Insights + Log Analytics)
- [ ] Create `main.bicep` that orchestrates all modules
- [ ] Create parameter files: `dev.bicepparam`, `qa.bicepparam`, `staging.bicepparam`, `prod.bicepparam`
- [ ] Configure outputs for connection strings and endpoints
- [ ] Provision the DEV environment: `az deployment sub create --location eastus2 --template-file main.bicep --parameters dev.bicepparam`
- [ ] Verify that all resources are created correctly
- [ ] Configure Key Vault with the initial secrets
- [ ] Document the naming convention and applied tags

---

## Resource Diagram per Environment

```mermaid
graph TB
    subgraph "Resource Group: rg-legal-ai-ar-{env}"
        SQL[sql-legal-ai-ar-{env}<br/>Azure SQL Server]
        SQLDB[(sqldb-legal-ai-ar-{env}<br/>Azure SQL Database)]
        SRCH[srch-legal-ai-ar-{env}<br/>AI Search]
        AOAI[oai-legal-ai-ar-{env}<br/>Azure OpenAI]
        KV[kv-legal-ai-ar-{env}<br/>Key Vault]
        ST[st-legal-ai-ar-{env}<br/>Storage Account]
        APP[app-legal-ai-ar-{env}<br/>App Service]
        PLAN[plan-legal-ai-ar-{env}<br/>App Service Plan]
        SWA[swa-legal-ai-ar-{env}<br/>Static Web App]
        FUNC[func-legal-ai-ar-{env}<br/>Function App]
        SR[sr-legal-ai-ar-{env}<br/>SignalR]
        AI[ai-legal-ai-ar-{env}<br/>App Insights]
        LOG[log-legal-ai-ar-{env}<br/>Log Analytics]
    end

    SQL --> SQLDB
    PLAN --> APP
    PLAN --> FUNC
    AI --> LOG
    APP --> KV
    APP --> SQL
    APP --> SRCH
    APP --> AOAI
    APP --> ST
    APP --> SR
    FUNC --> KV
    FUNC --> SQL
    FUNC --> SRCH
```

---

## SKU per Environment

| Service          | DEV             | QA              | STAGING         | PROD            |
| ---------------- | --------------- | --------------- | --------------- | --------------- |
| SQL Database     | Basic (5 DTU)   | S0 (10 DTU)     | S1 (20 DTU)     | S2 (50 DTU)     |
| AI Search        | Free            | Basic           | Basic           | Standard        |
| OpenAI           | S0              | S0              | S0              | S0              |
| App Service Plan | F1 (Free)       | B1 (Basic)      | B2 (Basic)      | S1 (Standard)   |
| Storage          | LRS             | LRS             | LRS             | GRS             |
| SignalR          | Free            | Free            | Standard        | Standard        |
| Key Vault        | Standard        | Standard        | Standard        | Standard        |
| App Insights     | (pay-as-you-go) | (pay-as-you-go) | (pay-as-you-go) | (pay-as-you-go) |

---

## Standard Tags

All resources must have these tags:

```json
{
    "project": "legal-ai-ar",
    "environment": "{dev|qa|staging|prod}",
    "owner": "legal-ai-ar-team",
    "costCenter": "TBD",
    "managedBy": "bicep"
}
```

---

## Module Example (`sql.bicep`)

```bicep
param location string
param environment string
param sqlAdminLogin string
@secure()
param sqlAdminPassword string
param tags object

var serverName = 'sql-legal-ai-ar-${environment}'
var dbName = 'sqldb-legal-ai-ar-${environment}'

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: serverName
  location: location
  tags: tags
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: dbName
  location: location
  tags: tags
  sku: {
    name: environment == 'prod' ? 'S2' : (environment == 'staging' ? 'S1' : (environment == 'qa' ? 'S0' : 'Basic'))
  }
}

// Allow Azure services
resource firewallRule 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output serverFqdn string = sqlServer.properties.fullyQualifiedDomainName
output databaseName string = sqlDb.name
output connectionString string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Database=${dbName};Encrypt=true;'
```

---

## Acceptance Criteria

- [ ] `az deployment sub what-if` shows the resources to create with no errors
- [ ] The DEV environment is provisioned completely with a single command
- [ ] All resources follow the defined naming convention
- [ ] All resources have the standard tags applied
- [ ] Key Vault has the correct access policies for App Service and Functions
- [ ] Connection strings and endpoints are stored as outputs
- [ ] The modules are reusable across environments (only the parameters change)

---

## Dependencies

- **Depends on:** An Azure subscription with Contributor permissions
- **Blocks:** FT05-W03 (CD pipelines need infrastructure to deploy to)

---

_FT05 - W02 - Azure Infrastructure with Bicep (IaC) — Legal Ai Ar_
