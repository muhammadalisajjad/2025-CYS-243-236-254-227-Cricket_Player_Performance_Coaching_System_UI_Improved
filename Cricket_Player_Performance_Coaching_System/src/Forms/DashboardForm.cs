using System;
using System.Drawing;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;

namespace CricketPlayerManagementSystem.Forms;

public class DashboardForm : BaseForm
{
    private readonly FlowLayoutPanel _statsPanel = new();

    public DashboardForm()
    {
        Text = "Dashboard - Cricket Player Performance System";
        BuildUi();
        Load += (_, _) => LoadStats();
    }

    private void BuildUi()
    {
        var shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = UiTheme.Background
        };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var sidebar = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.PrimaryDark,
            Padding = new Padding(16, 58, 16, 16)
        };

        var brand = new Label
        {
            Text = "CRICKET PMS",
            Dock = DockStyle.Top,
            Height = 58,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 17, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var menuPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.PrimaryDark,
            Padding = new Padding(0, 8, 0, 0)
        };

        AddMenuButton(menuPanel, "  Error Logs", () => new SimpleTableForm("Error Logs", "SELECT * FROM error_logs ORDER BY created_at DESC").ShowDialog());
        AddMenuButton(menuPanel, "  Reports", () => new ReportsForm().ShowDialog());
        AddMenuButton(menuPanel, "  Performance", () => new PerformanceForm().ShowDialog());
        AddMenuButton(menuPanel, "  Matches", () => new MatchForm().ShowDialog());
        AddMenuButton(menuPanel, "  Fitness", () => new FitnessForm().ShowDialog());
        AddMenuButton(menuPanel, "  Training", () => new TrainingForm().ShowDialog());
        AddMenuButton(menuPanel, "  Teams", () => new TeamForm().ShowDialog());
        AddMenuButton(menuPanel, "  Coaches", () => new SimpleTableForm("Coaches", "SELECT c.coach_id, u.full_name, c.specialization, c.experience_years FROM coaches c JOIN users u ON c.user_id=u.user_id").ShowDialog());
        AddMenuButton(menuPanel, "  Players", () => new PlayerForm().ShowDialog());

        sidebar.Controls.Add(menuPanel);
        sidebar.Controls.Add(brand);
        brand.BringToFront();

        var content = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Background,
            Padding = new Padding(22, 54, 22, 22)
        };

        var header = CreateHeader("Dashboard", "Welcome, " + SessionManager.CurrentFullName + "  |  Manage academy operations from one screen.");
        content.Controls.Add(header);

        _statsPanel.Dock = DockStyle.Top;
        _statsPanel.Height = 150;
        _statsPanel.Padding = new Padding(0, 18, 0, 8);
        _statsPanel.WrapContents = false;
        _statsPanel.AutoScroll = true;
        content.Controls.Add(_statsPanel);
        _statsPanel.BringToFront();

        var infoCard = UiTheme.CardPanel(24);
        infoCard.Dock = DockStyle.Fill;

        var infoTitle = new Label
        {
            Text = "System Modules",
            Dock = DockStyle.Top,
            Height = 36,
            Font = UiTheme.SectionFont,
            ForeColor = UiTheme.Text
        };

        var info = new Label
        {
            Text = "Use the sidebar to manage cricket players, teams, training sessions, fitness records, matches, performance, and PDF reports.\n\nTeams, Matches, Training and Fitness now open proper forms where you can add, edit and delete data.",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11),
            ForeColor = UiTheme.MutedText
        };

        infoCard.Controls.Add(info);
        infoCard.Controls.Add(infoTitle);
        content.Controls.Add(infoCard);
        infoCard.BringToFront();

        shell.Controls.Add(sidebar, 0, 0);
        shell.Controls.Add(content, 1, 0);
        Controls.Add(shell);
        MainMenu.BringToFront();
    }

    private void LoadStats()
    {
        _statsPanel.Controls.Clear();
        AddStatCard("Players", Count("players"), "Registered players");
        AddStatCard("Coaches", Count("coaches"), "Academy coaches");
        AddStatCard("Teams", Count("teams"), "Cricket teams");
        AddStatCard("Matches", Count("matches"), "Scheduled matches");
    }

    private static int Count(string table)
    {
        try
        {
            return Convert.ToInt32(DbHelper.ExecuteScalar($"SELECT COUNT(*) FROM {table}") ?? 0);
        }
        catch
        {
            return 0;
        }
    }

    private void AddStatCard(string title, int count, string subtitle)
    {
        var card = UiTheme.CardPanel(16);
        card.Width = 215;
        card.Height = 110;
        card.Margin = new Padding(0, 0, 16, 0);

        var lblTitle = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 28,
            Font = UiTheme.SectionFont,
            ForeColor = UiTheme.Text
        };

        var lblCount = new Label
        {
            Text = count.ToString(),
            Dock = DockStyle.Top,
            Height = 42,
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = UiTheme.Primary
        };

        var lblSub = new Label
        {
            Text = subtitle,
            Dock = DockStyle.Fill,
            Font = UiTheme.SmallFont,
            ForeColor = UiTheme.MutedText
        };

        card.Controls.Add(lblSub);
        card.Controls.Add(lblCount);
        card.Controls.Add(lblTitle);
        _statsPanel.Controls.Add(card);
    }

    private static void AddMenuButton(Control parent, string text, Action onClick)
    {
        var button = UiTheme.SidebarButton(text);
        button.Click += (_, _) => onClick();
        parent.Controls.Add(button);
        button.BringToFront();
    }
}