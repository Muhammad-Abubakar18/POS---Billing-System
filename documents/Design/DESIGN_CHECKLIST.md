# Design Checklist — DrMusa Design System

Every developer (human or AI agent) must verify this checklist before marking any screen or component as complete. Do not submit a screen that fails any item below.

## Tokens & Values

- [ ] Uses only typography sizes/weights from `DESIGN_TOKENS.json` / `TYPOGRAPHY.md`
- [ ] Uses only spacing values from the 4/8/12/16/24/32/40/48 scale — no arbitrary margins/padding
- [ ] Uses only colors from `DESIGN_TOKENS.json` / `COLORS.md` — no hardcoded hex codes in XAML
- [ ] No hardcoded numeric literals for corner radius, border width, shadow, or control height
- [ ] Uses correct control heights (`buttonMedium`, `inputMedium`, `tableRow`, etc.) per component

## Components

- [ ] Reuses existing shared Styles/UserControls — no duplicated or reinvented component XAML
- [ ] Buttons follow variant rules (one primary per screen; secondary/subtle/danger used correctly)
- [ ] Forms include labels, helper text, and inline validation per `COMPONENT_RULES.md`
- [ ] Tables follow the standard DataGrid style (header, row height, hover/selected states, empty state)
- [ ] Status/badges use the correct semantic or medical color token and include an icon or label, not color alone

## Layout

- [ ] Matches the correct screen pattern from `SCREEN_RULES.md` (Dashboard / Form / Table / Split Panel)
- [ ] Top bar and sidebar are consistent with every other screen (height, width, active states)
- [ ] Content padding and section spacing follow `LAYOUT_RULES.md`
- [ ] Grid/column alignment is consistent — no ad hoc floating elements

## Visual Hierarchy & Accessibility

- [ ] One clear primary action per screen; secondary/tertiary actions are visually subordinate
- [ ] Text/background color pairs meet WCAG AA contrast (see `COLORS.md`)
- [ ] Every interactive element has a visible keyboard focus state
- [ ] Every interactive element is operable via keyboard, not mouse-only
- [ ] No meaning is conveyed by color alone (status, errors, alerts all paired with icon/text)

## Fluent 2 / Enterprise Fit

- [ ] Flat, minimal surfaces — no excessive or stacked shadows
- [ ] Corner radii are small and consistent (4px controls, 8px cards, 12px pill badges only)
- [ ] Icons are from the Fluent System Icons set, correct size token, correct color token
- [ ] No decorative or unnecessary animation; any motion used has a functional purpose and correct duration token

## XAML Quality (MVVM)

- [ ] No hardcoded UI values when a token/resource exists
- [ ] No business logic in code-behind — all actions bound to ViewModel `ICommand`s
- [ ] No duplicated XAML blocks that should be a shared `Style` or `UserControl`
- [ ] Resource references use `DynamicResource` for colors/brushes, `StaticResource` for structural values
- [ ] Follows the naming conventions in `XAML_STYLE_GUIDE.md`

## Functional UX

- [ ] Loading state shown for any operation >300ms
- [ ] Empty state designed for any list/table that can be empty
- [ ] Success feedback (toast or equivalent) shown after key actions (save, print, delete)
- [ ] Destructive actions require confirmation with a `Danger`-styled confirm button
- [ ] Form data is not silently lost on navigation away/back within a session

## Final Pass

- [ ] Screen visually matches the spacing, typography, and color rhythm of at least one other existing screen when placed side-by-side
- [ ] Ran through `AGENTS.md` pre-flight checklist before submission
