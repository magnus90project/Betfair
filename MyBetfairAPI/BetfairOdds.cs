using Betfair_API_NG.TO;
using System.Collections.Generic;
using System.Web.Caching;

namespace MyBetfairAPI
{
    public class BetfairOdds
    {
        public string RunnerName { get; set; }
        public List<RunnerOdds> RunnerBackOdds { get; set; } = new List<RunnerOdds>();
        public List<RunnerOdds> RunnerLayOdds { get; set; } = new List<RunnerOdds>();
    }
}