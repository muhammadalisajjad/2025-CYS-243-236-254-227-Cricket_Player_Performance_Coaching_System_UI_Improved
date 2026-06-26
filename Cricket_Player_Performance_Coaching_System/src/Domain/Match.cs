namespace CricketPlayerManagementSystem.Domain;

public class Match
{
    public int MatchId { get; set; }
    public int? TournamentId { get; set; }
    public int TeamId { get; set; }
    public string OpponentName { get; set; } = string.Empty;
    public DateTime MatchDate { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string MatchType { get; set; } = string.Empty;
    public string Result { get; set; } = "Upcoming";
}
