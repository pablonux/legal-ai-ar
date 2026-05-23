---
component: accordion
framework: angular
---

# Accordion Component

## Overview

AppKit Accordion component for Angular framework.

<!-- SECTION:usage -->
## Usage

### Usage

Accordions enable the user to toggle the display of a section of content. Use accordions to:
- Reduce chunk information and reduce clutter in the UI whenever possible. Example: FAQs, Forms.
- Navigate within screens and pages to help users navigate within interfaces.
- Organize related information.

### Anatomy

- **Label:** Text that is in the upper section of the accordion. The wording of the label must be clear.
- **Icon:** We use a chevron icon to indicate the status of the accordion. These can be: default and expanded.
- **Pane:** The pane is the part of the accordion that is hidden when the accordion is closed or in default. This part is the one that expands to contain the hidden text.

### Variants

Usage of one or the other depends on the user and the product.

#### Single expand

Only one item can be opened at once.

#### Multi expand

Many items can open at once.

### Figma

[View in Figma](https://www.figma.com/embed?embed_host=share&url=https%3A%2f%2fwww.figma.com%2fproto%2fBmrAOae85hXrSMa1rpqGkQ%2fAK4--(Blue%252C-Orange%252C-Teal%252C-Pink)%3Fpage-id%3D161642%253A106436%26type%3Ddesign%26node-id%3D161642-106470%26viewport%3D63%252C434%252C0.53%26t%3DFEszbR06xfnuUUp6-1%26scaling%3Dmin-zoom%26mode%3Ddesign)

### Best Practices

Accordions should be placed in a consistent and prominent location throughout the screen. We recommend placing accordions in a container.(Needs visual)Each accordion item should be placed between 4px and 8px apart from top to bottom.

#### How to use

- Accordions should be used to shorten pages with typically more extended lengthy displays.
- Can be used to contain links or content.
- Use short but descriptive labels that best describe the content within the accordion.

#### How not to use

- Do not use an accordion to contain important content that shouldn't be hidden to the user.
- Do not use accordions for big amount of items (more than 10? )with long pane content as that may result with unexpected behavior in viewport shifting. That may be solved by combination with tabs, including search or making sub-sections of categories.

### Behavior

The width of the accordion container must adapt to the length of the line. We recommend a maximum width of 120 characters per line or switch to a multi-column layout. Do not scale without adjusting other areas of the screen, such as the length of the text, as it can result in reading difficulties. 

We recommend default to close items vs open. The idea of using an accordion is to chunk information as much as possible SO it is recommended unless you use an accordion as a navigation tool, rather default it to closed. If information must be displayed, we recommend having it outside the accordion.

### Accessibility

- Each accordion header needs a unique title, this provides context for assistive technology users without the need to expand all sections.
- Use aria-expanded on trigger to express the default state.
- Use unique ids. Each aria-controls="id" associates the control to the appropriate region by referencing the controlled element's ID.

<!-- /SECTION:usage -->

<!-- SECTION:examples -->
## Code Examples

Total examples: 4


<!-- EXAMPLE-INDEX -->
### Example Index

| # | Variation | Quick Access |
|---|-----------|-------------|
| 1 | Default | `section: "example:1"` |
| 2 | expanded | `section: "example:2"` |
| 3 | multiExpand-expanded | `section: "example:3"` |
| 4 | multiExpand | `section: "example:4"` |

*Use `section: "examples:N-M"` for ranges, e.g., `"examples:1-5"`*
<!-- /EXAMPLE-INDEX -->

<!-- EXAMPLE:1 -->
### Default


**Example #1** | **Variation**: None | **Modifier**: None | **State**: None

#### Module Import

```typescript
import { AccordionModule } from '@appkit4/angular-components/accordion';
```

#### HTML Template

```html
<ap-accordion-group [multiple]="false" (onClick)="onClickAccordion($event)">
    <ap-accordion [title]="'Hongkong'" [expanded]="false">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion [title]="'Stockholm'">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion [title]="'São Paulo'">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion>
        <!-- Use 'accodionTitle' template to customize the title of accordion -->
        <ng-template ngTemplate="accordionTitle">
            <span>Saint Petersburg</span>
        </ng-template>
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
</ap-accordion-group>
```

#### TypeScript

```typescript
onClickAccordion(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:1 -->

<!-- EXAMPLE:2 -->
### expanded


**Example #2** | **Variation**: None | **Modifier**: expanded | **State**: None

#### Module Import

```typescript
import { AccordionModule } from '@appkit4/angular-components/accordion';
```

#### HTML Template

```html
<ap-accordion-group [multiple]="false" (onClick)="onClickAccordion($event)">
    <ap-accordion [title]="'Hongkong'" [expanded]="true">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion [title]="'Stockholm'">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion [title]="'São Paulo'">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion>
        <!-- Use 'accodionTitle' template to customize the title of accordion -->
        <ng-template ngTemplate="accordionTitle">
            <span>Saint Petersburg</span>
        </ng-template>
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
</ap-accordion-group>
```

#### TypeScript

```typescript
onClickAccordion(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:2 -->

<!-- EXAMPLE:3 -->
### multiExpand-expanded


**Example #3** | **Variation**: None | **Modifier**: multiExpand-expanded | **State**: None

#### Module Import

```typescript
import { AccordionModule } from '@appkit4/angular-components/accordion';
```

#### HTML Template

```html
<ap-accordion-group [multiple]="true" (onClick)="onClickAccordion($event)">
    <ap-accordion [title]="'Hongkong'" [expanded]="true">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion [title]="'Stockholm'">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion [title]="'São Paulo'">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion>
        <!-- Use 'accodionTitle' template to customize the title of accordion -->
        <ng-template ngTemplate="accordionTitle">
            <span>Saint Petersburg</span>
        </ng-template>
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
</ap-accordion-group>
```

#### TypeScript

```typescript
onClickAccordion(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:3 -->

<!-- EXAMPLE:4 -->
### multiExpand


**Example #4** | **Variation**: None | **Modifier**: multiExpand | **State**: None

#### Module Import

```typescript
import { AccordionModule } from '@appkit4/angular-components/accordion';
```

#### HTML Template

```html
<ap-accordion-group [multiple]="true" (onClick)="onClickAccordion($event)">
    <ap-accordion [title]="'Hongkong'" [expanded]="false">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion [title]="'Stockholm'">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion [title]="'São Paulo'">
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
    <ap-accordion>
        <!-- Use 'accodionTitle' template to customize the title of accordion -->
        <ng-template ngTemplate="accordionTitle">
            <span>Saint Petersburg</span>
        </ng-template>
        <span class="ap-accordion-text">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
            laboris nisi ut aliquip ex ea commodo consequat.</span>
    </ap-accordion>
</ap-accordion-group>
```

#### TypeScript

```typescript
onClickAccordion(event: any) {
  console.log(event);
}
```

<!-- /EXAMPLE:4 -->

<!-- /SECTION:examples -->

<!-- SECTION:properties -->
## Properties

### ap-accordion-group

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| accordionId | string | The id string of accordions, each accordion content element will have the id attribute with its index. | 'accordion' | 4.0.0 |
| multiple | boolean | Whether multiple accordions can be expanded at the same time. | false | 4.0.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| onClick | EventEmitter<{ accordionList: QueryList<AccordionComponent>, clickedAccordion: AccordionComponent }> | Callback when the accordion item is clicked. Event properties: • accordionList: QueryList of each AccordionComponent. • clickedAccordion: The AccordionComponent component which is clicked. | - | 4.0.0 |
| expandAll | (accordionIndexArr?: Array<number>) => void | Expand all the accordions if not passing the 'accordionIndexArr' or passing an empty array of 'accordionIndexArr'. Expand the accordions with corresponding index in the 'accordionIndexArr', e.g., \[0, 1, 2, 3\]. | - | 4.6.0 |
| collapseAll | (accordionIndexArr?: Array<number>) => void | Collapse all the accordions if not passing the 'accordionIndexArr' or passing an empty array of 'accordionIndexArr'. Collapse the accordions with corresponding index in the 'accordionIndexArr', e.g., \[0, 1, 2, 3\]. | - | 4.6.0 |

### ap-accordion

| Name | Type | Description | Default | Version |
|------|------|-------------|---------|---------|
| id | string | The id of accordion item. | '' | 4.0.0 |
| expanded | boolean | Expand the accordion or not by default. | false | 4.0.0 |
| title | string | The text of the accordion title. | '' | 4.0.0 |
| exclElementToToggle | string | The CSS selector string, click on the elements with this selector string will not trigger the toggle of accordion. | '' | 4.0.0 |
| inclElementToToggle | string | The CSS selector string, click on the elements with this selector string will trigger the toggle of accordion. | '' | 4.1.0 |
| toggleFromHeaderIconOnly | boolean | Whether only click on the accordion header icon will trigger the toggle of accordion. If it is false, it will be able to trigger the toggle when clicking on the header of the accordion. | false | 4.1.0 |
| onClick | EventEmitter<number> | Callback when the accordion item is clicked. | - | 4.3.0 |
| style | string | The inline style of the component. | '' | 4.0.0 |
| styleClass | string | The style class names of the component. | '' | 4.0.0 |
| stopPropagationOfBodyToggle | boolean | StopPropagation when toggle the accordion by clicking the accordion body. | true | 4.13.0 |

<!-- /SECTION:properties -->
