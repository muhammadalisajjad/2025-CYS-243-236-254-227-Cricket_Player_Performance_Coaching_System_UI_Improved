# PDF Report Specifications

The Reports screen supports the following business level reports. Each report can be generated as PDF.

| # | Report Name | Parameters | Source |
|---|---|---|---|
| 1 | Player Profile Report | Player ID | `v_player_profile` |
| 2 | Team Player List Report | Team ID | `v_team_players` |
| 3 | Coach Assigned Players Report | Coach ID | `coach_team`, `player_team`, `v_player_profile` |
| 4 | Fitness Progress Report | Player ID, Date From, Date To | `fitness_records` |
| 5 | Injury Status Report | Player ID or Status | `injury_records` |
| 6 | Training Attendance Report | Team ID, Date From, Date To | `training_attendance`, `training_sessions` |
| 7 | Match Squad Report | Match ID | `match_squad` |
| 8 | Batting Performance Report | Match ID or Player ID | `batting_performance` |
| 9 | Bowling Performance Report | Match ID or Player ID | `bowling_performance` |
| 10 | Overall Player Ranking Report | Date From, Date To | `v_top_batsmen`, `v_top_bowlers`, `player_career_stats` |

## Report Generation Flow

1. User selects report type from dropdown.
2. User enters parameters.
3. System validates parameters.
4. System queries database using views/stored procedures.
5. System writes PDF file.
6. System inserts audit entry in `report_audit` table.
7. If any error occurs, system records it in `error_logs`.
