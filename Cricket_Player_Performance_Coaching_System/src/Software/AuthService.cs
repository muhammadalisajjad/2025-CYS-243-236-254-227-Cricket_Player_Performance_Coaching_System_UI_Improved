using System.Data;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace CricketPlayerManagementSystem.Software;

public static class AuthService
{
    public static bool Login(string username, string password)
    {
        string hash = Sha256(password);
        string query = @"
            SELECT u.user_id, u.username, u.full_name, r.role_name
            FROM users u
            JOIN roles r ON u.role_id = r.role_id
            WHERE u.username = @username AND u.password_hash = @password_hash AND u.is_active = TRUE";

        DataTable table = DbHelper.ExecuteDataTable(query,
            new MySqlParameter("@username", username),
            new MySqlParameter("@password_hash", hash));

        if (table.Rows.Count == 0)
            return false;

        DataRow row = table.Rows[0];
        SessionManager.Start(
            Convert.ToInt32(row["user_id"]),
            row["username"].ToString() ?? string.Empty,
            row["role_name"].ToString() ?? string.Empty,
            row["full_name"].ToString() ?? string.Empty);
        return true;
    }

    public static string Sha256(string value)
    {
        using SHA256 sha = SHA256.Create();
        byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}
