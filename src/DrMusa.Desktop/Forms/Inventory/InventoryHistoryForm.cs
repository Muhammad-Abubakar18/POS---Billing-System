using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Inventory;

public sealed class InventoryHistoryForm : Form
{
    private readonly IInventoryService _inventoryService;
    private readonly int _productId;

    private DataGridView _grid = null!;

    public InventoryHistoryForm(IServiceProvider serviceProvider, int productId)
    {
        _inventoryService = serviceProvider.GetRequiredService<IInventoryService>();
        _productId = productId;

        InitializeComponent();
        Shown += async (s, e) => await LoadHistoryAsync();
    }

    private void InitializeComponent()
    {
        Text = "Inventory History";
        Size = new Size(800, 500);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = AppTheme.BackgroundDark;
        ForeColor = AppTheme.TextPrimary;
        Font = AppTheme.FontBody;

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = AppTheme.BackgroundPanel,
            Padding = new Padding(20)
        };
        var lblTitle = new Label
        {
            Text = "Movement History",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 15)
        };
        header.Controls.Add(lblTitle);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = AppTheme.BackgroundDark,
            BorderStyle = BorderStyle.None,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            RowTemplate = { Height = 40 },
            ColumnHeadersHeight = 40,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundPanel,
                ForeColor = AppTheme.TextSecondary,
                Font = AppTheme.FontBodyBold
            },
            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.BackgroundCard,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.FontBody,
                SelectionBackColor = AppTheme.AccentPrimary,
                SelectionForeColor = Color.White
            }
        };

        var container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        container.Controls.Add(_grid);

        Controls.Add(container);
        Controls.Add(header);
    }

    private async Task LoadHistoryAsync()
    {
        try
        {
            var history = await _inventoryService.GetHistoryAsync(_productId);
            var list = history.Select(h => new
            {
                Date = h.CreatedAt,
                Type = h.MovementType.ToString(),
                Qty = h.Quantity,
                Before = h.StockBefore,
                After = h.StockAfter,
                h.Notes
            }).OrderByDescending(h => h.Date).ToList();

            _grid.DataSource = list;

            if (_grid.Columns["Date"] != null)
                _grid.Columns["Date"].DefaultCellStyle.Format = "g";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading history: {ex.Message}");
        }
    }
}
