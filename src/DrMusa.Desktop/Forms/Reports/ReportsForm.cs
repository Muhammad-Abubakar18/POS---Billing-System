using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Data;

namespace DrMusa.Desktop.Forms.Reports;

/// <summary>
/// Full Reports screen — Module 8.
/// Features: Analytics Dashboard & Detailed Data Grids.
/// </summary>
public sealed class ReportsForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IReportService _reportService;

    // Filters
    private DateTimePicker _dtpStart = null!;
    private DateTimePicker _dtpEnd = null!;
    private ComboBox _cmbGrouping = null!;
    private Button _btnGenerate = null!;
    private ComboBox _cmbReportType = null!; // For Grid Tab

    // Dashboard Elements
    private Label _lblGrandTotal = null!;
    private Label _lblRawProfit = null!;
    private Label _lblTax = null!;
    private Label _lblDiscount = null!;
    
    private CartesianChart _trendChart = null!;
    private CartesianChart _topProductsChart = null!;
    private CartesianChart _lowProductsChart = null!;

    // Grid Elements
    private DataGridView _gridReports = null!;
    private Label _lblGridSummary = null!;

    public ReportsForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _reportService = serviceProvider.GetRequiredService<IReportService>();

        InitializeComponent();
        Shown += async (_, __) => await GenerateDashboardAsync();
    }

    private void InitializeComponent()
    {
        Text = "DrMusa — Analytics & Reports";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1200, 800);
        MinimumSize = new Size(1000, 700);
        BackColor = AppTheme.BackgroundDark;
        ForeColor = AppTheme.TextPrimary;
        Font = AppTheme.FontBody;

        // ── Header Panel ───────────────────────────────────────────
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 110,
            BackColor = AppTheme.BackgroundPanel,
            Padding = new Padding(20, 14, 20, 14)
        };

        var lblTitle = new Label
        {
            Text = "Business Analytics",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 10)
        };

        var lblSubtitle = new Label
        {
            Text = "Monitor your business performance, profit trends, and top products.",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(20, 50)
        };

        // Filter Controls
        var filterPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Location = new Point(20, 70),
            BackColor = Color.Transparent,
            Padding = new Padding(0)
        };

        _dtpStart = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Width = 130,
            Font = AppTheme.FontInput,
            Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) // Start of month
        };

        _dtpEnd = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Width = 130,
            Font = AppTheme.FontInput,
            Value = DateTime.Today
        };

        _cmbGrouping = new ComboBox
        {
            Width = 120,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = AppTheme.FontInput
        };
        _cmbGrouping.Items.AddRange(new string[] { "Daily", "Weekly", "Monthly" });
        _cmbGrouping.SelectedIndex = 0;

        _btnGenerate = new Button { Text = "Refresh", Width = 100, Height = 32 };
        AppTheme.StylePrimaryButton(_btnGenerate);
        _btnGenerate.Click += async (_, __) => { await GenerateDashboardAsync(); await GenerateGridReportAsync(); };

        filterPanel.Controls.Add(new Label { Text = "From:", AutoSize = true, Padding = new Padding(0, 6, 0, 0) });
        filterPanel.Controls.Add(_dtpStart);
        filterPanel.Controls.Add(new Label { Text = "To:", AutoSize = true, Padding = new Padding(10, 6, 0, 0) });
        filterPanel.Controls.Add(_dtpEnd);
        filterPanel.Controls.Add(new Label { Text = "Trend By:", AutoSize = true, Padding = new Padding(10, 6, 0, 0) });
        filterPanel.Controls.Add(_cmbGrouping);
        filterPanel.Controls.Add(new Panel { Width = 10 }); // Spacer
        filterPanel.Controls.Add(_btnGenerate);

        header.Controls.Add(lblTitle);
        header.Controls.Add(lblSubtitle);
        header.Controls.Add(filterPanel);
        
        Controls.Add(header);

        // ── Tabs ───────────────────────────────────────────
        var tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = AppTheme.FontBodyBold,
            Padding = new Point(20, 10)
        };
        
        var tabDashboard = new TabPage("Visual Dashboard") { BackColor = AppTheme.BackgroundDark };
        var tabReports = new TabPage("Detailed Grid Reports") { BackColor = AppTheme.BackgroundDark };

        tabControl.TabPages.Add(tabDashboard);
        tabControl.TabPages.Add(tabReports);
        Controls.Add(tabControl);
        tabControl.BringToFront();

        // ====== BUILD DASHBOARD TAB ======
        BuildDashboardTab(tabDashboard);

        // ====== BUILD REPORTS TAB ======
        BuildGridReportsTab(tabReports);
    }

    private void BuildDashboardTab(TabPage page)
    {
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(10)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F)); // Summary Cards
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Charts

        // Summary Cards Container
        var cardsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            Margin = new Padding(0)
        };
        for (int i = 0; i < 4; i++) cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

        _lblGrandTotal = CreateSummaryCard(cardsLayout, 0, "Grand Total Revenue", AppTheme.AccentPrimary);
        _lblRawProfit = CreateSummaryCard(cardsLayout, 1, "Raw Profit (No Tax/Disc)", AppTheme.AccentSuccess);
        _lblTax = CreateSummaryCard(cardsLayout, 2, "Total Tax Collected", AppTheme.AccentWarning);
        _lblDiscount = CreateSummaryCard(cardsLayout, 3, "Total Discount Given", AppTheme.AccentDanger);

        mainLayout.Controls.Add(cardsLayout, 0, 0);
        mainLayout.SetColumnSpan(cardsLayout, 2);

        // Trend Chart
        var trendPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        _trendChart = new CartesianChart { Dock = DockStyle.Fill };
        trendPanel.Controls.Add(_trendChart);
        mainLayout.Controls.Add(trendPanel, 0, 1);

        // Right side for Products Charts
        var productsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        productsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        productsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        var topPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        topPanel.Controls.Add(new Label { Text = "High Selling Products", Dock = DockStyle.Top, Font = AppTheme.FontBodyBold });
        _topProductsChart = new CartesianChart { Dock = DockStyle.Fill };
        topPanel.Controls.Add(_topProductsChart);

        var lowPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        lowPanel.Controls.Add(new Label { Text = "Low Selling Products", Dock = DockStyle.Top, Font = AppTheme.FontBodyBold });
        _lowProductsChart = new CartesianChart { Dock = DockStyle.Fill };
        lowPanel.Controls.Add(_lowProductsChart);

        productsLayout.Controls.Add(topPanel, 0, 0);
        productsLayout.Controls.Add(lowPanel, 0, 1);
        mainLayout.Controls.Add(productsLayout, 1, 1);

        page.Controls.Add(mainLayout);
    }

    private Label CreateSummaryCard(TableLayoutPanel parent, int column, string title, Color color)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.BackgroundCard,
            Margin = new Padding(10)
        };

        var lblTitle = new Label
        {
            Text = title,
            ForeColor = AppTheme.TextSecondary,
            Font = AppTheme.FontSmall,
            Location = new Point(15, 15),
            AutoSize = true
        };

        var lblValue = new Label
        {
            Text = "PKR 0.00",
            ForeColor = color,
            Font = AppTheme.FontHeading,
            Location = new Point(15, 45),
            AutoSize = true
        };

        card.Controls.Add(lblTitle);
        card.Controls.Add(lblValue);
        parent.Controls.Add(card, column, 0);
        
        return lblValue;
    }

    private void BuildGridReportsTab(TabPage page)
    {
        var topBar = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
        _cmbReportType = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = AppTheme.FontInput,
            Width = 200,
            Location = new Point(10, 15)
        };
        _cmbReportType.Items.AddRange(new string[] { "Daily Sales Report", "Profit Report", "Inventory Report", "Low Stock Report" });
        _cmbReportType.SelectedIndex = 0;
        _cmbReportType.SelectedIndexChanged += async (_, __) => await GenerateGridReportAsync();
        
        _lblGridSummary = new Label
        {
            AutoSize = true,
            Font = AppTheme.FontBodyBold,
            Location = new Point(230, 20),
            ForeColor = AppTheme.AccentPrimary
        };

        topBar.Controls.Add(_cmbReportType);
        topBar.Controls.Add(_lblGridSummary);

        _gridReports = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = AppTheme.BackgroundDark,
            BorderStyle = BorderStyle.None,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            RowTemplate = { Height = 40 },
            ColumnHeadersHeight = 40,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundPanel,
                ForeColor = AppTheme.TextSecondary,
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

        page.Controls.Add(_gridReports);
        page.Controls.Add(topBar);
    }

    private async Task GenerateDashboardAsync()
    {
        DateTime start = _dtpStart.Value.Date;
        DateTime end = _dtpEnd.Value.Date;
        var grouping = (ReportGroupingType)_cmbGrouping.SelectedIndex;

        var summary = await _reportService.GetDashboardSummaryAsync(start, end);
        _lblGrandTotal.Text = UIHelper.FormatCurrency(summary.GrandTotal);
        _lblRawProfit.Text = UIHelper.FormatCurrency(summary.RawProfit);
        _lblTax.Text = UIHelper.FormatCurrency(summary.TotalTax);
        _lblDiscount.Text = UIHelper.FormatCurrency(summary.TotalDiscount);

        var trendData = await _reportService.GetProfitTrendAsync(start, end, grouping);
        _trendChart.Series = new ISeries[]
        {
            new LineSeries<decimal>
            {
                Values = trendData.Select(t => t.Profit).ToArray(),
                Name = "Net Profit (After Disc)",
                Fill = new SolidColorPaint(new SKColor(46, 204, 113, 90)), // Semi-transparent green
                Stroke = new SolidColorPaint(new SKColor(46, 204, 113)) { StrokeThickness = 3 },
                GeometrySize = 10
            }
        };
        _trendChart.XAxes = new[]
        {
            new Axis
            {
                Labels = trendData.Select(t => t.Label).ToArray(),
                LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200))
            }
        };
        _trendChart.YAxes = new[] { new Axis { LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200)) } };

        var topProducts = await _reportService.GetTopSellingProductsAsync(start, end, 5);
        _topProductsChart.Series = new ISeries[]
        {
            new RowSeries<int>
            {
                Values = topProducts.Select(p => p.TotalQuantitySold).ToArray(),
                Name = "Quantity Sold",
                Fill = new SolidColorPaint(new SKColor(52, 152, 219)),
                DataLabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.End
            }
        };
        _topProductsChart.YAxes = new[]
        {
            new Axis
            {
                Labels = topProducts.Select(p => p.ProductName).ToArray(),
                LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200))
            }
        };

        var lowProducts = await _reportService.GetLowSellingProductsAsync(start, end, 5);
        _lowProductsChart.Series = new ISeries[]
        {
            new RowSeries<int>
            {
                Values = lowProducts.Select(p => p.TotalQuantitySold).ToArray(),
                Name = "Quantity Sold",
                Fill = new SolidColorPaint(new SKColor(231, 76, 60)),
                DataLabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.End
            }
        };
        _lowProductsChart.YAxes = new[]
        {
            new Axis
            {
                Labels = lowProducts.Select(p => p.ProductName).ToArray(),
                LabelsPaint = new SolidColorPaint(new SKColor(200, 200, 200))
            }
        };
    }

    private async Task GenerateGridReportAsync()
    {
        DateTime start = _dtpStart.Value.Date;
        DateTime end = _dtpEnd.Value.Date;
        int reportIndex = _cmbReportType.SelectedIndex;

        _gridReports.DataSource = null;
        _lblGridSummary.Text = "";

        if (reportIndex == 0) // Daily Sales
        {
            var data = await _reportService.GetDailySalesReportAsync(start, end);
            var list = data.ToList();
            _gridReports.DataSource = list;
            _lblGridSummary.Text = $"Total Invoices: {list.Sum(x => x.TotalInvoices)} | Net Sales: {list.Sum(x => x.NetAmount):C2}";
            if (_gridReports.Columns["Date"] != null) _gridReports.Columns["Date"].DefaultCellStyle.Format = "dd MMM yyyy";
            FormatCurrencyColumns("TotalSales", "TotalDiscount", "TotalTax", "NetAmount");
        }
        else if (reportIndex == 1) // Profit
        {
            var data = await _reportService.GetDailyProfitReportAsync(start, end);
            var list = data.ToList();
            _gridReports.DataSource = list;
            _lblGridSummary.Text = $"Total Profit: {list.Sum(x => x.Profit):C2}";
            if (_gridReports.Columns["Date"] != null) _gridReports.Columns["Date"].DefaultCellStyle.Format = "dd MMM yyyy";
            FormatCurrencyColumns("Revenue", "Cost", "Profit");
        }
        else if (reportIndex == 2) // Inventory
        {
            var data = await _reportService.GetInventoryReportAsync();
            _gridReports.DataSource = data.ToList();
            FormatCurrencyColumns("PurchasePrice");
        }
        else if (reportIndex == 3) // Low Stock
        {
            var data = await _reportService.GetLowStockReportAsync();
            _gridReports.DataSource = data.ToList();
            FormatCurrencyColumns("PurchasePrice");
        }
    }

    private void FormatCurrencyColumns(params string[] colNames)
    {
        foreach (var name in colNames)
        {
            if (_gridReports.Columns[name] != null)
            {
                _gridReports.Columns[name].DefaultCellStyle.Format = "N2";
                _gridReports.Columns[name].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
    }
}
