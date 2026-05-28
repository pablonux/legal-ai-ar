# Search & Filter

**Category:** Search

**Portal:** [https://appkit.pwc.com/appkit4/content/pattern_detail?name=Search%20%26%20Filter&background=transparent](https://appkit.pwc.com/appkit4/content/pattern_detail?name=Search%20%26%20Filter&background=transparent)

**Pattern list:** [https://appkit.pwc.com/appkit4/content/pattern-list](https://appkit.pwc.com/appkit4/content/pattern-list)

## Portal assets

On the pattern detail page (link above):

- **Overview / Usage** — behavior and layout
- **Get Figma file** — design specs and variants (PwC / CT&I Figma)
- **Download source code** — React ZIP (re-download anytime from the portal)

## In-repo React reference (adapt to Angular)

`patterns/source/react/search-filter/`

Extracted variant folders:
- `search-filter-solid`

## Angular implementation

`search` + `filter` / `field` in toolbar; often above `table` or `list`.

## React → Angular notes

1. Map `className` → `class` on host elements where needed.
2. Replace JSX with Angular templates; keep `ap-*` / `ap-pattern-*` classes from React SCSS where applicable.
3. Import modules from `@appkit4/angular-components/*` (see [components/INDEX.md](../components/INDEX.md)).
4. Use design tokens from [design-tokens/README.md](../design-tokens/README.md) — never hardcode colors/spacing.
5. Cross-check layout on the portal (Figma / Live Preview) if spacing or states are unclear.
