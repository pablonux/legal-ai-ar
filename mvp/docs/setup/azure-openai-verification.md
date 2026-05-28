# Verificación de deployments Azure OpenAI — Legal AI AR

| Campo | Valor |
|---|---|
| **Feature** | F0-2 · Infraestructura Azure |
| **Tarea** | T-06 |
| **Fecha** | 2026-03-10 |

---

## Propósito

Este documento describe cómo verificar que los deployments de Azure OpenAI requeridos por Legal AI AR existan y respondan correctamente:

- **gpt-4o** (chat): usado por EnrichmentWorker y API (chat RAG)
- **text-embedding-3-large** (embeddings): usado por IndexerWorker y API (búsqueda semántica)

Si los deployments no existen o están mal configurados, el pipeline de ingesta y la búsqueda fallarán.

---

## 1. Verificación automática (script)

### Requisitos

- PowerShell 5.1 o superior (incluido en Windows)
- Archivo `.env` en la raíz del repositorio con las variables de Azure OpenAI configuradas, o variables de entorno exportadas

### Ejecución

Desde la raíz del repositorio:

```powershell
.\infra\scripts\verify-azure-openai.ps1
```

O especificando la ruta al `.env`:

```powershell
.\infra\scripts\verify-azure-openai.ps1 -EnvPath "C:\ruta\a\.env"
```

### Resultado esperado

**Éxito:**
```
=== Verificación de Azure OpenAI deployments (F0-2 T-06) ===

Cargando variables desde: C:\...\legal-ai-ar\.env
Endpoint: https://...
Chat deployment: azure.gpt-4o
Embedding deployment: azure.text-embedding-3-large

Chat (gpt-4o): OK
Embeddings (text-embedding-3-large): OK

=== Verificación completada: todos los deployments están operativos ===
```

**Fallo:** El script mostrará el error específico (HTTP status, mensaje de la API) y saldrá con código 1.

### Variables utilizadas

| Variable | Descripción |
|---|---|
| `AzureOpenAI__Endpoint` | URL del recurso (ej: `https://legal-ai-openai.openai.azure.com/`) |
| `AzureOpenAI__ApiKey` | API key del recurso |
| `AzureOpenAI__ChatDeploymentName` | Nombre del deployment de gpt-4o |
| `AzureOpenAI__EmbeddingDeploymentName` | Nombre del deployment de text-embedding-3-large |

---

## 2. Verificación manual (Azure Portal / Azure OpenAI Studio)

Si el script falla o prefieres verificar manualmente:

### 2.1 Azure Portal

1. Inicia sesión en [Azure Portal](https://portal.azure.com)
2. Busca **Azure OpenAI** y selecciona el recurso del proyecto
3. En el menú izquierdo: **Resource management** → **Model deployments** (o **Deployments**)
4. Verifica que existan:
   - Un deployment de **gpt-4o** (modelo de chat)
   - Un deployment de **text-embedding-3-large** (modelo de embeddings)
5. Anota los **nombres exactos** de cada deployment — deben coincidir con `AzureOpenAI__ChatDeploymentName` y `AzureOpenAI__EmbeddingDeploymentName` en `.env`

### 2.2 Azure OpenAI Studio

1. Accede a [Azure OpenAI Studio](https://oai.azure.com/)
2. Selecciona el recurso y el deployment correspondiente
3. En **Playground** → **Chat**, prueba una consulta simple con el deployment de gpt-4o
4. Para embeddings, usa la pestaña **Embeddings** (si está disponible) o valida vía API

### 2.3 Crear deployments si no existen

Si los deployments no existen:

1. En Azure OpenAI Studio → **Deployments** → **Create new deployment**
2. Para **gpt-4o**:
   - Model: `gpt-4o`
   - Deployment name: elegir un nombre (ej: `gpt-4o` o `azure.gpt-4o`)
   - Actualizar `AzureOpenAI__ChatDeploymentName` en `.env`
3. Para **text-embedding-3-large**:
   - Model: `text-embedding-3-large`
   - Deployment name: elegir un nombre (ej: `text-embedding-3-large` o `azure.text-embedding-3-large`)
   - Actualizar `AzureOpenAI__EmbeddingDeploymentName` en `.env`

### 2.4 Verificación con curl (opcional)

**Chat (gpt-4o):**
```bash
curl -X POST "https://{ENDPOINT}/openai/deployments/{CHAT_DEPLOYMENT}/chat/completions?api-version=2024-06-01" \
  -H "api-key: {API_KEY}" \
  -H "Content-Type: application/json" \
  -d '{"messages":[{"role":"user","content":"OK"}],"max_tokens":5}'
```

**Embeddings:**
```bash
curl -X POST "https://{ENDPOINT}/openai/deployments/{EMBEDDING_DEPLOYMENT}/embeddings?api-version=2024-06-01" \
  -H "api-key: {API_KEY}" \
  -H "Content-Type: application/json" \
  -d '{"input":"test"}'
```

Reemplazar `{ENDPOINT}`, `{API_KEY}`, `{CHAT_DEPLOYMENT}` y `{EMBEDDING_DEPLOYMENT}` por los valores de `.env`.

---

## 3. Errores comunes

| Error | Causa probable | Solución |
|---|---|---|
| `401 Unauthorized` | API key inválida o expirada | Verificar `AzureOpenAI__ApiKey` en Azure Portal → Keys and Endpoint |
| `404 Not Found` | Deployment no existe o nombre incorrecto | Crear deployment en Azure OpenAI Studio o corregir el nombre en `.env` |
| `429 Too Many Requests` | Rate limit excedido | Esperar y reintentar; reducir frecuencia de llamadas |
| `Endpoint no responde` | URL incorrecta o recurso no accesible | Verificar `AzureOpenAI__Endpoint`, firewall y conectividad de red |

---

## Referencias

- `docs/design/f0-2-infrastructure.md` — section 2.5 (Azure OpenAI Service)
- `docs/design/f0-2-environment-variables.md` — section 3.2 (Azure OpenAI)
- `docs/setup/azure-credentials-guide.md` — sección 4 (cómo obtener credenciales)
