# Installation Guide

## Required Software

1. Visual Studio 2022 or newer
2. MySQL Server
3. MySQL Workbench
4. .NET Desktop Development workload in Visual Studio
5. NuGet package restore enabled

## Database Setup

1. Open MySQL Workbench.
2. Open `database/cricket_player_management_system.sql`.
3. Run the complete script.
4. Confirm that database `cricket_player_management` is created.
5. Check that tables, views, procedures, and triggers exist.

## C# Setup

1. Open `src/CricketPlayerManagementSystem.csproj` in Visual Studio.
2. Restore NuGet packages.
3. Open `src/Software/DbHelper.cs`.
4. Update this connection string if your MySQL password is different:

```csharp
server=localhost;user id=root;password=;database=cricket_player_management;
```

5. Build the project.
6. Run the project.

## Testing Steps

1. Login using admin account.
2. Add a coach.
3. Add a team.
4. Add a player.
5. Assign player to team.
6. Create training session.
7. Mark attendance.
8. Add fitness record.
9. Add match.
10. Save batting/bowling/fielding performance.
11. Generate PDF reports.
12. Check `error_logs` table if any error occurs.
