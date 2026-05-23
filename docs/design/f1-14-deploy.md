# Deploy — Phase 1 Strategy

| Field | Value |
|---|---|
| **ID** | E120 |
| **Feature** | F1-14 · Deploy — Fase 1 |
| **Date** | 2026-03-10 |

---

## Purpose

This document specifies the deployment strategy for Phase 1: branching model, staging/production slots, rollback, and environment variables per environment. It serves as the design reference for T-01 to T-08 and is consumed by DevOps and developers.

**Reference**: Architecture section 7 (Azure infrastructure); E010 (environment variables); F0-2 T-08 (deferred deploy).

---

## 1. Branching Model

| Branch | Purpose |
|---|---|
| `main` | Production-ready. Triggers CD deploy to staging. |
| `develop` | Integration branch. Optional. Feature branches merge here first. |
| `feature/*` | Feature branches. Merge to `develop` or `main` via PR. |

**Phase 1**: Simple model. `main` is the canonical branch. All PRs merge to `main`. CD triggers on merge to `main`.

**Optional**: Use `develop` for integration; promote to `main` for release. Phase 1 can start with `main`-only.

---

## 2. Deployment Slots

### 2.1 API — Azure App Service

| Slot | URL | Purpose |
|---|---|---|
| **staging** | `legal-ai-ar-api-staging.azurewebsites.net` | Pre-production validation |
| **production** | `legal-ai-ar-api.azurewebsites.net` | Live API |

**Flow**: Deploy to staging first. Run smoke test. Swap staging ↔ production (or promote via pipeline).

**Swap**: Azure App Service slot swap. Zero-downtime. Staging becomes production; previous production becomes staging.

### 2.2 SPA — Azure Static Web Apps

| Environment | URL | Purpose |
|---|---|---|
| **staging** | `legal-ai-ar-staging.azurestaticapps.net` | Preview. PR or branch deploy. |
| **production** | `legal-ai-ar.azurestaticapps.net` | Live SPA |

**Flow**: Staging deploys from `main` (or staging branch). Production deploys from `main` after smoke test. Static Web Apps supports branch-based environments.

### 2.3 Workers — Azure Container Apps

| Environment | Purpose |
|---|---|
| **staging** | Same Container Apps environment; different revision or separate environment. Phase 1: may share with production (single environment) for simplicity. |
| **production** | Live workers. |

**Phase 1**: Single Container Apps environment. Staging and production may share workers (same queues, same DB). Risk: staging crawl can affect production data. Mitigation: use separate Blob container or DB schema for staging if needed. **Simpler**: single environment; staging API + SPA point to same backend; smoke test uses real data.

---

## 3. Rollback Strategy

### 3.1 API (App Service)

| Method | Action |
|---|---|
| **Slot swap** | Swap back: production ↔ staging. Previous production (now in staging) becomes live again. |
| **Redeploy** | Trigger pipeline to deploy previous commit. Manual or automated. |

### 3.2 SPA (Static Web Apps)

| Method | Action |
|---|---|
| **Redeploy** | Deploy previous build from pipeline history. |
| **Rollback** | Static Web Apps keeps deployment history. Restore previous deployment from Azure Portal or CLI. |

### 3.3 Workers (Container Apps)

| Method | Action |
|---|---|
| **Revision rollback** | Container Apps supports multiple revisions. Activate previous revision. |
| **Redeploy** | Push previous image tag. Update Container App to use it. |

### 3.4 Database

**No automatic rollback**. Migrations are forward-only. Rollback requires manual migration (down migration) if supported. Phase 1: avoid destructive migrations; prefer additive changes.

---

## 4. Environment Variables by Environment

### 4.1 Local (Development)

| Source | Notes |
|---|---|
| `.env` | Loaded by `dotenv` or `appsettings.Development.json`. Not committed. |
| `appsettings.Development.json` | Placeholders. Override with env vars. |

### 4.2 Staging

| Component | Config Source |
|---|---|
| **API** | App Service staging slot → Application settings. Same variables as E010; values point to staging resources (or shared). |
| **SPA** | Build-time env (e.g. `VITE_API_URL` or Angular `environment.staging.ts`). API URL = staging API. |
| **Workers** | Container Apps staging (if separate) or shared. Connection strings to shared Azure resources. |

**Staging vs Production**: Use same Azure SQL, Blob, Search, OpenAI for Phase 1 (cost). Optionally: separate Blob container (`rulings-pdfs-staging`), separate DB, or separate resource group for full isolation. Phase 1: shared resources acceptable.

### 4.3 Production

| Component | Config Source |
|---|---|
| **API** | App Service production slot → Application settings. Secrets from Key Vault or App Service managed identity. |
| **SPA** | Build-time env. API URL = production API. |
| **Workers** | Container Apps. Same as staging if shared; otherwise production-specific. |

### 4.4 Variables That Differ by Environment

| Variable | Staging | Production |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Staging` | `Production` |
| `Auth__Simulated__SigningKey` | Dev key | Prod key (different, rotated) |
| SPA `apiUrl` | Staging API URL | Production API URL |
| `CORS__AllowedOrigins` | Staging SPA URL | Production SPA URL |

**Secrets**: Never in repo. Use Azure Key Vault references in App Service (e.g. `@Microsoft.KeyVault(SecretUri=...)`) or App Service Application settings (encrypted at rest).

---

## 5. CD Pipeline Overview

1. **Trigger**: Merge to `main`.
2. **Build**: API, SPA, 4 workers. Run tests.
3. **Push**: Container images to Azure Container Registry.
4. **Deploy API**: To App Service staging slot.
5. **Deploy SPA**: To Static Web Apps staging.
6. **Deploy Workers**: To Container Apps (staging or shared).
7. **Smoke test**: Crawl → index → search → chat. Automated or manual.
8. **Promote**: Swap API slots; promote SPA to production; activate worker revision.

**Detail**: E121 (CD pipeline diagram).

---

## 6. References

- `docs/architecture/legal-ai-ar-architecture.md` — section 7 (Azure infrastructure)
- `docs/design/f0-2-environment-variables.md` — variable matrix (E010)
- `docs/design/f1-14-cd-pipeline.mermaid` — CD pipeline diagram (E121)
