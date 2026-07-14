using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using DrMusa.Business.DTOs;

namespace DrMusa.Desktop.Helpers;

public class ReceiptPrinter
{
    private readonly SaleDto _sale;
    private readonly string _businessName;
    private readonly string _businessPhone;
    private readonly string _businessAddress;
    private readonly string _footerText;
    private readonly string _currency;
    private readonly string _receiptHeader;
    private readonly string _logoBase64;

    // Thermal printer standard 80mm roll width is approx 3.14 inches -> ~314 pixels at 100dpi
    // But graphics resolution might vary. We'll use page bounds.

    public ReceiptPrinter(SaleDto sale, string businessName, string businessPhone, string businessAddress, string receiptHeader, string footerText, string currency, string logoBase64 = "")
    {
        _sale = sale;
        _businessName = businessName;
        _businessPhone = businessPhone;
        _businessAddress = businessAddress;
        _receiptHeader = receiptHeader;
        _footerText = footerText;
        _currency = currency;
        _logoBase64 = logoBase64;
    }

    public void Print(bool preview = true)
    {
        if (PrinterSettings.InstalledPrinters.Count == 0)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Save Receipt As Image",
                FileName = $"Receipt_{_sale.InvoiceNumber}.png"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using var bmp = new Bitmap(314, 1000);
                using var g = Graphics.FromImage(bmp);
                g.Clear(Color.White);
                
                DrawReceipt(g, 10, 10, 294); // 314 width - 20 margin
                
                bmp.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                MessageBox.Show("Receipt downloaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return;
        }

        using var pd = new PrintDocument();
        
        // Typical thermal printer settings
        pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 314, 1000); 
        pd.DefaultPageSettings.Margins = new Margins(10, 10, 20, 20);
        
        pd.PrintPage += Pd_PrintPage;

        if (preview)
        {
            using var previewDlg = new PrintPreviewDialog
            {
                Document = pd,
                Width = 400,
                Height = 600,
                ShowIcon = false,
                Text = "Receipt Preview"
            };
            previewDlg.ShowDialog();
        }
        else
        {
            pd.Print();
        }
    }

    private void Pd_PrintPage(object sender, PrintPageEventArgs e)
    {
        var g = e.Graphics;
        if (g == null) return;

        float y = e.MarginBounds.Top;
        float x = e.MarginBounds.Left;
        float width = e.MarginBounds.Width;

        DrawReceipt(g, x, y, width);
        e.HasMorePages = false;
    }

    private void DrawReceipt(Graphics g, float x, float y, float width)
    {

        using var fontTitle = new Font("Courier New", 14, FontStyle.Bold);
        using var fontHeader = new Font("Courier New", 10, FontStyle.Regular);
        using var fontBody = new Font("Courier New", 9, FontStyle.Regular);
        using var fontBold = new Font("Courier New", 9, FontStyle.Bold);
        using var brush = new SolidBrush(Color.Black);
        using var pen = new Pen(Color.Black, 1);

        var centerFormat = new StringFormat { Alignment = StringAlignment.Center };
        var rightFormat = new StringFormat { Alignment = StringAlignment.Far };

        // 1. Header Logo
        if (!string.IsNullOrEmpty(_logoBase64))
        {
            try
            {
                var bytes = Convert.FromBase64String(_logoBase64);
                using var ms = new System.IO.MemoryStream(bytes);
                using var img = Image.FromStream(ms);
                // Draw logo centered
                float logoWidth = 100;
                float logoHeight = (float)img.Height / img.Width * logoWidth;
                float logoX = x + (width - logoWidth) / 2;
                g.DrawImage(img, logoX, y, logoWidth, logoHeight);
                y += logoHeight + 10;
            }
            catch { /* Ignore invalid logo */ }
        }

        // 2. Header Text
        g.DrawString(_businessName, fontTitle, brush, new RectangleF(x, y, width, 25), centerFormat);
        y += 25;
        if (!string.IsNullOrEmpty(_businessAddress))
        {
            g.DrawString(_businessAddress, fontHeader, brush, new RectangleF(x, y, width, 20), centerFormat);
            y += 20;
        }
        if (!string.IsNullOrEmpty(_businessPhone))
        {
            g.DrawString("Phone: " + _businessPhone, fontHeader, brush, new RectangleF(x, y, width, 20), centerFormat);
            y += 20;
        }

        y += 10;
        g.DrawLine(pen, x, y, x + width, y);
        y += 5;

        // 3. Custom Header
        if (!string.IsNullOrEmpty(_receiptHeader))
        {
            g.DrawString(_receiptHeader, fontBody, brush, new RectangleF(x, y, width, 40), centerFormat);
            y += 40;
            g.DrawLine(pen, x, y, x + width, y);
            y += 5;
        }

        // 4. Info
        g.DrawString($"Inv#: {_sale.InvoiceNumber}", fontBody, brush, x, y);
        y += 15;
        g.DrawString($"Date: {_sale.SaleDate:yyyy-MM-dd HH:mm}", fontBody, brush, x, y);
        y += 15;
        g.DrawString($"Type: {_sale.OrderType}", fontBody, brush, x, y);
        y += 15;

        y += 10;
        g.DrawLine(pen, x, y, x + width, y);
        y += 5;

        // 5. Items Headers
        g.DrawString("Item", fontBold, brush, x, y);
        g.DrawString("Qty", fontBold, brush, x + (width * 0.5f), y);
        g.DrawString("Price", fontBold, brush, x + (width * 0.7f), y);
        g.DrawString("Total", fontBold, brush, new RectangleF(x, y, width, 20), rightFormat);
        y += 15;
        g.DrawLine(pen, x, y, x + width, y);
        y += 5;

        // 6. Items
        foreach (var item in _sale.Items)
        {
            // Truncate long names
            var name = item.ProductName.Length > 15 ? item.ProductName.Substring(0, 15) : item.ProductName;
            g.DrawString(name, fontBody, brush, x, y);
            g.DrawString(item.Quantity.ToString(), fontBody, brush, x + (width * 0.5f), y);
            g.DrawString($"{_currency} {item.UnitPrice:0.00}", fontBody, brush, x + (width * 0.7f), y);
            g.DrawString($"{_currency} {item.TotalPrice:0.00}", fontBody, brush, new RectangleF(x, y, width, 20), rightFormat);
            y += 15;
        }

        y += 5;
        g.DrawLine(pen, x, y, x + width, y);
        y += 5;

        // 7. Totals
        g.DrawString("SubTotal:", fontBody, brush, x + (width * 0.4f), y);
        g.DrawString($"{_currency} {_sale.SubTotal:0.00}", fontBody, brush, new RectangleF(x, y, width, 20), rightFormat);
        y += 15;

        if (_sale.DiscountAmount > 0)
        {
            g.DrawString($"Discount:", fontBody, brush, x + (width * 0.4f), y);
            g.DrawString($"-{_currency} {_sale.DiscountAmount:0.00}", fontBody, brush, new RectangleF(x, y, width, 20), rightFormat);
            y += 15;
        }

        if (_sale.TaxAmount > 0)
        {
            g.DrawString($"Tax:", fontBody, brush, x + (width * 0.4f), y);
            g.DrawString($"{_currency} {_sale.TaxAmount:0.00}", fontBody, brush, new RectangleF(x, y, width, 20), rightFormat);
            y += 15;
        }

        g.DrawString("Total:", fontBold, brush, x + (width * 0.4f), y);
        g.DrawString($"{_currency} {_sale.TotalAmount:0.00}", fontBold, brush, new RectangleF(x, y, width, 20), rightFormat);
        y += 20;

        g.DrawString("Paid:", fontBody, brush, x + (width * 0.4f), y);
        g.DrawString($"{_currency} {_sale.PaidAmount:0.00}", fontBody, brush, new RectangleF(x, y, width, 20), rightFormat);
        y += 15;

        g.DrawString("Change:", fontBody, brush, x + (width * 0.4f), y);
        g.DrawString($"{_currency} {_sale.ChangeAmount:0.00}", fontBody, brush, new RectangleF(x, y, width, 20), rightFormat);
        y += 20;

        // 8. Footer
        g.DrawLine(pen, x, y, x + width, y);
        y += 10;
        
        if (!string.IsNullOrEmpty(_footerText))
        {
            g.DrawString(_footerText, fontHeader, brush, new RectangleF(x, y, width, 40), centerFormat);
            y += 40;
        }

        // Draw simple barcode lines using Invoice Number length
        y += 10;
        using var bcPen = new Pen(Color.Black, 2);
        float bcX = x + (width / 2) - 50;
        for (int i = 0; i < 20; i++)
        {
            bcPen.Width = (i % 3 == 0) ? 3 : 1;
            g.DrawLine(bcPen, bcX, y, bcX, y + 30);
            bcX += 5;
        }
        y += 35;
        g.DrawString(_sale.InvoiceNumber, fontBody, brush, new RectangleF(x, y, width, 20), centerFormat);
        y += 20;
    }
}
