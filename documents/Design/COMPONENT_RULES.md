# Component Rules — DrMusa Design System

Every component below references tokens from `DESIGN_TOKENS.json`. Implement each as a shared WPF `Style` or `UserControl` — never restyle inline per screen.

---

## Buttons

**Purpose:** Trigger actions. One primary button per screen/section max.

| Property | Value |
|---|---|
| Height | `controlHeight.buttonMedium` (32px) default; `buttonLarge` (40px) for primary screen actions; `buttonSmall` (28px) inside tables/toolbars |
| Corner radius | `cornerRadius.button` (4px) |
| Padding | horizontal `spacing.md` (16px), vertical `spacing.xs` (8px) |
| Typography | `body`/`bodyStrong` (14px SemiBold) |
| Min width | 80px |

**Variants:**
- **Primary:** bg `brand.primary`, text `onPrimary`. Used for the single main action (Save, Confirm, Print Receipt).
- **Secondary:** bg `surface`, border `borderStrong`, text `text.primary`. Used for Cancel, Back, secondary actions.
- **Subtle/Ghost:** no border/bg, text `brand.primary`. Used for tertiary actions, table row actions.
- **Danger:** bg `semantic.danger`, text white. Used only for destructive actions (Delete, Void Transaction).

**States:** Hover = `state.hoverOverlay` on top of base; Pressed = `state.pressedOverlay`; Focused = 2px `state.focusRing` outline offset 2px; Disabled = `opacity.disabled` (0.4), no pointer events.

**Do:** Use icon + label for actions where icon adds clarity (e.g. 🖨 Print). Keep label verbs short ("Save", "Print Receipt", "Void").
**Don't:** Don't use more than one primary button per view. Don't use all-caps labels. Don't make buttons full-width unless in a mobile-width dialog.

---

## Text Boxes

| Property | Value |
|---|---|
| Height | `controlHeight.inputMedium` (32px); `inputLarge` (40px) for prominent search/POS fields |
| Corner radius | `cornerRadius.input` (4px) |
| Border | 1px `neutral.border`; focus: 2px `state.focusRing` |
| Padding | horizontal `spacing.sm` (12px) |
| Typography | `body` (14px Regular) |

**States:** Default border `neutral.border` → Hover border `borderStrong` → Focus 2px `focusRing` + subtle `selectedBg` tint → Error border `semantic.danger` + caption error message below in `semantic.danger` → Disabled bg `backgroundAlt`, text `text.disabled`.

**Do:** Always pair with a `bodyStrong` label above and optional `caption` helper text below. Show validation errors inline immediately below the field.
**Don't:** Don't use placeholder text as a substitute for a label. Don't rely on red border alone to indicate error — include an error icon + message.

---

## ComboBoxes / Dropdowns

Same sizing as text boxes (`inputMedium`). Chevron icon (`iconSize.sm`, 16px) right-aligned, `text.secondary`. Dropdown panel: `shadow.flyout`, `cornerRadius.card`, max-height with scroll after 8 items. Selected item row: `state.selectedBg` background, `bodyStrong` text, checkmark icon.

**Do:** Support type-ahead filtering for lists >10 items (e.g. medication lists, doctor lists).
**Don't:** Don't use for binary choices — use a toggle/checkbox instead.

---

## DataGrid (Tables)

The core component of DrMusa — patient lists, product lists, billing lines, appointment schedules.

| Property | Value |
|---|---|
| Row height | `controlHeight.tableRow` (40px) standard; `tableRowCompact` (32px) for dense POS cart view |
| Header row height | `controlHeight.tableHeaderRow` (40px) |
| Header style | bg `neutral.backgroundAlt`, text `bodyStrong`, `text.secondary`, uppercase, `letterSpacing.wide` |
| Row border | 1px bottom `neutral.divider` (no vertical grid lines — Fluent style) |
| Cell padding | horizontal `spacing.sm` (12px) |
| Zebra striping | Not used by default — use `state.hoverOverlay` on hover row instead |
| Selected row | bg `state.selectedBg`, left 3px accent bar `brand.primary` |

**States:** Hover row = subtle `hoverOverlay` bg. Selected row = `selectedBg` + accent bar. Empty state = centered icon + `body` message + optional action button (see UX_RULES.md).

**Do:** Right-align numeric columns (price, quantity, totals). Sticky header on scroll. Sortable column headers show a small chevron icon.
**Don't:** Don't use vertical grid lines. Don't shrink row height below `tableRowCompact` — touch/click targets must stay usable for fast POS workflows.

---

## Cards

| Property | Value |
|---|---|
| Corner radius | `cornerRadius.card` (8px) |
| Shadow | `shadow.card` |
| Padding | `spacing.lg` (24px) |
| Background | `neutral.surface` |
| Header | `subtitle` (18px SemiBold) + optional `caption` subtext |

**Do:** Use cards to group related KPIs, form sections, or a single patient/appointment summary. Keep one clear header per card.
**Don't:** Don't nest cards inside cards. Don't apply `shadow.dialog` to a card — reserve heavier shadow for true modals.

---

## Dialogs / Modals

| Property | Value |
|---|---|
| Width | `window.dialogSmallWidth` (400px, confirmations) / `dialogMediumWidth` (560px, forms) / `dialogLargeWidth` (800px, complex forms e.g. patient intake) |
| Corner radius | `cornerRadius.dialog` (8px) |
| Shadow | `shadow.dialog` |
| Overlay | `neutral.overlay` behind dialog, dims the app |
| Header | `subtitle` (18px SemiBold) + close (×) icon button top-right |
| Footer | Right-aligned button row: Secondary (Cancel) then Primary (Confirm), `spacing.sm` gap |

**Do:** Always provide a Cancel/Close path (Esc key + × icon + Cancel button). Focus the first input automatically on open.
**Don't:** Don't stack dialogs on top of dialogs. Don't use dialogs for simple confirmations that a toast could handle.

---

## Navigation Drawer / Sidebar

| Property | Value |
|---|---|
| Width | `window.sidebarWidth` (260px) expanded / `sidebarCollapsedWidth` (64px) icon-only |
| Background | `neutral.surface` or a slightly darker `backgroundAlt`, with `neutral.border` right edge |
| Item height | 40px |
| Item padding | horizontal `spacing.md` |
| Active item | bg `state.selectedBg`, left 3px accent bar `brand.primary`, text `bodyStrong` `brand.primary` |
| Icon size | `iconSize.md` (20px) |

**Do:** Group items by module (Clinical, Billing, Admin) with `caption` section labels. Support collapse/expand toggle at bottom.
**Don't:** Don't use more than 2 levels of nesting — flatten navigation for fast clinical workflows.

---

## Top Bar

Height `window.topBarHeight` (56px). Background `neutral.surface`, bottom border `neutral.border`. Contains: app logo/name (left), global search (center-left, optional), user profile + notifications + role badge (right).

**Do:** Always show current logged-in user + role (Doctor/Receptionist/etc.) — critical for shared clinic workstations.
**Don't:** Don't put primary navigation in the top bar — that belongs in the sidebar.

---

## Search Box

Same as text box but with search icon (`iconSize.sm`) left-aligned, clear (×) icon right-aligned when text present. Height `inputMedium`/`inputLarge` depending on context (global search = large).

**Do:** Debounce live-search by ~250ms (`animation.durationSlow`) to avoid excessive queries.

---

## Date Picker / Calendar

Input field styled as standard text box with calendar icon. Popup calendar: `shadow.flyout`, `cornerRadius.card`, current day outlined with `brand.primary`, selected day filled `brand.primary` bg + white text, today's date has subtle `selectedBg` if not selected.

**Do:** Default to today's date for new records (appointments, visits). Support keyboard date entry, not just picker clicks.

---

## Radio Buttons & Checkboxes

Size `controlHeight.checkboxRadio` (18px). Unchecked: 1px `neutral.borderStrong` outline, transparent fill. Checked: `brand.primary` fill + white check/dot icon. Label: `body`, `spacing.xs` gap from control.

**Do:** Always pair with a clickable label (increases hit target). Use radio buttons for 2–5 mutually exclusive options; use dropdown beyond 5.

---

## Tabs

Height 40px. Underline style (Fluent 2): active tab has 2px bottom border `brand.primary`, text `bodyStrong` `text.primary`; inactive tabs text `text.secondary`, no border. `spacing.lg` gap between tab labels.

**Don't:** Don't use boxed/pill tabs — underline style only, per Fluent 2.

---

## Tooltips

Background `text.primary` (dark), text white, `caption` size, `cornerRadius.tooltip` (4px), padding `spacing.xs`/`spacing.sm`. Appears after ~400ms hover delay, `shadow.flyout`.

**Do:** Use for icon-only buttons and truncated table cell content.
**Don't:** Don't put critical information only in a tooltip — it's not accessible via keyboard-only or touch use.

---

## Toast Notifications

Width ~360px, `cornerRadius.card`, `shadow.raised`, positioned top-right, stacked with `spacing.xs` gap. Left accent bar (4px) in semantic color (success/warning/danger/info) + matching icon. Auto-dismiss after 4–6s for success/info; warnings/errors require manual dismiss.

**Do:** Use for confirmations of background actions (e.g. "Receipt printed", "Patient record saved").
**Don't:** Don't use toasts for errors that block a workflow — use inline validation or a dialog instead.

---

## Status Badges

Height ~22px, `cornerRadius.badge` (12px, pill shape — the one intentional exception to DrMusa's 4/8px radius rule, matching Fluent 2 badge convention), padding `spacing.xs` horizontal, `captionSmall`/`caption` text, uppercase, `letterSpacing.wide`. Background = semantic/medical tint color, text/dot = full-strength semantic/medical color.

**Do:** Always include a leading status dot or icon alongside color, never color alone.

---

## Progress Bars & Loading Indicators

Progress bar: height 4px, track `neutral.border`, fill `brand.primary`, `cornerRadius` 2px (half of height). Loading spinner: Fluent-style indeterminate ring, `brand.primary`, sizes matching `iconSize` tokens (sm for inline, xl for full-page load).

**Do:** Show a loading state for any operation >300ms (DB queries, report generation). Use skeleton placeholders (gray blocks, `backgroundAlt`) for table/card loading rather than a centered spinner when loading content into an existing layout.

---

## Pagination

Height 32px controls, `body` text for page numbers, `brand.primary` for active page (bg `selectedBg`, `cornerRadius.button`), `text.secondary` for inactive. Prev/Next as icon buttons.

**Do:** Show "Showing 1–20 of 248" alongside page controls for data-heavy tables.

---

## Charts (Reports/Analytics)

Use brand + secondary + semantic colors only, in this priority order: `brand.primary`, `secondary.default`, `semantic.success`, `semantic.warning`, `semantic.info`. Axis labels `caption`, `text.secondary`. Gridlines `neutral.divider`, thin, low opacity. No 3D effects, no drop shadows on chart elements.

**Do:** Label data directly where practical instead of relying solely on a legend.
**Don't:** Don't use more than 5 distinct colors in one chart — split into multiple charts instead.

---

## Global Accessibility Rules (all components)

- Every interactive control must be reachable and operable via keyboard (Tab, Enter, Esc, Arrow keys where applicable).
- Every interactive control must show a visible focus ring (`state.focusRing`, 2px, `borderWidth.focus`).
- Never use color as the sole differentiator of meaning — pair with icon and/or text label.
- Minimum hit target: 32×32px, even if the visual element is smaller (pad the click area).
