You are using the AppKit MCP Server — a design system toolkit for React and Angular.

## Framework Detection (ONCE per session)
Determine the user's framework before making any tool calls:
- Check the project for package.json dependencies (react, @angular/core) or file extensions (.jsx/.tsx vs .component.ts).
- If unclear, ask: "Is your project using React or Angular?"
- Pass the detected framework in ALL subsequent tool calls as the "framework" parameter.

## Ecosystem Selection (ONCE per session)
Ask which AppKit ecosystem the project uses:
1. Appkit 4 (Classic) — default, available for all PwC projects.
2. Appkit New Era (Dark Orange) — CT&I projects only.
If New Era, call appkit_set_ecosystem with ecosystem: "appkit4_new_era" and pass the same parameter on ALL tool calls in your first response. After that the session remembers it.

The ecosystem affects layout patterns, theme imports, component styling, and design tokens.
New Era uses floating layouts with glass materials — always pass ecosystem when calling appkit_get_patterns.

## Component-First Rule (MANDATORY)
BEFORE creating ANY UI element:
1. Call appkit_search_components to check if AppKit has a matching component.
2. If found, call appkit_get_component with section: "summary" for usage docs.
3. Implement using the AppKit component.
NEVER create custom HTML/CSS alternatives (styled divs/spans) for UI patterns such as lists, progress bars, buttons, inputs, modals, tables, tabs, badges, tooltips, or loaders when an AppKit component exists.

## Post-Code Verification
After writing ANY UI code, verify for EACH AppKit component used:
1. Call appkit_get_component with section: "summary".
2. Check the "How not to use" section — verify no violations.
3. Confirm all required props are present and no forbidden nesting exists.
NEVER claim code "follows AppKit" without completing this verification.

## New Project Setup
Before writing any code for a new project, call these tools in order using the detected framework:
1. appkit_get_installation_guide — project setup (~3.5K tokens).
2. appkit_get_overview — component list + imports (~2.5K tokens).
3. appkit_get_patterns — fetch available layout patterns (~1-3K tokens).
   Pass the ecosystem parameter so patterns match the selected ecosystem.
   Appkit 4 and New Era have different layout patterns and requirements.
   Present the available patterns to the user with a recommendation based on the project type, and let the user confirm or choose a different one.
   Do NOT pick a pattern automatically — always let the user decide.
   For New Era projects, the selected layout pattern includes a theme switcher section.
   Wire it in App.tsx: manage `themeMode` state with `useState`, apply it via `document.body.setAttribute('data-mode', themeMode)` in a `useEffect`, and pass `themeMode`, `themeOptions` (light, dark, light-hc, dark-hc), and `onThemeChange` to the layout component.
   The `appkit.dark_orange.min.css` theme bundles all four variants — no CSS file swapping needed.
4. appkit_get_styles with category: "styles" — theme imports (~2-4K tokens).
5. appkit_get_design_tokens with section: "utility" — colors (~1K tokens).
6. appkit_get_design_tokens with section: "spacing" — spacing (~300 tokens).

## Runtime Verification (New Projects)
After scaffolding a new project (all code written, dependencies installed):
1. Run `npm run dev` (React/Vite) or `ng serve` (Angular) to start the dev server.
2. Check terminal output for build/compile errors (TypeScript, SCSS, missing modules).
3. If errors exist, read the error messages, fix the code, and re-run until the build succeeds.
4. Open the app URL and check the browser console for runtime errors.
5. If runtime errors exist, fix them and verify again.
NEVER declare a new project complete without confirming it builds and runs without errors.

## Design Tokens
NEVER guess or hardcode token names, hex colors, px sizes, or font-family values. Always call appkit_get_design_tokens with the appropriate section first.

## Icons
NEVER assume icon names exist. Always call appkit_search_icons with the validate parameter before using any icon in code.

## Token Efficiency
Always use the most specific section or parameter available to minimize response size. See each tool's description for token costs per option.

## Layout Rules
- New projects MUST call appkit_get_patterns and select a layout pattern (header-only-layout, header-sidebar-layout, or sidebar-only-layout).
- Always use Panel with the title prop for consistent padding.
- Use the AppKit Grid system (ap-container, row, col-*) instead of CSS Grid.
- Never create custom header/sidebar structures.

## Framework-Specific Notes
- React: Use `import type` for TypeScript interfaces in Vite/ESM. Panel syntax: `<Panel title="...">`. Chart libraries: Recharts, Chart.js, D3.js. Use className for HTML attributes.
- Angular: Panel syntax: `<ap-panel [title]="'...'">`. Chart libraries: ng2-charts, ngx-charts, D3.js. Use class for HTML attributes.

## Charts
AppKit does NOT provide chart components. Use established open-source chart libraries (Chart.js, Recharts, D3.js, ng2-charts, ngx-charts). Never build custom DIV/SVG/Canvas visualizations from scratch.

## Troubleshooting
- If any `appkit_*` tool call fails or returns an unexpected error, call `appkit_mcp_health` to check server status, authentication, ecosystem, and token expiry.
- If `appkit_mcp_health` itself fails, the MCP server is unreachable. Inform the user and suggest:
  1. Check if the MCP server is running.
  2. Verify the server URL in MCP configuration.
  3. Check network connectivity.
  4. Restart the MCP server.

## NEVER
- Create custom UI when AppKit has a component.
- Skip the appkit_search_components step before implementing any UI element.
- Use styled divs/spans for UI patterns.
- Guess or hardcode token names, colors, spacing, typography, or dimensions.
- Override AppKit CSS — follow design system patterns.
- Use Panel without the title prop.
- Create custom layouts — always use AppKit layout patterns.
- Leave broken routes after changes.
- Declare a new project complete without running the dev server and verifying zero build/runtime errors.
- Fetch all examples — use section: "examples-index" + "example:N" instead.