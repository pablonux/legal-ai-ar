---
component: panel
framework: angular
---

# Panel Component

## Overview

AppKit Panel component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Panel component:

- As the container space for displaying a group of related information or options in a compact space.
- Presenting detailed information or controls related to a specific item or task.
- Organizing and grouping content within a larger UI.

### Anatomy

**1. Header area:** Title or heading

**2. Body area:** Area for content and/or controls

**3. Container:** Defines the area of the panel. It can have different levels according to the depth token that is required.

### Variants

None

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-101509%26viewport%3D1588%252C-33854%252C0.46%26t%3Dxa7pxUpRWPFNa1EO-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Panels can contain content such tables, visualizations or metrics. Panels can also contain detail information or a form.
- Use appropriate padding to separate the content from the edges of the panel.
- Appkit's padding recommendations for panels and containers is at least 20px.
- Ensure the panel is easily skimmable, with clear visual breaks between sections of content.
- Make sure the panel is accessible to all users, including keyboard-only and screen reader users.
- Consider the context in which the panel will be used, and design it appropriately (e.g. consider its size, placement, and interaction with other UI elements).
- Tabs or pagination can be used with a table to switch between specific views.

#### How not to use

- Do not overpopulate panels.
- Do not display unrelated data or content within a panel.
- Do not place panels or cards within a panel.

### Behavior

- The panel should adjust its size and position as necessary, depending on the content and context.
- The panel should respond to user interactions (e.g. clicking a button) as expected.

### Accessibility

- Ensure main window is not focusable when panel is open, use WAI ARIA role="dialog" and aria-modal="true" for screen reader to identify that the window behind panel can not be interacted with.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 2


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | simple | `section: "example:1"` |
| 2 | header | `section: "example:2"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### simple


**Example #1** | **Variation**: simple | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { PanelModule } from '@appkit4/angular-components/panel';
```

#### HTML Template

```html
<div class='ap-simple-panel-container'>
  <ap-panel [bordered]='true' [title]="'Lorem ipsum dolor sit'">
    <div class='ap-panel-content'>
      Lorem ipsum dolor sit amet, consectetur adipiscing elit, sedos eiusmod temporincididunt ut labore et dolore magna 
      aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
    </div>
  </ap-panel>
</div>
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

 .ap-simple-panel-container {
   width: rem(540px);
 }
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### header


**Example #2** | **Variation**: header | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { PanelModule } from '@appkit4/angular-components/panel';
import { TabsModule } from '@appkit4/angular-components/tabs';
import { ButtonModule } from '@appkit4/angular-components/button';
```

#### HTML Template

```html
<div class='ap-panel-with-extra-container'>
  <ap-panel [bordered]='true' [title]="'Lorem ipsum dolor sit'" [extra]="extraTemplate">
    <div class='ap-panel-content'>
      Lorem ipsum dolor sit amet, consectetur adipiscing elit, sedos eiusmod tempor incididunt ut labore et dolore 
      magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip.
    </div>
    <div class='ap-panel-footer'>
      <ap-button [btnType]="'primary'" [label]="'Lorem'"></ap-button>
    </div>
  </ap-panel>
</div>
<ng-template #extraTemplate>
  <div class='ap-extra-template-container'>
    <ap-tabset [type]="'filled'">
      <ap-tab [icon]="icon1"></ap-tab>
      <ap-tab [icon]="icon2"></ap-tab>
    </ap-tabset>
    <button type="button" aria-label="Close" class="ap-modal-header-icon ap-modal-header-close">
      <span class="Appkit4-icon icon-close-outline height ap-font-medium"></span>
    </button>
  </div>
</ng-template>
```

#### TypeScript

```typescript
icon1: string = "grid-view-outline";
icon2: string = "menu-outline";
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

 .ap-panel-with-extra-container {
   width: rem(540px);

   .ap-panel .ap-panel-footer {
     display: flex;
     justify-content: flex-end;
     margin-top: $spacing-6;
   }
 }

 :host ::ng-deep .ap-extra-template-container{
   display: flex;

   .ap-tabset-container{
     flex-direction: row;
     width: rem(76px);
     margin-right: $spacing-2;

     .ap-tabset-toggle{
       width: rem(32px) !important;
       padding: $spacing-2 !important;
       height: rem(32px) !important;
     }
   }

   .ap-modal-header-icon {
     position: relative;
     width: rem(40px);
     height: rem(40px);
     padding: $spacing-3;
     background-color: transparent;
     color: $color-text-heading;

     &:hover {
       background-color: $color-background-hover;
       border-radius: $border-radius-2;
       cursor: pointer;
     }

     .height{
       height: rem(24px);
     }
   }
 }
```

<!-- /EXAMPLE:2 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| bordered | boolean | When specified, displays the border of the panel. | true | 4.0.0 |
| title | string \| TemplateRef<void> | Title of the panel. | - | 4.0.0 |
| extra | string \| TemplateRef<void> | Extra content which will be added to the header of the panel. | - | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |


<!-- /SECTION:properties -->
