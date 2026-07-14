using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using DrMusa.Desktop.Forms.Login;
using DrMusa.Desktop.Forms.Products;
using DrMusa.Desktop.Forms.Categories;
using DrMusa.Desktop.Forms.Reports;
using DrMusa.Desktop.Forms.Inventory;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Dashboard;

/// <summary>
/// Main shell form — hosts the top navigation bar and will load module panels.
/// Provides application-level actions: Lock, Change Password, Logout.
/// </summary>
public partial class MainForm : Form
{
    // ── Services ──────────────────────────────────────────────────────────────
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthService     _authService;
    private readonly ISaleService     _saleService;

    // ── Controls ──────────────────────────────────────────────────────────────
    private Panel  _topBar        = null!;
    private Panel  _contentArea   = null!;
    private Label  _lblUserName   = null!;
    private Label  _lblRole       = null!;
    private Panel  _sideBar       = null!;

    // ── Constructor ───────────────────────────────────────────────────────────
    public MainForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _authService     = serviceProvider.GetRequiredService<IAuthService>();
        _saleService     = serviceProvider.GetRequiredService<ISaleService>();

        BuildUI();

        if (SessionManager.HasRole(Common.Enums.UserRole.Cashier))
            OpenBillingModule();
        else
            LoadDashboard();
    }

    // ── UI Construction ───────────────────────────────────────────────────────

    private void BuildUI()
    {
        Text            = "DrMusa POS";
        Size            = new Size(1280, 760);
        MinimumSize     = new Size(1100, 650);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = AppTheme.BackgroundDark;
        ForeColor       = AppTheme.TextPrimary;
        Font            = AppTheme.FontBody;
        WindowState     = FormWindowState.Maximized;

        BuildTopBar();
        BuildSideBar();
        BuildContentArea();

        Controls.AddRange(new Control[] { _contentArea, _sideBar, _topBar });
    }

    // ── Top Navigation Bar ────────────────────────────────────────────────────

    private void BuildTopBar()
    {
        _topBar = new Panel
        {
            Height    = 56,
            Dock      = DockStyle.Top,
            BackColor = AppTheme.BackgroundPanel
        };
        _topBar.Paint += TopBar_Paint;

        // App logo/name
        var lblApp = new Label
        {
            Text      = "DrMusa POS",
            Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(20, 16)
        };

        // Right-side user info
        _lblUserName = new Label
        {
            Text      = SessionManager.CurrentFullName ?? SessionManager.CurrentUsername ?? "User",
            Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(0, 12)
        };

        _lblRole = new Label
        {
            Text      = SessionManager.CurrentRole?.ToString() ?? "",
            Font      = new Font("Segoe UI", 8f, FontStyle.Regular),
            ForeColor = AppTheme.TextSecondary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(0, 32)
        };

        // Action buttons — right-aligned
        var btnLock = CreateTopBarButton("🔒  Lock", AppTheme.BackgroundCard);
        btnLock.Click += BtnLock_Click;

        var btnChangePass = CreateTopBarButton("🔑  Change Password", AppTheme.BackgroundCard);
        btnChangePass.Click += BtnChangePass_Click;

        var btnLogout = CreateTopBarButton("⏻  Sign Out", AppTheme.BackgroundCard);
        btnLogout.ForeColor = AppTheme.AccentDanger;
        btnLogout.MouseEnter += (s, e) => btnLogout.BackColor = AppTheme.ColorFromHex("#3D1010");
        btnLogout.MouseLeave += (s, e) => btnLogout.BackColor = AppTheme.BackgroundCard;
        btnLogout.Click += BtnLogout_Click;

        // Layout buttons right-to-left
        _topBar.SizeChanged += (s, e) => LayoutTopBarButtons(btnLogout, btnChangePass, btnLock, _lblUserName, _lblRole);

        _topBar.Controls.AddRange(new Control[]
            { lblApp, _lblUserName, _lblRole, btnLock, btnChangePass, btnLogout });

        // Trigger initial layout
        _topBar.SizeChanged += (s, e) => LayoutTopBarButtons(btnLogout, btnChangePass, btnLock, _lblUserName, _lblRole);
        Load += (s, e) => LayoutTopBarButtons(btnLogout, btnChangePass, btnLock, _lblUserName, _lblRole);
    }

    private void LayoutTopBarButtons(Button btnLogout, Button btnChangePass, Button btnLock,
                                     Label lblUser, Label lblRole)
    {
        int right  = _topBar.Width - 16;
        int margin = 8;

        btnLogout.Left    = right - btnLogout.Width;
        btnChangePass.Left = btnLogout.Left - margin - btnChangePass.Width;
        btnLock.Left      = btnChangePass.Left - margin - btnLock.Width;

        // User info left of lock button
        int userRight = btnLock.Left - 20;
        lblUser.Left  = userRight - lblUser.Width;
        lblRole.Left  = userRight - lblRole.Width;
    }

    private static Button CreateTopBarButton(string text, Color bg)
    {
        var btn = new Button
        {
            Text      = text,
            AutoSize  = false,
            Size      = new Size(0, 34),
            Width     = TextRenderer.MeasureText(text, new Font("Segoe UI", 9f, FontStyle.Bold)).Width + 28,
            Height    = 34,
            Location  = new Point(0, 11),
            FlatStyle = FlatStyle.Flat,
            BackColor = bg,
            ForeColor = AppTheme.TextSecondary,
            Cursor    = Cursors.Hand,
            Font      = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        btn.FlatAppearance.BorderColor = AppTheme.BorderDefault;
        btn.FlatAppearance.BorderSize  = 1;
        btn.MouseEnter += (s, e) => btn.ForeColor = AppTheme.TextPrimary;
        btn.MouseLeave += (s, e) => btn.ForeColor = AppTheme.TextSecondary;
        return btn;
    }

    private void TopBar_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        using var pen = new Pen(AppTheme.BorderDefault, 1f);
        g.DrawLine(pen, 0, _topBar.Height - 1, _topBar.Width, _topBar.Height - 1);
    }

    // ── Sidebar ───────────────────────────────────────────────────────────────
    private void BuildSideBar()
    {
        _sideBar = new Panel
        {
            Width = 200,
            Dock = DockStyle.Left,
            BackColor = AppTheme.BackgroundPanel,
            Padding = new Padding(0, 20, 0, 0)
        };
        _sideBar.Paint += (s, e) =>
        {
            using var pen = new Pen(AppTheme.BorderDefault, 1f);
            e.Graphics.DrawLine(pen, _sideBar.Width - 1, 0, _sideBar.Width - 1, _sideBar.Height);
        };

        var btnDashboard = CreateSidebarButton("📊  Dashboard");
        btnDashboard.Click += (s, e) => LoadDashboard();
        btnDashboard.Visible = SessionManager.HasRole(DrMusa.Common.Enums.UserRole.Owner, DrMusa.Common.Enums.UserRole.SubAdmin);

        var btnCategories = CreateSidebarButton("🏷️  Categories");
        btnCategories.Click += (s, e) => OpenCategoriesModule();
        btnCategories.Visible = SessionManager.HasRole(DrMusa.Common.Enums.UserRole.Owner, DrMusa.Common.Enums.UserRole.SubAdmin);

        var btnProducts = CreateSidebarButton("📦  Products");
        btnProducts.Click += (s, e) => OpenProductsModule();
        btnProducts.Visible = SessionManager.HasRole(DrMusa.Common.Enums.UserRole.Owner, DrMusa.Common.Enums.UserRole.SubAdmin);

        var btnInventory = CreateSidebarButton("📦  Inventory");
        btnInventory.Click += (s, e) => OpenInventoryModule();
        btnInventory.Visible = SessionManager.HasRole(DrMusa.Common.Enums.UserRole.Owner, DrMusa.Common.Enums.UserRole.SubAdmin);

        var btnBilling = CreateSidebarButton("🛒  Billing");
        btnBilling.Click += (s, e) => OpenBillingModule();
        btnBilling.Visible = SessionManager.HasRole(DrMusa.Common.Enums.UserRole.Owner, DrMusa.Common.Enums.UserRole.SubAdmin, DrMusa.Common.Enums.UserRole.Cashier);
        
        var btnCustomers = CreateSidebarButton("👥  Customers");
        btnCustomers.Visible = SessionManager.HasRole(DrMusa.Common.Enums.UserRole.Owner, DrMusa.Common.Enums.UserRole.SubAdmin);
        
        var btnReports = CreateSidebarButton("📈  Reports");
        btnReports.Click += (s, e) => OpenReportsModule();
        btnReports.Visible = SessionManager.HasRole(DrMusa.Common.Enums.UserRole.Owner);

        var btnUsers = CreateSidebarButton("🧑‍💼  Users");
        btnUsers.Click += (s, e) => OpenUsersModule();
        btnUsers.Visible = SessionManager.HasRole(DrMusa.Common.Enums.UserRole.Owner);

        // Note: Controls are added to the panel in reverse visual order because DockStyle.Top stacks them.
        _sideBar.Controls.AddRange(new Control[] { btnUsers, btnReports, btnCustomers, btnInventory, btnProducts, btnCategories, btnBilling, btnDashboard });
    }

    private Button CreateSidebarButton(string text)
    {
        var btn = new Button
        {
            Text = text,
            Dock = DockStyle.Top,
            Height = 45,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            ForeColor = AppTheme.TextSecondary,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(20, 0, 0, 0),
            Font = new Font("Segoe UI", 10f, FontStyle.Regular),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.MouseEnter += (s, e) => { btn.BackColor = AppTheme.BackgroundCard; btn.ForeColor = AppTheme.TextPrimary; };
        btn.MouseLeave += (s, e) => { btn.BackColor = Color.Transparent; btn.ForeColor = AppTheme.TextSecondary; };
        return btn;
    }

    // ── Content Area ──────────────────────────────────────────────────────────

    private void BuildContentArea()
    {
        _contentArea = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = AppTheme.BackgroundDark
        };
    }

    private void LoadContent(Control content)
    {
        foreach (Control c in _contentArea.Controls)
        {
            c.Dispose();
        }
        _contentArea.Controls.Clear();

        content.Dock = DockStyle.Fill;
        _contentArea.Controls.Add(content);
        
        if (content is Form f)
        {
            f.Show();
        }
    }

    private void OpenUsersModule()
    {
        var usersForm = new DrMusa.Desktop.Forms.Users.UsersForm(_serviceProvider)
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None
        };
        LoadContent(usersForm);
    }

    private async void LoadDashboard()
    {
        var dashCtrl = new DashboardControl(_saleService);
        LoadContent(dashCtrl);
        await dashCtrl.LoadDataAsync();
    }

    private void OpenReportsModule()
    {
        try
        {
            var reportsForm = new ReportsForm(_serviceProvider)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            LoadContent(reportsForm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open Reports module: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenBillingModule()
    {
        try
        {
            var billingForm = new Forms.Billing.BillingForm(_serviceProvider)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            LoadContent(billingForm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open Billing module: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenProductsModule()
    {
        try
        {
            var productForm = new ProductListForm(_serviceProvider)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            LoadContent(productForm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open Products module: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenInventoryModule()
    {
        try
        {
            var inventoryForm = new InventoryForm(_serviceProvider)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            LoadContent(inventoryForm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open Inventory module: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenCategoriesModule()
    {
        try
        {
            var categoryForm = new CategoryListForm(_serviceProvider)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            LoadContent(categoryForm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open Categories module: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Toolbar Actions ───────────────────────────────────────────────────────

    private void BtnLock_Click(object? sender, EventArgs e)
    {
        SessionManager.Lock();
        var lockForm = new LockForm(_serviceProvider);
        lockForm.FormClosed += (_, __) =>
        {
            if (!SessionManager.IsLocked)
                this.Show();
        };
        this.Hide();
        lockForm.ShowDialog();

        if (!SessionManager.IsLocked && SessionManager.IsLoggedIn)
            this.Show();
    }

    private void BtnChangePass_Click(object? sender, EventArgs e)
    {
        using var dlg = new ChangePasswordForm(_serviceProvider);
        dlg.ShowDialog(this);
    }

    private async void BtnLogout_Click(object? sender, EventArgs e)
    {
        if (!UIHelper.Confirm("Are you sure you want to sign out?", "Sign Out"))
            return;

        try
        {
            if (SessionManager.CurrentUserId.HasValue)
                await _authService.LogoutAsync(SessionManager.CurrentUserId.Value);
        }
        catch { /* Non-critical log failure */ }

        SessionManager.Clear();

        this.Close();
    }
}
