using System.Drawing;
using System.Windows.Forms;

namespace DrMusa.Desktop.Helpers;

/// <summary>
/// Central theme engine for DrMusa POS.
/// Provides color palette, fonts, and control styling helpers used by all forms.
/// Dark professional theme with indigo-blue accent.
/// </summary>
public static class AppTheme
{
    // ── Color Palette ─────────────────────────────────────────────────────────

    /// <summary>Main window background — near-black navy.</summary>
    public static readonly Color BackgroundDark   = ColorFromHex("#0F1117");

    /// <summary>Panel/sidebar background.</summary>
    public static readonly Color BackgroundPanel  = ColorFromHex("#1A1D2E");

    /// <summary>Card/widget background.</summary>
    public static readonly Color BackgroundCard   = ColorFromHex("#222538");

    /// <summary>Input field background.</summary>
    public static readonly Color BackgroundInput  = ColorFromHex("#2A2D42");

    /// <summary>Primary accent — indigo blue.</summary>
    public static readonly Color AccentPrimary    = ColorFromHex("#4F6EF7");

    /// <summary>Accent hover state.</summary>
    public static readonly Color AccentHover      = ColorFromHex("#6B87FA");

    /// <summary>Success / positive actions.</summary>
    public static readonly Color AccentSuccess    = ColorFromHex("#22C55E");

    /// <summary>Warning / caution.</summary>
    public static readonly Color AccentWarning    = ColorFromHex("#F59E0B");

    /// <summary>Error / danger.</summary>
    public static readonly Color AccentDanger     = ColorFromHex("#EF4444");

    /// <summary>Primary text — light slate.</summary>
    public static readonly Color TextPrimary      = ColorFromHex("#F1F5F9");

    /// <summary>Secondary / dimmed text.</summary>
    public static readonly Color TextSecondary    = ColorFromHex("#8B95A9");

    /// <summary>Muted / placeholder text.</summary>
    public static readonly Color TextMuted        = ColorFromHex("#4B5468");

    /// <summary>Text on accent color buttons.</summary>
    public static readonly Color TextOnAccent     = Color.White;

    /// <summary>Default border.</summary>
    public static readonly Color BorderDefault    = ColorFromHex("#2D3147");

    /// <summary>Focused border.</summary>
    public static readonly Color BorderFocus      = ColorFromHex("#4F6EF7");

    // ── Fonts ─────────────────────────────────────────────────────────────────

    public static readonly Font FontDisplay   = new Font("Segoe UI", 22f, FontStyle.Bold);
    public static readonly Font FontTitle     = new Font("Segoe UI", 15f, FontStyle.Bold);
    public static readonly Font FontHeading   = new Font("Segoe UI", 11f, FontStyle.Bold);
    public static readonly Font FontBody      = new Font("Segoe UI", 9.5f, FontStyle.Regular);
    public static readonly Font FontBodyBold  = new Font("Segoe UI", 9.5f, FontStyle.Bold);
    public static readonly Font FontSmall     = new Font("Segoe UI", 8f,  FontStyle.Regular);
    public static readonly Font FontCaption   = new Font("Segoe UI", 7.5f, FontStyle.Regular);
    public static readonly Font FontButton    = new Font("Segoe UI", 9.5f, FontStyle.Bold);
    public static readonly Font FontInput     = new Font("Segoe UI", 10f, FontStyle.Regular);

    // ── Control Styling Helpers ───────────────────────────────────────────────

    /// <summary>Applies the dark theme to a Form and all its controls recursively.</summary>
    public static void ApplyDarkTheme(Form form)
    {
        form.BackColor = BackgroundDark;
        form.ForeColor = TextPrimary;
        form.Font = FontBody;
    }

    /// <summary>
    /// Styles a TextBox to match the dark theme with custom border painting.
    /// Returns a Panel wrapper that draws the border.
    /// </summary>
    public static Panel WrapInputPanel(TextBox textBox, string placeholder = "")
    {
        textBox.BorderStyle = BorderStyle.None;
        textBox.BackColor = BackgroundInput;
        textBox.ForeColor = TextPrimary;
        textBox.Font = FontInput;
        textBox.Margin = new Padding(0);

        if (!string.IsNullOrEmpty(placeholder))
            SetPlaceholder(textBox, placeholder);

        var wrapper = new Panel
        {
            BackColor = BackgroundInput,
            Padding = new Padding(12, 0, 12, 0),
            Height = 44
        };

        textBox.Dock = DockStyle.Fill;
        textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        textBox.TextAlign = HorizontalAlignment.Left;

        wrapper.Controls.Add(textBox);
        wrapper.Paint += (s, e) =>
        {
            var panel = (Panel)s!;
            bool isFocused = textBox.Focused;
            using var pen = new Pen(isFocused ? BorderFocus : BorderDefault, 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
        };

        textBox.GotFocus  += (s, e) => wrapper.Invalidate();
        textBox.LostFocus += (s, e) => wrapper.Invalidate();

        return wrapper;
    }

    /// <summary>Styles a primary action button (accent colored).</summary>
    public static void StylePrimaryButton(Button btn)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = AccentPrimary;
        btn.ForeColor = TextOnAccent;
        btn.Font = FontButton;
        btn.Cursor = Cursors.Hand;
        btn.Height = 44;
        btn.TextAlign = ContentAlignment.MiddleCenter;

        btn.MouseEnter += (s, e) => btn.BackColor = AccentHover;
        btn.MouseLeave += (s, e) => btn.BackColor = AccentPrimary;
    }

    /// <summary>Styles a secondary / ghost button.</summary>
    public static void StyleSecondaryButton(Button btn)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderColor = BorderDefault;
        btn.FlatAppearance.BorderSize = 1;
        btn.BackColor = BackgroundCard;
        btn.ForeColor = TextSecondary;
        btn.Font = FontButton;
        btn.Cursor = Cursors.Hand;
        btn.Height = 44;
        btn.TextAlign = ContentAlignment.MiddleCenter;

        btn.MouseEnter += (s, e) =>
        {
            btn.BackColor = BackgroundInput;
            btn.ForeColor = TextPrimary;
        };
        btn.MouseLeave += (s, e) =>
        {
            btn.BackColor = BackgroundCard;
            btn.ForeColor = TextSecondary;
        };
    }

    /// <summary>Styles a danger button (logout, delete).</summary>
    public static void StyleDangerButton(Button btn)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = AccentDanger;
        btn.ForeColor = TextOnAccent;
        btn.Font = FontButton;
        btn.Cursor = Cursors.Hand;
        btn.Height = 44;

        btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(220, 38, 38);
        btn.MouseLeave += (s, e) => btn.BackColor = AccentDanger;
    }

    /// <summary>Creates a separator line.</summary>
    public static Panel CreateSeparator(bool horizontal = true)
    {
        return new Panel
        {
            BackColor = BorderDefault,
            Height = horizontal ? 1 : 0,
            Width = horizontal ? 0 : 1,
            Dock = horizontal ? DockStyle.Top : DockStyle.Left,
            Margin = new Padding(0, 8, 0, 8)
        };
    }

    // ── Private Helpers ───────────────────────────────────────────────────────

    public static Color ColorFromHex(string hex)
    {
        hex = hex.TrimStart('#');
        return Color.FromArgb(
            Convert.ToInt32(hex[..2], 16),
            Convert.ToInt32(hex[2..4], 16),
            Convert.ToInt32(hex[4..6], 16));
    }

    private static void SetPlaceholder(TextBox textBox, string placeholder)
    {
        textBox.Text = placeholder;
        textBox.ForeColor = TextMuted;

        textBox.GotFocus += (s, e) =>
        {
            if (textBox.Text == placeholder)
            {
                textBox.Text = "";
                textBox.ForeColor = TextPrimary;
            }
        };
        textBox.LostFocus += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = TextMuted;
            }
        };
    }
}
