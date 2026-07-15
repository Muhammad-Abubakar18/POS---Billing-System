using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Categories;

/// <summary>
/// Add / Edit category dialog for Module 4 — Category Management.
/// Returns DialogResult.OK on successful save.
/// </summary>
public sealed class CategoryEditForm : Form
{
    private readonly ICategoryService _categoryService;
    private readonly int? _categoryId;

    private TextBox _txtName = null!;
    private TextBox _txtDescription = null!;
    private Button _btnSave = null!;
    private Button _btnCancel = null!;

    public CategoryEditForm(IServiceProvider serviceProvider, int? categoryId = null)
    {
        _categoryService = serviceProvider.GetRequiredService<ICategoryService>();
        _categoryId = categoryId;

        InitializeComponent();
        Shown += async (_, __) => await LoadDataAsync();
    }

    private void InitializeComponent()
    {
        Text = _categoryId.HasValue ? "DrMusa — Edit Category" : "DrMusa — Add Category";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(520, 480);
        MinimumSize = new Size(520, 480);
        BackColor = AppTheme.BackgroundDark;
        ForeColor = AppTheme.TextPrimary;
        Font = AppTheme.FontBody;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        // ── Container ─────────────────────────────────────────────
        var container = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.BackgroundDark,
            Padding = new Padding(28)
        };

        // ── Title ──────────────────────────────────────────────────
        var lblTitle = new Label
        {
            Text = _categoryId.HasValue ? "Edit Category" : "Add Category",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(28, 24)
        };

        var lblSubtitle = new Label
        {
            Text = "Categories organise products on the POS billing screen.",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(28, 56)
        };

        // ── Fields ─────────────────────────────────────────────────
        _txtName = new TextBox();
        var pnlName = AppTheme.WrapInputPanel(_txtName, "Category name");

        _txtDescription = new TextBox
        {
            Multiline = true,
            Height = 80,
            ScrollBars = ScrollBars.Vertical
        };
        var pnlDescription = AppTheme.WrapInputPanel(_txtDescription, "Description (optional)");
        pnlDescription.Height = 96;

        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 1,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            Padding = new Padding(28, 86, 28, 0)
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        fields.Controls.Add(CreateFieldGroup("Category Name *", pnlName), 0, 0);
        fields.Controls.Add(CreateFieldGroup("Description", pnlDescription), 0, 1);

        // ── Footer ─────────────────────────────────────────────────
        _btnCancel = new Button { Text = "Cancel", Width = 110 };
        AppTheme.StyleSecondaryButton(_btnCancel);
        _btnCancel.Height = 44;
        _btnCancel.Margin = new Padding(0, 0, 12, 0); // Spacing between buttons
        _btnCancel.Click += (_, __) => DialogResult = DialogResult.Cancel;

        _btnSave = new Button { Text = _categoryId.HasValue ? "Save Changes" : "Add Category", Width = 140 };
        AppTheme.StylePrimaryButton(_btnSave);
        _btnSave.Height = 44;
        _btnSave.Click += async (_, __) => await SaveAsync();

        var footer = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 68,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(20, 12, 20, 12),
            BackColor = AppTheme.BackgroundPanel
        };
        footer.Controls.AddRange(new Control[] { _btnSave, _btnCancel });

        container.AutoScroll = true;
        container.Controls.Add(footer);
        container.Controls.Add(fields);
        container.Controls.Add(lblSubtitle);
        container.Controls.Add(lblTitle);

        Controls.Add(container);
    }

    private static Panel CreateFieldGroup(string labelText, Control field)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 14),
            Height = field.Height + 28
        };

        var label = new Label
        {
            Text = labelText,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        };

        field.Location = new Point(0, 24);
        field.Width = panel.Width;
        field.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        panel.Controls.Add(label);
        panel.Controls.Add(field);
        return panel;
    }

    private async Task LoadDataAsync()
    {
        if (!_categoryId.HasValue) return;

        try
        {
            var category = await _categoryService.GetByIdAsync(_categoryId.Value);
            if (category is null)
            {
                UIHelper.ShowError("Category not found.");
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }

            _txtName.Text = category.Name;
            _txtDescription.Text = category.Description ?? string.Empty;
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to load category: {ex.Message}");
            DialogResult = DialogResult.Abort;
            Close();
        }
    }

    private async Task SaveAsync()
    {
        var name = _txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name) || name == "Category name")
        {
            UIHelper.ShowWarning("Please enter a category name.");
            _txtName.Focus();
            return;
        }

        if (name.Length > 100)
        {
            UIHelper.ShowWarning("Category name must not exceed 100 characters.");
            _txtName.Focus();
            return;
        }

        var description = _txtDescription.Text.Trim();
        if (description == "Description (optional)")
            description = string.Empty;

        var dto = new CreateCategoryDto(
            name,
            string.IsNullOrWhiteSpace(description) ? null : description
        );

        _btnSave.Enabled = false;

        try
        {
            if (_categoryId.HasValue)
                await _categoryService.UpdateAsync(_categoryId.Value, dto);
            else
                await _categoryService.CreateAsync(dto);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Save failed: {ex.Message}");
        }
        finally
        {
            _btnSave.Enabled = true;
        }
    }
}
