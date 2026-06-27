using System;
using System.Drawing;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;
using MySql.Data.MySqlClient;

namespace CricketPlayerManagementSystem.Forms;

public class FitnessForm : BaseForm
{
    private int? editingId;

    private readonly ComboBox cmbPlayer = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox cmbCoach = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker dtRecordDate = new();
    private readonly NumericUpDown numHeight = new() { DecimalPlaces = 2, Minimum = 100, Maximum = 230, Increment = 0.5M };
    private readonly NumericUpDown numWeight = new() { DecimalPlaces = 2, Minimum = 30, Maximum = 160, Increment = 0.5M };
    private readonly NumericUpDown numStamina = new() { Minimum = 0, Maximum = 100 };
    private readonly NumericUpDown numSpeed = new() { Minimum = 0, Maximum = 100 };
    private readonly NumericUpDown numStrength = new() { Minimum = 0, Maximum = 100 };
    private readonly TextBox txtRemarks = new() { Multiline = true, ScrollBars = ScrollBars.Vertical };
    private readonly DataGridView grid = new() { Dock = DockStyle.Fill };
    private readonly Label lblTitle = new();

    public FitnessForm()
    {
        Text = "Fitness Records";
        WindowState = FormWindowState.Maximized;
        BuildUi();
        Load += (_, _) =>
        {
            LoadPlayers();
            LoadCoaches();
            LoadData();
        };
    }

    private void BuildUi()
    {
        dtRecordDate.Format = DateTimePickerFormat.Short;

        foreach (Control c in new Control[] { cmbPlayer, cmbCoach, dtRecordDate, numHeight, numWeight, numStamina, numSpeed, numStrength, txtRemarks })
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

        var header = CreateHeader("Fitness Records", "Manage player fitness measurements and scores.");
        root.Controls.Add(header, 0, 0);
        root.SetColumnSpan(header, 2);

        var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 12, 0), AutoScroll = true, BackColor = UiTheme.Background };
        var right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 8, 0, 0), BackColor = UiTheme.Background };
        root.Controls.Add(left, 0, 1);
        root.Controls.Add(right, 1, 1);

        var formCard = UiTheme.CardPanel(18);
        formCard.Dock = DockStyle.Top;
        formCard.AutoSize = true;

        lblTitle.Text = "Add Fitness Record";
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

        AddField(fields, "Player", cmbPlayer);
        AddField(fields, "Coach", cmbCoach);
        AddField(fields, "Record Date", dtRecordDate);
        AddField(fields, "Height (cm)", numHeight);
        AddField(fields, "Weight (kg)", numWeight);
        AddField(fields, "Stamina", numStamina);
        AddField(fields, "Speed", numSpeed);
        AddField(fields, "Strength", numStrength);
        AddField(fields, "Remarks", txtRemarks, 100);

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
            Text = "Fitness Record List",
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

    private void LoadPlayers()
    {
        var dt = DbHelper.ExecuteDataTable(@"
            SELECT p.player_id, u.full_name
            FROM players p
            JOIN users u ON p.user_id = u.user_id
            ORDER BY u.full_name
        ");
        cmbPlayer.DisplayMember = "full_name";
        cmbPlayer.ValueMember = "player_id";
        cmbPlayer.DataSource = dt;
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

    private void LoadData()
    {
        try
        {
            grid.Columns.Clear();
            grid.DataSource = DbHelper.ExecuteDataTable(@"
                SELECT f.fitness_id,
                       up.full_name AS player_name,
                       uc.full_name AS coach_name,
                       f.record_date,
                       f.height_cm,
                       f.weight_kg,
                       f.bmi,
                       f.stamina_score,
                       f.speed_score,
                       f.strength_score,
                       f.overall_score
                FROM fitness_records f
                JOIN players p ON f.player_id = p.player_id
                JOIN users up ON p.user_id = up.user_id
                JOIN coaches c ON f.coach_id = c.coach_id
                JOIN users uc ON c.user_id = uc.user_id
                ORDER BY f.fitness_id DESC
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
            LoggerService.LogError(ex, nameof(FitnessForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            var height = numHeight.Value;
            var weight = numWeight.Value;
            var bmi = Math.Round((double)weight / Math.Pow((double)height / 100.0, 2), 2);
            var overall = (int)Math.Round((numStamina.Value + numSpeed.Value + numStrength.Value) / 3M);

            if (editingId == null)
            {
                DbHelper.ExecuteNonQuery(@"
                    INSERT INTO fitness_records
                    (player_id, coach_id, record_date, height_cm, weight_kg, bmi, stamina_score, speed_score, strength_score, overall_score, remarks)
                    VALUES(@player, @coach, @date, @height, @weight, @bmi, @stamina, @speed, @strength, @overall, @remarks)",
                    new MySqlParameter("@player", Convert.ToInt32(cmbPlayer.SelectedValue)),
                    new MySqlParameter("@coach", Convert.ToInt32(cmbCoach.SelectedValue)),
                    new MySqlParameter("@date", dtRecordDate.Value.Date),
                    new MySqlParameter("@height", height),
                    new MySqlParameter("@weight", weight),
                    new MySqlParameter("@bmi", bmi),
                    new MySqlParameter("@stamina", (int)numStamina.Value),
                    new MySqlParameter("@speed", (int)numSpeed.Value),
                    new MySqlParameter("@strength", (int)numStrength.Value),
                    new MySqlParameter("@overall", overall),
                    new MySqlParameter("@remarks", txtRemarks.Text.Trim())
                );
            }
            else
            {
                DbHelper.ExecuteNonQuery(@"
                    UPDATE fitness_records
                    SET player_id=@player, coach_id=@coach, record_date=@date, height_cm=@height, weight_kg=@weight,
                        bmi=@bmi, stamina_score=@stamina, speed_score=@speed, strength_score=@strength,
                        overall_score=@overall, remarks=@remarks
                    WHERE fitness_id=@id",
                    new MySqlParameter("@player", Convert.ToInt32(cmbPlayer.SelectedValue)),
                    new MySqlParameter("@coach", Convert.ToInt32(cmbCoach.SelectedValue)),
                    new MySqlParameter("@date", dtRecordDate.Value.Date),
                    new MySqlParameter("@height", height),
                    new MySqlParameter("@weight", weight),
                    new MySqlParameter("@bmi", bmi),
                    new MySqlParameter("@stamina", (int)numStamina.Value),
                    new MySqlParameter("@speed", (int)numSpeed.Value),
                    new MySqlParameter("@strength", (int)numStrength.Value),
                    new MySqlParameter("@overall", overall),
                    new MySqlParameter("@remarks", txtRemarks.Text.Trim()),
                    new MySqlParameter("@id", editingId.Value)
                );
            }

            MessageBox.Show("Fitness record saved successfully.");
            ClearForm();
            LoadData();
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(FitnessForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void GridCellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

        var id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["fitness_id"].Value);
        var columnName = grid.Columns[e.ColumnIndex].Name;

        try
        {
            if (columnName == "Edit")
            {
                var row = DbHelper.ExecuteDataTable("SELECT * FROM fitness_records WHERE fitness_id=@id", new MySqlParameter("@id", id)).Rows[0];

                editingId = id;
                cmbPlayer.SelectedValue = Convert.ToInt32(row["player_id"]);
                cmbCoach.SelectedValue = Convert.ToInt32(row["coach_id"]);
                dtRecordDate.Value = Convert.ToDateTime(row["record_date"]);
                numHeight.Value = Convert.ToDecimal(row["height_cm"]);
                numWeight.Value = Convert.ToDecimal(row["weight_kg"]);
                numStamina.Value = Convert.ToDecimal(row["stamina_score"]);
                numSpeed.Value = Convert.ToDecimal(row["speed_score"]);
                numStrength.Value = Convert.ToDecimal(row["strength_score"]);
                txtRemarks.Text = row["remarks"].ToString();
                lblTitle.Text = "Edit Fitness Record";
            }
            else if (columnName == "Delete")
            {
                var confirm = MessageBox.Show("Delete this fitness record?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    DbHelper.ExecuteNonQuery("DELETE FROM fitness_records WHERE fitness_id=@id", new MySqlParameter("@id", id));
                    LoadData();
                }
            }
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(FitnessForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void ClearForm()
    {
        editingId = null;
        if (cmbPlayer.Items.Count > 0) cmbPlayer.SelectedIndex = 0;
        if (cmbCoach.Items.Count > 0) cmbCoach.SelectedIndex = 0;
        dtRecordDate.Value = DateTime.Today;
        numHeight.Value = 170;
        numWeight.Value = 65;
        numStamina.Value = 50;
        numSpeed.Value = 50;
        numStrength.Value = 50;
        txtRemarks.Clear();
        lblTitle.Text = "Add Fitness Record";
    }
}