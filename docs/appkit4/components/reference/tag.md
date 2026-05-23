---
component: tag
framework: angular
---

# Tag Component

## Overview

AppKit Tag component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Tags for:

- Adding keywords or tags to blog posts, articles, or products
- Categorizing items in a list or table
- Filtering content by topic or category

### Anatomy

1. **Label:** Text that represents the tag.

2. **Container:** Background color that distinguishes and shapes the tag.

3. **Close icon:** Allows the user to remove the tag.

### Variants

#### Inline:

A standalone Tag that can also be nested within components.

#### Contained:

An emphasized Tag that is always attached to a background container.

#### Group:

Tag groups are typically left-aligned when stacked and display a specific margin between them, depending on the size of tag used. The recommended space between each tag should be a multiple of 4 pixels.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-131550%26viewport%3D1286%252C-37503%252C0.36%26t%3D7t9sNELJs4rKTvrL-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Use clear and concise labels that accurately describe the content or item being tagged
- Limit the number of tags to avoid clutter and confusion
- Use a consistent background color for each tag to create a clear visual hierarchy
- There are no definitive rules for selecting a color for a tag component. Designers can choose to associate any color with a product, category, or user persona. The only suggestion is to maintain consistency between the color and the associated category, and keep in mind that using too many colors in certain situations could create a high cognitive load for some users.
- Provide feedback when tags are added, removed, or edited to confirm the action was successful
- Tags should be short and ideally contain only one or two words.
- Tags can be used to enable users to attach searchable keywords to a record.

#### How not to use

- Do not use tags as a primary navigation.
- Do not use tags to indicate a status, use badges instead.

### Behavior

- Adding a Tag: When the user adds a tag, it should appear visually in the UI. 
- Removing a Tag: The dismissible feature enables users to actively remove any tag, commonly used in filtering.

### Accessibility

- Link text is the visible label for native HTML link.
- Ensure link text is clear and concise.
- Ensure tags have appropriate contrast against the surrounding background.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 16


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | inline - outlined | `section: "example:1"` |
| 2 | inline - outlined - disabled | `section: "example:2"` |
| 3 | inline - filled | `section: "example:3"` |
| 4 | inline - filled - disabled | `section: "example:4"` |
| 5 | inline - outlined-group | `section: "example:5"` |
| 6 | inline - outlined-group - disabled | `section: "example:6"` |
| 7 | inline - filled-group | `section: "example:7"` |
| 8 | inline - filled-group - disabled | `section: "example:8"` |
| 9 | contained - outlined | `section: "example:9"` |
| 10 | contained - outlined - disabled | `section: "example:10"` |
| 11 | contained - filled | `section: "example:11"` |
| 12 | contained - filled - disabled | `section: "example:12"` |
| 13 | contained - outlined-group | `section: "example:13"` |
| 14 | contained - outlined-group - disabled | `section: "example:14"` |
| 15 | contained - filled-group | `section: "example:15"` |
| 16 | contained - filled-group - disabled | `section: "example:16"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### inline - outlined


**Example #1** | **Variation**: inline | **Modifier**: outlined | **State**: None

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ap-tag [type]="'outlined'" [size]="size" [text]="'Primary'" (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
```

#### TypeScript

```typescript
tagDisabled: boolean = false;
size:string = 'small';
closeTag(el:any) {
  console.log(el);
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### inline - outlined - disabled


**Example #2** | **Variation**: inline | **Modifier**: outlined | **State**: disabled

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ap-tag [type]="'outlined'" [size]="size" [text]="'Primary'" (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
```

#### TypeScript

```typescript
tagDisabled: boolean = true;
size:string = 'small';
closeTag(el:any) {
  console.log(el);
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### inline - filled


**Example #3** | **Variation**: inline | **Modifier**: filled | **State**: None

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ap-tag [text]="'Primary'" [size]="size" (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
```

#### TypeScript

```typescript
tagDisabled: boolean = false;
size:string = 'small';
closeTag(el:any) {
  console.log(el);
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### inline - filled - disabled


**Example #4** | **Variation**: inline | **Modifier**: filled | **State**: disabled

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ap-tag [text]="'Primary'" [size]="size" (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
```

#### TypeScript

```typescript
tagDisabled: boolean = true;
size:string = 'small';
closeTag(el:any) {
  console.log(el);
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### inline - outlined-group


**Example #5** | **Variation**: inline | **Modifier**: outlined-group | **State**: None

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ul class="tagList">
    <li>
        <ap-tag [type]="type" [text]="'Miami'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Denver'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Milwaukee'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Dallas'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
</ul>
```

#### TypeScript

```typescript
type:string = 'outlined'
tagDisabled: boolean = false
closeTag(el:any) {
  console.log(el);
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

.tagList {
  list-style-type: none;
  display: inline-flex;

  li {
      margin-right: 4px;
  }
}        

[data-mode="dark"] {
    .ap-tag.ap-tag-outlined {
      color: #fff !important;

      .Appkit4-icon {
        color: #fff !important;
      }
    } 

}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### inline - outlined-group - disabled


**Example #6** | **Variation**: inline | **Modifier**: outlined-group | **State**: disabled

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ul class="tagList">
    <li>
        <ap-tag [type]="type" [text]="'Miami'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Denver'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Milwaukee'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Dallas'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
</ul>
```

#### TypeScript

```typescript
type:string = 'outlined'
tagDisabled: boolean = true
closeTag(el:any) {
  console.log(el);
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

.tagList {
  list-style-type: none;
  display: inline-flex;

  li {
      margin-right: 4px;
  }
}        

[data-mode="dark"] {
    .ap-tag.ap-tag-outlined {
      color: #fff !important;

      .Appkit4-icon {
        color: #fff !important;
      }
    } 

}
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### inline - filled-group


**Example #7** | **Variation**: inline | **Modifier**: filled-group | **State**: None

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ul class="tagList">
    <li>
        <ap-tag [type]="type" [text]="'Miami'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Denver'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Milwaukee'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Dallas'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
</ul>
```

#### TypeScript

```typescript
type:string = 'filled'
tagDisabled: boolean = false
closeTag(el:any) {
  console.log(el);
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

.tagList {
  list-style-type: none;
  display: inline-flex;

  li {
      margin-right: 4px;
  }
}        

[data-mode="dark"] {
    .ap-tag.ap-tag-outlined {
      color: #fff !important;

      .Appkit4-icon {
        color: #fff !important;
      }
    } 

}
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### inline - filled-group - disabled


**Example #8** | **Variation**: inline | **Modifier**: filled-group | **State**: disabled

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ul class="tagList">
    <li>
        <ap-tag [type]="type" [text]="'Miami'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Denver'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Milwaukee'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [text]="'Dallas'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
</ul>
```

#### TypeScript

```typescript
type:string = 'filled'
tagDisabled: boolean = true
closeTag(el:any) {
  console.log(el);
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

.tagList {
  list-style-type: none;
  display: inline-flex;

  li {
      margin-right: 4px;
  }
}        

[data-mode="dark"] {
    .ap-tag.ap-tag-outlined {
      color: #fff !important;

      .Appkit4-icon {
        color: #fff !important;
      }
    } 

}
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### contained - outlined


**Example #9** | **Variation**: contained | **Modifier**: outlined | **State**: None

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ap-tag [type]="'outlined'" [size]="size" [text]="'Primary'" (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
```

#### TypeScript

```typescript
tagDisabled: boolean = false;
size:string = 'large';
closeTag(el:any) {
  console.log(el);
}
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### contained - outlined - disabled


**Example #10** | **Variation**: contained | **Modifier**: outlined | **State**: disabled

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ap-tag [type]="'outlined'" [size]="size" [text]="'Primary'" (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
```

#### TypeScript

```typescript
tagDisabled: boolean = true;
size:string = 'large';
closeTag(el:any) {
  console.log(el);
}
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### contained - filled


**Example #11** | **Variation**: contained | **Modifier**: filled | **State**: None

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ap-tag [text]="'Primary'" [size]="size" (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
```

#### TypeScript

```typescript
tagDisabled: boolean = false;
size:string = 'large';
closeTag(el:any) {
  console.log(el);
}
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### contained - filled - disabled


**Example #12** | **Variation**: contained | **Modifier**: filled | **State**: disabled

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ap-tag [text]="'Primary'" [size]="size" (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
```

#### TypeScript

```typescript
tagDisabled: boolean = true;
size:string = 'large';
closeTag(el:any) {
  console.log(el);
}
```

<!-- /EXAMPLE:12 -->

<!-- EXAMPLE:13 -->
### contained - outlined-group


**Example #13** | **Variation**: contained | **Modifier**: outlined-group | **State**: None

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ul class="tagList">
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Miami'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Denver'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Milwaukee'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [size]="'large'" [text]="'Dallas'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
</ul>
```

#### TypeScript

```typescript
type:string = 'outlined'
tagDisabled: boolean = false
closeTag(el:any) {
  console.log(el);
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

.tagList {
  list-style-type: none;
  display: inline-flex;

  li {
      margin-right: 4px;
  }
}        

[data-mode="dark"] {
    .ap-tag.ap-tag-outlined {
      color: #fff !important;

      .Appkit4-icon {
        color: #fff !important;
      }
    } 

}
```

<!-- /EXAMPLE:13 -->

<!-- EXAMPLE:14 -->
### contained - outlined-group - disabled


**Example #14** | **Variation**: contained | **Modifier**: outlined-group | **State**: disabled

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ul class="tagList">
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Miami'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Denver'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Milwaukee'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [size]="'large'" [text]="'Dallas'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
</ul>
```

#### TypeScript

```typescript
type:string = 'outlined'
tagDisabled: boolean = true
closeTag(el:any) {
  console.log(el);
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

.tagList {
  list-style-type: none;
  display: inline-flex;

  li {
      margin-right: 4px;
  }
}        

[data-mode="dark"] {
    .ap-tag.ap-tag-outlined {
      color: #fff !important;

      .Appkit4-icon {
        color: #fff !important;
      }
    } 

}
```

<!-- /EXAMPLE:14 -->

<!-- EXAMPLE:15 -->
### contained - filled-group


**Example #15** | **Variation**: contained | **Modifier**: filled-group | **State**: None

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ul class="tagList">
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Miami'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Denver'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Milwaukee'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [size]="'large'" [text]="'Dallas'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
</ul>
```

#### TypeScript

```typescript
type:string = 'filled'
tagDisabled: boolean = false
closeTag(el:any) {
  console.log(el);
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

.tagList {
  list-style-type: none;
  display: inline-flex;

  li {
      margin-right: 4px;
  }
}        

[data-mode="dark"] {
    .ap-tag.ap-tag-outlined {
      color: #fff !important;

      .Appkit4-icon {
        color: #fff !important;
      }
    } 

}
```

<!-- /EXAMPLE:15 -->

<!-- EXAMPLE:16 -->
### contained - filled-group - disabled


**Example #16** | **Variation**: contained | **Modifier**: filled-group | **State**: disabled

#### Module Import

```typescript
import { TagModule } from '@appkit4/angular-components/tag';
```

#### HTML Template

```html
<ul class="tagList">
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Miami'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Denver'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type"  [size]="'large'" [text]="'Milwaukee'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
    <li>
        <ap-tag [type]="type" [size]="'large'" [text]="'Dallas'"  (onClose)="closeTag($event)" [disabled]="tagDisabled"></ap-tag>
    </li>
</ul>
```

#### TypeScript

```typescript
type:string = 'filled'
tagDisabled: boolean = true
closeTag(el:any) {
  console.log(el);
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

.tagList {
  list-style-type: none;
  display: inline-flex;

  li {
      margin-right: 4px;
  }
}        

[data-mode="dark"] {
    .ap-tag.ap-tag-outlined {
      color: #fff !important;

      .Appkit4-icon {
        color: #fff !important;
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
| size | string: 'small'\|'large' | Size of the tag. | 'small' | 4.0.0 |
| text | string | Text of the tag. | - | 4.0.0 |
| backgroundColor | string | Background color of tag. | Default value is the primary color. | 4.0.0 |
| type | string: 'filled'\|'outlined' | The type of tag. | Default value is the 'filled'. | 4.14.0 |
| fontColor | string | Text color and X icon color of tag. | \#FFFFFF | 4.1.0 |
| inputId | string | Identifier of the focus input to match a label defined for the component. | - | 4.0.0 |
| showClose | boolean | When specified, shows the close button. | true | 4.0.0 |
| disabled | boolean | If it is true, it specifies that the tag should be disabled. | false | 4.0.0 |
| ariaLabel | string | The tag label. | - | 4.7.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| onClose | EventEmitter<ElementRef> | Callback to invoke when the close button is clicked. | - | 4.0.0 |


<!-- /SECTION:properties -->