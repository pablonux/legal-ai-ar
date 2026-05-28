# Setup — Legal AI AR

Documentation for configuring the development and deployment environment.

| Document / File | Description |
|---|---|
| [azure-credentials-guide.md](./azure-credentials-guide.md) | Guide to obtain credentials, connection strings and endpoints for existing Azure services (T-01) |
| [staging-verification-tutorial.md](./staging-verification-tutorial.md) | Step-by-step: verify E011–E018 against staging Azure and update ROADMAP |
| [ef-migrations.md](./ef-migrations.md) | EF Core migrations, Blob container (T-04) and AI Search indexes (T-05) |
| [docker-compose-local.md](./docker-compose-local.md) | Local SQL Server with Docker Compose (T-09) |
| [azure-connectivity-verification.md](./azure-connectivity-verification.md) | Connectivity verification to all Azure services (T-10) |
| `../scripts/load-env.ps1` | PowerShell script to load `.env` in the current session |
| `../.env.example` (repo root) | Environment variables template. Copy to `.env` and replace secret placeholders (`DB_SECRET`, `STORAGE_KEY`, `SEARCH_KEY`, `OPENAI_KEY`) |
