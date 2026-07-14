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
    // ── Color Palette (Fluent 2 Light) ────────────────────────────────────────

    /// <summary>Main window background — neutral.background</summary>
    public static readonly Color BackgroundDark   = ColorFromHex("#F5F7FA");

    /// <summary>Panel/sidebar background — neutral.surface</summary>
    public static readonly Color BackgroundPanel  = ColorFromHex("#FFFFFF");

    /// <summary>Card/widget background — neutral.surfaceAlt</summary>
    public static readonly Color BackgroundCard   = ColorFromHex("#FAFBFC");

    /// <summary>Input field background</summary>
    public static readonly Color BackgroundInput  = ColorFromHex("#FFFFFF");

    /// <summary>Primary accent — brand.primary</summary>
    public static readonly Color AccentPrimary    = ColorFromHex("#0A828C");

    /// <summary>Accent hover state — brand.primaryHover</summary>
    public static readonly Color AccentHover      = ColorFromHex("#086972");

    /// <summary>Success / positive actions — semantic.success</summary>
    public static readonly Color AccentSuccess    = ColorFromHex("#0F7B3F");

    /// <summary>Warning / caution — semantic.warning</summary>
    public static readonly Color AccentWarning    = ColorFromHex("#E17B3A");

    /// <summary>Error / danger — semantic.danger</summary>
    public static readonly Color AccentDanger     = ColorFromHex("#D32F2F");

    /// <summary>Primary text — text.primary</summary>
    public static readonly Color TextPrimary      = ColorFromHex("#1A1D21");

    /// <summary>Secondary / dimmed text — text.secondary</summary>
    public static readonly Color TextSecondary    = ColorFromHex("#5C6470");

    /// <summary>Muted / placeholder text — text.tertiary</summary>
    public static readonly Color TextMuted        = ColorFromHex("#8A919C");

    /// <summary>Text on accent color buttons — text.onPrimaryBrand</summary>
    public static readonly Color TextOnAccent     = Color.White;

    /// <summary>Default border — neutral.border</summary>
    public static readonly Color BorderDefault    = ColorFromHex("#E1E4E8");

    /// <summary>Focused border — state.focusRing</summary>
    public static readonly Color BorderFocus      = ColorFromHex("#0A828C");

    // ── Fonts ─────────────────────────────────────────────────────────────────

    private const string FontFamily = "Segoe UI Variable"; // System fallback is Segoe UI

    public static readonly Font FontDisplay   = new Font(FontFamily, 28f, FontStyle.Bold);
    public static readonly Font FontTitle     = new Font(FontFamily, 20f, FontStyle.Bold);
    public static readonly Font FontHeading   = new Font(FontFamily, 16f, FontStyle.Bold);
    public static readonly Font FontBody      = new Font(FontFamily, 12f, FontStyle.Regular);
    public static readonly Font FontBodyBold  = new Font(FontFamily, 12f, FontStyle.Bold);
    public static readonly Font FontSmall     = new Font(FontFamily, 10f, FontStyle.Regular);
    public static readonly Font FontCaption   = new Font(FontFamily, 9f, FontStyle.Regular);
    public static readonly Font FontButton    = new Font(FontFamily, 12f, FontStyle.Bold);
    public static readonly Font FontInput     = new Font(FontFamily, 12f, FontStyle.Regular);

    // ── Control Styling Helpers ───────────────────────────────────────────────

    /// <summary>Applies the Fluent 2 Light theme to a Form and all its controls recursively.</summary>
    public static void ApplyTheme(Form form)
    {
        form.BackColor = BackgroundDark;
        form.ForeColor = TextPrimary;
        form.Font = FontBody;
    }

    /// <summary>
    /// Styles a TextBox to match the light theme with custom border painting.
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
            btn.BackColor = ColorFromHex("#EFF2F6"); // neutral.backgroundAlt
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
