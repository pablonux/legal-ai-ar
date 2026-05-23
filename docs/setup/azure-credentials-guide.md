# Guía de obtención de credenciales Azure — Legal AI AR

| Campo | Valor |
|---|---|
| **Feature** | F0-2 · Infraestructura Azure |
| **Tarea** | T-01 |
| **Fecha** | 2026-03-10 |

---

## Propósito

Esta guía indica **dónde y cómo obtener** las credenciales, connection strings y endpoints de los servicios Azure existentes que Legal AI AR utiliza. No se crean recursos nuevos; solo se recopilan los valores necesarios para configurar la aplicación.

**Resultado esperado**: Al finalizar, tendrás todos los valores para completar `.env` (desarrollo local) o las variables de entorno de App Service y Container Apps.

---

## Requisitos previos

- Acceso al [Azure Portal](https://portal.azure.com) con permisos de lectura sobre los recursos del proyecto
- Suscripción Azure donde están aprovisionados los servicios

---

## Valores del proyecto (entorno actual)

| Servicio | Valor (no secret) |
|---|---|
| **Azure SQL** | `u2zfnlvisdsp0001.database.windows.net`, DB: `LegalAI-DEV`, user: `sqladmin` |
| **Blob Storage** | Account: `nus2slfnlvid001`, container: `legalaiar-dev` |
| **AI Search** | `https://u2zfnlviadcs003.search.windows.net` |
| **Azure OpenAI** | `https://genai-sharedservice-americas.pwcinternal.com`, deployments: `azure.gpt-4o`, `azure.text-embedding-3-large` |

**Secrets a reemplazar manualmente** (en `.env` o variables de entorno):

| Placeholder | Dónde obtener el valor real |
|---|---|
| `DB_SECRET` | Contraseña del usuario `sqladmin` en Azure SQL |
| `STORAGE_KEY` | Azure Portal → Storage account `nus2slfnlvid001` → Access keys → Connection string (AccountKey) |
| `SEARCH_KEY` | Azure Portal → AI Search `u2zfnlviadcs003` → Keys → Primary admin key |
| `OPENAI_KEY` | Portal del servicio OpenAI (PwC) o Azure OpenAI → Keys and Endpoint |

---

## 1. Azure SQL Database

| Variable | Descripción |
|---|---|
| `AzureSql__ConnectionString` | Connection string completo de la base de datos |

### Cómo obtener

1. En Azure Portal → **SQL databases** → selecciona la base de datos del proyecto
2. En el menú izquierdo, **Settings** → **Connection strings**
3. Copia el **ADO.NET** connection string
4. Reemplaza `{your_password}` por la contraseña real del usuario
5. Si usas desarrollo local contra SQL Server en Docker, el valor será:
   ```
   Server=localhost,1433;Database=LegalAiAr;User Id=sa;Password=Dev_Password123!;TrustServerCertificate=True
   ```

**Formato esperado**: `Server=tcp:{server}.database.windows.net,1433;Database={db};User ID={user};Password={password};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;`

---

## 2. Azure Blob Storage (y Storage Queues)

El mismo Storage Account alberga Blob Storage y Storage Queues. Una sola connection string sirve para ambos.

| Variable | Descripción |
|---|---|
| `AzureBlob__ConnectionString` | Connection string del Storage Account |
| `AzureBlob__ContainerName` | Nombre del container de PDFs (ej: `rulings-pdfs`) |

### Cómo obtener

1. En Azure Portal → **Storage accounts** → selecciona el account del proyecto
2. En el menú izquierdo, **Security + networking** → **Access keys**
3. En **key1** o **key2**, haz clic en **Show** y copia el valor de **Connection string**
4. Para el container: **Containers** (bajo Data storage) → anota el nombre del container de PDFs. Si no existe, créalo (T-04 de F0-2)

**Formato esperado**: `DefaultEndpointsProtocol=https;AccountName={name};AccountKey={key};EndpointSuffix=core.windows.net`

**Nota índice AI Search**: La arquitectura define dos índices (`rulings-by-ruling`, `rulings-by-chunk`). Si tu entorno usa `legal-ai-indexes` u otra estructura, actualiza `AzureSearch__RulingIndexName` y `AzureSearch__ChunkIndexName` en `.env` o créalos en T-05.

---

## 3. Azure AI Search

| Variable | Descripción |
|---|---|
| `AzureSearch__Endpoint` | URL del servicio (ej: `https://legal-ai-search.search.windows.net`) |
| `AzureSearch__ApiKey` | API key (admin key) |
| `AzureSearch__RulingIndexName` | Nombre del índice a nivel fallo: `rulings-by-ruling` |
| `AzureSearch__ChunkIndexName` | Nombre del índice a nivel fragmento: `rulings-by-chunk` |

### Cómo obtener

1. En Azure Portal → **Azure AI Search** (o Azure Cognitive Search) → selecciona el recurso
2. En **Overview**, copia la **URL** (ej: `https://legal-ai-search.search.windows.net`) → es `AzureSearch__Endpoint`
3. En el menú izquierdo, **Settings** → **Keys**
4. Copia **Primary admin key** o **Secondary admin key** → es `AzureSearch__ApiKey`
5. Los nombres de índices son fijos: `rulings-by-ruling` y `rulings-by-chunk` (se crean en T-05 si no existen)

---

## 4. Azure OpenAI Service

| Variable | Descripción |
|---|---|
| `AzureOpenAI__Endpoint` | URL del recurso (ej: `https://legal-ai-openai.openai.azure.com/`) |
| `AzureOpenAI__ApiKey` | API key del recurso |
| `AzureOpenAI__ChatDeploymentName` | Nombre del deployment de gpt-4o |
| `AzureOpenAI__EmbeddingDeploymentName` | Nombre del deployment de text-embedding-3-large |

### Cómo obtener

1. En Azure Portal → **Azure OpenAI** → selecciona el recurso
2. En **Overview**, copia **Endpoint** (incluye la barra final) → es `AzureOpenAI__Endpoint`
3. En el menú izquierdo, **Resource management** → **Keys and Endpoint**
4. Copia **KEY 1** o **KEY 2** → es `AzureOpenAI__ApiKey`
5. En **Resource management** → **Model deployments** (o **Deployments** en Azure OpenAI Studio):
   - Localiza el deployment de **gpt-4o** → anota el nombre exacto → `AzureOpenAI__ChatDeploymentName`
   - Localiza el deployment de **text-embedding-3-large** → anota el nombre exacto → `AzureOpenAI__EmbeddingDeploymentName`

**Nota**: Si los deployments no existen, créalos en Azure OpenAI Studio (T-06 de F0-2).

---

## 5. Azure Storage Queues

En Fase 1 se usan **Azure Storage Queues** (mismo Storage Account que Blob). La connection string es la misma que `AzureBlob__ConnectionString`.

Las colas `queue-crawler`, `queue-parser`, `queue-enrichment`, `queue-indexer` se crean en T-07 si no existen. No requieren credenciales adicionales.

---

## 6. Azure App Service (API)

Las variables de entorno de la API son la **unión** de todas las anteriores más las de Entra ID. En Fase 1, Entra ID puede diferirse; en ese caso, las variables `AzureAd__*` pueden omitirse o usar valores placeholder para autenticación simulada.

| Variable | Origen |
|---|---|
| `AzureSql__ConnectionString` | Sección 1 |
| `AzureBlob__ConnectionString` | Sección 2 |
| `AzureBlob__ContainerName` | Sección 2 |
| `AzureSearch__*` | Sección 3 |
| `AzureOpenAI__*` | Sección 4 |
| `AzureAd__TenantId` | Sección 7 (Fase 3) |
| `AzureAd__ClientId` | Sección 7 (Fase 3) |
| `AzureAd__Audience` | Sección 7 (Fase 3) |

**Configuración en App Service**: App Service → **Settings** → **Configuration** → **Application settings** → agregar cada variable.

---

## 7. Azure Entra ID (Fase 3)

En Fase 1 la autenticación puede ser simulada. Para Fase 3 (Entra ID):

| Variable | Descripción |
|---|---|
| `AzureAd__TenantId` | Tenant ID del directorio (Microsoft 365 del estudio) |
| `AzureAd__ClientId` | Client ID de la app registrada |
| `AzureAd__Audience` | Audience del API (ej: `api://legal-ai-ar`) |

### Cómo obtener

1. En Azure Portal → **Microsoft Entra ID** → **Overview** → copia **Tenant ID**
2. **App registrations** → selecciona la app del proyecto (o créala) → **Overview** → copia **Application (client) ID**
3. **Expose an API** → define el **Application ID URI** (ej: `api://legal-ai-ar`) → ese valor es la audience

---

## 8. Azure Static Web Apps

No requiere variables de entorno propias en el backend. La configuración relevante es:

- **Build**: carpeta `/frontend`, framework Angular
- **Redirect URIs**: si usas Entra ID, agregar la URL del SPA desplegado a la app registration

---

## 9. Azure Container Apps (Workers)

Each worker has a subset of variables per `docs/design/f0-2-environment-variables.md`:

| Worker | Variables necesarias |
|---|---|
| **crawler-worker** | AzureSql, AzureBlob (ConnectionString, ContainerName) |
| **parser-worker** | AzureBlob (ConnectionString, ContainerName) |
| **enrichment-worker** | AzureBlob, AzureOpenAI (Endpoint, ApiKey, ChatDeploymentName) |
| **indexer-worker** | AzureSql, AzureBlob, AzureOpenAI (Endpoint, ApiKey, EmbeddingDeploymentName), AzureSearch (todas) |

**Configuración**: Container App → **Settings** → **Secrets** (para valores sensibles) y **Environment variables** (referenciando secrets o valores no sensibles).

---

## Checklist de obtención

| # | Servicio | Variable(s) | ✓ |
|---|---|---|---|
| 1 | Azure SQL | `AzureSql__ConnectionString` | |
| 2 | Blob Storage | `AzureBlob__ConnectionString`, `AzureBlob__ContainerName` | |
| 3 | AI Search | `AzureSearch__Endpoint`, `AzureSearch__ApiKey`, `AzureSearch__RulingIndexName`, `AzureSearch__ChunkIndexName` | |
| 4 | Azure OpenAI | `AzureOpenAI__Endpoint`, `AzureOpenAI__ApiKey`, `AzureOpenAI__ChatDeploymentName`, `AzureOpenAI__EmbeddingDeploymentName` | |
| 5 | Storage Queues | (usa `AzureBlob__ConnectionString`) | |
| 6 | Entra ID (Fase 3) | `AzureAd__TenantId`, `AzureAd__ClientId`, `AzureAd__Audience` | |

---

## Siguiente paso

Una vez obtenidas las credenciales:

1. Crear archivo `.env` en la raíz (o en `backend/` según convención del proyecto) con los valores. **No commitear** `.env` (debe estar en `.gitignore`)
2. Usar `.env.example` como plantilla (E017) con placeholders en lugar de valores reales
3. Continuar con T-02 (EF Core migrations) y T-03 (seed de Sources y CrawlerConfigs)

---

## Referencias

- `docs/design/f0-2-infrastructure.md` — Azure resources catalog
- `docs/design/f0-2-environment-variables.md` — Variables matrix per component
- `docs/roadmap/ROADMAP.md` — F0-2, tareas T-01 a T-11
