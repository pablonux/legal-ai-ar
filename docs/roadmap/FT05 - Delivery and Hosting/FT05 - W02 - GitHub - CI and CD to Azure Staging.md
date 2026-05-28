# FT05 - W02 - GitHub - CI and CD to Azure Staging

> **Feature:** FT05 - Delivery and Hosting
> **Release:** Cross-cutting | **Sprint:** S00-S01
> **Type:** devops | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Backend Dev / DevOps

---

## Description

GitHub is the canonical source of truth. Two workflows run on GitHub-hosted runners: **CI** (build, test, format check — no deploy) and **CD** (build artifacts and deploy API + SPA to Azure staging). GitHub Actions does NOT deploy to GCaaS.

The MVP already implements both workflows; the remaining work is hardening, the `staging` environment protection, and aligning the implemented `cd.yml` (API + SPA staging only) with the broader target flow described in `docs/deployment/github-delivery.md` §5.

---

## Tasks

### CI (`ci.yml`)

- [ ] Triggers: `push` and `pull_request` to `main`; runner `ubuntu-latest`
- [ ] Steps: checkout, setup .NET `10.0.x`, restore, build (Release), `dotnet test`, `dotnet format --verify-no-changes`
- [ ] Require CI to pass before merge to `main` (branch protection)

### CD (`cd.yml`)

- [ ] Trigger: `push` to `main`; deploy jobs gated on `github.ref == 'refs/heads/main'` and the `staging` environment
- [ ] `build-and-test` job: publish `LegalAiAr.Api` and upload `api-publish` artifact
- [ ] `build-spa` job: Node 20, `npm ci`, `npm run build -- --configuration=staging`, upload `spa-dist`
- [ ] `deploy-api` job: `azure/login` (`AZURE_CREDENTIALS`) → `azure/webapps-deploy` to App Service `staging` slot
- [ ] `deploy-spa` job: `Azure/static-web-apps-deploy` with `AZURE_STATIC_WEB_APPS_API_TOKEN`

### Configuration

- [ ] GitHub secrets: `AZURE_CREDENTIALS`, `AZURE_STATIC_WEB_APPS_API_TOKEN`; optional var `APP_SERVICE_NAME`
- [ ] GitHub environment `staging` (optional required reviewers)
- [ ] Document reserved container secrets (`ACR_*`) for future worker image push

---

## Secrets and environments

| Name | Type | Used by | Purpose |
|------|------|---------|---------|
| `AZURE_CREDENTIALS` | Secret | `deploy-api` | Service principal JSON for `azure/login` |
| `AZURE_STATIC_WEB_APPS_API_TOKEN` | Secret | `deploy-spa` | Static Web Apps deployment token |
| `APP_SERVICE_NAME` | Variable (optional) | `deploy-api` | Override default `legal-ai-ar-api` |

---

## Acceptance Criteria

- [ ] CI passes (build, tests, format) on every PR and blocks merge on failure
- [ ] Merge to `main` deploys API to the App Service staging slot and SPA to Static Web Apps
- [ ] Staging SPA build uses `environment.staging.ts` (`usePlatformCredentials: false`)
- [ ] Rollback documented (workflow re-run or slot swap per `docs/deployment/github-delivery.md` §9)

---

## Dependencies

- **Blocks:** Production promotion (out of scope of current `cd.yml`)
- **Prerequisites:** Azure App Service (staging slot) and Static Web App provisioned (`mvp/infra/scripts/*.ps1`)

---

*FT05 - W02 - GitHub - CI and CD to Azure Staging — Legal Ai Ar*
