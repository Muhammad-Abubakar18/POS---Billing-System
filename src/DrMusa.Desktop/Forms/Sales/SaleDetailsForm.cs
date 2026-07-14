using System;
using System.Drawing;
using System.Windows.Forms;
using DrMusa.Business.DTOs;
using DrMusa.Desktop.Helpers;

namespace DrMusa.Desktop.Forms.Sales;

public sealed class SaleDetailsForm : Form
{
    private readonly SaleDto _sale;

    public SaleDetailsForm(SaleDto sale)
    {
        _sale = sale;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = $"Invoice Details: {_sale.InvoiceNumber}";
        this.Size = new Size(600, 500);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = AppTheme.BackgroundPanel;
        this.Font = AppTheme.FontBody;

        var lblTitle = new Label { Text = $"Invoice: {_sale.InvoiceNumber}", Font = AppTheme.FontTitle, AutoSize = true, Location = new Point(20, 20), ForeColor = AppTheme.TextPrimary };
        this.Controls.Add(lblTitle);

        var lblDate = new Label { Text = $"Date: {_sale.SaleDate:yyyy-MM-dd HH:mm}", AutoSize = true, Location = new Point(20, 50), ForeColor = AppTheme.TextSecondary };
        this.Controls.Add(lblDate);

        var lblStatus = new Label { Text = $"Status: {_sale.Status}", AutoSize = true, Location = new Point(400, 50), ForeColor = _sale.Status == DrMusa.Common.Enums.SaleStatus.Cancelled ? Color.Red : Color.Green, Font = new Font(AppTheme.FontBody, FontStyle.Bold) };
        this.Controls.Add(lblStatus);

        var gridItems = new DataGridView
        {
            Location = new Point(20, 90),
            Size = new Size(540, 250),
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = AppTheme.BackgroundCard,
            RowHeadersVisible = false,
            AllowUserToResizeRows = false
        };
        AppTheme.StyleDataGridView(gridItems);

        gridItems.Columns.Add("ProductName", "Product");
        gridItems.Columns.Add("Quantity", "Qty");
        gridItems.Columns.Add("UnitPrice", "Unit Price");
        gridItems.Columns.Add("Total", "Total");

        foreach (var item in _sale.Items)
        {
            gridItems.Rows.Add(item.ProductName, item.Quantity, item.UnitPrice.ToString("C"), item.TotalPrice.ToString("C"));
        }
        this.Controls.Add(gridItems);

        var lblSubTotal = new Label { Text = $"SubTotal: {_sale.SubTotal:C}", AutoSize = true, Location = new Point(400, 350), ForeColor = AppTheme.TextPrimary };
        var lblDiscount = new Label { Text = $"Discount: -{_sale.DiscountAmount:C}", AutoSize = true, Location = new Point(400, 370), ForeColor = AppTheme.TextPrimary };
        var lblTax = new Label { Text = $"Tax: {_sale.TaxAmount:C}", AutoSize = true, Location = new Point(400, 390), ForeColor = AppTheme.TextPrimary };
        var lblTotal = new Label { Text = $"Total: {_sale.TotalAmount:C}", AutoSize = true, Location = new Point(400, 410), Font = new Font(AppTheme.FontBody, FontStyle.Bold), ForeColor = AppTheme.TextPrimary };
        
        this.Controls.Add(lblSubTotal);
        this.Controls.Add(lblDiscount);
        this.Controls.Add(lblTax);
        this.Controls.Add(lblTotal);

        var btnClose = new Button { Text = "Close", Width = 100, Height = 35, Location = new Point(460, 410) };
        AppTheme.StyleSecondaryButton(btnClose);
        btnClose.Click += (s, e) => this.Close();
        
        var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = AppTheme.BackgroundDark };
        btnClose.Location = new Point(bottomPanel.Width - btnClose.Width - 20, 12);
        btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Top;
        bottomPanel.Controls.Add(btnClose);
        this.Controls.Add(bottomPanel);
    }
}
