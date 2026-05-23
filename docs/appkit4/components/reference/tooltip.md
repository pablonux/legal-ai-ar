---
component: tooltip
framework: angular
---

# Tooltip Component

## Overview

AppKit Tooltip component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Tooltips:

- To provide additional information or context to the user without cluttering the UI.

### Anatomy

1. **Content:** The text that displays in the tooltip.

2. **Container:** The outer layer that encapsulates the tooltip component.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-132866%26viewport%3D1526%252C-54806%252C0.5%26t%3DULk5YzlUsOCHi6pI-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Use a tooltip where additional information would be helpful to the user.
- Use tooltips sparingly and only as needed.

#### How not to use

- Do not use tooltips for information that is vital to complete a task.
- Do not hide primary actions within a tooltip.
- Do not use a tool within a tooltip.

### Behavior

- Hovering over the anchor element shows the tooltip.
- Clicking on the anchor element shows or hides the tooltip.
- Moving the cursor away from the anchor element hides the tooltip.

### Accessibility

- Tooltip dialog is triggered and stays open when in focus or on hover.
- Tooltip dialog stays open when mouse hovers into dialog.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 1


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | Default | `section: "example:1"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### Default


**Example #1** | **Variation**: None | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { TooltipModule } from '@appkit4/angular-components/tooltip';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [label]="'Tooltip'" ap-tooltip [tooltipId]="'tooltipDesc'"
  ariaDescribedby="tooltipDesc" [tooltipContent]="'Nature, is the natural, physical, material world or universe. It can refer to the phenomena of the physical world, and also to life in general.'">
</ap-button>
<!-- Tooltip with customized template -->
<!--
  <ap-button [btnType]="'primary'" [label]="'Tooltip'" ap-tooltip [tooltipId]="'tooltipDesc'" 
    ariaDescribedby="tooltipDesc" [tooltipContent]="tooltip">
  </ap-button>
  <ng-template #tooltip>
    <div>
      Nature, is the natural, physical, material world or universe. It can refer to the phenomena of the physical world, and also to life in general.
    </div>
  </ng-template>
-->
```

#### SCSS Styles

```scss
/*  
    Please set the baseUrl in tsconfig.json file
    "compilerOptions": {
      "baseUrl": "./"
    }
    If baseUrl is not set, please use a relative path.
*/
@import './node_modules/sass-rem/_rem.scss';

::ng-deep .ap-tooltip#tooltipDesc {
  width: rem(240px);
}
```

<!-- /EXAMPLE:1 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| tooltipContent | string\|TemplateRef | Display text or customized template of Tooltip. | '' | 4.0.0 |
| trigger | string: 'hover'\|'click' | The trigger of the tooltip. | 'hover' | 4.0.0 |
| direction | string: 'top'\|'top-left'\|'top-right'\|'right'\|'right-top'\|'right-bottom'\|'bottom'\|'bottom-left'\|'bottom-right'\|'left'\|'left-top'\|'left-bottom' | The direction of the tooltip. | 'right' | 4.0.0 |
| distance | number | The distance between the tooltip content and the host element. | 8 | 4.0.0 |
| hideTooltipOnBlur | boolean | When specified, hides the tooltip when the host element is blurred. | true | 4.0.0 |
| tooltipId | string | The id of Tooltip. Notice: "This value should be same as the value of aria-describedby." | The current timestamp | 4.0.0 |
| tooltipStyle | string | The inline style of the component. | '' | 4.5.0 |
| tooltipStyleClass | string | The style class names of the component. | '' | 4.5.0 |
| tooltipDisabled | boolean | When present, it specifies that the component should be disabled when trigger is 'hover'. | false | 4.0.0 |
| duration | number | When trigger is 'hover' and mouse leaves the host element, the time(milliseconds) that the tooltip will display. | 200 | 4.0.0 |
| autoLoad | boolean | If true, generate tooltip element on initialization. Otherwise, generate tooltip only when hovering or clicking on the host element. If set to false, then the accessibility will fail. | true | 4.0.0 |
| stopPropagation | boolean | When specified, stops propagation of the event when the trigger button is clicked, only works when trigger is 'click'. | true | 4.0.0 |
| appendAfterTarget | boolean | If true, append the tooltip element after the host element of the tooltip. If false, append the tooltip element to the body element. | false | 4.5.0 |


<!-- /SECTION:properties -->
