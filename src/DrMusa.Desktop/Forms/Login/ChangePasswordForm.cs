using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Login;

/// <summary>
/// Change Password dialog — allows the logged-in user to update their password.
/// Validates current password, enforces minimum length, and checks confirmation match.
/// </summary>
public sealed class ChangePasswordForm : Form
{
    // ── Services ──────────────────────────────────────────────────────────────
    private readonly IAuthService _authService;

    // ── Controls ──────────────────────────────────────────────────────────────
    private TextBox _txtCurrent  = null!;
    private TextBox _txtNew      = null!;
    private TextBox _txtConfirm  = null!;
    private Button  _btnSave     = null!;
    private Button  _btnCancel   = null!;
    private Label   _lblError    = null!;
    private Label   _lblStrength = null!;
    private Panel   _pnlStrength = null!;

    // ── Constructor ───────────────────────────────────────────────────────────
    public ChangePasswordForm(IServiceProvider serviceProvider)
    {
        _authService = serviceProvider.GetRequiredService<IAuthService>();
        BuildUI();
    }

    // ── UI Construction ───────────────────────────────────────────────────────

    private void BuildUI()
    {
        Text            = "Change Password";
        Size            = new Size(480, 540);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;
        BackColor       = AppTheme.BackgroundDark;
        KeyPreview      = true;

        Paint += (s, e) =>
        {
            using var accentBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Rectangle(0, 0, Width, 3),
                AppTheme.AccentPrimary,
                AppTheme.AccentHover,
                System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(accentBrush, 0, 0, Width, 3);
        };

        // ── Card panel ────────────────────────────────────────────────────────
        var card = new Panel
        {
            Size      = new Size(420, 460),
            Location  = new Point(30, 30),
            BackColor = AppTheme.BackgroundPanel
        };
        card.Paint += (s, e) =>
        {
            using var borderPen = new Pen(AppTheme.BorderDefault, 1f);
            e.Graphics.DrawRectangle(borderPen, 0, 0, card.Width - 1, card.Height - 1);
        };

        // ── Heading ───────────────────────────────────────────────────────────
        var lblIcon = new Label
        {
            Text      = "🔑",
            Font      = new Font("Segoe UI Emoji", 22f),
            ForeColor = AppTheme.AccentPrimary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(30, 25)
        };

        var lblTitle = new Label
        {
            Text      = "Change Password",
            Font      = AppTheme.FontTitle,
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(72, 30)
        };

        var lblSub = new Label
        {
            Text      = $"Account: {SessionManager.CurrentUsername}",
            Font      = new Font("Segoe UI", 9f),
            ForeColor = AppTheme.TextSecondary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(72, 62)
        };

        var sep = new Panel
        {
            BackColor = AppTheme.BorderDefault,
            Size      = new Size(360, 1),
            Location  = new Point(30, 96)
        };

        // ── Current Password ──────────────────────────────────────────────────
        var lblCurrent = new Label
        {
            Text      = "CURRENT PASSWORD",
            Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = AppTheme.TextSecondary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(30, 116)
        };
        _txtCurrent = new TextBox { PasswordChar = '●', TabIndex = 0 };
        var pnlCurrent = AppTheme.WrapInputPanel(_txtCurrent, "");
        pnlCurrent.Size     = new Size(360, 44);
        pnlCurrent.Location = new Point(30, 136);

        // ── New Password ──────────────────────────────────────────────────────
        var lblNew = new Label
        {
            Text      = "NEW PASSWORD",
            Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = AppTheme.TextSecondary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(30, 198)
        };
        _txtNew = new TextBox { PasswordChar = '●', TabIndex = 1 };
        var pnlNew = AppTheme.WrapInputPanel(_txtNew, "");
        pnlNew.Size     = new Size(360, 44);
        pnlNew.Location = new Point(30, 218);

        // Strength bar
        _pnlStrength = new Panel
        {
            Size      = new Size(0, 3),
            Location  = new Point(30, 266),
            BackColor = AppTheme.AccentDanger
        };

        var pnlStrengthBg = new Panel
        {
            Size      = new Size(360, 3),
            Location  = new Point(30, 266),
            BackColor = AppTheme.BorderDefault
        };
        pnlStrengthBg.Controls.Add(_pnlStrength);

        _lblStrength = new Label
        {
            Text      = "",
            Font      = new Font("Segoe UI", 7.5f),
            ForeColor = AppTheme.TextMuted,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(30, 274)
        };

        _txtNew.TextChanged += TxtNew_TextChanged;

        // ── Confirm Password ──────────────────────────────────────────────────
        var lblConfirm = new Label
        {
            Text      = "CONFIRM NEW PASSWORD",
            Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = AppTheme.TextSecondary,
            BackColor = Color.Transparent,
            AutoSize  = true,
            Location  = new Point(30, 300)
        };
        _txtConfirm = new TextBox { PasswordChar = '●', TabIndex = 2 };
        var pnlConfirm = AppTheme.WrapInputPanel(_txtConfirm, "");
        pnlConfirm.Size     = new Size(360, 44);
        pnlConfirm.Location = new Point(30, 320);

        // ── Error Label ───────────────────────────────────────────────────────
        _lblError = new Label
        {
            Text      = "",
            Font      = new Font("Segoe UI", 8.5f),
            ForeColor = AppTheme.AccentDanger,
            BackColor = Color.Transparent,
            AutoSize  = false,
            Size      = new Size(360, 20),
            Location  = new Point(30, 372),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // ── Buttons ───────────────────────────────────────────────────────────
        _btnSave = new Button
        {
            Text      = "Save Password",
            Size      = new Size(175, 44),
            Location  = new Point(30, 400),
            TabIndex  = 3
        };
        AppTheme.StylePrimaryButton(_btnSave);
        _btnSave.Click += BtnSave_Click;

        _btnCancel = new Button
        {
            Text      = "Cancel",
            Size      = new Size(175, 44),
            Location  = new Point(215, 400),
            TabIndex  = 4
        };
        AppTheme.StyleSecondaryButton(_btnCancel);
        _btnCancel.Click += (s, e) => this.Close();

        card.Controls.AddRange(new Control[]
        {
            lblIcon, lblTitle, lblSub, sep,
            lblCurrent, pnlCurrent,
            lblNew, pnlNew,
            pnlStrengthBg, _lblStrength,
            lblConfirm, pnlConfirm,
            _lblError, _btnSave, _btnCancel
        });

        Controls.Add(card);
    }

    // ── Password Strength ─────────────────────────────────────────────────────

    private void TxtNew_TextChanged(object? sender, EventArgs e)
    {
        var pwd = _txtNew.Text;
        var (score, label, color) = GetPasswordStrength(pwd);

        int width = (int)(360 * (score / 4.0));
        _pnlStrength.Width    = Math.Max(0, width);
        _pnlStrength.BackColor = color;
        _lblStrength.Text     = string.IsNullOrEmpty(pwd) ? "" : label;
        _lblStrength.ForeColor = color;
    }

    private static (int score, string label, Color color) GetPasswordStrength(string pwd)
    {
        if (string.IsNullOrEmpty(pwd)) return (0, "", AppTheme.TextMuted);

        int score = 0;
        if (pwd.Length >= 6)  score++;
        if (pwd.Length >= 10) score++;
        if (pwd.Any(char.IsUpper) && pwd.Any(char.IsLower)) score++;
        if (pwd.Any(char.IsDigit) || pwd.Any(c => !char.IsLetterOrDigit(c))) score++;

        return score switch
        {
            1 => (1, "Weak",   AppTheme.AccentDanger),
            2 => (2, "Fair",   AppTheme.AccentWarning),
            3 => (3, "Good",   AppTheme.AccentSuccess),
            4 => (4, "Strong", AppTheme.AccentSuccess),
            _ => (0, "",       AppTheme.TextMuted)
        };
    }

    // ── Save Logic ────────────────────────────────────────────────────────────

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        _lblError.Text = "";

        var current = _txtCurrent.Text;
        var newPwd  = _txtNew.Text;
        var confirm = _txtConfirm.Text;

        // Validation
        if (string.IsNullOrWhiteSpace(current))
        {
            _lblError.Text = "⚠  Current password is required.";
            _txtCurrent.Focus();
            return;
        }
        if (string.IsNullOrWhiteSpace(newPwd) || newPwd.Length < 6)
        {
            _lblError.Text = "⚠  New password must be at least 6 characters.";
            _txtNew.Focus();
            return;
        }
        if (newPwd != confirm)
        {
            _lblError.Text = "⚠  New passwords do not match.";
            _txtConfirm.Focus();
            return;
        }
        if (newPwd == current)
        {
            _lblError.Text = "⚠  New password must differ from the current password.";
            _txtNew.Focus();
            return;
        }

        _btnSave.Enabled = false;
        _btnSave.Text    = "Saving…";

        try
        {
            var userId  = SessionManager.CurrentUserId!.Value;
            var success = await _authService.ChangePasswordAsync(userId, current, newPwd);

            if (!success)
            {
                _lblError.Text = "⚠  Current password is incorrect.";
                _txtCurrent.Clear();
                _txtCurrent.Focus();
                return;
            }

            UIHelper.ShowSuccess(
                "Your password has been changed successfully.",
                "Password Changed");
            this.Close();
        }
        catch (Exception ex)
        {
            _lblError.Text = $"⚠  Error: {ex.Message}";
        }
        finally
        {
            _btnSave.Enabled = true;
            _btnSave.Text    = "Save Password";
        }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape) { this.Close(); return true; }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
