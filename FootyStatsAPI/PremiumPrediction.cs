using System;
using System.Collections.Generic;
using System.Text;

namespace FootyStatsAPI
{
    public class PremiumPrediction
    {
        public int fixture_id { get; set; }
        public string league_name { get; set; }
        public string home_team_name { get; set; }
        public string away_team_name { get; set; }
        public string prediction { get; set; }
        public double prediction_confidence { get; set; }
        public int home_wins_last_5 { get; set; }
        public int away_wins_last_5 { get; set; }
        public int draws_last_5 { get; set; }
        public int home_losses_last_5 { get; set; }
        public int away_losses_last_5 { get; set; }
    }
}
