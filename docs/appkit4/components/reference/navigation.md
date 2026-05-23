---
component: navigation
framework: angular
---

# Navigation Component

## Overview

AppKit Navigation component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Sidebar component:

- To display a list of navigational links, menu options, or supplementary content. 
- When there are a large number of navigation options or supplementary content that needs to be displayed on the same page.

### Anatomy

1. **Sidebar Container:** This is the container that holds all the content of the Sidebar. It usually has a fixed width and height.

2. **Header:** This is the top part of the Sidebar that contains a title or a logo.

3. **Menu Items:** These are the individual items that make up the navigational menu or list of options. 

4. **Collapse option:** Triggers the collapsed state.

5. **Footer:** This is the bottom part of the Sidebar that may contain additional links or buttons.

### Variants

#### Expanded:

Displays all the menu items along with the Header and Footer.

#### Collapsed:

Hides the menu item labels and only displays icons, Header and Footer.

#### Solid:

The solid background variant gives users the option to add emphasis to the sidebar. This should be used if the default variant does not provide enough contrast from the main content.

#### Simple:

A simpler variant of the sidebar with only menu items and does not contain a logo, title, avatar or other elements.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-120030%26viewport%3D1295%252C-33894%252C0.36%26t%3DXey5pyXUeSZqNCXQ-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Sidebar navigation can be used as the primary navigation or when combined with a header navigation as a secondary navigation.
- If combined with a header, the PwC logo and product name should reside within the header and not the sidebar navigation.
- Take the time to understand how your users expect things to be organized. This will guide how the navigation should be labeled, categorized, and organized. Sidebar navigation should always be displayed on the left.

#### How not to use

- Do not use sidebar navigation within a modal.
- Do not stack multiple sidebar navigation.

### Behavior

- Hover State: When the user hovers over a menu item, it should highlight or change color to indicate that it is clickable.
- Active State: When the user clicks on a menu item, it should become active to indicate the current page or section.
- Use the Sidebar component if there are more than five secondary navigation items, or if you expect a user to switch between secondary items frequently. 
- Sub-menus are denoted with a chevron and expand when clicked, pushing the other items down in the panel.
- Appkit sidebar does not support three tiers of navigation. If you have additional content to display beneath a sub-menu, use tabs within the page.

### Accessibility

- Use <nav> for regions that consist of menus and navigation elements.
- Some apps/pages have primary and secondary menus, provide a label for each menu by <nav aria-label="Side Navigation">...</nav>.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 16


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | expanded | `section: "example:1"` |
| 2 | expanded - solid | `section: "example:2"` |
| 3 | expanded - simple | `section: "example:3"` |
| 4 | expanded - submenu | `section: "example:4"` |
| 5 | expanded - solid-simple | `section: "example:5"` |
| 6 | expanded - solid-submenu | `section: "example:6"` |
| 7 | expanded - simple-submenu | `section: "example:7"` |
| 8 | expanded - solid-simple-submenu | `section: "example:8"` |
| 9 | collapsed | `section: "example:9"` |
| 10 | collapsed - solid | `section: "example:10"` |
| 11 | collapsed - simple | `section: "example:11"` |
| 12 | collapsed - submenu | `section: "example:12"` |
| 13 | collapsed - solid-simple | `section: "example:13"` |
| 14 | collapsed - solid-submenu | `section: "example:14"` |
| 15 | collapsed - simple-submenu | `section: "example:15"` |
| 16 | collapsed - solid-simple-submenu | `section: "example:16"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### expanded


**Example #1** | **Variation**: expanded | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = false;
simple: boolean = false;
collapsed: boolean = false;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

.ap-navigation-demo-wrapper {
  background-color: $color-background-default;
  width: fit-content;
  border-radius: $border-radius-3;
}

::ng-deep {
  .ap-navigation-item.selected,  .ap-navigation-item.active {
    // custom styles for the selected item
    font-weight: $font-weight-2;
    color: $color-text-primary !important;
    background-color: $color-background-hover-selected !important;

    .prefix-content{
      color: $color-text-primary !important;
      // custom styles for the selected prefix icon
    }
    .suffixIcon{
      // custom styles for the selected suffix icon
    }
  }
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### expanded - solid


**Example #2** | **Variation**: expanded | **Modifier**: solid | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = true;
simple: boolean = false;
collapsed: boolean = false;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

 .ap-navigation-demo-wrapper {
   background-color: $color-background-default;
   width: fit-content;
   border-radius: $border-radius-3;
 }

 ::ng-deep {
   .ap-navigation-item.selected,  .ap-navigation-item.active {
     // custom styles for the selected item
     font-weight: $font-weight-2;
     color: $color-text-primary !important;
     background-color: $color-background-selected !important;

     .prefix-content{
       color: $color-text-primary !important;
       // custom styles for the selected prefix icon
     }
     .suffixIcon{
       // custom styles for the selected suffix icon
     }
   }
 }
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### expanded - simple


**Example #3** | **Variation**: expanded | **Modifier**: simple | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = false;
simple: boolean = true;
collapsed: boolean = false;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

.ap-navigation-demo-wrapper {
  background-color: $color-background-default;
  width: fit-content;
  border-radius: $border-radius-3;
}

::ng-deep {
  .ap-navigation-item.selected,  .ap-navigation-item.active {
    // custom styles for the selected item
    font-weight: $font-weight-2;
    color: $color-text-primary !important;
    background-color: $color-background-hover-selected !important;

    .prefix-content{
      color: $color-text-primary !important;
      // custom styles for the selected prefix icon
    }
    .suffixIcon{
      // custom styles for the selected suffix icon
    }
  }
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### expanded - submenu


**Example #4** | **Variation**: expanded | **Modifier**: submenu | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = false;
simple: boolean = false;
collapsed: boolean = false;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    open: true,
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    open: true,
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

.ap-navigation-demo-wrapper {
  background-color: $color-background-default;
  width: fit-content;
  border-radius: $border-radius-3;
}

::ng-deep {
  .ap-navigation-item.selected,  .ap-navigation-item.active {
    // custom styles for the selected item
    font-weight: $font-weight-2;
    color: $color-text-primary !important;
    background-color: $color-background-hover-selected !important;

    .prefix-content{
      color: $color-text-primary !important;
      // custom styles for the selected prefix icon
    }
    .suffixIcon{
      // custom styles for the selected suffix icon
    }
  }
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### expanded - solid-simple


**Example #5** | **Variation**: expanded | **Modifier**: solid-simple | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = true;
simple: boolean = true;
collapsed: boolean = false;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

 .ap-navigation-demo-wrapper {
   background-color: $color-background-default;
   width: fit-content;
   border-radius: $border-radius-3;
 }

 ::ng-deep {
   .ap-navigation-item.selected,  .ap-navigation-item.active {
     // custom styles for the selected item
     font-weight: $font-weight-2;
     color: $color-text-primary !important;
     background-color: $color-background-selected !important;

     .prefix-content{
       color: $color-text-primary !important;
       // custom styles for the selected prefix icon
     }
     .suffixIcon{
       // custom styles for the selected suffix icon
     }
   }
 }
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### expanded - solid-submenu


**Example #6** | **Variation**: expanded | **Modifier**: solid-submenu | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = true;
simple: boolean = false;
collapsed: boolean = false;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    open: true,
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    open: true,
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

 .ap-navigation-demo-wrapper {
   background-color: $color-background-default;
   width: fit-content;
   border-radius: $border-radius-3;
 }

 ::ng-deep {
   .ap-navigation-item.selected,  .ap-navigation-item.active {
     // custom styles for the selected item
     font-weight: $font-weight-2;
     color: $color-text-primary !important;
     background-color: $color-background-selected !important;

     .prefix-content{
       color: $color-text-primary !important;
       // custom styles for the selected prefix icon
     }
     .suffixIcon{
       // custom styles for the selected suffix icon
     }
   }
 }
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### expanded - simple-submenu


**Example #7** | **Variation**: expanded | **Modifier**: simple-submenu | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = false;
simple: boolean = true;
collapsed: boolean = false;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    open: true,
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    open: true,
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

.ap-navigation-demo-wrapper {
  background-color: $color-background-default;
  width: fit-content;
  border-radius: $border-radius-3;
}

::ng-deep {
  .ap-navigation-item.selected,  .ap-navigation-item.active {
    // custom styles for the selected item
    font-weight: $font-weight-2;
    color: $color-text-primary !important;
    background-color: $color-background-hover-selected !important;

    .prefix-content{
      color: $color-text-primary !important;
      // custom styles for the selected prefix icon
    }
    .suffixIcon{
      // custom styles for the selected suffix icon
    }
  }
}
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### expanded - solid-simple-submenu


**Example #8** | **Variation**: expanded | **Modifier**: solid-simple-submenu | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = true;
simple: boolean = true;
collapsed: boolean = false;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    open: true,
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    open: true,
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

 .ap-navigation-demo-wrapper {
   background-color: $color-background-default;
   width: fit-content;
   border-radius: $border-radius-3;
 }

 ::ng-deep {
   .ap-navigation-item.selected,  .ap-navigation-item.active {
     // custom styles for the selected item
     font-weight: $font-weight-2;
     color: $color-text-primary !important;
     background-color: $color-background-selected !important;

     .prefix-content{
       color: $color-text-primary !important;
       // custom styles for the selected prefix icon
     }
     .suffixIcon{
       // custom styles for the selected suffix icon
     }
   }
 }
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### collapsed


**Example #9** | **Variation**: collapsed | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = false;
simple: boolean = false;
collapsed: boolean = true;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

.ap-navigation-demo-wrapper {
  background-color: $color-background-default;
  width: fit-content;
  border-radius: $border-radius-3;
}

::ng-deep {
  .ap-navigation-item.selected,  .ap-navigation-item.active {
    // custom styles for the selected item
    font-weight: $font-weight-2;
    color: $color-text-primary !important;
    background-color: $color-background-hover-selected !important;

    .prefix-content{
      color: $color-text-primary !important;
      // custom styles for the selected prefix icon
    }
    .suffixIcon{
      // custom styles for the selected suffix icon
    }
  }
}
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### collapsed - solid


**Example #10** | **Variation**: collapsed | **Modifier**: solid | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = true;
simple: boolean = false;
collapsed: boolean = true;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

 .ap-navigation-demo-wrapper {
   background-color: $color-background-default;
   width: fit-content;
   border-radius: $border-radius-3;
 }

 ::ng-deep {
   .ap-navigation-item.selected,  .ap-navigation-item.active {
     // custom styles for the selected item
     font-weight: $font-weight-2;
     color: $color-text-primary !important;
     background-color: $color-background-selected !important;

     .prefix-content{
       color: $color-text-primary !important;
       // custom styles for the selected prefix icon
     }
     .suffixIcon{
       // custom styles for the selected suffix icon
     }
   }
 }
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### collapsed - simple


**Example #11** | **Variation**: collapsed | **Modifier**: simple | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = false;
simple: boolean = true;
collapsed: boolean = true;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

.ap-navigation-demo-wrapper {
  background-color: $color-background-default;
  width: fit-content;
  border-radius: $border-radius-3;
}

::ng-deep {
  .ap-navigation-item.selected,  .ap-navigation-item.active {
    // custom styles for the selected item
    font-weight: $font-weight-2;
    color: $color-text-primary !important;
    background-color: $color-background-hover-selected !important;

    .prefix-content{
      color: $color-text-primary !important;
      // custom styles for the selected prefix icon
    }
    .suffixIcon{
      // custom styles for the selected suffix icon
    }
  }
}
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### collapsed - submenu


**Example #12** | **Variation**: collapsed | **Modifier**: submenu | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = false;
simple: boolean = false;
collapsed: boolean = true;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    open: true,
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    open: true,
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

.ap-navigation-demo-wrapper {
  background-color: $color-background-default;
  width: fit-content;
  border-radius: $border-radius-3;
}

::ng-deep {
  .ap-navigation-item.selected,  .ap-navigation-item.active {
    // custom styles for the selected item
    font-weight: $font-weight-2;
    color: $color-text-primary !important;
    background-color: $color-background-hover-selected !important;

    .prefix-content{
      color: $color-text-primary !important;
      // custom styles for the selected prefix icon
    }
    .suffixIcon{
      // custom styles for the selected suffix icon
    }
  }
}
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### collapsed - solid-simple


**Example #13** | **Variation**: collapsed | **Modifier**: solid-simple | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = true;
simple: boolean = true;
collapsed: boolean = true;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

 .ap-navigation-demo-wrapper {
   background-color: $color-background-default;
   width: fit-content;
   border-radius: $border-radius-3;
 }

 ::ng-deep {
   .ap-navigation-item.selected,  .ap-navigation-item.active {
     // custom styles for the selected item
     font-weight: $font-weight-2;
     color: $color-text-primary !important;
     background-color: $color-background-selected !important;

     .prefix-content{
       color: $color-text-primary !important;
       // custom styles for the selected prefix icon
     }
     .suffixIcon{
       // custom styles for the selected suffix icon
     }
   }
 }
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### collapsed - solid-submenu


**Example #14** | **Variation**: collapsed | **Modifier**: solid-submenu | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = true;
simple: boolean = false;
collapsed: boolean = true;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    open: true,
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    open: true,
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

 .ap-navigation-demo-wrapper {
   background-color: $color-background-default;
   width: fit-content;
   border-radius: $border-radius-3;
 }

 ::ng-deep {
   .ap-navigation-item.selected,  .ap-navigation-item.active {
     // custom styles for the selected item
     font-weight: $font-weight-2;
     color: $color-text-primary !important;
     background-color: $color-background-selected !important;

     .prefix-content{
       color: $color-text-primary !important;
       // custom styles for the selected prefix icon
     }
     .suffixIcon{
       // custom styles for the selected suffix icon
     }
   }
 }
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### collapsed - simple-submenu


**Example #15** | **Variation**: collapsed | **Modifier**: simple-submenu | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = false;
simple: boolean = true;
collapsed: boolean = true;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    open: true,
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    open: true,
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

.ap-navigation-demo-wrapper {
  background-color: $color-background-default;
  width: fit-content;
  border-radius: $border-radius-3;
}

::ng-deep {
  .ap-navigation-item.selected,  .ap-navigation-item.active {
    // custom styles for the selected item
    font-weight: $font-weight-2;
    color: $color-text-primary !important;
    background-color: $color-background-hover-selected !important;

    .prefix-content{
      color: $color-text-primary !important;
      // custom styles for the selected prefix icon
    }
    .suffixIcon{
      // custom styles for the selected suffix icon
    }
  }
}
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### collapsed - solid-simple-submenu


**Example #16** | **Variation**: collapsed | **Modifier**: solid-simple-submenu | **State**: None

#### Module Import

```typescript
import { NavigationModule } from '@appkit4/angular-components/navigation';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="ap-navigation-demo-wrapper">
    <ap-navigation 
        [title]="'Appkit'"
        [(selectedIndex)]="selectedIndex"
        [(selectedSubIndex)]="selectedSubIndex"
        [avatarName]="'VR'"
        [navList]="navList"
        [width]="'17.5rem'"
        [solid]="solid"
        [hasHeader]="!simple"
        [collapsed]="collapsed"
        (onCollapsedItems) ="onCollapsedSidebar($event)"
        (onClick)="redirect($event)">
    </ap-navigation>
</div>
```

#### TypeScript

```typescript
import { NavigationItem } from "@appkit4/angular-components/navigation";

solid: boolean = true;
simple: boolean = true;
collapsed: boolean = true;
selectedIndex: number = 0;
selectedSubIndex:number = -1;

navList: Array<NavigationItem> = [
  {
    name: 'Welcome',
    routerLink: '/welcome',
    prefixIcon: 'hand-wave',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Getting started',
    routerLink: '/start',
    prefixIcon: 'download-cloud',
    prefixCollapsedIcon: 'grid-view'
  },
  {
    name: 'Styleguide',
    routerLink: '/style-guide',
    prefixIcon: 'venn-abc',
    prefixCollapsedIcon: 'venn-abc'
  },
  {
    name: 'Components',
    routerLink: '/components',
    prefixIcon: 'particulates',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'particulates',
    open: true,
    children: [
      { name: 'Lorem ipsum',  routerLink: '/components/subItem', prefixIcon: 'particulates',suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/11', prefixIcon: 'particulates' ,prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/22',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/33', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
        { name: 'Lorem ipsum',  routerLink: '/components/subItem/44',  prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view',},
      ]},
      { name: 'Lorem ipsum',  routerLink: '/components/2',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/3', prefixIcon: 'particulates',  prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/4',  prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
      { name: 'Lorem ipsum',  routerLink: '/components/5', prefixIcon: 'particulates', prefixCollapsedIcon: 'grid-view',},
    ],
  },
  {
    name: 'Support',
    prefixIcon: 'help-question',
    suffixIcon: 'down-chevron',
    prefixCollapsedIcon: 'help-question',
    open: true,
    children: [
      { name: 'FAQs',  routerLink: '/support/faqs', suffixIcon: 'down-chevron', prefixCollapsedIcon: 'grid-view', open: true, children: [
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/11', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/22', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/33', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
        { name: 'Lorem ipsum',  routerLink: '/support/faqs/44', prefixIcon: 'particulates',prefixCollapsedIcon: 'grid-view'},
      ] },
      { name: 'Versions',  routerLink: '/support/versions', },
    ],
    divider: true,
  }

];

onCollapsedSidebar(event: any): void {
  console.log(event);
}

redirect(event: any) {
  console.log(event);
  // routing logic here
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

 .ap-navigation-demo-wrapper {
   background-color: $color-background-default;
   width: fit-content;
   border-radius: $border-radius-3;
 }

 ::ng-deep {
   .ap-navigation-item.selected,  .ap-navigation-item.active {
     // custom styles for the selected item
     font-weight: $font-weight-2;
     color: $color-text-primary !important;
     background-color: $color-background-selected !important;

     .prefix-content{
       color: $color-text-primary !important;
       // custom styles for the selected prefix icon
     }
     .suffixIcon{
       // custom styles for the selected suffix icon
     }
   }
 }
```

<!-- /EXAMPLE:16 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| hasHeader | boolean | When specified, displays the header which includes the PwC logo, the title and the avatar. | true | 4.1.0 |
| hasFooter | boolean | When specified, displays the collapse button. | true | 4.2.0 |
| hasLogo | boolean | When specified, displays the PwC logo. | true | 4.0.0 |
| title | string | The title of the sidebar. | - | 4.0.0 |
| avatarName | string | The avatar name of the sidebar. | - | 4.0.0 |
| navList | Array<NavigationItem> | The list of the sidebar items. NavigationItem interface structure is { name: string, routerLink?: string, prefixIcon?: string, suffixIcon?: string, prefixCollapsedIcon?: string, customClass?: string, children?: Array&lt;NavigationItem&gt;, open?: boolean, divider?: boolean }. Notice: the 'divider' property is supported from 4.6.0. | \[\] | 4.0.0 |
| solid | boolean | Apply solid styles to the sidebar. | false | 4.0.0 |
| selected | Array<string>: \[<parent name>\] \| \[<parent name>, <children name>\] | The list of sidebar selected parent name and children name. When setting only \[&lt;parent name&gt;\], it will expand the node if the menu item has children, the L2 would not be expanded if L2 has children. It will select the node if the menu item doesn't have children. When setting \[&lt;parent name&gt;, &lt;children name&gt;\], it will select the children node under the parent node. User can choose to use selected attributes or index relevant attributes to control the activity name. | \[\] | 4.5.0 |
| selectedIndex | number | Index of the selected sidebar item. | - | 4.0.0 |
| selectedSubIndex | number | Index of the selected secondary sidebar item, only works when 'selectedIndex' have a value and children. Note: two-way binding is supported, for example, \[(selectedSubIndex)\]='customVariable'. | - | 4.2.0 |
| selectedNestedIndex | number | Index of the selected nested sidebar item, only works when 'selectedIndex' and 'selectedSubIndex' have a value and children. Note: two-way binding is supported, for example, \[(selectedNestedIndex)\]='customVariable'. | - | 4.9.1 |
| disabledMenuPopup | boolean | Disabled the menu popup when collapsed sidebar. | false | 4.9.1 |
| width | string | Width of the sidebar. | '280px' | 4.0.0 |
| role | string | The role of each sidebar item | 'menuitem' | 4.0.0 |
| collapsed | boolean:'true'\|'false' | The status of sidebar. | false | 4.0.0 |
| toggleFromSuffixIconOnly | boolean:'true'\|'false' | Whether only click the suffix icon will trigger each sidebar item. When this value is true, parent item and sub item will support different routing pages.When collapsed value is true, the setting toggleFromSuffixIconOnly is not effective. | false | 4.3.0 |
| showTooltip | boolean:'true'\|'false' | Display the tooltip when collapsed sidebar item. | true | 4.3.0 |
| direction | string: 'top'\|'top-left'\|'top-right'\|'right'\|'right-top'\|'right-bottom'\|'bottom'\|'bottom-left'\|'bottom-right'\|'left'\|'left-top'\|'left-bottom' | The direction of the tooltip. | 'right' | 4.3.0 |
| distance | number | The distance between the tooltip content and the host element. | 8 | 4.3.0 |
| onClickLogo | EventEmitter<object> | Callback to invoke when clicking the PwC logo | - | 4.0.0 |
| onClick | EventEmitter<object> | Callback to invoke when clicking the sidebar item | - | 4.0.0 |
| onCollapsedItems | EventEmitter<object> | Callback to invoke when collapsed all the items. | - | 4.0.0 |
| onClickSuffixIcon | EventEmitter<object> | Callback to invoke when click the item suffix icon.Only been support when toggleFromSuffixIconOnly property value is true. | - | 4.5.0 |
| style | string | The inline style of the component | '' | 4.0.0 |
| styleClass | string | The style class names of the component | '' | 4.0.0 |


<!-- /SECTION:properties -->