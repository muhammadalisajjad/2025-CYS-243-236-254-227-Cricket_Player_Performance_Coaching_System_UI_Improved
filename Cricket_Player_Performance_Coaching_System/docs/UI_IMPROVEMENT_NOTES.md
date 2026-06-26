# UI Improvement Notes

The WinForms UI has been redesigned with a cleaner modern layout while keeping the same C# + MySQL project structure.

## Updated files

- `src/Forms/UiTheme.cs` - new shared theme class for colors, fonts, buttons, inputs, and tables.
- `src/Forms/BaseForm.cs` - improved base window, menu strip, header, grid styling.
- `src/Forms/LoginForm.cs` - redesigned login screen with a modern left branding area and right login card.
- `src/Forms/DashboardForm.cs` - redesigned dashboard with dark green sidebar, module buttons, welcome header, and statistics cards.
- `src/Forms/PlayerForm.cs` - redesigned add/edit form using cards, responsive split layout, styled inputs, and styled player table.
- `src/Forms/PerformanceForm.cs` - redesigned match performance form with grouped batting, bowling, and fielding sections.
- `src/Forms/ReportsForm.cs` - redesigned reports screen with parameter panel, browse button, preview button, and styled report preview table.
- `src/Forms/SimpleTableForm.cs` - improved common table viewer with modern card layout and refresh button.

## UI requirements still covered

- Text boxes
- Password fields
- Radio buttons
- Check boxes
- Dropdowns
- Date selector fields
- Text areas
- Scroll bars
- Tables
- Buttons inside table
- Panels
- File menu
- Same form for add/edit
- Responsive UI using docking, split containers, panels, and table layouts

## How to run

1. Run `database/cricket_player_management_system.sql` in MySQL Workbench.
2. Update MySQL password in `src/Software/DbHelper.cs`.
3. Open `src` folder in VS Code.
4. Run:

```bash
dotnet restore
dotnet build
dotnet run
```

Demo login:

- username: `admin`
- password: `admin123`
