# Definition of Done (DoD)

> **Project rule — applies to every work item, regardless of which developer + AI implements it
> (Cursor or Cowork).** A work item **cannot be closed** (marked done, PR merged) until **all** of the
> items below are satisfied, including the **documentation round-trip**. This is non-negotiable and is
> enforced at review/PR time.

---

## A work item is *Done* only when…

### 1. Code

- [ ] All **Tasks** in the work item are implemented and checked `[x]`.
- [ ] `dotnet build` passes with **no warnings** (warnings-as-errors) and `dotnet test` is green.
- [ ] Frontend (if touched) builds and lints clean.
- [ ] All **Acceptance Criteria** in the work item are met and checked `[x]`.
- [ ] Conventions respected (Clean Architecture, naming `LegalAiAr.*`, English-first language rule).

### 2. Documentation round-trip (mandatory — the work item cannot close without this)

The principle is **code ⇄ docs stay in sync**: the plan/docs guide the code, and when the code lands,
the docs are updated to match what was actually built.

- [ ] **Work item file** updated: Tasks `[x]`, Acceptance Criteria `[x]`, notes on any deviation.
- [ ] **`docs/roadmap/STATUS.md`** updated: a row added to the **progress log**, and **"Next up"** advanced to the following work item.
- [ ] **Affected docs synced** — if the change touched any of these areas, update the matching doc so it reflects the merged code:
  - Architecture / ADRs / diagrams → `docs/technical/10–12`
  - Ingestion (CSJN / SAIJ web / thesaurus) → `docs/technical/13–15`
  - Chat / RAG / agents / tools / guardrails → `docs/technical/16`
  - Data model / entities → `docs/technical/17` (+ `docs/ontology/`)
  - Frontend (views, routes, services) → `docs/technical/18`
  - Admin / pipeline operations → `docs/technical/19`
  - Data sources → `docs/technical/20-legal-data-sources.md`
  - Endpoints / KPIs / scope → `docs/roadmap/features.md`
  - Delivery / hosting / infra → `docs/deployment/*`
  - Local setup / dev workflow → `docs/onboarding/*`, `docs/developer-guide.md`
- [ ] **New public sources** discovered → documented in the data-sources catalog (don't keep static data files; document the source).
- [ ] Cross-links and references still resolve (no broken links to moved/renamed files).

### 3. Pull request

- [ ] PR references the work item (`Closes FXX-WYY`) and lists created/changed files.
- [ ] The PR checklist — including the documentation items above — is **fully checked**.
- [ ] CI is green; at least one approval (or single-contributor waiver) per the
      [GitHub Delivery](../deployment/github-delivery.md) policy.

---

## Enforcement

- **AI assistants (Cursor & Cowork):** must **not** mark a work item complete, set its status to done,
  or treat a PR as finished while any DoD item — *especially the documentation round-trip* — is
  incomplete. The `reviewer` skill verifies the DoD before approving.
- **Humans / reviewers:** must **not** merge a PR with unchecked DoD items. The
  [PR template](../../.github/PULL_REQUEST_TEMPLATE.md) carries the checklist; branch protection +
  CODEOWNERS make it a gate.

---

*Definition of Done — Legal Ai Ar*
