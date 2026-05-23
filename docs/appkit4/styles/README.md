# Styles and themes

Global styling for AppKit 4 Angular SPAs: base styles, themes, typography, elevation, and grid.

## Offline reference

| Category | File |
|----------|------|
| Theme imports | [styles.md](styles.md) |
| Colors | [colors.md](colors.md) |
| Typography | [typography.md](typography.md) |
| Elevation | [elevation.md](elevation.md) |
| Grid | [grid.md](grid.md) |

## Themes and CDN (not in repo)

- **Recommended:** SCSS from npm `@appkit4/styles` — see [styles.md](styles.md) section 2.  
- **CDN:** minified CSS on [AppKit CDN](../references/portal-links.md#themes-and-fonts) (e.g. `appkit.min.css`, theme variants).

## Ecosystem

- **Classic:** [appkit4-classic.md](../ecosystems/appkit4-classic.md)  
- **Re-Branded:** [appkit4-rebranded.md](../ecosystems/appkit4-rebranded.md)

Import **one** theme chain per application.

## Angular `angular.json`

Add AppKit global styles before application styles so components inherit tokens and grid utilities.

## Portal

https://appkit.pwc.com/appkit4 — Styleguide section.
