using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;
using MySql.Data.MySqlClient;

namespace CricketPlayerManagementSystem.Forms;

public class ReportsForm : BaseForm
{
    private readonly ComboBox cmbReport = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown numPlayer = new() { Minimum = 0, Maximum = 100000 };
    private readonly NumericUpDown numTeam = new() { Minimum = 0, Maximum = 100000 };
    private readonly NumericUpDown numMatch = new() { Minimum = 0, Maximum = 100000 };
    private readonly DateTimePicker dtFrom = new() { Value = DateTime.Today.AddMonths(-1) };
    private readonly DateTimePicker dtTo = new() { Value = DateTime.Today };
    private readonly TextBox txtOutput = new();
    private readonly DataGridView preview = new() { Dock = DockStyle.Fill };

    public ReportsForm()
    {
        Text = "PDF Reports";
        WindowState = FormWindowState.Maximized;
        BuildUi();
    }

    private void BuildUi()
    {
        BackColor = UiTheme.Background;

        foreach (Control input in new Control[] { cmbReport, numPlayer, numTeam, numMatch, dtFrom, dtTo, txtOutput })
        {
            UiTheme.StyleInput(input);
            input.Margin = new Padding(0);
        }

        cmbReport.Items.AddRange(ReportGenerator.ReportNames.Cast<object>().ToArray());
        if (cmbReport.Items.Count > 0)
            cmbReport.SelectedIndex = 0;

        txtOutput.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "cricket_report.pdf"
        );

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Background,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(18)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var header = CreateHeader(
            "Reports",
            "Generate business-level PDF reports using player, team, match and date parameters."
        );
        header.Dock = DockStyle.Fill;
        root.Controls.Add(header, 0, 0);
        root.SetColumnSpan(header, 2);

        var leftHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Background,
            AutoScroll = true,
            Padding = new Padding(0, 8, 12, 0)
        };

        var rightHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Background,
            Padding = new Padding(12, 8, 0, 0)
        };

        root.Controls.Add(leftHost, 0, 1);
        root.Controls.Add(rightHost, 1, 1);

        var formCard = UiTheme.CardPanel(18);
        formCard.Dock = DockStyle.Top;
        formCard.AutoSize = true;
        formCard.AutoSizeMode = AutoSizeMode.GrowAndShrink;

        var title = new Label
        {
            Text = "Report Parameters",
            Dock = DockStyle.Top,
            Height = 34,
            Font = UiTheme.SectionFont,
            ForeColor = UiTheme.Text
        };

        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2,
            Padding = new Padding(0, 10, 0, 0)
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        AddField(fields, "Report Type", cmbReport, 46);
        AddField(fields, "Player ID", numPlayer, 46);
        AddField(fields, "Team ID", numTeam, 46);
        AddField(fields, "Match ID", numMatch, 46);
        AddField(fields, "Date From", dtFrom, 46);
        AddField(fields, "Date To", dtTo, 46);
        AddField(fields, "Output Path", txtOutput, 56);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 96,
            Padding = new Padding(130, 12, 0, 0),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };

        var btnPreview = UiTheme.SecondaryButton("Preview Data");
        var btnBrowse = UiTheme.SecondaryButton("Browse");
        var btnGenerate = UiTheme.PrimaryButton("Generate PDF");

        btnPreview.Width = 125;
        btnBrowse.Width = 90;
        btnGenerate.Width = 135;

        btnPreview.Click += (_, _) => LoadPreview();
        btnBrowse.Click += BrowseClicked;
        btnGenerate.Click += GenerateClicked;

        actions.Controls.Add(btnPreview);
        actions.Controls.Add(btnBrowse);
        actions.Controls.Add(btnGenerate);

        var help = new Label
        {
            Text = "Tip: Use 0 for Player ID, Team ID or Match ID when you want all records where supported.",
            Dock = DockStyle.Top,
            Height = 52,
            Font = UiTheme.SmallFont,
            ForeColor = UiTheme.MutedText
        };

        formCard.Controls.Add(help);
        formCard.Controls.Add(actions);
        formCard.Controls.Add(fields);
        formCard.Controls.Add(title);

        leftHost.Controls.Add(formCard);

        var previewCard = UiTheme.CardPanel(16);
        previewCard.Dock = DockStyle.Fill;

        var previewTitle = new Label
        {
            Text = "Report Preview",
            Dock = DockStyle.Top,
            Height = 34,
            Font = UiTheme.SectionFont,
            ForeColor = UiTheme.Text
        };

        UiTheme.StyleGrid(preview);
        preview.ReadOnly = true;
        preview.AllowUserToAddRows = false;
        preview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        preview.MultiSelect = false;

        previewCard.Controls.Add(preview);
        previewCard.Controls.Add(previewTitle);

        rightHost.Controls.Add(previewCard);

        Controls.Add(root);
        MainMenu.BringToFront();
    }

    private static void AddField(TableLayoutPanel table, string labelText, Control input, int rowHeight)
    {
        int row = table.RowCount;
        table.RowCount += 1;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));

        var label = UiTheme.FieldLabel(labelText);
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.Margin = new Padding(0, 0, 10, 10);

        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(0, 0, 0, 10);

        table.Controls.Add(label, 0, row);
        table.Controls.Add(input, 1, row);
    }

    private void BrowseClicked(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "PDF Files (*.pdf)|*.pdf",
            FileName = "cricket_report.pdf"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
            txtOutput.Text = dialog.FileName;
    }

    private void LoadPreview()
    {
        try
        {
            preview.DataSource = cmbReport.Text switch
            {
                "Player Profile Report" => DbHelper.ExecuteDataTable(
                    "SELECT * FROM v_player_profile WHERE (@id = 0 OR player_id = @id)",
                    new MySqlParameter("@id", (int)numPlayer.Value)
                ),

                "Team Player List Report" => DbHelper.ExecuteDataTable(
                    "SELECT * FROM v_team_players WHERE (@id = 0 OR team_id = @id)",
                    new MySqlParameter("@id", (int)numTeam.Value)
                ),

                "Fitness Progress Report" => DbHelper.ExecuteDataTable(
                    "SELECT * FROM fitness_records WHERE (@id = 0 OR player_id = @id) AND record_date BETWEEN @from AND @to",
                    new MySqlParameter("@id", (int)numPlayer.Value),
                    new MySqlParameter("@from", dtFrom.Value.Date),
                    new MySqlParameter("@to", dtTo.Value.Date)
                ),

                "Injury Status Report" => DbHelper.ExecuteDataTable(
                    "SELECT * FROM injury_records WHERE (@id = 0 OR player_id = @id)",
                    new MySqlParameter("@id", (int)numPlayer.Value)
                ),

                "Training Attendance Report" => DbHelper.ExecuteDataTable(
                    "SELECT * FROM v_training_attendance_summary WHERE session_date BETWEEN @from AND @to",
                    new MySqlParameter("@from", dtFrom.Value.Date),
                    new MySqlParameter("@to", dtTo.Value.Date)
                ),

                "Batting Performance Report" => DbHelper.ExecuteDataTable(
                    "SELECT * FROM batting_performance WHERE (@match = 0 OR match_id = @match) AND (@player = 0 OR player_id = @player)",
                    new MySqlParameter("@match", (int)numMatch.Value),
                    new MySqlParameter("@player", (int)numPlayer.Value)
                ),

                "Bowling Performance Report" => DbHelper.ExecuteDataTable(
                    "SELECT * FROM bowling_performance WHERE (@match = 0 OR match_id = @match) AND (@player = 0 OR player_id = @player)",
                    new MySqlParameter("@match", (int)numMatch.Value),
                    new MySqlParameter("@player", (int)numPlayer.Value)
                ),

                "Overall Player Ranking Report" => DbHelper.ExecuteDataTable(
                    "SELECT * FROM v_top_batsmen LIMIT 20"
                ),

                _ => DbHelper.ExecuteDataTable(
                    "SELECT report_id, report_name, parameters, generated_at FROM report_audit ORDER BY generated_at DESC LIMIT 50"
                )
            };
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(ReportsForm));
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void GenerateClicked(object? sender, EventArgs e)
    {
        try
        {
            Validator.Require(dtTo.Value.Date >= dtFrom.Value.Date, "Date To must be after Date From.");

            ReportGenerator.Generate(
                cmbReport.Text,
                txtOutput.Text,
                numPlayer.Value == 0 ? null : (int)numPlayer.Value,
                numTeam.Value == 0 ? null : (int)numTeam.Value,
                numMatch.Value == 0 ? null : (int)numMatch.Value,
                dtFrom.Value.Date,
                dtTo.Value.Date
            );

            MessageBox.Show(
                "PDF report generated:\n" + txtOutput.Text,
                "Report Generated",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            LoadPreview();
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(ReportsForm));
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}