---
component: drawer
framework: angular
---

# Drawer Component

## Overview

AppKit Drawer component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Drawer component: 

- Use the Drawer component to provide context-specific information and actions without disrupting the user's current workflow. 
- Full width/height is needed to show a lot of data.
- Selecting multiple items in bulk (i.e. table).
- Seeing a side-by-side of the main body is important.

### Anatomy

1.** Drawer container:** The main container element that holds the Drawer content.

2.** Header area:** Contains title, badge, and subtitle.

3.** Action icons:** Allows users to trigger a dropdown menu or close the Drawer. 

4.** Drawer content:** The area inside the Drawer container where the detailed view and fields are displayed. This content is meant to be provided by the users. 

5.** Resize handle:** Allows the user to adjust the width or height of the vertical or horizontal variation, respectively.

6.** CTA:** Optional buttons to prompt the user to take action.

### Variants

#### Default

Appears on top of the screen allowing for a side-by-side comparison with the underlying content.

#### Focused

Adds an overlay to the underlying content to focus the user on just the drawer.

 Both variants have a Horizontal and Vertical variation.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2F%2Fwww.figma.com%2Fproto%2FBmrAOae85hXrSMa1rpqGkQ%2FAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D248373-100991%26viewport%3D1347%252C-4408%252C0.25%26t%3DW8WjBZaCtzU3WOib-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Use the Drawer component to provide context-specific information and actions without disrupting the user's current workflow.
- Ensure that the trigger element is clearly visible and appropriately labeled to indicate its purpose.
- Design the Drawer content in a concise and structured manner, prioritizing the most relevant information and actions.
- Provide clear affordances for editing fields, such as distinct visual styles or explicit "Edit" buttons.
- Consider allowing users to dismiss the Drawer by clicking outside the Drawer container or pressing the Esc key.

#### How not to use

- Avoid overloading the Drawer with excessive information or actions, as it may lead to cognitive overload for users.
- Do not use the Drawer for critical alerts or notifications that require immediate attention. Consider using a different component for such scenarios.
- Do not rely solely on the Drawer for presenting essential functionality or features. It should complement the main interface rather than replace it.
- Do not use when info or data can easily fit in a modal.
- Do not use it when multiple steps are needed (a Wizard approach should be used instead).
- Do not use when the drawer would cover important parts of the screen such as a sidebar or notifications panel.

### Behavior

The Drawer component exhibits the following behavior:

- Opening and closing: The Drawer flies out smoothly from the specified side of the screen when triggered and slides back in when closed.
- Modal behavior: The Drawer typically behaves as a modal, meaning it prevents interactions with the underlying content while open.
- Scrolling: If the content within the Drawer exceeds the available height, it should provide scrollbars to ensure all content remains accessible.
- Focus management: When the Drawer opens, it should set focus to the first interactive element within the Drawer content to ensure keyboard accessibility.
- Resizing: If the viewport size changes while the Drawer is open, the Drawer should adapt accordingly to remain fully visible and usable.
- Resize/Draggable: Drawer component can be resized manually by users by dragging the drag handle. The width should be set to where the users topped the drag action (by releasing the cursor) and the elements in the drawer should adapt to the size.

### Accessibility

- Ensure that the trigger element has appropriate text or labeling for screen readers to understand its purpose.
- Set the aria-expanded attribute on the trigger element to reflect the open or closed state of the Drawer.
- Apply appropriate aria-modal attributes to indicate that the Drawer behaves as a modal dialog.
- Implement keyboard accessibility by allowing users to navigate within the Drawer using the Tab key and close it using the Esc key.
- Provide alternative text descriptions for non-textual content within the Drawer, such as images or icons.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 4


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | vertical - default | `section: "example:1"` |
| 2 | vertical - focused | `section: "example:2"` |
| 3 | horizontal - default | `section: "example:3"` |
| 4 | horizontal - focused | `section: "example:4"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### vertical - default


**Example #1** | **Variation**: vertical | **Modifier**: default | **State**: None

#### Module Import

```typescript
                import { DrawerModule } from '@appkit4/angular-components/drawer';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from '@appkit4/angular-components/badge';
import { ToggleModule} from '@appkit4/angular-components/toggle';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
import { RadioModule } from '@appkit4/angular-components/radio';
import { TabsModule } from '@appkit4/angular-components/tabs';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [label]="'Open drawer'" (onClick)="this.isVerticalDrawerVisible=true"></ap-button>
<ap-drawer [(visible)]="isVerticalDrawerVisible" [placement]="'right'" [mask]="false"
[drawerTitle]="'Drawer'" [resizable]="true">
  <ng-template ngTemplate="icons">
      <button type="button" aria-label="more"
          class="ap-drawer-header-icon ap-drawer-header-more">
          <span class="Appkit4-icon icon-horizontal-more-outline"></span>
      </button>
  </ng-template>
  <ng-template ngTemplate="header">
    <ap-badge value="New" type="primary" size="small"></ap-badge>
  </ng-template>
  <ap-tabset [type]="'underline'" [(activeIndex)]="activeIndexInDrawer">
    <ap-tab *ngFor="let subtab of drawerTabs" [label]="subtab.label"></ap-tab>
    <div *ngIf="activeIndexInDrawer===0" class="tab-content">
      <fieldset>
        <legend class="drawer-info-title">
            Components default mode
        </legend>
        <ng-container *ngFor="let item of defaultModelist">
            <ap-radio name="components default mode"
                [value]="item.name" [(ngModel)]="defaultModeSelect">
                <span>{{ item.name }}</span>
            </ap-radio>
        </ng-container>
      </fieldset>
      <fieldset>
        <legend class="drawer-info-title">
            Interface appearance
        </legend>
        <ng-container *ngFor="let item of defaultModelist1">
            <ap-radio name="Interface appearance"
                [value]="item.name"
                [(ngModel)]="defaultModeSelect1">
                <span>{{ item.name }}</span>
            </ap-radio>
        </ng-container>
      </fieldset>
      <fieldset>
        <legend class="drawer-info-title">
            Notifications settings
        </legend>
        <ap-toggle [checked]="false"><span>Appkit releases</span></ap-toggle><br />
        <ap-toggle [checked]="false"><span>Issue replies</span></ap-toggle><br />
        <ap-toggle [checked]="false"><span>Issue closed</span></ap-toggle>
      </fieldset>
    </div>
  </ap-tabset> 
  <ng-template ngTemplate="footer">
      <ap-button [btnType]="'secondary'"
          (onClick)="this.isVerticalDrawerVisible=false"
          [label]="'Cancel'"></ap-button>
      <ap-button [btnType]="'primary'"
          (onClick)="this.isVerticalDrawerVisible=false"
          [label]="'Ok'"></ap-button>
  </ng-template>
</ap-drawer>
```

#### TypeScript

```typescript
activeIndexInDrawer: number = 0;
isVerticalDrawerVisible = false;
drawerTabs: any[] = [
  { label: 'Label' },
  { label: 'Label' },
  { label: 'Label' },
];
defaultModelist: any[] = [
  { name: 'Transparent', key: 'Transparent' },
  { name: 'Light mode', key: 'Light mode' },
  { name: 'Dark mode', key: 'Dark mode' }
];
defaultModeSelect = 'Transparent';
defaultModelist1: any[] = [
  { name: 'Light mode', key: 'Light mode' },
  { name: 'Dark mode', key: 'Dark mode' }
];
defaultModeSelect1 = 'Light mode';
handleOk(event: any) {
  console.log(event);
}
handleCancel(event: any) {
  console.log(event);
}
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

  ::ng-deep .ap-drawer-wrapper .ap-drawer-header .ap-badge {
    margin-right: var(--spacing-3);
  }

  ::ng-deep .ap-drawer-wrapper .ap-drawer-footer .ap-button:last-of-type {
    margin-right: $spacing-3;
  }

  ::ng-deep .ap-drawer-actions .ap-drawer-header-icon {
    position: relative;
    width: 2.5rem;
    height: 2.5rem;
    padding: var(--spacing-3);
    background-color: transparent;

    &:hover {
        background-color: var(--color-background-hover);
        border-radius: var(--border-radius-2);
        cursor: pointer;
    }
  }

  ::ng-deep .tab-content {
    padding: $spacing-6 0;

    fieldset{
        display: block;
        padding: 0;
        margin-top: $spacing-5;
        &:first-of-type{
            margin-top: rem(8px);
        }
        .ap-radio-group{
            .ap-radio-container:last-of-type{
                margin-bottom: 0;
            }
        }
    }

    legend{
        display: block;
        width: 100%;
    }

    .drawer-info-title {
        margin-top: $spacing-5;
        color: $color-text-heading;
        font-size: rem(14px);
        font-weight: 400;
        line-height: rem(20px);
        background-color: $color-background-selected;
        border-radius: rem(4px);
        height: rem(28px);
        padding-left: rem(8px);
        display: flex;
        align-items: center;
        margin-bottom: $spacing-3;

        &:first-child {
            margin-top: rem(8px);
        }
    }

    .ap-radio-container {
        margin-bottom: $spacing-3;
    }

    .ap-switch {
        margin-bottom: $spacing-2;
    }
  }

  ::ng-deep .ap-drawer-body {
    .ap-table-demo td:last-child {
        text-align: right;
    }

    .ap-table {
        box-shadow: none !important;

        .actions-warpper {
            span {
                margin-right: rem(16px);
                cursor: pointer;
            }
        }
    }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0"
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### vertical - focused


**Example #2** | **Variation**: vertical | **Modifier**: focused | **State**: None

#### Module Import

```typescript
                import { DrawerModule } from '@appkit4/angular-components/drawer';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from '@appkit4/angular-components/badge';
import { ToggleModule} from '@appkit4/angular-components/toggle';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
import { RadioModule } from '@appkit4/angular-components/radio';
import { TabsModule } from '@appkit4/angular-components/tabs';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" [label]="'Open drawer'" (onClick)="this.isVerticalDrawerVisible=true"></ap-button>
<ap-drawer [(visible)]="isVerticalDrawerVisible" [placement]="'right'" [mask]="true"
[drawerTitle]="'Drawer'" [resizable]="true">
  <ng-template ngTemplate="icons">
      <button type="button" aria-label="more"
          class="ap-drawer-header-icon ap-drawer-header-more">
          <span class="Appkit4-icon icon-horizontal-more-outline"></span>
      </button>
  </ng-template>
  <ng-template ngTemplate="header">
    <ap-badge value="New" type="primary" size="small"></ap-badge>
  </ng-template>
  <ap-tabset [type]="'underline'" [(activeIndex)]="activeIndexInDrawer">
    <ap-tab *ngFor="let subtab of drawerTabs" [label]="subtab.label"></ap-tab>
    <div *ngIf="activeIndexInDrawer===0" class="tab-content">
      <fieldset>
        <legend class="drawer-info-title">
            Components default mode
        </legend>
        <ng-container *ngFor="let item of defaultModelist">
            <ap-radio name="components default mode"
                [value]="item.name" [(ngModel)]="defaultModeSelect">
                <span>{{ item.name }}</span>
            </ap-radio>
        </ng-container>
      </fieldset>
      <fieldset>
        <legend class="drawer-info-title">
            Interface appearance
        </legend>
        <ng-container *ngFor="let item of defaultModelist1">
            <ap-radio name="Interface appearance"
                [value]="item.name"
                [(ngModel)]="defaultModeSelect1">
                <span>{{ item.name }}</span>
            </ap-radio>
        </ng-container>
      </fieldset>
      <fieldset>
        <legend class="drawer-info-title">
            Notifications settings
        </legend>
        <ap-toggle [checked]="false"><span>Appkit releases</span></ap-toggle><br />
        <ap-toggle [checked]="false"><span>Issue replies</span></ap-toggle><br />
        <ap-toggle [checked]="false"><span>Issue closed</span></ap-toggle>
      </fieldset>
    </div>
  </ap-tabset> 
  <ng-template ngTemplate="footer">
      <ap-button [btnType]="'secondary'"
          (onClick)="this.isVerticalDrawerVisible=false"
          [label]="'Cancel'"></ap-button>
      <ap-button [btnType]="'primary'"
          (onClick)="this.isVerticalDrawerVisible=false"
          [label]="'Ok'"></ap-button>
  </ng-template>
</ap-drawer>
```

#### TypeScript

```typescript
activeIndexInDrawer: number = 0;
isVerticalDrawerVisible = false;
drawerTabs: any[] = [
  { label: 'Label' },
  { label: 'Label' },
  { label: 'Label' },
];
defaultModelist: any[] = [
  { name: 'Transparent', key: 'Transparent' },
  { name: 'Light mode', key: 'Light mode' },
  { name: 'Dark mode', key: 'Dark mode' }
];
defaultModeSelect = 'Transparent';
defaultModelist1: any[] = [
  { name: 'Light mode', key: 'Light mode' },
  { name: 'Dark mode', key: 'Dark mode' }
];
defaultModeSelect1 = 'Light mode';
handleOk(event: any) {
  console.log(event);
}
handleCancel(event: any) {
  console.log(event);
}
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

  ::ng-deep .ap-drawer-wrapper .ap-drawer-header .ap-badge {
    margin-right: var(--spacing-3);
  }

  ::ng-deep .ap-drawer-wrapper .ap-drawer-footer .ap-button:last-of-type {
    margin-right: $spacing-3;
  }

  ::ng-deep .ap-drawer-actions .ap-drawer-header-icon {
    position: relative;
    width: 2.5rem;
    height: 2.5rem;
    padding: var(--spacing-3);
    background-color: transparent;

    &:hover {
        background-color: var(--color-background-hover);
        border-radius: var(--border-radius-2);
        cursor: pointer;
    }
  }

  ::ng-deep .tab-content {
    padding: $spacing-6 0;

    fieldset{
        display: block;
        padding: 0;
        margin-top: $spacing-5;
        &:first-of-type{
            margin-top: rem(8px);
        }
        .ap-radio-group{
            .ap-radio-container:last-of-type{
                margin-bottom: 0;
            }
        }
    }

    legend{
        display: block;
        width: 100%;
    }

    .drawer-info-title {
        margin-top: $spacing-5;
        color: $color-text-heading;
        font-size: rem(14px);
        font-weight: 400;
        line-height: rem(20px);
        background-color: $color-background-selected;
        border-radius: rem(4px);
        height: rem(28px);
        padding-left: rem(8px);
        display: flex;
        align-items: center;
        margin-bottom: $spacing-3;

        &:first-child {
            margin-top: rem(8px);
        }
    }

    .ap-radio-container {
        margin-bottom: $spacing-3;
    }

    .ap-switch {
        margin-bottom: $spacing-2;
    }
  }

  ::ng-deep .ap-drawer-body {
    .ap-table-demo td:last-child {
        text-align: right;
    }

    .ap-table {
        box-shadow: none !important;

        .actions-warpper {
            span {
                margin-right: rem(16px);
                cursor: pointer;
            }
        }
    }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0"
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### horizontal - default


**Example #3** | **Variation**: horizontal | **Modifier**: default | **State**: None

#### Module Import

```typescript
                import { DrawerModule } from '@appkit4/angular-components/drawer';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from '@appkit4/angular-components/badge';
import { ToggleModule} from '@appkit4/angular-components/toggle';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
import { RadioModule } from '@appkit4/angular-components/radio';
import { TabsModule } from '@appkit4/angular-components/tabs';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" label="Open drawer" (onClick)="this.isHorizontalDrawerVisible=true"></ap-button>
<ap-drawer [(visible)]="isHorizontalDrawerVisible" [placement]="'bottom'" [mask]="false" [drawerTitle]="'Employees'" 
  [resizable]="true">
    <ng-template ngTemplate="icons">
        <button type="button" aria-label="more"
            class="ap-drawer-header-icon ap-drawer-header-more">
            <span class="Appkit4-icon icon-horizontal-more-outline"></span>
        </button>
    </ng-template>
    <ap-table #table class="ap-table-demo ap-table-checkbox">
        <table>
            <tbody>
                <tr *ngFor="let data of tableData;"
                    [class.selected]="data.selected">
                    <td>
                        <ap-checkbox [(ngModel)]="data.selected"
                          [stopKeydownPropagation] = "false"
                            class="ap-table-checkbox-container">
                            <span>{{data.order}}</span>
                        </ap-checkbox>
                    </td>
                    <td>
                        <span>{{data.city}}</span>
                    </td>
                    <td>
                        <span>{{data.date}}</span>
                    </td>
                    <td>
                        <span>{{data.description}}</span>
                    </td>
                    <td>
                        <div class="actions-warpper">
                          <span role="button" aria-label="copy" tabindex="0"
                              class="Appkit4-icon icon-duplicate-outline"></span>
                          <span role="button" aria-label="edit" tabindex="0"
                              class="Appkit4-icon icon-edit-outline"></span>
                          <span role="button" aria-label="delete" tabindex="0"
                              class="Appkit4-icon icon-delete-outline"></span>
                          <span role="button" aria-label="more function" tabindex="0"
                              class="Appkit4-icon icon-vertical-more-outline"></span>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </ap-table>
    <ng-template ngTemplate="footer">
        <ap-button [btnType]="'secondary'"
            (onClick)="this.isHorizontalDrawerVisible=false"
            [label]="'Cancel'"></ap-button>
        <ap-button [btnType]="'primary'"
            (onClick)="this.isHorizontalDrawerVisible=false"
            [label]="'Email'"></ap-button>
    </ng-template>
  </ap-drawer>
```

#### TypeScript

```typescript
isHorizontalDrawerVisible = false;
tableData: any[] = [
  {
    order: 'Albert Flores',
    city: 'Fairfield',
    date: '1/15/12',
    description: 'New England Revolution'
  },
  {
    order: 'Jerome Bell',
    city: 'Austin',
    date: '6/21/19',
    description: 'Philadelphia Union'
  }
];
handleOk(event: any) {
  console.log(event);
}
handleCancel(event: any) {
  console.log(event);
}
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

  ::ng-deep .ap-drawer-wrapper .ap-drawer-header .ap-badge {
    margin-right: var(--spacing-3);
  }

  ::ng-deep .ap-drawer-wrapper .ap-drawer-footer .ap-button:last-of-type {
    margin-right: $spacing-3;
  }

  ::ng-deep .ap-drawer-actions .ap-drawer-header-icon {
    position: relative;
    width: 2.5rem;
    height: 2.5rem;
    padding: var(--spacing-3);
    background-color: transparent;

    &:hover {
        background-color: var(--color-background-hover);
        border-radius: var(--border-radius-2);
        cursor: pointer;
    }
  }

  ::ng-deep .tab-content {
    padding: $spacing-6 0;

    fieldset{
        display: block;
        padding: 0;
        margin-top: $spacing-5;
        &:first-of-type{
            margin-top: rem(8px);
        }
        .ap-radio-group{
            .ap-radio-container:last-of-type{
                margin-bottom: 0;
            }
        }
    }

    legend{
        display: block;
        width: 100%;
    }

    .drawer-info-title {
        margin-top: $spacing-5;
        color: $color-text-heading;
        font-size: rem(14px);
        font-weight: 400;
        line-height: rem(20px);
        background-color: $color-background-selected;
        border-radius: rem(4px);
        height: rem(28px);
        padding-left: rem(8px);
        display: flex;
        align-items: center;
        margin-bottom: $spacing-3;

        &:first-child {
            margin-top: rem(8px);
        }
    }

    .ap-radio-container {
        margin-bottom: $spacing-3;
    }

    .ap-switch {
        margin-bottom: $spacing-2;
    }
  }

  ::ng-deep .ap-drawer-body {
    .ap-table-demo td:last-child {
        text-align: right;
    }

    .ap-table {
        box-shadow: none !important;

        .actions-warpper {
            span {
                margin-right: rem(16px);
                cursor: pointer;
            }
        }
    }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0"
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### horizontal - focused


**Example #4** | **Variation**: horizontal | **Modifier**: focused | **State**: None

#### Module Import

```typescript
                import { DrawerModule } from '@appkit4/angular-components/drawer';
import { ButtonModule } from '@appkit4/angular-components/button';
import { BadgeModule } from '@appkit4/angular-components/badge';
import { ToggleModule} from '@appkit4/angular-components/toggle';
import { TableModule } from '@appkit4/angular-components/table';
import { CheckboxModule } from '@appkit4/angular-components/checkbox';
import { RadioModule } from '@appkit4/angular-components/radio';
import { TabsModule } from '@appkit4/angular-components/tabs';
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<ap-button [btnType]="'primary'" label="Open drawer" (onClick)="this.isHorizontalDrawerVisible=true"></ap-button>
<ap-drawer [(visible)]="isHorizontalDrawerVisible" [placement]="'bottom'" [mask]="true" [drawerTitle]="'Employees'" 
  [resizable]="true">
    <ng-template ngTemplate="icons">
        <button type="button" aria-label="more"
            class="ap-drawer-header-icon ap-drawer-header-more">
            <span class="Appkit4-icon icon-horizontal-more-outline"></span>
        </button>
    </ng-template>
    <ap-table #table class="ap-table-demo ap-table-checkbox">
        <table>
            <tbody>
                <tr *ngFor="let data of tableData;"
                    [class.selected]="data.selected">
                    <td>
                        <ap-checkbox [(ngModel)]="data.selected"
                          [stopKeydownPropagation] = "false"
                            class="ap-table-checkbox-container">
                            <span>{{data.order}}</span>
                        </ap-checkbox>
                    </td>
                    <td>
                        <span>{{data.city}}</span>
                    </td>
                    <td>
                        <span>{{data.date}}</span>
                    </td>
                    <td>
                        <span>{{data.description}}</span>
                    </td>
                    <td>
                        <div class="actions-warpper">
                          <span role="button" aria-label="copy" tabindex="0"
                              class="Appkit4-icon icon-duplicate-outline"></span>
                          <span role="button" aria-label="edit" tabindex="0"
                              class="Appkit4-icon icon-edit-outline"></span>
                          <span role="button" aria-label="delete" tabindex="0"
                              class="Appkit4-icon icon-delete-outline"></span>
                          <span role="button" aria-label="more function" tabindex="0"
                              class="Appkit4-icon icon-vertical-more-outline"></span>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </ap-table>
    <ng-template ngTemplate="footer">
        <ap-button [btnType]="'secondary'"
            (onClick)="this.isHorizontalDrawerVisible=false"
            [label]="'Cancel'"></ap-button>
        <ap-button [btnType]="'primary'"
            (onClick)="this.isHorizontalDrawerVisible=false"
            [label]="'Email'"></ap-button>
    </ng-template>
  </ap-drawer>
```

#### TypeScript

```typescript
isHorizontalDrawerVisible = false;
tableData: any[] = [
  {
    order: 'Albert Flores',
    city: 'Fairfield',
    date: '1/15/12',
    description: 'New England Revolution'
  },
  {
    order: 'Jerome Bell',
    city: 'Austin',
    date: '6/21/19',
    description: 'Philadelphia Union'
  }
];
handleOk(event: any) {
  console.log(event);
}
handleCancel(event: any) {
  console.log(event);
}
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

  ::ng-deep .ap-drawer-wrapper .ap-drawer-header .ap-badge {
    margin-right: var(--spacing-3);
  }

  ::ng-deep .ap-drawer-wrapper .ap-drawer-footer .ap-button:last-of-type {
    margin-right: $spacing-3;
  }

  ::ng-deep .ap-drawer-actions .ap-drawer-header-icon {
    position: relative;
    width: 2.5rem;
    height: 2.5rem;
    padding: var(--spacing-3);
    background-color: transparent;

    &:hover {
        background-color: var(--color-background-hover);
        border-radius: var(--border-radius-2);
        cursor: pointer;
    }
  }

  ::ng-deep .tab-content {
    padding: $spacing-6 0;

    fieldset{
        display: block;
        padding: 0;
        margin-top: $spacing-5;
        &:first-of-type{
            margin-top: rem(8px);
        }
        .ap-radio-group{
            .ap-radio-container:last-of-type{
                margin-bottom: 0;
            }
        }
    }

    legend{
        display: block;
        width: 100%;
    }

    .drawer-info-title {
        margin-top: $spacing-5;
        color: $color-text-heading;
        font-size: rem(14px);
        font-weight: 400;
        line-height: rem(20px);
        background-color: $color-background-selected;
        border-radius: rem(4px);
        height: rem(28px);
        padding-left: rem(8px);
        display: flex;
        align-items: center;
        margin-bottom: $spacing-3;

        &:first-child {
            margin-top: rem(8px);
        }
    }

    .ap-radio-container {
        margin-bottom: $spacing-3;
    }

    .ap-switch {
        margin-bottom: $spacing-2;
    }
  }

  ::ng-deep .ap-drawer-body {
    .ap-table-demo td:last-child {
        text-align: right;
    }

    .ap-table {
        box-shadow: none !important;

        .actions-warpper {
            span {
                margin-right: rem(16px);
                cursor: pointer;
            }
        }
    }
}
```

#### Dependency

```text
"focus-trap": "^5.1.0"
```

<!-- /EXAMPLE:4 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| id | string | Unique id of the drawer. | '' | 4.10.0 |
| visible | boolean | Visibility of the drawer. | - | 4.10.0 |
| placement | string | 'bottom'\|'left'\|'right'\|'top' | 'right' | 4.10.0 |
| drawerTitle | string | Title of the drawer. | '' | 4.10.0 |
| resizable | boolean | If true, resizable bar is available and the drawer can be resized. | false | 4.10.0 |
| closable | boolean | Show the close button. | true | 4.10.0 |
| closeOnPressEscape | boolean | Whether the drawer can be closed by pressing the ESC key. | true | 4.10.0 |
| initialFocusIndex | number | Index of the element which receives the focus in all focusable elements when the drawer opens and mask is true. If the value = -1, the initialFocus will not work. | 0 | 4.10.0 |
| maskClosable | boolean | Whether to close the drawer dialog when the mask is clicked. | true | 4.10.0 |
| mask | boolean | The mask of the drawer. | true | 4.10.0 |
| ariaLabel | string | The aria-label of the drawer. | - | 4.10.0 |
| closeAriaLabel | string | The aria-label of close button in the upper right corner of the drawer. | 'close' | 4.10.0 |
| closeTabindex | string | The tabindex of close button in the upper right corner of the drawer. | '0' | 4.10.0 |
| styleClass | string | The className of the drawer wrapper. | - | 4.10.0 |
| onScrollDrawer | EventEmitter<Event> | Callback to scroll the drawer body. | - | 4.10.0 |
| onClose | EventEmitter<Event> | Callback to close the drawer. | - | 4.10.0 |
| onOpen | EventEmitter<Event> | Callback to open the drawer. | - | 4.10.0 |


<!-- /SECTION:properties -->