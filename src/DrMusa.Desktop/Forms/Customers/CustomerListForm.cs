namespace DrMusa.Desktop.Forms.Customers;

/// <summary>Placeholder for the Customer Management form. Implement full UI in the next phase.</summary>
public partial class CustomerListForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public CustomerListForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Customer Management";
        this.Size = new Size(1000, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblPlaceholder = new Label
        {
            Text = "Customer Management — Full UI coming in Phase implementation.",
            Font = new Font("Segoe UI", 14),
            AutoSize = true,
            Location = new Point(50, 50)
        };
        this.Controls.Add(lblPlaceholder);
    }
}
