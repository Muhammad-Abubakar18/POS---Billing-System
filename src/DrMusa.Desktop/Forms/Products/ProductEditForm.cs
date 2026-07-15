using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using DrMusa.Data.Context;
using DrMusa.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Products;

public sealed class ProductEditForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IProductService _productService;
    private readonly DrMusaDbContext _dbContext;
    private readonly int? _productId;

    private TextBox _txtName = null!;
    private TextBox _txtBarcode = null!;
    private TextBox _txtDescription = null!;
    private ComboBox _cmbCategory = null!;
    private TextBox _txtPurchasePrice = null!;
    private TextBox _txtSellingPrice = null!;
    private TextBox _txtImagePath = null!;
    private Button _btnBrowseImage = null!;
    private Button _btnSave = null!;
    private Button _btnCancel = null!;

    public ProductEditForm(IServiceProvider serviceProvider, int? productId = null)
    {
        _serviceProvider = serviceProvider;
        _productService = serviceProvider.GetRequiredService<IProductService>();
        _dbContext = serviceProvider.GetRequiredService<DrMusaDbContext>();
        _productId = productId;

        InitializeComponent();
        Shown += async (_, __) => await LoadLookupDataAsync();
    }

    private void InitializeComponent()
    {
        Text = _productId.HasValue ? "DrMusa — Edit Product" : "DrMusa — Add Product";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(760, 680);
        MinimumSize = new Size(720, 620);
        BackColor = AppTheme.BackgroundDark;
        ForeColor = AppTheme.TextPrimary;
        Font = AppTheme.FontBody;

        var container = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.BackgroundDark,
            Padding = new Padding(24)
        };

        var title = new Label
        {
            Text = _productId.HasValue ? "Edit Product" : "Add Product",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(24, 20)
        };

        var subtitle = new Label
        {
            Text = "Barcode, pricing, and category details for the product catalog.",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(24, 50)
        };

        _txtName = new TextBox();
        var pnlName = AppTheme.WrapInputPanel(_txtName, "Product name");

        _txtBarcode = new TextBox();
        var pnlBarcode = AppTheme.WrapInputPanel(_txtBarcode, "Barcode (optional)");

        _txtDescription = new TextBox { Multiline = true, Height = 72, ScrollBars = ScrollBars.Vertical };
        var pnlDescription = AppTheme.WrapInputPanel(_txtDescription, "Description (optional)");
        pnlDescription.Height = 88;

        _cmbCategory = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = AppTheme.BackgroundInput,
            ForeColor = AppTheme.TextPrimary,
            FlatStyle = FlatStyle.Flat
        };
        var pnlCategory = AppTheme.WrapInputPanel(new TextBox(), "");
        pnlCategory.Controls.Clear();
        _cmbCategory.Dock = DockStyle.Fill;
        pnlCategory.Controls.Add(_cmbCategory);

        _txtPurchasePrice = new TextBox();
        var pnlPurchase = AppTheme.WrapInputPanel(_txtPurchasePrice, "Purchase price");

        _txtSellingPrice = new TextBox();
        var pnlSelling = AppTheme.WrapInputPanel(_txtSellingPrice, "Selling price");

        _txtImagePath = new TextBox();
        var pnlImagePath = AppTheme.WrapInputPanel(_txtImagePath, "Image path (optional)");

        _btnBrowseImage = new Button { Text = "Browse", Width = 90 };
        AppTheme.StyleSecondaryButton(_btnBrowseImage);
        _btnBrowseImage.Height = 44;
        _btnBrowseImage.Click += (_, __) => BrowseImage();

        _btnCancel = new Button { Text = "Cancel", Width = 120 };
        AppTheme.StyleSecondaryButton(_btnCancel);
        _btnCancel.Click += (_, __) => DialogResult = DialogResult.Cancel;

        _btnSave = new Button { Text = "Save Product", Width = 150 };
        AppTheme.StylePrimaryButton(_btnSave);
        _btnSave.Click += async (_, __) => await SaveAsync();

        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            Padding = new Padding(24, 78, 24, 0)
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        fields.Controls.Add(CreateFieldGroup("Product Name", pnlName), 0, 0);
        fields.Controls.Add(CreateFieldGroup("Barcode", pnlBarcode), 1, 0);
        
        var grpDesc = CreateFieldGroup("Description", pnlDescription);
        fields.Controls.Add(grpDesc, 0, 1);
        fields.SetColumnSpan(grpDesc, 2);

        fields.Controls.Add(CreateFieldGroup("Category", pnlCategory), 0, 2);
        fields.Controls.Add(CreateFieldGroup("Purchase Price", pnlPurchase), 1, 2);

        fields.Controls.Add(CreateFieldGroup("Selling Price", pnlSelling), 0, 3);
        fields.Controls.Add(CreateFieldGroup("Image Path", CreateImagePathPanel(pnlImagePath)), 1, 3);

        var footer = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 70,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(24, 12, 24, 12),
            BackColor = AppTheme.BackgroundPanel
        };
        footer.Controls.AddRange(new Control[] { _btnSave, _btnCancel });

        container.AutoScroll = true;
        container.Controls.Add(footer);
        container.Controls.Add(fields);
        container.Controls.Add(subtitle);
        container.Controls.Add(title);

        Controls.Add(container);
    }

    private static Panel CreateFieldGroup(string labelText, Control field)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(8, 4, 8, 12),
            Height = field.Height + 24
        };
        var label = new Label
        {
            Text = labelText,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        };
        field.Location = new Point(0, 22);
        field.Width = panel.Width;
        field.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        
        panel.Controls.Add(label);
        panel.Controls.Add(field);
        return panel;
    }

    private Control CreateImagePathPanel(Control textPanel)
    {
        var panel = new Panel { Height = 44, Dock = DockStyle.Top };
        textPanel.Dock = DockStyle.Fill;
        _btnBrowseImage.Dock = DockStyle.Right;
        panel.Controls.Add(textPanel);
        panel.Controls.Add(_btnBrowseImage);
        return panel;
    }

    private async Task LoadLookupDataAsync()
    {
        try
        {
            var categories = await _dbContext.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            _cmbCategory.DisplayMember = nameof(Category.Name);
            _cmbCategory.ValueMember = nameof(Category.Id);
            _cmbCategory.DataSource = categories;

            if (_productId.HasValue)
            {
                var product = await _productService.GetByIdAsync(_productId.Value);
                if (product != null)
                {
                    _txtName.Text = product.Name;
                    _txtBarcode.Text = product.Barcode ?? string.Empty;
                    _txtDescription.Text = product.Description ?? string.Empty;
                    _cmbCategory.SelectedValue = product.CategoryId;
                    _txtPurchasePrice.Text = product.PurchasePrice.ToString("0.##");
                    _txtSellingPrice.Text = product.SellingPrice.ToString("0.##");
                    _txtImagePath.Text = product.ImagePath ?? string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to load product editor: {ex.Message}");
            DialogResult = DialogResult.Abort;
            Close();
        }
    }

    private void BrowseImage()
    {
        using var dlg = new OpenFileDialog
        {
            Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All Files|*.*"
        };

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            _txtImagePath.Text = dlg.FileName;
        }
    }

    private string? GetValueOrNull(TextBox textBox, string placeholder)
    {
        var text = textBox.Text.Trim();
        return string.IsNullOrWhiteSpace(text) || text == placeholder ? null : text;
    }

    private async Task SaveAsync()
    {
        if (_cmbCategory.SelectedValue is not int categoryId)
        {
            UIHelper.ShowWarning("Please select a category.");
            return;
        }

        if (!decimal.TryParse(_txtPurchasePrice.Text, out var purchasePrice) ||
            !decimal.TryParse(_txtSellingPrice.Text, out var sellingPrice))
        {
            UIHelper.ShowWarning("Please enter valid numeric values for prices.");
            return;
        }

        var name = _txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name) || name == "Product name")
        {
            UIHelper.ShowWarning("Please enter a product name.");
            return;
        }

        var dto = new CreateProductDto(
            name,
            GetValueOrNull(_txtBarcode, "Barcode (optional)"),
            GetValueOrNull(_txtDescription, "Description (optional)"),
            categoryId,
            purchasePrice,
            sellingPrice,
            GetValueOrNull(_txtImagePath, "Image path (optional)")
        );

        try
        {
            if (_productId.HasValue)
                await _productService.UpdateAsync(_productId.Value, dto);
            else
                await _productService.CreateAsync(dto);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Save failed: {ex.Message}\n\nInner: {ex.InnerException?.Message}");
        }
    }
}
