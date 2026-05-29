# Modal

## Module import

```typescript
import { ModalModule } from '@appkit4/angular-components/modal';
```

Register in the consuming NgModule / standalone `imports` array.

## Full API reference

[modal.md](reference/modal.md) — complete usage, anatomy, examples, and “how not to use” from AppKit MCP.

## Portal

Browse components on the AppKit portal: [https://appkit.pwc.com/appkit4/content/component-list](https://appkit.pwc.com/appkit4/content/component-list) (search for **modal**).

## Rules

- Prefer this component over custom markup for the same UI pattern.
- Use `<ap-panel [title]="'...'">` when wrapping section content.
- Verify required inputs/outputs in the reference doc before shipping.
