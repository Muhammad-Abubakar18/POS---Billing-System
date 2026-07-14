using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Common.Enums;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Billing;

public partial class BillingForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly ISaleService _saleService;

    // UI Controls
    private FlowLayoutPanel _pnlCategories = null!;
    private FlowLayoutPanel _pnlProducts = null!;
    private DataGridView _gridCart = null!;
    private TextBox _txtSearch = null!;
    private TextBox _txtBarcode = null!;

    private Label _lblSubTotal = null!;
    private TextBox _txtDiscount = null!;
    private TextBox _txtTax = null!;
    private Label _lblTotal = null!;
    private TextBox _txtPaidAmount = null!;
    private Label _lblChange = null!;
    
    private Button _btnCash = null!;
    private Button _btnCard = null!;
    private Button _btnComplete = null!;

    // State
    private bool _isInitializing = true;
    private List<CategoryDto> _categories = new();
    private List<ProductDto> _allProducts = new();
    private int? _selectedCategoryId = null;
    private List<CartItem> _cart = new();
    private PaymentMethod _selectedPayment = PaymentMethod.Cash;

    public BillingForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _categoryService = serviceProvider.GetRequiredService<ICategoryService>();
        _productService = serviceProvider.GetRequiredService<IProductService>();
        _saleService = serviceProvider.GetRequiredService<ISaleService>();

        InitializeComponent();
        _isInitializing = false;
        Shown += async (s, e) => await LoadDataAsync();
    }

    private void InitializeComponent()
    {
        Text = "DrMusa — Billing / POS";
        BackColor = AppTheme.BackgroundDark;
        Font = AppTheme.FontBody;

        var mainSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            FixedPanel = FixedPanel.Panel2,
            SplitterWidth = 1,
            BackColor = AppTheme.BorderDefault
        };

        this.Load += (s, e) =>
        {
            if (ClientSize.Width > 450)
                mainSplit.SplitterDistance = ClientSize.Width - 450;
        };

        this.SizeChanged += (s, e) =>
        {
            if (ClientSize.Width > 450)
                mainSplit.SplitterDistance = ClientSize.Width - 450;
        };

        var leftPanel = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.BackgroundDark, Padding = new Padding(20) };
        var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.BackgroundPanel, Padding = new Padding(20) };

        mainSplit.Panel1.Controls.Add(leftPanel);
        mainSplit.Panel2.Controls.Add(rightPanel);

        // --- Left Panel: Catalog ---
        
        var topBar = new Panel { Dock = DockStyle.Top, Height = 60 };
        _txtSearch = new TextBox { Width = 300, PlaceholderText = "🔍 Search products...", Font = new Font("Segoe UI", 12f) };
        _txtSearch.TextChanged += (s, e) => FilterProducts();
        
        _txtBarcode = new TextBox { Width = 200, PlaceholderText = "||| Scan barcode", Font = new Font("Segoe UI", 12f), Location = new Point(320, 0) };
        _txtBarcode.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { ProcessBarcode(_txtBarcode.Text); _txtBarcode.Clear(); e.SuppressKeyPress = true; } };

        topBar.Controls.Add(AppTheme.WrapInputPanel(_txtSearch, "Search"));
        var pnlBc = AppTheme.WrapInputPanel(_txtBarcode, "Barcode");
        pnlBc.Left = 320;
        topBar.Controls.Add(pnlBc);

        _pnlCategories = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 60,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(0, 10, 0, 10)
        };

        _pnlProducts = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(0, 10, 0, 0)
        };

        leftPanel.Controls.Add(_pnlProducts);
        leftPanel.Controls.Add(_pnlCategories);
        leftPanel.Controls.Add(topBar);

        // --- Right Panel: Cart & Checkout ---

        var lblCartTitle = new Label { Text = "Current Order", Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary, Dock = DockStyle.Top, Height = 40 };

        _gridCart = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = AppTheme.BackgroundPanel,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            EnableHeadersVisualStyles = false,
            GridColor = AppTheme.BorderDefault,
            DefaultCellStyle = new DataGridViewCellStyle { BackColor = AppTheme.BackgroundPanel, ForeColor = AppTheme.TextPrimary, SelectionBackColor = AppTheme.AccentPrimary },
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = AppTheme.BackgroundDark, ForeColor = AppTheme.TextSecondary, Font = new Font("Segoe UI", 9f, FontStyle.Bold) },
            RowTemplate = { Height = 40 }
        };
        _gridCart.CellClick += GridCart_CellClick;

        var pnlCheckout = new Panel { Dock = DockStyle.Bottom, Height = 320, Padding = new Padding(0, 10, 0, 0) };
        
        _lblSubTotal = new Label { Text = "SubTotal: 0.00", Font = new Font("Segoe UI", 12f), ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(0, 10) };
        
        _txtDiscount = new TextBox { Width = 80, Text = "0", TextAlign = HorizontalAlignment.Right };
        _txtDiscount.TextChanged += (s, e) => CalculateTotals();
        var pnlDisc = AppTheme.WrapInputPanel(_txtDiscount, "Disc %");
        pnlDisc.Location = new Point(0, 40);
        pnlDisc.Width = 100;

        _txtTax = new TextBox { Width = 80, Text = "0", TextAlign = HorizontalAlignment.Right };
        _txtTax.TextChanged += (s, e) => CalculateTotals();
        var pnlTax = AppTheme.WrapInputPanel(_txtTax, "Tax %");
        pnlTax.Location = new Point(110, 40);
        pnlTax.Width = 100;

        _lblTotal = new Label { Text = "Total: 0.00", Font = new Font("Segoe UI", 16f, FontStyle.Bold), ForeColor = AppTheme.AccentSuccess, AutoSize = true, Location = new Point(0, 110) };

        _btnCash = new Button { Text = "Cash", Width = 100, Height = 40, Location = new Point(0, 160) };
        AppTheme.StylePrimaryButton(_btnCash);
        _btnCash.Click += (s, e) => SelectPayment(PaymentMethod.Cash);

        _btnCard = new Button { Text = "Card", Width = 100, Height = 40, Location = new Point(110, 160) };
        AppTheme.StyleSecondaryButton(_btnCard);
        _btnCard.Click += (s, e) => SelectPayment(PaymentMethod.Card);

        _txtPaidAmount = new TextBox { Width = 100, Text = "0", TextAlign = HorizontalAlignment.Right };
        _txtPaidAmount.TextChanged += (s, e) => CalculateTotals();
        var pnlPaid = AppTheme.WrapInputPanel(_txtPaidAmount, "Paid Amt");
        pnlPaid.Location = new Point(220, 160);
        pnlPaid.Width = 120;

        _lblChange = new Label { Text = "Change: 0.00", Font = new Font("Segoe UI", 12f), ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(220, 215) };

        _btnComplete = new Button { Text = "Complete Order", Dock = DockStyle.Bottom, Height = 50, Font = new Font("Segoe UI", 12f, FontStyle.Bold) };
        AppTheme.StylePrimaryButton(_btnComplete);
        _btnComplete.Click += async (s, e) => await CompleteOrderAsync();

        pnlCheckout.Controls.AddRange(new Control[] { _lblSubTotal, pnlDisc, pnlTax, _lblTotal, _btnCash, _btnCard, pnlPaid, _lblChange, _btnComplete });

        rightPanel.Controls.Add(_gridCart);
        rightPanel.Controls.Add(lblCartTitle);
        rightPanel.Controls.Add(pnlCheckout);

        Controls.Add(mainSplit);
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _categories = (await _categoryService.GetAllAsync()).ToList();
            _allProducts = (await _productService.GetAllAsync()).ToList();

            BuildCategoryTabs();
            FilterProducts();
            UpdateCartGrid();
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to load POS data: {ex.Message}");
        }
    }

    private void BuildCategoryTabs()
    {
        _pnlCategories.Controls.Clear();

        var btnAll = new Button { Text = "All Categories", AutoSize = true, Height = 40, Padding = new Padding(10, 0, 10, 0), Cursor = Cursors.Hand };
        AppTheme.StylePrimaryButton(btnAll);
        btnAll.Click += (s, e) => { _selectedCategoryId = null; HighlightCategoryButton(btnAll); FilterProducts(); };
        _pnlCategories.Controls.Add(btnAll);

        foreach (var cat in _categories)
        {
            var btn = new Button { Text = cat.Name, AutoSize = true, Height = 40, Padding = new Padding(10, 0, 10, 0), Cursor = Cursors.Hand };
            AppTheme.StyleSecondaryButton(btn);
            btn.Click += (s, e) => { _selectedCategoryId = cat.Id; HighlightCategoryButton(btn); FilterProducts(); };
            _pnlCategories.Controls.Add(btn);
        }
    }

    private void HighlightCategoryButton(Button selected)
    {
        foreach (Control c in _pnlCategories.Controls)
        {
            if (c is Button b)
            {
                if (b == selected) AppTheme.StylePrimaryButton(b);
                else AppTheme.StyleSecondaryButton(b);
            }
        }
    }

    private void FilterProducts()
    {
        if (_isInitializing) return;
        if (_pnlProducts == null) return;

        _pnlProducts.SuspendLayout();
        _pnlProducts.Controls.Clear();

        var searchTerm = _txtSearch.Text.Trim();
        if (searchTerm == "🔍 Search products...") searchTerm = "";
        searchTerm = searchTerm.ToLower();

        var filtered = _allProducts.Where(p => 
            (!_selectedCategoryId.HasValue || p.CategoryId == _selectedCategoryId) &&
            (string.IsNullOrEmpty(searchTerm) || p.Name.ToLower().Contains(searchTerm))
        ).ToList();

        foreach (var p in filtered)
        {
            var tile = new ProductTileControl(p);
            tile.ProductClicked += (s, prod) => AddToCart(prod);
            _pnlProducts.Controls.Add(tile);
        }

        _pnlProducts.ResumeLayout();
    }

    private void ProcessBarcode(string barcode)
    {
        var product = _allProducts.FirstOrDefault(p => p.Barcode == barcode);
        if (product != null)
        {
            AddToCart(product);
        }
        else
        {
            UIHelper.ShowWarning($"Product with barcode '{barcode}' not found.");
        }
    }

    private void AddToCart(ProductDto product)
    {
        var existing = _cart.FirstOrDefault(c => c.ProductId == product.Id);
        if (existing != null)
        {
            existing.Quantity++;
        }
        else
        {
            _cart.Add(new CartItem { ProductId = product.Id, ProductName = product.Name, UnitPrice = product.SellingPrice, Quantity = 1 });
        }
        UpdateCartGrid();
    }

    private void UpdateCartGrid()
    {
        _gridCart.DataSource = null;
        _gridCart.DataSource = _cart;

        if (_gridCart.Columns["ProductId"] != null) _gridCart.Columns["ProductId"].Visible = false;
        
        if (_gridCart.Columns["ProductName"] != null)
            _gridCart.Columns["ProductName"].HeaderText = "Item";

        if (_gridCart.Columns["UnitPrice"] != null)
            _gridCart.Columns["UnitPrice"].DefaultCellStyle.Format = "C2";

        if (_gridCart.Columns["Total"] != null)
            _gridCart.Columns["Total"].DefaultCellStyle.Format = "C2";

        if (_gridCart.Columns["RemoveBtn"] == null)
        {
            _gridCart.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "RemoveBtn",
                HeaderText = "",
                Text = "X",
                UseColumnTextForButtonValue = true,
                Width = 40,
                FlatStyle = FlatStyle.Flat
            });
        }

        CalculateTotals();
    }

    private void GridCart_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

        if (_gridCart.Columns[e.ColumnIndex].Name == "RemoveBtn")
        {
            _cart.RemoveAt(e.RowIndex);
            UpdateCartGrid();
        }
    }

    private void CalculateTotals()
    {
        if (_isInitializing) return;
        
        decimal subTotal = _cart.Sum(c => c.Total);
        _lblSubTotal.Text = $"SubTotal: {UIHelper.FormatCurrency(subTotal)}";

        decimal.TryParse(_txtDiscount.Text, out decimal discPct);
        decimal.TryParse(_txtTax.Text, out decimal taxPct);
        decimal.TryParse(_txtPaidAmount.Text, out decimal paid);

        decimal discAmt = subTotal * (discPct / 100);
        decimal taxAmt = (subTotal - discAmt) * (taxPct / 100);
        decimal total = subTotal - discAmt + taxAmt;

        _lblTotal.Text = $"Total: {UIHelper.FormatCurrency(total)}";
        
        decimal change = paid - total;
        _lblChange.Text = $"Change: {UIHelper.FormatCurrency(change)}";
        _lblChange.ForeColor = change >= 0 ? AppTheme.TextSecondary : AppTheme.AccentDanger;
    }

    private void SelectPayment(PaymentMethod method)
    {
        _selectedPayment = method;
        if (method == PaymentMethod.Cash)
        {
            AppTheme.StylePrimaryButton(_btnCash);
            AppTheme.StyleSecondaryButton(_btnCard);
        }
        else
        {
            AppTheme.StyleSecondaryButton(_btnCash);
            AppTheme.StylePrimaryButton(_btnCard);
        }
    }

    private async Task CompleteOrderAsync()
    {
        if (!_cart.Any())
        {
            UIHelper.ShowWarning("Cart is empty.");
            return;
        }

        decimal.TryParse(_txtDiscount.Text, out decimal discPct);
        decimal.TryParse(_txtTax.Text, out decimal taxPct);
        decimal.TryParse(_txtPaidAmount.Text, out decimal paid);

        var dto = new CreateSaleDto(
            CustomerId: null,
            UserId: SessionManager.CurrentUserId ?? 1,
            Items: _cart.Select(c => new CreateSaleItemDto(c.ProductId, c.Quantity, c.UnitPrice, 0)).ToList(),
            DiscountPercent: discPct,
            TaxPercent: taxPct,
            PaidAmount: paid,
            PaymentMethod: _selectedPayment,
            Notes: ""
        );

        try
        {
            _btnComplete.Enabled = false;
            var sale = await _saleService.CreateSaleAsync(dto);
            UIHelper.ShowSuccess($"Order Complete!\nInvoice: {sale.InvoiceNumber}");
            
            // Clear cart
            _cart.Clear();
            _txtDiscount.Text = "0";
            _txtTax.Text = "0";
            _txtPaidAmount.Text = "0";
            _txtSearch.Clear();
            SelectPayment(PaymentMethod.Cash);
            UpdateCartGrid();
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to complete order: {ex.Message}");
        }
        finally
        {
            _btnComplete.Enabled = true;
        }
    }

    private class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
