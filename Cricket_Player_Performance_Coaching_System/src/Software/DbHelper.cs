using System.Data;
using MySql.Data.MySqlClient;

namespace CricketPlayerManagementSystem.Software;

public static class DbHelper
{
    // Change password if your MySQL root password is different.
    public static string ConnectionString { get; set; } =
        "server=localhost;user id=root;password=12345;database=cricket_player_management;Allow User Variables=True;";

    public static MySqlConnection GetConnection()
    {
        return new MySqlConnection(ConnectionString);
    }

    public static DataTable ExecuteDataTable(string query, params MySqlParameter[] parameters)
    {
        using var connection = GetConnection();
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddRange(parameters);
        using var adapter = new MySqlDataAdapter(command);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public static int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
    {
        using var connection = GetConnection();
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddRange(parameters);
        connection.Open();
        return command.ExecuteNonQuery();
    }

    public static object? ExecuteScalar(string query, params MySqlParameter[] parameters)
    {
        using var connection = GetConnection();
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddRange(parameters);
        connection.Open();
        return command.ExecuteScalar();
    }
}
