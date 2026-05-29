# FAQs

**Category:** Miscellaneous

**Portal:** [https://appkit.pwc.com/appkit4/content/pattern_detail?name=FAQs&background=transparent](https://appkit.pwc.com/appkit4/content/pattern_detail?name=FAQs&background=transparent)

**Pattern list:** [https://appkit.pwc.com/appkit4/content/pattern-list](https://appkit.pwc.com/appkit4/content/pattern-list)

## Portal assets

On the pattern detail page (link above):

- **Overview / Usage** — behavior and layout
- **Get Figma file** — design specs and variants (PwC / CT&I Figma)
- **Download source code** — React ZIP (re-download anytime from the portal)

## In-repo React reference (adapt to Angular)

`patterns/source/react/faqs/`

Extracted variant folders:
- `faq-default`

## Angular implementation

`accordion` for Q&A sections; `panel` for page sections.

## React → Angular notes

1. Map `className` → `class` on host elements where needed.
2. Replace JSX with Angular templates; keep `ap-*` / `ap-pattern-*` classes from React SCSS where applicable.
3. Import modules from `@appkit4/angular-components/*` (see [components/INDEX.md](../components/INDEX.md)).
4. Use design tokens from [design-tokens/README.md](../design-tokens/README.md) — never hardcode colors/spacing.
5. Cross-check layout on the portal (Figma / Live Preview) if spacing or states are unclear.
