# F00 - W05 - Infraestructura Azure con Bicep (IaC)

> **Feature:** F00 - Entorno y Estructura de Desarrollo
> **Release:** 0.0 | **Sprint:** S00
> **Tipo:** devops | **Prioridad:** Alta
> **Estimación:** 8 story points
> **Asignable a:** Dev Backend

---

## Descripción

Definir toda la infraestructura Azure como código usando Bicep. Crear módulos reutilizables para cada servicio y archivos de parámetros por ambiente (dev, qa, staging, prod). Provisionar al menos el ambiente DEV como validación.

---

## Tareas

- [ ] Crear estructura `infra/bicep/` con `main.bicep` y carpeta `modules/`
- [ ] Crear módulo `sql.bicep` (Azure SQL Server + Database)
- [ ] Crear módulo `search.bicep` (Azure AI Search)
- [ ] Crear módulo `openai.bicep` (Azure OpenAI)
- [ ] Crear módulo `keyvault.bicep` (Key Vault con access policies)
- [ ] Crear módulo `storage.bicep` (Storage Account: blobs, queues, tables)
- [ ] Crear módulo `appservice.bicep` (App Service Plan + Web App .NET 10)
- [ ] Crear módulo `staticwebapp.bicep` (Static Web App para Angular)
- [ ] Crear módulo `functions.bicep` (Function App)
- [ ] Crear módulo `signalr.bicep` (SignalR Service)
- [ ] Crear módulo `appinsights.bicep` (Application Insights + Log Analytics)
- [ ] Crear `main.bicep` que orquesta todos los módulos
- [ ] Crear archivos de parámetros: `dev.bicepparam`, `qa.bicepparam`, `staging.bicepparam`, `prod.bicepparam`
- [ ] Configurar outputs para connection strings y endpoints
- [ ] Provisionar ambiente DEV: `az deployment sub create --location eastus2 --template-file main.bicep --parameters dev.bicepparam`
- [ ] Verificar que todos los recursos se crean correctamente
- [ ] Configurar Key Vault con secretos iniciales
- [ ] Documentar naming convention y tags aplicados

---

## Diagrama de Recursos por Ambiente

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

## SKU por Ambiente

| Servicio | DEV | QA | STAGING | PROD |
|---|---|---|---|---|
| SQL Database | Basic (5 DTU) | S0 (10 DTU) | S1 (20 DTU) | S2 (50 DTU) |
| AI Search | Free | Basic | Basic | Standard |
| OpenAI | S0 | S0 | S0 | S0 |
| App Service Plan | F1 (Free) | B1 (Basic) | B2 (Basic) | S1 (Standard) |
| Storage | LRS | LRS | LRS | GRS |
| SignalR | Free | Free | Standard | Standard |
| Key Vault | Standard | Standard | Standard | Standard |
| App Insights | (pay-as-you-go) | (pay-as-you-go) | (pay-as-you-go) | (pay-as-you-go) |

---

## Tags Estándar

Todos los recursos deben tener estos tags:

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

## Ejemplo de Módulo (`sql.bicep`)

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

## Criterios de Aceptación

- [ ] `az deployment sub what-if` muestra los recursos a crear sin errores
- [ ] El ambiente DEV se provisiona completamente con un solo comando
- [ ] Todos los recursos siguen la naming convention definida
- [ ] Todos los recursos tienen los tags estándar aplicados
- [ ] Key Vault tiene las access policies correctas para App Service y Functions
- [ ] Los connection strings y endpoints se almacenan como outputs
- [ ] Los módulos son reutilizables entre ambientes (solo cambian los parámetros)

---

## Dependencias

- **Depende de:** Suscripción Azure con permisos de Contributor
- **Bloquea:** F00-W06 (CD pipelines necesitan infra para deployar)

---

*F00 - W05 - Infraestructura Azure con Bicep (IaC) — Legal Ai Ar*
