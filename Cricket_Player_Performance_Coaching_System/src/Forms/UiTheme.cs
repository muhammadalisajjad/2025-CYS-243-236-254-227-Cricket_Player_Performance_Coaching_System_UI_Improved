using System.Drawing;
using System.Windows.Forms;

namespace CricketPlayerManagementSystem.Forms;

public static class UiTheme
{
    public static readonly Color Primary = Color.FromArgb(20, 83, 45);
    public static readonly Color PrimaryDark = Color.FromArgb(5, 46, 22);
    public static readonly Color Accent = Color.FromArgb(34, 197, 94);
    public static readonly Color Background = Color.FromArgb(241, 245, 249);
    public static readonly Color Card = Color.White;
    public static readonly Color Text = Color.FromArgb(15, 23, 42);
    public static readonly Color MutedText = Color.FromArgb(100, 116, 139);
    public static readonly Color Border = Color.FromArgb(226, 232, 240);

    public static readonly Font TitleFont = new("Segoe UI", 20, FontStyle.Bold);
    public static readonly Font SectionFont = new("Segoe UI", 12, FontStyle.Bold);
    public static readonly Font NormalFont = new("Segoe UI", 10, FontStyle.Regular);
    public static readonly Font SmallFont = new("Segoe UI", 9, FontStyle.Regular);

    public static Panel CardPanel(int padding = 18)
    {
        return new Panel
        {
            BackColor = Card,
            Padding = new Padding(padding),
            Margin = new Padding(12)
        };
    }

    public static Button PrimaryButton(string text)
    {
        return StyledButton(text, Primary, Color.White);
    }

    public static Button SecondaryButton(string text)
    {
        return StyledButton(text, Color.FromArgb(226, 232, 240), Text);
    }

    public static Button DangerButton(string text)
    {
        return StyledButton(text, Color.FromArgb(220, 38, 38), Color.White);
    }

    public static Button SidebarButton(string text)
    {
        var button = StyledButton(text, PrimaryDark, Color.White);
        button.TextAlign = ContentAlignment.MiddleLeft;
        button.Padding = new Padding(18, 0, 0, 0);
        button.Height = 48;
        button.Dock = DockStyle.Top;
        button.Margin = new Padding(0, 0, 0, 8);
        return button;
    }

    private static Button StyledButton(string text, Color backColor, Color foreColor)
    {
        var button = new Button
        {
            Text = text,
            BackColor = backColor,
            ForeColor = foreColor,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Height = 38,
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    public static Label FieldLabel(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = false,
            Height = 22,
            Dock = DockStyle.Fill,
            Font = SmallFont,
            ForeColor = MutedText,
            TextAlign = ContentAlignment.MiddleLeft
        };
    }

    public static void StyleInput(Control input)
    {
        input.Font = NormalFont;
        input.BackColor = Color.White;
        input.ForeColor = Text;
        if (input is TextBox box)
        {
            box.BorderStyle = BorderStyle.FixedSingle;
        }
        if (input is ComboBox combo)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.FlatStyle = FlatStyle.Flat;
        }
        if (input is DateTimePicker picker)
        {
            picker.Format = DateTimePickerFormat.Short;
        }
        if (input is NumericUpDown num)
        {
            num.BorderStyle = BorderStyle.FixedSingle;
        }
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.BorderStyle = BorderStyle.None;
        grid.BackgroundColor = Card;
        grid.GridColor = Border;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        grid.ColumnHeadersHeight = 40;
        grid.RowHeadersVisible = false;
        grid.DefaultCellStyle.Font = NormalFont;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 252, 231);
        grid.DefaultCellStyle.SelectionForeColor = Text;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        grid.RowTemplate.Height = 34;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.AllowUserToAddRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
    }
}
