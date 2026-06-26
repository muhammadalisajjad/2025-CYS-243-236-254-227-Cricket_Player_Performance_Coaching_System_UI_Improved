using System.Drawing;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;

namespace CricketPlayerManagementSystem.Forms;

public class BaseForm : Form
{
    protected MenuStrip MainMenu = new();

    public BaseForm()
    {
        Width = 1180;
        Height = 760;
        MinimumSize = new Size(980, 640);
        StartPosition = FormStartPosition.CenterScreen;
        Font = UiTheme.NormalFont;
        BackColor = UiTheme.Background;
        BuildFileMenu();
    }

    private void BuildFileMenu()
    {
        MainMenu = new MenuStrip
        {
            BackColor = UiTheme.PrimaryDark,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Padding = new Padding(8, 3, 8, 3)
        };

        var file = new ToolStripMenuItem("File") { ForeColor = Color.White };
        var logout = new ToolStripMenuItem("Logout");
        var exit = new ToolStripMenuItem("Exit");

        logout.Click += (_, _) =>
        {
            SessionManager.Clear();
            Hide();
            new LoginForm().Show();
        };

        exit.Click += (_, _) => Application.Exit();
        file.DropDownItems.Add(logout);
        file.DropDownItems.Add(exit);
        MainMenu.Items.Add(file);
        Controls.Add(MainMenu);
        MainMenu.Dock = DockStyle.Top;
        MainMenu.BringToFront();
    }

    protected Panel CreateHeader(string title, string subtitle)
    {
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 92,
            BackColor = UiTheme.Card,
            Padding = new Padding(24, 14, 24, 8)
        };

        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 36,
            Font = UiTheme.TitleFont,
            ForeColor = UiTheme.Text
        };

        var subLabel = new Label
        {
            Text = subtitle,
            Dock = DockStyle.Top,
            Height = 24,
            Font = UiTheme.SmallFont,
            ForeColor = UiTheme.MutedText
        };

        header.Controls.Add(subLabel);
        header.Controls.Add(titleLabel);
        titleLabel.BringToFront();
        return header;
    }

    protected DataGridView CreateStyledGrid()
    {
        var grid = new DataGridView { Dock = DockStyle.Fill };
        UiTheme.StyleGrid(grid);
        return grid;
    }

    protected DataGridView CreateGridWithButtons()
    {
        var grid = CreateStyledGrid();
        grid.Columns.Add(new DataGridViewButtonColumn { Name = "Edit", HeaderText = "Edit", Text = "Edit", UseColumnTextForButtonValue = true });
        grid.Columns.Add(new DataGridViewButtonColumn { Name = "Delete", HeaderText = "Delete", Text = "Delete", UseColumnTextForButtonValue = true });
        return grid;
    }
}
