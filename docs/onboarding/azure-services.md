# Azure Services — Credentials, Variables & Verification

> How to obtain the credentials for the shared **cloud DEV** services, which environment variable each
> component needs, and how to verify connectivity. All environments (DEV and beyond) use the same
> cloud services — there is no local stack (see the [onboarding hub](README.md)).
>
> **Last updated:** 2026-05-28

---

## 1. What you need and where to get it

You don't create Azure resources — you collect the values for an existing DEV environment and put them
in `.env` (`cp .env.example .env` from the repo root). Endpoints are pre-filled in the template; you only
replace the secret placeholders. Ask the Tech Lead for access to the DEV resources.

| Service | Non-secret values (already in `.env.example`) | Secret to replace |
|---------|-----------------------------------------------|-------------------|
| **Azure SQL** | server `…database.windows.net`, DB `LegalAI-DEV`, user `sqladmin` | `DB_SECRET` — `sqladmin` password |
| **Blob Storage** | account name, container `legalaiar-kb-dev` | `STORAGE_KEY` — Storage account access key |
| **AI Search** | endpoint URL, index names | `SEARCH_KEY` — Search **admin** key |
| **Azure OpenAI** | endpoint, deployments `azure.gpt-4o`, `azure.text-embedding-3-large` | `OPENAI_KEY` — Azure OpenAI key |

### Where in the Azure Portal

- **Azure SQL** → SQL databases → *(the DEV DB)* → Settings → **Connection strings** → ADO.NET; replace the password.
- **Blob / Queues** (same Storage account) → Security + networking → **Access keys** → Connection string (`AccountKey`).
- **AI Search** → Settings → **Keys** → Primary admin key; endpoint is on **Overview**.
- **Azure OpenAI** → Resource management → **Keys and Endpoint**; deployment names under **Model deployments** (Azure OpenAI Studio).

> Blob Storage **and** Storage Queues share one account, so `AzureBlob__ConnectionString` serves both.

---

## 2. Environment variable matrix

The API needs the union of all variables. Each worker needs only a subset:

| Variable | API | Crawler | Parser | Enrichment | Indexer |
|----------|:---:|:-------:|:------:|:----------:|:-------:|
| `AzureSql__ConnectionString` | ✅ | ✅ | | | ✅ |
| `AzureBlob__ConnectionString` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `AzureBlob__ContainerName` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `AzureOpenAI__Endpoint` / `ApiKey` | ✅ | | | ✅ | ✅ |
| `AzureOpenAI__ChatDeploymentName` | ✅ | | | ✅ | |
| `AzureOpenAI__EmbeddingDeploymentName` | ✅ | | | | ✅ |
| `AzureSearch__*` (endpoint, key, index names) | ✅ | | | | ✅ |
| `WorkerControl__HubAccessKey` | ✅ | ✅ | ✅ | ✅ | ✅ |

> **.NET key format:** environment variables use double underscores (`AzureSql__ConnectionString` →
> `AzureSql:ConnectionString`). Never commit a filled-in `.env`.

### Authentication variables

- **Local Development:** the API auto-injects a signed `id_token` cookie (`Auth:Development:InjectIdentity = true`). Nothing to configure.
- **GCaaS / production:** platform-managed Entra SSO via the `id_token` cookie, validated with `Auth:Platform:*` (issuer, audience, signing key) sourced from Vault. See [`gcaas-hosting.md`](../deployment/gcaas-hosting.md). Do **not** configure MSAL.

---

## 3. Storage Queues

The ingestion pipeline uses Azure Storage Queues in the same Storage account as Blob.

**Naming convention:** `{source}-{class}-{stage}` (configurable via `Pipeline__QueuePrefix`, default `csjn-ruling`).

| Main queue | DLQ (failed messages) |
|------------|------------------------|
| `csjn-ruling-crawler` | `csjn-ruling-crawler-dlq` |
| `csjn-ruling-parser` | `csjn-ruling-parser-dlq` |
| `csjn-ruling-enrichment` | `csjn-ruling-enrichment-dlq` |
| `csjn-ruling-indexer` | `csjn-ruling-indexer-dlq` |

**Retry policy (per ADR-009):** max delivery count **3**, lock duration **5 min**, message TTL **7 days**.
After 3 failed attempts the worker moves the message to the matching DLQ (Storage Queues has no native
DLQ, so workers implement this in code).

Create the queues (idempotent — skips existing):

```powershell
.\infra\scripts\create-storage-queues.ps1            # 4 main + 4 DLQ
.\infra\scripts\create-storage-queues.ps1 -SkipDlq   # main queues only
```

---

## 4. Verifying connectivity

The repo ships PowerShell verification scripts under `infra/scripts/`:

```powershell
.\infra\scripts\verify-azure-connectivity.ps1            # SQL, Blob, Queues, AI Search, OpenAI
.\infra\scripts\verify-azure-connectivity.ps1 -SkipOpenAI   # faster; skips OpenAI calls
.\infra\scripts\verify-azure-openai.ps1                  # OpenAI deployments only
```

A successful run reports `OK` for each service and exits `0`; any failure prints the offending service
and exits `1`.

> **Heads-up:** `verify-azure-connectivity.ps1` invokes a .NET helper (`LegalAiAr.VerifyConnectivity`).
> That CLI tool, along with a few `scripts/*` helpers used in the MVP (`load-env.ps1`,
> `run-setup-blob.ps1`, `run-setup-search.ps1`), is **not currently in the repo** — it's backed up from
> the MVP and can be restored if needed. Until then, verify manually: open a SQL client against the
> connection string, confirm the Blob container and the 8 queues exist, check both AI Search indexes,
> and run `verify-azure-openai.ps1` for the deployments.

### Common OpenAI errors

| Error | Cause | Fix |
|-------|-------|-----|
| `401 Unauthorized` | Invalid/expired key | Re-copy `AzureOpenAI__ApiKey` from Keys and Endpoint |
| `404 Not Found` | Wrong deployment name | Match `…DeploymentName` to the actual deployment |
| `429 Too Many Requests` | Rate limit | Back off and retry; lower call frequency |

---

## 5. References

- [Onboarding hub](README.md) — full local setup
- [Troubleshooting](troubleshooting.md) — connectivity, firewall, VPN issues
- [GCaaS Hosting](../deployment/gcaas-hosting.md) — production auth, Vault secrets
- [System Architecture](../technical/10-system-architecture.md) — messaging/queues design (imported, pending review)

---

*Azure Services — Legal Ai Ar*
