namespace DrMusa.Desktop.Helpers;

/// <summary>Centralised UI helper methods shared across all forms.</summary>
public static class UIHelper
{
    public static void ShowError(string message, string title = "Error")
        => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static void ShowSuccess(string message, string title = "Success")
        => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static bool Confirm(string message, string title = "Confirm")
        => MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
           == DialogResult.Yes;

    public static void ShowWarning(string message, string title = "Warning")
        => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

    /// <summary>Formats a decimal as currency using the configured symbol.</summary>
    public static string FormatCurrency(decimal value, string symbol = "PKR")
        => $"{symbol} {value:N2}";

    public static Icon? AppIcon
    {
        get
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo", "DrMusa-logo.ico");
                if (File.Exists(path)) return new Icon(path);

                // Fallback for some local dev scenarios
                path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "assets", "logo", "DrMusa-logo.ico"));
                if (File.Exists(path)) return new Icon(path);
            }
            catch { }
            return null;
        }
    }
}
