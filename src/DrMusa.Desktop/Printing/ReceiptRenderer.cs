using System;
using System.Collections.Generic;
using System.Drawing;
using DrMusa.Business.DTOs;

namespace DrMusa.Desktop.Printing;

public class ReceiptRenderer
{
    private readonly Graphics _g;
    private readonly ReceiptDto _data;
    private readonly int _printableWidth;
    private readonly int _offsetX;
    
    // Fonts
    private readonly Font _fontRegular;
    private readonly Font _fontBold;
    private readonly Font _fontHeader;
    private readonly Font _fontTotal;
    
    private float _currentY = 0;
    
    // String Formats for alignment
    private readonly StringFormat _formatLeft;
    private readonly StringFormat _formatCenter;
    private readonly StringFormat _formatRight;
    
    public ReceiptRenderer(Graphics g, ReceiptDto data, int printableWidth, int offsetX = 0)
    {
        _g = g;
        _data = data;
        _printableWidth = printableWidth;
        _offsetX = offsetX;
        
        // Font sizes based on requirements
        string fontFamily = "Courier New"; // Monospaced font for POS standard
        
        _fontRegular = new Font(fontFamily, 10f, FontStyle.Regular);
        _fontBold = new Font(fontFamily, 10f, FontStyle.Bold);
        _fontHeader = new Font(fontFamily, 16f, FontStyle.Bold); // 16-18pt
        _fontTotal = new Font(fontFamily, 14f, FontStyle.Bold); // 14pt
        
        _formatLeft = new StringFormat { Alignment = StringAlignment.Near };
        _formatCenter = new StringFormat { Alignment = StringAlignment.Center };
        _formatRight = new StringFormat { Alignment = StringAlignment.Far };
    }
    
    public void DrawReceipt()
    {
        _currentY = 10; // Start with a small padding
        
        DrawHeader();
        DrawSeparator();
        DrawInvoiceInfo();
        DrawSeparator();
        DrawItems();
        DrawSeparator();
        DrawTotals();
        DrawSeparator();
        DrawFooter();
    }
    
    private void DrawHeader()
    {
        // Logo could be drawn here if available
        // if (logo != null) _g.DrawImage(logo, x, _currentY, w, h);
        
        DrawTextCentered(_data.RestaurantName, _fontHeader);
        _currentY += 5; // Padding below title
        
        DrawTextCentered(_data.RestaurantAddress, _fontRegular);
        DrawTextCentered(_data.RestaurantPhone, _fontRegular);
        if (!string.IsNullOrEmpty(_data.TaxNumber))
        {
            DrawTextCentered($"Tax No: {_data.TaxNumber}", _fontRegular);
        }
    }
    
    private void DrawInvoiceInfo()
    {
        _currentY += 5;
        DrawTextLeftRight("Invoice No:", _data.InvoiceNumber, _fontRegular);
        DrawTextLeftRight("Date:", _data.Date.ToString("yyyy-MM-dd"), _fontRegular);
        DrawTextLeftRight("Time:", _data.Date.ToString("HH:mm"), _fontRegular);
        DrawTextLeftRight("Cashier:", _data.Cashier, _fontRegular);
        DrawTextLeftRight("Order Type:", _data.OrderType, _fontRegular);
        _currentY += 5;
    }
    
    private void DrawItems()
    {
        _currentY += 5;
        
        // Column Proportions: Qty 15%, Item 45%, Price 20%, Total 20%
        float qtyW = _printableWidth * 0.15f;
        float itemW = _printableWidth * 0.45f;
        float priceW = _printableWidth * 0.20f;
        float totalW = _printableWidth * 0.20f;
        
        float qtyX = _offsetX;
        float itemX = qtyX + qtyW;
        float priceX = itemX + itemW;
        float totalX = priceX + priceW;
        
        // Headers
        float maxH = MeasureStringHeight("Qty", _fontBold, qtyW);
        
        RectangleF rectQty = new RectangleF(qtyX, _currentY, qtyW, maxH);
        RectangleF rectItem = new RectangleF(itemX, _currentY, itemW, maxH);
        RectangleF rectPrice = new RectangleF(priceX, _currentY, priceW, maxH);
        RectangleF rectTotal = new RectangleF(totalX, _currentY, totalW, maxH);
        
        _g.DrawString("Qty", _fontBold, Brushes.Black, rectQty, _formatLeft);
        _g.DrawString("Item", _fontBold, Brushes.Black, rectItem, _formatLeft);
        _g.DrawString("Unit Price", _fontBold, Brushes.Black, rectPrice, _formatRight);
        _g.DrawString("Total", _fontBold, Brushes.Black, rectTotal, _formatRight);
        
        _currentY += maxH;
        DrawSeparator();
        _currentY += 5;
        
        // Items
        foreach (var item in _data.Items)
        {
            string qtyStr = item.Quantity.ToString();
            string nameStr = item.Name;
            string priceStr = item.UnitPrice.ToString("N2");
            string totalStr = item.Total.ToString("N2");
            
            // Measure height needed for the wrapping item name
            float itemHeight = MeasureStringHeight(nameStr, _fontRegular, itemW);
            float standardHeight = MeasureStringHeight(qtyStr, _fontRegular, qtyW); // height of 1 line
            
            float rowHeight = Math.Max(itemHeight, standardHeight);
            
            rectQty = new RectangleF(qtyX, _currentY, qtyW, rowHeight);
            rectItem = new RectangleF(itemX, _currentY, itemW, rowHeight);
            rectPrice = new RectangleF(priceX, _currentY, priceW, rowHeight);
            rectTotal = new RectangleF(totalX, _currentY, totalW, rowHeight);
            
            _g.DrawString(qtyStr, _fontRegular, Brushes.Black, rectQty, _formatLeft);
            _g.DrawString(nameStr, _fontRegular, Brushes.Black, rectItem, _formatLeft);
            _g.DrawString(priceStr, _fontRegular, Brushes.Black, rectPrice, _formatRight);
            _g.DrawString(totalStr, _fontRegular, Brushes.Black, rectTotal, _formatRight);
            
            _currentY += rowHeight;
        }
        
        _currentY += 5;
    }
    
    private void DrawTotals()
    {
        _currentY += 5;
        
        DrawTextLeftRight("Subtotal", _data.SubTotal.ToString("N2"), _fontRegular);
        if (_data.Discount > 0)
        {
            DrawTextLeftRight("Discount", _data.Discount.ToString("N2"), _fontRegular);
        }
        if (_data.Tax > 0)
        {
            DrawTextLeftRight("Tax", _data.Tax.ToString("N2"), _fontRegular);
        }
        
        DrawThickSeparator();
        
        DrawTextLeftRight("TOTAL", _data.GrandTotal.ToString("N2"), _fontTotal);
        
        DrawThickSeparator();
        
        DrawTextLeftRight("Paid", _data.PaidAmount.ToString("N2"), _fontRegular);
        DrawTextLeftRight("Change", _data.Change.ToString("N2"), _fontRegular);
        
        _currentY += 5;
    }
    
    private void DrawFooter()
    {
        _currentY += 10;
        DrawTextCentered(_data.FooterMessage, _fontRegular);
        
        _currentY += 10;
        DrawTextCentered("Barcode", _fontRegular); // Mock barcode
        DrawTextCentered(_data.InvoiceNumber, _fontRegular);
    }
    
    // --- Helper Methods ---
    
    private float MeasureStringHeight(string text, Font font, float width)
    {
        SizeF size = _g.MeasureString(text, font, new SizeF(width, 1000));
        return size.Height;
    }
    
    private void DrawTextCentered(string text, Font font)
    {
        RectangleF rect = new RectangleF(_offsetX, _currentY, _printableWidth, MeasureStringHeight(text, font, _printableWidth));
        _g.DrawString(text, font, Brushes.Black, rect, _formatCenter);
        _currentY += rect.Height;
    }
    
    private void DrawTextLeftRight(string leftText, string rightText, Font font)
    {
        // Divide into 50/50 for left right sections
        float halfW = _printableWidth / 2f;
        
        float leftH = MeasureStringHeight(leftText, font, halfW);
        float rightH = MeasureStringHeight(rightText, font, halfW);
        float maxH = Math.Max(leftH, rightH);
        
        RectangleF rectLeft = new RectangleF(_offsetX, _currentY, halfW, maxH);
        RectangleF rectRight = new RectangleF(_offsetX + halfW, _currentY, halfW, maxH);
        
        _g.DrawString(leftText, font, Brushes.Black, rectLeft, _formatLeft);
        _g.DrawString(rightText, font, Brushes.Black, rectRight, _formatRight);
        
        _currentY += maxH;
    }
    
    private void DrawSeparator()
    {
        _currentY += 8;
        using (Pen pen = new Pen(Color.Black, 1))
        {
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            _g.DrawLine(pen, _offsetX, _currentY, _offsetX + _printableWidth, _currentY);
        }
        _currentY += 8;
    }
    
    private void DrawThickSeparator()
    {
        _currentY += 8;
        using (Pen pen = new Pen(Color.Black, 1.5f))
        {
            // Usually thick separators on receipts are represented by double lines or bold equals signs
            // We'll draw two dashed lines close together
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            _g.DrawLine(pen, _offsetX, _currentY, _offsetX + _printableWidth, _currentY);
            _g.DrawLine(pen, _offsetX, _currentY + 3, _offsetX + _printableWidth, _currentY + 3);
        }
        _currentY += 11;
    }
}
