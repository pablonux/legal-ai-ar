# Form template

**Category:** Input and form

**Portal:** [https://appkit.pwc.com/appkit4/content/pattern_detail?name=Form%20template&background=transparent](https://appkit.pwc.com/appkit4/content/pattern_detail?name=Form%20template&background=transparent)

**Pattern list:** [https://appkit.pwc.com/appkit4/content/pattern-list](https://appkit.pwc.com/appkit4/content/pattern-list)

## Portal assets

On the pattern detail page (link above):

- **Overview / Usage** — behavior and layout
- **Get Figma file** — design specs and variants (PwC / CT&I Figma)
- **Download source code** — React ZIP (re-download anytime from the portal)

## In-repo React reference (adapt to Angular)

`patterns/source/react/form-template/`

Extracted variant folders:
- `form-default`
- `form-2col`
- `form-login`
- `form-register`

## Angular implementation

Compose with `field`, `dropdown`, `checkbox`, `radio`, `button`; wrap sections in `ap-panel` with `[title]`.

## React → Angular notes

1. Map `className` → `class` on host elements where needed.
2. Replace JSX with Angular templates; keep `ap-*` / `ap-pattern-*` classes from React SCSS where applicable.
3. Import modules from `@appkit4/angular-components/*` (see [components/INDEX.md](../components/INDEX.md)).
4. Use design tokens from [design-tokens/README.md](../design-tokens/README.md) — never hardcode colors/spacing.
5. Cross-check layout on the portal (Figma / Live Preview) if spacing or states are unclear.
