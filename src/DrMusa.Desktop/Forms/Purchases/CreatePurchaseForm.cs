namespace DrMusa.Desktop.Forms.Purchases;

/// <summary>Placeholder for the Create Purchase form. Implement full UI in the next phase.</summary>
public partial class CreatePurchaseForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public CreatePurchaseForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Create Purchase";
        this.Size = new Size(1000, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblPlaceholder = new Label
        {
            Text = "Create Purchase — Full UI coming in Phase implementation.",
            Font = new Font("Segoe UI", 14),
            AutoSize = true,
            Location = new Point(50, 50)
        };
        this.Controls.Add(lblPlaceholder);
    }
}
