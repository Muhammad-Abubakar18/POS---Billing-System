using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Sales;

public partial class SalesHistoryForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISaleService _saleService;
    private readonly ISettingService _settingService;
    private DataGridView _grid = null!;
    private TextBox _txtSearch = null!;
    private DateTimePicker _dtFrom = null!;
    private DateTimePicker _dtTo = null!;
    private List<SaleDto> _allSales = new();

    public SalesHistoryForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _saleService = serviceProvider.GetRequiredService<ISaleService>();
        _settingService = serviceProvider.GetRequiredService<ISettingService>();
        InitializeComponent();
        this.Load += async (s, e) => await LoadDataAsync();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Sales History";
        this.BackColor = AppTheme.BackgroundDark;
        this.Font = AppTheme.FontBody;
        this.Size = new Size(1040, 700);

        var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        this.Controls.Add(mainPanel);

        var lblTitle = new Label { Text = "Sales History", Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(0, 0) };

        var filterPanel = new Panel { Location = new Point(0, 50), Size = new Size(1000, 60), BackColor = AppTheme.BackgroundCard, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        
        var lblFrom = new Label { Text = "From:", AutoSize = true, Location = new Point(10, 20), ForeColor = AppTheme.TextPrimary };
        _dtFrom = new DateTimePicker { Location = new Point(60, 16), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-7) };
        
        var lblTo = new Label { Text = "To:", AutoSize = true, Location = new Point(230, 20), ForeColor = AppTheme.TextPrimary };
        _dtTo = new DateTimePicker { Location = new Point(270, 16), Width = 130, Format = DateTimePickerFormat.Short, Value = DateTime.Today };

        var btnFilter = new Button { Text = "Load Dates", Location = new Point(420, 14), Width = 100, Height = 30 };
        AppTheme.StyleSecondaryButton(btnFilter);
        btnFilter.Click += async (s, e) => await LoadDataAsync();

        var lblSearch = new Label { Text = "Invoice Number:", AutoSize = true, Location = new Point(540, 20), ForeColor = AppTheme.TextPrimary };
        _txtSearch = new TextBox { Location = new Point(660, 16), Width = 200, Font = new Font("Segoe UI", 11f) };
        _txtSearch.TextChanged += (s, e) => FilterGrid();

        filterPanel.Controls.Add(lblFrom);
        filterPanel.Controls.Add(_dtFrom);
        filterPanel.Controls.Add(lblTo);
        filterPanel.Controls.Add(_dtTo);
        filterPanel.Controls.Add(btnFilter);
        filterPanel.Controls.Add(lblSearch);
        filterPanel.Controls.Add(_txtSearch);

        var headerPanel = new Panel { Dock = DockStyle.Top, Height = 120 };
        headerPanel.Controls.Add(lblTitle);
        headerPanel.Controls.Add(filterPanel);

        // --- Action Buttons ---
        var actionPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 15, 0, 0) };
        
        var btnView = new Button { Text = "View Details", Width = 120, Height = 40 };
        AppTheme.StylePrimaryButton(btnView);
        btnView.Click += BtnView_Click;

        var btnReprint = new Button { Text = "Reprint Receipt", Width = 140, Height = 40 };
        AppTheme.StyleSecondaryButton(btnReprint);
        btnReprint.Click += async (s, e) => await BtnReprint_Click();

        var btnCancel = new Button { Text = "Cancel Invoice", Width = 140, Height = 40 };
        AppTheme.StyleDangerButton(btnCancel);
        btnCancel.Click += async (s, e) => await BtnCancel_Click();

        actionPanel.Controls.Add(btnView);
        actionPanel.Controls.Add(btnReprint);
        
        if (SessionManager.HasRole(DrMusa.Common.Enums.UserRole.Owner, DrMusa.Common.Enums.UserRole.SubAdmin))
        {
            actionPanel.Controls.Add(btnCancel);
        }

        // --- Grid ---
        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = AppTheme.BackgroundCard,
            RowHeadersVisible = false,
            AllowUserToResizeRows = false,
            ScrollBars = ScrollBars.Both
        };
        AppTheme.StyleDataGridView(_grid);

        _grid.Columns.Add("Id", "Id");
        _grid.Columns["Id"].Visible = false;
        _grid.Columns.Add("Invoice", "Invoice Number");
        _grid.Columns.Add("Date", "Date");
        _grid.Columns.Add("Items", "Items");
        _grid.Columns.Add("Total", "Total Amount");
        _grid.Columns.Add("Status", "Status");

        mainPanel.Controls.Add(_grid);        // Fill (must be added last so it occupies remaining space)
        mainPanel.Controls.Add(headerPanel);  // Top
        mainPanel.Controls.Add(actionPanel);  // Bottom
    }

    private async Task LoadDataAsync()
    {
        try
        {
            // End of day
            var toDate = _dtTo.Value.Date.AddDays(1).AddTicks(-1); 
            var sales = await _saleService.GetByDateRangeAsync(_dtFrom.Value.Date, toDate);
            _allSales = sales.OrderByDescending(s => s.SaleDate).ToList();
            FilterGrid();
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to load sales: {ex.Message}");
        }
    }

    private void FilterGrid()
    {
        var term = _txtSearch.Text.Trim().ToLower();
        var filtered = string.IsNullOrEmpty(term)
            ? _allSales
            : _allSales.Where(s => s.InvoiceNumber.ToLower().Contains(term)).ToList();

        _grid.Rows.Clear();
        foreach (var s in filtered)
        {
            var row = new DataGridViewRow();
            row.CreateCells(_grid, s.Id, s.InvoiceNumber, s.SaleDate.ToString("yyyy-MM-dd HH:mm"), s.Items.Sum(i => i.Quantity), s.TotalAmount.ToString("C"), s.Status.ToString());
            
            if (s.Status == DrMusa.Common.Enums.SaleStatus.Cancelled)
                row.DefaultCellStyle.ForeColor = Color.Red;

            _grid.Rows.Add(row);
        }
    }

    private SaleDto? GetSelectedSale()
    {
        if (_grid.SelectedRows.Count == 0) return null;
        var id = (int)_grid.SelectedRows[0].Cells["Id"].Value;
        return _allSales.FirstOrDefault(s => s.Id == id);
    }

    private void BtnView_Click(object? sender, EventArgs e)
    {
        var sale = GetSelectedSale();
        if (sale == null)
        {
            UIHelper.ShowWarning("Please select a sale to view.");
            return;
        }

        using var frm = new SaleDetailsForm(sale);
        frm.ShowDialog();
    }

    private async Task BtnReprint_Click()
    {
        var sale = GetSelectedSale();
        if (sale == null)
        {
            UIHelper.ShowWarning("Please select a sale to reprint.");
            return;
        }

        try
        {
            var bName = await _settingService.GetValueAsync("BusinessName") ?? "DrMusa Store";
            var bPhone = await _settingService.GetValueAsync("BusinessPhone") ?? "";
            var bAddress = await _settingService.GetValueAsync("BusinessAddress") ?? "";
            var rHeader = await _settingService.GetValueAsync("ReceiptHeader") ?? "Thank you for shopping!";
            var rFooter = await _settingService.GetValueAsync("ReceiptFooter") ?? "Please come again";
            var currency = await _settingService.GetValueAsync("Currency") ?? "PKR";
            var logo = await _settingService.GetValueAsync("BusinessLogo") ?? "";

            var printer = new ReceiptPrinter(sale, bName, bPhone, bAddress, rHeader, rFooter, currency, logo);
            printer.Print(preview: false);
            
            // Print Kitchen Receipt
            printer.IsKitchenReceipt = true;
            printer.Print(preview: false);
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to print receipt: {ex.Message}");
        }
    }

    private async Task BtnCancel_Click()
    {
        var sale = GetSelectedSale();
        if (sale == null)
        {
            UIHelper.ShowWarning("Please select a sale to cancel.");
            return;
        }

        if (sale.Status == DrMusa.Common.Enums.SaleStatus.Cancelled)
        {
            UIHelper.ShowWarning("This invoice is already cancelled.");
            return;
        }

        var confirm = MessageBox.Show(
            $"Are you sure you want to cancel Invoice {sale.InvoiceNumber}?\n\nThis will refund the entire order amount ({sale.TotalAmount:C}) and automatically return {sale.Items.Sum(i => i.Quantity)} items to stock.",
            "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (confirm == DialogResult.Yes)
        {
            try
            {
                var success = await _saleService.CancelSaleAsync(sale.Id);
                if (success)
                {
                    UIHelper.ShowSuccess("Invoice cancelled and stock returned successfully!");
                    await LoadDataAsync();
                }
                else
                {
                    UIHelper.ShowError("Failed to cancel the invoice.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"An error occurred: {ex.Message}");
            }
        }
    }
}
