---
component: search
framework: angular
---

# Search Component

## Overview

AppKit Search component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Use Search component:

- To allow users to find something specific or explore what's available via relevant keywords.
- To help users find relevant content in large amounts of content quickly.

### Anatomy

1. **Search input:** Allows users to enter their search query.

2. **Magnifying glass icon:** This icon contributes to the user's familiarity and expectations about the search component.

3. **Enter button:** Initiates the search.

4. **Clear button:** Allows users to clear the search input field.

### Variants

#### Primary Search:

This is the main search feature of a website or application. It is typically placed in the header or navigation bar and should be easily accessible to users.

#### Secondary Search:

This variant is used when there is a need for additional search functionality. It may be placed in a secondary navigation menu or sidebar.

#### Global Search:

This variant allows users to search across all content on a website or application. It may be accessed from any page or screen.

#### Recent Search:

This variant displays a list of the user's most recent searches. It can be used to provide quick access to frequently searched items.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161659-112579%26viewport%3D1087%252C-33945%252C0.37%26t%3DXMQuZFEU35ks6Hqt-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

#### How to use

- Consider incorporating a visible label to assist users in understanding the purpose of the search field. For instance, labeling a search field as "Search comments" can help users comprehend the scope of the search.
- A search component should be used whenever the user is presented with a long list of records to quickly locate a specific record.
- If the user can only search within a given section, make sure that the search is within close proximity of that section. For example, when searching within a table the search should be located within close proximity to that table.

#### How not to use

- Do not use a search component for a fixed list under 10 items.
- Do not use multiple search fields for the same collection of data in the same view.
- Do not have more than 1 search field on a page at a time.
- For a small amount of content, use Checkbox, Dropdown or List component instead of search.

### Behavior

- When the user enters a search query, the component should initiate the search automatically or when the user presses the search button.
- The search results should be displayed in a clear and easy-to-understand format.
- The Primary Search variant should provide feedback to the user if no search results are found.
- The clear button should clear the search input field and reset the search results.

### Accessibility

- If a page includes more than one search landmark, each search field should have a unique label.
- The role="search" attribute defines a search landmark.

<!-- /SECTION:usage -->
<!-- SECTION:examples -->
## Code Examples

Total examples: 10


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | primary | `section: "example:1"` |
| 2 | secondary | `section: "example:2"` |
| 3 | globalSearch | `section: "example:3"` |
| 4 | recentSearch | `section: "example:4"` |
| 5 | primary - disabled | `section: "example:5"` |
| 6 | secondary - disabled | `section: "example:6"` |
| 7 | globalSearch - disabled | `section: "example:7"` |
| 8 | recentSearch - disabled | `section: "example:8"` |
| 9 | globalSearch - category | `section: "example:9"` |
| 10 | globalSearch - category - disabled | `section: "example:10"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### primary


**Example #1** | **Variation**: primary | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { SearchModule} from '@appkit4/angular-components/search';
```

#### HTML Template

```html
<ap-search [placeholder]="placeholder" [searchType]="'primary'"  [disabled]="searchDisabled" (onSearch)="testSearch($event)"></ap-search>
```

#### TypeScript

```typescript
placeholder= "Search";
searchDisabled: boolean = false;

testSearch(event: {originEvent: any, queryStr: string}):void {
  console.log(event.queryStr); 
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### secondary


**Example #2** | **Variation**: secondary | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { SearchModule} from '@appkit4/angular-components/search';
```

#### HTML Template

```html
<ap-search [placeholder]="placeholder" [searchType]="'secondary'" [disabled]="searchDisabled"  (onSearch)="testSearch($event)"></ap-search>
```

#### TypeScript

```typescript
placeholder= "Search";
searchDisabled: boolean = false;

testSearch(event: {originEvent: any, queryStr: string}):void {
  console.log(event.queryStr); 
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### globalSearch


**Example #3** | **Variation**: globalSearch | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SearchModule} from '@appkit4/angular-components/search';  
```

#### HTML Template

```html
<ap-search #search [placeholder]="placeholder"
      [(ngModel)]="globalValue"
      [searchType]="'secondary'"
      [disabled]="searchDisabled"
      [filtered]="true"
      [searchStatus]="searchLabel"
      [list]="list"
      (onSearch)="onGlobalSearch($event)"
      (onClear)="onClear1($event)">

      <ng-container *ngFor="let item of list">
          <ap-dropdown-list-item #dropdownListItem  [item]="item"
              [highlightText]="searchText"
              (onSelectItem)="onGlobalClickItem($event)"
              (onMouseEnter)="onMouseEnterItem($event)"
              (onMouseLeave)="onMouseLeaveItem($event)">
              <span class="Appkit4-icon
                  icon-arrow-increase-small-outline"></span>
          </ap-dropdown-list-item>
      </ng-container>
  </ap-search>
  <!--Notes: The global search uses the search type as secondary. Users can define styles and behaviors according to their preferences. -->
```

#### TypeScript

```typescript
import { Renderer2, ViewChild } from '@angular/core';


    placeholder: string = 'Search';
    globalValue: string = '';
    searchDisabled: boolean = false;
    searchText: string= '';
    searchLabel: string= '';
    list: any[] = [
      { value: 'type1', label: 'Badge' },
      { value: 'type2', label: 'Dropdown with badge' },
      { value: 'type3', label: 'Modal with badge', disabled: 'true' }
    ]

    constructor(public render: Renderer2){}

    onGlobalSearch(event:any): void {
      this.searchText = event.queryStr;
      if (event.result && event.result.length > 0) {
        this.list = event.result;
        let length = this.list.length;
        for(let i =0; i< this.list.length; i++) {
            if(this.list[i].disabled) {
              length = length -1;
            }
        }
        this.searchLabel =  'There are '+length+' items available.';
      } else {
        this.list = [];
        this.searchLabel =  "Nothing matches your keywords";
      }
    }

    onClear1(event:any): void {
      this.list=[];
    }

    onGlobalClickItem(event:any): void {
      console.log(event);
    }

    onMouseEnterItem(event:any): void{
      this.render.addClass(event.originEvent.target, 'iconLink');
    }

    onMouseLeaveItem(event:any): void {
      this.render.removeClass(event.originEvent.target, 'iconLink')
    }
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### recentSearch


**Example #4** | **Variation**: recentSearch | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SearchModule} from '@appkit4/angular-components/search';  
```

#### HTML Template

```html
<ap-search #search [placeholder]="placeholder"
    [(ngModel)]="recentValue"
    [searchType]="'secondary'"
    [disabled]="searchDisabled" [filtered]="true"
    [list]="recentList"
    [searchStatus]="searchLabel"
    [isRecentSearch]="isRecentSearch"
    (onFocus)="onRecentFocus($event)"
    (onSearch)="onRecentSearch($event)"
    (onClear)="onClear2($event)">

    <ng-container *ngFor="let item of recentList">
        <ap-dropdown-list-item #dropdownListItem
            [item]="item"
            [highlightText]="searchText"
            [ariaLabel]="itemActiveDelete ? setAriaLabel(item): item.label"
            (onSelectItem)="onRecentClickItem($event)"
            (onMouseEnter)="onMouseEnterItem($event)"
            (onMouseLeave)="onMouseLeaveItem($event)">
            <span *ngIf="itemActiveArrow" class="Appkit4-icon icon-arrow-increase-small-outline"></span>
            <span *ngIf="itemActiveDelete" role="button" [attr.aria-label]="'clear recent history'"
                [attr.tabindex]="0" class="recent-delete-icon" (keydown.enter)="onIconKeydown($event,item)"
                (click)="onConfirmDelete($event,item)">
                <span  class="Appkit4-icon icon-close-outline"></span>
            </span>
        </ap-dropdown-list-item>
    </ng-container>
</ap-search>
<!--Notes: Recent search uses the search type as secondary. Users can define styles and behaviors according to their preferences. -->
```

#### TypeScript

```typescript
import { Renderer2, ViewChild } from '@angular/core';
import { SearchComponent } from '@appkit4/angular-components/search';


    @ViewChild('search') searchInstance!: SearchComponent;  
    placeholder: string = "Search";
    recentValue: string = "";
    recentList: any[]=  [
      { value: 'type1', label: 'Badge' },
      { value: 'type2', label: 'Install library' },
    ];
    searchHistoryList: any[] = [];
    isRecentSearch: boolean = true;
    itemActiveArrow: boolean = false
    itemActiveDelete: boolean = false;
    searchDisabled: boolean = false;
    searchText: string= '';
    searchLabel: string= '';

    constructor(public render: Renderer2){}

    setAriaLabel(item:any): any{
      const label = item.label? item.label : '';
      if(this.itemActiveDelete){
        return  `${label}, clear icon`  ;
      }
    }

    onRecentFocus(event:any): void {
      event.originEvent.preventDefault();
      event.originEvent.stopPropagation();
      let value: any = event.queryStr;
      if (!(value.length > 0)) {
        let hlist = JSON.parse(localStorage.getItem("history") || '{}');
        this.recentList = Object.values(hlist);
        if (this.recentList != null && this.recentList.length > 0) {
          this.searchInstance.showList = true;
          this.searchText = "";
          this.searchInstance.showBlankPage = false;
          this.itemActiveDelete = true;
        } else {
          this.searchInstance.showList = false;
          this.searchInstance.showBlankPage = false;
          this.itemActiveDelete = false;
        }
      } else {
        this.itemActiveDelete = false;
      }
    }

    onRecentSearch(event:any): void {
      this.itemActiveDelete = false;
      this.searchText = event.queryStr;
      if (event.result && event.result.length > 0) {
        this.recentList = event.result;
        let length = this.recentList.length;
        for(let i =0; i< this.recentList.length; i++) {
            if(this.recentList[i].disabled) {
              length = length -1;
            }
        }
        this.searchLabel =  'There are '+length+' items available.'
      } else {
        this.recentList = [];
        this.searchLabel =  "Nothing matches your keywords"
      }
    }

    onClear2(event:any): void {
      this.recentList=[];
    }

    onRecentClickItem(event:any): void {
      let option = event.selected;
      if(this.isRecentSearch) {
        let index = this.searchHistoryList.findIndex(
          (item) => item.label === option.label
        );
        if (!(index > -1)) {
          this.searchHistoryList.push({ value: "type1", label: option.label });
          localStorage.setItem(
            "history",
            JSON.stringify(this.searchHistoryList) || "{}"
          );
        }
      }
    }

    onMouseEnterItem(event:any): void{
      if(this.isRecentSearch) {
        if (this.itemActiveDelete) {
          this.itemActiveArrow = false;
          this.render.removeClass(event.originEvent.target, "iconLink");
        } else {
          this.itemActiveArrow = true;
          this.render.addClass(event.originEvent.target, "iconLink");
        }
      } else {
        this.itemActiveArrow = true;
        this.render.addClass(event.originEvent.target, 'iconLink');
      }
    }

    onMouseLeaveItem(event:any): void {
      this.itemActiveArrow = false;
      this.render.removeClass(event.originEvent.target, 'iconLink')
    }

    onIconKeydown(event: any, item: any): void{
      event.preventDefault();
      event.stopPropagation();
      this.onDeleteHistory(event, item);
    }

    onConfirmDelete(event: any, item: any): void {
      event.preventDefault();
      event.stopPropagation();  
      this.onDeleteHistory(event, item);
    }

    onDeleteHistory(event: any, item: any): void {
      this.searchInstance.inputViewChild.nativeElement.focus();
      let index = this.searchHistoryList.findIndex(
        (opt) => opt.label === item.label
      );
      this.searchHistoryList.splice(index, 1);
      this.recentList = this.searchHistoryList;
      if (this.recentList.length === 0) {
        this.searchInstance.showHistoryMsg = true;
        this.searchInstance.showBlankPage = true;
        this.searchInstance.showList = true;
        this.itemActiveDelete = true;
      }
      localStorage.setItem("history", JSON.stringify(this.searchHistoryList));
      this.searchLabel =  "No recent searches";
    }
```

<!-- /EXAMPLE:4 -->

<!-- EXAMPLE:5 -->
### primary - disabled


**Example #5** | **Variation**: primary | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { SearchModule} from '@appkit4/angular-components/search';
```

#### HTML Template

```html
<ap-search [placeholder]="placeholder" [searchType]="'primary'"  [disabled]="searchDisabled" (onSearch)="testSearch($event)"></ap-search>
```

#### TypeScript

```typescript
placeholder= "Search";
searchDisabled: boolean = true;

testSearch(event: {originEvent: any, queryStr: string}):void {
  console.log(event.queryStr); 
}
```

<!-- /EXAMPLE:5 -->

<!-- EXAMPLE:6 -->
### secondary - disabled


**Example #6** | **Variation**: secondary | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { SearchModule} from '@appkit4/angular-components/search';
```

#### HTML Template

```html
<ap-search [placeholder]="placeholder" [searchType]="'secondary'" [disabled]="searchDisabled"  (onSearch)="testSearch($event)"></ap-search>
```

#### TypeScript

```typescript
placeholder= "Search";
searchDisabled: boolean = true;

testSearch(event: {originEvent: any, queryStr: string}):void {
  console.log(event.queryStr); 
}
```

<!-- /EXAMPLE:6 -->

<!-- EXAMPLE:7 -->
### globalSearch - disabled


**Example #7** | **Variation**: globalSearch | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SearchModule} from '@appkit4/angular-components/search';  
```

#### HTML Template

```html
<ap-search #search [placeholder]="placeholder"
      [(ngModel)]="globalValue"
      [searchType]="'secondary'"
      [disabled]="searchDisabled"
      [filtered]="true"
      [searchStatus]="searchLabel"
      [list]="list"
      (onSearch)="onGlobalSearch($event)"
      (onClear)="onClear1($event)">

      <ng-container *ngFor="let item of list">
          <ap-dropdown-list-item #dropdownListItem  [item]="item"
              [highlightText]="searchText"
              (onSelectItem)="onGlobalClickItem($event)"
              (onMouseEnter)="onMouseEnterItem($event)"
              (onMouseLeave)="onMouseLeaveItem($event)">
              <span class="Appkit4-icon
                  icon-arrow-increase-small-outline"></span>
          </ap-dropdown-list-item>
      </ng-container>
  </ap-search>
  <!--Notes: The global search uses the search type as secondary. Users can define styles and behaviors according to their preferences. -->
```

#### TypeScript

```typescript
import { Renderer2, ViewChild } from '@angular/core';


    placeholder: string = 'Search';
    globalValue: string = '';
    searchDisabled: boolean = true;
    searchText: string= '';
    searchLabel: string= '';
    list: any[] = [
      { value: 'type1', label: 'Badge' },
      { value: 'type2', label: 'Dropdown with badge' },
      { value: 'type3', label: 'Modal with badge', disabled: 'true' }
    ]

    constructor(public render: Renderer2){}

    onGlobalSearch(event:any): void {
      this.searchText = event.queryStr;
      if (event.result && event.result.length > 0) {
        this.list = event.result;
        let length = this.list.length;
        for(let i =0; i< this.list.length; i++) {
            if(this.list[i].disabled) {
              length = length -1;
            }
        }
        this.searchLabel =  'There are '+length+' items available.';
      } else {
        this.list = [];
        this.searchLabel =  "Nothing matches your keywords";
      }
    }

    onClear1(event:any): void {
      this.list=[];
    }

    onGlobalClickItem(event:any): void {
      console.log(event);
    }

    onMouseEnterItem(event:any): void{
      this.render.addClass(event.originEvent.target, 'iconLink');
    }

    onMouseLeaveItem(event:any): void {
      this.render.removeClass(event.originEvent.target, 'iconLink')
    }
```

<!-- /EXAMPLE:7 -->

<!-- EXAMPLE:8 -->
### recentSearch - disabled


**Example #8** | **Variation**: recentSearch | **Modifier**: None | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SearchModule} from '@appkit4/angular-components/search';  
```

#### HTML Template

```html
<ap-search #search [placeholder]="placeholder"
    [(ngModel)]="recentValue"
    [searchType]="'secondary'"
    [disabled]="searchDisabled" [filtered]="true"
    [list]="recentList"
    [searchStatus]="searchLabel"
    [isRecentSearch]="isRecentSearch"
    (onFocus)="onRecentFocus($event)"
    (onSearch)="onRecentSearch($event)"
    (onClear)="onClear2($event)">

    <ng-container *ngFor="let item of recentList">
        <ap-dropdown-list-item #dropdownListItem
            [item]="item"
            [highlightText]="searchText"
            [ariaLabel]="itemActiveDelete ? setAriaLabel(item): item.label"
            (onSelectItem)="onRecentClickItem($event)"
            (onMouseEnter)="onMouseEnterItem($event)"
            (onMouseLeave)="onMouseLeaveItem($event)">
            <span *ngIf="itemActiveArrow" class="Appkit4-icon icon-arrow-increase-small-outline"></span>
            <span *ngIf="itemActiveDelete" role="button" [attr.aria-label]="'clear recent history'"
                [attr.tabindex]="0" class="recent-delete-icon" (keydown.enter)="onIconKeydown($event,item)"
                (click)="onConfirmDelete($event,item)">
                <span  class="Appkit4-icon icon-close-outline"></span>
            </span>
        </ap-dropdown-list-item>
    </ng-container>
</ap-search>
<!--Notes: Recent search uses the search type as secondary. Users can define styles and behaviors according to their preferences. -->
```

#### TypeScript

```typescript
import { Renderer2, ViewChild } from '@angular/core';
import { SearchComponent } from '@appkit4/angular-components/search';


    @ViewChild('search') searchInstance!: SearchComponent;  
    placeholder: string = "Search";
    recentValue: string = "";
    recentList: any[]=  [
      { value: 'type1', label: 'Badge' },
      { value: 'type2', label: 'Install library' },
    ];
    searchHistoryList: any[] = [];
    isRecentSearch: boolean = true;
    itemActiveArrow: boolean = false
    itemActiveDelete: boolean = false;
    searchDisabled: boolean = true;
    searchText: string= '';
    searchLabel: string= '';

    constructor(public render: Renderer2){}

    setAriaLabel(item:any): any{
      const label = item.label? item.label : '';
      if(this.itemActiveDelete){
        return  `${label}, clear icon`  ;
      }
    }

    onRecentFocus(event:any): void {
      event.originEvent.preventDefault();
      event.originEvent.stopPropagation();
      let value: any = event.queryStr;
      if (!(value.length > 0)) {
        let hlist = JSON.parse(localStorage.getItem("history") || '{}');
        this.recentList = Object.values(hlist);
        if (this.recentList != null && this.recentList.length > 0) {
          this.searchInstance.showList = true;
          this.searchText = "";
          this.searchInstance.showBlankPage = false;
          this.itemActiveDelete = true;
        } else {
          this.searchInstance.showList = false;
          this.searchInstance.showBlankPage = false;
          this.itemActiveDelete = false;
        }
      } else {
        this.itemActiveDelete = false;
      }
    }

    onRecentSearch(event:any): void {
      this.itemActiveDelete = false;
      this.searchText = event.queryStr;
      if (event.result && event.result.length > 0) {
        this.recentList = event.result;
        let length = this.recentList.length;
        for(let i =0; i< this.recentList.length; i++) {
            if(this.recentList[i].disabled) {
              length = length -1;
            }
        }
        this.searchLabel =  'There are '+length+' items available.'
      } else {
        this.recentList = [];
        this.searchLabel =  "Nothing matches your keywords"
      }
    }

    onClear2(event:any): void {
      this.recentList=[];
    }

    onRecentClickItem(event:any): void {
      let option = event.selected;
      if(this.isRecentSearch) {
        let index = this.searchHistoryList.findIndex(
          (item) => item.label === option.label
        );
        if (!(index > -1)) {
          this.searchHistoryList.push({ value: "type1", label: option.label });
          localStorage.setItem(
            "history",
            JSON.stringify(this.searchHistoryList) || "{}"
          );
        }
      }
    }

    onMouseEnterItem(event:any): void{
      if(this.isRecentSearch) {
        if (this.itemActiveDelete) {
          this.itemActiveArrow = false;
          this.render.removeClass(event.originEvent.target, "iconLink");
        } else {
          this.itemActiveArrow = true;
          this.render.addClass(event.originEvent.target, "iconLink");
        }
      } else {
        this.itemActiveArrow = true;
        this.render.addClass(event.originEvent.target, 'iconLink');
      }
    }

    onMouseLeaveItem(event:any): void {
      this.itemActiveArrow = false;
      this.render.removeClass(event.originEvent.target, 'iconLink')
    }

    onIconKeydown(event: any, item: any): void{
      event.preventDefault();
      event.stopPropagation();
      this.onDeleteHistory(event, item);
    }

    onConfirmDelete(event: any, item: any): void {
      event.preventDefault();
      event.stopPropagation();  
      this.onDeleteHistory(event, item);
    }

    onDeleteHistory(event: any, item: any): void {
      this.searchInstance.inputViewChild.nativeElement.focus();
      let index = this.searchHistoryList.findIndex(
        (opt) => opt.label === item.label
      );
      this.searchHistoryList.splice(index, 1);
      this.recentList = this.searchHistoryList;
      if (this.recentList.length === 0) {
        this.searchInstance.showHistoryMsg = true;
        this.searchInstance.showBlankPage = true;
        this.searchInstance.showList = true;
        this.itemActiveDelete = true;
      }
      localStorage.setItem("history", JSON.stringify(this.searchHistoryList));
      this.searchLabel =  "No recent searches";
    }
```

<!-- /EXAMPLE:8 -->

<!-- EXAMPLE:9 -->
### globalSearch - category


**Example #9** | **Variation**: globalSearch | **Modifier**: category | **State**: None

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SearchModule} from '@appkit4/angular-components/search';  
```

#### HTML Template

```html
<ap-search #search [placeholder]="placeholder"
      [(ngModel)]="globalValue"
      [searchType]="'secondary'"
      [disabled]="searchDisabled"
      [filtered]="true"
      [searchStatus]="searchLabel"
      [list]="list"
      (onSearch)="onGlobalSearch($event)"
      (onClear)="onClear1($event)">
      <ng-container *ngFor="let group of list">
          <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
          <ng-container *ngFor="let item of group.items">
              <ap-dropdown-list-item #dropdownListItem [item]="item" [highlightText]="searchText"
                  (onSelectItem)="onGlobalClickItem($event)"
                  (onMouseEnter)="onMouseEnterItem($event)"
                  (onMouseLeave)="onMouseLeaveItem($event)">
                  <span class="Appkit4-icon icon-arrow-increase-small-outline"></span>
              </ap-dropdown-list-item>
          </ng-container>
      </ng-container>
  </ap-search>
  <!--Notes: The global search uses the search type as secondary. Users can define styles and behaviors according to their preferences. -->
```

#### TypeScript

```typescript
import { Renderer2, ViewChild } from '@angular/core';


    placeholder: string = 'Search';
    globalValue: string = '';
    searchDisabled: boolean = false;
    searchText: string= '';
    searchLabel: string= '';
    list: any[] = [
      { label: 'Sales', type: 'group', items: [
          { value: 'type1', label: 'Jeff', groupName: 'Sales' },
          { value: 'type2', label: 'Olivia', groupName: 'Sales' },
        ] },
      { label: 'PR', type: 'group', items: [
          { value: 'type3', label: 'Jenifer', groupName: 'PR' },
        ]}
    ]

    constructor(public render: Renderer2){}

    onGlobalSearch(event:any): void {
      this.searchText = event.queryStr;
      if (event.result && event.result.length > 0) {
        this.list = event.result;
        let length = this.list.length;
        for(let i =0; i< this.list.length; i++) {
            if(this.list[i].disabled) {
              length = length -1;
            }
        }
        this.searchLabel =  'There are '+length+' items available.';
      } else {
        this.list = [];
        this.searchLabel =  "Nothing matches your keywords";
      }
    }

    onClear1(event:any): void {
      this.list=[];
    }

    onGlobalClickItem(event:any): void {
      console.log(event);
    }

    onMouseEnterItem(event:any): void{
      this.render.addClass(event.originEvent.target, 'iconLink');
    }

    onMouseLeaveItem(event:any): void {
      this.render.removeClass(event.originEvent.target, 'iconLink')
    }
```

<!-- /EXAMPLE:9 -->

<!-- EXAMPLE:10 -->
### globalSearch - category - disabled


**Example #10** | **Variation**: globalSearch | **Modifier**: category | **State**: disabled

#### Module Import

```typescript
import { FormsModule } from '@angular/forms';
import { SearchModule} from '@appkit4/angular-components/search';  
```

#### HTML Template

```html
<ap-search #search [placeholder]="placeholder"
      [(ngModel)]="globalValue"
      [searchType]="'secondary'"
      [disabled]="searchDisabled"
      [filtered]="true"
      [searchStatus]="searchLabel"
      [list]="list"
      (onSearch)="onGlobalSearch($event)"
      (onClear)="onClear1($event)">
      <ng-container *ngFor="let group of list">
          <ap-dropdown-list-item [item]="group"></ap-dropdown-list-item>
          <ng-container *ngFor="let item of group.items">
              <ap-dropdown-list-item #dropdownListItem [item]="item" [highlightText]="searchText"
                  (onSelectItem)="onGlobalClickItem($event)"
                  (onMouseEnter)="onMouseEnterItem($event)"
                  (onMouseLeave)="onMouseLeaveItem($event)">
                  <span class="Appkit4-icon icon-arrow-increase-small-outline"></span>
              </ap-dropdown-list-item>
          </ng-container>
      </ng-container>
  </ap-search>
  <!--Notes: The global search uses the search type as secondary. Users can define styles and behaviors according to their preferences. -->
```

#### TypeScript

```typescript
import { Renderer2, ViewChild } from '@angular/core';


    placeholder: string = 'Search';
    globalValue: string = '';
    searchDisabled: boolean = true;
    searchText: string= '';
    searchLabel: string= '';
    list: any[] = [
      { label: 'Sales', type: 'group', items: [
          { value: 'type1', label: 'Jeff', groupName: 'Sales' },
          { value: 'type2', label: 'Olivia', groupName: 'Sales' },
        ] },
      { label: 'PR', type: 'group', items: [
          { value: 'type3', label: 'Jenifer', groupName: 'PR' },
        ]}
    ]

    constructor(public render: Renderer2){}

    onGlobalSearch(event:any): void {
      this.searchText = event.queryStr;
      if (event.result && event.result.length > 0) {
        this.list = event.result;
        let length = this.list.length;
        for(let i =0; i< this.list.length; i++) {
            if(this.list[i].disabled) {
              length = length -1;
            }
        }
        this.searchLabel =  'There are '+length+' items available.';
      } else {
        this.list = [];
        this.searchLabel =  "Nothing matches your keywords";
      }
    }

    onClear1(event:any): void {
      this.list=[];
    }

    onGlobalClickItem(event:any): void {
      console.log(event);
    }

    onMouseEnterItem(event:any): void{
      this.render.addClass(event.originEvent.target, 'iconLink');
    }

    onMouseLeaveItem(event:any): void {
      this.render.removeClass(event.originEvent.target, 'iconLink')
    }
```

<!-- /EXAMPLE:10 -->

<!-- /SECTION:examples -->
<!-- SECTION:properties -->
## Properties

### Properties

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| searchType | string: 'primary'\|'secondary' | Search box type, must be declared when use the search box | 'primary' | 4.0.0 |
| placeholder | string | Default text to display when there is no value in the search input box. | - | 4.0.0 |
| disabled | boolean | If it is true, it specifies that search box should be disabled. | false | 4.0.0 |
| ariaLabel | string | The ariaLabel of search input box. | '' | 4.7.0 |
| filtered | boolean | When specified, filter displays with the label property. | false | 4.0.0 |
| isRecentSearch | boolean | If it is true, it specifies that the search box will keep the search history. | false | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| ngModel | string | Current search value, two-way binding is supported.FormsModule needs to be imported from @angular/forms. | - | 4.0.0 |
| list | array | The list data of the search component where the label property is required. | - | 4.0.0 |
| noResultMsg | string | Text to display when searching does not return any results. | 'Nothing matches your keywords' | 4.0.0 |
| noHistoryMsg | string | Text to display when there is no recent search histoy. | 'No recent searches' | 4.0.0 |
| searchStatus | string | Content of live region to announce current search status. | - | 4.0.0 |
| onSearch | EventEmitter<any> | Callback to invoke when searching a value. | - | 4.0.0 |
| onFocus | EventEmitter<any> | Callback to invoke when the search box is focused. | - | 4.0.0 |
| onBlur | EventEmitter<any> | Callback to invoke when the search box is blurred. | - | 4.0.0 |
| onClear | EventEmitter<any> | Callback to invoke when search clears the value. | - | 4.0.0 |
| onKeydown | EventEmitter<any> | Callback to invoke when pressing a key. | - | 4.0.0 |


<!-- /SECTION:properties -->