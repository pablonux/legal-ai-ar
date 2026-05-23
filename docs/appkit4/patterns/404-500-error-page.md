# 404 / 500 Error Page

**Category:** Error handling

**Portal:** [https://appkit.pwc.com/appkit4/content/pattern_detail?name=404%20/%20500%20Error%20Page&background=transparent](https://appkit.pwc.com/appkit4/content/pattern_detail?name=404%20/%20500%20Error%20Page&background=transparent)

**Pattern list:** [https://appkit.pwc.com/appkit4/content/pattern-list](https://appkit.pwc.com/appkit4/content/pattern-list)

## Portal assets

On the pattern detail page (link above):

- **Overview / Usage** — behavior and layout
- **Get Figma file** — design specs and variants (PwC / CT&I Figma)
- **Download source code** — React ZIP (re-download anytime from the portal)

## In-repo React reference (adapt to Angular)

`patterns/source/react/404-500-error-page/`

Extracted variant folders:
- `error-page-404`
- `error-page-500`

## Angular implementation

Use `notice` or static content in `panel`; typography and spacing from design tokens. No dedicated error component.

## React → Angular notes

1. Map `className` → `class` on host elements where needed.
2. Replace JSX with Angular templates; keep `ap-*` / `ap-pattern-*` classes from React SCSS where applicable.
3. Import modules from `@appkit4/angular-components/*` (see [components/INDEX.md](../components/INDEX.md)).
4. Use design tokens from [design-tokens/README.md](../design-tokens/README.md) — never hardcode colors/spacing.
5. Cross-check layout on the portal (Figma / Live Preview) if spacing or states are unclear.
