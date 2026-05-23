# Installation (Angular SPA)

## Packages (Angular 19)

| Package | Version | Purpose |
|---------|---------|---------|
| `@appkit4/styles` | 4.11.x | Global SCSS/CSS |
| `@appkit4/angular-components` | **4.31.x** | UI components (except rich text) |
| `@appkit4/angular-text-editor` | 4.10.x | Rich text editor (`texteditor`) |

Align `@angular/cdk` with `@angular/core` (19.x with 19.x).

## Global styles

1. Import AppKit base styles per [styles/README.md](../styles/README.md).  
2. Import the **theme CSS** for your ecosystem (Classic or Re-Branded) from `@appkit4/styles` (SCSS) or the [AppKit CDN](../references/portal-links.md#themes-and-fonts).  
3. Configure `angular.json` `styles` array so tokens and grid load **before** app components.

## Feature modules

Import only the modules you need, e.g.:

```typescript
import { ButtonModule } from '@appkit4/angular-components/button';
import { PanelModule } from '@appkit4/angular-components/panel';
import { HeaderModule } from '@appkit4/angular-components/header';
```

See [components/INDEX.md](../components/INDEX.md) for all 35 imports.

## Peer setup

- Include AppKit fonts via `@appkit4/styles` assets or the CDN bundle linked in [styles/styles.md](../styles/styles.md).  
- Use AppKit grid classes for layout — not ad-hoc CSS Grid for page structure.

## Full procedure

See the complete MCP export: [installation-angular.md](../guides/source/installation-angular.md).

## Portal

https://appkit.pwc.com/appkit4 — Getting started section (requires PwC login).
