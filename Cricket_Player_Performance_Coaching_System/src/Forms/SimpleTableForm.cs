using System;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;

namespace CricketPlayerManagementSystem.Forms;

public class SimpleTableForm : BaseForm
{
    private readonly string _query;
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill };

    public SimpleTableForm(string title, string query)
    {
        Text = title;
        _query = query;
        BuildUi(title);
        Load += (_, _) => LoadData();
    }

    private void BuildUi(string title)
    {
        var shell = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Background, Padding = new Padding(18, 54, 18, 18) };
        shell.Controls.Add(CreateHeader(title, "View database records in a clean responsive table."));

        var card = UiTheme.CardPanel(16);
        card.Dock = DockStyle.Fill;

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 6, 0, 8)
        };
        var refresh = UiTheme.PrimaryButton("Refresh");
        refresh.Width = 110;
        refresh.Click += (_, _) => LoadData();
        toolbar.Controls.Add(refresh);

        UiTheme.StyleGrid(_grid);
        card.Controls.Add(_grid);
        card.Controls.Add(toolbar);
        toolbar.BringToFront();

        shell.Controls.Add(card);
        card.BringToFront();
        Controls.Add(shell);
        MainMenu.BringToFront();
    }

    private void LoadData()
    {
        try
        {
            _grid.DataSource = DbHelper.ExecuteDataTable(_query);
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, Text);
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
