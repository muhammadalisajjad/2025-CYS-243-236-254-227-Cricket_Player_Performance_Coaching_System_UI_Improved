namespace CricketPlayerManagementSystem.Domain;

public class FitnessRecord
{
    public int FitnessId { get; set; }
    public int PlayerId { get; set; }
    public int CoachId { get; set; }
    public DateTime RecordDate { get; set; }
    public decimal HeightCm { get; set; }
    public decimal WeightKg { get; set; }
    public decimal Bmi { get; set; }
    public int StaminaScore { get; set; }
    public int SpeedScore { get; set; }
    public int StrengthScore { get; set; }
    public int OverallScore { get; set; }
    public string? Remarks { get; set; }
}
