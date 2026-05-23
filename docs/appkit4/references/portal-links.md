# AppKit 4 portal links

All live documentation requires **PwC authentication**.

| Area | URL |
|------|-----|
| Home | https://appkit.pwc.com/appkit4 |
| Component list | https://appkit.pwc.com/appkit4/content/component-list |
| Pattern list | https://appkit.pwc.com/appkit4/content/pattern-list |
| Styleguide | https://appkit.pwc.com/appkit4 (nav: Styleguide) |
| Preference center (Classic / Re-Branded) | https://appkit.pwc.com/appkit4 (user menu → preferences) |

## Pattern detail

Template (replace `{name}` with the pattern title from the catalog):

`https://appkit.pwc.com/appkit4/content/pattern_detail?name={name}&background=transparent`

Examples:

- Card: `...pattern_detail?name=Card&background=transparent`
- Login Page: `...pattern_detail?name=Login%20Page&background=transparent`
- Search & Filter: `...pattern_detail?name=Search%20%26%20Filter&background=transparent`

## On each pattern detail page

| Action | Use for |
|--------|---------|
| **Overview / Usage** tabs | Behavior and layout |
| **Download source code** | React reference (re-download anytime) |
| **Get Figma file** | Design specs and variants (CT&I Figma org) |
| **Live Preview** | Visual check |

Figma libraries and ZIPs are **not** stored in this repo — use the portal.

## Themes and fonts

| Source | URL / package |
|--------|----------------|
| **npm (recommended)** | `@appkit4/styles` — SCSS imports documented in [styles/styles.md](../styles/styles.md) |
| **CDN base** | https://appkitcdn.pwc.com/appkit4/cdn/styles/ |
| **Example (Classic light)** | https://appkitcdn.pwc.com/appkit4/cdn/styles/4.11.0/appkit.min.css |
| **Orange theme** | https://appkitcdn.pwc.com/appkit4/cdn/styles/4.11.0/themes/appkit.orange.min.css |

Pin the version segment (`4.11.0`) to match your `@appkit4/styles` release. Fonts ship with the npm package or CDN bundle — see the installation guide on the portal and [getting-started/installation.md](../getting-started/installation.md).

**Not in repo:** minified CSS copies under `assets/` were removed; fetch from CDN or use npm.
