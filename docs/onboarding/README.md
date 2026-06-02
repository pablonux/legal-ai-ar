# Developer Onboarding — Legal Ai Ar

> **Start here.** This is the single starting point for any developer joining Legal Ai Ar.
> Follow it top to bottom and you will have the project running locally and your IDE ready
> in well under an hour — whether you work in **VS Code + Claude** or **Cursor**.
>
> **Last updated:** 2026-06-01 (F0.0-W07 — setup scripts, cloud DEV workflow)

---

## 0. Choose your environment

The project supports two interchangeable development environments. Both use the same repo, the same
Docker tooling, and the same AI conventions — they only differ in the IDE and AI assistant. Pick the
one you prefer:

| | **Environment A** | **Environment B** |
|---|---|---|
| **Editor** | Visual Studio Code | Cursor |
| **AI assistant** | Claude (in VS Code) | Cursor AI |
| **AI config it reads** | `CLAUDE.md` + `.claude/skills/` | `.cursor/rules/` + `.cursor/skills/` |
| **Container tooling** | Docker | Docker |
| **Setup guide** | [§7 Environment A](#7-environment-a--vs-code--claude) | [§8 Environment B](#8-environment-b--cursor) |

Sections **1–6** are common to both environments. Do them first. Then jump to your environment's
section (7 or 8).

> **Golden rule (both environments):** the AI assistant **never writes code directly into files**.
> It tells you which file to create or change, in which path, and gives you the code to paste.
> This is intentional so a human always reviews what enters the repo. See the
> [Developer Guide](developer-guide.md).

---

## 1. Prerequisites

Install these once on your machine. Versions are the minimums the project is built against.

| Tool | Version | Why | Check |
|------|---------|-----|-------|
| **Git** | any recent | Clone the repo, branches, PRs | `git --version` |
| **Docker Desktop** | any recent | Build / run the app (API, SPA, workers) in containers | `docker --version` |
| **.NET SDK** | **10.0.100+** | Build and run the backend | `dotnet --version` |
| **Node.js** | **22 LTS+** | Build and run the Angular SPA | `node --version` |
| **AppKit npm access** | JFrog + Entra SSO | Install `@appkit4/*` for the SPA (see [appkit-npm-access.md](appkit-npm-access.md)) | `npm whoami --registry=https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm/` |
| **Azure CLI** | any recent | Sign in to the DEV subscription / deploy tooling | `az --version` |

> The Angular CLI is **not** required globally — `npm ci` installs the pinned local version and you
> run it through `npm` scripts. If you want it globally anyway: `npm i -g @angular/cli`.

> **One shared stack — no local services.** Every external service (Azure SQL, Blob Storage, AI
> Search, Azure OpenAI) is the **same shared cloud DEV** environment used by all other environments.
> There is **no local or offline substitute** — there is no Docker image for Azure OpenAI, for
> example — and we deliberately do **not** run a different local stack (no local SQL Server, no
> Neo4j). You therefore need **DEV credentials before you can run anything** (see
> [§3](#3-configuration--secrets-shared-cloud-dev)). Docker is used to run the **application itself**,
> not the dependencies.

---

## 2. Clone the repository

```bash
git clone <repo-url> legal-ai-ar
cd legal-ai-ar
```

Application code lives at the repo root: `backend/`, `frontend/`, plus `infra/`, `deployment/`,
and `docker-compose.app.yml`.

### Automated setup (recommended)

From the repo root, run the setup script for your OS. It checks prerequisites, creates `.env` from
the template if missing, and runs `dotnet restore` / `dotnet build` / `npm ci`:

```powershell
# Windows (PowerShell)
.\infra\scripts\setup-local.ps1

# Optional: verify Azure DEV connectivity (requires filled .env)
.\infra\scripts\setup-local.ps1 -VerifyConnectivity
```

```bash
# Linux / macOS
chmod +x infra/scripts/setup-local.sh
./infra/scripts/setup-local.sh
```

Then continue at [§3](#3-configuration--secrets-shared-cloud-dev) to fill secrets, and [§4–5](#4-run-the-backend-net)
to start backend and frontend. Expected time with DEV credentials ready: **under 30 minutes**.

---

## 3. Configuration & secrets (shared cloud DEV)

Before running anything, point the app at the shared cloud DEV services. The repo ships a template:

```bash
# from repo root
cp .env.example .env
```

Then fill the placeholders the template calls out with the DEV values (ask the Tech Lead for access):

| Placeholder | Service | Where to get it |
|-------------|---------|-----------------|
| `DB_SECRET` | Azure SQL DEV password | Tech Lead / Key Vault |
| `STORAGE_KEY` | Storage Account access key | Azure Portal → Storage account → Access keys |
| `SEARCH_KEY` | Azure AI Search admin key | Azure Portal → AI Search → Keys |
| `OPENAI_KEY` | Azure OpenAI API key | Azure Portal → Azure OpenAI → Keys and Endpoint |

The template's endpoints (Azure SQL, Blob, AI Search, Azure OpenAI) already point at the DEV resources
— you only replace the secret placeholders. **Do not** change them to local hosts: there is no local
stack.

> **.NET key format:** environment variables use double underscores, e.g. `AzureSql__ConnectionString`
> maps to the `AzureSql:ConnectionString` config key. Never commit a filled-in `.env` — it is
> git-ignored for a reason. If a secret ever leaks, rotate it and tell the Tech Lead.

### Local authentication — nothing to do

In `Development`, the API **auto-injects a signed `id_token` session cookie** (configured in
`appsettings.Development.json`: `Auth:Development:InjectIdentity = true`, user `dev@legal-ai.local`,
role `admin`). You are "logged in" automatically — there is **no local login screen** and **no MSAL**
to configure. Production auth is platform-managed Entra SSO via the same `id_token` cookie; see
[`docs/deployment/gcaas-hosting.md`](../deployment/gcaas-hosting.md).

---

## 4. Run the backend (.NET)

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/api/LegalAiAr.Api
```

| Endpoint | URL |
|----------|-----|
| API (HTTP) | http://localhost:5088 |
| API (HTTPS) | https://localhost:7064 |
| Swagger UI | http://localhost:5088/swagger |

The API connects to the shared Azure SQL DEV database from your `.env`. The schema there is normally
already provisioned and migrations are applied through **CD** (see
[GitHub Delivery](../deployment/github-delivery.md)). Run a migration yourself **only when
coordinating a schema change** with the team:

```bash
dotnet tool install --global dotnet-ef        # once, if you don't have it

# Apply the latest schema
dotnet ef database update \
  --project src/shared/LegalAiAr.Infrastructure \
  --startup-project src/api/LegalAiAr.Api

# Create a new migration (output goes to Persistence/Migrations)
dotnet ef migrations add <Name> \
  --project src/shared/LegalAiAr.Infrastructure \
  --startup-project src/api/LegalAiAr.Api \
  --output-dir Persistence/Migrations

# Revert the last (unapplied) migration
dotnet ef migrations remove \
  --project src/shared/LegalAiAr.Infrastructure \
  --startup-project src/api/LegalAiAr.Api
```

The 6 background workers (`src/workers/`) and CLI tools are only needed when working on the ingestion
pipeline — see [§6](#6-run-in-docker--verify) to run them in containers. For credentials, the
per-component variable matrix, queues, and connectivity verification, see
[azure-services.md](azure-services.md).

---

## 5. Run the frontend (Angular)

In a second terminal:

```bash
cd frontend
# One-time: npm login to PwC AppKit registry — see appkit-npm-access.md
npm ci
npm start          # ng serve (proxy to API on :5088)
```

| Endpoint | URL |
|----------|-----|
| SPA | http://localhost:4200 |

The SPA calls the API with `withCredentials` (cookie-based session). With the API running and the dev
identity auto-injected, the app works end to end without any login step.

---

## 6. Run in Docker + Verify

You can run the whole app — API, SPA, and the pipeline workers — in containers identical to what
deploys. This still uses your `.env` and the **same shared cloud DEV** services (it does **not** start
any database container):

```bash
# from repo root
docker compose -f docker-compose.app.yml up -d
# SPA: http://localhost:4200  ·  API: http://localhost:5088  ·  Docs: http://localhost:5088/docs
```

```bash
docker compose -f docker-compose.app.yml ps     # check container health
docker compose -f docker-compose.app.yml logs -f api
```

> The repo uses **`docker-compose.app.yml`** to run the API, SPA, and workers in containers. It reads
> your `.env` and connects to the **same shared cloud DEV** services — it does **not** start a local
> database or Azurite.

### Verify your setup

You're done when all of these work:

- Swagger UI loads at http://localhost:5088/swagger (or `/docs` when running in containers).
- The SPA loads at http://localhost:4200 and reaches the API (no 401 — the dev cookie is injected).
- `dotnet build` and `dotnet test` pass in `backend`.

If anything fails, see [troubleshooting.md](troubleshooting.md).

---

## 7. Environment A — VS Code + Claude

1. **Install [Visual Studio Code](https://code.visualstudio.com/).**
2. **Open the repo folder.** VS Code reads `.vscode/extensions.json` and prompts to install the
   workspace-recommended extensions — accept. The full list and what each is for is in
   [recommended-extensions.md](recommended-extensions.md) (.NET, Angular, ESLint/Prettier,
   EditorConfig, GitLens, Azure, SQL, Docker). The repo's root `.editorconfig` is applied
   automatically once `EditorConfig.EditorConfig` is installed.
3. **AI assistant — Claude in VS Code.** Claude reads the project's `CLAUDE.md` at the repo root and
   the shared skills in `.claude/skills/`. These encode the architecture, naming, language rule, and
   the work-item workflow, so the assistant behaves consistently with the rest of the team.
4. **Debugging.** The repo ships ready-made launch configurations in `.vscode/launch.json` (Run and
   Debug panel, `Ctrl+Shift+D`):

   - **Backend (API)** — runs `LegalAiAr.Api` with the debugger (port 5088), loads `.env`.
   - **Frontend (Angular)** — starts `ng serve` and opens the browser debugger (port 4200).
   - **Full Stack (Front + Back)** — compound that launches both at once.
   - **Worker: Crawler / Parser / Enrichment / Indexer** — debug a single pipeline worker; set
     breakpoints in e.g. `CrawlerWorkerService.cs` and the worker breaks when a queue message arrives.

Day-to-day, you drive work items with the AI exactly as described in the
[Developer Guide](developer-guide.md) (analysis → task breakdown → implement → review → PR).

---

## 8. Environment B — Cursor

1. **Install [Cursor](https://cursor.com/).** Cursor is VS Code-compatible, so the same extensions and
   the same `.editorconfig` apply — install the workspace recommendations from
   [recommended-extensions.md](recommended-extensions.md).
2. **AI assistant — Cursor AI.** Cursor automatically reads the rules in `.cursor/rules/`
   (`project.mdc`, `backend-dotnet.mdc`, `frontend-angular.mdc`, `work-items.mdc`) and the skills in
   `.cursor/skills/`. These mirror `CLAUDE.md` and `.claude/skills/` one-to-one, so Environment B
   produces the same conventions as Environment A.
3. **Debugging.** Same as Environment A — Cursor reads the same `.vscode/launch.json` configurations
   (Backend, Frontend, Full Stack, per-worker).

The AI workflow (skills: `architect`, `task-breakdown`, `developer`, `designer`, `reviewer`, …) is
documented in the [Developer Guide](developer-guide.md) and the
[Cowork/Cursor setup tutorial](cowork-setup-tutorial.md).

---

## 9. Your first work item

Once your environment is up:

1. Read the [Developer Guide](developer-guide.md) — the full flow from a work item to a PR.
2. Pick your assigned work item under `docs/roadmap/{Feature}/`.
3. Ask the AI to analyze it (`architect`), then break it down (`task-breakdown`).
4. Branch: `git checkout -b feature/fXX-wYY-short-description`.
5. Implement task by task (the AI gives you code; you place it), then `dotnet build` / `dotnet test`.
6. Ask the AI to `review`, fix issues, and open a PR to `main` (`Closes FXX-WYY`).

CI (build, tests, format / lint) must pass before merge when GitHub Actions is enabled — see
[GitHub Delivery](../deployment/github-delivery.md). If Actions is disabled, run `dotnet build`,
`dotnet test`, and `npm run lint:ci` locally before opening a PR.

---

## 10. Reference documentation

Everything you might need, grouped by purpose.

### Getting started & workflow

| Document | What it's for |
|----------|---------------|
| [Developer Guide](developer-guide.md) | The work-item → PR workflow, AI skills, conventions, FAQ |
| [Definition of Done](../roadmap/DEFINITION-OF-DONE.md) | Mandatory close criteria incl. the documentation round-trip — a work item can't close without it |
| [Azure Services](azure-services.md) | Credentials, per-component variable matrix, queues, connectivity verification |
| [Recommended Extensions](recommended-extensions.md) | IDE extensions for VS Code and Cursor |
| [Troubleshooting](troubleshooting.md) | Common setup errors and fixes |
| [Cowork/Cursor Setup Tutorial](cowork-setup-tutorial.md) | How the AI assistants are configured |

### Planning & scope

| Document | What it's for |
|----------|---------------|
| [Project Status](../roadmap/STATUS.md) | **Where we are / what's next** — read first |
| [Features Roadmap](../roadmap/features.md) | Full plan: releases, features, endpoints, KPIs, stack |
| [Backlog](../roadmap/backlog.md) | Feature/work-item inventory and totals |
| `docs/roadmap/{Feature}/` | The work items themselves (e.g. F0.0 = dev environment & structure) |

### Technical deep-dives

| Document | Topic |
|----------|-------|
| [01 RAG & Retrieval](../technical/01-rag-retrieval.md) | Hybrid search, embeddings, re-ranking |
| [02 Agentic Architecture](../technical/02-agentic-architecture.md) | Agents, router, tool calling |
| [03 Prompt Engineering](../technical/03-prompt-engineering.md) | Templates, system prompts |
| [04 Ingestion & Processing](../technical/04-ingestion-processing.md) | The 6-stage pipeline |
| [05 AI Quality & Evaluation](../technical/05-ai-quality-evaluation.md) | Metrics, golden set, LLM-as-judge |
| [06 Security & Compliance](../technical/06-ai-security-compliance.md) | Content filtering, PII, privilege |
| [07 Observability & LLMOps](../technical/07-observability-llmops.md) | Tracing, token usage, caching |
| [08 Legal AI UX](../technical/08-legal-ai-ux.md) | Streaming, inline citation, feedback |
| [09 Data & Knowledge Management](../technical/09-data-knowledge-management.md) | Taxonomy, versioning, lineage |
| [Argentine Legal Ontology](../ontology/argentine-legal-ontology.md) | Domain model (classes, properties, relationships) |
| [AppKit 4 (UI library)](../appkit4/README.md) | PwC AppKit 4 reference: components, design tokens, icons, patterns (start at `AGENTS.md`) |

### Delivery & hosting

| Document | What it's for |
|----------|---------------|
| [GitHub Delivery](../deployment/github-delivery.md) | Branching, CI quality gates, CD to Azure staging |
| [GCaaS Hosting](../deployment/gcaas-hosting.md) | PwC corporate hosting: Helm/Knative, Entra SSO, Vault |

---

*Developer Onboarding — Legal Ai Ar*
