# AppKit Accessibility Rules

## WCAG AA Compliance

- Maintain contrast ratio ≥ 4.5:1 for body text, ≥ 3:1 for large text
- Minimum base font size: 14px (use `$typography-body-s` or larger)
- Line height ≥ 1.4 (AppKit typography tokens handle this)

## Keyboard Navigation

- All interactive elements must be keyboard accessible (Tab / Enter / Escape)
- Never remove or hide focus outlines
- Ensure logical tab order follows visual layout
- Use `aria-expanded`, `aria-controls` for collapsible elements (Accordion, Drawer)

## Labels & ARIA

- All buttons, links, and form inputs must have accessible labels
- Icon-only buttons require `aria-label`
- Decorative icons must have `aria-hidden="true"`
- Use `aria-current="page"` on active navigation items
- Associate form labels with inputs using `label` prop or `aria-labelledby`

## Touch Targets

- Minimum touch target: 44×44px
- Use appropriate button sizes for primary actions

## Color & State

- Never rely on color alone to convey information
- Always pair colors with icons, text, or patterns for states (error, success, warning)
- Use AppKit status tokens: `$color-background-danger`, `$color-background-success`, `$color-background-warning`

## Never

- Remove focus indicators
- Use color alone to indicate state
- Skip `aria-label` on icon-only buttons
- Use contrast below WCAG AA standards
- Hard-code colors bypassing AppKit tokens
