namespace DrMusa.Desktop.Forms.Inventory;

/// <summary>Placeholder for the Inventory Management form. Implement full UI in the next phase.</summary>
public partial class InventoryForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public InventoryForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Inventory Management";
        this.Size = new Size(1000, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblPlaceholder = new Label
        {
            Text = "Inventory Management — Full UI coming in Phase implementation.",
            Font = new Font("Segoe UI", 14),
            AutoSize = true,
            Location = new Point(50, 50)
        };
        this.Controls.Add(lblPlaceholder);
    }
}
