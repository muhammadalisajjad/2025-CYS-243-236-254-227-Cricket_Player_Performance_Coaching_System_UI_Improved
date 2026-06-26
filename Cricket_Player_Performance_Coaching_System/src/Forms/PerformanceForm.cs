using System;
using System.Drawing;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;

namespace CricketPlayerManagementSystem.Forms;

public class PerformanceForm : BaseForm
{
    private readonly ComboBox cmbMatch = new();
    private readonly ComboBox cmbPlayer = new();
    private readonly NumericUpDown numRuns = new() { Maximum = 500 };
    private readonly NumericUpDown numBalls = new() { Maximum = 300 };
    private readonly NumericUpDown numFours = new() { Maximum = 100 };
    private readonly NumericUpDown numSixes = new() { Maximum = 100 };
    private readonly NumericUpDown numOvers = new() { DecimalPlaces = 1, Maximum = 50, Increment = 0.1M };
    private readonly NumericUpDown numMaidens = new() { Maximum = 50 };
    private readonly NumericUpDown numRunsGiven = new() { Maximum = 500 };
    private readonly NumericUpDown numWickets = new() { Maximum = 10 };
    private readonly NumericUpDown numCatches = new() { Maximum = 10 };
    private readonly NumericUpDown numRunouts = new() { Maximum = 10 };
    private readonly NumericUpDown numStumpings = new() { Maximum = 10 };

    public PerformanceForm()
    {
        Text = "Match Performance Entry";
        BuildUi();
        Load += (_, _) => LoadLookups();
    }

    private void BuildUi()
    {
        foreach (Control input in new Control[] { cmbMatch, cmbPlayer, numRuns, numBalls, numFours, numSixes, numOvers, numMaidens, numRunsGiven, numWickets, numCatches, numRunouts, numStumpings })
        {
            UiTheme.StyleInput(input);
        }

        var shell = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Background, Padding = new Padding(18, 54, 18, 18) };
        shell.Controls.Add(CreateHeader("Match Performance", "Enter batting, bowling and fielding performance in one transaction."));

        var card = UiTheme.CardPanel(22);
        card.Dock = DockStyle.Fill;

        var title = new Label
        {
            Text = "Performance Entry Form",
            Dock = DockStyle.Top,
            Height = 36,
            Font = UiTheme.SectionFont,
            ForeColor = UiTheme.Text
        };

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 4,
            Padding = new Padding(0, 10, 0, 0)
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        AddRow(grid, "Match", cmbMatch, "Player", cmbPlayer);
        AddSection(grid, "Batting Details");
        AddRow(grid, "Runs", numRuns, "Balls", numBalls);
        AddRow(grid, "Fours", numFours, "Sixes", numSixes);
        AddSection(grid, "Bowling Details");
        AddRow(grid, "Overs", numOvers, "Maidens", numMaidens);
        AddRow(grid, "Runs Given", numRunsGiven, "Wickets", numWickets);
        AddSection(grid, "Fielding Details");
        AddRow(grid, "Catches", numCatches, "Run Outs", numRunouts);
        AddSingleRow(grid, "Stumpings", numStumpings);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 56,
            Padding = new Padding(130, 12, 0, 0)
        };
        var save = UiTheme.PrimaryButton("Save Performance");
        var clear = UiTheme.SecondaryButton("Clear");
        save.Width = 165;
        clear.Width = 90;
        save.Click += SaveClicked;
        clear.Click += (_, _) => ClearInputs();
        actions.Controls.Add(save);
        actions.Controls.Add(clear);

        card.Controls.Add(actions);
        card.Controls.Add(grid);
        card.Controls.Add(title);
        title.BringToFront();
        shell.Controls.Add(card);
        card.BringToFront();
        Controls.Add(shell);
        MainMenu.BringToFront();
    }

    private static void AddRow(TableLayoutPanel table, string label1, Control input1, string label2, Control input2)
    {
        int row = table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        input1.Dock = DockStyle.Fill;
        input2.Dock = DockStyle.Fill;
        input1.Margin = new Padding(0, 0, 18, 10);
        input2.Margin = new Padding(0, 0, 0, 10);
        table.Controls.Add(UiTheme.FieldLabel(label1), 0, row);
        table.Controls.Add(input1, 1, row);
        table.Controls.Add(UiTheme.FieldLabel(label2), 2, row);
        table.Controls.Add(input2, 3, row);
    }

    private static void AddSingleRow(TableLayoutPanel table, string label, Control input)
    {
        int row = table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(0, 0, 18, 10);
        table.Controls.Add(UiTheme.FieldLabel(label), 0, row);
        table.Controls.Add(input, 1, row);
    }

    private static void AddSection(TableLayoutPanel table, string text)
    {
        int row = table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        var label = new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            Font = UiTheme.SectionFont,
            ForeColor = UiTheme.Primary,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(0, 8, 0, 0)
        };
        table.Controls.Add(label, 0, row);
        table.SetColumnSpan(label, 4);
    }

    private void LoadLookups()
    {
        cmbMatch.DisplayMember = "match_name";
        cmbMatch.ValueMember = "match_id";
        cmbMatch.DataSource = DbHelper.ExecuteDataTable("SELECT match_id, CONCAT(opponent_name, ' - ', DATE_FORMAT(match_date, '%d %b %Y')) AS match_name FROM matches ORDER BY match_date DESC");

        cmbPlayer.DisplayMember = "full_name";
        cmbPlayer.ValueMember = "player_id";
        cmbPlayer.DataSource = DbHelper.ExecuteDataTable("SELECT p.player_id, u.full_name FROM players p JOIN users u ON p.user_id=u.user_id ORDER BY u.full_name");
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            Validator.Require(cmbMatch.SelectedValue != null, "Please select a match.");
            Validator.Require(cmbPlayer.SelectedValue != null, "Please select a player.");

            TransactionService.SaveMatchPerformance(
                Convert.ToInt32(cmbMatch.SelectedValue), Convert.ToInt32(cmbPlayer.SelectedValue), (int)numRuns.Value, (int)numBalls.Value,
                (int)numFours.Value, (int)numSixes.Value, numOvers.Value, (int)numMaidens.Value,
                (int)numRunsGiven.Value, (int)numWickets.Value, (int)numCatches.Value,
                (int)numRunouts.Value, (int)numStumpings.Value);
            MessageBox.Show("Performance saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearInputs();
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(PerformanceForm));
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ClearInputs()
    {
        foreach (var input in new NumericUpDown[] { numRuns, numBalls, numFours, numSixes, numOvers, numMaidens, numRunsGiven, numWickets, numCatches, numRunouts, numStumpings })
        {
            input.Value = 0;
        }
    }
}
