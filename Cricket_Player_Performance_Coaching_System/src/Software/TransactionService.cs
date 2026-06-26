using MySql.Data.MySqlClient;

namespace CricketPlayerManagementSystem.Software;

public static class TransactionService
{
    // Transaction Example 1: add user, player, and player-team assignment together.
    public static void AddPlayerWithUserAndTeam(string username, string password, string fullName, string email,
        string phone, string regNo, DateTime dob, string gender, string battingStyle, string bowlingStyle,
        string playerRole, string dominantHand, DateTime joiningDate, int teamId)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            int roleId;
            using (var roleCmd = new MySqlCommand("SELECT role_id FROM roles WHERE role_name='Player'", connection, transaction))
                roleId = Convert.ToInt32(roleCmd.ExecuteScalar());

            var userCmd = new MySqlCommand(@"
                INSERT INTO users(role_id, username, password_hash, full_name, email, phone)
                VALUES(@role_id, @username, @password_hash, @full_name, @email, @phone);
                SELECT LAST_INSERT_ID();", connection, transaction);
            userCmd.Parameters.AddWithValue("@role_id", roleId);
            userCmd.Parameters.AddWithValue("@username", username);
            userCmd.Parameters.AddWithValue("@password_hash", AuthService.Sha256(password));
            userCmd.Parameters.AddWithValue("@full_name", fullName);
            userCmd.Parameters.AddWithValue("@email", email);
            userCmd.Parameters.AddWithValue("@phone", phone);
            int userId = Convert.ToInt32(userCmd.ExecuteScalar());

            var playerCmd = new MySqlCommand(@"
                INSERT INTO players(user_id, registration_no, dob, gender, batting_style, bowling_style,
                                    player_role, dominant_hand, joining_date)
                VALUES(@user_id, @reg, @dob, @gender, @bat, @bowl, @role, @hand, @join);
                SELECT LAST_INSERT_ID();", connection, transaction);
            playerCmd.Parameters.AddWithValue("@user_id", userId);
            playerCmd.Parameters.AddWithValue("@reg", regNo);
            playerCmd.Parameters.AddWithValue("@dob", dob);
            playerCmd.Parameters.AddWithValue("@gender", gender);
            playerCmd.Parameters.AddWithValue("@bat", battingStyle);
            playerCmd.Parameters.AddWithValue("@bowl", bowlingStyle);
            playerCmd.Parameters.AddWithValue("@role", playerRole);
            playerCmd.Parameters.AddWithValue("@hand", dominantHand);
            playerCmd.Parameters.AddWithValue("@join", joiningDate);
            int playerId = Convert.ToInt32(playerCmd.ExecuteScalar());

            var teamCmd = new MySqlCommand(@"
                INSERT INTO player_team(player_id, team_id, start_date, is_current)
                VALUES(@player_id, @team_id, @start, TRUE)", connection, transaction);
            teamCmd.Parameters.AddWithValue("@player_id", playerId);
            teamCmd.Parameters.AddWithValue("@team_id", teamId);
            teamCmd.Parameters.AddWithValue("@start", joiningDate);
            teamCmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // Transaction Example 2: close old team assignment and insert new team assignment.
    public static void AssignPlayerToNewTeam(int playerId, int newTeamId, DateTime startDate)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            var closeCmd = new MySqlCommand(@"
                UPDATE player_team
                SET is_current=FALSE, end_date=@end_date
                WHERE player_id=@player_id AND is_current=TRUE", connection, transaction);
            closeCmd.Parameters.AddWithValue("@end_date", startDate.AddDays(-1));
            closeCmd.Parameters.AddWithValue("@player_id", playerId);
            closeCmd.ExecuteNonQuery();

            var insertCmd = new MySqlCommand(@"
                INSERT INTO player_team(player_id, team_id, start_date, is_current)
                VALUES(@player_id, @team_id, @start_date, TRUE)", connection, transaction);
            insertCmd.Parameters.AddWithValue("@player_id", playerId);
            insertCmd.Parameters.AddWithValue("@team_id", newTeamId);
            insertCmd.Parameters.AddWithValue("@start_date", startDate);
            insertCmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // Transaction Example 3: save batting, bowling, and fielding performance together.
    public static void SaveMatchPerformance(int matchId, int playerId, int runs, int balls, int fours, int sixes,
        decimal overs, int maidens, int runsGiven, int wickets, int catches, int runouts, int stumpings)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            decimal strikeRate = balls == 0 ? 0 : Math.Round((decimal)runs / balls * 100, 2);
            decimal economy = overs == 0 ? 0 : Math.Round(runsGiven / overs, 2);

            var batCmd = new MySqlCommand(@"
                INSERT INTO batting_performance(match_id, player_id, runs, balls, fours, sixes, out_status, strike_rate)
                VALUES(@match, @player, @runs, @balls, @fours, @sixes, 'Out', @sr)", connection, transaction);
            batCmd.Parameters.AddWithValue("@match", matchId);
            batCmd.Parameters.AddWithValue("@player", playerId);
            batCmd.Parameters.AddWithValue("@runs", runs);
            batCmd.Parameters.AddWithValue("@balls", balls);
            batCmd.Parameters.AddWithValue("@fours", fours);
            batCmd.Parameters.AddWithValue("@sixes", sixes);
            batCmd.Parameters.AddWithValue("@sr", strikeRate);
            batCmd.ExecuteNonQuery();

            var bowlCmd = new MySqlCommand(@"
                INSERT INTO bowling_performance(match_id, player_id, overs, maidens, runs_given, wickets, economy)
                VALUES(@match, @player, @overs, @maidens, @runs_given, @wickets, @economy)", connection, transaction);
            bowlCmd.Parameters.AddWithValue("@match", matchId);
            bowlCmd.Parameters.AddWithValue("@player", playerId);
            bowlCmd.Parameters.AddWithValue("@overs", overs);
            bowlCmd.Parameters.AddWithValue("@maidens", maidens);
            bowlCmd.Parameters.AddWithValue("@runs_given", runsGiven);
            bowlCmd.Parameters.AddWithValue("@wickets", wickets);
            bowlCmd.Parameters.AddWithValue("@economy", economy);
            bowlCmd.ExecuteNonQuery();

            var fieldCmd = new MySqlCommand(@"
                INSERT INTO fielding_performance(match_id, player_id, catches, runouts, stumpings)
                VALUES(@match, @player, @catches, @runouts, @stumpings)", connection, transaction);
            fieldCmd.Parameters.AddWithValue("@match", matchId);
            fieldCmd.Parameters.AddWithValue("@player", playerId);
            fieldCmd.Parameters.AddWithValue("@catches", catches);
            fieldCmd.Parameters.AddWithValue("@runouts", runouts);
            fieldCmd.Parameters.AddWithValue("@stumpings", stumpings);
            fieldCmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
