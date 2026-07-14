using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Common.Enums;
using DrMusa.Desktop.Helpers;

namespace DrMusa.Desktop.Forms.Users;

public sealed class UserEditForm : Form
{
    private readonly IUserService _userService;
    private UserDto? _user;

    private TextBox _txtUsername = null!;
    private TextBox _txtFullName = null!;
    private TextBox _txtPassword = null!;
    private ComboBox _cmbRole = null!;
    private CheckBox _chkActive = null!;
    private Button _btnSave = null!;

    public UserEditForm(IUserService userService)
    {
        _userService = userService;

        Text = "Edit User";
        Size = new Size(500, 600);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = AppTheme.BackgroundDark;

        InitializeUI();
    }

    private void InitializeUI()
    {
        var lblTitle = new Label { Text = "User Details", Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(40, 30) };

        _txtUsername = new TextBox { Width = 400 };
        var pnlUsername = AppTheme.WrapInputPanel(_txtUsername, "Username");
        pnlUsername.Location = new Point(40, 80);

        _txtFullName = new TextBox { Width = 400 };
        var pnlFullName = AppTheme.WrapInputPanel(_txtFullName, "Full Name");
        pnlFullName.Location = new Point(40, 160);

        _cmbRole = new ComboBox { Width = 380, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12f) };
        _cmbRole.Items.AddRange(new object[] { UserRole.Owner, UserRole.SubAdmin, UserRole.Cashier });
        var pnlRole = new Panel { BackColor = AppTheme.BackgroundInput, Padding = new Padding(12, 5, 12, 5), Height = 44, Width = 400 };
        _cmbRole.Dock = DockStyle.Fill;
        pnlRole.Controls.Add(_cmbRole);
        pnlRole.Location = new Point(40, 240);

        _txtPassword = new TextBox { Width = 400, UseSystemPasswordChar = true };
        var pnlPassword = AppTheme.WrapInputPanel(_txtPassword, "Password (leave blank to keep current)");
        pnlPassword.Location = new Point(40, 320);

        _chkActive = new CheckBox { Text = "Is Active", Font = new Font("Segoe UI", 10f), Location = new Point(40, 400), AutoSize = true, Checked = true };

        _btnSave = new Button { Text = "Save", Width = 120, Height = 40, Location = new Point(320, 460) };
        AppTheme.StylePrimaryButton(_btnSave);
        _btnSave.Click += async (s, e) => await SaveAsync();

        var btnCancel = new Button { Text = "Cancel", Width = 100, Height = 40, Location = new Point(200, 460) };
        AppTheme.StyleSecondaryButton(btnCancel);
        btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

        Controls.AddRange(new Control[] { lblTitle, pnlUsername, pnlFullName, pnlRole, pnlPassword, _chkActive, _btnSave, btnCancel });
    }

    public void SetUser(UserDto? user)
    {
        _user = user;
        if (_user != null)
        {
            _txtUsername.Text = _user.Username;
            _txtFullName.Text = _user.FullName;
            _cmbRole.SelectedItem = _user.Role;
            _chkActive.Checked = _user.IsActive;
            _txtPassword.Enabled = false; // Cannot update password here directly
        }
        else
        {
            _cmbRole.SelectedItem = UserRole.Cashier;
        }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtUsername.Text) || string.IsNullOrWhiteSpace(_txtFullName.Text) || _cmbRole.SelectedItem == null)
        {
            UIHelper.ShowWarning("Please fill in all required fields.");
            return;
        }

        try
        {
            _btnSave.Enabled = false;
            var role = (UserRole)_cmbRole.SelectedItem;

            if (_user == null)
            {
                if (string.IsNullOrWhiteSpace(_txtPassword.Text))
                {
                    UIHelper.ShowWarning("Password is required for new users.");
                    _btnSave.Enabled = true;
                    return;
                }

                var dto = new CreateUserDto(_txtUsername.Text, _txtFullName.Text, role, _txtPassword.Text, _chkActive.Checked);
                await _userService.CreateAsync(dto);
            }
            else
            {
                var dto = new UpdateUserDto(_txtUsername.Text, _txtFullName.Text, role, _chkActive.Checked);
                await _userService.UpdateAsync(_user.Id, dto);
            }

            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            _btnSave.Enabled = true;
            UIHelper.ShowError(ex.Message);
        }
    }
}
