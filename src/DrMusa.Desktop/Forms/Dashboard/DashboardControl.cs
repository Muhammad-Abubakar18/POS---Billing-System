using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DrMusa.Desktop.Forms.Dashboard;

public partial class DashboardControl : UserControl
{
    private readonly ISaleService _saleService;

    // Stat labels
    private Label _lblTodaySales = null!;
    private Label _lblMonthlySales = null!;
    private Label _lblTotalProducts = null!;

    private Label _lblLowStock = null!;

    // Grids
    private DataGridView _gridRecent = null!;
    private DataGridView _gridLowStock = null!;
    private DataGridView _gridTopSelling = null!;

    // Chart
    private CartesianChart _chartPanel = null!;

    public DashboardControl(ISaleService saleService)
    {
        _saleService = saleService;
        BuildUI();
        Dock = DockStyle.Fill;
    }

    private void BuildUI()
    {
        BackColor = AppTheme.BackgroundDark;
        Padding = new Padding(24);
        AutoScroll = false;

        // Dashboard heading
        var lblDash = new Label
        {
            Text = "Dashboard",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true,
        };
        
        var lblDate = new Label
        {
            Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy"),
            Font = new Font("Segoe UI", 9f),
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true
        };

        lblDash.Location = new Point(0, 0);
        lblDate.Location = new Point(0, 42);
        var headerPanel = new Panel { Dock = DockStyle.Fill, Height = 80, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 16) };
        headerPanel.Controls.AddRange(new Control[] { lblDash, lblDate });

        var statsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            Margin = new Padding(0, 0, 0, 16)
        };

        var stats = new[]
        {
            ("Today's Sales",   AppTheme.AccentPrimary,  "₨"),
            ("Monthly Sales",   AppTheme.AccentSuccess,  "📈"),
            ("Total Products",  AppTheme.AccentWarning,  "📦"),
            ("Low Stock",       AppTheme.AccentDanger,   "⚠"),
        };

        _lblTodaySales = new Label();
        _lblMonthlySales = new Label();
        _lblTotalProducts = new Label();
        _lblLowStock = new Label();

        var valueLabels = new[] { _lblTodaySales, _lblMonthlySales, _lblTotalProducts, _lblLowStock };

        for (int i = 0; i < stats.Length; i++)
        {
            var (title, accent, icon) = stats[i];
            var card = BuildStatCard(title, icon, accent, valueLabels[i]);
            card.Width = 240; // Make cards wider now that there are fewer of them
            statsPanel.Controls.Add(card);
        }

        var layoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            BackColor = Color.Transparent,
            Margin = new Padding(0)
        };
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        _chartPanel = new CartesianChart { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 10), BackColor = AppTheme.BackgroundCard };

        _gridRecent = CreateDataGrid();
        _gridLowStock = CreateDataGrid();
        _gridTopSelling = CreateDataGrid();

        layoutPanel.Controls.Add(CreateSectionPanel("Sales Last 7 Days", _chartPanel), 0, 0);
        layoutPanel.Controls.Add(CreateSectionPanel("Recent Transactions", _gridRecent), 1, 0);
        layoutPanel.Controls.Add(CreateSectionPanel("Top Selling Products", _gridTopSelling), 0, 1);
        layoutPanel.Controls.Add(CreateSectionPanel("Low Stock Alerts", _gridLowStock), 1, 1);

        var rootTable = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.Transparent,
            Padding = new Padding(24)
        };
        rootTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        rootTable.Controls.Add(headerPanel, 0, 0);
        rootTable.Controls.Add(statsPanel, 0, 1);
        rootTable.Controls.Add(layoutPanel, 0, 2);

        Controls.Add(rootTable);
    }

    private Panel CreateSectionPanel(string title, Control content)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.BackgroundCard,
            Margin = new Padding(8)
        };
        
        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = AppTheme.TextPrimary,
            Location = new Point(10, 10),
            AutoSize = true
        };

        content.Location = new Point(10, 40);
        content.Width = panel.Width - 20;
        content.Height = panel.Height - 50;
        content.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        panel.Controls.Add(lblTitle);
        panel.Controls.Add(content);

        // Border
        panel.Paint += (s, e) =>
        {
            using var pen = new Pen(AppTheme.BorderDefault, 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
        };

        return panel;
    }

    private DataGridView CreateDataGrid()
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = AppTheme.BackgroundCard,
            BorderStyle = BorderStyle.None,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            EnableHeadersVisualStyles = false,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
            GridColor = AppTheme.BorderDefault,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundCard,
                ForeColor = AppTheme.TextPrimary,
                SelectionBackColor = AppTheme.AccentPrimary,
                SelectionForeColor = Color.White
            },
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundDark,
                ForeColor = AppTheme.TextPrimary,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            }
        };
    }

    private Panel BuildStatCard(string title, string icon, Color accent, Label valueLabel)
    {
        var card = new Panel
        {
            Size = new Size(184, 120),
            BackColor = AppTheme.BackgroundCard,
            Margin = new Padding(0, 0, 16, 0)
        };
        card.Paint += (s, e) =>
        {
            using var accentBrush = new SolidBrush(accent);
            e.Graphics.FillRectangle(accentBrush, 0, 0, 4, card.Height);

            using var borderPen = new Pen(AppTheme.BorderDefault, 1f);
            e.Graphics.DrawRectangle(borderPen, 0, 0, card.Width - 1, card.Height - 1);
        };

        var lblIcon = new Label
        {
            Text = icon,
            Font = new Font("Segoe UI Emoji", 14f),
            ForeColor = accent,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(16, 12)
        };

        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(16, 85)
        };

        valueLabel.Text = "—";
        valueLabel.Font = new Font("Segoe UI Variable", 16f, FontStyle.Bold);
        valueLabel.ForeColor = AppTheme.TextPrimary;
        valueLabel.BackColor = Color.Transparent;
        valueLabel.AutoSize = true;
        valueLabel.Location = new Point(16, 48);

        card.Controls.AddRange(new Control[] { lblIcon, valueLabel, lblTitle });
        return card;
    }

    public async Task LoadDataAsync()
    {
        try
        {
            var data = await _saleService.GetDashboardDataAsync();

            if (InvokeRequired)
            {
                Invoke(() => UpdateUI(data));
            }
            else
            {
                UpdateUI(data);
            }
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Dashboard load error: {ex.Message}");
        }
    }

    private void UpdateUI(DashboardDto data)
    {
        _lblTodaySales.Text = UIHelper.FormatCurrency(data.TodaySales);
        _lblMonthlySales.Text = UIHelper.FormatCurrency(data.MonthlySales);
        _lblTotalProducts.Text = data.TotalProducts.ToString("N0");
        _lblLowStock.Text = data.LowStockCount.ToString();
        _lblLowStock.ForeColor = data.LowStockCount > 0 ? AppTheme.AccentDanger : AppTheme.AccentSuccess;

        // Bind Grids
        _gridRecent.DataSource = data.RecentTransactions.Select(t => new
        {
            t.InvoiceNumber,
            Date = t.SaleDate.ToString("MMM dd HH:mm"),
            Total = UIHelper.FormatCurrency(t.TotalAmount)
        }).ToList();

        _gridLowStock.DataSource = data.LowStockProducts.Select(p => new
        {
            p.Name,
            Category = p.CategoryName ?? "—"
        }).ToList();

        _gridTopSelling.DataSource = data.TopSellingProducts.Select(p => new
        {
            p.ProductName,
            Sold = p.TotalQuantitySold,
            Revenue = UIHelper.FormatCurrency(p.TotalRevenue)
        }).ToList();

        // Bind Chart
        _chartPanel.Series = new ISeries[]
        {
            new ColumnSeries<decimal>
            {
                Values = data.SalesGraphData.Select(d => d.Amount).ToArray(),
                Name = "Sales",
                Fill = new SolidColorPaint(new SKColor(10, 130, 140)), // Match AccentPrimary
                DataLabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top
            }
        };

        _chartPanel.XAxes = new[]
        {
            new Axis
            {
                Labels = data.SalesGraphData.Select(d => d.Date.ToString("dd MMM")).ToArray(),
                LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200))
            }
        };

        _chartPanel.YAxes = new[]
        {
            new Axis
            {
                LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200))
            }
        };
    }
}
