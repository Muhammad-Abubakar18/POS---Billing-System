using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using DrMusa.Business.DTOs;

namespace DrMusa.Desktop.Printing;

public class ThermalPrinter
{
    private readonly ReceiptDto _data;
    private int _printableWidth;

    public ThermalPrinter(ReceiptDto data)
    {
        _data = data;
    }

    public void PrintReceipt(string printerName = null)
    {
        using (PrintDocument pd = CreatePrintDocument(printerName))
        {
            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public void ShowPrintPreview()
    {
        using (PrintDocument pd = CreatePrintDocument(null))
        {
            using (PrintPreviewDialog previewDialog = new PrintPreviewDialog())
            {
                previewDialog.Document = pd;
                previewDialog.Width = 400;
                previewDialog.Height = 600;
                previewDialog.ShowIcon = false;
                previewDialog.Text = "Receipt Preview";
                previewDialog.ShowDialog();
            }
        }
    }

    private PrintDocument CreatePrintDocument(string printerName)
    {
        PrintDocument pd = new PrintDocument();
        
        if (!string.IsNullOrEmpty(printerName))
        {
            pd.PrinterSettings.PrinterName = printerName;
        }

        // Standard margins for thermal printers are very small
        pd.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

        pd.PrintPage += Pd_PrintPage;
        
        return pd;
    }

    private void Pd_PrintPage(object sender, PrintPageEventArgs e)
    {
        // Detect printable width from the selected printer's page settings
        // If not available or valid, default to ~80mm (around 300 hundredths of an inch)
        _printableWidth = e.PageSettings.PrintableArea.Width > 0 
            ? (int)e.PageSettings.PrintableArea.Width 
            : 300; 
            
        // Reduce width slightly to account for margins
        int effectiveWidth = _printableWidth - e.PageSettings.Margins.Left - e.PageSettings.Margins.Right;
        if (effectiveWidth <= 0) effectiveWidth = 280; // fallback

        int receiptWidth = effectiveWidth;
        int offsetX = 0;

        // If printing to A4 or a large page, simulate an 80mm thermal receipt (around 320 width)
        if (effectiveWidth > 400)
        {
            receiptWidth = 320;
            offsetX = (effectiveWidth - receiptWidth) / 2;
        }

        var renderer = new ReceiptRenderer(e.Graphics, _data, receiptWidth, offsetX);
        renderer.DrawReceipt();
        
        e.HasMorePages = false;
    }
}
