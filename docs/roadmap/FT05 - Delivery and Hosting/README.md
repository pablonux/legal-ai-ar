# FT05 — Delivery and Hosting

> **Cross-cutting feature** (catalog **FT.5** in [`features.md`](../features.md)). CI/CD, Azure IaC
> (Bicep), and deployment pipelines for the GitHub → Azure staging path and alignment with GCaaS
> corporate production (see [`docs/deployment/`](../../deployment/)).

## Status

**On hold — pending DevOps consultation** (2026-06-02). Moved out of **F00 / R0.0** so application
developers can continue **F00-W10+** without blocking on org CI policy, Azure subscription, or
platform delivery choices.

| Order | Work item                                                                                                                 |  SP | Status                                               |
| ----: | ------------------------------------------------------------------------------------------------------------------------- | --: | ---------------------------------------------------- |
|     1 | [FT05 - W01 - GitHub Actions CI Configuration](FT05%20-%20W01%20-%20GitHub%20Actions%20CI%20Configuration.md)             |   5 | 🟡 in progress (blocked — Actions disabled / CI TBD) |
|     2 | [FT05 - W02 - Azure Infrastructure with Bicep](FT05%20-%20W02%20-%20Azure%20Infrastructure%20with%20Bicep.md)             |   8 | pending                                              |
|     3 | [FT05 - W03 - CD Deployment Pipelines Configuration](FT05%20-%20W03%20-%20CD%20Deployment%20Pipelines%20Configuration.md) |   5 | pending                                              |

**FT05 total:** 18 SP (workflow YAML partially on `main`; completion blocked on platform decisions).

## Former IDs

| Former (F00) | Current (FT05) |
| ------------ | -------------- |
| F00-W04      | FT05-W01       |
| F00-W05      | FT05-W02       |
| F00-W06      | FT05-W03       |

## Prerequisites (application)

- **F00-W02**, **F00-W03** (monorepo scaffolding) — done.
- **F00-W08** (quality gates in CI) — done; gates apply when CI is enabled.

## DevOps decisions to confirm

1. **CI runner:** GitHub Actions (enable on repo) vs Azure DevOps / other.
2. **Azure landing zone:** subscription, naming, Bicep vs existing shared infra.
3. **CD scope:** GitHub → Azure staging only vs GCaaS Helm path first ([`gcaas-hosting.md`](../../deployment/gcaas-hosting.md)).
4. **Secrets & environments:** Service principals, Key Vault, GitHub Environments / approvals.

Track progress in [`STATUS.md`](../STATUS.md) (section **FT05 — Delivery and Hosting**).

---

_FT05 — Delivery and Hosting — Legal Ai Ar_
