---
component: list
framework: angular
---

# List Component

## Overview

AppKit List component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use list component:

- To display a collection of items in a structured and organized way.
- To display information such as a list of items, a list of tasks, or a list of contacts.

### Anatomy

**1. ****List items:** Each item in the list is represented by a list item. A list item can contain text, images, and other elements such as buttons or icons.

**2. ****Element after list item:** Elements that are used to provide additional information about the list item, such as description text, timestamp, toggle and close or custom icons.

**3. Element before list item:** Seleccion element before list item, radio or checkboxes.

**4. Container:** Contains the list items.

### Variants

#### Default:

Basic list that contains list items and can have an additional element after the list item.

#### Selection:

Selection list that allows the user to select one or multiple items in the list using checkboxes or radio buttons.

#### Avatar:

Displays user avatars in the list items.

#### Complex:

Lista that allows more information in the list items.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-91558%26viewport%3D187%252C-32452%252C0.59%26t%3DyGRxvpUFVlbaMvbS-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Lists should be used anytime a collection of related data needs to be displayed to the user in an ordered fashion.
- Lists are displayed using checkboxes, within dropdown menus or other graphical elements.
- Make lists scrollable.
- Keep the list items short and to the point to make them easy to scan and understand.
- Use meaningful icons and images to provide additional information about the items in the list.

#### How not to use

- Avoid using unnecessary elements in the list to keep it simple and easy to use.
- Do not use a list to display content that is completely unrelated or in an illogical order.

### Behavior

- The list component should have the ability to handle different types of interactions such as hover, click, and touch events.
- The list component should be able to handle different types of states such as selected, disabled, and error states.
- Consider making lists sortable.

### Accessibility

- Must provide proper title/name of list.
- List items must be contained in a group for screen reader to announce number of items.
- Use bulleted list (ul) for items of no specific order.
- Use numbered list/ordered list (ol) for items in specific order such as step by step directions.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 12


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | default - timeAfterListItem | `section: "example:1"` |
| 2 | default - textAfterListItem | `section: "example:2"` |
| 3 | default - toggleAfterListItem | `section: "example:3"` |
| 4 | default - customIconAfterListItem | `section: "example:4"` |
| 5 | default - closeIconAfterListItem | `section: "example:5"` |
| 6 | default - draggable | `section: "example:6"` |
| 7 | selection - checkboxList | `section: "example:7"` |
| 8 | selection - radioButtonList | `section: "example:8"` |
| 9 | selection - checkboxListWithBadge | `section: "example:9"` |
| 10 | avatar - description | `section: "example:10"` |
| 11 | avatar - noDescription | `section: "example:11"` |
| 12 | complex | `section: "example:12"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### default - timeAfterListItem


**Example #1** | **Variation**: default | **Modifier**: timeAfterListItem | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" [style]="'width:19.6875rem'">
    <ng-container *ngFor="let item of sampleData">
      <ap-list-item-divider *ngIf="item.divider"></ap-list-item-divider>
      <ap-list-item [selectedItem]="selectedItem" [attr.role]="'option'"
        [attr.aria-selected]="item.primary===selectedItem.primary" [class.selected]="item.primary===selectedItem.primary"
        (click)="handleListItemClick(item)" (keyup.enter)="handleListItemClick(item)">
        <span class='primary-text'>{{ item.primary }}</span>
        <ul ap-list-item-actions>
          <ap-list-item-action>
            <span class='secondary-text'>{{ item.secondary }}</span>
          </ap-list-item-action>
        </ul>
      </ap-list-item>
    </ng-container>
  </ap-list>
</div>
```

#### TypeScript

```typescript
sampleData = [
  { primary: "Hong Kong", secondary: "Just now" },
  { primary: "Stockholm", secondary: "3m ago" },
  { primary: "São Paulo", secondary: "34m ago" },
  { primary: "Saint Petersburg", secondary: "1h 58m ago", divider: false },
];

selectedItem = { primary: "Saint Petersburg", secondary: "1h 58m ago" };

handleListItemClick(item: any) {
  this.selectedItem = item;
}
```

#### SCSS Styles

```scss
.list-container {
  display: flex;
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### default - textAfterListItem


**Example #2** | **Variation**: default | **Modifier**: textAfterListItem | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" [style]="'width:19.6875rem'">
    <ap-list-item *ngFor="let item of sampleData" [selectedItem]="selectedItem" [attr.role]="'option'"
      [attr.aria-selected]="item.primary===selectedItem.primary" [class.selected]="item.primary===selectedItem.primary"
      [attr.aria-label]="item.primary +', '+ item.secondary" (click)="handleListItemClick(item)"
      (keyup.enter)="handleListItemClick(item)">
      <span class='primary-text' aria-hidden="true">{{ item.primary }}</span>
      <ul ap-list-item-actions [attr.aria-hidden]="true">
        <ap-list-item-action>
          <span class='secondary-text'>{{ item.secondary }}</span>
        </ap-list-item-action>
      </ul>
    </ap-list-item>
  </ap-list>
</div>
```

#### TypeScript

```typescript
sampleData = [
  { primary: "Myrtle Campbell", secondary: "Hong Kong" },
  { primary: "Franklin Tate", secondary: "Stockholm" },
  { primary: "Madge Wells", secondary: "São Paulo" },
  { primary: "Randy Hill", secondary: "Saint Petersburg" }
];

selectedItem: any = {};

handleListItemClick(item: any) {
  this.selectedItem = item;
}
```

#### SCSS Styles

```scss
.list-container {
  display: flex;
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### default - toggleAfterListItem


**Example #3** | **Variation**: default | **Modifier**: toggleAfterListItem | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
import { ToggleModule } from "@appkit4/angular-components/toggle";
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" [style]="'width:19.6875rem'">
    <ap-list-item *ngFor="let item of sampleData; let i = index;" (click)="handleSelectionClick(i)"
      (keyup.enter)="handleSelectionClick(i)" role="switch" [attr.aria-checked]="item.checked" aria-live="off"
      [attr.aria-label]="getAriaLabelWithToggle(item)">
      <ap-list-item-meta-avatar>{{ item.primary }} </ap-list-item-meta-avatar>
      <ap-list-item-extra>
        <ap-toggle [tabindex]="-1" (onChange)="onToggleSelected($event)" [(ngModel)]="item.checked"></ap-toggle>
      </ap-list-item-extra>
    </ap-list-item>
  </ap-list>
</div>
```

#### TypeScript

```typescript
sampleData = [
  { primary: "Myrtle Campbell", checked: false  },
  { primary: "Franklin Tate", checked: false  },
  { primary: "Madge Wells", checked: false  },
  { primary: "Randy Hill", checked: false  },
];
handleSelectionClick(index:number){
  this.sampleData[index].checked = !this.sampleData[index].checked;
}

onToggleSelected(event:any) {
  if (event) event.originEvent.stopPropagation();
}

getAriaLabelWithToggle(item: any) {
  let label = item.primary;
  return item.checked ? label : label + " "; // add one space to activate the aria-live to announce the changed status
}
```

#### SCSS Styles

```scss
.list-container {
  display: flex;
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### default - customIconAfterListItem


**Example #4** | **Variation**: default | **Modifier**: customIconAfterListItem | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" [style]="'width:19.6875rem'">
    <ap-list-item *ngFor="let item of sampleData" (click)="handleListItemClick(item)" [attr.role]="'radio'"
      (keyup.enter)="handleListItemClick(item)" [attr.aria-label]="getAriaLabelWithCustomIcon(item,'star','button')"
      [attr.aria-checked]="item.name===selectedItem.name" aria-live="off">
      <ul ap-list-item-actions>
        <ap-list-item-action>
          <span aria-hidden="true" class="Appkit4-icon icon-rating-fill" [class.selected]="item.name===selectedItem.name"></span>
        </ap-list-item-action>
      </ul>
      {{ item.name }}
    </ap-list-item>
  </ap-list>
</div>
```

#### TypeScript

```typescript
sampleData = [
  { name: "Hong Kong" },
  { name: "Stockholm" },
  { name: "São Paulo" },
  { name: "Saint Petersburg" },
];
selectedItem = { name: "Saint Petersburg" };
handleListItemClick(item: any) {
  this.selectedItem = item;
}
getAriaLabelWithCustomIcon(item: any, icon: string, role?: string) {
  let label = item.name;
  let button = role==="button" ? "" : "button";
  return item.active
    ? label + "," + icon + button
    : label + "," + icon + button + " "; // add one space to activate the aria-live to announce the changed status
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

 .list-container {
   display: flex;
 }
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### default - closeIconAfterListItem


**Example #5** | **Variation**: default | **Modifier**: closeIconAfterListItem | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" [style]="'width:19.6875rem'">
    <ap-list-item *ngFor="let item of sampleData" [attr.role]="'listitem'" aria-live="off"
      [attr.aria-label]="getAriaLabelWithCustomIcon(item,'close')">
      <ul ap-list-item-actions [attr.aria-label]="'close'" [attr.tabindex]="'0'" [attr.role]="'button'">
        <ap-list-item-action>
          <span class="Appkit4-icon icon-close-outline" aria-hidden="true"></span>
        </ap-list-item-action>
      </ul>
      <span aria-hidden="true">{{ item.name }}</span>
    </ap-list-item>
  </ap-list>
</div>
```

#### TypeScript

```typescript
sampleData = [
  { name: "Hong Kong" },
  { name: "Stockholm" },
  { name: "São Paulo" },
  { name: "Saint Petersburg" },
];
getAriaLabelWithCustomIcon(item: any, icon: string, role?: string) {
  let label = item.name;
  let button = role==="button" ? "" : "button";
  return item.active
    ? label + "," + icon + button
    : label + "," + icon + button + " "; // add one space to activate the aria-live to announce the changed status
}
```

#### SCSS Styles

```scss
.list-container {
  display: flex;
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### default - draggable


**Example #6** | **Variation**: default | **Modifier**: draggable | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
import { DragDropModule } from '@angular/cdk/drag-drop';
```

#### HTML Template

```html
<ap-list  cdkDropList (cdkDropListDropped)="drop($event)" [dragStatus]="dragStatus" >
  <ap-list-item  #listItem *ngFor="let item of sampleData; let index = index" cdkDrag [attr.role]="'option'"
    [class.selected]="item.primary===selectedItem.primary"
    [cdkDragDisabled]="isKeydownOperation ? item.disabled : null"
    [attr.aria-dropeffect]="'move'"
    [attr.aria-grabbed]="item.primary===selectedItem.primary"
    (click)="handleListItemClick($event,item, index)"
    (keydown)="keydownItemHandel($event,item, index)">
    <ap-list-item-draggable>
      <ng-container>
        <span *ngIf="item.dragging" class="Appkit4-icon icon-elevator-outline" [attr.aria-hidden]="true"></span>
        <span *ngIf="!item.dragging"  class="Appkit4-icon icon-menu-outline" [attr.aria-hidden]="true"></span>
      </ng-container>
      <ng-template  *ngIf="item.primary"  ngTemplate="headerTemp">
          <span>{{ item.primary}}</span>
      </ng-template>
      <ng-template  *ngIf="item.primary"  ngTemplate="bodyTemp">
        <span>{{ item.description}}</span>
    </ng-template>
      </ap-list-item-draggable>
  </ap-list-item>
</ap-list>

<!-- keyboard accessibility -->
<!--
    TAB KEY to focus on item
    ENTER and SPACE key to “GRAB” item
    UP/LEFT arrows to move UP
    DOWN/RIGHT arrows to move DOWN
    ENTER and SPACE key to “Release” item
-->
```

#### TypeScript

```typescript
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { QueryList, ViewChildren} from '@angular/core';
import { ApListItemComponent } from '@appkit4/angular-components/list';
import { Observable, Subscription, fromEvent } from "rxjs";
import { map } from "rxjs/operators";

sampleData = [
  { primary: "Hong Kong", disabled: true,dragging: false, description:"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. " },
  { primary: "Stockholm", disabled: true,dragging: false, description:"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. " },
  { primary: "São Paulo", disabled: true,dragging: false, description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. "},
  { primary: "Saint Petersburg",disabled: true,dragging: false, description:"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. "  },
];

selectedItem: any = { };
activeItem: number = 3;
dragStatus: string = " ";
isKeydownOperation: boolean = false;
keydownObservable$!: Observable<Event>;
keydownSubscription$!: Subscription;
clickObservable$!: Observable<Event>;
clickSubscription$!: Subscription;
listSubscription!: Subscription;
@ViewChildren("listItem") listItems!:QueryList<ApListItemComponent> | any;


handleListItemClick(event: any, item: any, index: any) {
  this.selectedItem = item;
}

keydownItemHandel(event: any, item: any, currentIndex: any) {
  let newIndex = currentIndex;
  let keyCode = event.keyCode;
  switch(keyCode) {
    case 40: // down
    case 39: // right
        if(item.disabled) return;
        event.preventDefault();
        newIndex += 1;
        if (newIndex === this.listItems.length) {
          newIndex = 0;
        }
        this.dragStatus = `${item.primary} selected, current position ${newIndex + 1} of ${this.listItems.length}`;
        break;
    case 38: // up
    case 37: // left
        if(item.disabled) return;
        event.preventDefault();
        // item.focus();
        newIndex -= 1;
        if (newIndex === -1) {
          newIndex = this.listItems.length - 1;
        }
        this.dragStatus = `${item.primary} selected, current position ${newIndex + 1} of ${this.listItems.length}`;
        break;
    case 13://enter
    case 32: // space
        event.preventDefault();
        if(item.disabled) {
          this.selectedItem = item;
          this.sampleData.forEach( o => {
            if(o.primary === item.primary) {
              o.disabled = false;
              o.dragging = true;
              setTimeout(()=> {
                this.dragStatus = `${item.primary} grabbed, use arrow keys to rearrange.`;
              },300)
            }else {
              o.disabled = true;
              o.dragging = false;
            }
          })
        }else {
          this.selectedItem = {};
          this.sampleData.forEach( o => {
            if(o.primary === item.primary) {
              o.disabled = true;
              o.dragging = false;
              setTimeout(()=> {
                this.dragStatus = `${item.primary} dropped`;
              },300)
            }
          })
        }
        break;
  }

  moveItemInArray(this.sampleData, currentIndex, newIndex);
  this.activeItem = newIndex;
  this.setFocus();
}

drop(event: CdkDragDrop<[]>) {
  moveItemInArray(this.sampleData, event.previousIndex, event.currentIndex);
  this.activeItem = event.currentIndex;
  this.selectedItem = this.sampleData[this.activeItem];
  this.setFocus();
}

ngOnDestroy() {
  this.listSubscription.unsubscribe();
  this.keydownSubscription$.unsubscribe();
  this.clickSubscription$.unsubscribe();
}

ngAfterViewInit() {
  this.listSubscription = this.listItems.changes.subscribe(() => {
    this.setFocus();
  });
  this.keydownObservable$ = fromEvent(window, 'keyup');
  this.keydownSubscription$ = this.keydownObservable$.subscribe((event: any) => {
    this.isKeydownOperation = true;
  });
  this.clickObservable$ = fromEvent(window, 'click');
  this.clickSubscription$ = this.clickObservable$.subscribe((event: any) => {
    this.isKeydownOperation = false;
  });
}

setFocus() {
  this.listItems._results[this.activeItem].elementRef?.nativeElement.focus();
}
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### selection - checkboxList


**Example #7** | **Variation**: selection | **Modifier**: checkboxList | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" [style]="'width:19.6875rem'" aria-multiselectable="true">
    <ap-list-item *ngFor="let item of sampleData; let i = index;" (click)="handleSelectionClick(i)"
      (keyup.enter)="handleSelectionClick(i)" [attr.role]="'checkbox'" [attr.aria-checked]="item.checked" aria-live="off"
      [attr.aria-label]="getAriaLabelWithCheckbox(item)">
      <ap-list-item-meta-avatar>
        <ap-checkbox [tabindex]="-1" (onChange)="onCheckboxSelected($event)" [(ngModel)]="item.checked">{{ item.primary }} </ap-checkbox>
      </ap-list-item-meta-avatar>
      <ap-list-item-extra>
        <span>{{ item.secondary }}</span>
      </ap-list-item-extra>
    </ap-list-item>
  </ap-list>
</div>
```

#### TypeScript

```typescript
sampleData = [
  { primary: "Hong Kong", secondary: "Hong Kong", checked:false },
  { primary: "Stockholm", secondary: "Stockholm", checked:false },
  { primary: "São Paulo", secondary: "São Paulo", checked:false },
  { primary: "Saint Petersburg", secondary: "Saint Petersburg", checked:false }
];

handleSelectionClick(index:number){
  this.sampleData[index].checked = !this.sampleData[index].checked;
}

onCheckboxSelected(event:any){
  if (event)event.originEvent.stopPropagation();
}

getAriaLabelWithCheckbox(item: any) {
  let label = item.primary + "," + item.secondary;
  return item.checked ? label : label + " "; // add one space to activate the aria-live to announce the changed status
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

 .list-container {
   display: flex;

   .ap-list-item {
     padding-left: $spacing-3;
   }
 }
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### selection - radioButtonList


**Example #8** | **Variation**: selection | **Modifier**: radioButtonList | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
import { RadioModule } from "@appkit4/angular-components/radio";
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" [style]="'width:19.6875rem'">
    <ap-list-item *ngFor="let item of sampleData; let i = index;" (click)="handleSelectionClick(i)"
      (keyup.enter)="handleSelectionClick(i)" [attr.role]="'radio'" aria-live="off"
      [attr.aria-checked]="locationVal===item.primary"  [attr.aria-label]="getAriaLabelWithRadio(item)">
      <ap-list-item-meta-avatar>
        <ap-radio [tabindex]="-1" [(ngModel)]="locationVal" [value]="item.primary" label="item.primary">{{ item.primary }}</ap-radio>
      </ap-list-item-meta-avatar>
      <ap-list-item-extra>
        <span>{{ item.secondary }}</span>
      </ap-list-item-extra>
    </ap-list-item>
  </ap-list>
</div>
```

#### TypeScript

```typescript
locationVal = "São Paulo";
sampleData = [
  { primary: "Hong Kong", secondary: "Hong Kong" },
  { primary: "Stockholm", secondary: "Stockholm" },
  { primary: "São Paulo", secondary: "São Paulo" },
  { primary: "Saint Petersburg", secondary: "Saint Petersburg" }
];

handleSelectionClick(index:number){
  this.locationVal = this.sampleData[index].primary;
}

getAriaLabelWithRadio(item: any) {
  let label = item.primary + "," + item.secondary;
  return this.locationVal===item.primary ? label : label + " "; // add one space to activate the aria-live to announce the changed status
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

 .list-container {
   display: flex;

   .ap-list-item {
     padding-left: $spacing-3;
   }
 }
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### selection - checkboxListWithBadge


**Example #9** | **Variation**: selection | **Modifier**: checkboxListWithBadge | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
import { CheckboxModule } from "@appkit4/angular-components/checkbox";
import { BadgeModule } from "@appkit4/angular-components/badge";
import { FormsModule } from '@angular/forms';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" [style]="'width:19.6875rem'" aria-multiselectable="true">
    <ap-list-item *ngFor="let item of sampleData; let i = index;" (click)="handleSelectionClick(i)"
      (keyup.enter)="handleSelectionClick(i)" [attr.role]="'checkbox'" [attr.aria-checked]="item.checked" aria-live="off"
      [attr.aria-label]="getAriaLabelWithCheckboxAndBadge(item)">
      <ap-list-item-meta-avatar>
        <ap-checkbox [tabindex]="-1" (onChange)="onCheckboxSelected($event)" [(ngModel)]="item.checked">{{ item.primary }}</ap-checkbox>
      </ap-list-item-meta-avatar>
      <ap-list-item-extra>
        <ap-badge value="{{item.value}}" type="primary-outlined" [attr.alt]="item.value" [attr.aria-label]="item.value"></ap-badge>
      </ap-list-item-extra>
    </ap-list-item>
  </ap-list>
</div>
```

#### TypeScript

```typescript
sampleData = [
  { primary: "Myrtle Campbell", value: 45, checked: false  },
  { primary: "Franklin Tate", value: 335 , checked: false },
  { primary: "Madge Wells", value: 25, checked: false  },
  { primary: "Randy Hill", value: 2, checked: false  },
];
handleSelectionClick(index:number){
  this.sampleData[index].checked = !this.sampleData[index].checked;
}

onCheckboxSelected(event:any){
  if (event)event.originEvent.stopPropagation();
}

getAriaLabelWithCheckboxAndBadge(item: any) {
  let label = item.primary + ", " + item.value;
  return item.checked ? label : label + " "; // add one space to activate the aria-live to announce the changed status
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

 .list-container {
   display: flex;

   .ap-list-item {
     padding-left: $spacing-3;
   }
 }
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### avatar - description


**Example #10** | **Variation**: avatar | **Modifier**: description | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" [style]="'width:19.6875rem'">
    <ap-list-item *ngFor="let item of sampleData" [attr.role]="'option'" (click)="handleListItemClick(item)"
    (keyup.enter)="handleListItemClick(item)" [attr.aria-selected]="item.primary===selectedItem.primary"
    [class.selected]="item.primary===selectedItem.primary">
      <ap-list-item-meta>
        <ap-list-item-meta-avatar>
          <div class="ap-list-avatar-container">
            <ap-avatar [borderWidth]="'0'" [diameter]="'40'" [name]="item.avatar" [attr.role]="item.avatar"
              [backgroundColor]="item.backgroundColor" [fontColor]="item.fontColor" [disabled]="true" [attr.aria-hidden]="false" [attr.aria-label]="item.avatar + ', '">
            </ap-avatar>
          </div>
        </ap-list-item-meta-avatar>
        <ap-list-item-meta-title>
          <span class="list-title-avatar">{{ item.primary }}</span>
        </ap-list-item-meta-title>
        <ap-list-item-meta-description>
          <span>{{ item.description }}</span>
        </ap-list-item-meta-description>
      </ap-list-item-meta>
    </ap-list-item>
  </ap-list>
</div>
```

#### TypeScript

```typescript
sampleData = [
  {
    primary: "Myrtle Campbell",
    avatar: "MC",
    description: "Hong Kong",
    backgroundColor: "#ff2d55",
    fontColor: "#ffffff"
  },
  {
    primary: "Franklin Tate",
    avatar: "FT",
    description: "Stockholm",
    backgroundColor: "#007aff",
    fontColor: "#ffffff"
  },
  {
    primary: "Madge Wells",
    avatar: "MW",
    description: "São Paulo",
    backgroundColor: "#34c759",
    fontColor: "#ffffff"
  },
  {
    primary: "Randy Hill",
    avatar: "RH",
    description: "Saint Petersburg",
    backgroundColor: "#ff9500",
    fontColor: "#ffffff"
  }
];
selectedItem: any = {};
handleListItemClick(item: any) {
  this.selectedItem = item;
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

 .list-container {
   display: flex;

   .ap-list-item {
     padding: $spacing-3;

     .ap-list-avatar-container{
       padding: $spacing-2;
     }

     .list-title-avatar {
       color: $color-text-body
     }
   }
 }
```

<!-- /EXAMPLE:10 -->

<!-- EXAMPLE:11 -->
### avatar - noDescription


**Example #11** | **Variation**: avatar | **Modifier**: noDescription | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
import { AvatarModule } from '@appkit4/angular-components/avatar';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" size="small" [style]="'width:19.6875rem'">
    <ap-list-item *ngFor="let item of sampleData" [attr.role]="'option'" (click)="handleListItemClick(item)"
      (keyup.enter)="handleListItemClick(item)" [attr.aria-selected]="item.primary===selectedItem.primary"
      [class.selected]="item.primary===selectedItem.primary" [attr.aria-label]="item.avatar + ',' + item.primary">
      <ap-list-item-meta [attr.aria-hidden]="true">
        <ap-list-item-meta-avatar>
          <div class="ap-list-avatar-container">
            <ap-avatar [borderWidth]="'0'" [diameter]="'40'" [name]="item.avatar" [backgroundColor]="item.backgroundColor" [fontColor]="item.fontColor" [disabled]="true"></ap-avatar>
          </div>
        </ap-list-item-meta-avatar>
        <ap-list-item-meta-title>
          <span class="list-title-avatar">{{ item.primary }}</span>
        </ap-list-item-meta-title>
      </ap-list-item-meta>
    </ap-list-item>
  </ap-list>
</div>
```

#### TypeScript

```typescript
sampleData = [
  { primary: "Myrtle Campbell", avatar: "MC", backgroundColor: "#ff2d55", fontColor: "#ffffff"  },
  { primary: "Franklin Tate", avatar: "FT", backgroundColor: "#007aff", fontColor: "#ffffff"  },
  { primary: "Madge Wells", avatar: "MW", backgroundColor: "#34c759", fontColor: "#ffffff"  },
  { primary: "Randy Hill", avatar: "RH", backgroundColor: "#ff9500", fontColor: "#ffffff"  }
];

selectedItem: any = {};

handleListItemClick(item: any) {
  this.selectedItem = item;
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

 .list-container {
   display: flex;

   .ap-list-item {
     padding: 0;

     .ap-list-avatar-container{
       padding: $spacing-2;
     }
   }

   .list-title-avatar {
     color: $color-text-body
   }
 }
```

<!-- /EXAMPLE:11 -->

<!-- EXAMPLE:12 -->
### complex


**Example #12** | **Variation**: complex | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { ListModule } from '@appkit4/angular-components/list';
```

#### HTML Template

```html
<div class='list-container'>
  <ap-list [bordered]="true" [style]="'width:658px'">
    <ap-list-item *ngFor="let item of sampleData" [attr.role]="'option'" (click)="handleListItemClick(item)"
      (keyup.enter)="handleListItemClick(item)" [attr.aria-selected]="item.primary===selectedItem.primary"
      [class.selected]="item.primary===selectedItem.primary">
      <ap-list-item-meta>
        <ap-list-item-meta-title>{{ item.primary }}</ap-list-item-meta-title>
        <ap-list-item-meta-description>
          <span class='secondary-text'>{{ item.description }}</span>
        </ap-list-item-meta-description>
      </ap-list-item-meta>
    </ap-list-item>
  </ap-list>
</div>
```

#### TypeScript

```typescript
sampleData = [
  {
    primary: "Myrtle Campbell",
    description:
    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip."
  },
  {
    primary: "Franklin Tate",
    description:
    "Consectetur adipiscing elit, sedos eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip."
  },
  {
    primary: "Madge Wells",
    description:
    "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sedos eiusmod tempor incididunt."
  },
  {
    primary: "Randy Hill",
    description:
    "Ipsum dolor sit amet, consectetur adipiscing elit, sedos eiusmod tempor incididunt."
  }
];
selectedItem: any = {};
handleListItemClick(item: any) {
  this.selectedItem = item;
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

 .list-container {
   display: flex;

   .ap-list-item {
     padding: $spacing-3 $spacing-4;
   }
 }
```

<!-- /EXAMPLE:12 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### ap-list

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| bordered | boolean | Type of the button. | false | 4.0.0 |
| role | string | The value of the HTML attribute role. | listbox | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| dragStatus | string | The draggable list item screen reader read value. | '' | 4.8.0 |

### ap-list-item

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| noFlex | boolean | Whether it is the flex layout rendering. | false | 4.0.0 |
| selectedItem | object | The selected list item. | {} | 4.0.0 |

### slot components

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| ap-list-item-action | Component | Action component for the list items. | - | 4.0.0 |
| ap-list-item-extra | Component | Extra content for the list items. | - | 4.0.0 |
| ap-list-item-meta | Component | Meta content of list item. | - | 4.0.0 |
| ap-list-item-meta-title | Component | Title component for the list items meta part. | - | 4.0.0 |
| ap-list-item-meta-description | Component | Description component for the list items meta part. | - | 4.0.0 |
| ap-list-item-meta-avatar | Component | Avatar component for the list items meta part. | - | 4.0.0 |
| ap-list-item-divider | Component | The separator in front of list item. | - | 4.6.1 |
| ap-list-item-draggable | Component | The drag and drop list item. | - | 4.8.0 |


<!-- /SECTION:properties -->