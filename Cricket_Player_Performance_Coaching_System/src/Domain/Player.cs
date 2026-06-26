namespace CricketPlayerManagementSystem.Domain;

public class Player
{
    public int PlayerId { get; set; }
    public int UserId { get; set; }
    public string RegistrationNo { get; set; } = string.Empty;
    public DateTime Dob { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BattingStyle { get; set; } = string.Empty;
    public string BowlingStyle { get; set; } = string.Empty;
    public string PlayerRole { get; set; } = string.Empty;
    public string DominantHand { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public DateTime JoiningDate { get; set; }
    public string Status { get; set; } = "Active";
}
