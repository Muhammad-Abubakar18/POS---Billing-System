using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Inventory;

public sealed class AdjustMinStockForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IProductService _productService;
    private readonly int _productId;

    private NumericUpDown _numMinStock = null!;
    private ProductDto? _product;

    public AdjustMinStockForm(IServiceProvider serviceProvider, int productId)
    {
        _serviceProvider = serviceProvider;
        _productService = serviceProvider.GetRequiredService<IProductService>();
        _productId = productId;

        InitializeComponent();
        Shown += async (s, e) => await LoadDataAsync();
    }

    private void InitializeComponent()
    {
        Text = "Adjust Minimum Stock";
        Size = new Size(400, 300);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = AppTheme.BackgroundDark;
        ForeColor = AppTheme.TextPrimary;
        Font = AppTheme.FontBody;

        var header = new Label
        {
            Text = Text,
            Font = AppTheme.FontTitle,
            Location = new Point(30, 20),
            AutoSize = true
        };
        Controls.Add(header);

        var contentPanel = new Panel
        {
            Location = new Point(0, 70),
            Size = new Size(Width, 130),
            BackColor = AppTheme.BackgroundCard
        };

        var lblMinStock = new Label { Text = "Minimum Stock Threshold *", Location = new Point(30, 20), AutoSize = true, ForeColor = AppTheme.TextSecondary };
        _numMinStock = new NumericUpDown
        {
            Location = new Point(30, 45),
            Width = 320,
            Font = AppTheme.FontInput,
            Minimum = 0,
            Maximum = 1000000
        };
        contentPanel.Controls.Add(lblMinStock);
        contentPanel.Controls.Add(_numMinStock);

        Controls.Add(contentPanel);

        var bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = AppTheme.BackgroundPanel
        };

        var btnCancel = new Button { Text = "Cancel", Location = new Point(150, 15), Width = 100, Height = 32 };
        AppTheme.StyleSecondaryButton(btnCancel);
        btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        var btnSave = new Button { Text = "Save", Location = new Point(260, 15), Width = 100, Height = 32 };
        AppTheme.StylePrimaryButton(btnSave);
        btnSave.Click += async (s, e) => await SaveAsync();

        bottomPanel.Controls.Add(btnCancel);
        bottomPanel.Controls.Add(btnSave);
        Controls.Add(bottomPanel);
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _product = await _productService.GetByIdAsync(_productId);
            if (_product != null)
            {
                _numMinStock.Value = _product.MinimumStock;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading product: {ex.Message}");
        }
    }

    private async Task SaveAsync()
    {
        if (_product == null) return;

        try
        {
            await _productService.UpdateMinimumStockAsync(_productId, (int)_numMinStock.Value);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving: {ex.Message}");
        }
    }
}
