# Design tokens

Use AppKit **SCSS/CSS variables** for color, spacing, typography, elevation, radius, blur, and opacity — never hardcode design values in feature code.

## Offline reference (full)

| Section | File |
|---------|------|
| All | [all.md](all.md) |
| Utility | [utility.md](utility.md) |
| Primary | [primary.md](primary.md) |
| Status | [status.md](status.md) |
| Data | [data.md](data.md) |
| Neutral | [neutral.md](neutral.md) |
| Spacing | [spacing.md](spacing.md) |
| Typography | [typography.md](typography.md) |
| Border radius | [border-radius.md](border-radius.md) |
| Elevation | [elevation.md](elevation.md) |
| Blur | [blur.md](blur.md) |
| Opacity | [opacity.md](opacity.md) |

## Usage in Angular

1. Ensure global tokens are loaded via `@appkit4/styles` (see [styles](../styles/README.md)).  
2. In component SCSS, reference token variables documented in the offline files.  
3. Re-Branded vs Classic may change primary/status hues — verify with the active theme.

## Portal

Design token browser: https://appkit.pwc.com/appkit4 (styleguide section).

## AI rule

Before inventing a color or spacing value, open the relevant offline token file and use an existing token name.
