using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Suppliers;

/// <summary>
/// Full Supplier Management screen — Module 6.
/// Features: Search, Add, Edit, Delete suppliers.
/// </summary>
public sealed class SupplierListForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISupplierService _supplierService;

    private TextBox _txtSearch = null!;
    private DataGridView _gridSuppliers = null!;
    private Button _btnRefresh = null!;
    private Button _btnAdd = null!;
    private Button _btnEdit = null!;
    private Button _btnDelete = null!;
    private Button _btnToggleStatus = null!;
    private Label _lblCount = null!;

    public SupplierListForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _supplierService = serviceProvider.GetRequiredService<ISupplierService>();

        InitializeComponent();
        Shown += async (_, __) => await LoadSuppliersAsync();
    }

    private void InitializeComponent()
    {
        Text = "DrMusa — Supplier Management";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1000, 640);
        MinimumSize = new Size(900, 560);
        BackColor = AppTheme.BackgroundDark;
        ForeColor = AppTheme.TextPrimary;
        Font = AppTheme.FontBody;

        // ── Header Panel ───────────────────────────────────────────
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = AppTheme.BackgroundPanel,
            Padding = new Padding(20, 14, 20, 14)
        };

        var lblTitle = new Label
        {
            Text = "Supplier Management",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 10)
        };

        var lblSubtitle = new Label
        {
            Text = "Manage your product suppliers and view their details.",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(20, 50)
        };

        // Search box
        _txtSearch = new TextBox { Width = 260 };
        var searchPanel = AppTheme.WrapInputPanel(_txtSearch, "Search suppliers...");
        searchPanel.Width = 260;
        _txtSearch.TextChanged += async (_, __) => await FilterSuppliersAsync();

        // Toolbar buttons
        _btnRefresh = new Button { Text = "Refresh", Width = 90 };
        AppTheme.StyleSecondaryButton(_btnRefresh);
        _btnRefresh.Click += async (_, __) => await LoadSuppliersAsync();

        _btnAdd = new Button { Text = "+ Add Supplier", Width = 130 };
        AppTheme.StylePrimaryButton(_btnAdd);
        _btnAdd.Click += async (_, __) => await OpenEditorAsync();

        _btnEdit = new Button { Text = "Edit", Width = 80 };
        AppTheme.StyleSecondaryButton(_btnEdit);
        _btnEdit.Click += async (_, __) => await EditSelectedAsync();

        _btnToggleStatus = new Button { Text = "Toggle Active", Width = 110 };
        AppTheme.StyleSecondaryButton(_btnToggleStatus);
        _btnToggleStatus.Click += async (_, __) => await ToggleSelectedStatusAsync();

        _btnDelete = new Button { Text = "Delete", Width = 80 };
        AppTheme.StyleDangerButton(_btnDelete);
        _btnDelete.Click += async (_, __) => await DeleteSelectedAsync();

        var rightTools = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Right,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 2, 0, 0)
        };
        rightTools.Controls.AddRange(new Control[] { searchPanel, _btnRefresh, _btnAdd, _btnEdit, _btnToggleStatus, _btnDelete });

        header.Controls.AddRange(new Control[]
        {
            lblTitle, lblSubtitle, rightTools
        });

        // ── Status bar ─────────────────────────────────────────────
        var statusBar = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 32,
            BackColor = AppTheme.BackgroundPanel,
            Padding = new Padding(20, 6, 20, 6)
        };

        _lblCount = new Label
        {
            Text = "Loading...",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(20, 8)
        };
        statusBar.Controls.Add(_lblCount);

        // ── DataGridView ───────────────────────────────────────────
        _gridSuppliers = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = AppTheme.BackgroundCard,
            BorderStyle = BorderStyle.None,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            RowTemplate = { Height = 40 },
            ColumnHeadersHeight = 40,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundDark,
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontBodyBold,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                SelectionBackColor = AppTheme.BackgroundDark
            },
            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundCard,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.FontBody,
                SelectionBackColor = AppTheme.AccentPrimary,
                SelectionForeColor = Color.White,
                Padding = new Padding(10, 0, 10, 0)
            }
        };

        _gridSuppliers.CellFormatting += GridSuppliers_CellFormatting;
        _gridSuppliers.CellDoubleClick += async (_, __) => await EditSelectedAsync();

        var gridContainer = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20)
        };
        gridContainer.Controls.Add(_gridSuppliers);

        Controls.Add(gridContainer);
        Controls.Add(statusBar);
        Controls.Add(header);
    }

    private void GridSuppliers_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.Value == null) return;
        var grid = (DataGridView)sender!;

        if (grid.Columns[e.ColumnIndex].Name == "IsActive")
        {
            bool isActive = (bool)e.Value;
            e.Value = isActive ? "Active" : "Inactive";
            e.FormattingApplied = true;

            if (!isActive)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = AppTheme.AccentDanger;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }
    }

    private IEnumerable<SupplierDto> _allSuppliers = new List<SupplierDto>();

    private async Task LoadSuppliersAsync()
    {
        try
        {
            _allSuppliers = await _supplierService.GetAllAsync();
            await FilterSuppliersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading suppliers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Task FilterSuppliersAsync()
    {
        var searchText = _txtSearch.Text.Trim().ToLower();
        var filtered = string.IsNullOrEmpty(searchText)
            ? _allSuppliers
            : _allSuppliers.Where(s =>
                s.Name.ToLower().Contains(searchText) ||
                (s.ContactPerson?.ToLower().Contains(searchText) ?? false) ||
                (s.Phone?.ToLower().Contains(searchText) ?? false) ||
                (s.Email?.ToLower().Contains(searchText) ?? false)
            );

        var list = filtered.ToList();

        _gridSuppliers.DataSource = null;
        _gridSuppliers.DataSource = list;

        if (_gridSuppliers.Columns["Id"] != null)
            _gridSuppliers.Columns["Id"].Visible = false;
        
        if (_gridSuppliers.Columns["CreatedAt"] != null)
            _gridSuppliers.Columns["CreatedAt"].DefaultCellStyle.Format = "dd MMM yyyy";

        if (_gridSuppliers.Columns["Name"] != null)
            _gridSuppliers.Columns["Name"].HeaderText = "Supplier Name";
        
        if (_gridSuppliers.Columns["ContactPerson"] != null)
            _gridSuppliers.Columns["ContactPerson"].HeaderText = "Contact Person";

        _lblCount.Text = $"Showing {list.Count} suppliers.";

        return Task.CompletedTask;
    }

    private async Task OpenEditorAsync(SupplierDto? existingSupplier = null)
    {
        using var editor = new SupplierEditForm(existingSupplier);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                if (existingSupplier == null)
                {
                    await _supplierService.CreateAsync(editor.SupplierDto);
                }
                else
                {
                    await _supplierService.UpdateAsync(editor.SupplierDto);
                }
                await LoadSuppliersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    private async Task EditSelectedAsync()
    {
        if (_gridSuppliers.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a supplier to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var supplier = (SupplierDto)_gridSuppliers.SelectedRows[0].DataBoundItem;
        await OpenEditorAsync(supplier);
    }

    private async Task ToggleSelectedStatusAsync()
    {
        if (_gridSuppliers.SelectedRows.Count == 0) return;

        var supplier = (SupplierDto)_gridSuppliers.SelectedRows[0].DataBoundItem;
        var action = supplier.IsActive ? "deactivate" : "activate";

        if (MessageBox.Show($"Are you sure you want to {action} '{supplier.Name}'?", "Confirm Action",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                await _supplierService.ToggleStatusAsync(supplier.Id);
                await LoadSuppliersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async Task DeleteSelectedAsync()
    {
        if (_gridSuppliers.SelectedRows.Count == 0) return;

        var supplier = (SupplierDto)_gridSuppliers.SelectedRows[0].DataBoundItem;

        if (MessageBox.Show($"Are you sure you want to permanently delete '{supplier.Name}'?\n\nConsider deactivating instead if they have past purchases.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            try
            {
                await _supplierService.DeleteAsync(supplier.Id);
                await LoadSuppliersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
