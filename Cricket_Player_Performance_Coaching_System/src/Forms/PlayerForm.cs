using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Software;
using MySql.Data.MySqlClient;

namespace CricketPlayerManagementSystem.Forms;

public class PlayerForm : BaseForm
{
    private int? editingPlayerId;

    private readonly TextBox txtUsername = new();
    private readonly TextBox txtPassword = new();
    private readonly TextBox txtFullName = new();
    private readonly TextBox txtEmail = new();
    private readonly TextBox txtPhone = new();
    private readonly TextBox txtRegNo = new();
    private readonly DateTimePicker dtDob = new();
    private readonly DateTimePicker dtJoin = new();
    private readonly RadioButton rbMale = new() { Text = "Male", Checked = true };
    private readonly RadioButton rbFemale = new() { Text = "Female" };
    private readonly ComboBox cmbRole = new();
    private readonly ComboBox cmbBat = new();
    private readonly ComboBox cmbBowl = new();
    private readonly ComboBox cmbHand = new();
    private readonly ComboBox cmbTeam = new();
    private readonly CheckBox chkActive = new() { Text = "Active", Checked = true };
    private readonly TextBox txtNotes = new() { Multiline = true, ScrollBars = ScrollBars.Vertical };
    private readonly DataGridView grid = new() { Dock = DockStyle.Fill };
    private readonly Label formTitle = new();

    public PlayerForm()
    {
        Text = "Player Management";
        WindowState = FormWindowState.Maximized;
        BuildUi();

        Load += (_, _) =>
        {
            LoadTeams();
            LoadPlayers();
        };
    }

    private void BuildUi()
    {
        BackColor = UiTheme.Background;

        cmbRole.Items.AddRange(new object[] { "Batsman", "Bowler", "All Rounder", "Wicket Keeper" });
        cmbBat.Items.AddRange(new object[] { "Right Hand", "Left Hand" });
        cmbBowl.Items.AddRange(new object[] { "Right Arm Fast", "Left Arm Fast", "Right Arm Spin", "Left Arm Spin", "None" });
        cmbHand.Items.AddRange(new object[] { "Right", "Left" });

        cmbRole.SelectedIndex = 0;
        cmbBat.SelectedIndex = 0;
        cmbBowl.SelectedIndex = 0;
        cmbHand.SelectedIndex = 0;

        txtPassword.UseSystemPasswordChar = true;
        dtDob.Format = DateTimePickerFormat.Short;
        dtJoin.Format = DateTimePickerFormat.Short;
        txtNotes.Height = 90;

        foreach (Control c in new Control[]
        {
            txtUsername, txtPassword, txtFullName, txtEmail, txtPhone, txtRegNo,
            dtDob, dtJoin, cmbRole, cmbBat, cmbBowl, cmbHand, cmbTeam, txtNotes
        })
        {
            UiTheme.StyleInput(c);
            c.Margin = new Padding(0);
        }

        rbMale.Font = UiTheme.NormalFont;
        rbFemale.Font = UiTheme.NormalFont;
        chkActive.Font = UiTheme.NormalFont;
        chkActive.AutoSize = true;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Background,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(18)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 470F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var header = CreateHeader(
            "Player Management",
            "Add, edit and view cricket player records from the same form."
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

        formTitle.Text = "Add New Player";
        formTitle.Dock = DockStyle.Top;
        formTitle.Height = 34;
        formTitle.Font = UiTheme.SectionFont;
        formTitle.ForeColor = UiTheme.Text;

        var formGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2,
            Padding = new Padding(0, 10, 0, 0)
        };
        formGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
        formGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        AddField(formGrid, "Username", txtUsername, 46);
        AddField(formGrid, "Password", txtPassword, 46);
        AddField(formGrid, "Full Name", txtFullName, 46);
        AddField(formGrid, "Email", txtEmail, 46);
        AddField(formGrid, "Phone", txtPhone, 46);
        AddField(formGrid, "Registration No", txtRegNo, 46);
        AddField(formGrid, "Date of Birth", dtDob, 46);
        AddGenderField(formGrid);
        AddField(formGrid, "Player Role", cmbRole, 46);
        AddField(formGrid, "Batting Style", cmbBat, 46);
        AddField(formGrid, "Bowling Style", cmbBowl, 46);
        AddField(formGrid, "Dominant Hand", cmbHand, 46);
        AddField(formGrid, "Joining Date", dtJoin, 46);
        AddField(formGrid, "Team", cmbTeam, 46);
        AddCheckField(formGrid);
        AddField(formGrid, "Notes", txtNotes, 100);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(145, 12, 0, 0),
            WrapContents = false
        };

        var btnSave = UiTheme.PrimaryButton("Save Player");
        var btnClear = UiTheme.SecondaryButton("Clear");

        btnSave.Width = 130;
        btnClear.Width = 90;

        btnSave.Click += SaveClicked;
        btnClear.Click += (_, _) => ClearForm();

        buttonPanel.Controls.Add(btnSave);
        buttonPanel.Controls.Add(btnClear);

        formCard.Controls.Add(buttonPanel);
        formCard.Controls.Add(formGrid);
        formCard.Controls.Add(formTitle);

        leftHost.Controls.Add(formCard);

        var tableCard = UiTheme.CardPanel(16);
        tableCard.Dock = DockStyle.Fill;

        var tableTitle = new Label
        {
            Text = "Player List",
            Dock = DockStyle.Top,
            Height = 34,
            Font = UiTheme.SectionFont,
            ForeColor = UiTheme.Text
        };

        UiTheme.StyleGrid(grid);
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;

        if (!grid.Columns.Contains("Edit"))
        {
            var btnCol = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "Action",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Width = 90
            };
            grid.Columns.Add(btnCol);
        }

        grid.CellContentClick -= GridCellContentClick;
        grid.CellContentClick += GridCellContentClick;

        tableCard.Controls.Add(grid);
        tableCard.Controls.Add(tableTitle);

        rightHost.Controls.Add(tableCard);

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

    private void AddGenderField(TableLayoutPanel table)
    {
        int row = table.RowCount;
        table.RowCount += 1;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        var label = UiTheme.FieldLabel("Gender");
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.Margin = new Padding(0, 0, 10, 10);

        var genderPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 10)
        };

        genderPanel.Controls.Add(rbMale);
        genderPanel.Controls.Add(rbFemale);

        table.Controls.Add(label, 0, row);
        table.Controls.Add(genderPanel, 1, row);
    }

    private void AddCheckField(TableLayoutPanel table)
    {
        int row = table.RowCount;
        table.RowCount += 1;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

        var label = UiTheme.FieldLabel("Status");
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.Margin = new Padding(0, 0, 10, 10);

        var holder = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 10)
        };
        chkActive.Location = new Point(0, 8);
        holder.Controls.Add(chkActive);

        table.Controls.Add(label, 0, row);
        table.Controls.Add(holder, 1, row);
    }

    private void LoadTeams()
    {
        try
        {
            DataTable table = DbHelper.ExecuteDataTable(
                "SELECT team_id, team_name FROM teams ORDER BY team_name"
            );

            cmbTeam.DisplayMember = "team_name";
            cmbTeam.ValueMember = "team_id";
            cmbTeam.DataSource = table;
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(PlayerForm));
            MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadPlayers()
    {
        try
        {
            grid.DataSource = DbHelper.ExecuteDataTable(@"
                SELECT player_id, full_name, registration_no, player_role, team_name, status
                FROM v_player_profile
            ");
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(PlayerForm));
            MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveClicked(object? sender, EventArgs e)
    {
        try
        {
            Validator.Require(Validator.Required(txtUsername.Text), "Username is required.");
            Validator.Require(Validator.Required(txtFullName.Text), "Full name is required.");
            Validator.Require(Validator.Email(txtEmail.Text), "Valid email is required.");
            Validator.Require(Validator.Phone(txtPhone.Text), "Valid phone is required.");
            Validator.Require(Validator.Required(txtRegNo.Text), "Registration number is required.");

            string gender = rbMale.Checked ? "Male" : "Female";

            if (editingPlayerId == null)
            {
                TransactionService.AddPlayerWithUserAndTeam(
                    txtUsername.Text.Trim(),
                    txtPassword.Text,
                    txtFullName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtPhone.Text.Trim(),
                    txtRegNo.Text.Trim(),
                    dtDob.Value.Date,
                    gender,
                    cmbBat.Text,
                    cmbBowl.Text,
                    cmbRole.Text,
                    cmbHand.Text,
                    dtJoin.Value.Date,
                    Convert.ToInt32(cmbTeam.SelectedValue)
                );
            }
            else
            {
                DbHelper.ExecuteNonQuery(@"
                    UPDATE players p
                    JOIN users u ON p.user_id = u.user_id
                    SET u.full_name = @name,
                        u.email = @email,
                        u.phone = @phone,
                        p.gender = @gender,
                        p.batting_style = @bat,
                        p.bowling_style = @bowl,
                        p.player_role = @role,
                        p.dominant_hand = @hand,
                        p.status = @status
                    WHERE p.player_id = @id",
                    new MySqlParameter("@name", txtFullName.Text.Trim()),
                    new MySqlParameter("@email", txtEmail.Text.Trim()),
                    new MySqlParameter("@phone", txtPhone.Text.Trim()),
                    new MySqlParameter("@gender", gender),
                    new MySqlParameter("@bat", cmbBat.Text),
                    new MySqlParameter("@bowl", cmbBowl.Text),
                    new MySqlParameter("@role", cmbRole.Text),
                    new MySqlParameter("@hand", cmbHand.Text),
                    new MySqlParameter("@status", chkActive.Checked ? "Active" : "Dropped"),
                    new MySqlParameter("@id", editingPlayerId.Value)
                );
            }

            MessageBox.Show("Player saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadPlayers();
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(PlayerForm));
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void GridCellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;

        if (grid.Columns[e.ColumnIndex].Name != "Edit")
            return;

        try
        {
            editingPlayerId = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["player_id"].Value);

            var table = DbHelper.ExecuteDataTable(
                "SELECT * FROM v_player_profile WHERE player_id = @id",
                new MySqlParameter("@id", editingPlayerId.Value)
            );

            if (table.Rows.Count == 0)
                return;

            var row = table.Rows[0];

            txtFullName.Text = row["full_name"].ToString();
            txtEmail.Text = row["email"].ToString();
            txtPhone.Text = row["phone"].ToString();
            txtRegNo.Text = row["registration_no"].ToString();
            cmbRole.Text = row["player_role"].ToString();
            cmbBat.Text = row["batting_style"].ToString();
            cmbBowl.Text = row["bowling_style"].ToString();
            chkActive.Checked = row["status"].ToString() == "Active";

            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            formTitle.Text = "Edit Player";
        }
        catch (Exception ex)
        {
            LoggerService.LogError(ex, nameof(PlayerForm));
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ClearForm()
    {
        editingPlayerId = null;

        txtUsername.Enabled = true;
        txtPassword.Enabled = true;

        txtUsername.Clear();
        txtPassword.Clear();
        txtFullName.Clear();
        txtEmail.Clear();
        txtPhone.Clear();
        txtRegNo.Clear();
        txtNotes.Clear();

        rbMale.Checked = true;
        chkActive.Checked = true;

        if (cmbRole.Items.Count > 0) cmbRole.SelectedIndex = 0;
        if (cmbBat.Items.Count > 0) cmbBat.SelectedIndex = 0;
        if (cmbBowl.Items.Count > 0) cmbBowl.SelectedIndex = 0;
        if (cmbHand.Items.Count > 0) cmbHand.SelectedIndex = 0;

        formTitle.Text = "Add New Player";
    }
}