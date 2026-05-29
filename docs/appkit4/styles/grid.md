---
styles: grid
framework: angular
---

# Grid system

Our 8px grid system, which starts with a 12-column flexible grid layout, allows for consistent and scalable spacing, making scaling for various devices easy and consistent, therefore helping designers and developers work much faster on projects.

Use our powerful mobile-first twelve column grid to quickly build multi-device responsive layouts.

## Breakpoints

Breakpoints are the point at which the content and layout will adapt based on the viewport width. We define breakpoints in ranges including Large Desktop, Small Desktop, Tablet, and Mobile. Three things change based on a breakpoint: Navigation, Margins, and Gutters.

| Sizes | Margin | Gutter | No. of columns |
|-------|--------|--------|----------------|
| Large desktop 1440px + | 40px | 32px | 12 |
| Small desktop 1240px - 1439px | 32px | 24px | 12 |
| Tablet 600px - 1239px | 24px | 24px | 8 |
| Mobile 0px - 599px | 24px | 16px | 4 |

### Main content max width

On larger displays, the body content should not exceed 1160px. Gutters should be introduced on either side of the main body to fill this negative space.

## Designers

### Getting started

In Appkit, we have resources that spare the hassle of setting up the grid and starting page designs from scratch. Check out our [Page templates](https://appkit.pwc.com/appkit4/content/pattern-list) pattern, providing designers with ready-made layout options, complete with pre-set grids and main page components. *This pattern is available as a Figma asset.*

When aligning your outermost containers, strive to accommodate the specified column widths while considering the gutter widths.

However, you need not concern yourself with aligning the elements within the container.

### Alignment and placement guidelines

Find below guidance on how to align and place elements within the grid system. The grid system is defined by breakpoints, columns, margins, and gutters, ensuring a responsive and consistent layout across different devices.

> **Note:** Note that the header and sidebar navigation live outside of the grid.

#### Header and Sidebar Navigation

- Header: The header lives outside the grid bounding box and spans the full width of the viewport. Ensure it respects the overall margin guidelines to maintain visual consistency.
- Sidebar Navigation: The sidebar also lives outside the grid bounding box, typically on the left or right side of the viewport. It should remain fixed or sticky, depending on the design requirements, and should not interfere with the grid layout of the main content.

#### Placement of main content

- Within Columns: Place elements within the grid columns. For instance, on a large desktop, an element spanning 6 columns will take up half the width of the container.
- Edge Margins: Maintain the specified edge margins (40px for large desktop, 32px for small desktop, and 24px for tablet and mobile) to ensure content does not touch the screen edges.

#### Responsiveness

- Adaptation: Design elements should adapt to different breakpoints. Ensure that images, text blocks, and interactive components resize or reflow appropriately.
- Column Adjustments: At smaller breakpoints, elements might span more columns to maintain readability and usability. For example, a 4-column wide element on a large desktop may span all 4 columns on mobile.

#### Header and Sidebar responsiveness on mobile and XS breakpoints

##### Main Header

**Desktop and Tablet View**

- Header Layout: The main header spans the full width of the viewport, containing the logo, primary navigation links, and possibly secondary actions (e.g., search bar, user profile).
- Navigation: Side navigation appears separately on the left or right side of the viewport.

**Mobile and XS Breakpoints (0px - 599px)**

- Header Layout: Simplify the header by consolidating navigation and secondary actions into a hamburger menu.

##### Side Navigation

**Desktop and Tablet View**

- Sidebar Layout: Side navigation is displayed vertically, containing primary and secondary navigation items, possibly with expandable/collapsible sections.

**Mobile and XS Breakpoints**

- Sidebar Transformation: Side navigation collapses into the hamburger menu.

#### Vertical rhythm

- Content Blocks: Align content blocks vertically within the same row. For example, align headings, images, and text blocks along the same baseline where possible.

#### Special cases

- Full-Width Elements: Some elements may need to span the full width of the viewport. In such cases, they should respect the edge margins but not be confined to the column grid.
- Offset Columns: Use column offsets to create white space or to align content differently. For example, offsetting an element by 2 columns to the right on a large desktop.

#### Spanning rules

The following rule outlines how to scale down elements based on their column span at the largest breakpoint (Large Desktop).

| Large Desktop columns | Small Desktop columns | Tablet columns | Mobile columns |
|----------------------|----------------------|----------------|----------------|
| 12 | 12 | 8 | 4 |
| 11 | 11 | 7 | 4 |
| 10 | 10 | 7 | 4 |
| 9 | 9 | 6 | 4 |
| 8 | 8 | 6 | 4 |
| 7 | 7 | 5 | 4 |
| 6 | 6 | 4 | 4 |
| 5 | 5 | 4 | 4 |
| 4 | 4 | 4 | 2 |
| 3 | 3 | 3 | 2 |
| 2 | 2 | 2 | 2 |
| 1 | 1 | 1 | 1 |

##### Examples of column span translations

**Breakpoint: Large Desktop**
- Columns span: 12 columns

**Breakpoint: Small Desktop**
- Columns span: 12 columns

**Breakpoint: Tablet**
- Columns span: 8 columns

**Breakpoint: Mobile**
- Columns span: 4 columns

---

**Breakpoint: Large Desktop**
- Columns span: 8 columns

**Breakpoint: Small Desktop**
- Columns span: 8 columns

**Breakpoint: Tablet**
- Columns span: 6 columns

**Breakpoint: Mobile**
- Columns span: 4 columns

### Guidance for handling tables

For desktop sizes, the table content stays centered within the content area. The table content is scrollable in both directions and may feature an expand option in the table header to enable fullscreen view.

When in fullscreen mode, the table content expands to 100% of the content areas, capped at a maximum width of 1850px, and remains centered. The left sidebar is minimized, and there's a collapse option available in the table header.

## Developers

### Grid system

1. Add the class of '.ap-container' to the container element.

```html
<div class="ap-container"></div>
```

2. Add the class of '.row' to the parent element of elements which are in one row.

```html
<div class="ap-container">
  <div class="row">
    <div class="col">Column</div>
    <div class="col">Column</div>
    <div class="col">Column</div>
  </div>
</div>
```

### CSS Grid

1. Add the class of '.ap-grid' to the container element.

```html
<div class="ap-grid"></div>

<!--
  .ap-grid {
    display: grid;
    grid-template-rows: repeat(var(--ap-rows, 1), 1fr);
    grid-template-columns: repeat(var(--ap-columns, 12), 1fr);
    grid-gap: var(--ap-gap, 1rem);
    gap: var(--ap-gap, 1rem);
  }
-->
```

## Accessibility

Ensure a seamless user experience for product resolutions.

### Overview

- Application or pages should have the ability to increase text size and/or zoom text.
- All content must remain readable and functional even after zooming up to 400%. Avoid overlaps, text cut-offs, content loss, or the need for horizontal scrolling.
- Viewport should adapt to browser width/orientation

### Assumptions

Device Targeting ~ Firm IT Laptop, 1920 x 1080, 100% zoom, Chrome @ 1440 x 1080px

**EXCEPTIONS:**

Horizontal scrolling is allowed on data grids/tables, maps, complex diagrams, or canvas

### How to meet this requirement

- Consider what your designs would look like when zoomed at 200%
- Work with development team to make sure they:
  - Use Appkit Grid and follow grid system to identify these breakpoints
  - Do not include any inline CSS or !important tags on text size that overrides users defined stylesheet
  - Set the viewport to device-width
  - Sets viewport user-scalable="yes" to allow for zooming
  - Test in high contrast mode to make sure there is no overlap
  - Make sure dev team is using relative units for text size
  - Make sure text does not get truncated when zooming up to 400%
  - Make sure you can access all content without horizontal scrolling (unless listed exception)
  - If the page has any data table, or maps, and requires horizontal scrolling then it's highly suggested that a particular area should have a horizontal scrolling option, not the entire page
  - Ensure that scrolling parts must be accessible via the keyboard as well.