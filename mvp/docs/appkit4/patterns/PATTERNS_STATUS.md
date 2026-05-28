# UX patterns — inventory

## Summary

| | Count |
|---|-------|
| Patterns in catalog | 15 |
| With in-repo React reference | 12 patterns, **25 variants** in `patterns/source/react/` |
| Figma-only on portal (no React download) | **3** |
| Live downloads (ZIP / Figma) | [AppKit pattern list](https://appkit.pwc.com/appkit4/content/pattern-list) |

## Figma-only (portal → Get Figma file)

| Pattern | Slug | Portal |
|---------|------|--------|
| Dashboard Builder | `dashboard-builder` | [Open](https://appkit.pwc.com/appkit4/content/pattern_detail?name=Dashboard%20Builder&background=transparent) |
| Page Templates | `page-templates` | [Open](https://appkit.pwc.com/appkit4/content/pattern_detail?name=Page%20Templates&background=transparent) |
| Table Row Actions | `table-row-actions` | [Open](https://appkit.pwc.com/appkit4/content/pattern_detail?name=Table%20Row%20Actions&background=transparent) |

Implement using Figma from the portal plus [components/reference/](../components/reference/).

## With React reference (in-repo)

In-repo under `patterns/source/react/<slug>/<variant>/`. New ZIPs: download from the portal (**Download source code**) — see [../scripts/extract-pattern-zips.py](../scripts/extract-pattern-zips.py).

| Pattern | Variants (folder names) |
|---------|-------------------------|
| 404 / 500 Error Page | `error-page-404`, `error-page-500` |
| Form template | `form-default`, `form-2col`, `form-login`, `form-register` |
| Card | `card` |
| FAQs | `faq-default` |
| Hero Banner | 4× `hero-banner-*` |
| Delete Confirmation | `confirmation-default`, `confirmation-additional` |
| Multi-purpose action menu | `multiple-action-menu` |
| Page anchors | `page-anchors-default` |
| Page header with back button | small / medium / large |
| Search & Filter | `search-filter-solid` |
| Data List Header | `data-list-header-default`, `data-list-header-solid` |
| Login Page | `login-default`, `login-fractional`, `login-generic` |

## Angular adaptation

1. **In-repo React** — composition and `ap-pattern-*` in `patterns/source/react/`.
2. **Portal Figma** — spacing and variants ([portal links](../references/portal-links.md)).
3. **`components/reference/`** + **`design-tokens/`** — API and tokens.
