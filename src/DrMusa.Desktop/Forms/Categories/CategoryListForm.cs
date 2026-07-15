using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Categories;

/// <summary>
/// Full Category Management screen — Module 4.
/// Features: Search, Add, Edit, Soft-Delete categories.
/// </summary>
public sealed class CategoryListForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICategoryService _categoryService;

    private TextBox _txtSearch = null!;
    private DataGridView _gridCategories = null!;
    private Button _btnRefresh = null!;
    private Button _btnAdd = null!;
    private Button _btnEdit = null!;
    private Button _btnDelete = null!;
    private Label _lblCount = null!;

    public CategoryListForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _categoryService = serviceProvider.GetRequiredService<ICategoryService>();

        InitializeComponent();
        Shown += async (_, __) => await LoadCategoriesAsync();
    }

    private void InitializeComponent()
    {
        Text = "DrMusa — Category Management";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(900, 640);
        MinimumSize = new Size(800, 560);
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
            Text = "Category Management",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 10)
        };

        var lblSubtitle = new Label
        {
            Text = "Organise your menu into categories for easy POS navigation.",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 50)
        };

        // Search box
        _txtSearch = new TextBox { Width = 260 };
        var searchPanel = AppTheme.WrapInputPanel(_txtSearch, "Search categories...");
        searchPanel.Width = 260;

        // Toolbar buttons
        _btnRefresh = new Button { Text = "Refresh", Width = 90 };
        AppTheme.StyleSecondaryButton(_btnRefresh);
        _btnRefresh.Click += async (_, __) => await LoadCategoriesAsync();

        _btnAdd = new Button { Text = "+ Add Category", Width = 130 };
        AppTheme.StylePrimaryButton(_btnAdd);
        _btnAdd.Click += async (_, __) => await OpenEditorAsync();

        _btnEdit = new Button { Text = "Edit", Width = 80 };
        AppTheme.StyleSecondaryButton(_btnEdit);
        _btnEdit.Click += async (_, __) => await EditSelectedAsync();

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
        rightTools.Controls.AddRange(new Control[] { searchPanel, _btnRefresh, _btnAdd, _btnEdit, _btnDelete });

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
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 8)
        };
        statusBar.Controls.Add(_lblCount);

        // ── DataGridView ───────────────────────────────────────────
        _gridCategories = new DataGridView
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
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.FontBodyBold,
                Padding = new Padding(10, 0, 0, 0)
            },
            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundCard,
                ForeColor = AppTheme.TextPrimary,
                SelectionBackColor = AppTheme.AccentPrimary,
                SelectionForeColor = Color.White,
                Padding = new Padding(10, 0, 0, 0)
            },
            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundPanel,
                ForeColor = AppTheme.TextPrimary,
                SelectionBackColor = AppTheme.AccentPrimary,
                SelectionForeColor = Color.White,
                Padding = new Padding(10, 0, 0, 0)
            }
        };
        _gridCategories.CellDoubleClick += async (_, __) => await EditSelectedAsync();

        var content = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20, 16, 20, 0),
            BackColor = AppTheme.BackgroundDark
        };
        content.Controls.Add(_gridCategories);

        Controls.Add(content);
        Controls.Add(statusBar);
        Controls.Add(header);

        // Live search on each keystroke
        _txtSearch.TextChanged += async (_, __) => await LoadCategoriesAsync();

        // Right-click context menu
        AttachContextMenu();
    }

    // ── Data Loading ────────────────────────────────────────────────

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var search = _txtSearch.Text.Trim();
            var isPlaceholder = search == "Search categories...";

            var categories = (!string.IsNullOrWhiteSpace(search) && !isPlaceholder)
                ? await _categoryService.SearchAsync(search)
                : await _categoryService.GetAllAsync();

            _gridCategories.DataSource = categories.Select(c => new
            {
                c.Id,
                c.Name,
                Description = c.Description ?? "—",
                Products = c.ProductCount,
                Status = c.IsActive ? "Active" : "Inactive"
            }).ToList();

            // Hide internal Id column
            if (_gridCategories.Columns["Id"] is { } col)
                col.Visible = false;

            // Column sizing hints
            if (_gridCategories.Columns["Name"] is { } colName)
                colName.FillWeight = 30;
            if (_gridCategories.Columns["Description"] is { } colDesc)
                colDesc.FillWeight = 50;
            if (_gridCategories.Columns["Products"] is { } colProd)
            {
                colProd.FillWeight = 10;
                colProd.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (_gridCategories.Columns["Status"] is { } colStatus)
            {
                colStatus.FillWeight = 10;
                colStatus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            var total = _gridCategories.RowCount;
            _lblCount.Text = $"{total} categor{(total == 1 ? "y" : "ies")} found";
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to load categories: {ex.Message}");
        }
    }

    // ── Helpers ─────────────────────────────────────────────────────

    private int? GetSelectedId()
    {
        if (_gridCategories.CurrentRow?.DataBoundItem is null) return null;
        return _gridCategories.CurrentRow.Cells["Id"].Value is int id ? id : null;
    }

    private async Task OpenEditorAsync(int? categoryId = null)
    {
        using var editor = new CategoryEditForm(_serviceProvider, categoryId);
        if (editor.ShowDialog(this) == DialogResult.OK)
            await LoadCategoriesAsync();
    }

    private async Task EditSelectedAsync()
    {
        var id = GetSelectedId();
        if (id is null)
        {
            UIHelper.ShowWarning("Please select a category to edit.");
            return;
        }
        await OpenEditorAsync(id.Value);
    }

    private async Task DeleteSelectedAsync()
    {
        var id = GetSelectedId();
        if (id is null)
        {
            UIHelper.ShowWarning("Please select a category to delete.");
            return;
        }

        var name = _gridCategories.CurrentRow?.Cells["Name"].Value?.ToString() ?? "this category";

        if (!UIHelper.Confirm(
                $"Soft-delete category \"{name}\"?\n\nThis will hide it from the system. Products in this category must be reassigned first.",
                "Delete Category"))
            return;

        try
        {
            await _categoryService.DeleteAsync(id.Value);
            UIHelper.ShowSuccess($"Category \"{name}\" was deleted successfully.");
            await LoadCategoriesAsync();
        }
        catch (InvalidOperationException ex)
        {
            // Business rule: category still has active products
            UIHelper.ShowWarning(ex.Message);
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Delete failed: {ex.Message}");
        }
    }

    // ── Context Menu ─────────────────────────────────────────────────

    private void AttachContextMenu()
    {
        var menu = new ContextMenuStrip();

        var miEdit = new ToolStripMenuItem("✏️  Edit Category");
        miEdit.Click += async (_, __) => await EditSelectedAsync();

        var miDelete = new ToolStripMenuItem("🗑️  Delete Category");
        miDelete.Click += async (_, __) => await DeleteSelectedAsync();

        var miRefresh = new ToolStripMenuItem("🔄  Refresh");
        miRefresh.Click += async (_, __) => await LoadCategoriesAsync();

        menu.Items.AddRange(new ToolStripItem[] { miEdit, miDelete, new ToolStripSeparator(), miRefresh });
        _gridCategories.ContextMenuStrip = menu;

        // Only show context menu when a row is selected
        menu.Opening += (_, e) =>
        {
            e.Cancel = GetSelectedId() is null;
        };
    }
}
