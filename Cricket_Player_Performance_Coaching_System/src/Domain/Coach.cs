namespace CricketPlayerManagementSystem.Domain;

public class Coach
{
    public int CoachId { get; set; }
    public int UserId { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public DateTime JoinedDate { get; set; }
}
