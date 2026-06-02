<!-- Legal Ai Ar — Pull Request -->

## Work item

Closes **F__-W__**  <!-- e.g. Closes F0.0-W02 -->

## Summary

<!-- What this PR changes and why. List created/changed files. -->

## Type

- [ ] feat  - [ ] fix  - [ ] refactor  - [ ] test  - [ ] docs  - [ ] chore

## Definition of Done — must all be checked before merge

See [`docs/roadmap/DEFINITION-OF-DONE.md`](../docs/roadmap/DEFINITION-OF-DONE.md).

**Code**
- [ ] Work-item Tasks implemented and acceptance criteria met
- [ ] `dotnet build` clean (no warnings) and `dotnet test` green; frontend lints/builds if touched
- [ ] Conventions respected (Clean Architecture, `LegalAiAr.*`, English-first)

**Documentation round-trip (mandatory — do not merge if any is unchecked)**
- [ ] Work-item file updated (Tasks `[x]`, Acceptance Criteria `[x]`)
- [ ] `docs/roadmap/STATUS.md` updated (progress log + "Next up" advanced)
- [ ] Affected docs synced to match the merged code (`docs/standards/*`, `docs/technical/*`, `docs/ontology/*`, `docs/deployment/*`, `features.md`, onboarding — whichever applies)
- [ ] Architecture changes align with [`docs/standards/pwc-internal-app-architecture.md`](../docs/standards/pwc-internal-app-architecture.md) §16 checklist (or exception documented)
- [ ] New data sources documented in `docs/technical/20-legal-data-sources.md` (if any)
- [ ] No broken cross-links

> ⚠️ A work item **cannot be closed** while any Documentation item above is unchecked.
