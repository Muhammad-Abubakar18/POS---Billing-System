namespace DrMusa.Desktop.Forms.Users;

/// <summary>Placeholder for the User Management form. Implement full UI in the next phase.</summary>
public partial class UserListForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public UserListForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — User Management";
        this.Size = new Size(1000, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblPlaceholder = new Label
        {
            Text = "User Management — Full UI coming in Phase implementation.",
            Font = new Font("Segoe UI", 14),
            AutoSize = true,
            Location = new Point(50, 50)
        };
        this.Controls.Add(lblPlaceholder);
    }
}
