# FT05 - W06 - GCaaS - Post-Deploy Verification and Rollback Runbook

> **Feature:** FT05 - Delivery and Hosting
> **Release:** Cross-cutting | **Sprint:** S02
> **Type:** devops / docs | **Priority:** Medium
> **Estimate:** 3 story points
> **Assignable to:** DevOps / Backend Dev

---

## Description

Define the post-deploy verification checklist and rollback procedure for both delivery paths. On GCaaS, verify the Entra SSO flow and session cookies; on Azure staging, verify the smoke checks documented in the staging verification tutorial.

---

## Tasks

### GCaaS verification

- [ ] Open `https://{entraHostName}/{engagementId}/{release}-frontend/`
- [ ] Complete Microsoft Entra login (stage may require `@testenv.pwc.com`)
- [ ] Confirm browser cookies: `id_token`, `access_token`, `refresh_token`
- [ ] Confirm `GET .../api/auth/me` returns 200
- [ ] Confirm the legacy `hosted-apps-*` URL still works until PWC Identity decommission
- [ ] If 503 on the Entra domain, verify the `*-vs-entra` VirtualService exists

### Azure staging verification

- [ ] Run the connectivity checks in [`docs/onboarding/azure-services.md`](../../onboarding/azure-services.md) (§4) against staging
- [ ] Confirm SPA loads with `usePlatformCredentials: false`

### Rollback

- [ ] **API (Azure)**: redeploy a previous commit via workflow re-run, or swap App Service slots (`docs/deployment/github-delivery.md` §9)
- [ ] **SPA (Azure)**: restore previous deployment in Static Web Apps or redeploy from pipeline history
- [ ] **GCaaS**: redeploy the previous Helm revision (Knative keeps prior revisions)

---

## Troubleshooting reference

| Symptom | Likely cause | Action |
|---------|--------------|--------|
| 401 on `/api/auth/me` | Missing/expired `id_token` | Re-login via `platformLoginUrl`; check refresh timer |
| 503 on Entra URL | Missing `*-vs-entra` VirtualService | Check `authentication.entra` + `entraHostName` |
| Session drops after ~1h | Refresh not running | Verify `gcaasEngagementId` and the `/refresh` call |

---

## Acceptance Criteria

- [ ] Verification checklist runs green on both paths
- [ ] Rollback steps validated at least once per path
- [ ] Runbook linked from `docs/deployment/github-delivery.md` / `gcaas-hosting.md`

---

## Dependencies

- **Blocks:** None (final step of the delivery track)
- **Prerequisites:** FT05-W02 (Azure staging), FT05-W03..W05 (GCaaS deploy + secrets)

---

*FT05 - W06 - GCaaS - Post-Deploy Verification and Rollback Runbook — Legal Ai Ar*
