---
styles: elevation
framework: angular
---

# Elevation

Elevation allows Appkit users to establish zones of emphasis, priority, establish hierarchical zones and focus within digital experiences. It also helps distinguish parts of components that need to have raised surfaces. In Appkit, elevations are comprised of both surfaces and shadows. When combined, these elements create a sense of elevation or depth. Elevations can direct attention by layering, or suggest interactive possibilities.

> **Note:** Elevation tokens are currently in beta and will be introduced in future releases as part of the baseline component, available as an elevated variant.

## Elevation in light mode

Design tokens are utilized in elevations to assign various surface levels. The higher elevation container surfaces, also use shadows to enhance depth perception.

## Elevation in dark mode

In dark mode, shadows are less noticeable, thus dark mode elevations incorporate distinct surface colors. Picture the surfaces as if they were softly illuminated from the front: as the elevation increases, the surface appears lighter.

## Levels of elevation

There are 4 elevation levels in Appkit, going from flat elevation to high elevation.

### Flat

**Token:** `$elevation/container/flat` + no shadow

*as recommended by Appkit team

Resting surfaces, flat elements and primary UI shell.

### Low

**Token:** `$elevation/container/low` + `$elevation/shadow/low`

*as recommended by Appkit team

Slightly raised surfaces to indicate separation from the background. This elevation setting is where most Appkit components fall.

### Medium

**Token:** `$elevation/container/medium` + `$elevation/shadow/medium`

*as recommended by Appkit team

Components and parts that are elevated more providing a greater separation.

### High

**Token:** `$elevation/container/high` + `$elevation/shadow/high`

*as recommended by Appkit team

Higher elevation for prominent UI elements and components.

### Elevation level usage

| Elevation level | Description | Components and UI elements |
|----------------|-------------|---------------------------|
| Flat | Resting surfaces, flat elements, and primary UI shell. | Background, Main content area, Header, Sidebar |
| Low | Slightly raised surfaces to indicate separation from the background. | Accordion, Avatar, Badge, Breadcrumb, Button, Checkbox, Feed and comment, File explorer, Filter, Input field, List, Loading, Pagination, Progress, Radio button, Rating and review, Rich text editor, Search, Slider, Tab, Table, Tag, Toggle, Containers, Cards, Texts |
| Medium | Components that are elevated more, providing a greater separation. | Date picker (Overlay), Dropdown (Overlay), Notice, Panel, Tooltip, Upload form |
| High | Higher elevation for prominent UI elements. | Drawer, Modal, Notification |

> **Note:** Appkit team's recommended component elevation levels.

## Shadows

Shadow styles have been updated to enhance contrast and improve rendering in dark mode, providing a more realistic representation of shadow behavior in this environment. These updated styles are now accessible as elevation shadow tokens within the design system.

## Interaction states

Elevations employ changes in container surface color to indicate various interaction states. Use the hover, selected and selected hover elevation tokens to create these visual changes and show interaction at each level.

### Interaction states in light mode

Elevation tokens work with hover, selected, and selected-hover interaction states. Each elevation level has corresponding interaction tokens to reflect these states effectively.

### Interaction states in dark mode

In dark mode, the same interaction states apply, with elevation tokens providing appropriate visual feedback through surface color changes.

## Figma instructions

Elevation tokens can be optionally applied to override the default container tokens shipped by default in the system. This is particularly useful for depicting higher elevation or correcting color overlap issues that may arise in complex designs.

In Figma, elevation tokens are accessible as surface or container colors within the Figma Variables panel as selection colors. A new group of elevation tokens is available in the variables panel, offering four levels of elevation: flat, low, medium, and high elevation.

### Accessing elevation tokens

1. Open the Variables panel in Figma.
2. Navigate to the new Elevation group under Surface/Container Colors to find the elevation tokens corresponding to each level: (Flat), (Low), (Medium), and (High).

### Applying elevation to components

1. Select the component or frame where you want to apply elevation.
2. In the right-hand properties panel, under Fill, click on the color dropdown and select the appropriate elevation token from the Elevation group now available as color variables.

### Interaction states with elevation

1. Elevation can be combined with interaction states (hover, selected). Each elevation has a set of interaction token for each container level.
2. Apply the corresponding elevation token to each interaction state, ensuring that each state change reflects the intended visual effect.

## Instructions for devs

Use the below sample code to use elevation tokens.

Set the elevation tokens or function to the HTML element with the class of 'container'.

### HTML

```html
<div class="container"></div>
```

### SCSS

```scss
@import '/node_modules/@appkit4/styles/scss/mixin';
@import '/node_modules/@appkit4/styles/scss/variables';

.container {
    @include setElevation('flat');
    // @include setElevation('low');
    // @include setElevation('medium');
    // @include setElevation('high');
}

// Or you can set the elevation design tokens separately

.container {
    box-shadow: $elevation-shadow-high;
    background-color: $elevation-container-high-default;

    &:hover {
      background-color: $elevation-container-high-hover;
    }

    &:active {
      background-color: $elevation-container-high-selected;
    }

    &:hover:active {
      background-color: $elevation-container-high-hover-selected;
    }
}
```

## FAQs

### General overview questions

**What are elevation tokens in Appkit?**

They define surface levels and shadows to create a sense of depth, hierarchy, and focus. They help distinguish raised areas within components, making interactions clearer. Shadow styles have been updated to better convey depth in dark mode.

**How many elevation levels does Appkit offer?**

Appkit provides four elevation levels: Flat, Low, Medium, and High. Each level indicates a different degree of prominence and separation from the background.

**Why did we work/introduce elevation tokens?**

Elevation tokens help direct attention to key elements, create visual hierarchy, and provide better separation of content. We introduced elevation tokens to help with some of the overlapping issues we had, especially when rendering certain surfaces in dark mode.

**Are there plans for the future of Elevation tokens?**

Yes, elevation tokens are currently in beta, and we are considering including them in the future in directly applied to certain components that need to have a certain elevation by default as elevated variants.

### Design implementation questions

**How are elevation tokens accessed in Figma?**

Elevation tokens are available in the Variables panel under the Elevation group. Designers can apply them as overrides to surface or container colors to components or frames.

**When should elevation tokens be used?**

There's no need to change anything in your current designs. However, elevation tokens can be used to highlight components, avoid color overlap issues in dark mode, or create depth.

**How do interactions come into play when elevation tokens are applied?**

Elevation tokens work with hover, selected, and selected-hover interaction states. Each elevation level has corresponding interaction tokens to reflect these states effectively. If you have applied an override to a surface color, using container colors from the elevation group, and there are interactions you need to preserve/show at that level, you can override the interaction color by selecting the corresponding color from the elevation tokens.

### Development-related questions

**How do developers implement elevation tokens?**

There are corresponding tokens in dev to all the Figma variables we have introduced. Developers can apply elevation tokens by setting them as CSS classes (e.g., .container) on HTML elements. These tokens dictate the shadow and surface styles at each elevation level.

### Customization and use cases

**What are the most common scenarios to increase elevation levels?**

Use higher elevations for overlays (e.g., dropdowns, panels) and critical components that need immediate user attention, such as modals or notifications.