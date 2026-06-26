# Cricket Player Performance & Coaching Management System

A Database System LAB final project for managing cricket players, coaches, teams, training, fitness, injuries, match squads, and player performance reports.

## Main Technology

- Frontend: C# Windows Forms
- Backend language: C#
- Database: MySQL
- Reporting: Simple built-in PDF generator in C#
- Architecture: Domain classes + software/service classes + Windows Forms UI

## Project Modules

1. Login and role-based dashboard
2. Player profile add/edit management
3. Coach management
4. Team management
5. Training session management
6. Attendance management
7. Fitness record management
8. Injury record management
9. Match and squad management
10. Batting, bowling, and fielding performance management
11. Coach feedback management
12. PDF business reports
13. Error logging

## Demo Login

Run the SQL script first. Demo accounts are inserted by the script.

- Admin username: `admin`
- Password: `admin123`
- Coach username: `coach1`
- Password: `coach123`
- Player username: `player1`
- Password: `player123`

## How to Run

1. Install MySQL Server.
2. Open MySQL Workbench.
3. Run `database/cricket_player_management_system.sql`.
4. Open `src/CricketPlayerManagementSystem.csproj` in Visual Studio on Windows.
5. Install/restore NuGet package `MySql.Data`.
6. Open `src/Software/DbHelper.cs` and update MySQL password if needed.
7. Build and run the project.

## Repository Name

Use the repository naming format required by the lab guideline:

`DBS25F001`

Replace `001` with your real project ID from Eduko.
