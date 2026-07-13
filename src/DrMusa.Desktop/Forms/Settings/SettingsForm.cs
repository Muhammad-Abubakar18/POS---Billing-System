namespace DrMusa.Desktop.Forms.Settings;

/// <summary>Placeholder for the Settings form. Implement full UI in the next phase.</summary>
public partial class SettingsForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public SettingsForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Settings";
        this.Size = new Size(1000, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblPlaceholder = new Label
        {
            Text = "Settings — Full UI coming in Phase implementation.",
            Font = new Font("Segoe UI", 14),
            AutoSize = true,
            Location = new Point(50, 50)
        };
        this.Controls.Add(lblPlaceholder);
    }
}
