# Recommended Extensions — Legal Ai Ar

> IDE extensions recommended for working on Legal Ai Ar. They apply to **both** development
> environments: **VS Code + Claude** and **Cursor** (Cursor is VS Code-compatible and uses the same
> extension marketplace and the same workspace recommendation prompt).
>
> **Last updated:** 2026-05-28

---

## How to install

When you open the repo folder, VS Code / Cursor reads the workspace recommendations and prompts you to
install them in one click. If you skipped the prompt, open the Extensions panel and filter by
**"Recommended"**.

The recommendations are committed in `.vscode/extensions.json` at the workspace root — the source of
truth. Its [contents](#vscodeextensionsjson) are listed at the bottom of this page for reference.

> The **Usage %** below is an estimated relevance score for work on this repo, not IDE telemetry. To
> list what you currently have installed with versions: `code --list-extensions --show-versions`
> (or `cursor --list-extensions --show-versions`).

---

## Recommended extensions

| Extension ID | Usage % | Role on this project |
|--------------|--------:|----------------------|
| `ms-dotnettools.csdevkit` | **98%** | Backend .NET — solution, debug, refactor |
| `angular.ng-template` | **92%** | Angular SPA — templates, language service |
| `eamodio.gitlens` | **88%** | Git history, blame, PR context |
| `dbaeumer.vscode-eslint` | **80%** | TypeScript / Angular lint |
| `esbenp.prettier-vscode` | **78%** | Formatting (TS, HTML, JSON, MD) |
| `EditorConfig.EditorConfig` | **75%** | Enforces repo `.editorconfig` |
| `formulahendry.dotnet-test-explorer` | **70%** | Discover and run .NET tests |
| `ms-azuretools.vscode-azure-resource-groups` | **62%** | Browse Azure resources |
| `ms-mssql.mssql` | **60%** | Query the Azure SQL DEV database, `scripts/sql/` |
| `ms-azuretools.vscode-azureappservice` | **58%** | API deploy (App Service staging) |
| `ms-vscode.azure-account` | **55%** | Azure sign-in for deploy tooling |
| `ms-azuretools.vscode-azurestorage` | **52%** | Blob Storage and queues |
| `ms-azuretools.vscode-azure-sql` | **50%** | Azure SQL (complements MSSQL) |
| `ms-azuretools.vscode-docker` | **48%** | `docker-compose`, worker Dockerfiles, containers |
| `ms-azuretools.vscode-bicep` | **25%** | Infra-as-code (optional) |

### Usage % scale

| Score | Meaning |
|-------|---------|
| **90–100%** | Core daily use (.NET, Angular) |
| **60–89%** | Frequent (git, SQL, Azure deploy, tests) |
| **25–59%** | Occasional (storage, Docker, Bicep) |

---

## Notes per environment

**VS Code + Claude (Environment A).** Install all of the above. The `ms-dotnettools.csdevkit`
(C# Dev Kit) drives backend build/debug/test; `angular.ng-template` powers the SPA.

**Cursor (Environment B).** Same list — Cursor installs from the VS Code marketplace. A couple of
proprietary Microsoft extensions (notably the C# Dev Kit) can have marketplace/licensing differences
in non-VS Code editors; if one is unavailable, Cursor's built-in language tooling or the OmniSharp
alternative covers backend editing, and you can still build/test from the terminal
(`dotnet build` / `dotnet test`).

---

## `.vscode/extensions.json`

This file lives at the repo root so both editors show the install prompt. Current contents:

```json
{
  "recommendations": [
    "ms-dotnettools.csdevkit",
    "angular.ng-template",
    "eamodio.gitlens",
    "dbaeumer.vscode-eslint",
    "esbenp.prettier-vscode",
    "EditorConfig.EditorConfig",
    "formulahendry.dotnet-test-explorer",
    "ms-azuretools.vscode-azure-resource-groups",
    "ms-mssql.mssql",
    "ms-azuretools.vscode-azureappservice",
    "ms-vscode.azure-account",
    "ms-azuretools.vscode-azurestorage",
    "ms-azuretools.vscode-azure-sql",
    "ms-azuretools.vscode-docker",
    "ms-azuretools.vscode-bicep"
  ]
}
```

> If you also keep a frontend-scoped `frontend/.vscode/extensions.json`, recommend at least
> `angular.ng-template`, `dbaeumer.vscode-eslint`, and `esbenp.prettier-vscode` there.

---

## References

- [Onboarding hub](README.md) — full local setup for both environments
- [Developer Guide](../developer-guide.md) — work-item → PR workflow and AI skills
- [GitHub Delivery](../deployment/github-delivery.md) — GitHub Actions / Azure CD
- [GCaaS Hosting](../deployment/gcaas-hosting.md) — GCaaS / Helm deploy
- [Workspace recommended extensions (VS Code docs)](https://code.visualstudio.com/docs/editor/extension-marketplace#_workspace-recommended-extensions)

---

*Recommended Extensions — Legal Ai Ar*
