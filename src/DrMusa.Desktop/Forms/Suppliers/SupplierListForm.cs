namespace DrMusa.Desktop.Forms.Suppliers;

/// <summary>Placeholder for the Supplier Management form. Implement full UI in the next phase.</summary>
public partial class SupplierListForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public SupplierListForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Supplier Management";
        this.Size = new Size(1000, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblPlaceholder = new Label
        {
            Text = "Supplier Management — Full UI coming in Phase implementation.",
            Font = new Font("Segoe UI", 14),
            AutoSize = true,
            Location = new Point(50, 50)
        };
        this.Controls.Add(lblPlaceholder);
    }
}
