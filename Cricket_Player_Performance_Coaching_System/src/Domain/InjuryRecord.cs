namespace CricketPlayerManagementSystem.Domain;

public class InjuryRecord
{
    public int InjuryId { get; set; }
    public int PlayerId { get; set; }
    public string InjuryType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime InjuryDate { get; set; }
    public DateTime? ExpectedRecoveryDate { get; set; }
    public string Status { get; set; } = "Open";
    public string? DoctorNote { get; set; }
}
