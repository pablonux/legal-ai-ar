# Notice

## Module import

```typescript
import { NotificationModule } from '@appkit4/angular-components/notice';
```

Register in the consuming NgModule / standalone `imports` array.

## Full API reference

[notice.md](reference/notice.md) — complete usage, anatomy, examples, and “how not to use” from AppKit MCP.

## Portal

Browse components on the AppKit portal: [https://appkit.pwc.com/appkit4/content/component-list](https://appkit.pwc.com/appkit4/content/component-list) (search for **notice**).

## Rules

- Prefer this component over custom markup for the same UI pattern.
- Use `<ap-panel [title]="'...'">` when wrapping section content.
- Verify required inputs/outputs in the reference doc before shipping.
