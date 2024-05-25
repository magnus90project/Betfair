using System;
using MyBetfairAPI;

namespace Oddsportal
{
     public class OddsportalMatch : IMatch
    {
        public string Name { get; set; }
        public string Sport { get; set; }
        public string Country { get; set; }
        public string Competition { get; set; }
        public DateTime StartTime { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public MatchOdds Odds { get; set; }
        public BetfairMatch BetfairMatch { get; set; } 
    }




}
