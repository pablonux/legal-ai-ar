---
styles: colors
framework: angular
---

# Colors

Color use within Appkit falls into multiple categories - Primary UI, Extended Primary UI, Neutral, Data visualization and Status colors. See each color category below for more information on each.

For use of any color in UI, always use a contrast checker to confirm WCAG 2.1 AA color contrast requirements (between foreground and background colors) are met. A UI color that provides sufficient color contrast in light mode might not always work in dark mode. It is important to check color contrast for different scenarios in your product. For more information on colors and do's and don'ts, please refer to the Brand site for guidance.

## Primary UI colors

These are recommended primary UI colors that provide improved color contrast in both light and dark modes. The primary UI colors are PwC brand colors that are used within the UI to help accent, highlight, or enhance the look of the interface throughout the application.

### Light mode colors

| Color | Hex |
|-------|-----|
| Primary Blue | `#415385` |
| Primary Orange | `#D04A02` |
| Primary Teal | `#26776D` |
| Primary Pink | `#D93954` |
| Primary Red | `#E0301E` |
| Primary Black | `#2D2D2D` |

### Dark mode colors

#### Text/Links

| Color | Hex |
|-------|-----|
| Primary Blue | `#415385` |
| Primary Orange | `#D04A02` |
| Primary Teal | `#26776D` |
| Primary Pink | `#D93954` |
| Primary Red | `#E0301E` |
| Light Gray | `#EBEBEB` |

#### Backgrounds

| Color | Hex |
|-------|-----|
| Primary Blue | `#9AA4BE` |
| Primary Orange | `#FB734D` |
| Primary Teal | `#50AD95` |
| Primary Pink | `#E27588` |
| Primary Red | `#E96E61` |
| Light Gray | `#EBEBEB` |

> **Note:** For dark mode, always check color contrast for text/font against the dark background you plan to use. Since the primary UI colors do not provide sufficient color contrast when used as text/font color against a dark-mode background, we have recommended text/link colors (corresponding to each primary UI color) against a dark-mode background that provide the required color contrast.

## Extended primary UI colors

Extended color ramps for each of the primary UI colors.

### Primary blue

| Shade | Hex | SCSS | Contrast Ratio | WCAG AA | WCAG AAA |
|-------|-----|------|---------------|---------|----------|
| 0 | `#D2D7E2` | `$primary-blue-01` | 6.54:1 | Yes | Yes |
| 1 | `#9AA4BE` | `$primary-blue-02` | 3.78:1 | Yes | No |
| 2 | `#62719A` | `$primary-blue-03` | 4.82:1 | Yes | Yes |
| 3 (Primary blue) | `#415385` | `$primary-blue-04` | 7.49:1 | Yes | Yes |
| 4 | `#203570` | `$primary-blue-05` | 11.63:1 | Yes | Yes |
| 5 | `#1A2A5A` | `$primary-blue-06` | 13.79:1 | Yes | Yes |
| 6 | `#132043` | `$primary-blue-07` | 15.96:1 | Yes | Yes |
| 7 | `#0D152D` | `$primary-blue-08` | 18.06:1 | Yes | Yes |
| 8 | `#060B16` | `$primary-blue-09` | 19.67:1 | Yes | Yes |

### Primary orange

| Shade | Hex | SCSS | Contrast Ratio | WCAG AA | WCAG AAA |
|-------|-----|------|---------------|---------|----------|
| 0 | `#FEDACC` | `$primary-orange-01` | 7.24:1 | Yes | Yes |
| 1 | `#FDAB8D` | `$primary-orange-02` | 5.11:1 | Yes | No |
| 2 | `#FB7C4D` | `$primary-orange-03` | 3.63:1 | Yes | No |
| 3 | `#E45C2B` | `$primary-orange-04` | 3.58:1 | Yes | No |
| 4 (Primary orange) | `#D04A02` | `$primary-orange-05` | 4.5:1 | Yes | Yes |
| 5 | `#C34C2F` | `$primary-orange-06` | 4.77:1 | Yes | Yes |
| 6 | `#A7452C` | `$primary-orange-07` | 5.94:1 | Yes | Yes |
| 7 | `#773829` | `$primary-orange-08` | 8.81:1 | Yes | Yes |
| 8 | `#472B24` | `$primary-orange-09` | 12.81:1 | Yes | Yes |

### Primary teal

| Shade | Hex | SCSS | Contrast Ratio | WCAG AA | WCAG AAA |
|-------|-----|------|---------------|---------|----------|
| 0 | `#D4EBE9` | `$primary-teal-01` | 6.54:1 | Yes | Yes |
| 1 | `#9ED3CC` | `$primary-teal-02` | 3.78:1 | Yes | No |
| 2 | `#69BAB0` | `$primary-teal-03` | 4.82:1 | Yes | Yes |
| 3 | `#49ABA0` | `$primary-teal-04` | 7.49:1 | Yes | Yes |
| 4 | `#299D8F` | `$primary-teal-05` | 11.63:1 | Yes | Yes |
| 5 | `#27897E` | `$primary-teal-06` | 13.79:1 | Yes | Yes |
| 6 (Primary teal) | `#26776D` | `$primary-teal-07` | 15.96:1 | Yes | Yes |
| 7 | `#245952` | `$primary-teal-08` | 18.06:1 | Yes | Yes |
| 8 | `#223937` | `$primary-teal-09` | 19.67:1 | Yes | Yes |

### Primary pink

| Shade | Hex | SCSS | Contrast Ratio | WCAG AA | WCAG AAA |
|-------|-----|------|---------------|---------|----------|
| 0 | `#F8DDE1` | `$primary-pink-01` | 6.54:1 | Yes | Yes |
| 1 | `#F1BAC3` | `$primary-pink-02` | 3.78:1 | Yes | No |
| 2 | `#E998A6` | `$primary-pink-03` | 4.82:1 | Yes | Yes |
| 3 | `#E27588` | `$primary-pink-04` | 7.49:1 | Yes | Yes |
| 4 (Primary pink) | `#D93954` | `$primary-pink-05` | 11.63:1 | Yes | Yes |
| 5 | `#B5485B` | `$primary-pink-06` | 13.79:1 | Yes | Yes |
| 6 | `#903F4D` | `$primary-pink-07` | 15.96:1 | Yes | Yes |
| 7 | `#6B343D` | `$primary-pink-08` | 18.06:1 | Yes | Yes |
| 8 | `#462B2F` | `$primary-pink-09` | 19.67:1 | Yes | Yes |

### Primary red

| Shade | Hex | SCSS | Contrast Ratio | WCAG AA | WCAG AAA |
|-------|-----|------|---------------|---------|----------|
| 0 | `#F9D6D2` | `$primary-red-01` | 6.54:1 | Yes | Yes |
| 1 | `#F1A29A` | `$primary-red-02` | 3.78:1 | Yes | No |
| 2 | `#E96E61` | `$primary-red-03` | 4.82:1 | Yes | Yes |
| 3 | `#E44F3F` | `$primary-red-04` | 7.49:1 | Yes | Yes |
| 4 (Primary red) | `#E0301E` | `$primary-red-05` | 11.63:1 | Yes | Yes |
| 5 | `#C22D1D` | `$primary-red-06` | 13.79:1 | Yes | Yes |
| 6 | `#A62B1E` | `$primary-red-07` | 15.96:1 | Yes | Yes |
| 7 | `#772820` | `$primary-red-08` | 18.06:1 | Yes | Yes |
| 8 | `#472420` | `$primary-red-09` | 19.67:1 | Yes | Yes |

## Neutral colors

UI colors are defined neutral shades of light and dark grays used throughout the UI. They are used in combination with the primary brand colors.

| Shade | Hex | SCSS | Contrast Ratio | WCAG AA | WCAG AAA |
|-------|-----|------|---------------|---------|----------|
| 0 | `#FFFFFF` | `$neutral-01` | 9.43:1 | Yes | Yes |
| 1 | `#F3F3F3` | `$neutral-02` | 8.5:1 | Yes | Yes |
| 2 | `#E8E8E8` | `$neutral-03` | 7.7:1 | Yes | Yes |
| 3 | `#DDDDDD` | `$neutral-04` | 6.94:1 | Yes | Yes |
| 4 | `#D1D1D1` | `$neutral-05` | 6.18:1 | Yes | Yes |
| 5 | `#C5C5C5` | `$neutral-06` | 5.46:1 | Yes | Yes |
| 6 | `#BABABA` | `$neutral-07` | 4.86:1 | Yes | Yes |
| 7 | `#AFAFAF` | `$neutral-08` | 4.3:1 | Yes | No |
| 8 | `#A3A3A3` | `$neutral-09` | 3.74:1 | Yes | No |
| 9 | `#979797` | `$neutral-10` | 3.23:1 | Yes | No |
| 10 | `#8C8C8C` | `$neutral-11` | 3.36:1 | Yes | No |
| 11 | `#818181` | `$neutral-12` | 3.89:1 | Yes | No |
| 12 | `#757575` | `$neutral-13` | 4.6:1 | Yes | Yes |
| 13 | `#696969` | `$neutral-14` | 5.48:1 | Yes | Yes |
| 14 | `#5E5E5E` | `$neutral-15` | 6.48:1 | Yes | Yes |
| 15 | `#535353` | `$neutral-16` | 7.69:1 | Yes | Yes |
| 16 | `#474747` | `$neutral-17` | 9.29:1 | Yes | Yes |
| 17 | `#3B3B3B` | `$neutral-18` | 11.2:1 | Yes | Yes |
| 18 | `#303030` | `$neutral-19` | 13.19:1 | Yes | Yes |
| 19 | `#252525` | `$neutral-20` | 15.32:1 | Yes | Yes |
| 20 | `#191919` | `$neutral-21` | 17.58:1 | Yes | Yes |
| 21 | `#111111` | `$neutral-22` | 18.88:1 | Yes | Yes |
| 22 | `#000000` | `$neutral-23` | 21:1 | Yes | Yes |

## Data visualization colors

Visualization colors are used only for data visualizations. A range of colors are provided to support the categorization and signaling needs of a data visualization.

| Color | Hex | SCSS | Contrast Ratio | WCAG AA | WCAG AAA |
|-------|-----|------|---------------|---------|----------|
| Orange 0 | `#FEB791` | `$data-orange-lighter` | 9.04:1 | Yes | Yes |
| Orange 1 | `#FD6412` | `$data-orange-light` | 5.11:1 | Yes | Yes |
| Orange 2 | `#D04A02` | `$data-orange` | 4.5:1 | Yes | Yes |
| Orange 3 | `#933401` | `$data-orange-dark` | 7.68:1 | Yes | Yes |
| Orange 4 | `#571F01` | `$data-orange-darker` | 7.24:1 | Yes | Yes |
| Yellow 0 | `#FFDCA9` | `$data-tangerine-lighter` | 11.72:1 | Yes | Yes |
| Yellow 1 | `#FFA929` | `$data-tangerine-light` | 7.99:1 | Yes | Yes |
| Yellow 2 | `#EB8C00` | `$data-tangerine` | 6.04:1 | Yes | Yes |
| Yellow 3 | `#AE6800` | `$data-tangerine-dark` | 4.31:1 | Yes | No |
| Yellow 4 | `#714300` | `$data-tangerine-darker` | 8.38:1 | Yes | Yes |
| Yellow 5 | `#FFECBD` | `$data-yellow-lighter` | 13.12:1 | Yes | Yes |
| Yellow 6 | `#FFC83D` | `$data-yellow-light` | 9.22:1 | Yes | Yes |
| Yellow 7 | `#FFB600` | `$data-yellow` | 8.71:1 | Yes | No |
| Yellow 8 | `#C28A00` | `$data-yellow-dark` | 5.05:1 | Yes | Yes |
| Yellow 9 | `#855F0.0` | `$data-yellow-darker` | 5.77:1 | Yes | Yes |
| Red 0 | `#F7C8C4` | `$data-red-lighter` | 10.22:1 | Yes | Yes |
| Red 1 | `#E86153` | `$data-red-light` | 4.57:1 | Yes | Yes |
| Red 2 | `#E0301E` | `$data-red` | 4.55:1 | Yes | Yes |
| Red 3 | `#AA2417` | `$data-red-dark` | 7.24:1 | Yes | Yes |
| Red 4 | `#741910` | `$data-red-darker` | 11.11:1 | Yes | Yes |
| Pink 0 | `#F1BAC3` | `$data-rose-lighter` | 9.16:1 | Yes | Yes |
| Pink 1 | `#E27588` | `$data-rose-light` | 5.2:1 | Yes | Yes |
| Pink 2 | `#D93954` | `$data-rose` | 4.5:1 | Yes | Yes |
| Pink 3 | `#A43E50` | `$data-rose-dark` | 6.2:1 | Yes | Yes |
| Pink 4 | `#6E2A35` | `$data-rose-darker` | 10.26:1 | Yes | Yes |
| Gray 0 | `#F2F2F2` | `$data-gray-lighter` | 13.69:1 | Yes | Yes |
| Gray 1 | `#DEDEDE` | `$data-gray-light` | 11.39:1 | Yes | Yes |
| Gray 2 | `#7D7D7D` | `$data-gray` | 4.11:1 | Yes | No |
| Gray 3 | `#2D2D2D` | `$data-gray-dark` | 13.77:1 | Yes | Yes |
| Gray 4 | `#141414` | `$data-gray-darker` | 7.24:1 | Yes | Yes |
| Purple 0 | `#DCB4FC` | `$data-purple-lighter` | 8.74:1 | Yes | Yes |
| Purple 1 | `#B056F6` | `$data-purple-light` | 4.02:1 | Yes | No |
| Purple 2 | `#8E34F4` | `$data-purple` | 5.31:1 | Yes | Yes |
| Purple 3 | `#6B2CDA` | `$data-purple-dark` | 7:1 | Yes | Yes |
| Purple 4 | `#4B20AB` | `$data-purple-darker` | 7.24:1 | Yes | Yes |
| Blue 0 | `#B3DCF9` | `$data-blue-lighter` | 10.59:1 | Yes | Yes |
| Blue 1 | `#4DACF1` | `$data-blue-light` | 6.19:1 | Yes | Yes |
| Blue 2 | `#0089EB` | `$data-blue` | 3.63:1 | Yes | No |
| Blue 3 | `#0060D7` | `$data-blue-dark` | 5.74:1 | Yes | Yes |
| Blue 4 | `#003DAB` | `$data-blue-darker` | 9.31:1 | Yes | Yes |
| Green 0 | `#C4FC9F` | `$data-green-lighter` | 12.97:1 | Yes | Yes |
| Green 1 | `#86DB4F` | `$data-green-light` | 8.94:1 | Yes | Yes |
| Green 2 | `#4EB523` | `$data-green` | 5.8:1 | Yes | Yes |
| Green 3 | `#2C8646` | `$data-green-dark` | 4.55:1 | Yes | Yes |
| Green 4 | `#175C2C` | `$data-green-darker` | 8.06:1 | Yes | Yes |

## Status colors

Sometimes referred to as "traffic lights" status colors are used to help signal the level or status of something. These signals could be described as "high, medium, low" or "success, warning, error" for example. Who your target users are can determine how the status colors should be used. In most contexts green represents positive or success, yellow represents neutral or warning and red represents negative or error.

### States error

| Shade | Hex | SCSS | Contrast Ratio | WCAG AA | WCAG AAA |
|-------|-----|------|---------------|---------|----------|
| 0 | `#F3D4D1` | `$states-error-01` | 6.8:1 | Yes | Yes |
| 1 | `#E8AAA3` | `$states-error-02` | 4.81:1 | Yes | Yes |
| 2 | `#DC7F76` | `$states-error-03` | 3.21:1 | Yes | No |
| 3 | `#D15548` | `$states-error-04` | 4.11:1 | Yes | No |
| 4 (Error) | `#C52A1A` | `$states-error-05` | 5.64:1 | Yes | Yes |
| 5 | `#A4291D` | `$states-error-06` | 7.2:1 | Yes | Yes |
| 6 | `#822720` | `$states-error-07` | 9.29:1 | Yes | Yes |
| 7 | `#612622` | `$states-error-08` | 11.61:1 | Yes | Yes |
| 8 | `#3F2425` | `$states-error-09` | 12.58:1 | Yes | Yes |

### States warning

| Shade | Hex | SCSS | Contrast Ratio | WCAG AA | WCAG AAA |
|-------|-----|------|---------------|---------|----------|
| 0 | `#FFF2D2` | `$states-warning-01` | 8.48:1 | Yes | Yes |
| 1 | `#FFE5A5` | `$states-warning-02` | 7.64:1 | Yes | Yes |
| 2 | `#FFD979` | `$states-warning-03` | 6.94:1 | Yes | Yes |
| 3 | `#FFCC4C` | `$states-warning-04` | 6.28:1 | Yes | Yes |
| 4 (Warning) | `#FFBF1F` | `$states-warning-05` | 5.71:1 | Yes | Yes |
| 5 | `#D2A021` | `$states-warning-06` | 2.31:1 | No | No |
| 6 | `#A58123` | `$states-warning-07` | 3.64:1 | Yes | No |
| 7 | `#786124` | `$states-warning-08` | 5.94:1 | Yes | Yes |
| 8 | `#4B4226` | `$states-warning-09` | 9.96:1 | Yes | Yes |

### States success

| Shade | Hex | SCSS | Contrast Ratio | WCAG AA | WCAG AAA |
|-------|-----|------|---------------|---------|----------|
| 0 | `#D3EBD5` | `$states-success-01` | 7.46:1 | Yes | Yes |
| 1 | `#A7D6AB` | `$states-success-02` | 5.77:1 | Yes | Yes |
| 2 | `#7AC282` | `$states-success-03` | 4.43:1 | Yes | No |
| 3 | `#4EAD58` | `$states-success-04` | 3.34:1 | Yes | No |
| 4 (Success) | `#22992E` | `$states-success-05` | 3.7:1 | Yes | No |
| 5 | `#21812D` | `$states-success-06` | 4.94:1 | Yes | Yes |
| 6 | `#206A2C` | `$states-success-07` | 6.64:1 | Yes | Yes |
| 7 | `#20522A` | `$states-success-08` | 9.12:1 | Yes | Yes |
| 8 | `#1F3B29` | `$states-success-09` | 12.23:1 | Yes | Yes |