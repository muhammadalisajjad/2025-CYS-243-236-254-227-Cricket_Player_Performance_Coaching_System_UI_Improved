namespace CricketPlayerManagementSystem.Software;

public static class SessionManager
{
    public static int CurrentUserId { get; private set; }
    public static string CurrentUsername { get; private set; } = string.Empty;
    public static string CurrentRole { get; private set; } = string.Empty;
    public static string CurrentFullName { get; private set; } = string.Empty;

    public static void Start(int userId, string username, string role, string fullName)
    {
        CurrentUserId = userId;
        CurrentUsername = username;
        CurrentRole = role;
        CurrentFullName = fullName;
    }

    public static void Clear()
    {
        CurrentUserId = 0;
        CurrentUsername = string.Empty;
        CurrentRole = string.Empty;
        CurrentFullName = string.Empty;
    }
}
