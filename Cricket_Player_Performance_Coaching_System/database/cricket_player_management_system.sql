-- =========================================================
-- Cricket Player Performance & Coaching Management System
-- MySQL Database Script
-- =========================================================

DROP DATABASE IF EXISTS cricket_player_management;
CREATE DATABASE cricket_player_management;
USE cricket_player_management;

-- -----------------------------
-- Master Tables
-- -----------------------------
CREATE TABLE roles (
    role_id INT AUTO_INCREMENT PRIMARY KEY,
    role_name VARCHAR(30) NOT NULL UNIQUE
);

CREATE TABLE users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    role_id INT NOT NULL,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(128) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    phone VARCHAR(20) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_users_roles FOREIGN KEY (role_id) REFERENCES roles(role_id),
    CONSTRAINT chk_users_email CHECK (email LIKE '%@%'),
    CONSTRAINT chk_users_phone CHECK (CHAR_LENGTH(phone) >= 10)
);

CREATE TABLE coaches (
    coach_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    specialization VARCHAR(60) NOT NULL,
    experience_years INT NOT NULL DEFAULT 0,
    joined_date DATE NOT NULL,
    CONSTRAINT fk_coaches_users FOREIGN KEY (user_id) REFERENCES users(user_id),
    CONSTRAINT chk_coach_experience CHECK (experience_years >= 0)
);

CREATE TABLE players (
    player_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    registration_no VARCHAR(30) NOT NULL UNIQUE,
    dob DATE NOT NULL,
    gender ENUM('Male','Female') NOT NULL,
    batting_style ENUM('Right Hand','Left Hand') NOT NULL,
    bowling_style ENUM('Right Arm Fast','Left Arm Fast','Right Arm Spin','Left Arm Spin','None') NOT NULL,
    player_role ENUM('Batsman','Bowler','All Rounder','Wicket Keeper') NOT NULL,
    dominant_hand ENUM('Right','Left') NOT NULL,
    photo_path VARCHAR(255),
    joining_date DATE NOT NULL,
    status ENUM('Active','Injured','Dropped','Retired') NOT NULL DEFAULT 'Active',
    CONSTRAINT fk_players_users FOREIGN KEY (user_id) REFERENCES users(user_id),
    CONSTRAINT chk_player_joining CHECK (joining_date >= dob)
);

CREATE TABLE teams (
    team_id INT AUTO_INCREMENT PRIMARY KEY,
    team_name VARCHAR(100) NOT NULL UNIQUE,
    category ENUM('Under 16','Under 19','University','Club','District','National') NOT NULL,
    city VARCHAR(60) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE coach_team (
    coach_team_id INT AUTO_INCREMENT PRIMARY KEY,
    coach_id INT NOT NULL,
    team_id INT NOT NULL,
    assigned_date DATE NOT NULL,
    is_current BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT fk_ct_coach FOREIGN KEY (coach_id) REFERENCES coaches(coach_id),
    CONSTRAINT fk_ct_team FOREIGN KEY (team_id) REFERENCES teams(team_id),
    CONSTRAINT uq_coach_team UNIQUE (coach_id, team_id, assigned_date)
);

CREATE TABLE player_team (
    player_team_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    team_id INT NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE,
    is_current BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT fk_pt_player FOREIGN KEY (player_id) REFERENCES players(player_id),
    CONSTRAINT fk_pt_team FOREIGN KEY (team_id) REFERENCES teams(team_id),
    CONSTRAINT uq_player_team UNIQUE (player_id, team_id, start_date),
    CONSTRAINT chk_team_dates CHECK (end_date IS NULL OR end_date >= start_date)
);

CREATE TABLE tournaments (
    tournament_id INT AUTO_INCREMENT PRIMARY KEY,
    tournament_name VARCHAR(120) NOT NULL UNIQUE,
    location VARCHAR(100) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    CONSTRAINT chk_tournament_dates CHECK (end_date >= start_date)
);

CREATE TABLE matches (
    match_id INT AUTO_INCREMENT PRIMARY KEY,
    tournament_id INT,
    team_id INT NOT NULL,
    opponent_name VARCHAR(100) NOT NULL,
    match_date DATE NOT NULL,
    venue VARCHAR(120) NOT NULL,
    match_type ENUM('T20','ODI','Test','Practice') NOT NULL,
    result ENUM('Won','Lost','Tie','No Result','Upcoming') NOT NULL DEFAULT 'Upcoming',
    CONSTRAINT fk_matches_tournament FOREIGN KEY (tournament_id) REFERENCES tournaments(tournament_id),
    CONSTRAINT fk_matches_team FOREIGN KEY (team_id) REFERENCES teams(team_id)
);

CREATE TABLE match_squad (
    squad_id INT AUTO_INCREMENT PRIMARY KEY,
    match_id INT NOT NULL,
    player_id INT NOT NULL,
    selected_by_coach_id INT NOT NULL,
    selection_status ENUM('Selected','Benched','Rejected') NOT NULL DEFAULT 'Selected',
    batting_order INT,
    CONSTRAINT fk_squad_match FOREIGN KEY (match_id) REFERENCES matches(match_id),
    CONSTRAINT fk_squad_player FOREIGN KEY (player_id) REFERENCES players(player_id),
    CONSTRAINT fk_squad_coach FOREIGN KEY (selected_by_coach_id) REFERENCES coaches(coach_id),
    CONSTRAINT uq_match_player UNIQUE (match_id, player_id),
    CONSTRAINT chk_batting_order CHECK (batting_order IS NULL OR batting_order BETWEEN 1 AND 11)
);

CREATE TABLE training_sessions (
    session_id INT AUTO_INCREMENT PRIMARY KEY,
    coach_id INT NOT NULL,
    team_id INT NOT NULL,
    session_date DATE NOT NULL,
    title VARCHAR(100) NOT NULL,
    focus_area ENUM('Batting','Bowling','Fielding','Fitness','Strategy') NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    location VARCHAR(100) NOT NULL,
    notes TEXT,
    CONSTRAINT fk_ts_coach FOREIGN KEY (coach_id) REFERENCES coaches(coach_id),
    CONSTRAINT fk_ts_team FOREIGN KEY (team_id) REFERENCES teams(team_id),
    CONSTRAINT chk_session_time CHECK (end_time > start_time)
);

CREATE TABLE training_attendance (
    attendance_id INT AUTO_INCREMENT PRIMARY KEY,
    session_id INT NOT NULL,
    player_id INT NOT NULL,
    status ENUM('Present','Absent','Late','Excused') NOT NULL,
    remarks VARCHAR(255),
    CONSTRAINT fk_att_session FOREIGN KEY (session_id) REFERENCES training_sessions(session_id),
    CONSTRAINT fk_att_player FOREIGN KEY (player_id) REFERENCES players(player_id),
    CONSTRAINT uq_attendance UNIQUE (session_id, player_id)
);

CREATE TABLE fitness_records (
    fitness_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    coach_id INT NOT NULL,
    record_date DATE NOT NULL,
    height_cm DECIMAL(5,2) NOT NULL,
    weight_kg DECIMAL(5,2) NOT NULL,
    bmi DECIMAL(5,2),
    stamina_score INT NOT NULL,
    speed_score INT NOT NULL,
    strength_score INT NOT NULL,
    overall_score INT,
    remarks TEXT,
    CONSTRAINT fk_fit_player FOREIGN KEY (player_id) REFERENCES players(player_id),
    CONSTRAINT fk_fit_coach FOREIGN KEY (coach_id) REFERENCES coaches(coach_id),
    CONSTRAINT chk_height CHECK (height_cm BETWEEN 100 AND 230),
    CONSTRAINT chk_weight CHECK (weight_kg BETWEEN 30 AND 160),
    CONSTRAINT chk_stamina CHECK (stamina_score BETWEEN 0 AND 100),
    CONSTRAINT chk_speed CHECK (speed_score BETWEEN 0 AND 100),
    CONSTRAINT chk_strength CHECK (strength_score BETWEEN 0 AND 100)
);

CREATE TABLE injury_records (
    injury_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL,
    injury_type VARCHAR(100) NOT NULL,
    description TEXT NOT NULL,
    injury_date DATE NOT NULL,
    expected_recovery_date DATE,
    status ENUM('Open','Recovering','Recovered') NOT NULL DEFAULT 'Open',
    doctor_note TEXT,
    CONSTRAINT fk_injury_player FOREIGN KEY (player_id) REFERENCES players(player_id),
    CONSTRAINT chk_recovery_date CHECK (expected_recovery_date IS NULL OR expected_recovery_date >= injury_date)
);

CREATE TABLE batting_performance (
    batting_id INT AUTO_INCREMENT PRIMARY KEY,
    match_id INT NOT NULL,
    player_id INT NOT NULL,
    runs INT NOT NULL DEFAULT 0,
    balls INT NOT NULL DEFAULT 0,
    fours INT NOT NULL DEFAULT 0,
    sixes INT NOT NULL DEFAULT 0,
    out_status ENUM('Out','Not Out','Did Not Bat') NOT NULL DEFAULT 'Did Not Bat',
    strike_rate DECIMAL(6,2),
    CONSTRAINT fk_bat_match FOREIGN KEY (match_id) REFERENCES matches(match_id),
    CONSTRAINT fk_bat_player FOREIGN KEY (player_id) REFERENCES players(player_id),
    CONSTRAINT uq_batting UNIQUE (match_id, player_id),
    CONSTRAINT chk_batting_runs CHECK (runs >= 0),
    CONSTRAINT chk_batting_balls CHECK (balls >= 0),
    CONSTRAINT chk_boundaries CHECK (fours >= 0 AND sixes >= 0)
);

CREATE TABLE bowling_performance (
    bowling_id INT AUTO_INCREMENT PRIMARY KEY,
    match_id INT NOT NULL,
    player_id INT NOT NULL,
    overs DECIMAL(4,1) NOT NULL DEFAULT 0,
    maidens INT NOT NULL DEFAULT 0,
    runs_given INT NOT NULL DEFAULT 0,
    wickets INT NOT NULL DEFAULT 0,
    economy DECIMAL(6,2),
    CONSTRAINT fk_bowl_match FOREIGN KEY (match_id) REFERENCES matches(match_id),
    CONSTRAINT fk_bowl_player FOREIGN KEY (player_id) REFERENCES players(player_id),
    CONSTRAINT uq_bowling UNIQUE (match_id, player_id),
    CONSTRAINT chk_bowling CHECK (overs >= 0 AND maidens >= 0 AND runs_given >= 0 AND wickets BETWEEN 0 AND 10)
);

CREATE TABLE fielding_performance (
    fielding_id INT AUTO_INCREMENT PRIMARY KEY,
    match_id INT NOT NULL,
    player_id INT NOT NULL,
    catches INT NOT NULL DEFAULT 0,
    runouts INT NOT NULL DEFAULT 0,
    stumpings INT NOT NULL DEFAULT 0,
    CONSTRAINT fk_field_match FOREIGN KEY (match_id) REFERENCES matches(match_id),
    CONSTRAINT fk_field_player FOREIGN KEY (player_id) REFERENCES players(player_id),
    CONSTRAINT uq_fielding UNIQUE (match_id, player_id),
    CONSTRAINT chk_fielding CHECK (catches >= 0 AND runouts >= 0 AND stumpings >= 0)
);

CREATE TABLE coach_feedback (
    feedback_id INT AUTO_INCREMENT PRIMARY KEY,
    coach_id INT NOT NULL,
    player_id INT NOT NULL,
    feedback_date DATE NOT NULL,
    rating INT NOT NULL,
    remarks TEXT NOT NULL,
    action_plan TEXT,
    CONSTRAINT fk_feedback_coach FOREIGN KEY (coach_id) REFERENCES coaches(coach_id),
    CONSTRAINT fk_feedback_player FOREIGN KEY (player_id) REFERENCES players(player_id),
    CONSTRAINT chk_feedback_rating CHECK (rating BETWEEN 1 AND 10)
);

CREATE TABLE player_career_stats (
    stats_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT NOT NULL UNIQUE,
    matches_played INT NOT NULL DEFAULT 0,
    total_runs INT NOT NULL DEFAULT 0,
    total_wickets INT NOT NULL DEFAULT 0,
    total_catches INT NOT NULL DEFAULT 0,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_stats_player FOREIGN KEY (player_id) REFERENCES players(player_id)
);

CREATE TABLE report_audit (
    report_id INT AUTO_INCREMENT PRIMARY KEY,
    report_name VARCHAR(100) NOT NULL,
    generated_by_user_id INT,
    generated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    parameters TEXT,
    CONSTRAINT fk_report_user FOREIGN KEY (generated_by_user_id) REFERENCES users(user_id)
);

CREATE TABLE error_logs (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    error_message TEXT NOT NULL,
    stack_trace TEXT,
    form_name VARCHAR(80),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_error_user FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- -----------------------------
-- Views
-- -----------------------------
CREATE VIEW v_player_profile AS
SELECT p.player_id, u.full_name, u.email, u.phone, p.registration_no, p.gender,
       p.batting_style, p.bowling_style, p.player_role, p.status,
       TIMESTAMPDIFF(YEAR, p.dob, CURDATE()) AS age,
       t.team_name
FROM players p
JOIN users u ON p.user_id = u.user_id
LEFT JOIN player_team pt ON p.player_id = pt.player_id AND pt.is_current = TRUE
LEFT JOIN teams t ON pt.team_id = t.team_id;

CREATE VIEW v_team_players AS
SELECT t.team_id, t.team_name, t.category, u.full_name AS player_name, p.player_role, p.status
FROM teams t
JOIN player_team pt ON t.team_id = pt.team_id AND pt.is_current = TRUE
JOIN players p ON pt.player_id = p.player_id
JOIN users u ON p.user_id = u.user_id;

CREATE VIEW v_training_attendance_summary AS
SELECT ts.session_id, ts.title, ts.session_date, t.team_name,
       SUM(CASE WHEN ta.status='Present' THEN 1 ELSE 0 END) AS present_count,
       SUM(CASE WHEN ta.status='Absent' THEN 1 ELSE 0 END) AS absent_count,
       SUM(CASE WHEN ta.status='Late' THEN 1 ELSE 0 END) AS late_count
FROM training_sessions ts
JOIN teams t ON ts.team_id = t.team_id
LEFT JOIN training_attendance ta ON ts.session_id = ta.session_id
GROUP BY ts.session_id, ts.title, ts.session_date, t.team_name;

CREATE VIEW v_player_fitness_latest AS
SELECT fr.*
FROM fitness_records fr
JOIN (
    SELECT player_id, MAX(record_date) AS latest_date
    FROM fitness_records
    GROUP BY player_id
) x ON fr.player_id = x.player_id AND fr.record_date = x.latest_date;

CREATE VIEW v_match_full_performance AS
SELECT m.match_id, m.match_date, m.opponent_name, u.full_name AS player_name,
       COALESCE(bp.runs, 0) AS runs, COALESCE(bp.balls, 0) AS balls,
       COALESCE(bp.strike_rate, 0) AS strike_rate,
       COALESCE(bop.overs, 0) AS overs, COALESCE(bop.wickets, 0) AS wickets,
       COALESCE(bop.economy, 0) AS economy,
       COALESCE(fp.catches, 0) AS catches
FROM matches m
JOIN match_squad ms ON m.match_id = ms.match_id
JOIN players p ON ms.player_id = p.player_id
JOIN users u ON p.user_id = u.user_id
LEFT JOIN batting_performance bp ON m.match_id = bp.match_id AND p.player_id = bp.player_id
LEFT JOIN bowling_performance bop ON m.match_id = bop.match_id AND p.player_id = bop.player_id
LEFT JOIN fielding_performance fp ON m.match_id = fp.match_id AND p.player_id = fp.player_id;

CREATE VIEW v_top_batsmen AS
SELECT u.full_name, pcs.matches_played, pcs.total_runs
FROM player_career_stats pcs
JOIN players p ON pcs.player_id = p.player_id
JOIN users u ON p.user_id = u.user_id
ORDER BY pcs.total_runs DESC;

CREATE VIEW v_top_bowlers AS
SELECT u.full_name, pcs.matches_played, pcs.total_wickets
FROM player_career_stats pcs
JOIN players p ON pcs.player_id = p.player_id
JOIN users u ON p.user_id = u.user_id
ORDER BY pcs.total_wickets DESC;

-- -----------------------------
-- Stored Procedures with Transactions
-- -----------------------------
DELIMITER $$

CREATE PROCEDURE sp_add_player(
    IN p_username VARCHAR(50),
    IN p_password_hash VARCHAR(128),
    IN p_full_name VARCHAR(100),
    IN p_email VARCHAR(100),
    IN p_phone VARCHAR(20),
    IN p_registration_no VARCHAR(30),
    IN p_dob DATE,
    IN p_gender ENUM('Male','Female'),
    IN p_batting_style ENUM('Right Hand','Left Hand'),
    IN p_bowling_style ENUM('Right Arm Fast','Left Arm Fast','Right Arm Spin','Left Arm Spin','None'),
    IN p_player_role ENUM('Batsman','Bowler','All Rounder','Wicket Keeper'),
    IN p_dominant_hand ENUM('Right','Left'),
    IN p_joining_date DATE,
    IN p_team_id INT
)
BEGIN
    DECLARE new_user_id INT;
    DECLARE new_player_id INT;
    DECLARE player_role_id INT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    SELECT role_id INTO player_role_id FROM roles WHERE role_name = 'Player';

    INSERT INTO users(role_id, username, password_hash, full_name, email, phone)
    VALUES(player_role_id, p_username, p_password_hash, p_full_name, p_email, p_phone);

    SET new_user_id = LAST_INSERT_ID();

    INSERT INTO players(user_id, registration_no, dob, gender, batting_style, bowling_style,
                        player_role, dominant_hand, joining_date)
    VALUES(new_user_id, p_registration_no, p_dob, p_gender, p_batting_style, p_bowling_style,
           p_player_role, p_dominant_hand, p_joining_date);

    SET new_player_id = LAST_INSERT_ID();

    INSERT INTO player_team(player_id, team_id, start_date, is_current)
    VALUES(new_player_id, p_team_id, p_joining_date, TRUE);

    COMMIT;
END $$

CREATE PROCEDURE sp_assign_player_to_team(
    IN p_player_id INT,
    IN p_new_team_id INT,
    IN p_start_date DATE
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    UPDATE player_team
    SET is_current = FALSE, end_date = DATE_SUB(p_start_date, INTERVAL 1 DAY)
    WHERE player_id = p_player_id AND is_current = TRUE;

    INSERT INTO player_team(player_id, team_id, start_date, is_current)
    VALUES(p_player_id, p_new_team_id, p_start_date, TRUE);

    COMMIT;
END $$

CREATE PROCEDURE sp_record_match_performance(
    IN p_match_id INT,
    IN p_player_id INT,
    IN p_runs INT,
    IN p_balls INT,
    IN p_fours INT,
    IN p_sixes INT,
    IN p_overs DECIMAL(4,1),
    IN p_maidens INT,
    IN p_runs_given INT,
    IN p_wickets INT,
    IN p_catches INT,
    IN p_runouts INT,
    IN p_stumpings INT
)
BEGIN
    DECLARE v_strike_rate DECIMAL(6,2);
    DECLARE v_economy DECIMAL(6,2);

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    SET v_strike_rate = CASE WHEN p_balls = 0 THEN 0 ELSE (p_runs / p_balls) * 100 END;
    SET v_economy = CASE WHEN p_overs = 0 THEN 0 ELSE p_runs_given / p_overs END;

    INSERT INTO batting_performance(match_id, player_id, runs, balls, fours, sixes, out_status, strike_rate)
    VALUES(p_match_id, p_player_id, p_runs, p_balls, p_fours, p_sixes, 'Out', v_strike_rate)
    ON DUPLICATE KEY UPDATE runs=p_runs, balls=p_balls, fours=p_fours, sixes=p_sixes, strike_rate=v_strike_rate;

    INSERT INTO bowling_performance(match_id, player_id, overs, maidens, runs_given, wickets, economy)
    VALUES(p_match_id, p_player_id, p_overs, p_maidens, p_runs_given, p_wickets, v_economy)
    ON DUPLICATE KEY UPDATE overs=p_overs, maidens=p_maidens, runs_given=p_runs_given, wickets=p_wickets, economy=v_economy;

    INSERT INTO fielding_performance(match_id, player_id, catches, runouts, stumpings)
    VALUES(p_match_id, p_player_id, p_catches, p_runouts, p_stumpings)
    ON DUPLICATE KEY UPDATE catches=p_catches, runouts=p_runouts, stumpings=p_stumpings;

    COMMIT;
END $$

CREATE PROCEDURE sp_get_player_summary(IN p_player_id INT)
BEGIN
    SELECT * FROM v_player_profile WHERE player_id = p_player_id;
    SELECT * FROM fitness_records WHERE player_id = p_player_id ORDER BY record_date DESC;
    SELECT * FROM v_match_full_performance WHERE player_name IN (
        SELECT u.full_name FROM players p JOIN users u ON p.user_id = u.user_id WHERE p.player_id = p_player_id
    );
END $$

DELIMITER ;

-- -----------------------------
-- Triggers
-- -----------------------------
DELIMITER $$

CREATE TRIGGER trg_player_after_insert
AFTER INSERT ON players
FOR EACH ROW
BEGIN
    INSERT INTO player_career_stats(player_id) VALUES(NEW.player_id);
END $$

CREATE TRIGGER trg_batting_after_insert
AFTER INSERT ON batting_performance
FOR EACH ROW
BEGIN
    UPDATE player_career_stats
    SET total_runs = total_runs + NEW.runs,
        matches_played = matches_played + 1
    WHERE player_id = NEW.player_id;
END $$

CREATE TRIGGER trg_bowling_after_insert
AFTER INSERT ON bowling_performance
FOR EACH ROW
BEGIN
    UPDATE player_career_stats
    SET total_wickets = total_wickets + NEW.wickets
    WHERE player_id = NEW.player_id;
END $$

CREATE TRIGGER trg_fielding_after_insert
AFTER INSERT ON fielding_performance
FOR EACH ROW
BEGIN
    UPDATE player_career_stats
    SET total_catches = total_catches + NEW.catches
    WHERE player_id = NEW.player_id;
END $$

DELIMITER ;

-- -----------------------------
-- Sample Data
-- -----------------------------
INSERT INTO roles(role_name) VALUES ('Admin'), ('Coach'), ('Player');

INSERT INTO users(role_id, username, password_hash, full_name, email, phone)
VALUES
(1, 'admin', SHA2('admin123', 256), 'System Admin', 'admin@cricket.local', '03000000000'),
(2, 'coach1', SHA2('coach123', 256), 'Coach Ahmed', 'coach@cricket.local', '03001111111'),
(3, 'player1', SHA2('player123', 256), 'Ali Khan', 'ali@cricket.local', '03002222222');

INSERT INTO coaches(user_id, specialization, experience_years, joined_date)
VALUES(2, 'Batting Coach', 6, '2024-01-01');

INSERT INTO players(user_id, registration_no, dob, gender, batting_style, bowling_style, player_role, dominant_hand, joining_date)
VALUES(3, 'PLR-001', '2005-05-10', 'Male', 'Right Hand', 'Right Arm Fast', 'All Rounder', 'Right', '2025-01-15');

INSERT INTO teams(team_name, category, city) VALUES('UET Tigers', 'University', 'Lahore');

INSERT INTO coach_team(coach_id, team_id, assigned_date) VALUES(1, 1, '2025-01-15');
INSERT INTO player_team(player_id, team_id, start_date) VALUES(1, 1, '2025-01-15');

INSERT INTO tournaments(tournament_name, location, start_date, end_date)
VALUES('Inter University Cricket Cup', 'Lahore', '2026-02-01', '2026-02-20');

INSERT INTO matches(tournament_id, team_id, opponent_name, match_date, venue, match_type, result)
VALUES(1, 1, 'PU Eagles', '2026-02-05', 'UET Ground', 'T20', 'Won');

INSERT INTO match_squad(match_id, player_id, selected_by_coach_id, selection_status, batting_order)
VALUES(1, 1, 1, 'Selected', 3);

INSERT INTO training_sessions(coach_id, team_id, session_date, title, focus_area, start_time, end_time, location, notes)
VALUES(1, 1, '2026-01-20', 'Morning Batting Practice', 'Batting', '08:00:00', '10:00:00', 'UET Ground', 'Focus on cover drive and strike rotation');

INSERT INTO training_attendance(session_id, player_id, status, remarks)
VALUES(1, 1, 'Present', 'Good effort');

INSERT INTO fitness_records(player_id, coach_id, record_date, height_cm, weight_kg, bmi, stamina_score, speed_score, strength_score, overall_score, remarks)
VALUES(1, 1, '2026-01-21', 178, 68, 21.46, 80, 78, 75, 78, 'Good fitness level');

CALL sp_record_match_performance(1, 1, 45, 30, 5, 2, 3.0, 0, 22, 2, 1, 0, 0);

INSERT INTO coach_feedback(coach_id, player_id, feedback_date, rating, remarks, action_plan)
VALUES(1, 1, '2026-01-25', 8, 'Strong batting potential', 'Improve yorker defense and running between wickets');
