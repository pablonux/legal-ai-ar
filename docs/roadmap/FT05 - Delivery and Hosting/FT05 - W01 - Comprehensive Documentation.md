# FT05 - W01 - Comprehensive Documentation

> **Feature:** FT05 - Delivery and Hosting
> **Release:** Cross-cutting | **Sprint:** S00 (ongoing)
> **Type:** docs | **Priority:** High
> **Estimate:** 3 story points
> **Assignable to:** Tech Lead

---

## Description

Consolidate the delivery and hosting reference for Legal Ai Ar. The project uses two complementary paths that may share the same Azure data services but differ in compute and identity:

1. **GitHub → Azure staging** — GitHub Actions (`ci.yml`, `cd.yml`) build and deploy the API (App Service staging slot) and the SPA (Azure Static Web Apps). GitHub Actions does NOT deploy to GCaaS.
2. **GCaaS (corporate production)** — PwC's Kubernetes platform hosts the API and SPA as Knative services behind Istio, with Microsoft Entra SSO (`id_token` cookie), HashiCorp Vault secrets, and a Helm chart in `deployment/`.

This work item keeps the canonical references current and linked from the plan.

---

## Tasks

### Documentation

- [ ] Keep `docs/deployment/github-delivery.md` accurate (CI/CD, secrets, environments, Azure targets)
- [ ] Keep `docs/deployment/gcaas-hosting.md` accurate (Helm, Knative, Istio, Vault, session model)
- [ ] Ensure `features.md` §2.3 (Deployment and Hosting Model) and the §10 stack tables match the docs
- [ ] Ensure F1.1 (Authentication) reflects the platform `id_token` cookie model (no MSAL)
- [ ] Cross-link from `README.md` (Delivery & Hosting) and `CLAUDE.md` / `.cursor/rules/project.mdc`

---

## Reference documents

| Document | Scope |
|----------|-------|
| `docs/deployment/github-delivery.md` | GitHub branching, CI/CD workflows, secrets, Azure staging targets |
| `docs/deployment/gcaas-hosting.md` | GCaaS Knative/Istio/Helm deploy, Entra `id_token` auth, Vault, session lifecycle |
| `deployment/` | Helm chart (`Chart.yaml`, `values.yaml`, templates) |
| `.github/workflows/ci.yml`, `cd.yml` | CI and CD pipelines |

---

## Acceptance Criteria

- [ ] Both usage docs are linked from `README.md`, `features.md`, and the AI config rules
- [ ] The roadmap auth model matches `docs/deployment/gcaas-hosting.md` (no MSAL, `id_token` cookie)
- [ ] The §2.3 dual-path table matches the actual workflows and Helm chart
- [ ] No contradictory hosting/auth statements remain in the plan

---

## Dependencies

- **Blocks:** FT05-W02..W06 (reference the documented model)
- **Prerequisites:** None — canonical delivery/hosting docs live in `docs/deployment/` (content originated from the MVP design docs and is now self-contained)

---

*FT05 - W01 - Comprehensive Documentation — Legal Ai Ar*
