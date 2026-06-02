# AppKit 4 — NPM registry access

Legal Ai Ar consumes **PwC AppKit 4** from the corporate JFrog registry. Component API reference lives in [`docs/appkit4/`](../appkit4/README.md); this guide covers **npm access only**.

---

## Registry

| Item             | Value                                                                                  |
| ---------------- | -------------------------------------------------------------------------------------- |
| Portal           | https://appkit.pwc.com/appkit4 (PwC login)                                             |
| Artifactory      | https://artifacts-west.pwc.com/                                                        |
| NPM registry URL | `https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm/` |
| Scope            | `@appkit4/*`                                                                           |

Packages used by this SPA (Angular 19):

| Package                        | Version (pin)                        |
| ------------------------------ | ------------------------------------ |
| `@appkit4/styles`              | 4.11.x                               |
| `@appkit4/angular-components`  | 4.31.x                               |
| `@appkit4/angular-text-editor` | 4.10.x (only if rich text is needed) |

---

## One-time: authenticate (per machine)

JFrog uses **PwC Entra SSO** (no legacy Entrust token).

```bash
npm login --registry=https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm/
```

1. Press Enter when prompted to open the browser.
2. Sign in with **PwC-Azure-EntraID**.
3. Confirm CLI auth:

```bash
npm whoami --registry=https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm/
```

Expected: your PwC GUID/username (not an error).

Credentials are stored in your **user** `~/.npmrc` (never commit tokens).

---

## Project setup (already in repo)

`frontend/.npmrc` maps `@appkit4` to the AppKit registry:

```ini
always-auth=true
@appkit4:registry=https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm
```

After login, from `frontend/`:

```bash
npm install
# or explicit install (matches Angular 19):
npm install @appkit4/angular-components@4.31.0 @appkit4/styles@4.11.0 --save
```

Verify resolution:

```bash
npm ls @appkit4/angular-components @appkit4/styles
```

---

## Troubleshooting

| Symptom                            | Action                                                                                                                                               |
| ---------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| `401 Unauthorized` on `@appkit4/*` | Run `npm login` against the registry URL above.                                                                                                      |
| `404` / package not found          | Check VPN/corporate network; confirm scope `@appkit4` in `frontend/.npmrc`.                                                                          |
| SSL / certificate errors           | Corporate proxy; use PwC-managed Node/npm, not a bypass that breaks Artifactory.                                                                     |
| Works locally, fails in CI         | Add Artifactory npm token as a **secret** in GitHub Actions (FT.5-W01); map to `NODE_AUTH_TOKEN` or `.npmrc` in the pipeline — do not commit tokens. |

---

## CI (GitHub Actions)

For **FT.5-W01** and later pipelines:

1. Request/obtain an Artifactory identity token with read access to `g00020-pwc-gx-digital-appkit-npm`.
2. Store as repository secret (e.g. `APPKIT_NPM_TOKEN`).
3. In the workflow, before `npm ci` in `frontend/`:

```yaml
- name: Configure AppKit npm registry
  working-directory: frontend
  run: |
      echo "@appkit4:registry=https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm" >> .npmrc
      echo "//artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm/:_authToken=${{ secrets.APPKIT_NPM_TOKEN }}" >> .npmrc
      echo "always-auth=true" >> .npmrc
```

---

## Related docs

- [`docs/appkit4/getting-started/installation.md`](../appkit4/getting-started/installation.md)
- [`docs/appkit4/guides/source/installation-angular.md`](../appkit4/guides/source/installation-angular.md) — full install + `angular.json` assets
- [`docs/technical/18-frontend-architecture.md`](../technical/18-frontend-architecture.md) — SPA structure

---

_AppKit NPM access — Legal Ai Ar_
