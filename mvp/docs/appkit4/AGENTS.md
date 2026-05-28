# AppKit 4 — rules for AI assistants

Apply these rules when implementing or reviewing **Angular** UI in a SPA that uses AppKit 4.

## Project assumptions

- **Framework:** Angular 19 (match `@angular/cdk` to `@angular/core`).
- **Packages:** `@appkit4/angular-components` **4.31.x** (or latest version compatible with the repo).
- **Ecosystems in scope:** **AppKit 4 Classic** and **AppKit 4 Re-Branded** — confirm which one the product uses before choosing CSS theme files.
- **Documentation:** This folder (`docs/appkit4`) + full API in `components/reference/`.

## Mandatory workflow

### Before any UI code

1. Check [components/INDEX.md](components/INDEX.md) or search `components/reference/` for an AppKit component.
2. Read the component’s **summary / usage / how not to use** in `components/reference/<name>.md`.
3. For page-level UX, check [patterns/INDEX.md](patterns/INDEX.md) and the matching pattern guide.
4. Use **design tokens** — read `design-tokens/`; never guess hex, px, or font stacks.

### Layout and shell

- Use **`header`**, **`navigation`** or **`drawer`**, **`footer`**, and AppKit **grid** (`ap-container`, `row`, `col-*`) — do not invent custom app chrome.
- Wrap page sections in **`<ap-panel [title]="'Section title'">`** — Panel without `title` violates AppKit guidance.
- MCP layout scaffolds (`appkit_get_patterns`) may be **React-only**; for Angular, follow [layout-shell/README.md](layout-shell/README.md) and Figma/page templates.

### Angular syntax

- Panel: `<ap-panel [title]="'My section'">` … `</ap-panel>`
- Prefer `class` on native elements (not `className`).
- Import feature modules from `@appkit4/angular-components/<feature>` (see each [components/*.md](components/)).

### Icons

- Validate icon names against `icons/` before use.
- Portal: https://appkit.pwc.com/appkit4/content/pattern-list (icon search in live AppKit).

### Patterns

- **12 patterns** have React reference ZIPs under `source/react/` — adapt structure to Angular; do not copy JSX literally.
- **3 patterns** are **Figma-only** on the portal (no React download): Dashboard Builder, Page Templates, Table Row Actions — open pattern detail → **Get Figma file** + use [components/reference/](components/reference/).
- Always link users to the live pattern URL listed in each `patterns/<slug>.md`.

### Charts

AppKit does **not** ship charts. Use ng2-charts, ngx-charts, or D3 — not custom div/SVG chart hacks.

## Verification before claiming “AppKit compliant”

For each AppKit component used:

1. Re-read **usage** and **how not to use** in `components/reference/<name>.md`.
2. Confirm required props and forbidden nesting.
3. Run `ng build` / `ng serve` and fix template errors.

## NEVER

- Build custom buttons, inputs, modals, tables, tabs, etc., when an AppKit component exists.
- Hardcode token names, colors, spacing, or typography.
- Override AppKit CSS without design system approval.
- Use scraped `patterns/details/*.md` (unreliable).
- Assume Angular pattern scaffolds exist in MCP (often React-only stub for Angular).

## Live portal

https://appkit.pwc.com/appkit4 — requires PwC authentication for full content.
