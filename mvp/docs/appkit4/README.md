# AppKit 4 — Angular SPA documentation

Complete documentation for building **PwC-branded Angular SPAs** with AppKit 4 (Classic and Re-Branded).

## Start here

| Document | Purpose |
|----------|---------|
| [INDEX.md](INDEX.md) | Full map of topics |
| [AGENTS.md](AGENTS.md) | Mandatory rules for AI assistants |
| [getting-started/](getting-started/) | Install, versions, new SPA checklist |
| [references/portal-links.md](references/portal-links.md) | Live AppKit URLs (patterns, Figma, ZIP) |

## Scope

- **Angular 19** (align `@angular/cdk` with `@angular/core`)
- **AppKit `@appkit4/angular-components` 4.31.x**
- **Ecosystems:** AppKit 4 Classic and **AppKit 4 Re-Branded**

## Repository layout

| Folder | Content |
|--------|---------|
| [components/](components/) | Curated stubs + [reference/](components/reference/) full API |
| [patterns/](patterns/) | 15 UX guides + `source/react/` reference |
| [design-tokens/](design-tokens/) | SCSS tokens |
| [styles/](styles/) | Themes, typography, grid |
| [icons/](icons/) | Icon catalog |
| [guides/source/](guides/source/) | Raw MCP exports |
| [ecosystems/](ecosystems/) | Classic vs Re-Branded |
| [layout-shell/](layout-shell/) | App chrome |
| [accessibility/](accessibility/) | A11y |
| [references/](references/) | Asset map + portal links |

**Not in repo:** Figma, pattern ZIPs, CDN CSS/fonts — use [portal](references/portal-links.md) and npm `@appkit4/styles`

## Maintenance

Documentation is **edited manually**. Optional: `scripts/generate-from-offline.py` to refresh pattern stubs.
