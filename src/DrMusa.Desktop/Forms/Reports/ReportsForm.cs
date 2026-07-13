namespace DrMusa.Desktop.Forms.Reports;

/// <summary>Placeholder for the Reports form. Implement full UI in the next phase.</summary>
public partial class ReportsForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public ReportsForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Reports";
        this.Size = new Size(1000, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblPlaceholder = new Label
        {
            Text = "Reports — Full UI coming in Phase implementation.",
            Font = new Font("Segoe UI", 14),
            AutoSize = true,
            Location = new Point(50, 50)
        };
        this.Controls.Add(lblPlaceholder);
    }
}
