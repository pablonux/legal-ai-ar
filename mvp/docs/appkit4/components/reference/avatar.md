---
component: avatar
framework: angular
---

# Avatar Component

## Overview

AppKit Avatar component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Avatars act as a visual representation of a user in a product. Our avatars represent single individuals or the presence of large user groups to give more context. Use avatars as follows:
- Use Single avatars to identify users
- Use Group and Extra avatars to identify large product entities like projects, spaces, groups, rooms

### Anatomy

- **Color:** Background color used to represent the person or product.
- **Label:** It can be the initials of the user or letters that represent the name of the product.
- **Number:** A numeric badge is used to represent the total number of users in a group or space.

### Variants

Usage of one or the other depends on the user and the product.

#### Single

Represents one user

#### Group

Represents two or more users

#### Single (Compact)

Represents one user

#### Group (Compact)

Represents two or more users

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161642-106854%26viewport%3D-168%252C-71%252C0.47%26t%3Dg9ztr04A9nBK7Goj-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- There is no recommended position to place avatars on a product. However, it is common to use it as a complement to other design entities such as headers, sideNav, or user menus. They should not be closer than 12px from the edges of the container where they are placed.
- When additional information needs to be added, side or bottom texts can be used. These must be separated by 12px or more from the avatar.
- For better recognition, the colors in avatar groups should be diversified. Colors can be set from the primary colors or the data visualization ramps.
- One of the most common patterns for an avatar when it is hovered or clicked is the expansion of users’ additional info. When using menus that pop up from the avatar, the distance should be 8px.

#### How not to use

- Do not use or design avatars that will steal the spotlight.

### Behavior

- Avatars can be clickable or not and can have CTAs. The most usual action is to open a user menu.
- They can also be accompanied by a tooltip component.

### Accessibility

**Avatar-only**: provide alternative text for image. 
- In chat thread example for avatar with initials, alt= “User JD”
- In chat thread example for avatar with profile pic, alt= “John Doe avatar”
- Avatar with number, number should be announced by screen reader, ex. “18 items”

**Avatar with descriptive label or name:** use **blank** alt-text for avatar since label is available. 
- Example- alt= “”

**Avatar-clickable:** provide action of the element 
- Example for avatar used to open user menu- aria-label= “user menu” or “user profile”

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 8


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | single | `section: "example:1"` |
| 2 | single - dropdown | `section: "example:2"` |
| 3 | single - compact | `section: "example:3"` |
| 4 | single - dropdown-compact | `section: "example:4"` |
| 5 | group | `section: "example:5"` |
| 6 | group - extra | `section: "example:6"` |
| 7 | group - compact | `section: "example:7"` |
| 8 | group - extra-compact | `section: "example:8"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### single


**Example #1** | **Variation**: single | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-avatar diameter="40" borderWidth="0" name="AB" [disabled]="true"></ap-avatar>
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### single - dropdown


**Example #2** | **Variation**: single | **Modifier**: dropdown | **State**: None

#### Module Import

```typescript
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="avatar-dropdown-container">
  <ap-avatar diameter="40" borderWidth="0" name="AB" [disabled]="false" [withDropdown]="withDropdown"
  [list]="list1" [ariaLabel]="'settings menu'">
      <ng-template  ngTemplate="dropdownTemp">
          <ng-container *ngFor="let item of list1; let i = index;">
              <ap-dropdown-list-item #dropdownListItem [item]="item" (onSelectItem)="onSelectItem($event)">
                  <ng-template *ngIf="item.iconName" ngTemplate="prefixTemp">
                      <span class="Appkit4-icon icon-{{item.iconName}}"></span>
                  </ng-template>
              </ap-dropdown-list-item>
          </ng-container>
      </ng-template>
  </ap-avatar>
</div>
```

#### TypeScript

```typescript
withDropdown = true;
list1: any = [
  { value: 'item1', label: 'Keyboard shortcuts', iconName: 'keyboard-outline' },
  { value: 'item2', label: 'Preferences', iconName: 'setting-outline'},
  { value: 'item3', label: 'Logout' , iconName: 'lockclosed-locked-outline'},
]

onSelectItem(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.avatar-dropdown-container{
  display: flex;
  justify-content: end;
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### single - compact


**Example #3** | **Variation**: single | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<ap-avatar diameter="32" [compact]="true" borderWidth="0" name="AB" [disabled]="true"></ap-avatar>
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### single - dropdown-compact


**Example #4** | **Variation**: single | **Modifier**: dropdown-compact | **State**: None

#### Module Import

```typescript
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="avatar-dropdown-container">
  <ap-avatar diameter="32" [compact]="true" borderWidth="0" name="AB" [disabled]="false" [withDropdown]="withDropdown"
  [list]="list1" [ariaLabel]="'settings menu'">
      <ng-template  ngTemplate="dropdownTemp">
          <ng-container *ngFor="let item of list1; let i = index;">
              <ap-dropdown-list-item #dropdownListItem [item]="item" (onSelectItem)="onSelectItem($event)">
                  <ng-template *ngIf="item.iconName" ngTemplate="prefixTemp">
                      <span class="Appkit4-icon icon-{{item.iconName}}"></span>
                  </ng-template>
              </ap-dropdown-list-item>
          </ng-container>
      </ng-template>
  </ap-avatar>
</div>
```

#### TypeScript

```typescript
withDropdown = true;
list1: any = [
  { value: 'item1', label: 'Keyboard shortcuts', iconName: 'keyboard-outline' },
  { value: 'item2', label: 'Preferences', iconName: 'setting-outline'},
  { value: 'item3', label: 'Logout' , iconName: 'lockclosed-locked-outline'},
]

onSelectItem(event: any) {
  console.log(event);
}
```

#### SCSS Styles

```scss
.avatar-dropdown-container{
  display: flex;
  justify-content: end;
}
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### group


**Example #5** | **Variation**: group | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="avatar-wrapper">
  <ap-avatar name="AB" diameter="40" borderWidth="0" zIndex="5" [disabled]="true"></ap-avatar>
  <ap-avatar [withMask]="true" name="CD" diameter="40" borderWidth="0" backgroundColor="#007aff" zIndex="3" marginLeft="-4" [disabled]="true"></ap-avatar>
  <ap-avatar [withMask]="true" name="EF" diameter="40" borderWidth="0" backgroundColor="#34c759" zIndex="1" marginLeft="-4" [disabled]="true"></ap-avatar>
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.avatar-wrapper {
  display: flex;
  align-items: center;
}

.avatar-wrapper + .avatar-wrapper {
    margin-top: $spacing-2;
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### group - extra


**Example #6** | **Variation**: group | **Modifier**: extra | **State**: None

#### Module Import

```typescript
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="avatar-wrapper">
  <ap-avatar name="AB" diameter="40" borderWidth="0" zIndex="5" [disabled]="true"></ap-avatar>
  <ap-avatar [withMask]="true" name="CD" diameter="40" borderWidth="0" backgroundColor="#007aff" zIndex="3" marginLeft="-4" [disabled]="true"></ap-avatar>
  <ap-avatar [withMask]="true" name="24" diameter="40" borderWidth="0" backgroundColor="#f3f3f3" fontColor="#474747" zIndex="1" marginLeft="-4" [disabled]="true"></ap-avatar>
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.avatar-wrapper {
  display: flex;
  align-items: center;
}

.avatar-wrapper + .avatar-wrapper {
    margin-top: $spacing-2;
}
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### group - compact


**Example #7** | **Variation**: group | **Modifier**: compact | **State**: None

#### Module Import

```typescript
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="avatar-wrapper">
  <ap-avatar name="AB" diameter="32" [compact]="true" borderWidth="0" zIndex="5" [disabled]="true"></ap-avatar>
  <ap-avatar [withMask]="true" name="CD" diameter="32" [compact]="true" borderWidth="0" backgroundColor="#007aff" zIndex="3" marginLeft="-4" [disabled]="true"></ap-avatar>
  <ap-avatar [withMask]="true" name="EF" diameter="32" [compact]="true" borderWidth="0" backgroundColor="#34c759" zIndex="1" marginLeft="-4" [disabled]="true"></ap-avatar>
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.avatar-wrapper {
  display: flex;
  align-items: center;
}

.avatar-wrapper + .avatar-wrapper {
    margin-top: $spacing-2;
}
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### group - extra-compact


**Example #8** | **Variation**: group | **Modifier**: extra-compact | **State**: None

#### Module Import

```typescript
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class="avatar-wrapper">
  <ap-avatar name="AB" diameter="32" [compact]="true" borderWidth="0" zIndex="5" [disabled]="true"></ap-avatar>
  <ap-avatar [withMask]="true" name="CD" diameter="32" [compact]="true" borderWidth="0" backgroundColor="#007aff" zIndex="3" marginLeft="-4" [disabled]="true"></ap-avatar>
  <ap-avatar [withMask]="true" name="24" diameter="32" [compact]="true" borderWidth="0" backgroundColor="#f3f3f3" fontColor="#474747" zIndex="1" marginLeft="-4" [disabled]="true"></ap-avatar>
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
@import './node_modules/@appkit4/styles/scss/_variables.scss';

.avatar-wrapper {
  display: flex;
  align-items: center;
}

.avatar-wrapper + .avatar-wrapper {
    margin-top: $spacing-2;
}
```

<!-- /EXAMPLE:8 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| name | string | Name to display in the avatar. | '' | 4.0.0 |
| diameter | string | Width and height of the avatar. | '40' | 4.0.0 |
| backgroundColor | string | Background color of the avatar. | Default value is the primary color. | 4.0.0 |
| fontColor | string | Font color of the avatar. | '\#ffffff' | 4.0.0 |
| borderColor | string | Border color of the avatar. | '\#ffffff' | 4.0.0 |
| borderWidth | string | Border width of the avatar. | '4' | 4.0.0 |
| marginLeft | string | Margin left of the avatar. | '0' | 4.0.0 |
| zIndex | string | Z-index of the avatar. | '0' | 4.0.0 |
| imageSrc | string | The path of image src for avatar. When specified, it only shows this image without the name. | '' | 4.0.0 |
| style | string | Inline style of the component. | '' | 4.5.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| role | string: 'button'\|'link' | The role of the avatar. | - | 4.0.0 |
| disabled | boolean | If this value is true, it specifies that the avatar should be disabled. | false | 4.0.0 |
| withDropdown | boolean | If this value is true, the avatar will be used with dropdown. | false | 4.4.0 |
| list | Array<any> | The data of list, use with the 'withDropdown' property. | - | 4.4.0 |
| tabindex | number | The tabindex of avatar, will fail if the disabled is true. | 0 | 4.0.0 |
| ariaLabel | string | The aria-label of the avatar, only works when the avatar role is 'button' or 'link' | '' | 4.0.0 |
| withMask | boolean | If it is true, it specifies that the avatar will have an mask image, which can be applied to the group avatar. | false | 4.0.0 |
| onClick | EventEmitter<Event> | Callback to invoke when the avatar is clicked. | - | 4.0.0 |
| onFocus | EventEmitter<Event> | Callback to invoke when the avatar is focused. | - | 4.0.0 |
| onBlur | EventEmitter<Event> | Callback to invoke when the avatar is blurred. | - | 4.0.0 |
| compact | boolean | When specified, the avatar displays in the compact size. The default diameter for compact avatar is 32, if set a different diameter value, it will only change the width and height of the avatar, the font style will not change. | false | 4.6.0 |
| alignDirection | string: 'left'\|'right' | Align the direction of the dropdown list item | right | 4.6.0 |


<!-- /SECTION:properties -->
