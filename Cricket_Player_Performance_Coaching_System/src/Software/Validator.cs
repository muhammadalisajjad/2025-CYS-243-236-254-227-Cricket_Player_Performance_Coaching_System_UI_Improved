using System.Text.RegularExpressions;

namespace CricketPlayerManagementSystem.Software;

public static class Validator
{
    public static bool Required(string value) => !string.IsNullOrWhiteSpace(value);

    public static bool Email(string value)
    {
        return Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public static bool Phone(string value)
    {
        return Regex.IsMatch(value, @"^[0-9+\- ]{10,20}$");
    }

    public static bool Score(int value) => value >= 0 && value <= 100;

    public static bool Positive(decimal value) => value >= 0;

    public static bool DateNotFuture(DateTime date) => date.Date <= DateTime.Today;

    public static void Require(bool condition, string message)
    {
        if (!condition)
            throw new ArgumentException(message);
    }
}
