# Colors — DrMusa Design System

All hex values correspond exactly to `DESIGN_TOKENS.json → color`. Never use a color outside this palette.

## Primary Color — `#0F6CBD` (Fluent Blue)

The DrMusa brand color. Communicates trust, clinical calm, and professionalism.

| State | Token | Hex |
|---|---|---|
| Default | `brand.primary` | `#0F6CBD` |
| Hover | `brand.primaryHover` | `#115EA3` |
| Pressed | `brand.primaryPressed` | `#0C3B5E` |
| Disabled | `brand.primaryDisabled` | `#C7D9EA` |
| Text on Primary | `brand.onPrimary` | `#FFFFFF` |

**Usage:** Primary buttons, active nav item background, selected states, links, focus rings, progress bars.

## Secondary Color — `#5B5FC7` (Fluent Purple)

Used sparingly for secondary emphasis — never competes with primary blue.

| State | Token | Hex |
|---|---|---|
| Default | `secondary.default` | `#5B5FC7` |
| Hover | `secondary.hover` | `#4F53B7` |
| Pressed | `secondary.pressed` | `#3B3F8F` |

**Usage:** Secondary CTA buttons, analytics chart accents, badges for non-status categories (e.g. "Lab" module tag).

## Neutral Colors

| Purpose | Token | Hex |
|---|---|---|
| App Background | `neutral.background` | `#F5F7FA` |
| Background Alt | `neutral.backgroundAlt` | `#EFF2F6` |
| Surface (cards, panels) | `neutral.surface` | `#FFFFFF` |
| Surface Alt | `neutral.surfaceAlt` | `#FAFBFC` |
| Border | `neutral.border` | `#E1E4E8` |
| Border Strong | `neutral.borderStrong` | `#C7CCD1` |
| Divider | `neutral.divider` | `#EBEDEF` |
| Overlay Scrim | `neutral.overlay` | `rgba(0,0,0,0.4)` |

**Usage:** `background` is the window/page canvas. `surface` is every card, dialog, and panel sitting on top of it. This 2-layer system is how DrMusa creates depth without shadows.

## Text Colors

| Purpose | Token | Hex |
|---|---|---|
| Primary text | `text.primary` | `#1A1D21` |
| Secondary text | `text.secondary` | `#5C6470` |
| Tertiary / helper text | `text.tertiary` | `#8A919C` |
| Disabled text | `text.disabled` | `#B2B8C0` |
| Link | `text.link` | `#0F6CBD` |
| Link hover | `text.linkHover` | `#0C3B5E` |

## Semantic Colors

| Meaning | Text/Icon | Background | Border |
|---|---|---|---|
| Success | `#0F7B3F` | `#DFF6E4` | `#8FD8A4` |
| Warning | `#9D5D00` | `#FFF4CE` | `#F2C866` |
| Danger / Error | `#C4314B` | `#FDE7EA` | `#F1A6B2` |
| Info | `#0F6CBD` | `#E5F1FB` | `#8FC4EE` |

**Usage:** Toast notifications, form validation messages, status badges, alert banners. Always pair the color with an icon and text label — never rely on color alone (accessibility, and colorblind users identifying medical status).

## Medical Status Colors

A dedicated palette for clinical status indicators, separate from generic semantic colors so medical meaning is unambiguous:

| Status | Token | Hex |
|---|---|---|
| Critical | `medical.critical` | `#C4314B` |
| Stable | `medical.stable` | `#0F7B3F` |
| Pending | `medical.pending` | `#9D5D00` |
| In Progress | `medical.inProgress` | `#0F6CBD` |
| Discharged | `medical.discharged` | `#5C6470` |

**Usage:** Patient status badges, appointment status, lab result flags. These colors are reserved exclusively for clinical/workflow status — do not reuse `medical.critical` for a generic "delete" button; use `semantic.danger` for that instead, to keep medical alerts visually distinct.

## Interactive States

| State | Token | Value |
|---|---|---|
| Hover overlay | `state.hoverOverlay` | `rgba(15,108,189,0.06)` |
| Pressed overlay | `state.pressedOverlay` | `rgba(15,108,189,0.12)` |
| Focus ring | `state.focusRing` | `#0F6CBD` |
| Selected background | `state.selectedBg` | `#E5F1FB` |

Hover/Pressed are applied as a semi-transparent overlay on top of the base color/surface — not a hardcoded second color. This keeps every control's hover/pressed state mathematically consistent.

## Usage Examples

- **Primary "Save" button:** background `brand.primary`, text `brand.onPrimary`, hover `brand.primaryHover`, pressed `brand.primaryPressed`.
- **Danger "Delete Patient" button:** background `semantic.danger`, text white, used only for destructive actions.
- **"Critical" patient badge:** background `medical.critical` at 10% opacity tint (`dangerBg`-equivalent), text/icon `medical.critical`, bold caption label "CRITICAL".
- **Disabled input field:** background `neutral.backgroundAlt`, border `neutral.border`, text `text.disabled`.

## Contrast Rules

- Body text on `surface`/`background`: must be `text.primary` or `text.secondary` — both pass WCAG AA on white/light-gray backgrounds.
- Never place `text.tertiary` or `text.disabled` on colored semantic backgrounds — use full-strength semantic text colors instead.
- White text is only used on solid saturated backgrounds (`brand.primary`, `secondary.default`, `semantic.danger`, etc.), never on tinted backgrounds (`successBg`, `dangerBg`, etc.).
