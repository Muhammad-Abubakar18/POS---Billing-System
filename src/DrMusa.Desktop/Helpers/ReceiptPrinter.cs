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

        if (width > 400)
        {
            width = 314;
            x = e.MarginBounds.Left + (e.MarginBounds.Width - width) / 2;
        }

        DrawReceipt(g, x, y, width);
        e.HasMorePages = false;
    }

    private void DrawReceipt(Graphics g, float x, float startY, float width)
    {
        using var fontTitle = new Font("Courier New", 16, FontStyle.Bold);
        using var fontHeader = new Font("Courier New", 10, FontStyle.Regular);
        using var fontBody = new Font("Courier New", 10, FontStyle.Regular);
        using var fontBold = new Font("Courier New", 10, FontStyle.Bold);
        using var fontTotal = new Font("Courier New", 14, FontStyle.Bold);
        using var brush = new SolidBrush(Color.Black);
        using var pen = new Pen(Color.Black, 1);
        using var thickPen = new Pen(Color.Black, 1.5f);

        var formatLeft = new StringFormat { Alignment = StringAlignment.Near };
        var formatCenter = new StringFormat { Alignment = StringAlignment.Center };
        var formatRight = new StringFormat { Alignment = StringAlignment.Far };

        float y = startY;

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

        // Helper methods for layout
        float MeasureH(string text, Font f, float w) => g.MeasureString(text, f, new SizeF(w, 1000)).Height;
        
        void DrawCentered(string text, Font f)
        {
            float h = MeasureH(text, f, width);
            g.DrawString(text, f, brush, new RectangleF(x, y, width, h), formatCenter);
            y += h;
        }
        
        void DrawLeftRight(string left, string right, Font f)
        {
            float half = width / 2f;
            float lh = MeasureH(left, f, half);
            float rh = MeasureH(right, f, half);
            float maxH = Math.Max(lh, rh);
            g.DrawString(left, f, brush, new RectangleF(x, y, half, maxH), formatLeft);
            g.DrawString(right, f, brush, new RectangleF(x + half, y, half, maxH), formatRight);
            y += maxH;
        }

        // 2. Header Text
        DrawCentered(_businessName, fontTitle);
        y += 5;
        if (!string.IsNullOrEmpty(_businessAddress))
        {
            DrawCentered(_businessAddress, fontHeader);
        }
        if (!string.IsNullOrEmpty(_businessPhone))
        {
            DrawCentered("Phone: " + _businessPhone, fontHeader);
        }

        y += 10;
        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        g.DrawLine(pen, x, y, x + width, y);
        y += 5;

        // 3. Custom Header
        if (!string.IsNullOrEmpty(_receiptHeader))
        {
            DrawCentered(_receiptHeader, fontBody);
            y += 5;
            g.DrawLine(pen, x, y, x + width, y);
            y += 5;
        }

        // 4. Info
        DrawLeftRight("Inv#:", _sale.InvoiceNumber, fontBody);
        DrawLeftRight("Date:", _sale.SaleDate.ToString("yyyy-MM-dd HH:mm"), fontBody);
        DrawLeftRight("Type:", _sale.OrderType.ToString(), fontBody);

        y += 10;
        g.DrawLine(pen, x, y, x + width, y);
        y += 5;

        // 5. Items Headers
        // Proportions: Qty 15%, Item 45%, Price 20%, Total 20%
        float qtyW = width * 0.15f;
        float itemW = width * 0.45f;
        float priceW = width * 0.20f;
        float totalW = width * 0.20f;

        float qtyX = x;
        float itemX = qtyX + qtyW;
        float priceX = itemX + itemW;
        float totalX = priceX + priceW;

        float headerH = MeasureH("Qty", fontBold, qtyW);
        g.DrawString("Qty", fontBold, brush, new RectangleF(qtyX, y, qtyW, headerH), formatLeft);
        g.DrawString("Item", fontBold, brush, new RectangleF(itemX, y, itemW, headerH), formatLeft);
        g.DrawString("Price", fontBold, brush, new RectangleF(priceX, y, priceW, headerH), formatRight);
        g.DrawString("Total", fontBold, brush, new RectangleF(totalX, y, totalW, headerH), formatRight);
        y += headerH + 5;
        g.DrawLine(pen, x, y, x + width, y);
        y += 5;

        // 6. Items
        foreach (var item in _sale.Items)
        {
            string qtyStr = item.Quantity.ToString();
            string nameStr = item.ProductName;
            string priceStr = item.UnitPrice.ToString("0.00"); // No currency symbol here to avoid PKRPKR
            string totalStr = item.TotalPrice.ToString("0.00");

            float itemH = MeasureH(nameStr, fontBody, itemW);
            float singleH = MeasureH("1", fontBody, qtyW);
            float rowH = Math.Max(itemH, singleH);

            g.DrawString(qtyStr, fontBody, brush, new RectangleF(qtyX, y, qtyW, rowH), formatLeft);
            g.DrawString(nameStr, fontBody, brush, new RectangleF(itemX, y, itemW, rowH), formatLeft);
            g.DrawString(priceStr, fontBody, brush, new RectangleF(priceX, y, priceW, rowH), formatRight);
            g.DrawString(totalStr, fontBody, brush, new RectangleF(totalX, y, totalW, rowH), formatRight);
            
            y += rowH;
        }

        y += 5;
        g.DrawLine(pen, x, y, x + width, y);
        y += 5;

        // 7. Totals
        DrawLeftRight("SubTotal:", $"{_currency} {_sale.SubTotal:0.00}", fontBody);
        
        if (_sale.DiscountAmount > 0)
        {
            DrawLeftRight("Discount:", $"-{_currency} {_sale.DiscountAmount:0.00}", fontBody);
        }

        if (_sale.TaxAmount > 0)
        {
            string taxLabel = _sale.TaxPercent > 0 ? $"Tax ({_sale.TaxPercent:0}%):" : "Tax:";
            DrawLeftRight(taxLabel, $"{_currency} {_sale.TaxAmount:0.00}", fontBody);
        }

        y += 5;
        g.DrawLine(thickPen, x, y, x + width, y);
        g.DrawLine(thickPen, x, y + 2, x + width, y + 2);
        y += 7;

        DrawLeftRight("TOTAL:", $"{_currency} {_sale.TotalAmount:0.00}", fontTotal);

        y += 5;
        g.DrawLine(thickPen, x, y, x + width, y);
        g.DrawLine(thickPen, x, y + 2, x + width, y + 2);
        y += 7;

        DrawLeftRight("Paid:", $"{_currency} {_sale.PaidAmount:0.00}", fontBody);
        DrawLeftRight("Change:", $"{_currency} {_sale.ChangeAmount:0.00}", fontBody);

        y += 10;
        g.DrawLine(pen, x, y, x + width, y);
        y += 10;
        
        // 8. Footer
        if (!string.IsNullOrEmpty(_footerText))
        {
            DrawCentered(_footerText, fontHeader);
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
        DrawCentered(_sale.InvoiceNumber, fontBody);
    }
}
