# FT05 - W03 - GCaaS - Helm Chart and Knative Deployment

> **Feature:** FT05 - Delivery and Hosting
> **Release:** Cross-cutting | **Sprint:** S01-S02
> **Type:** devops | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Backend Dev / DevOps

---

## Description

GCaaS (PwC's container platform) hosts Legal Ai Ar for internal users via a Helm chart under `mvp/deployment/`. The API and SPA run as Knative services behind Istio; images are built by the GCaaS platform (`experimentalBuild: true`), not by GitHub Actions. This work item covers the chart, Knative services, and Istio routing.

---

## Tasks

### Helm chart (`mvp/deployment/`)

- [ ] `Chart.yaml` — chart metadata (`legal-ai`)
- [ ] `values.yaml` — apps, auth flag, secret keys, configMap, metadata placeholders
- [ ] `templates/ksvc.yaml` — Knative Service + Istio VirtualServices
- [ ] `templates/configmap.yaml` — non-secret env injection
- [ ] `templates/secrets.yaml` — Vault secret references
- [ ] `templates/daemonset.yaml` — optional image preloader

### Applications

- [ ] `backend` — `legal-ai-api` (.NET, port 8080), ingress enabled
- [ ] `frontend` — `legal-ai-ui` (Angular, port 8081), `minScale: 1`
- [ ] Workers (`crawler`, `parser`, …) — keep commented (future/optional on GCaaS)

### Istio routing and Entra ingress

- [ ] Default Knative gateway on `metadata.hostName` (legacy host)
- [ ] When `authentication.entra: true` and `metadata.entraHostName` is set, emit a second VirtualService (`*-vs-entra`) bound to `istio-system/entra-ingress-gateway`
- [ ] Validate platform-injected `runtimeReplaced` metadata: `engagementId`, `hostName`, `entraHostName`, `commitHash`

---

## Runtime metadata (platform-injected)

| Key | Purpose |
|-----|---------|
| `metadata.engagementId` | Engagement UUID in URLs and config |
| `metadata.hostName` | Legacy ingress host |
| `metadata.entraHostName` | Entra ingress host (e.g. `global-caas-us.pwcglb.com`) |
| `metadata.commitHash` | Knative revision suffix |

URL pattern: `https://{entraHostName}/{engagementId}/{releaseName}-{appName}/`

---

## Acceptance Criteria

- [ ] `helm template` renders valid Knative + Istio manifests
- [ ] API and SPA reachable on both the legacy and Entra hosts
- [ ] `*-vs-entra` VirtualService present when `authentication.entra: true`
- [ ] Entry point renders under 63 characters after templating

---

## Dependencies

- **Blocks:** FT05-W05 (Vault secrets), FT05-W06 (verification)
- **Prerequisites:** FT05-W04 (platform auth flag), GCaaS engagement provisioned

---

*FT05 - W03 - GCaaS - Helm Chart and Knative Deployment — Legal Ai Ar*
