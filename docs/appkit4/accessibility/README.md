# Accessibility

AppKit components are built for PwC accessibility standards. Follow component-specific guidance in the offline docs.

## Offline reference

[accessibility.md](../guides/source/accessibility.md) — full MCP export.

## Practices for Angular SPAs

- Use native AppKit components (correct roles/focus) instead of custom widgets.  
- Ensure `aria-label` / visible text on icon-only controls.  
- Respect modal focus trap and escape behavior per [modal.md](../components/modal.md).  
- Do not disable focus outlines without an approved alternative.  
- Test keyboard navigation on `header`, `navigation`, `drawer`, `tabs`, and `table`.

## Portal

https://appkit.pwc.com/appkit4 — accessibility documentation in the portal.
