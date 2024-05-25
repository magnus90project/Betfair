public class PredictionResult
{
    public int MatchId { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public double HomeTeamShots { get; set; }
    public double AwayTeamShots { get; set; }
    public double HomeTeamShotsOnTarget { get; set; }
    public double AwayTeamShotsOnTarget { get; set; }
    public double HomeTeamGoals { get; set; }
    public double AwayTeamGoals { get; set; }
    public double HomeTeamCorners { get; set; }
    public double AwayTeamCorners { get; set; }
    public double HomeTeamYellowCards { get; set; }
    public double AwayTeamYellowCards { get; set; }
    public double HomeTeamRedCards { get; set; }
    public double AwayTeamRedCards { get; set; }
    public string Prediction { get; set; }
}