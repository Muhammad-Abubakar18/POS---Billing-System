using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Settings;

public partial class SettingsForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISettingService _settingService;

    // UI Controls
    private TextBox _txtBusinessName = null!;
    private TextBox _txtBusinessPhone = null!;
    private TextBox _txtBusinessAddress = null!;
    private TextBox _txtCurrency = null!;
    private TextBox _txtTaxPercent = null!;

    private TextBox _txtReceiptHeader = null!;
    private TextBox _txtReceiptFooter = null!;
    private PictureBox _picLogo = null!;
    private string? _currentLogoBase64;
    private TextBox _txtAutoBackupDir = null!;
    private Label _lblLastBackup = null!;

    // Printer Settings Controls
    private ComboBox _cmbPrintMode = null!;
    private ComboBox _cmbCashierPrinter = null!;
    private ComboBox _cmbKitchenPrinter = null!;

    // Security Controls
    private ComboBox _cmbQ1 = null!;
    private TextBox _txtA1 = null!;
    private ComboBox _cmbQ2 = null!;
    private TextBox _txtA2 = null!;
    private ComboBox _cmbQ3 = null!;
    private TextBox _txtA3 = null!;
    private TextBox _txtMasterKey = null!;

    public SettingsForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _settingService = serviceProvider.GetRequiredService<ISettingService>();

        InitializeComponent();
        this.Load += async (s, e) => await LoadSettingsAsync();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Settings";
        this.BackColor = AppTheme.BackgroundDark;
        this.Font = AppTheme.FontBody;
        this.Size = new Size(1000, 750);
        this.StartPosition = FormStartPosition.CenterParent;

        var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        this.Controls.Add(mainPanel);

        var lblTitle = new Label { Text = "System Settings", Font = AppTheme.FontTitle, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(20, 20) };
        mainPanel.Controls.Add(lblTitle);

        var tabs = new TabControl
        {
            Location = new Point(20, 70),
            Size = new Size(920, 520),
            Font = new Font("Segoe UI", 11f)
        };
        mainPanel.Controls.Add(tabs);

        // --- Tab: General ---
        var tabGeneral = new TabPage("General Info");
        tabGeneral.BackColor = AppTheme.BackgroundPanel;
        
        _txtBusinessName = new TextBox { Width = 300, Font = new Font("Segoe UI", 11f) };
        _txtBusinessPhone = new TextBox { Width = 300, Font = new Font("Segoe UI", 11f) };
        _txtBusinessAddress = new TextBox { Width = 300, Multiline = true, Height = 60, Font = new Font("Segoe UI", 11f) };
        _txtCurrency = new TextBox { Width = 100, Font = new Font("Segoe UI", 11f) };
        _txtTaxPercent = new TextBox { Width = 100, Font = new Font("Segoe UI", 11f) };

        var layoutGeneral = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 2, RowCount = 6, AutoSize = true };
        layoutGeneral.Controls.Add(new Label { Text = "Business Name", ForeColor = AppTheme.TextPrimary, AutoSize = true }, 0, 0);
        layoutGeneral.Controls.Add(_txtBusinessName, 1, 0);
        layoutGeneral.Controls.Add(new Label { Text = "Phone", ForeColor = AppTheme.TextPrimary, AutoSize = true }, 0, 1);
        layoutGeneral.Controls.Add(_txtBusinessPhone, 1, 1);
        layoutGeneral.Controls.Add(new Label { Text = "Address", ForeColor = AppTheme.TextPrimary, AutoSize = true }, 0, 2);
        layoutGeneral.Controls.Add(_txtBusinessAddress, 1, 2);
        layoutGeneral.Controls.Add(new Label { Text = "Currency Symbol", ForeColor = AppTheme.TextPrimary, AutoSize = true }, 0, 3);
        layoutGeneral.Controls.Add(_txtCurrency, 1, 3);
        layoutGeneral.Controls.Add(new Label { Text = "Default Tax %", ForeColor = AppTheme.TextPrimary, AutoSize = true }, 0, 4);
        layoutGeneral.Controls.Add(_txtTaxPercent, 1, 4);

        tabGeneral.Controls.Add(layoutGeneral);
        tabs.TabPages.Add(tabGeneral);

        // --- Tab: Receipt ---
        var tabReceipt = new TabPage("Receipt Customization");
        tabReceipt.BackColor = AppTheme.BackgroundPanel;

        _txtReceiptHeader = new TextBox { Width = 400, Multiline = true, Height = 60, Font = new Font("Segoe UI", 11f) };
        _txtReceiptFooter = new TextBox { Width = 400, Multiline = true, Height = 60, Font = new Font("Segoe UI", 11f) };

        _picLogo = new PictureBox
        {
            Width = 150, Height = 150,
            BorderStyle = BorderStyle.FixedSingle,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.White
        };

        var btnBrowse = new Button { Text = "Browse Logo", Width = 120, Height = 35 };
        AppTheme.StyleSecondaryButton(btnBrowse);
        btnBrowse.Click += BtnBrowse_Click;

        var btnClear = new Button { Text = "Clear Logo", Width = 120, Height = 35 };
        AppTheme.StyleDangerButton(btnClear);
        btnClear.Click += (s, e) => { _picLogo.Image = null; _currentLogoBase64 = null; };

        var logoPanel = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown };
        logoPanel.Controls.Add(_picLogo);
        var btnPanel = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
        btnPanel.Controls.Add(btnBrowse);
        btnPanel.Controls.Add(btnClear);
        logoPanel.Controls.Add(btnPanel);

        var layoutReceipt = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 2, RowCount = 3, AutoSize = true };
        layoutReceipt.Controls.Add(new Label { Text = "Receipt Header", ForeColor = AppTheme.TextPrimary, AutoSize = true }, 0, 0);
        layoutReceipt.Controls.Add(_txtReceiptHeader, 1, 0);
        layoutReceipt.Controls.Add(new Label { Text = "Receipt Footer", ForeColor = AppTheme.TextPrimary, AutoSize = true }, 0, 1);
        layoutReceipt.Controls.Add(_txtReceiptFooter, 1, 1);
        layoutReceipt.Controls.Add(new Label { Text = "Business Logo", ForeColor = AppTheme.TextPrimary, AutoSize = true }, 0, 2);
        layoutReceipt.Controls.Add(logoPanel, 1, 2);

        tabReceipt.Controls.Add(layoutReceipt);
        tabs.TabPages.Add(tabReceipt);

        // --- Tab: Printer Settings ---
        var tabPrinter = new TabPage("Printer Settings");
        tabPrinter.BackColor = AppTheme.BackgroundPanel;

        var layoutPrinter = new TableLayoutPanel { Dock = DockStyle.Top, Padding = new Padding(20), ColumnCount = 2, RowCount = 3, AutoSize = true, Height = 150 };
        layoutPrinter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        layoutPrinter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _cmbPrintMode = new ComboBox { Width = 400, Font = new Font("Segoe UI", 11f), DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbPrintMode.Items.AddRange(new[] 
        { 
            "1 - Two Printers (Separate Cashier & Kitchen)",
            "2 - Single Printer Direct (Bypass Preview)",
            "3 - Single Printer (Show Preview)"
        });
        _cmbPrintMode.SelectedIndexChanged += (s, e) =>
        {
            bool showPrinters = _cmbPrintMode.SelectedIndex == 0 || _cmbPrintMode.SelectedIndex == 1;
            _cmbCashierPrinter.Enabled = showPrinters;
            _cmbKitchenPrinter.Enabled = showPrinters;
        };

        _cmbCashierPrinter = new ComboBox { Width = 400, Font = new Font("Segoe UI", 11f), DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbKitchenPrinter = new ComboBox { Width = 400, Font = new Font("Segoe UI", 11f), DropDownStyle = ComboBoxStyle.DropDownList };
        
        try 
        {
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                _cmbCashierPrinter.Items.Add(printer);
                _cmbKitchenPrinter.Items.Add(printer);
            }
        }
        catch { /* Ignore if WMI or printer spooler fails */ }

        layoutPrinter.Controls.Add(new Label { Text = "Print Mode", ForeColor = AppTheme.TextPrimary, AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        layoutPrinter.Controls.Add(_cmbPrintMode, 1, 0);
        
        layoutPrinter.Controls.Add(new Label { Text = "Cashier Printer", ForeColor = AppTheme.TextPrimary, AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        layoutPrinter.Controls.Add(_cmbCashierPrinter, 1, 1);
        
        layoutPrinter.Controls.Add(new Label { Text = "Kitchen Printer", ForeColor = AppTheme.TextPrimary, AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        layoutPrinter.Controls.Add(_cmbKitchenPrinter, 1, 2);

        var pnlGuide = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 0, 20, 20) };
        var txtGuide = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 11f),
            BackColor = AppTheme.BackgroundPanel,
            ForeColor = AppTheme.TextPrimary,
            Text = @"How to Setup Your Receipt Printer

1. USB Connection: Plug your thermal printer (e.g., EPSON TM-T88, Xprinter, generic POS-58 or POS-80) into your computer via USB.
2. Install Drivers: Install the official Windows drivers provided by your printer's manufacturer.
3. Windows Setup:
   - Go to Windows Settings > Devices > Printers & scanners.
   - You should see your newly installed printer. Click on it and select 'Manage', then 'Set as default' (Optional).
   - Click 'Printer properties'.
   - Go to 'Device Settings' or 'Advanced' and ensure the default paper size is set appropriately (e.g., 80x297mm or 58x297mm).
4. Testing:
   - In DrMusa POS, completing an order and selecting 'Yes' for receipt will trigger a Print Preview. 
   - Click the small 'Print' icon in the upper-left of the preview window to send it to the printer.
   - If the receipt cuts off early or is too narrow, adjust your Paper Size in the Windows Printer Properties."
        };
        pnlGuide.Controls.Add(txtGuide);
        
        tabPrinter.Controls.Add(pnlGuide);
        tabPrinter.Controls.Add(layoutPrinter);
        tabs.TabPages.Add(tabPrinter);

        // --- Tab: Database & Security ---
        var tabDb = new TabPage("Database & Security");
        tabDb.BackColor = AppTheme.BackgroundPanel;

        var lblDbInfo = new Label
        {
            Text = "Manage your POS Database backups. Automatic backups are taken daily upon startup.",
            Font = new Font("Segoe UI", 11f),
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            MaximumSize = new Size(880, 0),
            Location = new Point(20, 20)
        };
        tabDb.Controls.Add(lblDbInfo);

        var btnBackup = new Button { Text = "Manual Backup", Width = 200, Height = 45, Location = new Point(20, 75) };
        AppTheme.StyleSecondaryButton(btnBackup);
        btnBackup.Click += BtnBackup_Click;
        tabDb.Controls.Add(btnBackup);

        var btnRestore = new Button { Text = "Restore Database", Width = 200, Height = 45, Location = new Point(240, 75) };
        AppTheme.StyleDangerButton(btnRestore);
        btnRestore.Click += BtnRestore_Click;
        tabDb.Controls.Add(btnRestore);

        tabs.TabPages.Add(tabDb);

        var lblAutoBackup = new Label
        {
            Text = "Automatic Backup Directory (e.g., Google Drive sync folder):",
            Font = new Font("Segoe UI", 11f),
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            MaximumSize = new Size(880, 0),
            Location = new Point(20, 135)
        };
        tabDb.Controls.Add(lblAutoBackup);

        _txtAutoBackupDir = new TextBox { Width = 400, Font = new Font("Segoe UI", 11f), Location = new Point(20, 165), ReadOnly = true };
        tabDb.Controls.Add(_txtAutoBackupDir);

        var btnBrowseAutoBackup = new Button { Text = "Browse...", Width = 100, Height = 30, Location = new Point(430, 164) };
        AppTheme.StyleSecondaryButton(btnBrowseAutoBackup);
        btnBrowseAutoBackup.Click += (s, e) =>
        {
            using var fbd = new FolderBrowserDialog { Description = "Select Automatic Backup Directory" };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _txtAutoBackupDir.Text = fbd.SelectedPath;
            }
        };
        tabDb.Controls.Add(btnBrowseAutoBackup);

        var btnClearAutoBackup = new Button { Text = "Clear", Width = 80, Height = 30, Location = new Point(540, 164) };
        AppTheme.StyleDangerButton(btnClearAutoBackup);
        btnClearAutoBackup.Click += (s, e) => _txtAutoBackupDir.Text = "";
        tabDb.Controls.Add(btnClearAutoBackup);

        // --- Tab: Admin Security ---
        var tabSecurity = new TabPage("Admin Security");
        tabSecurity.BackColor = AppTheme.BackgroundPanel;

        var lblSecInfo = new Label
        {
            Text = "Configure Password Recovery options. These are used if you forget your password.",
            Font = new Font("Segoe UI", 11f),
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            MaximumSize = new Size(880, 0),
            Location = new Point(20, 20)
        };
        tabSecurity.Controls.Add(lblSecInfo);

        string[] questions = new[]
        {
            "What city were you born in?",
            "What is your mother's maiden name?",
            "What was the name of your first pet?",
            "What was the name of your first school?",
            "What is your favorite childhood friend's name?"
        };

        _cmbQ1 = new ComboBox { Width = 380, Location = new Point(20, 85), Font = new Font("Segoe UI", 11f), DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbQ1.Items.AddRange(questions);
        _txtA1 = new TextBox { Width = 380, Location = new Point(440, 85), Font = new Font("Segoe UI", 11f) };

        _cmbQ2 = new ComboBox { Width = 380, Location = new Point(20, 165), Font = new Font("Segoe UI", 11f), DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbQ2.Items.AddRange(questions);
        _txtA2 = new TextBox { Width = 380, Location = new Point(440, 165), Font = new Font("Segoe UI", 11f) };

        _cmbQ3 = new ComboBox { Width = 380, Location = new Point(20, 245), Font = new Font("Segoe UI", 11f), DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbQ3.Items.AddRange(questions);
        _txtA3 = new TextBox { Width = 380, Location = new Point(440, 245), Font = new Font("Segoe UI", 11f) };

        tabSecurity.Controls.Add(new Label { Text = "Question 1:", Location = new Point(20, 60), ForeColor = AppTheme.TextPrimary, AutoSize = true, Font = new Font("Segoe UI", 10f) });
        tabSecurity.Controls.Add(_cmbQ1);
        tabSecurity.Controls.Add(new Label { Text = "Answer (Leave empty to keep existing):", Location = new Point(440, 60), ForeColor = AppTheme.TextPrimary, AutoSize = true, Font = new Font("Segoe UI", 10f) });
        tabSecurity.Controls.Add(_txtA1);

        tabSecurity.Controls.Add(new Label { Text = "Question 2:", Location = new Point(20, 140), ForeColor = AppTheme.TextPrimary, AutoSize = true, Font = new Font("Segoe UI", 10f) });
        tabSecurity.Controls.Add(_cmbQ2);
        tabSecurity.Controls.Add(new Label { Text = "Answer (Leave empty to keep existing):", Location = new Point(440, 140), ForeColor = AppTheme.TextPrimary, AutoSize = true, Font = new Font("Segoe UI", 10f) });
        tabSecurity.Controls.Add(_txtA2);

        tabSecurity.Controls.Add(new Label { Text = "Question 3:", Location = new Point(20, 220), ForeColor = AppTheme.TextPrimary, AutoSize = true, Font = new Font("Segoe UI", 10f) });
        tabSecurity.Controls.Add(_cmbQ3);
        tabSecurity.Controls.Add(new Label { Text = "Answer (Leave empty to keep existing):", Location = new Point(440, 220), ForeColor = AppTheme.TextPrimary, AutoSize = true, Font = new Font("Segoe UI", 10f) });
        tabSecurity.Controls.Add(_txtA3);

        var lblMasterKey = new Label { Text = "Master Recovery Key (Type your own or click Generate):", Location = new Point(20, 310), ForeColor = AppTheme.TextPrimary, AutoSize = true, Font = new Font("Segoe UI", 10f) };
        tabSecurity.Controls.Add(lblMasterKey);

        _txtMasterKey = new TextBox { Width = 380, Location = new Point(20, 335), Font = new Font("Consolas", 14f, FontStyle.Bold), ReadOnly = false, TextAlign = HorizontalAlignment.Center };
        tabSecurity.Controls.Add(_txtMasterKey);

        var btnGenerateKey = new Button { Text = "Generate Random Key", Width = 200, Height = 34, Location = new Point(440, 334) };
        AppTheme.StyleDangerButton(btnGenerateKey);
        btnGenerateKey.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        btnGenerateKey.Click += (s, e) =>
        {
            var confirm = MessageBox.Show("Generating a new key will invalidate any old key. Continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                _txtMasterKey.Text = "DRMUSA-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper() + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
            }
        };
        tabSecurity.Controls.Add(btnGenerateKey);

        tabs.TabPages.Add(tabSecurity);

        // --- Tab: Data Management ---
        var tabData = new TabPage("Data Management");
        tabData.BackColor = AppTheme.BackgroundPanel;

        var lblDataInfo = new Label
        {
            Text = "DANGER ZONE: Wiping sales will permanently delete revenue data and receipt history.",
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = AppTheme.AccentDanger,
            AutoSize = true,
            MaximumSize = new Size(880, 0),
            Location = new Point(20, 20)
        };
        tabData.Controls.Add(lblDataInfo);

        var btnResetToday = new Button { Text = "Reset Today's Sales", Width = 200, Height = 45, Location = new Point(20, 85) };
        AppTheme.StyleDangerButton(btnResetToday);
        btnResetToday.Click += BtnResetToday_Click;
        tabData.Controls.Add(btnResetToday);

        var btnResetMonth = new Button { Text = "Reset This Month's Sales", Width = 220, Height = 45, Location = new Point(240, 85) };
        AppTheme.StyleDangerButton(btnResetMonth);
        btnResetMonth.Click += BtnResetMonth_Click;
        tabData.Controls.Add(btnResetMonth);

        var btnFactoryReset = new Button { Text = "Factory Reset (Wipe All Data)", Width = 250, Height = 45, Location = new Point(480, 85) };
        AppTheme.StyleDangerButton(btnFactoryReset);
        btnFactoryReset.Click += BtnFactoryReset_Click;
        tabData.Controls.Add(btnFactoryReset);

        tabs.TabPages.Add(tabData);

        // Last Backup Label
        _lblLastBackup = new Label
        {
            Text = "Last Backup Taken: Checking...",
            Font = new Font("Segoe UI", 10f, FontStyle.Italic),
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(20, 205)
        };
        tabDb.Controls.Add(_lblLastBackup);

        // --- Bottom Action ---
        var btnSave = new Button { Text = "Save Settings", Width = 150, Height = 45, Location = new Point(20, 610) };
        AppTheme.StylePrimaryButton(btnSave);
        btnSave.Click += async (s, e) => await SaveSettingsAsync();
        mainPanel.Controls.Add(btnSave);
    }

    private async Task LoadSettingsAsync()
    {
        var settings = (await _settingService.GetAllAsync()).ToDictionary(s => s.Key, s => s.Value);

        _txtBusinessName.Text = settings.GetValueOrDefault("BusinessName", "DrMusa Store");
        _txtBusinessPhone.Text = settings.GetValueOrDefault("BusinessPhone", "");
        _txtBusinessAddress.Text = settings.GetValueOrDefault("BusinessAddress", "");
        _txtCurrency.Text = settings.GetValueOrDefault("Currency", "PKR");
        _txtTaxPercent.Text = settings.GetValueOrDefault("TaxPercent", "0");
        _txtReceiptHeader.Text = settings.GetValueOrDefault("ReceiptHeader", "Thank you for shopping!");
        _txtReceiptFooter.Text = settings.GetValueOrDefault("ReceiptFooter", "Please come again");
        _txtAutoBackupDir.Text = settings.GetValueOrDefault("AutoBackupDirectory", "");

        if (int.TryParse(settings.GetValueOrDefault("PrintMode", "3"), out int modeIndex) && modeIndex >= 1 && modeIndex <= 3)
        {
            _cmbPrintMode.SelectedIndex = modeIndex - 1; // 1-based to 0-based
        }
        else
        {
            _cmbPrintMode.SelectedIndex = 2; // Default to Mode 3
        }
        
        var cPrinter = settings.GetValueOrDefault("CashierPrinter", "");
        if (!string.IsNullOrEmpty(cPrinter) && _cmbCashierPrinter.Items.Contains(cPrinter))
            _cmbCashierPrinter.SelectedItem = cPrinter;
            
        var kPrinter = settings.GetValueOrDefault("KitchenPrinter", "");
        if (!string.IsNullOrEmpty(kPrinter) && _cmbKitchenPrinter.Items.Contains(kPrinter))
            _cmbKitchenPrinter.SelectedItem = kPrinter;

        if (settings.TryGetValue("AdminRecoveryQ1", out string? q1) && !string.IsNullOrEmpty(q1)) _cmbQ1.SelectedItem = q1;
        if (settings.TryGetValue("AdminRecoveryQ2", out string? q2) && !string.IsNullOrEmpty(q2)) _cmbQ2.SelectedItem = q2;
        if (settings.TryGetValue("AdminRecoveryQ3", out string? q3) && !string.IsNullOrEmpty(q3)) _cmbQ3.SelectedItem = q3;
        // Do NOT populate answers or master key for security reasons. Only save if they are changed.

        if (settings.TryGetValue("BusinessLogo", out string? logoBase64) && !string.IsNullOrEmpty(logoBase64))
        {
            try
            {
                var bytes = Convert.FromBase64String(logoBase64);
                using var ms = new MemoryStream(bytes);
                _picLogo.Image = Image.FromStream(ms);
                _currentLogoBase64 = logoBase64;
            }
            catch { }
        }

        RefreshLastBackupTime();
    }


    private async void BtnResetToday_Click(object? sender, EventArgs e)
    {
        using var authForm = new DrMusa.Desktop.Forms.Login.AdminAuthForm(_serviceProvider);
        if (authForm.ShowDialog() != DialogResult.OK) return;

        var confirm = MessageBox.Show("Are you ABSOLUTELY sure you want to delete all sales recorded today? This action cannot be undone.", "Delete Today's Sales", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        try
        {
            var saleService = _serviceProvider.GetRequiredService<ISaleService>();
            var today = DateTime.Today;
            await saleService.DeleteSalesAsync(today, today.AddDays(1), restoreStock: true);
            MessageBox.Show("Today's sales have been successfully deleted. Stock has been restored.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error deleting sales: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnResetMonth_Click(object? sender, EventArgs e)
    {
        using var authForm = new DrMusa.Desktop.Forms.Login.AdminAuthForm(_serviceProvider);
        if (authForm.ShowDialog() != DialogResult.OK) return;

        var confirm = MessageBox.Show("Are you ABSOLUTELY sure you want to delete all sales recorded this month? This action cannot be undone.", "Delete Monthly Sales", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        try
        {
            var saleService = _serviceProvider.GetRequiredService<ISaleService>();
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            await saleService.DeleteSalesAsync(monthStart, today.AddDays(1), restoreStock: true);
            MessageBox.Show("This month's sales have been successfully deleted. Stock has been restored.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error deleting sales: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnFactoryReset_Click(object? sender, EventArgs e)
    {
        using var authForm = new DrMusa.Desktop.Forms.Login.AdminAuthForm(_serviceProvider);
        if (authForm.ShowDialog() != DialogResult.OK) return;

        var confirm1 = MessageBox.Show(
            "CRITICAL WARNING: This will permanently delete ALL data (Products, Sales, Categories, Users) and reset the system to its initial state.\n\nAre you absolutely sure you want to proceed?",
            "Factory Reset - Step 1", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        
        if (confirm1 != DialogResult.Yes) return;

        var confirm2 = MessageBox.Show(
            "FINAL WARNING: This action CANNOT be undone. You will lose all your configurations, inventory, and sales history. The app will restart after reset.\n\nProceed with Factory Reset?",
            "Factory Reset - Final", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);

        if (confirm2 != DialogResult.Yes) return;

        try
        {
            var dbContext = _serviceProvider.GetRequiredService<DrMusa.Data.Context.DrMusaDbContext>();
            
            // Clear active EF connections to release file lock
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            
            // Wipe database
            await dbContext.Database.EnsureDeletedAsync();
            
            // Reseed default data (this also runs migrations)
            await DrMusa.Data.Seed.DbSeeder.SeedAsync(dbContext);

            MessageBox.Show("Factory Reset complete. The system will now restart.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            Application.Restart();
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error performing factory reset: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RefreshLastBackupTime()
    {
        try
        {
            var dbPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "database", "DrMusa.db"));
            var defaultBackupDir = Path.Combine(Path.GetDirectoryName(dbPath)!, "backups");
            
            string searchDir = !string.IsNullOrWhiteSpace(_txtAutoBackupDir.Text) && Directory.Exists(_txtAutoBackupDir.Text)
                ? _txtAutoBackupDir.Text 
                : defaultBackupDir;

            if (Directory.Exists(searchDir))
            {
                var files = Directory.GetFiles(searchDir, "*.db");
                if (files.Length > 0)
                {
                    var lastFile = files.Select(f => new FileInfo(f)).OrderByDescending(f => f.CreationTime).First();
                    _lblLastBackup.Text = $"Last Backup Taken: {lastFile.CreationTime:dddd, MMMM dd, yyyy 'at' hh:mm tt}";
                    return;
                }
            }
            _lblLastBackup.Text = "Last Backup Taken: No backups found.";
        }
        catch 
        {
            _lblLastBackup.Text = "Last Backup Taken: Unable to read directory.";
        }
    }

    private void BtnBrowse_Click(object? sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
            Title = "Select Business Logo"
        };

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var img = Image.FromFile(dlg.FileName);
                // Resize if it's too large to save space in DB
                if (img.Width > 500 || img.Height > 500)
                {
                    var newWidth = 300;
                    var newHeight = (int)((float)img.Height / img.Width * newWidth);
                    img = new Bitmap(img, new Size(newWidth, newHeight));
                }

                _picLogo.Image = img;
                
                using var ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                _currentLogoBase64 = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Failed to load image: {ex.Message}");
            }
        }
    }

    private async Task SaveSettingsAsync()
    {
        if (decimal.TryParse(_txtTaxPercent.Text.Trim(), out decimal tax))
        {
            if (tax < 0 || tax > 100)
            {
                UIHelper.ShowError("Sale Tax must be between 0 and 100.");
                return;
            }
        }
        else
        {
            UIHelper.ShowError("Invalid Sale Tax percentage.");
            return;
        }

        try
        {
            await _settingService.SetValueAsync("BusinessName", _txtBusinessName.Text.Trim());
            await _settingService.SetValueAsync("BusinessPhone", _txtBusinessPhone.Text.Trim());
            await _settingService.SetValueAsync("BusinessAddress", _txtBusinessAddress.Text.Trim());
            await _settingService.SetValueAsync("Currency", _txtCurrency.Text.Trim());
            await _settingService.SetValueAsync("TaxPercent", _txtTaxPercent.Text.Trim());
            await _settingService.SetValueAsync("ReceiptHeader", _txtReceiptHeader.Text.Trim());
            await _settingService.SetValueAsync("ReceiptFooter", _txtReceiptFooter.Text.Trim());
            await _settingService.SetValueAsync("BusinessLogo", _currentLogoBase64 ?? "");
            await _settingService.SetValueAsync("AutoBackupDirectory", _txtAutoBackupDir.Text.Trim());

            // Printer Settings
            await _settingService.SetValueAsync("PrintMode", (_cmbPrintMode.SelectedIndex + 1).ToString());
            if (_cmbCashierPrinter.SelectedItem != null)
                await _settingService.SetValueAsync("CashierPrinter", _cmbCashierPrinter.SelectedItem.ToString()!);
            if (_cmbKitchenPrinter.SelectedItem != null)
                await _settingService.SetValueAsync("KitchenPrinter", _cmbKitchenPrinter.SelectedItem.ToString()!);

            // Save Security Questions
            if (_cmbQ1.SelectedItem != null) await _settingService.SetValueAsync("AdminRecoveryQ1", _cmbQ1.SelectedItem.ToString()!);
            if (_cmbQ2.SelectedItem != null) await _settingService.SetValueAsync("AdminRecoveryQ2", _cmbQ2.SelectedItem.ToString()!);
            if (_cmbQ3.SelectedItem != null) await _settingService.SetValueAsync("AdminRecoveryQ3", _cmbQ3.SelectedItem.ToString()!);

            // Only hash and save if they actually entered a new answer
            if (!string.IsNullOrWhiteSpace(_txtA1.Text))
                await _settingService.SetValueAsync("AdminRecoveryA1Hash", DrMusa.Common.Utilities.PasswordHelper.HashPassword(_txtA1.Text.Trim().ToLower()));
            if (!string.IsNullOrWhiteSpace(_txtA2.Text))
                await _settingService.SetValueAsync("AdminRecoveryA2Hash", DrMusa.Common.Utilities.PasswordHelper.HashPassword(_txtA2.Text.Trim().ToLower()));
            if (!string.IsNullOrWhiteSpace(_txtA3.Text))
                await _settingService.SetValueAsync("AdminRecoveryA3Hash", DrMusa.Common.Utilities.PasswordHelper.HashPassword(_txtA3.Text.Trim().ToLower()));

            // Only hash and save if they generated a new key
            if (!string.IsNullOrWhiteSpace(_txtMasterKey.Text))
                await _settingService.SetValueAsync("AdminRecoveryMasterKeyHash", DrMusa.Common.Utilities.PasswordHelper.HashPassword(_txtMasterKey.Text.Trim()));

            // Clear textboxes so they don't sit in plain text
            _txtA1.Text = ""; _txtA2.Text = ""; _txtA3.Text = ""; _txtMasterKey.Text = "";

            UIHelper.ShowSuccess("Settings saved successfully.");
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to save settings: {ex.Message}");
        }
    }

    private void BtnBackup_Click(object? sender, EventArgs e)
    {
        var dbPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "database", "DrMusa.db"));
        
        // Instant Google Drive Backup
        if (!string.IsNullOrWhiteSpace(_txtAutoBackupDir.Text) && Directory.Exists(_txtAutoBackupDir.Text))
        {
            try
            {
                string targetPath = Path.Combine(_txtAutoBackupDir.Text, $"DrMusa_ManualBackup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                File.Copy(dbPath, targetPath, overwrite: true);

                // Cleanup backups older than 7 days (Rolling Backup)
                try
                {
                    var oldBackups = Directory.GetFiles(_txtAutoBackupDir.Text, "DrMusa_*Backup*.db")
                        .Select(f => new FileInfo(f))
                        .Where(f => f.CreationTime < DateTime.Now.AddDays(-7));
                    foreach (var old in oldBackups) { try { old.Delete(); } catch { } }
                }
                catch { }

                UIHelper.ShowSuccess($"Instant Backup Complete!\nSaved directly to your Automatic Backup Directory:\n{targetPath}");
                RefreshLastBackupTime();
                return;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Failed to perform instant backup: {ex.Message}\nFalling back to manual save.");
            }
        }

        // Fallback Manual Backup
        using var dlg = new SaveFileDialog
        {
            Filter = "SQLite Database|*.db",
            Title = "Save Database Backup",
            FileName = $"DrMusa_ManualBackup_{DateTime.Now:yyyyMMdd_HHmmss}.db"
        };

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            try
            {
                File.Copy(dbPath, dlg.FileName, overwrite: true);
                UIHelper.ShowSuccess("Database successfully backed up to:\n" + dlg.FileName);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Failed to backup database: {ex.Message}");
            }
        }
    }

    private void BtnRestore_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            "WARNING: Restoring a database will OVERWRITE all your current data (sales, users, inventory).\n\n" +
            "The application will restart after the restore. Are you absolutely sure?", 
            "Critical Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes) return;

        using var dlg = new OpenFileDialog
        {
            Filter = "SQLite Database|*.db",
            Title = "Select Database Backup to Restore"
        };

        // Smart select latest backup
        if (!string.IsNullOrWhiteSpace(_txtAutoBackupDir.Text) && Directory.Exists(_txtAutoBackupDir.Text))
        {
            dlg.InitialDirectory = _txtAutoBackupDir.Text;
            try
            {
                var latestBackup = Directory.GetFiles(_txtAutoBackupDir.Text, "DrMusa_*Backup*.db")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .FirstOrDefault();
                
                if (latestBackup != null)
                {
                    dlg.FileName = latestBackup.Name;
                }
            }
            catch { }
        }

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var dbPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "database", "DrMusa.db"));
                
                // Clear active EF connections to release file lock
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                
                File.Copy(dlg.FileName, dbPath, overwrite: true);
                
                MessageBox.Show("Database restored successfully! The application will now restart.", "Restore Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Failed to restore database: {ex.Message}\nMake sure no other instances of the app are open.");
            }
        }
    }
}
