using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Desktop.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Desktop.Forms.Users;

public sealed class UsersForm : Form
{
    private readonly IUserService _userService;
    private DataGridView _grid = null!;
    private List<UserDto> _allUsers = new();

    public UsersForm(IServiceProvider serviceProvider)
    {
        _userService = serviceProvider.GetRequiredService<IUserService>();
        
        Text = "User Management";
        BackColor = AppTheme.BackgroundDark;
        FormBorderStyle = FormBorderStyle.None;
        Dock = DockStyle.Fill;
        
        InitializeUI();
    }

    private void InitializeUI()
    {
        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(30, 20, 30, 20) };
        var lblTitle = new Label { Text = "Users", Font = new Font("Segoe UI", 24f, FontStyle.Bold), ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(30, 20) };
        
        var btnAdd = new Button { Text = "+ Add User", Width = 150, Height = 40, Location = new Point(pnlHeader.Width - 180, 20), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        AppTheme.StylePrimaryButton(btnAdd);
        btnAdd.Click += async (s, e) => await ShowEditFormAsync(null);

        pnlHeader.Controls.AddRange(new Control[] { lblTitle, btnAdd });

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            ReadOnly = true,
            BackgroundColor = AppTheme.BackgroundDark,
            BorderStyle = BorderStyle.None,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Margin = new Padding(30)
        };
        _grid.EnableHeadersVisualStyles = false;
        _grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.BackgroundPanel;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextPrimary;
        _grid.ColumnHeadersDefaultCellStyle.Font = AppTheme.FontHeading;
        _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        _grid.ColumnHeadersHeight = 45;
        _grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        _grid.DefaultCellStyle.SelectionBackColor = AppTheme.BackgroundCard;
        _grid.DefaultCellStyle.SelectionForeColor = AppTheme.TextPrimary;
        _grid.DefaultCellStyle.BackColor = AppTheme.BackgroundDark;
        _grid.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
        _grid.DefaultCellStyle.Font = AppTheme.FontBody;
        _grid.RowTemplate.Height = 45;
        _grid.GridColor = AppTheme.BorderDefault;

        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Username", HeaderText = "Username" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FullName", HeaderText = "Full Name" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Role", HeaderText = "Role", Width = 120 });
        _grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "IsActive", HeaderText = "Active", Width = 80 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LastLoginAt", HeaderText = "Last Login", DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } });

        var editCol = new DataGridViewButtonColumn { HeaderText = "", Text = "Edit", UseColumnTextForButtonValue = true, Width = 80 };
        var resetCol = new DataGridViewButtonColumn { HeaderText = "", Text = "Reset Pwd", UseColumnTextForButtonValue = true, Width = 100 };
        var delCol = new DataGridViewButtonColumn { HeaderText = "", Text = "Delete", UseColumnTextForButtonValue = true, Width = 80 };

        _grid.Columns.AddRange(editCol, resetCol, delCol);

        _grid.CellClick += async (s, e) =>
        {
            if (e.RowIndex < 0) return;
            var user = _allUsers[e.RowIndex];

            if (e.ColumnIndex == editCol.Index)
            {
                await ShowEditFormAsync(user);
            }
            else if (e.ColumnIndex == resetCol.Index)
            {
                ResetPasswordAsync(user);
            }
            else if (e.ColumnIndex == delCol.Index)
            {
                await DeleteUserAsync(user);
            }
        };

        var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30, 0, 30, 30) };
        pnlGrid.Controls.Add(_grid);

        Controls.Add(pnlGrid);
        Controls.Add(pnlHeader);
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _allUsers = (await _userService.GetAllAsync()).ToList();
            _grid.DataSource = _allUsers;
        }
        catch (Exception ex)
        {
            UIHelper.ShowError($"Failed to load users: {ex.Message}");
        }
    }

    private async Task ShowEditFormAsync(UserDto? user)
    {
        using var form = new UserEditForm(_userService);
        form.SetUser(user);
        
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }

    private void ResetPasswordAsync(UserDto user)
    {
        var inputForm = new Form { Width = 400, Height = 250, Text = $"Reset Password - {user.Username}", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
        var lbl = new Label { Text = "New Password:", Location = new Point(30, 30), AutoSize = true };
        var txtPwd = new TextBox { Location = new Point(30, 60), Width = 320, UseSystemPasswordChar = true };
        var btnOk = new Button { Text = "Save", Location = new Point(250, 120), Width = 100, Height = 40 };
        AppTheme.StylePrimaryButton(btnOk);

        btnOk.Click += async (s, e) => {
            if (string.IsNullOrWhiteSpace(txtPwd.Text)) {
                UIHelper.ShowWarning("Password cannot be empty.");
                return;
            }
            try {
                await _userService.ResetPasswordAsync(user.Id, txtPwd.Text);
                UIHelper.ShowSuccess("Password reset successfully.");
                inputForm.DialogResult = DialogResult.OK;
            } catch (Exception ex) {
                UIHelper.ShowError(ex.Message);
            }
        };

        inputForm.Controls.AddRange(new Control[] { lbl, txtPwd, btnOk });
        inputForm.ShowDialog();
    }

    private async Task DeleteUserAsync(UserDto user)
    {
        if (SessionManager.CurrentUserId == user.Id)
        {
            UIHelper.ShowWarning("You cannot delete your own account while logged in.");
            return;
        }

        if (UIHelper.Confirm($"Are you sure you want to delete user '{user.Username}'?"))
        {
            try
            {
                await _userService.DeleteAsync(user.Id);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Failed to delete user: {ex.Message}");
            }
        }
    }
}
