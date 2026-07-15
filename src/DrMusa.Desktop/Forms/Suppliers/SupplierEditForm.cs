using DrMusa.Business.DTOs;
using DrMusa.Desktop.Helpers;

namespace DrMusa.Desktop.Forms.Suppliers;

public sealed class SupplierEditForm : Form
{
    private readonly SupplierDto? _existingSupplier;

    private TextBox _txtName = null!;
    private TextBox _txtContactPerson = null!;
    private TextBox _txtPhone = null!;
    private TextBox _txtEmail = null!;
    private TextBox _txtAddress = null!;
    private Button _btnSave = null!;
    private Button _btnCancel = null!;

    public SupplierDto SupplierDto { get; private set; } = new();

    public SupplierEditForm(SupplierDto? existingSupplier = null)
    {
        _existingSupplier = existingSupplier;
        InitializeComponent();
        
        if (_existingSupplier != null)
        {
            LoadSupplierData();
        }
    }

    private void InitializeComponent()
    {
        Text = _existingSupplier == null ? "DrMusa — Add Supplier" : "DrMusa — Edit Supplier";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(500, 560);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = AppTheme.BackgroundDark;
        ForeColor = AppTheme.TextPrimary;
        Font = AppTheme.FontBody;

        var headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = AppTheme.BackgroundPanel,
            Padding = new Padding(20, 15, 20, 15)
        };

        var lblTitle = new Label
        {
            Text = _existingSupplier == null ? "Add New Supplier" : "Edit Supplier",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 15)
        };
        var lblSubtitle = new Label
        {
            Text = _existingSupplier == null ? "Enter details for the new supplier." : "Update existing supplier details.",
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 45)
        };
        headerPanel.Controls.Add(lblTitle);
        headerPanel.Controls.Add(lblSubtitle);

        var contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(30, 20, 30, 20),
            BackColor = AppTheme.BackgroundDark
        };

        int startY = 20;
        int spacing = 65;

        // Name
        var lblName = new Label { Text = "Supplier Name *", Location = new Point(30, startY), AutoSize = true, ForeColor = AppTheme.TextPrimary };
        _txtName = new TextBox();
        var pnlName = AppTheme.WrapInputPanel(_txtName);
        pnlName.Location = new Point(30, startY + 25); pnlName.Width = 420;
        contentPanel.Controls.Add(lblName);
        contentPanel.Controls.Add(pnlName);
        startY += spacing;

        // Contact Person
        var lblContactPerson = new Label { Text = "Contact Person", Location = new Point(30, startY), AutoSize = true, ForeColor = AppTheme.TextPrimary };
        _txtContactPerson = new TextBox();
        var pnlContactPerson = AppTheme.WrapInputPanel(_txtContactPerson);
        pnlContactPerson.Location = new Point(30, startY + 25); pnlContactPerson.Width = 420;
        contentPanel.Controls.Add(lblContactPerson);
        contentPanel.Controls.Add(pnlContactPerson);
        startY += spacing;

        // Phone
        var lblPhone = new Label { Text = "Phone Number", Location = new Point(30, startY), AutoSize = true, ForeColor = AppTheme.TextPrimary };
        _txtPhone = new TextBox();
        var pnlPhone = AppTheme.WrapInputPanel(_txtPhone);
        pnlPhone.Location = new Point(30, startY + 25); pnlPhone.Width = 420;
        contentPanel.Controls.Add(lblPhone);
        contentPanel.Controls.Add(pnlPhone);
        startY += spacing;

        // Email
        var lblEmail = new Label { Text = "Email Address", Location = new Point(30, startY), AutoSize = true, ForeColor = AppTheme.TextPrimary };
        _txtEmail = new TextBox();
        var pnlEmail = AppTheme.WrapInputPanel(_txtEmail);
        pnlEmail.Location = new Point(30, startY + 25); pnlEmail.Width = 420;
        contentPanel.Controls.Add(lblEmail);
        contentPanel.Controls.Add(pnlEmail);
        startY += spacing;

        // Address
        var lblAddress = new Label { Text = "Address", Location = new Point(30, startY), AutoSize = true, ForeColor = AppTheme.TextPrimary };
        _txtAddress = new TextBox { Multiline = true, Height = 60 };
        var pnlAddress = AppTheme.WrapInputPanel(_txtAddress);
        pnlAddress.Location = new Point(30, startY + 25); pnlAddress.Width = 420; pnlAddress.Height = 70;
        contentPanel.Controls.Add(lblAddress);
        contentPanel.Controls.Add(pnlAddress);

        var bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 70,
            BackColor = AppTheme.BackgroundPanel
        };

        _btnSave = new Button { Text = "Save Supplier", Width = 140, Height = 40, Location = new Point(310, 15) };
        AppTheme.StylePrimaryButton(_btnSave);
        _btnSave.Click += BtnSave_Click;

        _btnCancel = new Button { Text = "Cancel", Width = 100, Height = 40, Location = new Point(200, 15) };
        AppTheme.StyleSecondaryButton(_btnCancel);
        _btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };

        bottomPanel.Controls.Add(_btnSave);
        bottomPanel.Controls.Add(_btnCancel);

        Controls.Add(contentPanel);
        Controls.Add(bottomPanel);
        Controls.Add(headerPanel);
    }

    private void LoadSupplierData()
    {
        if (_existingSupplier == null) return;

        _txtName.Text = _existingSupplier.Name;
        _txtContactPerson.Text = _existingSupplier.ContactPerson;
        _txtPhone.Text = _existingSupplier.Phone;
        _txtEmail.Text = _existingSupplier.Email;
        _txtAddress.Text = _existingSupplier.Address;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtName.Text))
        {
            MessageBox.Show("Supplier Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtName.Focus();
            return;
        }

        SupplierDto = new SupplierDto
        {
            Id = _existingSupplier?.Id ?? 0,
            Name = _txtName.Text.Trim(),
            ContactPerson = string.IsNullOrWhiteSpace(_txtContactPerson.Text) ? null : _txtContactPerson.Text.Trim(),
            Phone = string.IsNullOrWhiteSpace(_txtPhone.Text) ? null : _txtPhone.Text.Trim(),
            Email = string.IsNullOrWhiteSpace(_txtEmail.Text) ? null : _txtEmail.Text.Trim(),
            Address = string.IsNullOrWhiteSpace(_txtAddress.Text) ? null : _txtAddress.Text.Trim(),
            IsActive = _existingSupplier?.IsActive ?? true
        };

        DialogResult = DialogResult.OK;
        Close();
    }
}
