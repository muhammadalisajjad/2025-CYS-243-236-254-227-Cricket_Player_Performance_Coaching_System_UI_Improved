# Requirements Checklist

| Guideline Requirement | Fulfilled In This Project |
|---|---|
| Group project | Designed for 2-3 members |
| GitHub repository | Use name `DBS25F001`, replace ID with Eduko project ID |
| Save data in database | All modules save data in MySQL |
| Minimum 8 domain classes | User, Role, Player, Coach, Team, Match, TrainingSession, FitnessRecord, InjuryRecord, Performance classes |
| Minimum 5 software classes | DbHelper, Validator, LoggerService, SessionManager, AuthService, TransactionService, ReportGenerator |
| ERD minimum 10 entities | 21 entities included |
| Minimum 15 database tables | 21 tables included |
| Validators | `Validator.cs` validates required fields, email, phone, dates, numbers |
| Handle exceptions | Try/catch blocks in forms and services |
| UI controls | Text boxes, password fields, radio buttons, checkboxes, dropdowns, date selectors, text areas, scrollbars, tables, table buttons, panels, menu strip |
| At least 3 forms with panels | DashboardForm, PlayerForm, ReportsForm use panels |
| File menu on each screen | `BaseForm.cs` adds File menu to every screen |
| Same form for add and edit | `PlayerForm.cs` supports Add and Edit through same form |
| Error logging | `LoggerService.cs` and `error_logs` table |
| Transactions at least 3 | Add player, save match performance, assign player to team |
| Views at least 5 | 7 views included |
| Stored procedures minimum 3 | 4 stored procedures included |
| Triggers at least 2 | 4 triggers included |
| Constraints at least 10 | PK, FK, UNIQUE, NOT NULL, CHECK constraints included |
| PDF reports minimum 10 | 10 business reports available from ReportsForm |
| Responsive UI | Dock, Anchor, TableLayoutPanel, SplitContainer, AutoScroll used |
| Parameter based reports | Player, team, date range, match, and coach parameters supported |
