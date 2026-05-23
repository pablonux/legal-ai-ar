# Contribution Guide — Legal AI AR

This document summarizes the project's branch and pull request policy. The full specification is in [`docs/design/f0-1-repo-structure.md`](docs/design/f0-1-repo-structure.md).

---

## Branch policy

### Trunk-based model

- **Main branch**: `main` — single integration branch.
- `main` must remain stable and deployable at all times.
- The CI pipeline runs on every push to `main` and on every PR to `main`.

### Feature branches

**Pattern**: `feature/{feature-id}-{short-description}`

Examples:
- `feature/F0-1-repo-structure`
- `feature/F1-2-crawler-csjn`
- `fix/parser-null-reference`

**Rules**:
- Short-lived branches (days, not weeks).
- One branch per feature or fix.
- Create from `main` and merge back to `main`.

### Prohibited branches

- `develop` is not used as an integration branch.
- Long release branches are not maintained (deploy is direct from `main` via CD).

---

## Pull Requests

### Prerequisites

- Green CI pipeline (build + tests).
- No conflicts with `main`.
- Description indicating the feature/task and main changes.

### Review

- **Minimum 1 approver** before merge (except when there is only one active contributor).
- The author cannot approve their own PR (except in the above case).
- Review comments must be resolved before merge.

**Single contributor**: When only one person is working on the repo, the external approval requirement is waived. The merge may be performed after green CI and meeting the above prerequisites.

### Merge strategy

- **Squash merge** to `main`.
- Commit message: `{feature-id}: {description}` (e.g., `F0-1: monorepo structure and conventions`).

### Recommended size

- Focused PRs (< 400 lines of diff when possible).
- If a feature generates a very large PR, split into incremental PRs.

---

## Code conventions

- **Code**: English (classes, methods, variables, schemas).
- **User interface**: Spanish (labels, messages).
- **Files**: see [`.editorconfig`](.editorconfig) for indentation and format style.

---

## References

- [`docs/design/f0-1-repo-structure.md`](docs/design/f0-1-repo-structure.md) — structure, conventions, full policy
- [`docs/roadmap/ROADMAP.md`](docs/roadmap/ROADMAP.md) — features and tasks
- [`docs/architecture/legal-ai-ar-specs.md`](docs/architecture/legal-ai-ar-specs.md) — technical specifications
