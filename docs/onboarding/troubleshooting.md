# Troubleshooting — Local Setup

> Common errors when bringing Legal Ai Ar up locally, and how to fix them. If your issue isn't here,
> check the [onboarding hub](README.md) or ask the Tech Lead.
>
> **Last updated:** 2026-05-28

---

## 1. Connecting to the cloud DEV database

We use the shared **Azure SQL DEV** database only — there is no local database to start.

**`Login failed` / `Cannot open server` for Azure SQL.**
Your `.env` `AzureSql__ConnectionString` is missing the real password (`DB_SECRET`), or your IP isn't
allowed by the Azure SQL firewall. Confirm the password with the Tech Lead and ask them to add your IP
to the DEV server firewall.

**Network errors / timeouts reaching `*.database.windows.net`.**
You're offline, or behind a VPN/proxy that blocks port 1433, or the firewall rule is missing. Connect
to the corporate network/VPN and retry. Keep `Encrypt` / `TrustServerCertificate` as shipped in
`.env.example` — don't remove them.

---

## 2. EF Core migrations

**`dotnet ef` is not recognized.**
Install the global tool once: `dotnet tool install --global dotnet-ef`. Make sure `~/.dotnet/tools`
is on your `PATH` (restart the terminal after install).

**`Unable to create a DbContext` / design-time errors.**
Run the command with both projects specified, from `backend`:

```bash
dotnet ef database update \
  --project src/shared/LegalAiAr.Infrastructure \
  --startup-project src/api/LegalAiAr.Api
```

**`database update` fails with a connection error.**
You can't reach Azure SQL DEV — see §1 (password / firewall / VPN). Remember migrations normally run
via CD; only apply them manually when coordinating a schema change with the team.

---

## 3. Backend (.NET)

**`dotnet` version mismatch / SDK not found.**
The repo pins the SDK in `backend/global.json` (`10.0.100`, roll-forward `latestFeature`). Install
.NET 10 SDK. Check with `dotnet --version`.

**Build warnings fail the build.**
The backend treats warnings as errors (`Directory.Build.props`). Fix the warning — don't suppress it
unless the Tech Lead agrees.

**HTTPS dev certificate warning.**
Run `dotnet dev-certs https --trust` once to trust the local certificate (used by the `https` profile
on https://localhost:7064).

---

## 4. Frontend (Angular)

**`npm ci` fails on lockfile mismatch.**
Use `npm ci` (not `npm install`) and Node **20 LTS+**. If your Node is older, install/switch with `nvm`.

**SPA loads but every API call is 401.**
The dev identity cookie comes from the API. Make sure the **API is running** and you're on the
`Development` environment (`ASPNETCORE_ENVIRONMENT=Development`), which sets
`Auth:Development:InjectIdentity = true`. The SPA must call the API with `withCredentials` (it does by
default) and the API base URL must match where the API is actually listening.

**CORS errors in the browser console.**
You're hitting the API on a different host/port than it allows. Run the SPA on http://localhost:4200
and the API on http://localhost:5088 (the defaults), or align the API CORS config.

---

## 5. Configuration & secrets

**Azure-backed features (search, chat, ingestion) error out.**
Blob Storage, AI Search, and Azure OpenAI are cloud DEV services. They need real keys in `.env`
(`STORAGE_KEY`, `SEARCH_KEY`, `OPENAI_KEY`) — ask the Tech Lead for DEV access. There is no local
substitute, so the app needs these to work end to end.

**My `.env` changes aren't picked up.**
Restart `dotnet run` (env vars are read at startup). Remember the **double-underscore** mapping:
`AzureSql__ConnectionString` → `AzureSql:ConnectionString`.

**I accidentally committed secrets.**
`.env` is git-ignored — never force-add it. If a secret leaked, rotate it immediately and tell the
Tech Lead.

---

## 6. Running in Docker

**`docker compose -f docker-compose.app.yml up -d` builds but the API can't reach data services.**
That compose file runs the app containers only and reads your `.env` for the cloud DEV connection
strings — it does **not** start any database. Make sure `.env` is filled (§5) and you can reach Azure
DEV (§1). Don't use the repo's `docker-compose.yml` (local SQL / Neo4j) — it's not part of our
workflow.

---

## 7. IDE / AI assistant

**VS Code / Cursor didn't prompt for extensions.**
Open the Extensions panel and filter by "Recommended", or add `.vscode/extensions.json` (see
[recommended-extensions.md](recommended-extensions.md)).

**The AI tried to edit files directly / produced LegalKB names / wrote Spanish in code.**
That contradicts the project rules. The assistant should only tell you what to create and where, use
`LegalAiAr.*` naming, and keep everything in English except end-user-facing UI text. Re-point it at
`CLAUDE.md` (VS Code) or `.cursor/rules/project.mdc` (Cursor) and ask it to follow the rules.

---

*Troubleshooting — Local Setup — Legal Ai Ar*
