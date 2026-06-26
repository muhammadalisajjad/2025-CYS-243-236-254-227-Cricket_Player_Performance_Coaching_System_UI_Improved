using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;

namespace CricketPlayerManagementSystem.Forms;

public class LoginForm : Form
{
    private readonly TextBox txtUsername = new();
    private readonly TextBox txtPassword = new();

    public LoginForm()
    {
        Text = "Login - Cricket Player Performance System";
        Width = 900;
        Height = 560;
        MinimumSize = new Size(760, 500);
        StartPosition = FormStartPosition.CenterScreen;
        Font = UiTheme.NormalFont;
        BuildUi();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        using var brush = new LinearGradientBrush(ClientRectangle, UiTheme.PrimaryDark, UiTheme.Primary, 35f);
        e.Graphics.FillRectangle(brush, ClientRectangle);
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(38),
            BackColor = Color.Transparent
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));

        var left = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(20, 50, 30, 20) };
        var title = new Label
        {
            Text = "Cricket Player\nPerformance & Coaching\nManagement System",
            Dock = DockStyle.Top,
            Height = 150,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 25, FontStyle.Bold)
        };
        var subtitle = new Label
        {
            Text = "Manage players, coaches, teams, fitness, training attendance, match performance and PDF reports from one clean dashboard.",
            Dock = DockStyle.Top,
            Height = 92,
            ForeColor = Color.FromArgb(220, 252, 231),
            Font = new Font("Segoe UI", 11, FontStyle.Regular)
        };
        var hint = new Label
        {
            Text = "Demo login: admin / admin123",
            Dock = DockStyle.Top,
            Height = 32,
            ForeColor = Color.FromArgb(187, 247, 208),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        left.Controls.Add(hint);
        left.Controls.Add(subtitle);
        left.Controls.Add(title);

        var card = UiTheme.CardPanel(28);
        card.Dock = DockStyle.Fill;
        card.Margin = new Padding(20, 35, 20, 35);

        var loginTitle = new Label
        {
            Text = "Welcome back",
            Dock = DockStyle.Top,
            Height = 42,
            Font = new Font("Segoe UI", 21, FontStyle.Bold),
            ForeColor = UiTheme.Text
        };
        var loginSub = new Label
        {
            Text = "Sign in to continue to the system",
            Dock = DockStyle.Top,
            Height = 34,
            Font = UiTheme.SmallFont,
            ForeColor = UiTheme.MutedText
        };

        var form = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 220,
            ColumnCount = 1,
            RowCount = 6,
            Padding = new Padding(0, 18, 0, 0)
        };
        for (int i = 0; i < 6; i++) form.RowStyles.Add(new RowStyle(SizeType.Absolute, i % 2 == 0 ? 28 : 46));

        UiTheme.StyleInput(txtUsername);
        UiTheme.StyleInput(txtPassword);
        txtPassword.UseSystemPasswordChar = true;
        txtUsername.Dock = DockStyle.Fill;
        txtPassword.Dock = DockStyle.Fill;
        txtUsername.PlaceholderText = "Enter username";
        txtPassword.PlaceholderText = "Enter password";

        form.Controls.Add(UiTheme.FieldLabel("Username"), 0, 0);
        form.Controls.Add(txtUsername, 0, 1);
        form.Controls.Add(UiTheme.FieldLabel("Password"), 0, 2);
        form.Controls.Add(txtPassword, 0, 3);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var btnLogin = UiTheme.PrimaryButton("Login");
        var btnExit = UiTheme.SecondaryButton("Exit");
        btnLogin.Width = 128;
        btnExit.Width = 100;
        btnLogin.Click += LoginClicked;
        btnExit.Click += (_, _) => Application.Exit();
        buttons.Controls.Add(btnLogin);
        buttons.Controls.Add(btnExit);
        form.Controls.Add(buttons, 0, 5);

        card.Controls.Add(form);
        card.Controls.Add(loginSub);
        card.Controls.Add(loginTitle);

        root.Controls.Add(left, 0, 0);
        root.Controls.Add(card, 1, 0);
        Controls.Add(root);
    }

    private void LoginClicked(object? sender, EventArgs e)
    {
        try
        {
            Validator.Require(Validator.Required(txtUsername.Text), "Username is required.");
            Validator.Require(Validator.Required(txtPassword.Text), "Password is required.");

            if (!AuthService.Login(txtUsername.Text.Trim(), txtPassword.Text))
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Hide();
            new DashboardForm().Show();
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(LoginForm));
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
