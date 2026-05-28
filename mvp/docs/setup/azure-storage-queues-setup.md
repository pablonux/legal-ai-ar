# Creación de colas Storage Queues — Legal AI AR

| Campo | Valor |
|---|---|
| **Feature** | F0-2 · Infraestructura Azure |
| **Tarea** | T-07 |
| **Fecha** | 2026-03-10 |

---

## Propósito

Este documento describe cómo crear las colas de Azure Storage Queues para el pipeline de ingesta. Las colas se crean en el **mismo Storage Account** que Blob Storage; la connection string es `AzureBlob__ConnectionString`.

**Convención de nombres:** `{source}-{class}-{stage}` (configurable via `Pipeline__QueuePrefix`, default `csjn-ruling`).

**Colas requeridas:**

| Cola | Propósito |
|---|---|
| `csjn-ruling-crawler` | Mensajes de trigger para crawler (manual o cron) |
| `csjn-ruling-parser` | Documentos a parsear (PDF + metadata) |
| `csjn-ruling-enrichment` | Documentos a enriquecer con GPT-4o |
| `csjn-ruling-indexer` | Documentos a indexar en la Knowledge Base |

**Colas DLQ (mensajes fallidos):**

| Cola | Propósito |
|---|---|
| `csjn-ruling-crawler-dlq` | Mensajes fallidos tras 3 reintentos en crawler |
| `csjn-ruling-parser-dlq` | Mensajes fallidos en parser |
| `csjn-ruling-enrichment-dlq` | Mensajes fallidos en enrichment |
| `csjn-ruling-indexer-dlq` | Mensajes fallidos en indexer |

---

## 1. Creación automática (script)

### Requisitos

- **Az.Storage** (PowerShell) o **Azure CLI** instalado
- Archivo `.env` con `AzureBlob__ConnectionString` configurado

### Ejecución

Desde la raíz del repositorio:

```powershell
.\infra\scripts\create-storage-queues.ps1
```

Opciones:

```powershell
# Solo colas principales (sin DLQ)
.\infra\scripts\create-storage-queues.ps1 -SkipDlq

# Ruta explícita al .env
.\infra\scripts\create-storage-queues.ps1 -EnvPath "C:\ruta\a\.env"
```

### Resultado esperado

```
=== Crear colas Storage Queues (F0-2 T-07) ===

Cargando variables desde: C:\...\legal-ai-ar\.env
Storage Account conectado.

Colas principales:
  csjn-ruling-crawler (creada)
  csjn-ruling-parser (creada)
  csjn-ruling-enrichment (creada)
  csjn-ruling-indexer (creada)

Colas DLQ (mensajes fallidos):
  csjn-ruling-crawler-dlq (creada)
  csjn-ruling-parser-dlq (creada)
  csjn-ruling-enrichment-dlq (creada)
  csjn-ruling-indexer-dlq (creada)

=== Completado: 8 cola(s) creada(s) ===
```

Si una cola ya existe, el script muestra `(ya existe)` y continúa (idempotente).

### Instalación de dependencias

**Opción A — Az.Storage (PowerShell):**
```powershell
Install-Module -Name Az.Storage -Scope CurrentUser -Force
```

**Opción B — Azure CLI:**
Descargar e instalar desde [https://aka.ms/install-azure-cli](https://aka.ms/install-azure-cli)

---

## 2. Creación manual (Azure Portal)

1. Inicia sesión en [Azure Portal](https://portal.azure.com)
2. Busca **Storage accounts** → selecciona el account del proyecto
3. En el menú izquierdo: **Data storage** → **Queues**
4. Clic en **+ Queue**
5. Crear cada cola con el nombre exacto: `csjn-ruling-crawler`, `csjn-ruling-parser`, `csjn-ruling-enrichment`, `csjn-ruling-indexer`
6. Repetir para las DLQ: `csjn-ruling-crawler-dlq`, `csjn-ruling-parser-dlq`, `csjn-ruling-enrichment-dlq`, `csjn-ruling-indexer-dlq`

**Nota:** Los nombres de cola son case-sensitive y deben coincidir exactamente.

---

## 3. Creación manual (Azure CLI)

Con la connection string en variable de entorno:

```powershell
$env:AZURE_STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net"

az storage queue create -n csjn-ruling-crawler
az storage queue create -n csjn-ruling-parser
az storage queue create -n csjn-ruling-enrichment
az storage queue create -n csjn-ruling-indexer
az storage queue create -n csjn-ruling-crawler-dlq
az storage queue create -n csjn-ruling-parser-dlq
az storage queue create -n csjn-ruling-enrichment-dlq
az storage queue create -n csjn-ruling-indexer-dlq
```

O pasando `--connection-string` en cada comando:

```powershell
az storage queue create -n csjn-ruling-crawler --connection-string $env:AzureBlob__ConnectionString
```

---

## 4. Verificación

Comprobar que las colas existen:

```powershell
az storage queue list --connection-string $env:AzureBlob__ConnectionString -o table
```

O en Azure Portal: Storage account → Queues → verificar que aparecen las 8 colas.

---

## 5. Configuración de retry (referencia)

La arquitectura define (ADR-009):

| Parámetro | Valor |
|---|---|
| Max delivery count | 3 intentos |
| Lock duration | 5 minutos |
| Message TTL | 7 días |

Tras 3 intentos fallidos, el worker mueve el mensaje a la cola DLQ correspondiente. Los workers implementan esta lógica en código (Storage Queues no tiene DLQ nativa como Service Bus).

---

## Referencias

- `docs/architecture/legal-ai-ar-architecture.md` — section 8 (Messaging)
- `docs/design/f0-2-infrastructure.md` — section 2.4 (Azure Storage Queues)
- `docs/setup/azure-credentials-guide.md` — sección 5 (Storage Queues)
