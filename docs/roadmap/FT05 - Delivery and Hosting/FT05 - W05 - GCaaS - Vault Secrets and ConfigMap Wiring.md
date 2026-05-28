# FT05 - W05 - GCaaS - Vault Secrets and ConfigMap Wiring

> **Feature:** FT05 - Delivery and Hosting
> **Release:** Cross-cutting | **Sprint:** S01-S02
> **Type:** devops | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Backend Dev / DevOps

---

## Description

On GCaaS, secrets come from HashiCorp Vault (mapped into the release via `wrappingReplaced` placeholders in `values.yaml`), and non-secret configuration comes from a ConfigMap. This work item ensures the required Vault keys exist and that the ConfigMap wires the Azure endpoints, model deployment names, and engagement/backend URLs.

---

## Tasks

### Vault secret keys (`templates/secrets.yaml`)

- [ ] `AzureSql__ConnectionString` — database
- [ ] `AzureBlob__ConnectionString` — blob storage
- [ ] `AzureSearch__ApiKey` — AI Search
- [ ] `AzureOpenAI__ApiKey` — OpenAI
- [ ] `Auth__Platform__TenantId` — Entra tenant for `id_token` validation
- [ ] `Auth__Platform__ValidAudience` — App Registration client id (`aud` claim)

### ConfigMap (`templates/configmap.yaml`)

- [ ] Azure endpoints and model deployment names (non-secret)
- [ ] `ENGAGEMENT_ID`
- [ ] `BACKEND_ROOT_URL` and `BACKEND_ROOT_URL_ENTRA`

### Verification

- [ ] Confirm every key in `values.yaml` `secrets:` exists in Vault
- [ ] Confirm `.env.example` documents the `Auth__Platform__*` variable names for local parity

---

## Acceptance Criteria

- [ ] Release renders with all Vault secret references resolved
- [ ] API reads connection strings and `Auth:Platform` settings from the injected secrets
- [ ] ConfigMap provides the correct Entra/legacy backend URLs per host
- [ ] No secret values are committed to the repo or appsettings

---

## Dependencies

- **Blocks:** FT05-W04 (auth needs `TenantId`/`ValidAudience`), FT05-W06 (verification)
- **Prerequisites:** FT05-W03 (chart structure), Vault access for the engagement

---

*FT05 - W05 - GCaaS - Vault Secrets and ConfigMap Wiring — Legal Ai Ar*
