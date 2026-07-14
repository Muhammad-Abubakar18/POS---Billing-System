# Screen Rules — DrMusa Design System

All screens share the base structure defined in `LAYOUT_RULES.md` (Top Bar + Sidebar + Content) and the type/color/component rules from their respective files. This document adds per-screen specifics.

---

## Login

- Centered card (max-width ~400px) on a full-bleed `neutral.background` (or subtle brand-tinted) canvas — the one screen without sidebar/top bar.
- App name/logo at `displaySmall`, tagline at `caption`.
- Fields: Username/Email, Password (with show/hide toggle), Role selector if applicable.
- Primary button: "Sign In", full-width within the card, `buttonLarge`.
- Error state: inline banner above the form in `semantic.danger` for invalid credentials — never reveal whether username or password specifically was wrong.

## Dashboard

- Follows the Dashboard Layout pattern in `LAYOUT_RULES.md`.
- Role-aware: Doctors see clinical KPIs (patients seen, pending consults); Receptionists see appointments/check-ins; Admins see revenue/staff KPIs.
- KPI cards: `title`-sized number, `caption` label, optional trend indicator (small up/down arrow + `semantic.success`/`danger` text).
- Recent activity table below the fold, max 5-10 rows with a "View All" link.

## Patient Registration

- Form Layout pattern, multi-section card: Personal Info → Contact Info → Medical Info (allergies, blood type, existing conditions) → Emergency Contact.
- Required fields marked with `*`, validated inline (`UX_RULES.md`).
- Allergy/critical medical fields use `medical.critical` accent styling to stand out even during data entry.
- Sticky footer: Cancel (secondary) + Save Patient (primary).

## Doctor Management

- Table/List Layout: searchable/filterable table of doctors (name, specialty, status, schedule).
- Row click opens a detail dialog/panel: profile info, specialty, availability schedule, assigned patients count.
- "Add Doctor" primary button top-right of toolbar.

## Appointments

- Default view: calendar/day-view grid showing time slots per doctor (columns) — a specialized layout beyond the standard table, but still uses `neutral.divider` grid lines and `brand.primary` for the current time indicator.
- Alternate view: list/table view with filter by date/doctor/status, toggle between calendar and list via a segmented control near the toolbar.
- Appointment status badges use the standard Status Badge component (`Confirmed`, `Pending`, `Completed`, `Cancelled` → mapped to `medical`/`semantic` tokens).

## Billing / POS Cart

- Split Panel Layout: left 65% product/service search & selection, right 35% cart.
- Cart panel: scrollable line-item list (product, qty stepper, price, remove icon), sticky totals block at bottom (Subtotal, Tax, Discount, **Total** at `title` size bold), then primary "Print Receipt" / "Complete Sale" button (`buttonLarge`, full-width of cart panel).
- Product grid/list on the left uses clickable product cards/tiles with image, name, price — click adds to cart (per existing DrMusa product-click-to-add behavior).

## Invoices

- Table/List Layout for invoice history: Invoice #, Date, Patient/Customer, Amount, Status (Paid/Pending/Void).
- Detail view: printable invoice layout — clean, minimal, black-text-on-white for print compatibility, DrMusa logo header, itemized table, totals block, mirrors the physical receipt format.

## Reports

- Filter toolbar at top (date range, report type, doctor/department filter) using standard form controls.
- Report body: mix of KPI summary cards + charts (`COMPONENT_RULES.md` Charts section) + a detailed data table below.
- Export actions (PDF/Excel) as secondary buttons top-right, primary action reserved for "Generate Report" if report is on-demand.

## Settings

- Left-hand sub-navigation (within content area, not the main sidebar) listing setting categories: General, Users & Roles, Billing, Printer/Hardware, Backup.
- Each category is a form-style card list following Form Layout rules, with a persistent Save bar.

## Profile

- Card-based layout: profile photo/avatar, name, role badge, contact info form, "Change Password" as a separate secondary action/dialog rather than inline in the main form.

## Tables (General Standard)

Every data table across screens (Patients, Doctors, Invoices, Products, Appointments list view) follows the DataGrid component rules exactly: toolbar (search + filters left, primary action right) → table → pagination footer. No screen invents its own table styling.

## Forms (General Standard)

Every data-entry form (Patient Registration, Doctor Management add/edit, Settings) follows the Form Layout pattern: sectioned cards, 2-column field grid where fields are short, full-width for long fields (notes/address), sticky action footer.

## Popups / Dialogs

All confirmation, quick-add, and detail-preview popups use the Dialog component exactly as defined in `COMPONENT_RULES.md` — sizing (`dialogSmallWidth/MediumWidth/LargeWidth`), header/footer pattern, overlay scrim.

## Analytics

Shares layout DNA with Reports: KPI cards + charts, but more dashboard-like (auto-refreshing where relevant) and less export/print-oriented. Use consistent chart color mapping across all analytics screens (see `COMPONENT_RULES.md` Charts) so the same metric always uses the same color everywhere in the app.

---

## Cross-Screen Consistency Checklist

Every screen above must share:
- Same top bar height and content (56px, logo/search/profile).
- Same sidebar width and active-state styling.
- Same `spacing.xl` (32px) outer content padding.
- Same button placement convention (primary action bottom/top-right, destructive actions require confirmation).
- Same typography scale for titles/subtitles/body/captions.
- Same status badge / color-coding logic for equivalent concepts (e.g. "Pending" always renders identically whether it's an appointment, invoice, or lab result).
