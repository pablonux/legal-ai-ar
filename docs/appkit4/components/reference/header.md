---
component: header
framework: angular
---

# Header Component

## Overview

AppKit Header component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use the Header component:

- As the main navigation for pwc applications and products.
- To identify the application or product.
- As a globally persistent location for navigational links and utilities.

### Anatomy

**1. PwC Logo**

**2. Product name:** The application or product name is always preceded by pwc logo.

**3. Header links and utilities area:** In this space, links and utilities related to the product are placed, such as profile, search, notifications and similar functions. It is not required that all products on a system display the same utilities, but it is recommended for a better user experience between products.

**4. Container:** Container element that spans the full width of the viewport.

### Variants

#### Default:

Default header variant with utilities and search.

#### With dropdown:

Includes links and dropdowns for a simple navigation.

#### Solid:

Default and Dropdown variations have the option to be styled with a solid background color. If it is preferred to use the solid version versus the version with background blur, it should be paired with the solid version of the Sidebar component for consistency across the UI shell.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-82827%26viewport%3D1753%252C-22983%252C0.56%26t%3Dr48l3jtKMPHOHXzA-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- The header should appear at the top of every page within your application.
- At a minimum, the header should display the product version of the PwC logo and product name.
- Headers may include a global navigation, user menu (via clicking the avatar), and other global application links such as help and notifications.
- The PwC logo in combination with the application name make up the "lockup". The lockup should be located at the top left of the screen throughout the in app experience. This location may reside within a header or a sidebar navigation. Color options for lockup are restricted to black or white.
- Applications built using Appkit should use the lower case bold "pwc" logo.

#### How not to use

- Do not place the header anywhere other than at the very top of the page view of the application.
- Do not alter the size, color, or placement of the PwC logo within the header.
- Do not change the background color of the header.

### Behavior

- The header must behave in a responsive way to adapt to smaller screen sizes and when reaching the mobile screen size threshold, the menus and utilities must collapse into hamburger menu and move to the Sidebar.
- The header spans the full width of the screen and is the topmost element in UI and Z-index.
- The header is persistent throughout the product experience.

### Accessibility

- Use <header> to identify page region.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 16


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | default | `section: "example:1"` |
| 2 | default - solid | `section: "example:2"` |
| 3 | default - solid-compact | `section: "example:3"` |
| 4 | default - solid-compact-hamburger | `section: "example:4"` |
| 5 | default - solid-compact-lefthamburger | `section: "example:5"` |
| 6 | default - compact | `section: "example:6"` |
| 7 | default - compact-hamburger | `section: "example:7"` |
| 8 | default - compact-lefthamburger | `section: "example:8"` |
| 9 | dropdown | `section: "example:9"` |
| 10 | dropdown - solid | `section: "example:10"` |
| 11 | dropdown - solid-compact | `section: "example:11"` |
| 12 | dropdown - solid-compact-hamburger | `section: "example:12"` |
| 13 | dropdown - solid-compact-lefthamburger | `section: "example:13"` |
| 14 | dropdown - compact | `section: "example:14"` |
| 15 | dropdown - compact-hamburger | `section: "example:15"` |
| 16 | dropdown - compact-lefthamburger | `section: "example:16"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### default


**Example #1** | **Variation**: default | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'transparent'"
  [compact]="false"
>
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'help-question-outline'" [label]="'Support'">
    </ap-header-options-item>
    <ap-header-options-item [iconName]="'notification-outline'" [label]="'Alerts'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="VR" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### default - solid


**Example #2** | **Variation**: default | **Modifier**: solid | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'solid'"
  [compact]="false"
>
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'help-question-outline'" [label]="'Support'">
    </ap-header-options-item>
    <ap-header-options-item [iconName]="'notification-outline'" [label]="'Alerts'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="VR" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### default - solid-compact


**Example #3** | **Variation**: default | **Modifier**: solid-compact | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'solid'"
  [compact]="true"
>
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'help-question-outline'" [label]="'Support'">
    </ap-header-options-item>
    <ap-header-options-item [iconName]="'notification-outline'" [label]="'Alerts'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="VR" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### default - solid-compact-hamburger


**Example #4** | **Variation**: default | **Modifier**: solid-compact-hamburger | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'solid'" [hamburgerPosition]="'right'"
  [compact]="true"
>
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'help-question-outline'" [label]="'Support'">
    </ap-header-options-item>
    <ap-header-options-item [iconName]="'notification-outline'" [label]="'Alerts'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="VR" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### default - solid-compact-lefthamburger


**Example #5** | **Variation**: default | **Modifier**: solid-compact-lefthamburger | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'solid'" [hamburgerPosition]="'left'"
  [compact]="true"
>
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'help-question-outline'" [label]="'Support'">
    </ap-header-options-item>
    <ap-header-options-item [iconName]="'notification-outline'" [label]="'Alerts'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="VR" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### default - compact


**Example #6** | **Variation**: default | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'transparent'"
  [compact]="true"
>
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'help-question-outline'" [label]="'Support'">
    </ap-header-options-item>
    <ap-header-options-item [iconName]="'notification-outline'" [label]="'Alerts'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="VR" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### default - compact-hamburger


**Example #7** | **Variation**: default | **Modifier**: compact-hamburger | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'transparent'" [hamburgerPosition]="'right'"
  [compact]="true"
>
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'help-question-outline'" [label]="'Support'">
    </ap-header-options-item>
    <ap-header-options-item [iconName]="'notification-outline'" [label]="'Alerts'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="VR" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### default - compact-lefthamburger


**Example #8** | **Variation**: default | **Modifier**: compact-lefthamburger | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'transparent'" [hamburgerPosition]="'left'"
  [compact]="true"
>
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'help-question-outline'" [label]="'Support'">
    </ap-header-options-item>
    <ap-header-options-item [iconName]="'notification-outline'" [label]="'Alerts'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="VR" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### dropdown


**Example #9** | **Variation**: dropdown | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'transparent'"
  [compact]="false">
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="sub-title">
    Product Name
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'down-chevron-outline'" [label]="'Menu'"
      [iconSlot]="'end'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="JS" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### dropdown - solid


**Example #10** | **Variation**: dropdown | **Modifier**: solid | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'solid'"
  [compact]="false">
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="sub-title">
    Product Name
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'down-chevron-outline'" [label]="'Menu'"
      [iconSlot]="'end'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="JS" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### dropdown - solid-compact


**Example #11** | **Variation**: dropdown | **Modifier**: solid-compact | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'solid'"
  [compact]="true">
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="sub-title">
    Product Name
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'down-chevron-outline'" [label]="'Menu'"
      [iconSlot]="'end'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="JS" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### dropdown - solid-compact-hamburger


**Example #12** | **Variation**: dropdown | **Modifier**: solid-compact-hamburger | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'solid'" [hamburgerPosition]="'right'"
  [compact]="true">
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="sub-title">
    Product Name
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'down-chevron-outline'" [label]="'Menu'"
      [iconSlot]="'end'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="JS" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### dropdown - solid-compact-lefthamburger


**Example #13** | **Variation**: dropdown | **Modifier**: solid-compact-lefthamburger | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'solid'" [hamburgerPosition]="'left'"
  [compact]="true">
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="sub-title">
    Product Name
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'down-chevron-outline'" [label]="'Menu'"
      [iconSlot]="'end'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="JS" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### dropdown - compact


**Example #14** | **Variation**: dropdown | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'transparent'"
  [compact]="true">
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="sub-title">
    Product Name
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'down-chevron-outline'" [label]="'Menu'"
      [iconSlot]="'end'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="JS" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### dropdown - compact-hamburger


**Example #15** | **Variation**: dropdown | **Modifier**: compact-hamburger | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'transparent'" [hamburgerPosition]="'right'"
  [compact]="true">
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="sub-title">
    Product Name
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'down-chevron-outline'" [label]="'Menu'"
      [iconSlot]="'end'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="JS" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### dropdown - compact-lefthamburger


**Example #16** | **Variation**: dropdown | **Modifier**: compact-lefthamburger | **State**: None

#### Module Import

```typescript
import { HeaderModule } from '@appkit4/angular-components/header';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-header [type]="'transparent'" [hamburgerPosition]="'left'"
  [compact]="true">
  <ng-template ngTemplate="title">
    Appkit
  </ng-template>
  <ng-template ngTemplate="sub-title">
    Product Name
  </ng-template>
  <ng-template ngTemplate="content">
    <ap-header-options-item [iconName]="'search-outline'" [label]="'Search'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="options">
    <ap-header-options-item [iconName]="'down-chevron-outline'" [label]="'Menu'"
      [iconSlot]="'end'">
    </ap-header-options-item>
  </ng-template>
  <ng-template ngTemplate="user">
    <ap-avatar name="JS" borderWidth="0" diameter="40" [role]="'button'"></ap-avatar>
  </ng-template>
</ap-header>
```

<!-- /EXAMPLE:16 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-header

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| hasLogo | boolean | If true, displays the PwC logo. | true | 4.0.0 |
| type | string: 'transparent'\|'solid' | The background style of the header. | 'transparent' | 4.0.0 |
| onClickLogo | EventEmitter<Event> | Callback to invoke when clicking on the PwC logo. | - | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| responsive | boolean | Specify if this component is responsive. | true | 4.4.0 |
| ariaLabel | string | The aria-label of header logo icon. | 'PwC logo' | 4.4.0 |
| role | string | The role of header logo icon. | 'link' | 4.4.0 |
| compact | boolean | Whether show the label for component of header. | 'false' | 4.4.0 |
| hamburgerPosition | string: 'left'\|'right'\|'' | The position of the hamburger icon. If the value is '', the hamburger icon doesn't display. | '' | 4.4.0 |
| onClickHamburger | EventEmitter<Event> | Callback to invoke when clicking the hamburger icon | - | 4.4.0 |

### ap-header-options-item

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| iconName | string | The icon name of the header item. | '' | 4.0.0 |
| label | string | The label of the header item. | '' | 4.0.0 |
| iconSlot | string: 'start'\|'end' | The position of the icon in the header item. | 'start' | 4.0.0 |
| onClick | EventEmitter<Event> | Callback to invoke when the header item is clicked | - | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |


<!-- /SECTION:properties -->