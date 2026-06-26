namespace CricketPlayerManagementSystem.Domain;

public class CoachFeedback
{
    public int FeedbackId { get; set; }
    public int CoachId { get; set; }
    public int PlayerId { get; set; }
    public DateTime FeedbackDate { get; set; }
    public int Rating { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public string? ActionPlan { get; set; }
}
