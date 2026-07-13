namespace DrMusa.Desktop.Forms.Billing;

/// <summary>Placeholder for the Billing / POS form. Implement full UI in the next phase.</summary>
public partial class BillingForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public BillingForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Billing / POS";
        this.Size = new Size(1000, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblPlaceholder = new Label
        {
            Text = "Billing / POS — Full UI coming in Phase implementation.",
            Font = new Font("Segoe UI", 14),
            AutoSize = true,
            Location = new Point(50, 50)
        };
        this.Controls.Add(lblPlaceholder);
    }
}
