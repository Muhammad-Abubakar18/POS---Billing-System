using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Login;

/// <summary>
/// Lock Screen — shown when user locks the application.
/// The session is kept alive; only the correct password unlocks the app.
/// Choosing "Logout" ends the session and returns to LoginForm.
/// </summary>
public sealed class LockForm : Form
{
    // ── Services ──────────────────────────────────────────────────────────────
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthService     _authService;

    // ── Controls ──────────────────────────────────────────────────────────────
    private TextBox _txtPassword = null!;
    private Button  _btnUnlock   = null!;
    private Button  _btnLogout   = null!;
    private Label   _lblError    = null!;
    private Label   _lblUser     = null!;
    private bool    _passwordVisible = false;

    // ── Constructor ───────────────────────────────────────────────────────────
    public LockForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _authService     = serviceProvider.GetRequiredService<IAuthService>();
        BuildUI();
    }

    // ── UI Construction ───────────────────────────────────────────────────────

    private void BuildUI()
    {
        Text            = "DrMusa POS — Locked";
        Size            = new Size(480, 560);
        StartPosition   = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.None;          // Borderless for lock screen
        TopMost         = true;
        BackColor       = AppTheme.BackgroundDark;
        MaximizeBox     = false;
        MinimizeBox     = false;
        ControlBox      = false;
        KeyPreview      = true;

        Paint += LockForm_Paint;

        // ── Lock Icon ─────────────────────────────────────────────────────────
        var picIcon = new PictureBox
        {
            Size      = new Size(80, 80),
            Location  = new Point(200, 80),
            BackColor = Color.Transparent
        };
        picIcon.Paint += PicIcon_Paint;

        // ── Locked label ──────────────────────────────────────────────────────
        var lblLocked = new Label
        {
            Text      = "Application Locked",
            Font      = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(0, 178)
        };
        lblLocked.Left = (480 - lblLocked.PreferredWidth) / 2;

        // ── User name ─────────────────────────────────────────────────────────
        _lblUser = new Label
        {
            Text      = $"Signed in as  {SessionManager.CurrentFullName ?? SessionManager.CurrentUsername}",
            Font      = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            ForeColor = AppTheme.TextSecondary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(0, 212)
        };
        _lblUser.Left = (480 - _lblUser.PreferredWidth) / 2;

        // ── Separator ─────────────────────────────────────────────────────────
        var sep = new Panel
        {
            BackColor = AppTheme.BorderDefault,
            Size      = new Size(320, 1),
            Location  = new Point(80, 256)
        };

        // ── Password label ────────────────────────────────────────────────────
        var lblPass = new Label
        {
            Text      = "ENTER PASSWORD TO UNLOCK",
            Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = AppTheme.TextSecondary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(80, 276)
        };

        // ── Password input ────────────────────────────────────────────────────
        _txtPassword = new TextBox { PasswordChar = '●', TabIndex = 0 };
        var pnlPass  = AppTheme.WrapInputPanel(_txtPassword, "");
        pnlPass.Size     = new Size(320, 44);
        pnlPass.Location = new Point(80, 296);

        // Toggle password visibility
        var btnToggle = new Button
        {
            Text      = "👁",
            Size      = new Size(36, 36),
            Location  = new Point(286, 4),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            ForeColor = AppTheme.TextMuted,
            Cursor    = Cursors.Hand,
            Font      = new Font("Segoe UI", 9f),
            TabStop   = false
        };
        btnToggle.FlatAppearance.BorderSize = 0;
        btnToggle.Click += (s, e) =>
        {
            _passwordVisible = !_passwordVisible;
            _txtPassword.PasswordChar = _passwordVisible ? '\0' : '●';
        };
        pnlPass.Controls.Add(btnToggle);

        // ── Error label ───────────────────────────────────────────────────────
        _lblError = new Label
        {
            Text      = "",
            Font      = new Font("Segoe UI", 8.5f),
            ForeColor = AppTheme.AccentDanger,
            BackColor = Color.Transparent,
            AutoSize  = false,
            Size      = new Size(320, 20),
            Location  = new Point(80, 348),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // ── Unlock button ─────────────────────────────────────────────────────
        _btnUnlock = new Button
        {
            Text      = "Unlock",
            Size      = new Size(320, 48),
            Location  = new Point(80, 376),
            TabIndex  = 1
        };
        AppTheme.StylePrimaryButton(_btnUnlock);
        _btnUnlock.Height = 48;
        _btnUnlock.Font   = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        _btnUnlock.Click += BtnUnlock_Click;

        // ── Logout link ───────────────────────────────────────────────────────
        _btnLogout = new Button
        {
            Text      = "Not you? Sign out",
            Size      = new Size(320, 36),
            Location  = new Point(80, 434),
            TabIndex  = 2,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            ForeColor = AppTheme.TextSecondary,
            Cursor    = Cursors.Hand,
            Font      = new Font("Segoe UI", 9f, FontStyle.Regular)
        };
        _btnLogout.FlatAppearance.BorderSize = 0;
        _btnLogout.MouseEnter += (s, e) => _btnLogout.ForeColor = AppTheme.AccentDanger;
        _btnLogout.MouseLeave += (s, e) => _btnLogout.ForeColor = AppTheme.TextSecondary;
        _btnLogout.Click += BtnLogout_Click;

        // ── Wire keyboard ─────────────────────────────────────────────────────
        _txtPassword.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter) _ = PerformUnlockAsync();
        };

        Controls.AddRange(new Control[]
        {
            picIcon, lblLocked, _lblUser, sep,
            lblPass, pnlPass, _lblError,
            _btnUnlock, _btnLogout
        });

        _txtPassword.Focus();
    }

    private void LockForm_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        // Subtle top accent bar
        using var accentBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
            new Rectangle(0, 0, Width, 4),
            AppTheme.AccentPrimary,
            AppTheme.AccentHover,
            System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
        g.FillRectangle(accentBrush, 0, 0, Width, 4);
    }

    private void PicIcon_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Circle
        using var bgBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
            new Rectangle(0, 0, 80, 80),
            AppTheme.ColorFromHex("#2D3147"),
            AppTheme.ColorFromHex("#1A1D2E"),
            45f);
        g.FillEllipse(bgBrush, 2, 2, 76, 76);

        // Border ring
        using var ringPen = new Pen(AppTheme.AccentPrimary, 2f);
        g.DrawEllipse(ringPen, 2, 2, 76, 76);

        // Lock text symbol
        using var font = new Font("Segoe UI", 28f, FontStyle.Bold);
        using var brush = new SolidBrush(AppTheme.TextSecondary);
        var text = "🔒";
        using var emojiFont = new Font("Segoe UI Emoji", 22f);
        var size = g.MeasureString(text, emojiFont);
        g.DrawString(text, emojiFont, brush, (80 - size.Width) / 2f, (80 - size.Height) / 2f);
    }

    // ── Unlock Logic ──────────────────────────────────────────────────────────

    private async void BtnUnlock_Click(object? sender, EventArgs e)
        => await PerformUnlockAsync();

    private async Task PerformUnlockAsync()
    {
        _lblError.Text = "";
        var password = _txtPassword.Text;

        if (string.IsNullOrWhiteSpace(password))
        {
            _lblError.Text = "⚠  Please enter your password.";
            return;
        }

        _btnUnlock.Enabled = false;
        _btnUnlock.Text    = "Verifying…";

        try
        {
            var userId   = SessionManager.CurrentUserId!.Value;
            var username = SessionManager.CurrentUsername!;

            // Re-authenticate using the current username
            var user = await _authService.LoginAsync(username, password);

            if (user == null)
            {
                _lblError.Text = "⚠  Incorrect password. Please try again.";
                _txtPassword.Clear();
                _txtPassword.Focus();
                return;
            }

            SessionManager.Unlock();
            this.Close();
        }
        catch (Exception ex)
        {
            _lblError.Text = $"⚠  Error: {ex.Message}";
        }
        finally
        {
            _btnUnlock.Enabled = true;
            _btnUnlock.Text    = "Unlock";
        }
    }

    // ── Logout Logic ──────────────────────────────────────────────────────────

    private async void BtnLogout_Click(object? sender, EventArgs e)
    {
        if (!UIHelper.Confirm("Sign out and return to login screen?", "Sign Out"))
            return;

        try
        {
            if (SessionManager.CurrentUserId.HasValue)
                await _authService.LogoutAsync(SessionManager.CurrentUserId.Value);
        }
        catch { /* Non-critical */ }

        SessionManager.Clear();

        // Close all open forms and show login
        foreach (Form f in Application.OpenForms.Cast<Form>().ToList())
            if (f != this) f.Close();

        var loginForm = new LoginForm(_serviceProvider);
        loginForm.Show();
        this.Close();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Enter)
        {
            _ = PerformUnlockAsync();
            return true;
        }
        // Prevent Alt+F4 closing the lock screen
        if (keyData == (Keys.Alt | Keys.F4)) return true;
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
