# UX Rules — DrMusa Design System

DrMusa is used in real-time clinical and billing environments. UX mistakes here cost staff time and, in clinical contexts, can affect patient safety. These rules take priority over visual polish.

## Consistency

- The same action (Save, Delete, Print, Cancel) always appears in the same position across every screen (footer, right-aligned, primary rightmost).
- The same keyboard shortcuts work identically on every screen (see Keyboard Shortcuts below).
- Terminology is fixed app-wide: e.g. always "Patient," never mix with "Client" or "Customer" in some screens.

## Feedback

- Every user action gets a visible response within 100ms: a button press state, a spinner, or a toast — never silence.
- Successful actions (save, print, update) confirm via a toast notification (`semantic.success`), not just a silent state change.
- Long-running actions (report generation, bulk import) show a progress indicator with, where possible, a percentage or step count — not just an indeterminate spinner beyond a few seconds.

## Error Handling

- Validation errors appear inline, next to the field that caused them, in `semantic.danger` with a short, specific message ("Phone number must be 11 digits" — not "Invalid input").
- System/network errors (e.g. database write failure) surface as a toast or dialog with a plain-language explanation and a clear next step ("Couldn't save the record. Check your connection and try again.") — never raw exception text or error codes in the primary message (log the technical detail separately for support/diagnostics).
- Never let an error silently fail — the user must always know something didn't work.

## Loading States

- Any operation expected to take >300ms shows a loading indicator.
- Full-screen or full-panel loads: skeleton placeholders matching the eventual layout (preferred) or a centered spinner (`brand.primary`) for very short loads.
- Never block the entire app on a background task that doesn't require it (e.g. printing a receipt shouldn't freeze navigation).

## Empty States

- Every list/table has a designed empty state: icon (`iconSize.xl`, `text.tertiary`), a short `body` message, and — when applicable — a primary action to resolve it (e.g. "No patients yet" + "Add Patient" button).
- Empty states never look like a bug — they should feel intentional and helpful.

## Success States

- Confirm destructive or important actions with a clear, brief success indicator (toast, or in-context checkmark) before returning the user to their previous context.
- For multi-step flows (e.g. patient registration), show a final success screen or toast summarizing what was created.

## Confirmation Dialogs

- Required before any destructive or hard-to-reverse action: deleting a patient record, voiding a paid transaction, discharging a patient.
- Dialog states clearly what will happen ("This will permanently delete John Doe's record. This can't be undone.") and uses a `Danger` styled confirm button, never a default primary blue for destructive confirms.
- Not required for easily-reversible actions (e.g. removing an item from a cart before checkout) — don't over-use confirmation dialogs, as "confirmation fatigue" causes staff to click through them without reading.

## Keyboard Shortcuts

Critical for fast clinical/POS workflows:

| Shortcut | Action |
|---|---|
| `Esc` | Close dialog / cancel current action |
| `Enter` | Confirm/submit primary action in a focused dialog or form |
| `Ctrl+S` | Save current form |
| `Ctrl+P` | Print (receipt, report) |
| `Ctrl+F` / `F3` | Focus search box |
| `Tab` / `Shift+Tab` | Move between fields in logical order |
| `F2` | Edit selected row (tables) |
| `Delete` | Remove selected row (with confirmation if destructive) |

Document any screen-specific shortcuts (e.g. POS "F4 = Apply Discount") in that screen's section of `SCREEN_RULES.md`.

## Focus Management

- On dialog/form open, focus moves automatically to the first input.
- On dialog close, focus returns to the element that triggered it (e.g. back to the "Add Patient" button).
- Tab order follows visual/logical reading order (top-to-bottom, left-to-right) — never a scrambled tab index.
- Focus is always visibly indicated (`state.focusRing`) — critical for keyboard-driven receptionist/pharmacist workflows.

## Accessibility & Tab Navigation

- Every screen must be fully operable via keyboard alone (no mouse-only interactions).
- Color contrast follows `COLORS.md` contrast rules (WCAG AA minimum).
- Status/meaning is never conveyed by color alone — always paired with icon/text (critical for medical status accuracy).
- Interactive elements have accessible names (for screen readers) even though DrMusa's primary users are sighted staff — this future-proofs for accessibility compliance in healthcare procurement.

## Visual Hierarchy

- One primary action per screen — everything else is secondary or tertiary.
- The most important information (patient name, critical alerts, current total in POS) is always the largest/boldest element on screen.
- Group related information visually (cards, sections) rather than relying on the user to mentally group scattered fields.

## Medical Software UX Recommendations

- **Never auto-submit or auto-save silently** for clinical data — staff must explicitly confirm changes to patient records (contrast with e.g. a search box, which can debounce/auto-query).
- **Show "last updated by / at"** metadata on clinical records for accountability and audit trail visibility.
- **Critical alerts (allergies, drug interactions) are never dismissible with a single click** — require an explicit acknowledgment action, and log it.
- **Undo where safe:** for non-critical actions (e.g. removing a cart item), prefer an "Undo" toast over a confirmation dialog — faster and less disruptive for high-volume workflows.
- **Optimize for speed in repetitive workflows** (billing, patient check-in): minimize clicks, support full keyboard operation, default to the most common values (today's date, default doctor if only one on shift).
- **Design for interruption:** clinical staff get interrupted constantly — forms should preserve entered data if a user navigates away and back within a session (draft state), not force re-entry.
