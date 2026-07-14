# XAML Style Guide — DrMusa Design System

## Naming Conventions

| Element | Convention | Example |
|---|---|---|
| Views (Windows/Pages/UserControls) | PascalCase + suffix by type | `PatientRegistrationView.xaml`, `MainWindow.xaml`, `PatientCard.xaml` (UserControl) |
| ViewModels | Matches view name + `ViewModel` | `PatientRegistrationViewModel.cs` |
| Styles | `{ControlType}.{Variant}Style` | `Button.PrimaryStyle`, `TextBox.DefaultStyle`, `DataGrid.StandardStyle` |
| Brushes (colors) | `{Category}{Purpose}Brush` | `BrandPrimaryBrush`, `TextSecondaryBrush`, `SemanticDangerBrush` |
| Named x:Key resources | Match the token name in `DESIGN_TOKENS.json` | `<SolidColorBrush x:Key="BrandPrimaryBrush" .../>` mirrors `color.brand.primary` |
| Converters | `{Purpose}Converter` | `BoolToVisibilityConverter`, `StatusToColorConverter` |
| Commands (in ViewModel) | `{Verb}{Noun}Command` | `SavePatientCommand`, `PrintReceiptCommand` |

## Folder Structure

```
DrMusa/
├── App.xaml
├── Themes/
│   ├── Colors.xaml            ← generated from DESIGN_TOKENS.json color section
│   ├── Typography.xaml        ← font sizes/weights as resources
│   ├── Spacing.xaml           ← Thickness resources for margins/padding tokens
│   ├── Buttons.xaml           ← Style definitions for all button variants
│   ├── Inputs.xaml            ← TextBox, ComboBox, DatePicker styles
│   ├── DataGrid.xaml          ← DataGrid + row/header styles
│   ├── Cards.xaml
│   ├── Dialogs.xaml
│   ├── Badges.xaml
│   └── Generic.xaml           ← merges all of the above into one dictionary
├── Controls/                  ← reusable custom UserControls
│   ├── StatusBadge.xaml
│   ├── KpiCard.xaml
│   ├── SidebarNavItem.xaml
│   └── ToastNotification.xaml
├── Views/
│   ├── Dashboard/
│   ├── Patients/
│   ├── Billing/
│   ├── Appointments/
│   └── Settings/
├── ViewModels/                ← mirrors Views/ structure
├── Models/
├── Services/
└── Converters/
```

## ResourceDictionary Usage

- `App.xaml` merges `Themes/Generic.xaml` at the application level — every window/view inherits all styles automatically.
- Never define a `ResourceDictionary` inline inside a View unless it's a view-specific data template that has no reuse potential elsewhere.
- Colors, typography, and spacing are defined **once** in their respective theme files and never redefined downstream.

```xml
<!-- App.xaml -->
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Themes/Generic.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## Styles & Templates

- Every reusable control has an explicit `Style` with `TargetType` and, where the default template needs restructuring (e.g. Fluent-style flat buttons), a `ControlTemplate`.
- Use `BasedOn` to extend a base style for variants rather than duplicating the whole style:

```xml
<Style x:Key="Button.BaseStyle" TargetType="Button">
    <Setter Property="Height" Value="{StaticResource ButtonMediumHeight}"/>
    <Setter Property="FontSize" Value="{StaticResource BodyFontSize}"/>
    <Setter Property="FontWeight" Value="SemiBold"/>
    <Setter Property="Padding" Value="16,8"/>
    <Setter Property="Cursor" Value="Hand"/>
</Style>

<Style x:Key="Button.PrimaryStyle" TargetType="Button" BasedOn="{StaticResource Button.BaseStyle}">
    <Setter Property="Background" Value="{DynamicResource BrandPrimaryBrush}"/>
    <Setter Property="Foreground" Value="{DynamicResource OnPrimaryBrush}"/>
</Style>
```

## DynamicResource vs StaticResource

- **Use `DynamicResource` for colors/brushes** — allows future theme switching (e.g. potential dark mode or high-contrast accessibility mode) without recompiling.
- **Use `StaticResource` for values that never change at runtime**: font sizes, spacing/margin values, corner radius, control heights. These are structural, not theme-dependent.

```xml
<Border Background="{DynamicResource SurfaceBrush}"
        CornerRadius="{StaticResource CardCornerRadius}"
        Padding="{StaticResource SpacingLg}"/>
```

## Reusable Controls

- Any UI pattern used on 2+ screens becomes a `UserControl` in `/Controls/` — e.g. `StatusBadge`, `KpiCard`, `EmptyState`, `ConfirmationDialog`.
- UserControls expose `DependencyProperty` bindings for their variable content (text, status type, command) — never hardcode data inside a reusable control.

```xml
<!-- Usage across any screen -->
<controls:StatusBadge Status="{Binding Patient.Status}" />
<controls:KpiCard Title="Total Patients" Value="{Binding TotalPatients}" Trend="Up"/>
```

## MVVM Best Practices

- Views bind to ViewModels via `DataContext`, set either in code-behind constructor (simple cases) or via DI + `ViewModelLocator`/DI container (preferred for DrMusa's scale).
- All user actions bind to `ICommand` (`RelayCommand`/`AsyncRelayCommand` from CommunityToolkit.Mvvm or equivalent) — never an `x:Name` + code-behind `Click` handler for business logic.
- Use `INotifyPropertyChanged` (via `ObservableObject` base class) for all bindable ViewModel properties.
- Validation lives in the ViewModel (implementing `INotifyDataErrorInfo` or similar) — not in code-behind.
- Code-behind is reserved strictly for: view-only concerns like triggering a focus, playing a UI-only animation, or wiring up a third-party control event that has no clean command binding.

## Performance Recommendations

- Use `VirtualizingStackPanel`/virtualization (enabled by default in `DataGrid`/`ListView`) for any list that can exceed ~50 rows (patient lists, product catalogs, transaction history).
- Avoid deeply nested `Grid`/`Border`/`StackPanel` chains — flatten layout where possible; excessive visual tree depth slows rendering on lower-spec clinic hardware.
- Freeze `Freezable` resources (brushes, geometries) that don't change at runtime (`Brush.Freeze()` in code, or rely on `DynamicResource` brushes being frozen once at theme load).
- Avoid `Converter`-heavy bindings in hot paths (e.g. large DataGrids) — precompute formatted values in the ViewModel/Model where feasible instead of converting per-cell on every render.
- Use `x:Shared="False"` sparingly and only when a resource genuinely must not be shared (most Style/Brush resources should remain shared for memory efficiency).
