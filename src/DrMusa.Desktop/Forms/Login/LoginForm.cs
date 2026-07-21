using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Login;

/// <summary>
/// Professional dark-themed login form for DrMusa POS.
/// Split-panel layout: branded left panel + clean login form on the right.
/// </summary>
public sealed class LoginForm : Form
{
    // ── Services ──────────────────────────────────────────────────────────────
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthService _authService;

    // ── Controls ──────────────────────────────────────────────────────────────
    private Panel _leftPanel = null!;
    private Panel _rightPanel = null!;
    private Panel _formCard = null!;
    private Label _lblBrand = null!;
    private Label _lblTagline = null!;
    private Label _lblWelcome = null!;
    private Label _lblSubtitle = null!;
    private Label _lblUsername = null!;
    private Panel _pnlUsername = null!;
    private TextBox _txtUsername = null!;
    private Label _lblPassword = null!;
    private Panel _pnlPassword = null!;
    private TextBox _txtPassword = null!;
    private CheckBox _chkRemember = null!;
    private Button _btnLogin = null!;
    private Label _lblError = null!;
    private Label _lblCopyright = null!;
    private Label _lblVersion = null!;
    private PictureBox _picLock = null!;
    private bool _passwordVisible = false;
    private Image? _logoImage;

    // ── Constructor ───────────────────────────────────────────────────────────
    public LoginForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _authService = serviceProvider.GetRequiredService<IAuthService>();

        SessionManager.LoadSavedSettings();

        var logoPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "assets", "logo", "DrMusa-logo.jpg"));
        if (File.Exists(logoPath))
        {
            _logoImage = Image.FromFile(logoPath);
        }

        BuildUI();
        WireEvents();
        PreFillRememberedUser();
    }

    // ── UI Construction ───────────────────────────────────────────────────────

    private void BuildUI()
    {
        // Form properties
        Text = "DrMusa POS — Login";
        Size = new Size(900, 560);
        MinimumSize = new Size(900, 560);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        BackColor = AppTheme.BackgroundDark;
        Icon = SystemIcons.Shield;

        BuildLeftPanel();
        BuildRightPanel();

        var centerContainer = new Panel
        {
            Size = new Size(900, 560),
            Margin = new Padding(0),
            BackColor = Color.Transparent
        };
        centerContainer.Controls.AddRange(new Control[] { _leftPanel, _rightPanel });

        var rootGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 3,
            BackColor = Color.Transparent
        };

        rootGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        rootGrid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        rootGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        rootGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        rootGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        rootGrid.Controls.Add(centerContainer, 1, 1);

        Controls.Add(rootGrid);
        _rightPanel.BringToFront();
    }

    // ── Left Branding Panel ───────────────────────────────────────────────────

    private void BuildLeftPanel()
    {
        _leftPanel = new Panel
        {
            Size = new Size(370, 560),
            Location = new Point(0, 0),
            BackColor = AppTheme.BackgroundPanel
        };
        _leftPanel.Paint += LeftPanel_Paint;

        // Shield / lock icon (text-based, elegant)
        _picLock = new PictureBox
        {
            Size = new Size(120, 120),
            Location = new Point(125, 80),
            BackColor = Color.Transparent
        };
        _picLock.Paint += PicLock_Paint;

        _lblBrand = new Label
        {
            Text = "Dr. Musa",
            Font = AppTheme.FontDisplay,
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = false,
            Size = new Size(370, 50),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 220)
        };

        _lblTagline = new Label
        {
            Text = "Point of Sale System",
            Font = new Font("Segoe UI", 10f, FontStyle.Regular),
            ForeColor = Color.WhiteSmoke,
            BackColor = Color.Transparent,
            AutoSize = false,
            Size = new Size(370, 30),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 268)
        };

        // Feature pills
        int pillY = 340;
        foreach (var feature in new[] { "  ✓  Offline First", "  ✓  Inventory & Billing", "  ✓  Role-Based Access" })
        {
            var pill = new Label
            {
                Text = feature,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(110, pillY)
            };
            _leftPanel.Controls.Add(pill);
            pillY += 28;
        }

        _lblVersion = new Label
        {
            Text = "Version 1.0",
            Font = AppTheme.FontCaption,
            ForeColor = Color.LightGray,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(148, 490)
        };

        _leftPanel.Controls.AddRange(new Control[]
            { _picLock, _lblBrand, _lblTagline, _lblVersion });
    }

    private void LeftPanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Teal gradient overlay matching the logo
        using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
            new Rectangle(0, 0, 370, 560),
            AppTheme.AccentPrimary,
            AppTheme.ColorFromHex("#033036"),
            System.Drawing.Drawing2D.LinearGradientMode.Vertical);
        g.FillRectangle(brush, 0, 0, 370, 560);

        // Accent line at top
        using var accentBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
            new Rectangle(0, 0, 370, 4),
            AppTheme.AccentPrimary,
            AppTheme.AccentHover,
            System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
        g.FillRectangle(accentBrush, 0, 0, 370, 4);

        // Right border separator
        using var borderPen = new Pen(AppTheme.BorderDefault, 1f);
        g.DrawLine(borderPen, 369, 0, 369, 560);
    }

    private void PicLock_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        if (_logoImage != null)
        {
            // Draw logo in a perfect circle
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, 120, 120);
            g.SetClip(path);
            g.DrawImage(_logoImage, 0, 0, 120, 120);
            g.ResetClip();

            // Draw a thin border ring around it
            using var ringPen = new Pen(Color.White, 2f);
            g.DrawEllipse(ringPen, 1, 1, 118, 118);
        }
        else
        {
            // Circular background fallback
            using var bgBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Rectangle(0, 0, 120, 120),
                AppTheme.AccentPrimary,
                AppTheme.AccentHover,
                45f);
            g.FillEllipse(bgBrush, 2, 2, 116, 116);

            using var font = new Font("Segoe UI", 48f, FontStyle.Bold);
            using var textBrush = new SolidBrush(Color.White);
            var text = "D";
            var size = g.MeasureString(text, font);
            g.DrawString(text, font, textBrush,
                (120 - size.Width) / 2f,
                (120 - size.Height) / 2f);
        }
    }

    // ── Right Login Panel ─────────────────────────────────────────────────────

    private void BuildRightPanel()
    {
        _rightPanel = new Panel
        {
            Size = new Size(530, 560),
            Location = new Point(370, 0),
            BackColor = AppTheme.BackgroundDark
        };

        // Center card
        _formCard = new Panel
        {
            Size = new Size(380, 440),
            Location = new Point(75, 60),
            BackColor = AppTheme.BackgroundPanel
        };
        _formCard.Paint += FormCard_Paint;

        // Welcome heading
        _lblWelcome = new Label
        {
            Text = "Welcome Back",
            Font = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(30, 35)
        };

        _lblSubtitle = new Label
        {
            Text = "Sign in to your account to continue",
            Font = new Font("Segoe UI", 9f, FontStyle.Regular),
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(30, 68)
        };

        // Username field
        _lblUsername = new Label
        {
            Text = "USERNAME",
            Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(30, 110)
        };

        _txtUsername = new TextBox { TabIndex = 0 };
        _pnlUsername = AppTheme.WrapInputPanel(_txtUsername, "Enter your username");
        _pnlUsername.Size = new Size(320, 44);
        _pnlUsername.Location = new Point(30, 128);

        // Password field
        _lblPassword = new Label
        {
            Text = "PASSWORD",
            Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(30, 190)
        };

        _txtPassword = new TextBox { TabIndex = 1, PasswordChar = '●' };
        _pnlPassword = AppTheme.WrapInputPanel(_txtPassword, "");
        _pnlPassword.Size = new Size(320, 44);
        _pnlPassword.Location = new Point(30, 208);

        // Toggle password visibility button
        var btnTogglePass = new Button
        {
            Text = "👁",
            Size = new Size(36, 36),
            Location = new Point(280, 4),
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.BackgroundCard,
            ForeColor = AppTheme.TextPrimary,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI Emoji", 11f),
            TabStop = false
        };
        btnTogglePass.FlatAppearance.BorderSize = 0;
        btnTogglePass.Click += (s, e) =>
        {
            _passwordVisible = !_passwordVisible;
            _txtPassword.PasswordChar = _passwordVisible ? '\0' : '●';
            btnTogglePass.ForeColor = _passwordVisible ? AppTheme.AccentPrimary : AppTheme.TextMuted;
        };
        _pnlPassword.Controls.Add(btnTogglePass);
        btnTogglePass.BringToFront();

        // Error label
        _lblError = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
            ForeColor = AppTheme.AccentDanger,
            BackColor = Color.Transparent,
            AutoSize = false,
            Size = new Size(320, 30),
            Location = new Point(30, 255),
            TextAlign = ContentAlignment.TopLeft
        };

        // Remember Me checkbox
        _chkRemember = new CheckBox
        {
            Text = "Remember me",
            Font = new Font("Segoe UI", 9f, FontStyle.Regular),
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(30, 286),
            Cursor = Cursors.Hand,
            TabIndex = 2
        };
        StyleCheckbox(_chkRemember);

        // Forgot Password Link
        var lnkForgot = new LinkLabel
        {
            Text = "Forgot Password?",
            Font = new Font("Segoe UI", 9f, FontStyle.Regular),
            LinkColor = AppTheme.AccentPrimary,
            ActiveLinkColor = AppTheme.AccentHover,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(230, 286),
            Cursor = Cursors.Hand
        };
        lnkForgot.LinkClicked += async (s, e) => await HandleForgotPasswordAsync();

        // Login button
        _btnLogin = new Button
        {
            Text = "Sign In",
            Size = new Size(320, 48),
            Location = new Point(30, 326),
            TabIndex = 3
        };
        AppTheme.StylePrimaryButton(_btnLogin);
        _btnLogin.Height = 48;
        _btnLogin.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);

        // Copyright
        _lblCopyright = new Label
        {
            Text = "© 2026 Crabian Developers. All rights reserved.\nm.abubakar.prof@gmail.com",
            Font = AppTheme.FontCaption,
            ForeColor = AppTheme.TextMuted,
            BackColor = Color.Transparent,
            AutoSize = false,
            Size = new Size(360, 60),
            TextAlign = ContentAlignment.TopCenter,
            Location = new Point(10, 385)
        };

        _formCard.Controls.AddRange(new Control[]
        {
            _lblWelcome, _lblSubtitle,
            _lblUsername, _pnlUsername,
            _lblPassword, _pnlPassword,
            _lblError,
            _chkRemember,
            lnkForgot,
            _btnLogin,
            _lblCopyright
        });

        _rightPanel.Controls.Add(_formCard);
    }

    private void FormCard_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Card background
        using var bgBrush = new SolidBrush(AppTheme.BackgroundPanel);
        g.FillRectangle(bgBrush, 0, 0, _formCard.Width, _formCard.Height);

        // Accent top border
        using var accentBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
            new Rectangle(0, 0, _formCard.Width, 3),
            AppTheme.AccentPrimary,
            AppTheme.AccentHover,
            System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
        g.FillRectangle(accentBrush, 0, 0, _formCard.Width, 3);

        // Card border
        using var borderPen = new Pen(AppTheme.BorderDefault, 1f);
        g.DrawRectangle(borderPen, 0, 0, _formCard.Width - 1, _formCard.Height - 1);
    }

    private static void StyleCheckbox(CheckBox chk)
    {
        chk.FlatStyle = FlatStyle.Flat;
    }

    // ── Event Wiring ──────────────────────────────────────────────────────────

    private void WireEvents()
    {
        _btnLogin.Click += BtnLogin_Click;
        _txtPassword.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter) _ = PerformLoginAsync();
        };
        _txtUsername.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter) _txtPassword.Focus();
        };
    }

    private void PreFillRememberedUser()
    {
        if (!string.IsNullOrEmpty(SessionManager.RememberedUsername))
        {
            _txtUsername.Text = SessionManager.RememberedUsername;
            _txtUsername.ForeColor = AppTheme.TextPrimary;
            _chkRemember.Checked = true;
            _txtPassword.Focus();
            _txtPassword.Focus();
        }
    }

    private async Task HandleForgotPasswordAsync()
    {
        var username = _txtUsername.Text.Trim();
        if (string.IsNullOrWhiteSpace(username) || username == "Enter your username")
        {
            ShowError("Please enter your username first to recover password.");
            _txtUsername.Focus();
            return;
        }

        var userService = _serviceProvider.GetRequiredService<DrMusa.Business.Interfaces.IUserService>();
        var users = await userService.GetAllAsync();
        var user = users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            ShowError("Username not found.");
            return;
        }

        if (user.Role != DrMusa.Common.Enums.UserRole.Owner)
        {
            ShowError("Only admin can use this option.");
            return;
        }

        using var recoveryForm = new PasswordRecoveryForm(_serviceProvider, user.Id, user.Username);
        recoveryForm.ShowDialog(this);
    }

    // ── Login Logic ───────────────────────────────────────────────────────────

    private async void BtnLogin_Click(object? sender, EventArgs e)
        => await PerformLoginAsync();

    private async Task PerformLoginAsync()
    {
        _lblError.Text = "";

        var username = _txtUsername.Text.Trim();
        var password = _txtPassword.Text;

        // Client-side validation
        if (string.IsNullOrWhiteSpace(username) || username == "Enter your username")
        {
            ShowError("Please enter your username.");
            _txtUsername.Focus();
            return;
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            ShowError("Please enter your password.");
            _txtPassword.Focus();
            return;
        }

        // Disable controls during auth
        SetFormEnabled(false);
        _btnLogin.Text = "Signing in…";

        try
        {
            var user = await _authService.LoginAsync(username, password);

            if (user == null)
            {
                ShowError("Invalid username or password. Please try again.");
                _txtPassword.Clear();
                _txtPassword.Focus();
                return;
            }

            // Session
            SessionManager.SetUser(user, _chkRemember.Checked);

            // Navigate to main shell
            var mainForm = new Forms.Dashboard.MainForm(_serviceProvider);
            mainForm.FormClosed += (_, __) =>
            {
                if (!SessionManager.IsLoggedIn)
                {
                    this.Show();
                    _txtPassword.Clear();
                    _txtUsername.Focus();
                }
                else
                {
                    this.Close();
                }
            };
            mainForm.Show();
            this.Hide();
        }
        catch (Exception ex)
        {
            ShowError($"Login error: {ex.Message}");
        }
        finally
        {
            SetFormEnabled(true);
            _btnLogin.Text = "Sign In";
        }
    }

    private void ShowError(string message)
    {
        _lblError.Text = "⚠  " + message;
        _lblError.ForeColor = AppTheme.AccentDanger;
    }

    private void SetFormEnabled(bool enabled)
    {
        _txtUsername.Enabled = enabled;
        _txtPassword.Enabled = enabled;
        _btnLogin.Enabled = enabled;
        _chkRemember.Enabled = enabled;
    }

    // ── Keyboard shortcut: Enter to login ─────────────────────────────────────
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Enter && _btnLogin.Enabled)
        {
            _ = PerformLoginAsync();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
