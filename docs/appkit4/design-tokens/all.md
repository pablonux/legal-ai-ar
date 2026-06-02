---
design-tokens: all
ecosystem: appkit4
---

# Design tokens

Design tokens are the foundation of Appkit's design system, ensuring consistency and scalability across products. These reusable variables enable designers and developers to apply a unified style effortlessly across components and themes. By standardizing key UI elements, design tokens enhance efficiency, accessibility, and brand cohesion. Explore the token categories below to see their intended uses and implementation guidelines.

<!-- SECTION:utility -->
## Utility tokens

Utility tokens systematize the design language for a specific theme or component. These usually point to a generic or reference token.

### Base tokens

| Name | Description | Light Value | Dark Value |
|------|-------------|-------------|------------|
| `$color-background-default` | Default background color for the app | `#F3F3F3` | `#191919` |
| `$color-background-default-inverse` | Inverse background color | `#191919` | `#FFFFFF` |
| `$color-background-alt` | Second default color for the app background | `#FFFFFF` | `#000000` |
| `$color-background-alt-inverse` | Alternative background color for the dark mode | `#111111` | `#FFFFFF` |
| `$color-background-container` | Default background color for panels on app | `#FFFFFF` | `#252525` |
| `$color-background-container-alt` | Alternative background color for panel when used… | `#F3F3F3` | `#3B3B3B` |
| `$color-background-secondary` | Default color for secondary interactive elements… | `#FFFFFF` | `#FFFFFF` |
| `$color-background-tertiary` | Default color for tertiary elements. Ex: Tertiary button | `#474747` | `#FFFFFF` |
| `$color-background-hover` | Background color when hovering on an item | `#F3F3F3` | `#303030` |
| `$color-background-selected` | Background color when selecting an item | `#F3F3F3` | `#303030` |
| `$color-background-hover-selected` | Background color when hovering on a selected item | `#E8E8E8` | `#3B3B3B` |
| `$color-background-triple-hover` | Background color when hovering on a selected… | `#DDDDDD` | `#474747` |
| `$color-background-border` | Border color. Ex: Input fields | `#D1D1D1` | `#696969` |
| `$color-background-danger` | Default color for destructive button and error status | `#C52A1A` | `#C52A1A` |
| `$color-background-success` | Default color for success button and status | `#21812D` | `#21812D` |
| `$color-background-warning` | Default color for warning status | `#FFBF1F` | `#FFBF1F` |
| `$color-background-danger-inverse` | Default color for destructive button and error… | `#C52A1A` | `#C52A1A` |
| `$color-background-success-inverse` | Default color for success button and status in dark… | `#21812D` | `#21812D` |
| `$color-background-warning-inverse` | Default color for warning status in high contrast… | `#FFBF1F` | `#FFBF1F` |
| `$color-background-icon-secondary` | Default color for secondary icons | `#FFFFFF` | `#FFFFFF` |
| `$color-background-icon-alt` | Default color for icons on ui colors | `#FFFFFF` | `#FFFFFF` |
| `$color-text-secondary` | Default color for secondary text, body copy | `#FFFFFF` | `#FFFFFF` |
| `$color-text-light` | Text color for lighter text | `#696969` | `#979797` |
| `$color-text-heading` | Text color of heading title | `#252525` | `#FFFFFF` |
| `$color-text-body` | Text color of body text | `#474747` | `#D1D1D1` |
| `$color-text-disabled` | Text color of disabled text | `#DDDDDD` | `#696969` |
| `$color-text-success` | Default color for success message text | `#21812D` | `#21812D` |
| `$color-text-warning` | Default color for warning message text | `#FFBF1F` | `#FFBF1F` |

### Theme-specific tokens

The following tokens vary based on the selected color theme (Blue, Orange, Teal, Pink, Red, or Black).

| Name | Description | Blue (Light/Dark) | Orange (Light/Dark) | Teal (Light/Dark) | Pink (Light/Dark) | Red (Light/Dark) | Black (Light/Dark) |
|------|-------------|-------------------|---------------------|-------------------|-------------------|------------------|-------------------|
| `$color-background-primary` | Default color for primary interactive elements… | `#415385` / `#415385` | `#D04A02` / `#D04A02` | `#26776D` / `#26776D` | `#D93954` / `#D93954` | `#E0301E` / `#E0301E` | `#2D2D2D` / `#EBEBEB` |
| `$color-background-icon-primary` | Default color for icons | `#415385` / `#415385` | `#D04A02` / `#D04A02` | `#26776D` / `#26776D` | `#D93954` / `#D93954` | `#E0301E` / `#E0301E` | `#2D2D2D` / `#EBEBEB` |
| `$color-text-primary` | Default color for primary text, body copy | `#415385` / `#9AA4BE` | `#D04A02` / `#FB734D` | `#26776D` / `#50AD95` | `#D93954` / `#E27588` | `#E0301E` / `#E96E61` | `#2D2D2D` / `#EBEBEB` |
| `$color-text-link-primary` | Default color for primary links | `#415385` / `#9AA4BE` | `#D04A02` / `#FB734D` | `#26776D` / `#50AD95` | `#D93954` / `#E27588` | `#E0301E` / `#E96E61` | `#2D2D2D` / `#EBEBEB` |
| `$color-text-link-secondary` | Default color for secondary links | `#415385` / `#9AA4BE` | `#D04A02` / `#FB734D` | `#26776D` / `#50AD95` | `#D93954` / `#E27588` | `#E0301E` / `#E96E61` | `#2D2D2D` / `#EBEBEB` |
| `$color-text-link-primary-inverse` | To be used on dark background | `#415385` / `#9AA4BE` | `#D04A02` / `#FB734D` | `#26776D` / `#50AD95` | `#D93954` / `#E27588` | `#E0301E` / `#E96E61` | `#2D2D2D` / `#EBEBEB` |
| `$color-text-link-secondary-inverse` | To be used on dark background | `#415385` / `#9AA4BE` | `#D04A02` / `#FB734D` | `#26776D` / `#50AD95` | `#D93954` / `#E27588` | `#E0301E` / `#E96E61` | `#2D2D2D` / `#EBEBEB` |
| `$color-text-tertiary` | Default color for tertiary text, body copy | `#FFFFFF` / `#FFFFFF` | `#FFFFFF` / `#FFFFFF` | `#FFFFFF` / `#FFFFFF` | `#FFFFFF` / `#FFFFFF` | `#FFFFFF` / `#FFFFFF` | - / - |
| `$color-text-error` | Default color for error message text | `#C52A1A` / `#DC7F76` | `#C52A1A` / `#DC7F76` | `#C52A1A` / `#DC7F76` | `#C52A1A` / `#DC7F76` | `#C52A1A` / `#DC7F76` | `#C52A1A` / `#DC7F76` |
<!-- /SECTION:utility -->

<!-- SECTION:primary -->
## Primary colors

Tokens prefixed with "Primary" are PwC branded tokens. See the description of each token for its intended use.

| Name | Description | Value |
|------|-------------|-------|
| `$primary-red-01` | Primary red blue color | `#F9D6D2` |
| `$primary-red-02` | Primary red blue color | `#F1A29A` |
| `$primary-red-03` | Primary red blue color | `#E96E61` |
| `$primary-red-04` | Primary red blue color | `#E44F3F` |
| `$primary-red-05` | Primary red blue color | `#E0301E` |
| `$primary-red-06` | Primary red blue color | `#C22D1D` |
| `$primary-red-07` | Primary red blue color | `#A62B1E` |
| `$primary-red-08` | Primary red blue color | `#772820` |
| `$primary-red-09` | Primary red blue color | `#472420` |
| `$primary-pink-01` | Primary pink blue color | `#F8DDE1` |
| `$primary-pink-02` | Primary pink blue color | `#F1BAC3` |
| `$primary-pink-03` | Primary pink blue color | `#E998A6` |
| `$primary-pink-04` | Primary pink blue color | `#E27588` |
| `$primary-pink-05` | Primary pink blue color | `#D93954` |
| `$primary-pink-06` | Primary pink blue color | `#B5485B` |
| `$primary-pink-07` | Primary pink blue color | `#903F4D` |
| `$primary-pink-08` | Primary pink blue color | `#6B343D` |
| `$primary-pink-09` | Primary pink blue color | `#462B2F` |
| `$primary-teal-01` | Primary teal blue color | `#D4EBE9` |
| `$primary-teal-02` | Primary teal blue color | `#9ED3CC` |
| `$primary-teal-03` | Primary teal blue color | `#69BAB0` |
| `$primary-teal-04` | Primary teal blue color | `#49ABA0` |
| `$primary-teal-05` | Primary teal blue color | `#299D8F` |
| `$primary-teal-06` | Primary teal blue color | `#27897E` |
| `$primary-teal-07` | Primary teal blue color | `#26776D` |
| `$primary-teal-08` | Primary teal blue color | `#245952` |
| `$primary-teal-09` | Primary teal blue color | `#223937` |
| `$primary-orange-01` | Primary brand orange color | `#FEDACC` |
| `$primary-orange-02` | Primary brand orange color | `#FDAB8D` |
| `$primary-orange-03` | Primary brand orange color | `#FB7C4D` |
| `$primary-orange-04` | Primary brand orange color | `#E45C2B` |
| `$primary-orange-05` | Primary brand orange color | `#D04A02` |
| `$primary-orange-06` | Primary brand orange color | `#C34C2F` |
| `$primary-orange-07` | Primary brand orange color | `#A7452C` |
| `$primary-orange-08` | Primary brand orange color | `#773829` |
| `$primary-orange-09` | Primary brand orange color | `#472B24` |
| `$primary-blue-01` | Primary brand blue color | `#D2D7E2` |
| `$primary-blue-02` | Primary brand blue color | `#9AA4BE` |
| `$primary-blue-03` | Primary brand blue color | `#62719A` |
| `$primary-blue-04` | Primary brand blue color | `#415385` |
| `$primary-blue-05` | Primary brand blue color | `#203570` |
| `$primary-blue-06` | Primary brand blue color | `#1A2A5A` |
| `$primary-blue-07` | Primary brand blue color | `#132043` |
| `$primary-blue-08` | Primary brand blue color | `#0D152D` |
| `$primary-blue-09` | Primary brand blue color | `#060B16` |
<!-- /SECTION:primary -->

<!-- SECTION:status -->
## Status colors

| Name | Description | Value |
|------|-------------|-------|
| `$states-error-01` | States error | `#F3D4D1` |
| `$states-error-02` | States error | `#E8AAA3` |
| `$states-error-03` | States error | `#DC7F76` |
| `$states-error-04` | States error | `#D15548` |
| `$states-error-05` | States error | `#C52A1A` |
| `$states-error-06` | States error | `#A4291D` |
| `$states-error-07` | States error | `#822720` |
| `$states-error-08` | States error | `#612622` |
| `$states-error-09` | States error | `#3F2425` |
| `$states-warning-01` | States warning | `#FFF2D2` |
| `$states-warning-02` | States warning | `#FFE5A5` |
| `$states-warning-03` | States warning | `#FFD979` |
| `$states-warning-04` | States warning | `#FFCC4C` |
| `$states-warning-05` | States warning | `#FFBF1F` |
| `$states-warning-06` | States warning | `#D2A021` |
| `$states-warning-07` | States warning | `#A58123` |
| `$states-warning-08` | States warning | `#786124` |
| `$states-warning-09` | States warning | `#4B4226` |
| `$states-success-01` | States success | `#D3EBD5` |
| `$states-success-02` | States success | `#A7D6AB` |
| `$states-success-03` | States success | `#7AC282` |
| `$states-success-04` | States success | `#4EAD58` |
| `$states-success-05` | States success | `#22992E` |
| `$states-success-06` | States success | `#21812D` |
| `$states-success-07` | States success | `#206A2C` |
| `$states-success-08` | States success | `#20522A` |
| `$states-success-09` | States success | `#1F3B29` |
<!-- /SECTION:status -->

<!-- SECTION:data -->
## Data visualization colors

These colors should only be used to visualize data via charts, graphs or infographics.

| Name | Description | Value |
|------|-------------|-------|
| `$data-orange-lighter` | Data orange lighter | `#FEB791` |
| `$data-orange-light` | Data orange light | `#FD6412` |
| `$data-orange` | Data orange | `#D04A02` |
| `$data-orange-dark` | Data orange dark | `#933401` |
| `$data-orange-darker` | Data orange darker | `#571F01` |
| `$data-tangerine-lighter` | Data tangerine lighter | `#FFDCA9` |
| `$data-tangerine-light` | Data tangerine light | `#FFA929` |
| `$data-tangerine` | Data tangerine | `#EB8C00` |
| `$data-tangerine-dark` | Data tangerine dark | `#AE6800` |
| `$data-tangerine-darker` | Data tangerine darker | `#714300` |
| `$data-yellow-lighter` | Data yellow lighter | `#FFECBD` |
| `$data-yellow-light` | Data yellow light | `#FFC83D` |
| `$data-yellow` | Data yellow | `#FFB600` |
| `$data-yellow-dark` | Data yellow dark | `#C28A00` |
| `$data-yellow-darker` | Data yellow darker | `#855F0.0` |
| `$data-red-lighter` | Data red lighter | `#F7C8C4` |
| `$data-red-light` | Data red light | `#E86153` |
| `$data-red` | Data red | `#E0301E` |
| `$data-red-dark` | Data red dark | `#AA2417` |
| `$data-red-darker` | Data red darker | `#741910` |
| `$data-rose-lighter` | Data rose lighter | `#F1BAC3` |
| `$data-rose-light` | Data rose light | `#E27588` |
| `$data-rose` | Data rose | `#D93954` |
| `$data-rose-dark` | Data rose dark | `#A43E50` |
| `$data-rose-darker` | Data rose darker | `#6E2A35` |
| `$data-gray-lighter` | Data gray lighter | `#F2F2F2` |
| `$data-gray-light` | Data gray light | `#DEDEDE` |
| `$data-gray` | Data gray | `#7D7D7D` |
| `$data-gray-dark` | Data gray dark | `#2D2D2D` |
| `$data-gray-darker` | Data gray darker | `#141414` |
| `$data-purple-lighter` | Data purple lighter | `#DCB4FC` |
| `$data-purple-light` | Data purple light | `#B056F6` |
| `$data-purple` | Data purple | `#8E34F4` |
| `$data-purple-dark` | Data purple dark | `#6B2CDA` |
| `$data-purple-darker` | Data purple darker | `#4B20AB` |
| `$data-blue-lighter` | Data blue lighter | `#B3DCF9` |
| `$data-blue-light` | Data blue light | `#4DACF1` |
| `$data-blue` | Data blue | `#0089EB` |
| `$data-blue-dark` | Data blue dark | `#0060D7` |
| `$data-blue-darker` | Data blue darker | `#003DAB` |
| `$data-green-lighter` | Data green lighter | `#C4FC9F` |
| `$data-green-light` | Data green light | `#86DB4F` |
| `$data-green` | Data green | `#4EB523` |
| `$data-green-dark` | Data green dark | `#2C8646` |
| `$data-green-darker` | Data green darker | `#175C2C` |
<!-- /SECTION:data -->

<!-- SECTION:neutral -->
## Neutral colors

| Name | Description | Value |
|------|-------------|-------|
| `$neutral-01` | Color neutral 1 | `#FFFFFF` |
| `$neutral-02` | Color neutral 2 | `#F3F3F3` |
| `$neutral-03` | Color neutral 3 | `#E8E8E8` |
| `$neutral-04` | Color neutral 4 | `#DDDDDD` |
| `$neutral-05` | Color neutral 5 | `#D1D1D1` |
| `$neutral-06` | Color neutral 6 | `#C5C5C5` |
| `$neutral-07` | Color neutral 7 | `#BABABA` |
| `$neutral-08` | Color neutral 8 | `#AFAFAF` |
| `$neutral-09` | Color neutral 9 | `#A3A3A3` |
| `$neutral-10` | Color neutral 10 | `#979797` |
| `$neutral-11` | Color neutral 11 | `#8C8C8C` |
| `$neutral-12` | Color neutral 12 | `#818181` |
| `$neutral-13` | Color neutral 13 | `#757575` |
| `$neutral-14` | Color neutral 14 | `#696969` |
| `$neutral-15` | Color neutral 15 | `#5E5E5E` |
| `$neutral-16` | Color neutral 16 | `#535353` |
| `$neutral-17` | Color neutral 17 | `#474747` |
| `$neutral-18` | Color neutral 18 | `#3B3B3B` |
| `$neutral-19` | Color neutral 19 | `#303030` |
| `$neutral-20` | Color neutral 20 | `#252525` |
| `$neutral-21` | Color neutral 21 | `#191919` |
| `$neutral-22` | Color neutral 22 | `#111111` |
| `$neutral-23` | Color neutral 23 | `#000000` |
<!-- /SECTION:neutral -->

<!-- SECTION:spacing -->
## Spacing

Use these tokens to define element spacing. See the description of each token for its intended use.

| Name | Description | Value |
|------|-------------|-------|
| `$spacing-1` | Adds 2px spacing | `0.125rem` (2px) |
| `$spacing-2` | Adds 4px spacing | `0.25rem` (4px) |
| `$spacing-3` | Adds 8px spacing | `0.5rem` (8px) |
| `$spacing-4` | Adds 12px spacing | `0.75rem` (12px) |
| `$spacing-5` | Adds 16px spacing | `1rem` (16px) |
| `$spacing-6` | Adds 20px spacing | `1.25rem` (20px) |
| `$spacing-7` | Adds 24px spacing | `1.5rem` (24px) |
| `$spacing-8` | Adds 48px spacing | `3rem` (48px) |
<!-- /SECTION:spacing -->

<!-- SECTION:typography -->
## Typography

Use these tokens to set your font styles. See the description of each token for its intended use.

| Name | Description | Value |
|------|-------------|-------|
| `$typography-body-xs` | Sets font-size and line-height at 12px/14px | `0.75rem / 0.875rem` (12px/14px) |
| `$typography-body-s` | Sets font-size and line-height at 14px/20px | `0.875rem / 1.25rem` (14px/20px) |
| `$typography-body` | Sets font-size and line-height at 16px/24px | `1rem / 1.5rem` (16px/24px) |
| `$typography-heading-s` | Sets font-size and line-height at 20px/24px | `1.25rem / 1.5rem` (20px/24px) |
| `$typography-heading-m` | Sets font-size and line-height at 24px/32px | `1.5rem / 2rem` (24px/32px) |
| `$typography-heading-l` | Sets font-size and line-height at 36px/42px | `2.25rem / 2.625rem` (36px/42px) |
| `$typography-data` | Sets font-size and line-height at 48px/48px | `3rem / 3rem` (48px/48px) |

## Weight

Use these tokens for font weights. See the description of each token for its intended use.

| Name | Description | Value |
|------|-------------|-------|
| `$font-weight-1` | Sets font-weight to 400 | `400` (Regular) |
| `$font-weight-2` | Sets font-weight to 500 | `500` (Medium) |
| `$font-weight-3` | Sets font-weight to 700 | `700` (Bold) |
<!-- /SECTION:typography -->

<!-- SECTION:border-radius -->
## Border radius

Use these radius tokens to define element border radius. See the description of each token for its intended use.

| Name | Description | Value |
|------|-------------|-------|
| `$border-radius-1` | Sets radius to 2px smooth corners | `0.125rem` (2px) |
| `$border-radius-2` | Sets radius to 4px smooth corners | `0.25rem` (4px) |
| `$border-radius-3` | Sets radius to 8px smooth corners | `0.5rem` (8px) |
<!-- /SECTION:border-radius -->

<!-- SECTION:elevation -->
## Elevation

Use elevation tokens to set element depth. See the description of each token for its intended use.

| Name | Description | Value |
|------|-------------|-------|
| `$elevation-container-flat-default` | Use for resting surfaces and flat elements | Background: `#FFFFFF` with border |
| `$elevation-container-flat-hover` | Use for resting surfaces and flat elements | Background: `#F3F3F3` |
| `$elevation-container-flat-selected` | Use for resting surfaces and flat elements | Background: `#F3F3F3` |
| `$elevation-container-flat-hover-selected` | Use for resting surfaces and flat elements | Background: `#E8E8E8` |
| `$elevation-container-low-default` | Use for slightly raised surfaces | Background: `#FFFFFF` with border |
| `$elevation-container-low-hover` | Use for slightly raised surfaces | Background: `#F3F3F3` |
| `$elevation-container-low-selected` | Use for slightly raised surfaces | Background: `#F3F3F3` |
| `$elevation-container-low-hover-selected` | Use for slightly raised surfaces | Background: `#E8E8E8` |
| `$elevation-container-medium-default` | Use to provide a greater separation | Background: `#FFFFFF` with border |
| `$elevation-container-medium-hover` | Use to provide a greater separation | Background: `#F3F3F3` |
| `$elevation-container-medium-selected` | Use to provide a greater separation | Background: `#F3F3F3` |
| `$elevation-container-medium-hover-selected` | Use to provide a greater separation | Background: `#E8E8E8` |
| `$elevation-container-high-default` | Higher elevation for prominent UI elements | Background: `#FFFFFF` with border |
| `$elevation-container-high-hover` | Higher elevation for prominent UI elements | Background: `#F3F3F3` |
| `$elevation-container-high-selected` | Higher elevation for prominent UI elements | Background: `#F3F3F3` |
| `$elevation-container-high-hover-selected` | Higher elevation for prominent UI elements | Background: `#E8E8E8` |

## Shadow

Use depth tokens to set element box shadow. See the description of each token for its intended use.

| Name | Description | Value |
|------|-------------|-------|
| `$elevation-shadow-flat` | Use to depict flat elevation | No shadow |
| `$elevation-shadow-low` | Use to depict low elevation | `rgba(71, 71, 71, 0.24) 0px 0.125rem 0.25rem -0.125rem` |
| `$elevation-shadow-medium` | Use to depict medium elevation | `rgba(71, 71, 71, 0.24) 0px 0.25rem 0.5rem -0.125rem` |
| `$elevation-shadow-high` | Use to depict higher elevation | `rgba(71, 71, 71, 0.24) 0px 0.5rem 1rem -0.125rem` |
<!-- /SECTION:elevation -->

<!-- SECTION:blur -->
## Background blur

Use these tokens for background styling of headers and modal overlays. See the description of each token for its intended use.

| Name | Description | Value |
|------|-------------|-------|
| `$blur-1` | Modal background blur | `rgba(45, 45, 45, 0.32)` with `blur(0.125rem)` |
| `$blur-2` | Header background blur | `rgba(243, 243, 243, 0.72)` with `blur(0.5rem)` |
<!-- /SECTION:blur -->

<!-- SECTION:opacity -->
## Opacity

Use these opacity tokens for element transparency. See the description of each token for its intended use.

| Name | Description | Value |
|------|-------------|-------|
| `$opacity-1` | Sets the opacity at .04 | `0.04` |
| `$opacity-2` | Sets the opacity at .08 | `0.08` |
| `$opacity-3` | Sets the opacity at .12 | `0.12` |
| `$opacity-4` | Sets the opacity at .24 | `0.24` |
| `$opacity-5` | Sets the opacity at .32 | `0.32` |
| `$opacity-6` | Sets the opacity at .48 | `0.48` |
| `$opacity-7` | Sets the opacity at 1 | `1` |
<!-- /SECTION:opacity -->