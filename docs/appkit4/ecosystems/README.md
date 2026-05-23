# AppKit ecosystems (Classic vs Re-Branded)

This project documents **two** ecosystems for general PwC Angular SPAs:

| Ecosystem | Primary accent | Typical CSS |
|-----------|----------------|-------------|
| **AppKit 4** (Classic) | PwC blue | `@appkit4/styles` or [CDN](../references/portal-links.md#themes-and-fonts) |
| **AppKit 4 Re-Branded** | Orange accent | Re-branded theme bundle from `@appkit4/styles` |

**AppKit New Era** (dark orange / glass) is CT&I-specific and is not covered here unless a product explicitly requires it.

## Choosing

- Default new internal apps: **Classic**, unless brand guidelines say Re-Branded.  
- Preference center UI on the portal shows three cards: AppKit 4, AppKit 4 Re-Branded, AppKit New Era — match the product’s registered preference.

## Guides

- [appkit4-classic.md](appkit4-classic.md)  
- [appkit4-rebranded.md](appkit4-rebranded.md)

## Tokens and styles

Re-Branded may use alternate token sets in MCP (`*_new_era` is for New Era, not Re-Branded). For this repo, use offline:

- `design-tokens/`  
- `styles/`

Confirm active theme with design if colors look wrong.
