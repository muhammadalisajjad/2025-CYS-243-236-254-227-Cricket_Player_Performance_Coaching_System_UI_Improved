using System;
using System.Drawing;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;
using MySql.Data.MySqlClient;

namespace CricketPlayerManagementSystem.Forms;

public class TeamForm : BaseForm
{
    private int? editingId;

    private readonly TextBox txtTeamName = new();
    private readonly ComboBox cmbCategory = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox txtCity = new();
    private readonly DataGridView grid = new() { Dock = DockStyle.Fill };
    private readonly Label lblTitle = new();

    public TeamForm()
    {
        Text = "Teams";
        WindowState = FormWindowState.Maximized;
        BuildUi();
        Load += (_, _) => LoadData();
    }

    private void BuildUi()
    {
        cmbCategory.Items.AddRange(new object[]
        {
            "Under 16", "Under 19", "University", "Club", "District", "National"
        });
        cmbCategory.SelectedIndex = 0;

        UiTheme.StyleInput(txtTeamName);
        UiTheme.StyleInput(cmbCategory);
        UiTheme.StyleInput(txtCity);
        UiTheme.StyleGrid(grid);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            BackColor = UiTheme.Background,
            Padding = new Padding(18)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = CreateHeader("Teams", "Add, edit and delete team records.");
        root.Controls.Add(header, 0, 0);
        root.SetColumnSpan(header, 2);

        var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 12, 0), AutoScroll = true, BackColor = UiTheme.Background };
        var right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 8, 0, 0), BackColor = UiTheme.Background };

        root.Controls.Add(left, 0, 1);
        root.Controls.Add(right, 1, 1);

        var formCard = UiTheme.CardPanel(18);
        formCard.Dock = DockStyle.Top;
        formCard.AutoSize = true;

        lblTitle.Text = "Add Team";
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
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddField(fields, "Team Name", txtTeamName);
        AddField(fields, "Category", cmbCategory);
        AddField(fields, "City", txtCity);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 54,
            Padding = new Padding(120, 12, 0, 0)
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
            Text = "Team List",
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

    private void LoadData()
    {
        try
        {
            grid.Columns.Clear();
            grid.DataSource = DbHelper.ExecuteDataTable(@"
                SELECT team_id, team_name, category, city, created_at
                FROM teams
                ORDER BY team_id DESC
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
            LoggerService.LogError(ex, nameof(TeamForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            Validator.Require(Validator.Required(txtTeamName.Text), "Team name is required.");
            Validator.Require(Validator.Required(txtCity.Text), "City is required.");

            if (editingId == null)
            {
                DbHelper.ExecuteNonQuery(@"
                    INSERT INTO teams(team_name, category, city)
                    VALUES(@name, @category, @city)",
                    new MySqlParameter("@name", txtTeamName.Text.Trim()),
                    new MySqlParameter("@category", cmbCategory.Text),
                    new MySqlParameter("@city", txtCity.Text.Trim())
                );
            }
            else
            {
                DbHelper.ExecuteNonQuery(@"
                    UPDATE teams
                    SET team_name=@name, category=@category, city=@city
                    WHERE team_id=@id",
                    new MySqlParameter("@name", txtTeamName.Text.Trim()),
                    new MySqlParameter("@category", cmbCategory.Text),
                    new MySqlParameter("@city", txtCity.Text.Trim()),
                    new MySqlParameter("@id", editingId.Value)
                );
            }

            MessageBox.Show("Team saved successfully.");
            ClearForm();
            LoadData();
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(TeamForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void GridCellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

        var id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["team_id"].Value);
        var columnName = grid.Columns[e.ColumnIndex].Name;

        try
        {
            if (columnName == "Edit")
            {
                var row = DbHelper.ExecuteDataTable("SELECT * FROM teams WHERE team_id=@id", new MySqlParameter("@id", id)).Rows[0];
                editingId = id;
                txtTeamName.Text = row["team_name"].ToString();
                cmbCategory.Text = row["category"].ToString();
                txtCity.Text = row["city"].ToString();
                lblTitle.Text = "Edit Team";
            }
            else if (columnName == "Delete")
            {
                var confirm = MessageBox.Show("Delete this team?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    DbHelper.ExecuteNonQuery("DELETE FROM teams WHERE team_id=@id", new MySqlParameter("@id", id));
                    LoadData();
                }
            }
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(TeamForm));
            MessageBox.Show(ex.Message);
        }
    }

    private void ClearForm()
    {
        editingId = null;
        txtTeamName.Clear();
        txtCity.Clear();
        cmbCategory.SelectedIndex = 0;
        lblTitle.Text = "Add Team";
    }
}