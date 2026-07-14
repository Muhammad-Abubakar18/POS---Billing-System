using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Reports;

/// <summary>
/// Full Reports screen — Module 8.
/// Features: Sales, Profit, Inventory, and Low Stock Reports with date filtering.
/// </summary>
public sealed class ReportsForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IReportService _reportService;

    private ComboBox _cmbReportType = null!;
    private DateTimePicker _dtpStart = null!;
    private DateTimePicker _dtpEnd = null!;
    private Button _btnGenerate = null!;
    private DataGridView _gridReports = null!;
    
    private Label _lblSummaryTitle = null!;
    private Label _lblSummaryValue1 = null!;
    private Label _lblSummaryValue2 = null!;

    public ReportsForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _reportService = serviceProvider.GetRequiredService<IReportService>();

        InitializeComponent();
        Shown += async (_, __) => await GenerateReportAsync();
    }

    private void InitializeComponent()
    {
        Text = "DrMusa — Reports";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1100, 700);
        MinimumSize = new Size(900, 600);
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
            Text = "Reports & Analytics",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 10)
        };

        var lblSubtitle = new Label
        {
            Text = "View your sales, profit, and inventory data.",
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

        _cmbReportType = new ComboBox
        {
            Width = 200,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = AppTheme.FontInput
        };
        _cmbReportType.Items.AddRange(new string[] { "Daily Sales Report", "Profit Report", "Inventory Report", "Low Stock Report" });
        _cmbReportType.SelectedIndex = 0;
        _cmbReportType.SelectedIndexChanged += (_, __) => ToggleDatePickers();

        _dtpStart = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Width = 130,
            Font = AppTheme.FontInput,
            Value = DateTime.Today.AddDays(-30)
        };

        _dtpEnd = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Width = 130,
            Font = AppTheme.FontInput,
            Value = DateTime.Today
        };

        _btnGenerate = new Button { Text = "Generate", Width = 100, Height = 32 };
        AppTheme.StylePrimaryButton(_btnGenerate);
        _btnGenerate.Click += async (_, __) => await GenerateReportAsync();

        filterPanel.Controls.Add(new Label { Text = "Report:", AutoSize = true, Padding = new Padding(0, 6, 0, 0) });
        filterPanel.Controls.Add(_cmbReportType);
        filterPanel.Controls.Add(new Label { Text = "From:", AutoSize = true, Padding = new Padding(10, 6, 0, 0) });
        filterPanel.Controls.Add(_dtpStart);
        filterPanel.Controls.Add(new Label { Text = "To:", AutoSize = true, Padding = new Padding(10, 6, 0, 0) });
        filterPanel.Controls.Add(_dtpEnd);
        filterPanel.Controls.Add(new Panel { Width = 10 }); // Spacer
        filterPanel.Controls.Add(_btnGenerate);

        header.Controls.Add(lblTitle);
        header.Controls.Add(lblSubtitle);
        header.Controls.Add(filterPanel);

        // ── Summary Panel ──────────────────────────────────────────
        var summaryPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = AppTheme.BackgroundCard,
            Padding = new Padding(20)
        };

        _lblSummaryTitle = new Label { Font = AppTheme.FontHeading, ForeColor = AppTheme.AccentPrimary, AutoSize = true, Location = new Point(20, 15) };
        _lblSummaryValue1 = new Label { Font = AppTheme.FontBodyBold, AutoSize = true, Location = new Point(20, 45) };
        _lblSummaryValue2 = new Label { Font = AppTheme.FontBodyBold, AutoSize = true, Location = new Point(250, 45) };

        summaryPanel.Controls.Add(_lblSummaryTitle);
        summaryPanel.Controls.Add(_lblSummaryValue1);
        summaryPanel.Controls.Add(_lblSummaryValue2);

        // ── DataGridView ───────────────────────────────────────────
        _gridReports = new DataGridView
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
            RowTemplate = { Height = 40 },
            ColumnHeadersHeight = 40,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
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

        var gridContainer = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20)
        };
        gridContainer.Controls.Add(_gridReports);

        Controls.Add(gridContainer);
        Controls.Add(summaryPanel);
        Controls.Add(header);
    }

    private void ToggleDatePickers()
    {
        bool isDateReport = _cmbReportType.SelectedIndex == 0 || _cmbReportType.SelectedIndex == 1;
        _dtpStart.Enabled = isDateReport;
        _dtpEnd.Enabled = isDateReport;
    }

    private async Task GenerateReportAsync()
    {
        try
        {
            _btnGenerate.Enabled = false;
            _btnGenerate.Text = "Loading...";

            int reportIndex = _cmbReportType.SelectedIndex;
            DateTime start = _dtpStart.Value.Date;
            DateTime end = _dtpEnd.Value.Date;

            _gridReports.DataSource = null;

            if (reportIndex == 0) // Daily Sales
            {
                var data = await _reportService.GetDailySalesReportAsync(start, end);
                var list = data.ToList();
                _gridReports.DataSource = list;
                
                decimal totalNet = list.Sum(x => x.NetAmount);
                int totalInvoices = list.Sum(x => x.TotalInvoices);

                _lblSummaryTitle.Text = "Sales Summary";
                _lblSummaryValue1.Text = $"Total Invoices: {totalInvoices}";
                _lblSummaryValue2.Text = $"Total Net Sales: {totalNet:C2}";

                if (_gridReports.Columns["Date"] != null)
                    _gridReports.Columns["Date"].DefaultCellStyle.Format = "dd MMM yyyy";
                FormatCurrencyColumns("TotalSales", "TotalDiscount", "TotalTax", "NetAmount");
            }
            else if (reportIndex == 1) // Profit
            {
                var data = await _reportService.GetDailyProfitReportAsync(start, end);
                var list = data.ToList();
                _gridReports.DataSource = list;
                
                decimal totalProfit = list.Sum(x => x.Profit);
                decimal totalRevenue = list.Sum(x => x.Revenue);

                _lblSummaryTitle.Text = "Profit Summary";
                _lblSummaryValue1.Text = $"Total Revenue: {totalRevenue:C2}";
                _lblSummaryValue2.Text = $"Total Profit: {totalProfit:C2}";

                if (_gridReports.Columns["Date"] != null)
                    _gridReports.Columns["Date"].DefaultCellStyle.Format = "dd MMM yyyy";
                FormatCurrencyColumns("Revenue", "Cost", "Profit");
                if (_gridReports.Columns["ProfitMargin"] != null)
                    _gridReports.Columns["ProfitMargin"].DefaultCellStyle.Format = "0.00'%'";
            }
            else if (reportIndex == 2) // Inventory
            {
                var data = await _reportService.GetInventoryReportAsync();
                var list = data.ToList();
                _gridReports.DataSource = list;

                decimal totalValue = list.Sum(x => x.TotalValue);
                int totalItems = list.Count;

                _lblSummaryTitle.Text = "Inventory Summary";
                _lblSummaryValue1.Text = $"Unique Products: {totalItems}";
                _lblSummaryValue2.Text = $"Total Inventory Value: {totalValue:C2}";

                FormatCurrencyColumns("PurchasePrice", "TotalValue");
            }
            else if (reportIndex == 3) // Low Stock
            {
                var data = await _reportService.GetLowStockReportAsync();
                var list = data.ToList();
                _gridReports.DataSource = list;

                _lblSummaryTitle.Text = "Low Stock Alert";
                _lblSummaryValue1.Text = $"Products Low on Stock: {list.Count}";
                _lblSummaryValue2.Text = "";

                FormatCurrencyColumns("PurchasePrice", "TotalValue");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _btnGenerate.Enabled = true;
            _btnGenerate.Text = "Generate";
        }
    }

    private void FormatCurrencyColumns(params string[] columns)
    {
        foreach (var col in columns)
        {
            if (_gridReports.Columns[col] != null)
            {
                _gridReports.Columns[col].DefaultCellStyle.Format = "C2";
            }
        }
    }
}
