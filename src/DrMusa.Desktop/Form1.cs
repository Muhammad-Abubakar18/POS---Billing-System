using System;
using System.Windows.Forms;
using DrMusa.Business.DTOs;
using DrMusa.Desktop.Printing;

namespace DrMusa.Desktop;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void btnTestReceipt_Click(object sender, EventArgs e)
    {
        var testReceipt = new ReceiptDto
        {
            RestaurantName = "DrMusa POS Test",
            RestaurantAddress = "456 Test Ave",
            RestaurantPhone = "123-456-7890",
            InvoiceNumber = "INV-1001",
            Date = DateTime.Now,
            Cashier = "Admin",
            OrderType = "Dine In",
            SubTotal = 55.50m,
            Tax = 5.55m,
            GrandTotal = 61.05m,
            PaidAmount = 70.00m,
            Change = 8.95m,
            Items = new System.Collections.Generic.List<ReceiptItemDto>
            {
                new ReceiptItemDto { Name = "Burger Extra Cheese", Quantity = 2, UnitPrice = 15.00m, Total = 30.00m },
                new ReceiptItemDto { Name = "Fries", Quantity = 1, UnitPrice = 5.50m, Total = 5.50m },
                new ReceiptItemDto { Name = "This is a very long product name to test wrapping", Quantity = 1, UnitPrice = 20.00m, Total = 20.00m }
            }
        };

        var printer = new ThermalPrinter(testReceipt);
        printer.ShowPrintPreview();
    }
}
