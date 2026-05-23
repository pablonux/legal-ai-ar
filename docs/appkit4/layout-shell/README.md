# Layout shell (Angular SPA)

AppKit applications share a common **chrome**: header, navigation (or drawer), optional footer, and a grid-based content area.

## Components

| Role | AppKit component | Offline API |
|------|------------------|-------------|
| Top bar / logo / utilities | `header` | [header.md](../components/header.md) |
| Primary nav (vertical) | `navigation` | [navigation.md](../components/navigation.md) |
| Primary nav (overlay) | `drawer` | [drawer.md](../components/drawer.md) |
| Page sections | `panel` | [panel.md](../components/panel.md) |
| Bottom bar | `footer` | [footer.md](../components/footer.md) |

## Grid

Use AppKit layout grid — **not** raw CSS Grid for page structure:

- `ap-container`  
- `row`  
- `col-*`  

Details: [styles/grid](../styles/README.md) and offline [grid.md](grid.md).

## Page templates pattern

Full shell layouts (header + sidebar + content) are documented as the **Page Templates** UX pattern (Figma via portal — no React download):

- [page-templates.md](../patterns/page-templates.md)  
- Portal: https://appkit.pwc.com/appkit4/content/pattern_detail?name=Page%20Templates&background=transparent

## MCP layout scaffolds

`appkit_get_patterns` (header-only-layout, header-sidebar-layout, sidebar-only-layout) often returns **React-only** examples. For Angular, compose the components above to match the portal **Page Templates** / Figma spec.

## Rules

- Always set `[title]` on `ap-panel`.  
- Do not duplicate header/sidebar markup outside AppKit components.  
- Match ecosystem theme (Classic vs Re-Branded) globally.
