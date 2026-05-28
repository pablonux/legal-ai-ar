# AppKit 4 Re-Branded

Alternate PwC theme with **orange** primary accent (visible in the portal preference center as “AppKit 4 Re-Branded”).

## Visual differences (vs Classic)

- Primary buttons and selected controls use the orange palette.  
- Preference center screenshot reference: orange border on the Re-Branded card when selected.  
- Same component set (`@appkit4/angular-components`) — change is predominantly **theme CSS**, not different Angular APIs.

## CSS

- Use the Re-Branded theme entry from `@appkit4/styles` (see full import list in [styles/README.md](../styles/README.md) and offline [styles.md](styles.md)).  
- Do not mix Classic and Re-Branded theme files in the same build.

## Angular SPA

1. Select Re-Branded in product / design sign-off.  
2. Wire global styles once in `angular.json`.  
3. Use the same components and patterns as Classic ([components](../components/INDEX.md), [patterns](../patterns/INDEX.md)).

## Figma and patterns

Re-Branded variants are on the portal — open a pattern → **Get Figma file**. See [portal-links.md](../references/portal-links.md).

## Portal

https://appkit.pwc.com/appkit4 — switch ecosystem in the preference center to preview.
