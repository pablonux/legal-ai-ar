---
component: footer
framework: angular
---

# Footer Component

## Overview

AppKit Footer component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use the footer component:

- To globally display disclaimer, legal content, copyright and related hyperlinks.

### Anatomy

1. **Text:** Text content that includes the copyright notice.

2. **Links:** Links to commonly accessed pages or resources

### Variants

None

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-80966%26viewport%3D1491%252C-17081%252C0.45%26t%3DQu5ivuIxPCYBsmFp-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- All PwC applications require some form of footer content.
- The most common usage would be legal information.
- The footer should be used consistently on every page view within the application.

#### How not to use

- Do not use footers for content other than disclaimer, legal, copyright, and related hyperlinks.

### Behavior

- Links within the footer are styled correctly per Appkit styling and are expected to be functional when public.
- Footer should adapt to the main content area and be placed below it when possible.

### Accessibility

- Use <footer> to identify page region.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 2


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | text | `section: "example:1"` |
| 2 | links | `section: "example:2"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### text


**Example #1** | **Variation**: text | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FooterModule } from '@appkit4/angular-components/footer';
```

#### HTML Template

```html
<ap-footer [content]="footerContent" [type]="footerType"></ap-footer>
```

#### TypeScript

```typescript
footerContent: string = "© 2025 PwC US. All rights reserved. PwC US refers to the US group of member firms and may sometimes refer to the PwC network. Each member firm is a separate legal entity.";
footerType: string = 'text';
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### links


**Example #2** | **Variation**: links | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FooterModule } from '@appkit4/angular-components/footer';
```

#### HTML Template

```html
<ap-footer [content]="footerContent" [type]="footerType" [links]="footerLinks"></ap-footer>
```

#### TypeScript

```typescript
footerContent: string = "© 2025 PwC US. All rights reserved. PwC US refers to the US group of member firms and may sometimes refer to the PwC network. Each member firm is a separate legal entity.";
footerType: string = 'links';
footerLinks: any[] = [
  { name: 'Privacy policy', href: '#', target: '_blank' },
  { name: 'Cookie notice', href: '#', target: '_self'  },
  { name: 'Terms and conditions', href: '#'},
  { name: 'Customize cookie settings', href: '#' }
];
```

<!-- /EXAMPLE:2 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| content | string | Content of the footer. | '' | 4.0.0 |
| type | string: 'text'\|'links' | Type of the footer. | 'text' | 4.0.0 |
| links | Array<{ name: string, href?: string, routerLink?: string, target?: string }> | Configure hyperlinks show in th footer. | \[\] | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |


<!-- /SECTION:properties -->
