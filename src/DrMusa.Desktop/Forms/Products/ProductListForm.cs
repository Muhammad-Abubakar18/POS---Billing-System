using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Products;

public sealed class ProductListForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IProductService _productService;

    private TextBox _txtSearch = null!;
    private DataGridView _gridProducts = null!;
    private Button _btnRefresh = null!;
    private Button _btnAdd = null!;
    private Button _btnEdit = null!;
    private Button _btnDelete = null!;

    public ProductListForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _productService = serviceProvider.GetRequiredService<IProductService>();

        InitializeComponent();
        Shown += async (_, __) => await LoadProductsAsync();
    }

    private void InitializeComponent()
    {
        Text = "DrMusa — Product Management";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1180, 720);
        MinimumSize = new Size(1024, 640);
        BackColor = AppTheme.BackgroundDark;
        ForeColor = AppTheme.TextPrimary;
        Font = AppTheme.FontBody;

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 74,
            BackColor = AppTheme.BackgroundPanel,
            Padding = new Padding(20, 14, 20, 14)
        };

        var lblTitle = new Label
        {
            Text = "Product Management",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 10)
        };

        var lblSubtitle = new Label
        {
            Text = "Add, edit, search, and soft delete active products.",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(20, 40)
        };

        _txtSearch = new TextBox { Width = 280 };
        var searchPanel = AppTheme.WrapInputPanel(_txtSearch, "Search by name or barcode");
        searchPanel.Width = 280;
        searchPanel.Location = new Point(480, 16);

        _btnRefresh = new Button { Text = "Refresh", Width = 100 };
        AppTheme.StyleSecondaryButton(_btnRefresh);
        _btnRefresh.Location = new Point(780, 16);
        _btnRefresh.Click += async (_, __) => await LoadProductsAsync();

        _btnAdd = new Button { Text = "Add Product", Width = 120 };
        AppTheme.StylePrimaryButton(_btnAdd);
        _btnAdd.Location = new Point(890, 16);
        _btnAdd.Click += async (_, __) => await OpenEditorAsync();

        _btnEdit = new Button { Text = "Edit", Width = 90 };
        AppTheme.StyleSecondaryButton(_btnEdit);
        _btnEdit.Location = new Point(1020, 16);
        _btnEdit.Click += async (_, __) => await EditSelectedAsync();

        _btnDelete = new Button { Text = "Delete", Width = 90 };
        AppTheme.StyleDangerButton(_btnDelete);
        _btnDelete.Location = new Point(1118, 16);
        _btnDelete.Click += async (_, __) => await DeleteSelectedAsync();

        header.Controls.AddRange(new Control[] { lblTitle, lblSubtitle, searchPanel, _btnRefresh, _btnAdd, _btnEdit, _btnDelete });

        _gridProducts = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = AppTheme.BackgroundCard,
            BorderStyle = BorderStyle.None,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundDark,
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontBodyBold
            },
            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundCard,
                ForeColor = AppTheme.TextPrimary,
                SelectionBackColor = AppTheme.AccentPrimary,
                SelectionForeColor = Color.White
            }
        };
        _gridProducts.CellDoubleClick += async (_, __) => await EditSelectedAsync();

        var content = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            BackColor = AppTheme.BackgroundDark
        };
        content.Controls.Add(_gridProducts);

        Controls.Add(content);
        Controls.Add(header);

        _txtSearch.TextChanged += async (_, __) => await LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var products = await _productService.GetAllAsync();
            var search = _txtSearch.Text.Trim();

            if (!string.IsNullOrWhiteSpace(search) && search != "Search by name or barcode")
            {
                products = await _productService.SearchAsync(search);
            }

            _gridProducts.DataSource = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Barcode,
                Category = p.CategoryName,
                PurchasePrice = UIHelper.FormatCurrency(p.PurchasePrice),
                SellingPrice = UIHelper.FormatCurrency(p.SellingPrice),
                Status = p.IsActive ? "Active" : "Inactive"
            }).ToList();

            if (_gridProducts.Columns["Id"] != null)
            {
                _gridProducts.Columns["Id"].Visible = false;
            }
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to load products: {ex.Message}");
        }
    }

    private int? GetSelectedProductId()
    {
        if (_gridProducts.CurrentRow?.DataBoundItem is null)
            return null;

        return _gridProducts.CurrentRow.Cells["Id"].Value is int id ? id : null;
    }

    private async Task OpenEditorAsync(int? productId = null)
    {
        using var editor = new ProductEditForm(_serviceProvider, productId);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            await LoadProductsAsync();
        }
    }

    private async Task EditSelectedAsync()
    {
        var id = GetSelectedProductId();
        if (id is null) return;

        await OpenEditorAsync(id.Value);
    }

    private async Task DeleteSelectedAsync()
    {
        var id = GetSelectedProductId();
        if (id is null) return;

        if (!UIHelper.Confirm("Soft delete this product?", "Delete Product"))
            return;

        try
        {
            await _productService.DeleteAsync(id.Value);
            UIHelper.ShowSuccess("Product deleted successfully.");
            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Delete failed: {ex.Message}");
        }
    }
}