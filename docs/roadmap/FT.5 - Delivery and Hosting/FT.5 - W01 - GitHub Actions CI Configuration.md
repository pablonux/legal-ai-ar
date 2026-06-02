# FT.5 - W01 - GitHub Actions CI Configuration

> **Feature:** FT.5 - Delivery and Hosting (formerly F0.0-W04)
> **Release:** Cross-cutting (FT) | **Sprint:** —
> **Track status:** ⏸ **On hold** — pending DevOps consultation (CI platform / org policy)
> **Type:** devops | **Priority:** High
> **Estimate:** 5 story points
> **Assignable to:** Backend Dev (or whoever has the most DevOps experience)

---

## Description

Configure the Continuous Integration (CI) pipelines in GitHub Actions for backend (.NET 10) and frontend (Angular 19). The pipelines must run automatically on every push to `main` and on every PR, and must block the merge if they fail.

---

## Status — in progress (blocked)

| Area                                | State                                       |
| ----------------------------------- | ------------------------------------------- |
| Workflow files on `main`            | ✅ Done                                     |
| GitHub Actions enabled              | ❌ **Blocked** — disabled at org/repo level |
| First successful CI runs            | ⏸ Pending Actions or alternate CI           |
| Branch protection + required checks | ⏸ Pending                                   |
| Acceptance criteria verification    | ⏸ Pending                                   |

**Decision needed:** enable **GitHub Actions** on this repo, or confirm a **different CI** (update
delivery docs and gates; GHA YAML may remain as reference or be replaced).

---

## Tasks

- [x] Create `.github/workflows/ci-backend.yml`
- [x] Create `.github/workflows/ci-frontend.yml`
- [x] Configure path filters so each pipeline only runs on relevant changes
- [x] Configure NuGet and npm caching to speed up builds
- [ ] Configure branch protection rules on `main`:
    - Require PR reviews (1)
    - Require status checks (CI backend + CI frontend)
    - Require up-to-date branch
    - Squash merge only
- [x] Configure GitHub secrets for Azure (if needed for integration tests) — **N/A** (current tests do not require Azure secrets)
- [x] Add CI badges to README.md
- [ ] Verify that a PR with failing tests can NOT be merged
- [ ] Verify that a PR with lint errors can NOT be merged

### Implemented adaptations (vs. template below)

- Backend tests: `dotnet test LegalAiAr.sln` (no separate UnitTests/IntegrationTests projects).
- Backend format: `dotnet format` diagnostic only; `--verify-no-changes` deferred to **F0.0-W08**.
- Frontend lint in CI: `npm run lint:ci` (workspace libs only); app lint debt → **F0.0-W08**.
- Frontend artifact path: `frontend/dist/legal-ai-ar`.
- Removed legacy `.github/workflows/ci.yml`.

---

## Resume checklist (when unblocked)

1. **If using GitHub Actions:** Settings → Actions → General → enable workflows for this repo.
2. Confirm green runs of `CI Backend` and `CI Frontend` on `main` (Actions tab or PR checks).
3. Settings → General → Pull Requests → squash merge only.
4. Settings → Branches → rule for `main`:
    - Required checks: `CI Backend / build-and-test`, `CI Frontend / build-and-test`
    - 1 approval, require up-to-date branch
5. Verify AC: path-filtered runs, failing test blocks merge, artifacts on push to `main`.
6. Mark remaining tasks/AC `[x]`, update [`STATUS.md`](../STATUS.md) (FT.5 section) progress log, close WI.

**If using another CI:** document the chosen system in `docs/deployment/`, align branch/PR gates,
update README badges, and either remove or archive the GHA workflow files in a follow-up change.

---

## Backend CI Pipeline

```yaml
# .github/workflows/ci-backend.yml
name: CI Backend

on:
    push:
        branches: [main]
        paths: ["backend/**", ".github/workflows/ci-backend.yml"]
    pull_request:
        branches: [main]
        paths: ["backend/**"]

defaults:
    run:
        working-directory: backend

jobs:
    build-and-test:
        runs-on: ubuntu-latest
        steps:
            - uses: actions/checkout@v4

            - name: Setup .NET 10
              uses: actions/setup-dotnet@v4
              with:
                  dotnet-version: "10.0.x"

            - name: Cache NuGet
              uses: actions/cache@v4
              with:
                  path: ~/.nuget/packages
                  key: ${{ runner.os }}-nuget-${{ hashFiles('**/Directory.Packages.props') }}
                  restore-keys: ${{ runner.os }}-nuget-

            - name: Restore
              run: dotnet restore

            - name: Build
              run: dotnet build --no-restore -c Release -warnaserror

            - name: Unit Tests
              run: dotnet test tests/LegalAiAr.UnitTests -c Release --no-build --logger trx --collect:"XPlat Code Coverage"

            - name: Integration Tests
              run: dotnet test tests/LegalAiAr.IntegrationTests -c Release --no-build --logger trx
              continue-on-error: false

            - name: Publish Coverage
              uses: codecov/codecov-action@v4
              if: always()
              with:
                  directory: tests/LegalAiAr.UnitTests/TestResults
                  fail_ci_if_error: false

            - name: Publish Build Artifact
              if: github.ref == 'refs/heads/main' && github.event_name == 'push'
              run: dotnet publish src/LegalAiAr.Api -c Release -o ./publish

            - name: Upload Artifact
              if: github.ref == 'refs/heads/main' && github.event_name == 'push'
              uses: actions/upload-artifact@v4
              with:
                  name: backend-${{ github.sha }}
                  path: backend/publish/
```

---

## Frontend CI Pipeline

```yaml
# .github/workflows/ci-frontend.yml
name: CI Frontend

on:
    push:
        branches: [main]
        paths: ["frontend/**", ".github/workflows/ci-frontend.yml"]
    pull_request:
        branches: [main]
        paths: ["frontend/**"]

defaults:
    run:
        working-directory: frontend

jobs:
    build-and-test:
        runs-on: ubuntu-latest
        steps:
            - uses: actions/checkout@v4

            - name: Setup Node 22
              uses: actions/setup-node@v4
              with:
                  node-version: "22"
                  cache: "npm"
                  cache-dependency-path: frontend/package-lock.json

            - name: Install dependencies
              run: npm ci

            - name: Lint
              run: npm run lint

            - name: Build (production)
              run: npm run build:prod

            - name: Unit Tests
              run: npm run test -- --coverage

            - name: Upload Coverage
              uses: codecov/codecov-action@v4
              if: always()
              with:
                  directory: frontend/coverage
                  fail_ci_if_error: false

            - name: Upload Build Artifact
              if: github.ref == 'refs/heads/main' && github.event_name == 'push'
              uses: actions/upload-artifact@v4
              with:
                  name: frontend-${{ github.sha }}
                  path: frontend/dist/
```

---

## Branch Protection Rules (main)

```mermaid
graph TD
    PR[Pull Request to main] --> CHECK1{CI Backend passes?}
    CHECK1 -->|Yes| CHECK2{CI Frontend passes?}
    CHECK1 -->|No| BLOCK[❌ Merge blocked]
    CHECK2 -->|Yes| CHECK3{Review approved?}
    CHECK2 -->|No| BLOCK
    CHECK3 -->|Yes| CHECK4{Branch up-to-date?}
    CHECK3 -->|No| BLOCK
    CHECK4 -->|Yes| MERGE[✅ Squash merge]
    CHECK4 -->|No| REBASE[Rebase required]
    REBASE --> CHECK1
```

**Configuration in GitHub → Settings → Branches → Branch protection rules:**

| Setting                               | Value                                                         |
| ------------------------------------- | ------------------------------------------------------------- |
| Require a pull request before merging | ✅                                                            |
| Required approving reviews            | 1                                                             |
| Require status checks to pass         | ✅                                                            |
| Required checks                       | `CI Backend / build-and-test`, `CI Frontend / build-and-test` |
| Require branches to be up to date     | ✅                                                            |
| Allow squash merging                  | ✅ (this one only)                                            |
| Allow merge commits                   | ❌                                                            |
| Allow rebase merging                  | ❌                                                            |

---

## Acceptance Criteria

- [ ] A push to `main` with changes in `backend/` runs only CI Backend — _blocked until Actions enabled_
- [ ] A push to `main` with changes in `frontend/` runs only CI Frontend — _blocked until Actions enabled_
- [ ] A PR with failing tests shows ❌ and does not allow merge — _blocked until branch protection + CI runs_
- [ ] A PR with all checks ✅ and 1 review allows merge — _blocked until branch protection + CI runs_
- [ ] Artifacts are generated correctly on pushes to main — _blocked until Actions enabled_
- [ ] The NuGet/npm cache works (second build is faster) — _blocked until Actions enabled_
- [x] The CI badges appear in the README

---

## Dependencies

- **Depends on:** F0.0-W02 (backend scaffolding), F0.0-W03 (frontend scaffolding) — done
- **Blocks:** FT.5-W03 (CD pipelines)

---

_FT05 - W01 - GitHub Actions CI Configuration — Legal Ai Ar_
