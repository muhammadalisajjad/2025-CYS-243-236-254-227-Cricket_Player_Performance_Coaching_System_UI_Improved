using MySql.Data.MySqlClient;

namespace CricketPlayerManagementSystem.Software;

public static class LoggerService
{
    public static void LogError(Exception ex, string formName)
    {
        try
        {
            DbHelper.ExecuteNonQuery(
                "INSERT INTO error_logs(user_id, error_message, stack_trace, form_name) VALUES(@user_id, @msg, @stack, @form)",
                new MySqlParameter("@user_id", SessionManager.CurrentUserId == 0 ? (object)DBNull.Value : SessionManager.CurrentUserId),
                new MySqlParameter("@msg", ex.Message),
                new MySqlParameter("@stack", ex.ToString()),
                new MySqlParameter("@form", formName));
        }
        catch
        {
            // Avoid recursive logging failure.
        }
    }
}
