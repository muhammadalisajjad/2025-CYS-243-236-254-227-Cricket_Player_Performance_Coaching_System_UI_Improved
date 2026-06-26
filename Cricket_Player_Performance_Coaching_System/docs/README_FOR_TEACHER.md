# Teacher Review Notes

Project Title: Cricket Player Performance & Coaching Management System

This project is designed according to the DB Lab final project requirements. It is not only a basic CRUD project. It includes cricket-specific business modules such as player management, coach assignment, team management, training attendance, fitness tracking, injury tracking, match squad selection, and performance reporting.

## Why This Project Has Database Depth

- Many-to-many relationships are used through `player_team`, `coach_team`, and `match_squad`.
- Transactions are used where multi-table changes must be saved together.
- Views are used for business reports and dashboards.
- Stored procedures are used for important workflows.
- Triggers update career statistics automatically.
- Constraints protect data consistency.
- Error logs preserve system-level problems.

## Main Viva Points

1. Explain role-based users.
2. Explain player-team many-to-many relationship.
3. Explain coach-team relationship.
4. Explain why match performance is split into batting, bowling, and fielding tables.
5. Explain transaction examples.
6. Explain career stats trigger.
7. Explain parameter-based PDF reports.
8. Explain validators and exception logging.
