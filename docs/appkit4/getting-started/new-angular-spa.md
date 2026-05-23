# New Angular SPA checklist

Use this when bootstrapping a greenfield AppKit 4 Angular application.

## 1. Product decisions

- [ ] Confirm **AppKit 4 Classic** or **Re-Branded** with design ([ecosystems](../ecosystems/README.md))  
- [ ] Confirm **Angular 19** and AppKit **4.31.x**  
- [ ] Pick layout: header-only, header + sidebar, or sidebar-only ([layout-shell](../layout-shell/README.md))

## 2. Install

- [ ] `@appkit4/styles`, `@appkit4/angular-components`, optional `@appkit4/angular-text-editor`  
- [ ] Global theme CSS + fonts ([installation](installation.md), [styles](../styles/README.md))

## 3. Application shell

- [ ] `header` (+ product title, utilities)  
- [ ] `navigation` or `drawer` for primary nav  
- [ ] `footer` if required  
- [ ] Main content in `ap-container` / `row` / `col-*`  
- [ ] Section content inside `ap-panel` with `[title]`

## 4. Design tokens

- [ ] Load utility/spacing tokens from [design-tokens](../design-tokens/README.md)  
- [ ] No hardcoded brand colors

## 5. Features

- [ ] For each screen, check [patterns](../patterns/INDEX.md) then [components](../components/INDEX.md)  
- [ ] Figma-only patterns: Dashboard Builder, Page Templates, Table Row Actions — **Get Figma file** on portal ([portal-links](../references/portal-links.md))

## 6. Verify

- [ ] `ng build` clean  
- [ ] `ng serve` — no console errors  
- [ ] Keyboard/focus spot-check ([accessibility](../accessibility/README.md))

## Portal reference

https://appkit.pwc.com/appkit4
