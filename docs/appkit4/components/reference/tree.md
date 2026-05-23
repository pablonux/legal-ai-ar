---
component: tree
framework: angular
---

# Tree Component

## Overview

AppKit Tree component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use File explorer component:

- To provide users with a way to navigate nested information.
- To represent a hierarchical list or information.

### Anatomy

1. **Parent tree view item:** First level tree view item

2. **Child tree view item:** Second level tree view item

3. **Selected tree view item:** Third level tree view item

### Variants

None

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-75969%26viewport%3D1407%252C-15275%252C0.47%26t%3DxgA1b5vviDTvMYNl-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Choose icons that relate to the object type being represented in the tree view. These icons can be unique to specific data types, to add better clarification for users.
- When using for navigating items with long names, consider truncating to 2 lines per item max.
- Restrict hierarchy depth and avoid overflow by setting a limit on how many levels of hierarchy deep that a user can create in a tree view. Use only up to a third level of hierarchy as Appkit’s recommended limit.

#### How not to use

- Do not use a file explorer component as a navigation.

### Behavior

- Clicking the items will expand or collapse a tree view item that contains child tree view items.

### Accessibility

- Use ARIA tree view as the structure for File Explorer.
- Ensure that right arrow opens folders, left arrow closes folders, up/down arrow navigates within folders.
- Capability of using Tab key to navigate through treeview is an added bonus for switch button users (people navigating with a forward and enter button).

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
import { TreeModule } from '@appkit4/angular-components/tree';
```

#### HTML Template

```html
<div class="tree-demo-wrapper">
  <ap-tree [data]="attachmentsNodes" (onClick)="onClickEvent($event)">
  </ap-tree>
</div>
```

#### TypeScript

```typescript
attachmentsNodes = [
  {
    title: 'Archive',
    key: '1',
    expanded: false,
    iconOpened: "icon-folder-closed-outline",
    iconClosed: "icon-folder-closed-outline",
    children: [{
      title: 'Documentation.pdf',
      key: '101',
    }]
  },
  {
    title: 'Appkit',
    key: '2',
    expanded: false,
    iconOpened: "icon-folder-closed-outline",
    iconClosed: "icon-folder-closed-outline",
    children: [
      {
        title: 'Documentation.pdf',
        key: '201',
      }, {
        title: 'Presentation.key',
        key: '202',
      }, {
        title: 'Others',
        key: '203',
        expanded: true,
        iconOpened: "icon-folder-closed-outline",
        iconClosed: "icon-folder-closed-outline",
        children: [
          {
            title: 'Screenshot.png',
            key: '2031',
          }
        ]
      },
    ]
  },
  {
    title: 'Documents',
    key: '3',
    expanded: false,
    iconOpened: "icon-folder-closed-outline",
    iconClosed: "icon-folder-closed-outline",
    children: [{
      title: 'Presentation.key',
      key: '302',
    }]
  }, {
    title: 'Dropbox',
    key: '4',
    expanded: false,
    iconOpened: "icon-box-outline",
    iconClosed: "icon-box-outline",
    children: [{
      title: 'Presentation.key',
      key: '402',
    }]
  },
];
onClickEvent(event: any) {
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

.tree-demo-wrapper{
  width: rem(338px);
  margin: $spacing-6;
  padding: $spacing-3;
  border-radius: $border-radius-3;
  box-shadow: $box-shadow-1;
  background-color: $color-background-container;
}
```

<!-- /EXAMPLE:1 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| data | array: \[{
            title: 'Archive',
            key: '1',
            expanded: false,
            iconOpened: "icon-folder-fill",
            iconClosed: "icon-folder-fill",
            children: \[{
              title: 'Documentation.pdf',
              key: '101'}\]
            }\] | The data of file explorer. | \[ \] | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| onClick | EventEmitter<{ item: any, event: Event, treeElement: HTMLElement }> | Callback to invoke when the file item is clicked. Event properties: • item: Data of the clicked file item. • event: Event. • treeElement: HTMLElement of the clicked file item. | N/A | 4.0.0 |


<!-- /SECTION:properties -->
