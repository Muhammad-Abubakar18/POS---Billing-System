using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Inventory;

public sealed class InventoryForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IProductService _productService;
    
    private TextBox _txtSearch = null!;
    private DataGridView _grid = null!;

    public InventoryForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _productService = serviceProvider.GetRequiredService<IProductService>();

        InitializeComponent();
        Shown += async (s, e) => await LoadDataAsync();
    }

    private void InitializeComponent()
    {
        Text = "DrMusa — Inventory Management";
        BackColor = AppTheme.BackgroundDark;
        ForeColor = AppTheme.TextPrimary;
        Font = AppTheme.FontBody;

        // ── Header Panel ───────────────────────────────────────────
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100,
            BackColor = AppTheme.BackgroundPanel,
            Padding = new Padding(20)
        };

        var lblTitle = new Label
        {
            Text = "Inventory Management",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 15)
        };

        var lblSubtitle = new Label
        {
            Text = "Manage current stock, adjustments, and view history.",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 50)
        };

        _txtSearch = new TextBox
        {
            Width = 300,
            Font = AppTheme.FontInput
        };
        _txtSearch.TextChanged += async (s, e) => await SearchAsync();

        var pnlSearch = AppTheme.WrapInputPanel(_txtSearch, "Search Products...");
        pnlSearch.Width = 300;
        pnlSearch.Location = new Point(20, 80); // Wait, header is only 100 high.
        pnlSearch.Location = new Point(400, 30);

        var btnStockIn = new Button { Text = "Stock In", Width = 90, Height = 36, Location = new Point(610, 30) };
        AppTheme.StyleCustomColorButton(btnStockIn, AppTheme.ColorFromHex("#2E7D32")); // Greenish
        btnStockIn.Click += (s, e) => OpenMovementForm(Common.Enums.StockMovementType.StockIn);

        var btnStockOut = new Button { Text = "Stock Out", Width = 90, Height = 36, Location = new Point(710, 30) };
        AppTheme.StyleCustomColorButton(btnStockOut, AppTheme.AccentDanger);
        btnStockOut.Click += (s, e) => OpenMovementForm(Common.Enums.StockMovementType.StockOut);

        var btnAdjust = new Button { Text = "Adjust Stock", Width = 110, Height = 36, Location = new Point(810, 30) };
        AppTheme.StyleCustomColorButton(btnAdjust, AppTheme.ColorFromHex("#F57F17")); // Yellow/Orange
        btnAdjust.Click += (s, e) => OpenMovementForm(Common.Enums.StockMovementType.Adjustment);

        var btnMinStock = new Button { Text = "Set Min Stock", Width = 120, Height = 36, Location = new Point(930, 30) };
        AppTheme.StyleCustomColorButton(btnMinStock, AppTheme.ColorFromHex("#8E24AA")); // Purple
        btnMinStock.Click += async (s, e) => await OpenMinStockFormAsync();

        var btnHistory = new Button { Text = "History", Width = 90, Height = 36, Location = new Point(1060, 30) };
        AppTheme.StyleCustomColorButton(btnHistory, AppTheme.ColorFromHex("#0277BD")); // Blue
        btnHistory.Click += (s, e) => OpenHistoryForm();

        // Handle resizing/layout safely to prevent overlaps
        header.SizeChanged += (s, e) =>
        {
            btnHistory.Left = Math.Max(header.Width - btnHistory.Width - 20, 1060);
            btnMinStock.Left = btnHistory.Left - btnMinStock.Width - 10;
            btnAdjust.Left = btnMinStock.Left - btnAdjust.Width - 10;
            btnStockOut.Left = btnAdjust.Left - btnStockOut.Width - 10;
            btnStockIn.Left = btnStockOut.Left - btnStockIn.Width - 10;
            pnlSearch.Left = Math.Max(btnStockIn.Left - pnlSearch.Width - 20, lblTitle.Right + 20);
        };

        header.Controls.AddRange(new Control[] { lblTitle, lblSubtitle, pnlSearch, btnStockIn, btnStockOut, btnAdjust, btnMinStock, btnHistory });

        // ── DataGridView ───────────────────────────────────────────
        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = AppTheme.BackgroundDark,
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
            RowTemplate = { Height = 45 },
            ColumnHeadersHeight = 45,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundPanel,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.FontBodyBold,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                SelectionBackColor = AppTheme.BackgroundPanel
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

        _grid.CellFormatting += Grid_CellFormatting;

        var gridContainer = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20)
        };
        gridContainer.Controls.Add(_grid);

        Controls.Add(gridContainer);
        Controls.Add(header);
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var data = await _productService.GetAllAsync();
            BindGrid(data);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading inventory: {ex.Message}");
        }
    }

    private async Task SearchAsync()
    {
        if (_grid == null) return; // Prevent NullReferenceException if called during InitializeComponent
        
        string term = _txtSearch.Text.Trim();
        if (string.IsNullOrEmpty(term) || term == "Search Products...")
        {
            await LoadDataAsync();
            return;
        }

        try
        {
            var data = await _productService.SearchAsync(term);
            BindGrid(data);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching: {ex.Message}");
        }
    }

    private void BindGrid(IEnumerable<ProductDto> products)
    {
        var list = products.Select(p => new
        {
            p.Id,
            p.Name,
            Category = p.CategoryName,
            p.CurrentStock,
            p.MinimumStock,
            Status = p.CurrentStock <= p.MinimumStock ? "Low Stock" : "In Stock"
        }).ToList();

        _grid.DataSource = list;

        if (_grid.Columns["Id"] != null) _grid.Columns["Id"].Visible = false;
    }

    private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && _grid.Columns.Contains("Status"))
        {
            var statusCell = _grid.Rows[e.RowIndex].Cells["Status"];
            if (statusCell.Value?.ToString() == "Low Stock")
            {
                _grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = AppTheme.AccentDanger;
                _grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }
    }

    private void OpenMovementForm(Common.Enums.StockMovementType type)
    {
        int? productId = null;
        if (_grid.SelectedRows.Count > 0)
        {
            productId = (int)_grid.SelectedRows[0].Cells["Id"].Value;
        }

        using var form = new InventoryMovementForm(_serviceProvider, type, productId);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            _ = LoadDataAsync();
        }
    }

    private void OpenHistoryForm()
    {
        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a product to view its history.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int productId = (int)_grid.SelectedRows[0].Cells["Id"].Value;
        using var form = new InventoryHistoryForm(_serviceProvider, productId);
        form.ShowDialog(this);
    }
    private async Task OpenMinStockFormAsync()
    {
        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a product to adjust its minimum stock.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int productId = (int)_grid.SelectedRows[0].Cells["Id"].Value;
        
        using var form = new AdjustMinStockForm(_serviceProvider, productId);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }
}
