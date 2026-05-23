---
guide: installation-angular
framework: angular
---

# Getting Started with AppKit for Angular

AppKit designs are built into our HTML/CSS components, which can be used by any web UI. It is now possible to rapidly create high-resolution mockups, prototypes and develop PwC branded web-based products. AppKit provides the resources needed to rapidly create hi-res mockups, prototypes and develop PwC branded Angular applications.

---

# Appkit Version Information

## Latest Versions

| Package | Version | Description |
|---------|---------|-------------|
| `@appkit4/styles` | 4.11.0 | Basic package including all style files |
| `@appkit4/angular-components` | 4.31.0 | Angular components (excluding Rich text editor) (Angular 19, 20) |
| `@appkit4/angular-components` | 4.22.3 | Angular components (excluding Rich text editor) (Angular 17, 18) |
| `@appkit4/angular-text-editor` | 4.10.0 | Angular Rich text editor |

---

## Angular Version Compatibility

**Recommended:** Use the latest Appkit version (4.31.0) with Angular 19, 20 for new projects. For Angular 17, 18 projects, use 4.22.3.

**Note:** @angular/cdk (Component Dev Kit) is tightly coupled to the Angular framework version. If project uses @angular/core@19.x.x, use @angular/cdk@19.x.x. If project uses @angular/core@20.x.x, use @angular/cdk@20.x.x.

| Appkit Version | Angular Support | Ecosystems |
|----------------|-----------------|------------|
| **4.31.0** (Latest) | Angular 19, 20 | Appkit 4, Appkit New Era |
| 4.30.0 | Angular 19, 20 | Appkit 4 |
| 4.29.1 | Angular 19, 20 | Appkit 4 |
| 4.29.0 | Angular 19, 20 | Appkit 4 |
| 4.28.0 | Angular 19, 20 | Appkit 4 |
| 4.27.0 | Angular 19, 20 | Appkit 4 |
| 4.26.0 | Angular 19, 20 | Appkit 4 |
| 4.25.0 | Angular 19 | Appkit 4 |
| 4.24.0 | Angular 19 | Appkit 4 |
| 4.23.0 | Angular 19 | Appkit 4 |
| **4.22.3** (Latest for Angular 17, 18) | Angular 17, 18 | Appkit 4 |
| 4.22.1 | Angular 17, 18 | Appkit 4 |
| 4.22.0 | Angular 17, 18 | Appkit 4 |
| 4.21.0 | Angular 17, 18 | Appkit 4 |
| 4.20.0 | Angular 17, 18 | Appkit 4 |
| 4.19.0 | Angular 17, 18 | Appkit 4 |
| 4.18.0 | Angular 17, 18 | Appkit 4 |
| 4.17.0 | Angular 17, 18 | Appkit 4 |
| 4.16.0 | Angular 17, 18 | Appkit 4 |
| 4.15.0 | Angular 17, 18 | Appkit 4 |
| 4.14.0 | Angular 17 | Appkit 4 |
| 4.13.0 | Angular 17 | Appkit 4 |
| 4.12.0 | Angular 17 | Appkit 4 |
| 4.11.0 | Angular 16 | Appkit 4 |
| 4.10.0 | Angular 16 | Appkit 4 |
| 4.9.2 | Angular 16 | Appkit 4 |
| 4.9.1 | Angular 16 | Appkit 4 |
| 4.9.0 | Angular 16 | Appkit 4 |
| 4.8.1 | Angular 15 | Appkit 4 |
| 4.8.0 | Angular 15 | Appkit 4 |
| 4.7.0 | Angular 15 | Appkit 4 |
| 4.6.1 | Angular 13 | Appkit 4 |
| 4.6.0 | Angular 13 | Appkit 4 |
| 4.5.0 | Angular 13 | Appkit 4 |
| 4.4.0 | Angular 13 | Appkit 4 |
| 4.3.0 | Angular 13 | Appkit 4 |
| 4.2.0 | Angular 13 | Appkit 4 |
| 4.1.0 | Angular 13 | Appkit 4 |
| 4.0.0 | Angular 13 | Appkit 4 |

---

## Appkit New Era (Dark Orange Theme)

Appkit New Era theming ships as part of regular `@appkit4/*` releases. Pick the ecosystem at consumption time by importing the appropriate theme CSS and layout.

**First version with Appkit New Era support:**

- `@appkit4/styles` — **4.11.0**
- `@appkit4/angular-components` — **4.31.0**
- `@appkit4/angular-text-editor` — **4.10.0**
- `@appkit4/react-components` — **4.32.0**
- `@appkit4/react-text-editor` — **4.6.0**

See the **Ecosystems** column in the compatibility tables below for the full per-version matrix.

For setup instructions, ask for the **Appkit New Era** installation guide.

---

## Prerequisites

AppKit is available as an npm package on [PwC's JFrog Artifactory](https://artifacts-west.pwc.com/).

Private NPM Registry URL: `https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm/`

### Step 1: Authenticate with the Private NPM Registry (user level `.npmrc`)

JFrog Artifactory now supports Entra SSO, so the legacy Entrust + Identity Token flow is no longer required.

1. For Linux, macOS, or Windows users, open a terminal / cmd shell and enter the command below:

```bash
npm login --registry=https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm/
```

2. It will show instructions like below. Press ENTER to open the login page in the browser:

```bash
npm notice Log in on https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm/
Login at:
https://artifacts-west.pwc.com:443/ui/auth-provider/npm?uuid=xxxxxx
Press ENTER to open in the browser...
```

3. On the JFrog login page, click **Sign in with PwC-Azure-EntraID** and complete the SSO flow.
4. Once SSO completes, the CLI automatically creates the user-level `.npmrc` (if not present) with the auth token.
5. Then type `npm whoami` in the terminal — it should return your `GUID`. That means your npm auth is correctly configured.

### Step 2: Project-level `.npmrc` Configuration

It is recommended to have project level `.npmrc` with below configuration:

```.npmrc
always-auth=true
@appkit4:registry=https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm
```

---

## Create Angular Project

**ALWAYS use `npx` to run Angular CLI.** Do not install Angular CLI globally as it has npm version dependencies.

```bash
# Create new Angular 20 project (for latest AppKit 4.29.1) - RECOMMENDED
npx @angular/cli@20 new my-app

# Navigate to project directory
cd my-app
```

**Note:** Using `npx` ensures compatibility regardless of the npm version installed on your system.

---

## Install AppKit Packages

**IMPORTANT:** Install the AppKit version that matches your Angular version.

<!-- VERSION:install-angular-latest -->
```bash
# For Angular 19/20 - Install latest AppKit (RECOMMENDED)
npm install @appkit4/angular-components@4.31.0 @appkit4/angular-text-editor@4.10.0 @appkit4/styles@4.11.0 --save --registry=https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm/
```
<!-- /VERSION:install-angular-latest -->

**Or install latest without specifying version:**

```bash
npm install @appkit4/angular-components @appkit4/angular-text-editor @appkit4/styles --save --registry=https://artifacts-west.pwc.com/artifactory/api/npm/g00020-pwc-gx-digital-appkit-npm/
```

---

## Styles Import

After installing the `@appkit4/styles` package, you can import the styles using one of the following methods:

**IMPORTANT:** You must configure `angular.json` to copy assets before using SCSS imports with path variables.

### Step 1: Configure angular.json assets (Required)

Add the following to your `angular.json` under `projects > your-project > architect > build > options > assets`:

```json
{
  "projects": {
    "your-project": {
      "architect": {
        "build": {
          "options": {
            "assets": [
              "src/favicon.ico",
              "src/assets",
              {
                "glob": "**/*",
                "input": "node_modules/@appkit4/styles/images",
                "output": "/assets/appkit/images"
              },
              {
                "glob": "**/*",
                "input": "node_modules/@appkit4/styles/font",
                "output": "/assets/appkit/font"
              },
              {
                "glob": "**/*",
                "input": "node_modules/@appkit4/styles/font-icon",
                "output": "/assets/appkit/font-icon"
              }
            ]
          }
        }
      }
    }
  }
}
```

### Step 2: Define SCSS path variables in styles.scss

After configuring assets, use the output paths (not node_modules paths):

```scss
// Required: Configure asset paths
$ap-image-path: '/assets/appkit/images';
$ap-font-path: '/assets/appkit/font';
$ap-font-icon-path: '/assets/appkit/font-icon';

// Import theme (choose ONE)
@import '@appkit4/styles/scss/index.scss';                    // Blue Light (default)
@import '@appkit4/styles/scss/index.dark.scss';               // Blue Dark
@import '@appkit4/styles/scss/themes/index.orange.scss';      // Orange Light
@import '@appkit4/styles/scss/themes/index.orange.dark.scss'; // Orange Dark
// Same pattern for: teal, pink, red, black

// Optional: Import for custom styling
@import '@appkit4/styles/scss/variables';  // Design tokens
@import '@appkit4/styles/scss/mixin';      // Mixins like setElevation()
```

## Always Use AppKit Components

**IMPORTANT:** Always use AppKit components instead of native HTML elements. This ensures consistent styling, accessibility, and PwC branding.

| Instead of HTML | Use AppKit Component | Module Import |
|-----------------|---------------------|---------------|
| `<header>` | `<ap-header>` | `HeaderModule` from `@appkit4/angular-components/header` |
| `<footer>` | `<ap-footer>` | `FooterModule` from `@appkit4/angular-components/footer` |
| `<nav>` | `<ap-navigation>` | `NavigationModule` from `@appkit4/angular-components/navigation` |
| `<button>` | `<ap-button>` | `ButtonModule` from `@appkit4/angular-components/button` |
| `<input>` | `<ap-field>` | `FieldModule` from `@appkit4/angular-components/field` |
| `<textarea>` | `<ap-field>` | `FieldModule` from `@appkit4/angular-components/field` |
| `<select>` | `<ap-dropdown>` | `DropdownModule` from `@appkit4/angular-components/dropdown` |
| `<table>` | `<ap-table>` | `TableModule` from `@appkit4/angular-components/table` |
| `<div>` for cards | `<ap-panel>` | `PanelModule` from `@appkit4/angular-components/panel` |
| `<dialog>` / modal | `<ap-modal>` | `ModalModule` from `@appkit4/angular-components/modal` |
| `<input type="checkbox">` | `<ap-checkbox>` | `CheckboxModule` from `@appkit4/angular-components/checkbox` |
| `<input type="radio">` | `<ap-radio>` | `RadioModule` from `@appkit4/angular-components/radio` |
| Custom tabs | `<ap-tabs>` | `TabsModule` from `@appkit4/angular-components/tabs` |

---

## Module Imports

**IMPORTANT:** Module names follow the pattern `{ComponentName}Module` (e.g., `ButtonModule`, not `ApButtonModule`).

Each component module is imported from its specific path:

```typescript
// In your module file
import { ButtonModule } from '@appkit4/angular-components/button';
import { ModalModule } from '@appkit4/angular-components/modal';
import { DropdownModule } from '@appkit4/angular-components/dropdown';

@NgModule({
  imports: [
    ButtonModule,
    ModalModule,
    DropdownModule
    // ... other AppKit modules
  ]
})
export class YourModule { }
```

### ⚠️ Angular vs React Naming Differences

| Angular | React | Notes |
|---------|-------|-------|
| `ap-dropdown` | `Select` / `Combobox` | Different component name |
| `ap-toggle` | `Switch` | Different component name |
| `ap-field` | `Input` / `TextArea` | Separate components in React |
| `[btnType]="'primary'"` | `kind="primary"` | Different prop name |