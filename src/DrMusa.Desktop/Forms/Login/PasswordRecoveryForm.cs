using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrMusa.Business.Interfaces;
using DrMusa.Common.Utilities;
using DrMusa.Data.Context;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Login;

public class PasswordRecoveryForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISettingService _settingService;
    private readonly IUserService _userService;
    private readonly int _userId;
    private readonly string _username;

    // UI
    private TabControl _tabControl = null!;
    private TextBox _txtA1 = null!;
    private TextBox _txtA2 = null!;
    private TextBox _txtA3 = null!;
    private TextBox _txtMasterKey = null!;
    
    private Panel _pnlReset = null!;
    private TextBox _txtNewPassword = null!;

    public PasswordRecoveryForm(IServiceProvider serviceProvider, int userId, string username)
    {
        _serviceProvider = serviceProvider;
        _settingService = serviceProvider.GetRequiredService<ISettingService>();
        _userService = serviceProvider.GetRequiredService<IUserService>();
        _userId = userId;
        _username = username;

        InitializeComponent();
        this.Load += async (s, e) => await LoadQuestionsAsync();
    }

    private void InitializeComponent()
    {
        this.Text = "DrMusa — Password Recovery";
        this.Size = new Size(550, 650);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = AppTheme.BackgroundPanel;
        this.Font = AppTheme.FontBody;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var lblTitle = new Label { Text = $"Recover password for: {_username}", Font = new Font("Segoe UI", 16f, FontStyle.Bold), ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(20, 20) };
        this.Controls.Add(lblTitle);

        _tabControl = new TabControl { Location = new Point(20, 70), Size = new Size(490, 350), Font = new Font("Segoe UI", 10f) };
        
        // Tab 1: Security Questions
        var tabQuestions = new TabPage("Security Questions");
        tabQuestions.BackColor = AppTheme.BackgroundCard;

        _txtA1 = new TextBox { Width = 450, Location = new Point(15, 45) };
        _txtA2 = new TextBox { Width = 450, Location = new Point(15, 115) };
        _txtA3 = new TextBox { Width = 450, Location = new Point(15, 185) };

        var lblQ1 = new Label { Name = "lblQ1", Text = "Question 1...", Location = new Point(15, 20), AutoSize = true, ForeColor = AppTheme.TextPrimary };
        var lblQ2 = new Label { Name = "lblQ2", Text = "Question 2...", Location = new Point(15, 90), AutoSize = true, ForeColor = AppTheme.TextPrimary };
        var lblQ3 = new Label { Name = "lblQ3", Text = "Question 3...", Location = new Point(15, 160), AutoSize = true, ForeColor = AppTheme.TextPrimary };

        tabQuestions.Controls.Add(lblQ1); tabQuestions.Controls.Add(_txtA1);
        tabQuestions.Controls.Add(lblQ2); tabQuestions.Controls.Add(_txtA2);
        tabQuestions.Controls.Add(lblQ3); tabQuestions.Controls.Add(_txtA3);

        var btnSubmitQuestions = new Button { Text = "Verify Answers", Width = 150, Height = 40, Location = new Point(15, 240) };
        AppTheme.StylePrimaryButton(btnSubmitQuestions);
        btnSubmitQuestions.Click += async (s, e) => await VerifyQuestionsAsync();
        tabQuestions.Controls.Add(btnSubmitQuestions);

        _tabControl.TabPages.Add(tabQuestions);

        // Tab 2: Master Key
        var tabKey = new TabPage("Master Key");
        tabKey.BackColor = AppTheme.BackgroundCard;

        var lblKey = new Label { Text = "Enter your Master Recovery Key:", Location = new Point(15, 20), AutoSize = true, ForeColor = AppTheme.TextPrimary };
        _txtMasterKey = new TextBox { Width = 450, Location = new Point(15, 50), Font = new Font("Consolas", 14f, FontStyle.Bold), TextAlign = HorizontalAlignment.Center };
        
        var btnSubmitKey = new Button { Text = "Verify Key", Width = 150, Height = 40, Location = new Point(15, 100) };
        AppTheme.StylePrimaryButton(btnSubmitKey);
        btnSubmitKey.Click += async (s, e) => await VerifyMasterKeyAsync();

        tabKey.Controls.Add(lblKey);
        tabKey.Controls.Add(_txtMasterKey);
        tabKey.Controls.Add(btnSubmitKey);

        _tabControl.TabPages.Add(tabKey);
        this.Controls.Add(_tabControl);

        // Reset Panel (Hidden initially)
        _pnlReset = new Panel { Location = new Point(20, 440), Size = new Size(490, 150), Visible = false };
        var lblNewPass = new Label { Text = "Enter New Password:", Location = new Point(0, 0), AutoSize = true, ForeColor = AppTheme.TextPrimary, Font = AppTheme.FontTitle };
        _txtNewPassword = new TextBox { Width = 350, Location = new Point(0, 40), Font = new Font("Segoe UI", 11f), PasswordChar = '●' };
        
        var btnReset = new Button { Text = "Reset Password", Width = 160, Height = 40, Location = new Point(0, 85) };
        AppTheme.StyleDangerButton(btnReset);
        btnReset.Click += async (s, e) => await PerformResetAsync();

        _pnlReset.Controls.Add(lblNewPass);
        _pnlReset.Controls.Add(_txtNewPassword);
        _pnlReset.Controls.Add(btnReset);
        this.Controls.Add(_pnlReset);
    }

    private async Task LoadQuestionsAsync()
    {
        var settings = (await _settingService.GetAllAsync()).ToDictionary(s => s.Key, s => s.Value);
        
        if (!settings.ContainsKey("AdminRecoveryQ1"))
        {
            MessageBox.Show("No security questions or master key were ever configured for this system. Password recovery is impossible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
            return;
        }

        var tabQuestions = _tabControl.TabPages[0];
        tabQuestions.Controls["lblQ1"]!.Text = settings.GetValueOrDefault("AdminRecoveryQ1", "Question 1");
        tabQuestions.Controls["lblQ2"]!.Text = settings.GetValueOrDefault("AdminRecoveryQ2", "Question 2");
        tabQuestions.Controls["lblQ3"]!.Text = settings.GetValueOrDefault("AdminRecoveryQ3", "Question 3");
    }

    private async Task VerifyQuestionsAsync()
    {
        var settings = (await _settingService.GetAllAsync()).ToDictionary(s => s.Key, s => s.Value);

        var hash1 = settings.GetValueOrDefault("AdminRecoveryA1Hash", "");
        var hash2 = settings.GetValueOrDefault("AdminRecoveryA2Hash", "");
        var hash3 = settings.GetValueOrDefault("AdminRecoveryA3Hash", "");

        bool ok1 = PasswordHelper.VerifyPassword(_txtA1.Text.Trim().ToLower(), hash1 ?? "");
        bool ok2 = PasswordHelper.VerifyPassword(_txtA2.Text.Trim().ToLower(), hash2 ?? "");
        bool ok3 = PasswordHelper.VerifyPassword(_txtA3.Text.Trim().ToLower(), hash3 ?? "");

        if (ok1 && ok2 && ok3)
        {
            ShowResetPanel();
        }
        else
        {
            UIHelper.ShowError("One or more answers are incorrect.");
        }
    }

    private async Task VerifyMasterKeyAsync()
    {
        var settings = (await _settingService.GetAllAsync()).ToDictionary(s => s.Key, s => s.Value);
        var keyHash = settings.GetValueOrDefault("AdminRecoveryMasterKeyHash", "");

        if (PasswordHelper.VerifyPassword(_txtMasterKey.Text.Trim(), keyHash ?? ""))
        {
            ShowResetPanel();
        }
        else
        {
            UIHelper.ShowError("Invalid Master Recovery Key.");
        }
    }

    private void ShowResetPanel()
    {
        _tabControl.Enabled = false;
        _pnlReset.Visible = true;
        _txtNewPassword.Focus();
    }

    private async Task PerformResetAsync()
    {
        var newPass = _txtNewPassword.Text;
        if (string.IsNullOrWhiteSpace(newPass) || newPass.Length < 4)
        {
            UIHelper.ShowError("Password must be at least 4 characters long.");
            return;
        }

        try
        {
            var user = await _userService.GetByIdAsync(_userId);
            if (user == null) throw new Exception("User not found in DB.");

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DrMusaDbContext>();
            var entity = await db.Users.FindAsync(_userId);
            if (entity != null)
            {
                entity.PasswordHash = PasswordHelper.HashPassword(newPass);
                await db.SaveChangesAsync();
                
                UIHelper.ShowSuccess("Password successfully reset! You can now log in with your new password.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to reset password: {ex.Message}");
        }
    }
}
