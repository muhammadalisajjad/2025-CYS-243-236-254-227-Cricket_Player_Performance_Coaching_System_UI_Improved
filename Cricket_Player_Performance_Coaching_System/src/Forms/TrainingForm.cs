using System;
using System.Drawing;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;
using MySql.Data.MySqlClient;

namespace CricketPlayerManagementSystem.Forms;

public class TrainingForm : BaseForm
{
    private int? editingId;

    private readonly ComboBox cmbCoach = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox cmbTeam = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker dtSessionDate = new();
    private readonly TextBox txtTitle = new();
    private readonly ComboBox cmbFocus = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker tmStart = new();
    private readonly DateTimePicker tmEnd = new();
    private readonly TextBox txtLocation = new();
    private readonly TextBox txtNotes = new() { Multiline = true, ScrollBars = ScrollBars.Vertical };
    private readonly DataGridView grid = new() { Dock = DockStyle.Fill };
    private readonly Label lblTitle = new();

    public TrainingForm()
    {
        Text = "Training Sessions";
        WindowState = FormWindowState.Maximized;
        BuildUi();
        Load += (_, _) =>
        {
            LoadCoaches();
            LoadTeams();
            LoadData();
        };
    }

    private void BuildUi()
    {
        dtSessionDate.Format = DateTimePickerFormat.Short;

        tmStart.Format = DateTimePickerFormat.Custom;
        tmStart.CustomFormat = "hh:mm tt";
        tmStart.ShowUpDown = true;

        tmEnd.Format = DateTimePickerFormat.Custom;
        tmEnd.CustomFormat = "hh:mm tt";
        tmEnd.ShowUpDown = true;

        cmbFocus.Items.AddRange(new object[] { "Batting", "Bowling", "Fielding", "Fitness", "Strategy" });
        cmbFocus.SelectedIndex = 0;

        foreach (Control c in new Control[] { cmbCoach, cmbTeam, dtSessionDate, txtTitle, cmbFocus, tmStart, tmEnd, txtLocation, txtNotes })
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
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 470));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = CreateHeader("Training Sessions", "Create and manage training plans for teams.");
        root.Controls.Add(header, 0, 0);
        root.SetColumnSpan(header, 2);

        var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 12, 0), AutoScroll = true, BackColor = UiTheme.Background };
        var right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 8, 0, 0), BackColor = UiTheme.Background };
        root.Controls.Add(left, 0, 1);
        root.Controls.Add(right, 1, 1);

        var formCard = UiTheme.CardPanel(18);
        formCard.Dock = DockStyle.Top;
        formCard.AutoSize = true;

        lblTitle.Text = "Add Training Session";
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
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddField(fields, "Coach", cmbCoach);
        AddField(fields, "Team", cmbTeam);
        AddField(fields, "Session Date", dtSessionDate);
        AddField(fields, "Title", txtTitle);
        AddField(fields, "Focus Area", cmbFocus);
        AddField(fields, "Start Time", tmStart);
        AddField(fields, "End Time", tmEnd);
        AddField(fields, "Location", txtLocation);
        AddField(fields, "Notes", txtNotes, 100);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 54,
            Padding = new Padding(140, 12, 0, 0)
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
            Text = "Training Session List",
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

    private static void AddField(TableLayoutPanel table, string labelText, Control input, int height = 46)
    {
        int row = table.RowCount;
        table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, height));

        var lbl = UiTheme.FieldLabel(labelText);
        lbl.Dock = DockStyle.Fill;
        lbl.TextAlign = ContentAlignment.MiddleLeft;
        lbl.Margin = new Padding(0, 0, 10, 10);

        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(0, 0, 0, 10);

        table.Controls.Add(lbl, 0, row);
        table.Controls.Add(input, 1, row);
    }

    private void LoadCoaches()
    {
        var dt = DbHelper.ExecuteDataTable(@"
            SELECT c.coach_id, u.full_name
            FROM coaches c
            JOIN users u ON c.user_id = u.user_id
            ORDER BY u.full_name
        ");
        cmbCoach.DisplayMember = "full_name";
        cmbCoach.ValueMember = "coach_id";
        cmbCoach.DataSource = dt;
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
                SELECT ts.session_id,
                       u.full_name AS coach_name,
                       tm.team_name,
                       ts.session_date,
                       ts.title,
                       ts.focus_area,
                       ts.start_time,
                       ts.end_time,
                       ts.location
                FROM training_sessions ts
                JOIN coaches c ON ts.coach_id = c.coach_id
                JOIN users u ON c.user_id = u.user_id
                JOIN teams tm ON ts.team_id = tm.team_id
                ORDER BY ts.session_id DESC
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
            LoggerService.LogError(ex, nameof(TrainingForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            Validator.Require(Validator.Required(txtTitle.Text), "Title is required.");
            Validator.Require(Validator.Required(txtLocation.Text), "Location is required.");
            Validator.Require(tmEnd.Value.TimeOfDay > tmStart.Value.TimeOfDay, "End time must be after start time.");

            if (editingId == null)
            {
                DbHelper.ExecuteNonQuery(@"
                    INSERT INTO training_sessions
                    (coach_id, team_id, session_date, title, focus_area, start_time, end_time, location, notes)
                    VALUES(@coach, @team, @date, @title, @focus, @start, @end, @location, @notes)",
                    new MySqlParameter("@coach", Convert.ToInt32(cmbCoach.SelectedValue)),
                    new MySqlParameter("@team", Convert.ToInt32(cmbTeam.SelectedValue)),
                    new MySqlParameter("@date", dtSessionDate.Value.Date),
                    new MySqlParameter("@title", txtTitle.Text.Trim()),
                    new MySqlParameter("@focus", cmbFocus.Text),
                    new MySqlParameter("@start", tmStart.Value.ToString("HH:mm:ss")),
                    new MySqlParameter("@end", tmEnd.Value.ToString("HH:mm:ss")),
                    new MySqlParameter("@location", txtLocation.Text.Trim()),
                    new MySqlParameter("@notes", txtNotes.Text.Trim())
                );
            }
            else
            {
                DbHelper.ExecuteNonQuery(@"
                    UPDATE training_sessions
                    SET coach_id=@coach, team_id=@team, session_date=@date, title=@title,
                        focus_area=@focus, start_time=@start, end_time=@end, location=@location, notes=@notes
                    WHERE session_id=@id",
                    new MySqlParameter("@coach", Convert.ToInt32(cmbCoach.SelectedValue)),
                    new MySqlParameter("@team", Convert.ToInt32(cmbTeam.SelectedValue)),
                    new MySqlParameter("@date", dtSessionDate.Value.Date),
                    new MySqlParameter("@title", txtTitle.Text.Trim()),
                    new MySqlParameter("@focus", cmbFocus.Text),
                    new MySqlParameter("@start", tmStart.Value.ToString("HH:mm:ss")),
                    new MySqlParameter("@end", tmEnd.Value.ToString("HH:mm:ss")),
                    new MySqlParameter("@location", txtLocation.Text.Trim()),
                    new MySqlParameter("@notes", txtNotes.Text.Trim()),
                    new MySqlParameter("@id", editingId.Value)
                );
            }

            MessageBox.Show("Training session saved successfully.");
            ClearForm();
            LoadData();
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(TrainingForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void GridCellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

        var id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["session_id"].Value);
        var columnName = grid.Columns[e.ColumnIndex].Name;

        try
        {
            if (columnName == "Edit")
            {
                var row = DbHelper.ExecuteDataTable("SELECT * FROM training_sessions WHERE session_id=@id", new MySqlParameter("@id", id)).Rows[0];

                editingId = id;
                cmbCoach.SelectedValue = Convert.ToInt32(row["coach_id"]);
                cmbTeam.SelectedValue = Convert.ToInt32(row["team_id"]);
                dtSessionDate.Value = Convert.ToDateTime(row["session_date"]);
                txtTitle.Text = row["title"].ToString();
                cmbFocus.Text = row["focus_area"].ToString();
                tmStart.Value = DateTime.Today.Add(TimeSpan.Parse(row["start_time"].ToString()!));
                tmEnd.Value = DateTime.Today.Add(TimeSpan.Parse(row["end_time"].ToString()!));
                txtLocation.Text = row["location"].ToString();
                txtNotes.Text = row["notes"].ToString();
                lblTitle.Text = "Edit Training Session";
            }
            else if (columnName == "Delete")
            {
                var confirm = MessageBox.Show("Delete this training session?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    DbHelper.ExecuteNonQuery("DELETE FROM training_sessions WHERE session_id=@id", new MySqlParameter("@id", id));
                    LoadData();
                }
            }
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(TrainingForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void ClearForm()
    {
        editingId = null;
        if (cmbCoach.Items.Count > 0) cmbCoach.SelectedIndex = 0;
        if (cmbTeam.Items.Count > 0) cmbTeam.SelectedIndex = 0;
        dtSessionDate.Value = DateTime.Today;
        txtTitle.Clear();
        cmbFocus.SelectedIndex = 0;
        tmStart.Value = DateTime.Today.AddHours(16);
        tmEnd.Value = DateTime.Today.AddHours(18);
        txtLocation.Clear();
        txtNotes.Clear();
        lblTitle.Text = "Add Training Session";
    }
}