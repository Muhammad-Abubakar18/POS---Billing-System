using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Inventory;

public sealed class InventoryMovementForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IInventoryService _inventoryService;
    private readonly IProductService _productService;
    private readonly Common.Enums.StockMovementType _type;
    private readonly int? _initialProductId;

    private ComboBox _cmbProduct = null!;
    private NumericUpDown _numQuantity = null!;
    private TextBox _txtNotes = null!;

    public InventoryMovementForm(IServiceProvider serviceProvider, Common.Enums.StockMovementType type, int? initialProductId)
    {
        _serviceProvider = serviceProvider;
        _inventoryService = serviceProvider.GetRequiredService<IInventoryService>();
        _productService = serviceProvider.GetRequiredService<IProductService>();
        _type = type;
        _initialProductId = initialProductId;

        InitializeComponent();
        Shown += async (s, e) => await LoadProductsAsync();
    }

    private void InitializeComponent()
    {
        Text = _type switch
        {
            Common.Enums.StockMovementType.StockIn => "Stock In",
            Common.Enums.StockMovementType.StockOut => "Stock Out",
            Common.Enums.StockMovementType.Adjustment => "Adjust Stock",
            _ => "Inventory Movement"
        };
        
        Size = new Size(500, 450);
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
            Size = new Size(Width, 280),
            BackColor = AppTheme.BackgroundCard
        };

        int startY = 20;
        int spacing = 75;

        // Product
        var lblProduct = new Label { Text = "Product *", Location = new Point(30, startY), AutoSize = true, ForeColor = AppTheme.TextSecondary };
        _cmbProduct = new ComboBox
        {
            Location = new Point(30, startY + 25),
            Width = 420,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = AppTheme.FontInput
        };
        contentPanel.Controls.Add(lblProduct);
        contentPanel.Controls.Add(_cmbProduct);
        startY += spacing;

        // Quantity
        string qtyLabel = _type == Common.Enums.StockMovementType.Adjustment ? "New Stock Level *" : "Quantity *";
        var lblQty = new Label { Text = qtyLabel, Location = new Point(30, startY), AutoSize = true, ForeColor = AppTheme.TextSecondary };
        _numQuantity = new NumericUpDown
        {
            Location = new Point(30, startY + 25),
            Width = 420,
            Font = AppTheme.FontInput,
            Minimum = 0,
            Maximum = 1000000,
            Value = 1
        };
        contentPanel.Controls.Add(lblQty);
        contentPanel.Controls.Add(_numQuantity);
        startY += spacing;

        // Notes
        var lblNotes = new Label { Text = "Notes", Location = new Point(30, startY), AutoSize = true, ForeColor = AppTheme.TextSecondary };
        _txtNotes = new TextBox { Multiline = true, Height = 60 };
        var pnlNotes = AppTheme.WrapInputPanel(_txtNotes);
        pnlNotes.Location = new Point(30, startY + 25);
        pnlNotes.Width = 420; pnlNotes.Height = 70;
        contentPanel.Controls.Add(lblNotes);
        contentPanel.Controls.Add(pnlNotes);

        Controls.Add(contentPanel);

        var bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = AppTheme.BackgroundPanel
        };

        var btnCancel = new Button { Text = "Cancel", Location = new Point(250, 15), Width = 100, Height = 32 };
        AppTheme.StyleSecondaryButton(btnCancel);
        btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        var btnSave = new Button { Text = "Save", Location = new Point(360, 15), Width = 100, Height = 32 };
        AppTheme.StylePrimaryButton(btnSave);
        btnSave.Click += async (s, e) => await SaveAsync();

        bottomPanel.Controls.Add(btnCancel);
        bottomPanel.Controls.Add(btnSave);
        Controls.Add(bottomPanel);
    }

    private async Task LoadProductsAsync()
    {
        var products = await _productService.GetAllAsync();
        
        _cmbProduct.DisplayMember = "Name";
        _cmbProduct.ValueMember = "Id";
        _cmbProduct.DataSource = products.ToList();

        if (_initialProductId.HasValue)
        {
            _cmbProduct.SelectedValue = _initialProductId.Value;
        }
    }

    private async Task SaveAsync()
    {
        if (_cmbProduct.SelectedValue == null)
        {
            MessageBox.Show("Please select a product.");
            return;
        }

        int productId = (int)_cmbProduct.SelectedValue;
        int qty = (int)_numQuantity.Value;
        string notes = _txtNotes.Text.Trim();
        
        // Ensure user is logged in
        int userId = SessionManager.CurrentUserId ?? 1;

        try
        {
            if (_type == Common.Enums.StockMovementType.StockIn)
                await _inventoryService.StockInAsync(productId, qty, notes, userId);
            else if (_type == Common.Enums.StockMovementType.StockOut)
                await _inventoryService.StockOutAsync(productId, qty, notes, userId);
            else if (_type == Common.Enums.StockMovementType.Adjustment)
                await _inventoryService.AdjustStockAsync(productId, qty, notes, userId);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving movement: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
