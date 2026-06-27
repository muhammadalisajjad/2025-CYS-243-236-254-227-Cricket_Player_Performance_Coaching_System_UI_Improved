using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;
using MySql.Data.MySqlClient;

namespace CricketPlayerManagementSystem.Forms;

public class MatchForm : BaseForm
{
    private int? editingId;

    private readonly ComboBox cmbTournament = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox cmbTeam = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox txtOpponent = new();
    private readonly DateTimePicker dtMatchDate = new();
    private readonly TextBox txtVenue = new();
    private readonly ComboBox cmbMatchType = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox cmbResult = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DataGridView grid = new() { Dock = DockStyle.Fill };
    private readonly Label lblTitle = new();

    public MatchForm()
    {
        Text = "Matches";
        WindowState = FormWindowState.Maximized;
        BuildUi();
        Load += (_, _) =>
        {
            LoadTournaments();
            LoadTeams();
            LoadData();
        };
    }

    private void BuildUi()
    {
        dtMatchDate.Format = DateTimePickerFormat.Short;

        cmbMatchType.Items.AddRange(new object[] { "T20", "ODI", "Test", "Practice" });
        cmbResult.Items.AddRange(new object[] { "Upcoming", "Won", "Lost", "Tie", "No Result" });
        cmbMatchType.SelectedIndex = 0;
        cmbResult.SelectedIndex = 0;

        foreach (Control c in new Control[] { cmbTournament, cmbTeam, txtOpponent, dtMatchDate, txtVenue, cmbMatchType, cmbResult })
            UiTheme.StyleInput(c);

        UiTheme.StyleGrid(grid);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            BackColor = UiTheme.Background,
            Padding = new Padding(18)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 450));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = CreateHeader("Matches", "Manage matches, tournaments, teams and results.");
        root.Controls.Add(header, 0, 0);
        root.SetColumnSpan(header, 2);

        var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 12, 0), AutoScroll = true, BackColor = UiTheme.Background };
        var right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 8, 0, 0), BackColor = UiTheme.Background };
        root.Controls.Add(left, 0, 1);
        root.Controls.Add(right, 1, 1);

        var formCard = UiTheme.CardPanel(18);
        formCard.Dock = DockStyle.Top;
        formCard.AutoSize = true;

        lblTitle.Text = "Add Match";
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Height = 34;
        lblTitle.Font = UiTheme.SectionFont;
        lblTitle.ForeColor = UiTheme.Text;

        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2,
            Padding = new Padding(0, 10, 0, 0)
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddField(fields, "Tournament", cmbTournament);
        AddField(fields, "Team", cmbTeam);
        AddField(fields, "Opponent", txtOpponent);
        AddField(fields, "Match Date", dtMatchDate);
        AddField(fields, "Venue", txtVenue);
        AddField(fields, "Match Type", cmbMatchType);
        AddField(fields, "Result", cmbResult);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 54,
            Padding = new Padding(130, 12, 0, 0)
        };

        var btnSave = UiTheme.PrimaryButton("Save");
        var btnClear = UiTheme.SecondaryButton("Clear");
        btnSave.Width = 110;
        btnClear.Width = 90;
        btnSave.Click += SaveClicked;
        btnClear.Click += (_, _) => ClearForm();

        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnClear);

        formCard.Controls.Add(buttons);
        formCard.Controls.Add(fields);
        formCard.Controls.Add(lblTitle);
        left.Controls.Add(formCard);

        var tableCard = UiTheme.CardPanel(16);
        tableCard.Dock = DockStyle.Fill;

        var tableTitle = new Label
        {
            Text = "Match List",
            Dock = DockStyle.Top,
            Height = 34,
            Font = UiTheme.SectionFont,
            ForeColor = UiTheme.Text
        };

        tableCard.Controls.Add(grid);
        tableCard.Controls.Add(tableTitle);
        right.Controls.Add(tableCard);

        grid.CellContentClick += GridCellContentClick;

        Controls.Add(root);
        MainMenu.BringToFront();
    }

    private static void AddField(TableLayoutPanel table, string labelText, Control input)
    {
        int row = table.RowCount;
        table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        var lbl = UiTheme.FieldLabel(labelText);
        lbl.Dock = DockStyle.Fill;
        lbl.TextAlign = ContentAlignment.MiddleLeft;
        lbl.Margin = new Padding(0, 0, 10, 10);

        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(0, 0, 0, 10);

        table.Controls.Add(lbl, 0, row);
        table.Controls.Add(input, 1, row);
    }

    private void LoadTournaments()
    {
        var dt = DbHelper.ExecuteDataTable("SELECT tournament_id, tournament_name FROM tournaments ORDER BY tournament_name");
        var row = dt.NewRow();
        row["tournament_id"] = 0;
        row["tournament_name"] = "No Tournament";
        dt.Rows.InsertAt(row, 0);

        cmbTournament.DisplayMember = "tournament_name";
        cmbTournament.ValueMember = "tournament_id";
        cmbTournament.DataSource = dt;
    }

    private void LoadTeams()
    {
        var dt = DbHelper.ExecuteDataTable("SELECT team_id, team_name FROM teams ORDER BY team_name");
        cmbTeam.DisplayMember = "team_name";
        cmbTeam.ValueMember = "team_id";
        cmbTeam.DataSource = dt;
    }

    private void LoadData()
    {
        try
        {
            grid.Columns.Clear();
            grid.DataSource = DbHelper.ExecuteDataTable(@"
                SELECT m.match_id,
                       IFNULL(t.tournament_name, '-') AS tournament,
                       tm.team_name,
                       m.opponent_name,
                       m.match_date,
                       m.venue,
                       m.match_type,
                       m.result
                FROM matches m
                LEFT JOIN tournaments t ON m.tournament_id = t.tournament_id
                JOIN teams tm ON m.team_id = tm.team_id
                ORDER BY m.match_id DESC
            ");

            if (!grid.Columns.Contains("Edit"))
            {
                grid.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Edit",
                    HeaderText = "Edit",
                    Text = "Edit",
                    UseColumnTextForButtonValue = true,
                    Width = 80
                });
            }

            if (!grid.Columns.Contains("Delete"))
            {
                grid.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Delete",
                    HeaderText = "Delete",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true,
                    Width = 80
                });
            }
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(MatchForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            Validator.Require(Validator.Required(txtOpponent.Text), "Opponent is required.");
            Validator.Require(Validator.Required(txtVenue.Text), "Venue is required.");

            object tournamentValue = Convert.ToInt32(cmbTournament.SelectedValue) == 0
                ? DBNull.Value
                : Convert.ToInt32(cmbTournament.SelectedValue);

            if (editingId == null)
            {
                DbHelper.ExecuteNonQuery(@"
                    INSERT INTO matches(tournament_id, team_id, opponent_name, match_date, venue, match_type, result)
                    VALUES(@tournament, @team, @opponent, @date, @venue, @type, @result)",
                    new MySqlParameter("@tournament", tournamentValue),
                    new MySqlParameter("@team", Convert.ToInt32(cmbTeam.SelectedValue)),
                    new MySqlParameter("@opponent", txtOpponent.Text.Trim()),
                    new MySqlParameter("@date", dtMatchDate.Value.Date),
                    new MySqlParameter("@venue", txtVenue.Text.Trim()),
                    new MySqlParameter("@type", cmbMatchType.Text),
                    new MySqlParameter("@result", cmbResult.Text)
                );
            }
            else
            {
                DbHelper.ExecuteNonQuery(@"
                    UPDATE matches
                    SET tournament_id=@tournament, team_id=@team, opponent_name=@opponent,
                        match_date=@date, venue=@venue, match_type=@type, result=@result
                    WHERE match_id=@id",
                    new MySqlParameter("@tournament", tournamentValue),
                    new MySqlParameter("@team", Convert.ToInt32(cmbTeam.SelectedValue)),
                    new MySqlParameter("@opponent", txtOpponent.Text.Trim()),
                    new MySqlParameter("@date", dtMatchDate.Value.Date),
                    new MySqlParameter("@venue", txtVenue.Text.Trim()),
                    new MySqlParameter("@type", cmbMatchType.Text),
                    new MySqlParameter("@result", cmbResult.Text),
                    new MySqlParameter("@id", editingId.Value)
                );
            }

            MessageBox.Show("Match saved successfully.");
            ClearForm();
            LoadData();
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(MatchForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void GridCellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

        var id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["match_id"].Value);
        var columnName = grid.Columns[e.ColumnIndex].Name;

        try
        {
            if (columnName == "Edit")
            {
                var row = DbHelper.ExecuteDataTable("SELECT * FROM matches WHERE match_id=@id", new MySqlParameter("@id", id)).Rows[0];

                editingId = id;
                cmbTournament.SelectedValue = row["tournament_id"] == DBNull.Value ? 0 : Convert.ToInt32(row["tournament_id"]);
                cmbTeam.SelectedValue = Convert.ToInt32(row["team_id"]);
                txtOpponent.Text = row["opponent_name"].ToString();
                dtMatchDate.Value = Convert.ToDateTime(row["match_date"]);
                txtVenue.Text = row["venue"].ToString();
                cmbMatchType.Text = row["match_type"].ToString();
                cmbResult.Text = row["result"].ToString();
                lblTitle.Text = "Edit Match";
            }
            else if (columnName == "Delete")
            {
                var confirm = MessageBox.Show("Delete this match?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    DbHelper.ExecuteNonQuery("DELETE FROM matches WHERE match_id=@id", new MySqlParameter("@id", id));
                    LoadData();
                }
            }
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(MatchForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void ClearForm()
    {
        editingId = null;
        cmbTournament.SelectedIndex = 0;
        if (cmbTeam.Items.Count > 0) cmbTeam.SelectedIndex = 0;
        txtOpponent.Clear();
        txtVenue.Clear();
        dtMatchDate.Value = DateTime.Today;
        cmbMatchType.SelectedIndex = 0;
        cmbResult.SelectedIndex = 0;
        lblTitle.Text = "Add Match";
    }
}