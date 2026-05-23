# Verificación de conectividad Azure — Legal AI AR

| Campo | Valor |
|---|---|
| **Feature** | F0-2 · Infraestructura Azure |
| **Tarea** | T-10 |
| **Fecha** | 2026-03-10 |

---

## Propósito

Este documento describe cómo verificar la conectividad a todos los servicios Azure utilizados por Legal AI AR. Sirve para validar que la configuración (`.env` o variables de entorno) es correcta antes de ejecutar la API, workers o pipelines.

**Servicios verificados:**

| Servicio | Qué se comprueba |
|---|---|
| Azure SQL | Conexión exitosa con el connection string |
| Azure Blob Storage | Container configurado existe |
| Storage Queues | Las 4 colas `csjn-ruling-*` existen |
| Azure AI Search | Índices `rulings-by-ruling` y `rulings-by-chunk` existen |
| Azure OpenAI | Deployments gpt-4o y text-embedding-3-large responden |

---

## 1. Verificación automática

### Requisitos

- `.env` configurado en la raíz del repositorio
- `dotnet` SDK 8.0 (para el tool LegalAiAr.VerifyConnectivity)
- Azure CLI (opcional, no requerido — el tool .NET hace las verificaciones)

### Ejecución

Desde la raíz del repositorio:

```powershell
.\infra\scripts\verify-azure-connectivity.ps1
```

Opciones:

```powershell
# Omitir Azure OpenAI (más rápido, evita llamadas a la API)
.\infra\scripts\verify-azure-connectivity.ps1 -SkipOpenAI

# Ruta explícita al .env
.\infra\scripts\verify-azure-connectivity.ps1 -EnvPath "C:\ruta\a\.env"
```

### Resultado esperado

**Éxito:**
```
=== Verificación de conectividad Azure (F0-2 T-10) ===

Cargando variables desde: C:\...\legal-ai-ar\.env
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

**Fallo:** El script mostrará el servicio que falló y el mensaje de error. Sale con código 1.

---

## 2. Verificación individual

### Azure SQL

Usar el connection string de `.env` en un cliente (SSMS, Azure Data Studio, sqlcmd) o ejecutar migraciones:

```powershell
.\scripts\load-env.ps1
cd backend
dotnet ef database update --project src/shared/LegalAiAr.Infrastructure --startup-project src/api/LegalAiAr.Api
```

### Azure Blob y Storage Queues

```powershell
.\infra\scripts\create-storage-queues.ps1   # Crea colas si faltan
# Container: verificar en Azure Portal o con Azure CLI
az storage container exists -n legalaiar-dev --connection-string $env:AzureBlob__ConnectionString
```

### Azure AI Search

```powershell
.\scripts\run-setup-search.ps1   # Crea índices si faltan
```

### Azure OpenAI

```powershell
.\infra\scripts\verify-azure-openai.ps1
```

---

## 3. Tool .NET (uso directo)

El script invoca internamente `LegalAiAr.VerifyConnectivity`. Se puede ejecutar directamente:

```powershell
.\scripts\load-env.ps1
cd backend
dotnet run --project src/tools/LegalAiAr.VerifyConnectivity
```

Salida en formato `Servicio|OK|Mensaje` o `Servicio|FAIL|Mensaje`. Exit code 0 si todo OK, 1 si hay fallos.

---

## Referencias

- `docs/design/f0-2-environment-variables.md` — variables per service
- `docs/setup/azure-credentials-guide.md` — cómo obtener credenciales
- `docs/setup/azure-openai-verification.md` — verificación detallada de OpenAI
