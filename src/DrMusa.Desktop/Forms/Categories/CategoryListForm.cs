namespace DrMusa.Desktop.Forms.Categories;

/// <summary>Placeholder for the Category Management form. Implement full UI in the next phase.</summary>
public partial class CategoryListForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public CategoryListForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Category Management";
        this.Size = new Size(1000, 650);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblPlaceholder = new Label
        {
            Text = "Category Management — Full UI coming in Phase implementation.",
            Font = new Font("Segoe UI", 14),
            AutoSize = true,
            Location = new Point(50, 50)
        };
        this.Controls.Add(lblPlaceholder);
    }
}
