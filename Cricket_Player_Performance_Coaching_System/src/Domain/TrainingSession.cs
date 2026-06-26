namespace CricketPlayerManagementSystem.Domain;

public class TrainingSession
{
    public int SessionId { get; set; }
    public int CoachId { get; set; }
    public int TeamId { get; set; }
    public DateTime SessionDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FocusArea { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
