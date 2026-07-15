using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Login;

public sealed class AdminAuthForm : Form
{
    private readonly IAuthService _authService;
    private TextBox _txtPassword = null!;

    public AdminAuthForm(IServiceProvider serviceProvider)
    {
        _authService = serviceProvider.GetRequiredService<IAuthService>();
        BuildUI();
    }

    private void BuildUI()
    {
        this.Text = "Confirm Action";
        this.Size = new Size(500, 250);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = AppTheme.BackgroundPanel;
        this.Font = AppTheme.FontBody;
        this.KeyPreview = true;

        var lblPrompt = new Label
        {
            Text = "Please enter your password to confirm:",
            Location = new Point(20, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = AppTheme.TextPrimary
        };

        _txtPassword = new TextBox
        {
            Location = new Point(20, 60),
            Width = 440,
            Font = new Font("Segoe UI", 11f),
            PasswordChar = '●'
        };

        var btnConfirm = new Button
        {
            Text = "Confirm",
            Location = new Point(20, 110),
            Size = new Size(160, 40)
        };
        AppTheme.StylePrimaryButton(btnConfirm);
        btnConfirm.Click += async (s, e) => await ConfirmAsync();

        var btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(280, 110),
            Size = new Size(160, 40)
        };
        AppTheme.StyleSecondaryButton(btnCancel);
        btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

        this.Controls.Add(lblPrompt);
        this.Controls.Add(_txtPassword);
        this.Controls.Add(btnConfirm);
        this.Controls.Add(btnCancel);
        
        // Allow pressing Enter to submit
        this.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                btnConfirm.PerformClick();
            }
        };
    }

    private async Task ConfirmAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtPassword.Text))
        {
            MessageBox.Show("Password cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var currentUser = SessionManager.CurrentUsername;
        if (string.IsNullOrEmpty(currentUser))
        {
            MessageBox.Show("No user is currently logged in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var result = await _authService.LoginAsync(currentUser, _txtPassword.Text);
        if (result == null)
        {
            MessageBox.Show("Incorrect password.", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _txtPassword.Clear();
            _txtPassword.Focus();
            return;
        }

        this.DialogResult = DialogResult.OK;
    }
}
