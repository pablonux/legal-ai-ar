---
design-tokens: utility
ecosystem: appkit4
---

# Design Tokens: utility

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