using DrMusa.Business.DTOs;
using DrMusa.Desktop.Helpers;

namespace DrMusa.Desktop.Forms.Billing;

public class ProductTileControl : UserControl
{
    private readonly ProductDto _product;
    public event EventHandler<ProductDto>? ProductClicked;

    public ProductTileControl(ProductDto product)
    {
        _product = product;
        BuildUI();
    }

    private void BuildUI()
    {
        Size = new Size(160, 200);
        Margin = new Padding(10);
        BackColor = AppTheme.BackgroundCard;
        Cursor = Cursors.Hand;

        var pnlImage = new Panel
        {
            Dock = DockStyle.Top,
            Height = 120,
            BackColor = AppTheme.BackgroundInput
        };

        if (!string.IsNullOrEmpty(_product.ImagePath) && File.Exists(_product.ImagePath))
        {
            try
            {
                var pb = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = Image.FromFile(_product.ImagePath)
                };
                pb.Click += (s, e) => ProductClicked?.Invoke(this, _product);
                pnlImage.Controls.Add(pb);
            }
            catch
            {
                AddFallbackImage(pnlImage);
            }
        }
        else
        {
            AddFallbackImage(pnlImage);
        }

        var lblName = new Label
        {
            Text = _product.Name,
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = AppTheme.TextPrimary,
            AutoEllipsis = true
        };

        var lblPrice = new Label
        {
            Text = UIHelper.FormatCurrency(_product.SellingPrice),
            Dock = DockStyle.Top,
            Height = 30,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = AppTheme.AccentPrimary
        };

        // Forward clicks from all child controls
        pnlImage.Click += (s, e) => ProductClicked?.Invoke(this, _product);
        lblName.Click += (s, e) => ProductClicked?.Invoke(this, _product);
        lblPrice.Click += (s, e) => ProductClicked?.Invoke(this, _product);
        this.Click += (s, e) => ProductClicked?.Invoke(this, _product);

        Controls.Add(lblPrice);
        Controls.Add(lblName);
        Controls.Add(pnlImage);

        Paint += (s, e) =>
        {
            using var pen = new Pen(AppTheme.BorderDefault, 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        };
    }

    private void AddFallbackImage(Panel pnl)
    {
        var lbl = new Label
        {
            Text = _product.Name.Length > 2 ? _product.Name.Substring(0, 2).ToUpper() : _product.Name.ToUpper(),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 24f, FontStyle.Bold),
            ForeColor = AppTheme.TextSecondary,
            BackColor = AppTheme.BackgroundInput
        };
        lbl.Click += (s, e) => ProductClicked?.Invoke(this, _product);
        pnl.Controls.Add(lbl);
    }
}
