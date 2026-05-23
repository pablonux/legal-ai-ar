---
component: tabs
framework: angular
---

# Tabs Component

## Overview

AppKit Tabs component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

They always contain at least two items and one tab is active at a time. Tabs can be used on full page layouts or in components such as modals, cards, or side panels.

### Anatomy

1. **Container:** The container that holds all the tab items.

2. **Tab item:** The clickable element that represents a tab.

3. **Active tab indicator:** A visual indicator that highlights the currently selected tab item.

4. **Disabled tab item:** A tab item that is not clickable or selectable.

5. **Icon:** Additional visual indicator to enhance the functionality and usability of the tab component.

6. **Label:** Describes the tab item.

### Variants

#### Underlined:

Underlined tabs work well when you have a larger number of tabs and want to conserve space.

#### Filled:

Filled tabs work well when you have a limited number of tabs and want to draw attention to the active tab. Filled tabs are visually more prominent than underlined tabs and can help the user quickly identify which tab is currently active.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-122226%26viewport%3D1204%252C-35551%252C0.36%26t%3D8LwnKirwplVnOov7-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Tabs should not be used to indicate progress. Use the progress indicator instead.
- Tabs should not be used if the user is comparing information in two groups, as this would result in the user having to click back and forth to complete a task.

#### How not to use

- Don't combine different modifier type in single group

### Behavior

- Tab Selection: When a tab is selected, it should visually indicate its active state to provide clear feedback to the user.
- Clicking on a tab should switch the content to the associated section.
- The active tab should visually indicate its active state to provide feedback to the user.
- Keyboard navigation should be supported, allowing users to navigate through tabs using the Tab key or arrow keys.
- Content loading should be smooth and efficient when switching between tabs.
- Responsive behavior should adapt the Tab component to different screen sizes and orientations.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 6


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | underline - label | `section: "example:1"` |
| 2 | filled - label | `section: "example:2"` |
| 3 | filled - label-overflow | `section: "example:3"` |
| 4 | filled - icon | `section: "example:4"` |
| 5 | filled - iconAndLabel | `section: "example:5"` |
| 6 | filled - iconAndLabel-overflow | `section: "example:6"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### underline - label


**Example #1** | **Variation**: underline | **Modifier**: label | **State**: None

#### Module Import

```typescript
import { TabsModule } from '@appkit4/angular-components/tabs';
```

#### HTML Template

```html
<ap-tabset [type]="'underline'" [(activeIndex)]="activeIndex" [overflow]="isOverflowTab">
  <ap-tab *ngFor="let tab of tabs; let i = index" [label]="tab.label" [disabled]="tab.disabled">
    <!-- custom content here -->
  </ap-tab>
</ap-tabset>
```

#### TypeScript

```typescript
tabs: any[] = [
  { label: 'Mail', icon: 'email-outline' },
  { label: 'Archive', icon: 'archive-outline' },
  { label: 'Trash', icon: 'delete-outline' },
  { label: 'Junk', icon: 'junk-outline', disabled: true }
];
isOverflowTab: boolean = false;
activeIndex: number = 0;
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

::ng-deep .ap-tabset-content {
  padding: $spacing-4 $spacing-3;
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### filled - label


**Example #2** | **Variation**: filled | **Modifier**: label | **State**: None

#### Module Import

```typescript
import { TabsModule } from '@appkit4/angular-components/tabs';
```

#### HTML Template

```html
<ap-tabset [type]="'filled'" [(activeIndex)]="activeIndex" [overflow]="isOverflowTab">
  <ap-tab *ngFor="let tab of tabs; let i = index" [label]="tab.label" [disabled]="tab.disabled">
    <!-- custom content here -->
  </ap-tab>
</ap-tabset>
```

#### TypeScript

```typescript
tabs: any[] = [
  { label: 'Mail', icon: 'email-outline' },
  { label: 'Archive', icon: 'archive-outline' },
  { label: 'Trash', icon: 'delete-outline' },
  { label: 'Junk', icon: 'junk-outline', disabled: true }
];
isOverflowTab: boolean = false;
activeIndex: number = 0;
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

::ng-deep .ap-tabset-content {
  padding: $spacing-4 $spacing-3;
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### filled - label-overflow


**Example #3** | **Variation**: filled | **Modifier**: label-overflow | **State**: None

#### Module Import

```typescript
import { TabsModule } from '@appkit4/angular-components/tabs';
```

#### HTML Template

```html
<ap-tabset [type]="'filled'" [(activeIndex)]="activeIndex" [overflow]="isOverflowTab">
  <ap-tab *ngFor="let tab of tabs; let i = index" [label]="tab.label" [disabled]="tab.disabled">
    <!-- custom content here -->
  </ap-tab>
</ap-tabset>
```

#### TypeScript

```typescript
tabs: any[] = [
  { label: 'London', icon: 'family-outline', disabled: false },
  { label: 'Moscow', icon: 'archive-outline',disabled: false },
  { label: 'Paris', icon: 'family-outline', disabled: false },
  { label: 'Dubai', icon: 'family-outline', disabled: false},
  { label: 'Tokyo', icon: 'family-outline', disabled: false },
  { label: 'Rome', icon: 'family-outline', disabled: false },
  { label: 'Madrid', icon: 'family-outline', disabled: false },
  { label: 'Toronto', icon: 'family-outline', disabled: false },
  { label: 'Singapore', icon: 'family-outline', disabled: false },
  { label: 'Berlin', icon: 'archive-outline', disabled: false },
  { label: 'Havana', icon: 'delete-outline', disabled: false },
  { label: 'Shanghai', icon: 'email-outline', disabled: false },
  { label: 'Hong Kong', icon: 'family-outline', disabled: false},
  { label: 'New York', icon: 'family-outline', disabled: false },
  { label: 'Bangkok', icon: 'delete-outline', disabled: false },
  { label: 'Toronto', icon: 'email-outline', disabled: false }
];
isOverflowTab: boolean = true;
activeIndex: number = 0;
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

::ng-deep .ap-tabset-content {
  padding: $spacing-4 $spacing-3;
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### filled - icon


**Example #4** | **Variation**: filled | **Modifier**: icon | **State**: None

#### Module Import

```typescript
import { TabsModule } from '@appkit4/angular-components/tabs';
```

#### HTML Template

```html
<ap-tabset [type]="'filled'" [(activeIndex)]="activeIndex">
  <ap-tab *ngFor="let tab of tabs; let i = index" [icon]="tab.icon" [disabled]="tab.disabled">
    <!-- custom content here -->
  </ap-tab>
</ap-tabset>
```

#### TypeScript

```typescript
tabs: any[] = [
  { label: 'Mail', icon: 'email-outline' },
  { label: 'Archive', icon: 'archive-outline' },
  { label: 'Trash', icon: 'delete-outline' },
  { label: 'Junk', icon: 'junk-outline', disabled: true }
];
isOverflowTab: boolean = false;
activeIndex: number = 0;
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

::ng-deep .ap-tabset-content {
  padding: $spacing-4 $spacing-3;
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### filled - iconAndLabel


**Example #5** | **Variation**: filled | **Modifier**: iconAndLabel | **State**: None

#### Module Import

```typescript
import { TabsModule } from '@appkit4/angular-components/tabs';
```

#### HTML Template

```html
<ap-tabset [type]="'filled'" [(activeIndex)]="activeIndex" [overflow]="isOverflowTab">
  <ap-tab *ngFor="let tab of tabs; let i = index" [label]="tab.label" [icon]="tab.icon" [disabled]="tab.disabled">
    <!-- custom content here -->
  </ap-tab>
</ap-tabset>
```

#### TypeScript

```typescript
tabs: any[] = [
  { label: 'Mail', icon: 'email-outline' },
  { label: 'Archive', icon: 'archive-outline' },
  { label: 'Trash', icon: 'delete-outline' },
  { label: 'Junk', icon: 'junk-outline', disabled: true }
];
isOverflowTab: boolean = false;
activeIndex: number = 0;
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

::ng-deep .ap-tabset-content {
  padding: $spacing-4 $spacing-3;
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### filled - iconAndLabel-overflow


**Example #6** | **Variation**: filled | **Modifier**: iconAndLabel-overflow | **State**: None

#### Module Import

```typescript
import { TabsModule } from '@appkit4/angular-components/tabs';
```

#### HTML Template

```html
<ap-tabset [type]="'filled'" [(activeIndex)]="activeIndex" [overflow]="isOverflowTab">
  <ap-tab *ngFor="let tab of tabs; let i = index" [label]="tab.label" [icon]="tab.icon" [disabled]="tab.disabled">
    <!-- custom content here -->
  </ap-tab>
</ap-tabset>
```

#### TypeScript

```typescript
tabs: any[] = [
  { label: 'London', icon: 'family-outline', disabled: false },
  { label: 'Moscow', icon: 'archive-outline', disabled: false },
  { label: 'Paris', icon: 'family-outline', disabled: false },
  { label: 'Dubai', icon: 'family-outline', disabled: false },
  { label: 'Tokyo', icon: 'family-outline', disabled: false },
  { label: 'Rome', icon: 'family-outline', disabled: false },
  { label: 'Madrid', icon: 'family-outline', disabled: false },
  { label: 'Toronto', icon: 'family-outline', disabled: false },
  { label: 'Singapore', icon: 'family-outline', disabled: false },
  { label: 'Berlin', icon: 'archive-outline', disabled: false },
  { label: 'Havana', icon: 'delete-outline', disabled: false },
  { label: 'Shanghai', icon: 'email-outline', disabled: false },
  { label: 'Hong Kong', icon: 'family-outline', disabled: false },
  { label: 'New York', icon: 'family-outline',disabled: false },
  { label: 'Bangkok', icon: 'delete-outline',disabled: false },
  { label: 'Toronto', icon: 'email-outline', disabled: false}
];
isOverflowTab: boolean = true;
activeIndex: number = 0;
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

::ng-deep .ap-tabset-content {
  padding: $spacing-4 $spacing-3;
}
```

<!-- /EXAMPLE:6 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-tabset

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| type | string: 'underline'\|'filled' | Type of the tabset. | 'underline' | 4.0.0 |
| activeIndex | number | Index of the current active tab. Two-way binding is supported. | 0 | 4.0.0 |
| activeIndexChange | EventEmitter<number> | Callback to invoke when the tab is selected. | - | 4.0.0 |
| tabsetId | string | The id string of tabset | Random string of 14 characters in length. | 4.0.0 |
| stretched | boolean | Stretch each tab and set the width of tabset to 100%. | false | 4.4.0 |
| responsive | boolean | Enable the responsive tabset. | false | 4.3.0 |
| pinActiveTab | boolean | Pin the selected tab to the left. Only works when property 'responsive' is 'true'. If true, please note that the property 'activeIndex' still represents the index of the active tab in the original tablist. | false | 4.3.0 |
| showBothIndicators | boolean | Display both left and right arrow indicators of the underline responsive tabset. Only works when property 'responsive' is 'true' and 'type' is 'underline'. By default, only the right arrow indicator is displayed. | false | 4.3.0 |
| overflow | boolean | Enable the overflow filled tabset. Only supported when there is a label. | false | 4.7.0 |
| overflowNumber | number | More than how many tabs start pouring out. Used with the overflow property. | 8 | 4.7.0 |
| onCollapseOverflowTabs | EventEmitter<Event> | Callback to invoke when the overflow tabs collapsed. | - | 4.8.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |

### ap-tab

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| label | string | Label of each tab. (htmlSnippet supported). | '' | 4.0.0 |
| icon | string | Name of the icon displayed on the left of the label. Only applicable when type = 'filled'. | '' | 4.0.0 |
| disabled | boolean | Disable the current tab. | false | 4.1.0 |
| tooltipContent | string | Tooltip property. The content of the tooltip above the tab. | '' | 4.0.0 |
| trigger | string: 'hover'\|'click' | Tooltip property. The trigger of Tooltip. | 'hover' | 4.0.0 |
| direction | string: 'top'\|'top-left'\|'top-right'\|'right'\|'right-top'\|'right-bottom'\|'bottom'\|'bottom-left'\|'bottom-right'\|'left'\|'left-top'\|'left-bottom' | Tooltip property. The direction of Tooltip. | 'top-right' | 4.0.0 |
| distance | number | Tooltip property. The distance between tooltip content and the tab. | 12 | 4.0.0 |
| tooltipDisabled | boolean | Tooltip property. When present, it specifies that the component should be disabled when trigger is hover. | false | 4.0.0 |
| duration | number | Tooltip property. When trigger is hover and mouse left the host element, the time that the tooltip will display, the unit is milliseconds. | 200 | 4.0.0 |
| hideTooltipOnBlur | boolean | Tooltip property. If true, hide the tooltip after the tab loses focus. | true | 4.0.0 |
| autoLoad | boolean | Tooltip property. If true, generate tooltip element on initialization. Otherwise, generate tooltip only when hovering or clicking on the host element. If set to false, then the accessibility will fail. | true | 4.0.0 |
| stopPropagation | boolean | Tooltip property. When specified, stops propagation of the event when the trigger button is clicked, only works when trigger is 'click'. | true | 4.0.0 |
| appendAfterTarget | boolean | Tooltip property. If true, append the tooltip element after the host element of the tooltip. If false, append the tooltip element to the body element. | false | 4.5.0 |
| tooltipStyle | string | Tooltip property. The inline style of the tooltip. | '' | 4.5.0 |
| tooltipStyleClass | string | Tooltip property. The style class names of the tooltip. | '' | 4.5.0 |
| tabId | string | The id string of tab. | Random string of 15 characters in length | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |


<!-- /SECTION:properties -->