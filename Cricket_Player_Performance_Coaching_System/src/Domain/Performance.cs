namespace CricketPlayerManagementSystem.Domain;

public class BattingPerformance
{
    public int MatchId { get; set; }
    public int PlayerId { get; set; }
    public int Runs { get; set; }
    public int Balls { get; set; }
    public int Fours { get; set; }
    public int Sixes { get; set; }
}

public class BowlingPerformance
{
    public int MatchId { get; set; }
    public int PlayerId { get; set; }
    public decimal Overs { get; set; }
    public int Maidens { get; set; }
    public int RunsGiven { get; set; }
    public int Wickets { get; set; }
}

public class FieldingPerformance
{
    public int MatchId { get; set; }
    public int PlayerId { get; set; }
    public int Catches { get; set; }
    public int Runouts { get; set; }
    public int Stumpings { get; set; }
}
