---
component: breadcrumb
framework: angular
---

# Breadcrumb Component

## Overview

AppKit Breadcrumb component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use breadcrumbs:
- To help users orient themselves in the UI as an alternative wayfinding element.
- In pages where it is likely that users have landed after following a link, for example.
- In products or complex UIs with multiple levels of navigation, and pages organized in a hierarchical way.

### Anatomy

- **Icon:** Appkit 4 breadcrumbs can have icons to support wayfinding.
- **Item Label:** Location or page link.
- **Separator:** Our separator is a chevron icon that provides a visual distinction between items and helps understand direction.

### Best Practices

#### How to use

- Use for complex products that have a more complex page structure.
- Use for cases where users are likely to land on a page from an external source so they can quickly recognize where they are.
- Icons can be paired with each label and are up to the user's discretion. The arrow icons separating each label should not be changed.
- Place breadcrumbs below the header, at the top left corner of the screen, above the page title.

#### How not to use

- Don't replace main navigation elements such as Sidebars with breadcrumbs. They should be an addition but not replace the main navigation of the site.
- In the breadcrumb trail, don’t make the breadcrumb corresponding to the current page a link.
- Don't use breadcrumbs for sites with flat hierarchies that are only 1 or 2 levels deep.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161642-107946%26viewport%3D934%252C-2980%252C0.76%26t%3Dy3QB2lWKJne7qw8q-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Behavior

All elements of the breadcrumb must act as links except the item that represents the current location.

### Accessibility

- Apply breadcrumb navigation region by using **<nav aria-label= “breadcrumb”>...</nav>** for screen reader announcement

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 2


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | Default | `section: "example:1"` |
| 2 | icon | `section: "example:2"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### Default


**Example #1** | **Variation**: None | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { BreadcrumbModule } from '@appkit4/angular-components/breadcrumb';
```

#### HTML Template

```html
<ap-breadcrumb>
  <ap-breadcrumb-item><a tabindex="0" href="#">Hong Kong</a></ap-breadcrumb-item>
  <ap-breadcrumb-item><a tabindex="0" href="#">Stockholm</a></ap-breadcrumb-item>
  <ap-breadcrumb-item><a tabindex="0" href="#">São Paulo</a></ap-breadcrumb-item>
  <ap-breadcrumb-item><a href="#" aria-current="page" tabindex="-1">Saint Petersburg</a></ap-breadcrumb-item>
</ap-breadcrumb>
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### icon


**Example #2** | **Variation**: None | **Modifier**: icon | **State**: None

#### Module Import

```typescript
import { BreadcrumbModule } from '@appkit4/angular-components/breadcrumb';
```

#### HTML Template

```html
<ap-breadcrumb>
  <ap-breadcrumb-item>
    <span class="Appkit4-icon icon-folder-closed-outline"></span>
    <a tabindex="0" href="#">Hong Kong</a>
  </ap-breadcrumb-item>
  <ap-breadcrumb-item>
    <span class="Appkit4-icon icon-folder-closed-outline"></span>
    <a tabindex="0" href="#">Stockholm</a>
  </ap-breadcrumb-item >
  <ap-breadcrumb-item>
    <span class="Appkit4-icon icon-folder-closed-outline"></span>
    <a tabindex="0" href="#">São Paulo</a>
  </ap-breadcrumb-item>
  <ap-breadcrumb-item>
    <span class="Appkit4-icon icon-image-outline"></span>
    <a href="#" aria-current="page" tabindex="-1">Saint Petersburg</a>
  </ap-breadcrumb-item>
</ap-breadcrumb>
```

<!-- /EXAMPLE:2 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-breadcrumb

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| style | string | The inline style of the breadcrumb | '' | 4.0.0 |
| styleClass | string | The style class names of the breadcrumb | '' | 4.0.0 |

### ap-breadcrumb-item

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| style | string | The inline style of the breadcrumb item | '' | 4.0.0 |
| styleClass | string | The style class names of the breadcrumb item | '' | 4.0.0 |
| path | string | The path of breadcrumb item will navigate to. | - | 4.0.0 |
| onClick | EventEmitter<Event> | Callback to invoke when the breadcrumb item is clicked | - | 4.0.0 |


<!-- /SECTION:properties -->
